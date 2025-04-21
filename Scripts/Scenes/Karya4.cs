// File: Scripts/Scenes/Karya4.cs
// Scene: 050_Motif2D_Animasi_dan_Interaksi (Berwarna)
//
// – Empat motif berwarna (kelas warna dari Karya 3)
// – Interaksi real‑time: kecepatan, zoom, pan, ganti motif
// – Tidak memerlukan tombol UI, tetapi fungsi _on_Btn… tetap disediakan
// --------------------------------------------------------------------

using Godot;
using System;
using System.Collections.Generic;
using Godot.Core;
using Godot.Utils;         

public partial class Karya4 : Node2D
{
	// motif nodes
	private BatikKawungColor    _batik;
	private StupaBorobudurColor _stupa;
	private GongSebulColor      _gong;
	private GambangColor        _gambang;

	// base data
	private Vector2 _batikPos0,   _stupaPos0,   _gongPos0,   _gambangPos0;
	private Vector2 _batikScale0, _stupaScale0, _gongScale0, _gambangScale0;
	private float   _batikRot0,   _stupaRot0;
	private readonly Transformasi _tf = new Transformasi();

	// interaction
	private float   _time         = 0f;
	private float   _speedFactor  = 1f;          // +/‑  mempercepat / perlambat
	private float   _globalScale  = 1f;          // wheel zoom
	private Vector2 _globalOffset = Vector2.Zero; // pan
	private bool    _dragging     = false;
	private Vector2 _dragStart;

	// tambahan untuk gambang sound + auto‑mallet:
	private bool              _autoMallet      = true;   
	private AudioStreamPlayer _audioPlayer;             
	private AudioStream[]     _barSounds;              
	
	// UI labels
	private Label _infoLabel;    
	private Label _motifLabel;   
	private Timer _resetTimer;
	
	// constants
	private const float SPEED_STEP = 1.2f;
	private const float SCALE_STEP = 1.1f;
	private const float MAX_SCALE  = 3f;
	private const float MIN_SCALE  = 0.3f;

	// Gong lintasan kotak
	private readonly Vector2 _c1 = new Vector2(0, 100);
	private readonly Vector2 _c2 = new Vector2(400, 100);
	private readonly Vector2 _c3 = new Vector2(400, 250);
	private readonly Vector2 _c4 = new Vector2(0, 250);
	private const float GONG_CYCLE = 8f; // detik
	
	private PanelContainer _helpPanel;
	
	// Ready
	public override void _Ready()
	{
		ScreenUtils.Initialize(GetViewport());

		_batik   = new BatikKawungColor();
		_stupa   = new StupaBorobudurColor();
		_gong    = new GongSebulColor();
		_gambang = new GambangColor();

		AddChild(_batik);
		AddChild(_stupa);
		AddChild(_gong);
		AddChild(_gambang);

		float cx = (ScreenUtils.MarginLeft + ScreenUtils.MarginRight) / 2f;
		float cy = (ScreenUtils.MarginTop  + ScreenUtils.MarginBottom) / 2f;

		// Batik
		_batik.Scale    = new Vector2(0.7f, 0.7f);
		_batik.Position = new Vector2(cx - 140, cy - 140);
		_batikPos0      = _batik.Position;
		_batikScale0    = _batik.Scale;
		_batikRot0      = 0;

		// Stupa
		_stupa.Scale    = new Vector2(0.4f, 0.4f);
		_stupa.Position = new Vector2(cx - 60, cy - 60);
		_stupaPos0      = _stupa.Position;
		_stupaScale0    = _stupa.Scale;
		_stupaRot0      = 0;

		// Gong
		_gong.Scale   = new Vector2(0.5f, 0.5f);
		_gong.Position = _c1;
		_gongPos0      = _gong.Position;
		_gongScale0    = _gong.Scale;

		// Gambang
		_gambang.Scale    = new Vector2(0.6f, 0.6f);
		_gambang.Position = new Vector2(500, 100);
		_gambangPos0      = _gambang.Position;
		_gambangScale0    = _gambang.Scale;
		
		_helpPanel = GetNode<PanelContainer>("HelpPanel");
		_helpPanel.Visible = false;          // start hidden
		
		// load suara tiap bilah gambang
		_barSounds    = new AudioStream[7];
		for (int i = 0; i < 7; i++)
			_barSounds[i] = GD.Load<AudioStream>($"res://Assets/Audio/bilah{i+1}.mp3");
		_audioPlayer  = new AudioStreamPlayer();
		AddChild(_audioPlayer);
		
		// UI: InfoLabel
		_infoLabel = new Label();
		_infoLabel.Visible = false;
		AddChild(_infoLabel);

		// UI: MotifLabel
		_motifLabel = new Label();
		_motifLabel.Position = new Vector2(300, 20);
		_motifLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		_motifLabel.HorizontalAlignment = HorizontalAlignment.Center;
		AddChild(_motifLabel);

		// Timer untuk fade Reset
		_resetTimer = new Timer();
		_resetTimer.WaitTime = 2f;
		_resetTimer.OneShot = true;
		_resetTimer.Timeout += OnResetTimeout;
		AddChild(_resetTimer);
		
		ShowMotif("batik"); // default
	}
	
	// handle Reset timeout: sembunyikan InfoLabel jika pesan Reset
	private void OnResetTimeout()
	{
		if (_infoLabel.Text == "Reset Kecepatan dan Posisi")
			_infoLabel.Visible = false;
	}

	private void _on_BtnHelp_pressed()       
	{
		_helpPanel.Visible = !_helpPanel.Visible;   
	}
	
	// INPUT
	// Pusat kendali interaksi.
	// - Keyboard: +/-/=/num‑pad + – mengubah _speedFactor; R mereset; 1–4 ganti motif.
	// - Mouse: wheel zoom (_globalScale); drag LMB pan (_globalOffset).
	public override void _Input(InputEvent e)
	{
		// keyboard
		if (e is InputEventKey k && k.Pressed)
		{
			switch (k.Keycode)
			{
				case Key.Equal:                               // baris atas '=' atau '+'
					if (k.ShiftPressed) _speedFactor *= SPEED_STEP;   // '+'
					else                _speedFactor /= SPEED_STEP;   // '='
					break;

				case Key.KpAdd:     _speedFactor *= SPEED_STEP; break; // num‑pad +
				case Key.Minus:
				case Key.KpSubtract: _speedFactor /= SPEED_STEP; break;

				case Key.R:         // reset
					_speedFactor  = 1f;
					_globalScale  = 1f;
					_globalOffset = Vector2.Zero;
					_infoLabel.Position = new Vector2(55,  50);
					_infoLabel.Text = "Reset Kecepatan dan Posisi";
					_infoLabel.Visible = true;
					_resetTimer.Start(); // fade out nanti
					break;
					
				case Key.P:                           
					_autoMallet = !_autoMallet;
					if (_gambang.Visible)
					{
						_infoLabel.Position = new Vector2(55,  400);
						_infoLabel.Text = _autoMallet
							? "Mode normal"
							: "Mode bermain bilah gambang";
						_infoLabel.Visible = true;
					}
					break;
					
				case Key.Key1: ShowMotif("batik");   break;
				case Key.Key2: ShowMotif("stupa");   break;
				case Key.Key3: ShowMotif("gong");    break;
				case Key.Key4: ShowMotif("gambang"); break;
			}
		}

		// mouse
		if (e is InputEventMouseButton mb && mb.Pressed)
		{
			if (mb.ButtonIndex == MouseButton.WheelUp)
				_globalScale = Mathf.Clamp(_globalScale * SCALE_STEP, MIN_SCALE, MAX_SCALE);
			if (mb.ButtonIndex == MouseButton.WheelDown)
				_globalScale = Mathf.Clamp(_globalScale / SCALE_STEP, MIN_SCALE, MAX_SCALE);

			if (mb.ButtonIndex == MouseButton.Left)
			{
				if (!_autoMallet && _gambang.Visible)
				{
					// ubah dari world ke local:
					Vector2 localClick = _gambang.ToLocal(mb.Position);

					// hitung bounds tiap bilah dalam local coords
					for (int i = 0; i < 7; i++)
					{
						float barHeight  = 20f;
						float bottomW    = 100f;
						float topW       = 60f;
						float gap        = 5f;
						float widthStep  = (bottomW - topW) / 6f;
						float xShift     = 2f;
						float startX     = 0f;
						float startY     = 0f;
						float w          = topW + i * widthStep;
						float x          = startX - i * xShift;
						float y          = startY + i * (barHeight + gap);
						var   rect       = new Rect2(x, y, w, barHeight);

						if (rect.HasPoint(localClick))
						{
							_audioPlayer.Stream = _barSounds[i];
							_audioPlayer.Play();
							break;
						}
					}

					return; // cegah pan
				}

				// pan biasa
				_dragging  = true;
				_dragStart = mb.Position;
			}
		}

		if (e is InputEventMouseButton mbUp && !mbUp.Pressed && mbUp.ButtonIndex == MouseButton.Left)
		{
			_dragging = false;
		}

		if (e is InputEventMouseMotion mm && _dragging)
			_globalOffset += mm.Relative;      // pan
	}

	// PROCESS
	// Dipanggil setiap frame.
	// - Menambah _time dengan delta * _speedFactor.
	// - Memanggil empat fungsi animasi: AnimateBatik(), AnimateStupa(), AnimateGong(), AnimateGambang().
	public override void _Process(double delta)
	{
		_time += (float)delta * _speedFactor;
		if (_time > 60f) _time -= 60f;         // hindari overflow

		AnimateBatik();
		AnimateStupa();
		AnimateGong();
		AnimateGambang();
	}

	// Animations
	// Batik Kawung:
	// - Translasi sin‑cos (melayang).
	// - Rotasi ± 5°.
	// - Pulsing scale ± 5 %.
	// - Semua hasil dikalikan _globalScale & ditambah _globalOffset.
	private void AnimateBatik()
	{
		if (!_batik.Visible) return;

		float amp = 10f;
		Vector2 offset = new Vector2(Mathf.Sin(_time), Mathf.Cos(_time * 0.5f)) * amp;

		float rot = Mathf.Sin(_time * 0.5f) * 5f;
		float scl = 1f + 0.05f * Mathf.Sin(_time * 2f);

		_batik.Position        = (_batikPos0 + offset) * _globalScale + _globalOffset;
		_batik.RotationDegrees = _batikRot0 + rot;
		_batik.Scale           = _batikScale0 * scl * _globalScale;
	}
	
	// Stupa Candi Borobudur
	// - Rotasi konstan 5 °/s.
	// - Getaran vertikal (sinus 8 px).
	// - Diterapkan dengan transform global.
	private void AnimateStupa()
	{
		if (!_stupa.Visible) return;

		float rot = _stupaRot0 + _time * 5f;
		float dy  = Mathf.Sin(_time * 2f) * 8f;

		_stupa.Position        = (_stupaPos0 + new Vector2(0, dy)) * _globalScale + _globalOffset;
		_stupa.RotationDegrees = rot;
		_stupa.Scale           = _stupaScale0 * _globalScale;
	}
	
	// Gong Sebul
	// - Menjalankan gong di lintasan kotak (c1→c2→c3→c4).
	// - Interpolasi linear (lerp) per sisi, total 8 detik.
	// - Shake ± 5 px & rotasi kecil.
	private void AnimateGong()
	{
		if (!_gong.Visible) return;

		float segLen = GONG_CYCLE / 4f;
		float mod    = _time % GONG_CYCLE;
		int   seg    = (int)(mod / segLen);
		float t      = (mod % segLen) / segLen;

		Vector2 from = _c1, to = _c2;
		switch (seg)
		{
			case 0: from = _c1; to = _c2; break;
			case 1: from = _c2; to = _c3; break;
			case 2: from = _c3; to = _c4; break;
			case 3: from = _c4; to = _c1; break;
		}
		Vector2 pos   = from.Lerp(to, t);
		float   shake = 5f * Mathf.Sin(_time * 10f);

		_gong.Position        = (pos + new Vector2(0, shake)) * _globalScale + _globalOffset;
		_gong.RotationDegrees = shake;
		_gong.Scale           = _gongScale0 * _globalScale;
	}
	
	// Alat Musik Gambang dan Pemukulnya
	// - Memutar gambang 90°.
	// - Menganimasikan dua pemukul: fase “mendekat” 0–2 s (lerp), lalu “memukul” 2–4 s (bounce sinus).
	// - Offset pemukul dikirim ke node GambangColor melalui properti MalletOffset.
	private void AnimateGambang()
	{
		if (!_gambang.Visible) return;

		// posisi global
		_gambang.Position        = _gambangPos0 * _globalScale + _globalOffset;
		_gambang.RotationDegrees = 90f;
		_gambang.Scale           = _gambangScale0 * _globalScale;

		// hanya bounce, tanpa auto‑approach
		float bounce = 10f * Mathf.Sin(_time * 5f);
		Vector2 baseOffset = new Vector2(0, -100);
		_gambang.MalletOffset = (baseOffset + new Vector2(0, bounce)) * _globalScale;
	}

	// Draw border
	public override void _Draw()
	{
		var margin = new bentukdasar().Margin();
		GraphicsUtils.PutPixelAll(this, margin, GraphicsUtils.DrawStyle.DotDot, Colors.Black);
	}

	private void ShowMotif(string id)
	{
		_batik.Visible   = id == "batik";
		_stupa.Visible   = id == "stupa";
		_gong.Visible    = id == "gong";
		_gambang.Visible = id == "gambang";
		
		switch (id)
		{
			case "batik":   _motifLabel.Text = "Batik Kawung";    break;
			case "stupa":   _motifLabel.Text = "Stupa Borobudur"; break;
			case "gong":    _motifLabel.Text = "Gong Sebul";      break;
			case "gambang": _motifLabel.Text = "Gambang";         break;
		}
	}
}
