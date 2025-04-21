// File: Scripts/Scenes/Karya2.cs
// Scene: 050_Motif2D_Animasi
//
// – Empat motif sama seperti Karya 1, masih monokrom.
// – Setiap motif kini memiliki animasi transformasi 2D komposit:
//     • Batik : translasi sin‑cos + rotasi + pulsating scale
//     • Stupa : rotasi perlahan + getaran vertikal
//     • Gong  : lintasan kotak 4 sisi + shake/rotasi kecil
//     • Gambang: rotasi 90° & animasi dua pemukul
// – Belum ada interaksi; animasi berjalan otomatis.
// --------------------------------------------------------------------

using Godot;
using System;
using Godot.Utils;
using Godot.Core;

public partial class Karya2 : Node2D
{
	private bentukdasar _bentukDasar;

	// Motif
	private batikKawung    _batikKawung;
	private stupaBorobudur _stupaBorobudur;
	private gongSebul      _gongSebul;
	private gambang        _gambang;

	// Variabel waktu untuk animasi
	private float time = 0f;

	// =========================
	// Base transform / pivot
	// =========================

	// Batik
	private Vector2 _batikBasePivot;    // Titik acuan awal
	private float   _batikBaseRot;
	private Vector2 _batikBaseScale;

	// Stupa
	private Vector2 _stupaBasePivot;
	private float   _stupaBaseRot;
	private Vector2 _stupaBaseScale;

	// Gong
	private Vector2 _gongBasePivot;
	private Vector2 _gongBaseScale;

	// Gambang
	private Vector2 _gambangBasePivot;

	// Matriks transformasi
	private Transformasi _tf = new Transformasi();

	// 4 corner untuk Gong Sebul
	private Vector2 corner1 = new Vector2(0, 100);
	private Vector2 corner2 = new Vector2(400, 100);
	private Vector2 corner3 = new Vector2(400, 250);
	private Vector2 corner4 = new Vector2(0, 250);
	private float   cycleTime = 8f; // total 8 detik

	public override void _Ready()
	{
		ScreenUtils.Initialize(GetViewport());
		_bentukDasar = new bentukdasar();

		// Buat instans motif
		_batikKawung    = new batikKawung();
		_stupaBorobudur = new stupaBorobudur();
		_gongSebul      = new gongSebul();
		_gambang        = new gambang();

		// Tambahkan ke scene
		AddChild(_batikKawung);
		AddChild(_stupaBorobudur);
		AddChild(_gongSebul);
		AddChild(_gambang);

		// Perhitungan posisi awal sama seperti contoh
		float centerX = (ScreenUtils.MarginLeft + ScreenUtils.MarginRight) / 2f;
		float centerY = (ScreenUtils.MarginTop + ScreenUtils.MarginBottom) / 2f;

		// ========== Batik Kawung ==========
		float batikW = 400f;
		float batikH = 400f;
		float batikScaleVal = 0.7f;

		_batikKawung.Scale    = new Vector2(batikScaleVal, batikScaleVal);
		_batikKawung.Position = new Vector2(
			centerX - (batikW*batikScaleVal)/2f,
			centerY - (batikH*batikScaleVal)/2f
		);
		// Simpan "base" (pivot, rot, scale)
		_batikBasePivot = _batikKawung.Position;
		_batikBaseRot   = _batikKawung.RotationDegrees;
		_batikBaseScale = _batikKawung.Scale;

		// ========== Stupa Borobudur ==========
		float stupaW = 300f;
		float stupaH = 300f;
		float stupaScaleVal = 0.4f;

		_stupaBorobudur.Scale    = new Vector2(stupaScaleVal, stupaScaleVal);
		_stupaBorobudur.Position = new Vector2(
			centerX - (stupaW*stupaScaleVal)/2f,
			centerY - (stupaH*stupaScaleVal)/2f
		);
		_stupaBasePivot = _stupaBorobudur.Position;
		_stupaBaseRot   = _stupaBorobudur.RotationDegrees;
		_stupaBaseScale = _stupaBorobudur.Scale;

		// ========== Gong Sebul ==========
		float gongScaleVal = 0.5f;
		_gongSebul.Scale = new Vector2(gongScaleVal, gongScaleVal);

		// Mulai di corner1
		_gongSebul.Position = corner1;

		_gongBasePivot = _gongSebul.Position;
		_gongBaseScale = _gongSebul.Scale;

		// ========== Gambang ==========
		float gambangScaleVal = 0.6f;
		_gambang.Scale = new Vector2(gambangScaleVal, gambangScaleVal);

		_gambang.Position = new Vector2(500, 100);

		// Simpan pivot (untuk animasi, rotasi, dsb.)
		_gambangBasePivot = _gambang.Position;

		// Tampilkan motif default
		ShowMotif("batik");
	}

	public override void _Process(double delta)
	{
		time += (float)delta;

		// Batas clamp
		float offset = 10f;
		float minX = ScreenUtils.MarginLeft + offset;
		float maxX = ScreenUtils.MarginRight - offset;
		float minY = ScreenUtils.MarginTop + offset;
		float maxY = ScreenUtils.MarginBottom - offset;

		// ---------------------------
		// Animasi Batik Kawung
		// ---------------------------
		if (_batikKawung.Visible)
		{
			// = 1) Hitung "target translasi" (dx, dy)
			float amplitude = 10f;
			float dx = Mathf.Sin(time)    * amplitude;
			float dy = Mathf.Cos(time*0.5f) * amplitude;

			// = 2) Siapkan matrix & pivot
			float[,] matBatik = new float[3,3];
			Transformasi.Matrix3x3Identity(matBatik);

			// Kita akan transform pivot dari basePivot
			Vector2 batikPivotNow = _batikBasePivot; 

			// Lakukan translasi
			// ref => memodifikasi 'batikPivotNow'
			_tf.Translation(matBatik, dx, dy, ref batikPivotNow);

			// Clamp
			batikPivotNow.X = Mathf.Clamp(batikPivotNow.X, minX, maxX);
			batikPivotNow.Y = Mathf.Clamp(batikPivotNow.Y, minY, maxY);

			// Rotasi sin(time*0.5f)*5
			float rotAngle = Mathf.Sin(time*0.5f) * 5f; 
			// karena baseRot  + rotAngle => delta = rotAngle
			_tf.RotationClockwise(matBatik, rotAngle, batikPivotNow);

			// Scale => baseScale*(1 + 0.05f sin(time*2f))
			float scaleFactor = 1f + 0.05f*Mathf.Sin(time*2f);

			// Kita ambil base scale X & Y, lalu kalikan factor => new X, Y
			float scaleX = _batikBaseScale.X * scaleFactor;
			float scaleY = _batikBaseScale.Y * scaleFactor;

			// Pasang scaling => transform pivot jg
			_tf.Scaling(matBatik, scaleFactor, scaleFactor, batikPivotNow);

			// = 3) Terapkan ke Node2D
			// Posisi = pivotNow
			_batikKawung.Position = batikPivotNow;

			// Rotasi = baseRot + rotAngle
			float finalRot = _batikBaseRot + rotAngle;
			_batikKawung.RotationDegrees = finalRot;

			// Scale = (baseScale * scaleFactor)
			_batikKawung.Scale = new Vector2(scaleX, scaleY);
		}

		// ---------------------------
		// Animasi Stupa
		// ---------------------------
		if (_stupaBorobudur.Visible)
		{
			// Rotasi perlahan => baseRot + time*5
			float stupaRot = _stupaBaseRot + time*5f;

			// Getaran vertikal => basePivot.Y + sin(time*2)*8
			float dy = Mathf.Sin(time*2f)*8f;

			float[,] matStupa = new float[3,3];
			Transformasi.Matrix3x3Identity(matStupa);

			Vector2 stupaPivotNow = _stupaBasePivot;

			// translasi => dx=0, dy
			_tf.Translation(matStupa, 0, dy, ref stupaPivotNow);

			// clamp
			stupaPivotNow.X = Mathf.Clamp(stupaPivotNow.X, minX, maxX);
			stupaPivotNow.Y = Mathf.Clamp(stupaPivotNow.Y, minY, maxY);

			// rotasi
			float deltaRot = stupaRot - _stupaBaseRot; // misal per frame
			_tf.RotationClockwise(matStupa, deltaRot, stupaPivotNow);

			// terapkan
			_stupaBorobudur.Position        = stupaPivotNow;
			_stupaBorobudur.RotationDegrees = stupaRot;
			_stupaBorobudur.Scale           = _stupaBaseScale; 
		}

		// ---------------------------
		// Animasi Gong Sebul
		// ---------------------------
		if (_gongSebul.Visible)
		{
			// Bagi 8 detik => 4 segmen
			float segmentTime = cycleTime / 4f;
			float currentMod  = time % cycleTime;
			int segment       = (int)(currentMod / segmentTime);
			float t           = (currentMod % segmentTime) / segmentTime;

			Vector2 fromPos, toPos;
			switch(segment)
			{
				case 0:
					fromPos = corner1; toPos = corner2; break;
				case 1:
					fromPos = corner2; toPos = corner3; break;
				case 2:
					fromPos = corner3; toPos = corner4; break;
				default:
					fromPos = corner4; toPos = corner1; break;
			}
			// Interpolasi linear
			Vector2 lerpPos = fromPos.Lerp(toPos, t);

			// Goyangan ±5 px di sumbu Y, rot ±5 deg
			float goy    = 5f*Mathf.Sin(time*10f);
			float rotGoy = 5f*Mathf.Sin(time*10f);

			// Buat matrix
			float[,] matGong = new float[3,3];
			Transformasi.Matrix3x3Identity(matGong);

			// Kita mulai dari pivot base = corner1? 
			// Namun basePivot= _gongBasePivot ( corner1 )
			Vector2 gongPivotNow = _gongBasePivot;

			// translasi => dx=(lerpPos.x - basePivot.x), dy=(lerpPos.y - basePivot.y) + goy
			float dx = (lerpPos.X - _gongBasePivot.X);
			float dy = (lerpPos.Y - _gongBasePivot.Y) + goy;

			_tf.Translation(matGong, dx, dy, ref gongPivotNow);

			// rot
			_tf.RotationClockwise(matGong, rotGoy, gongPivotNow); 
			// rotGoy di sini diperlakukan "derajat" => sinus( time * 10f ) ± 5

			// terapkan
			// Supaya sesuai animasi semula, Node2D akan kita set sbb:
			Vector2 finalPos = gongPivotNow;

			_gongSebul.Position        = finalPos;
			// Rotasinya = rotGoy saja ( +/- 5 derajat)
			_gongSebul.RotationDegrees = rotGoy;
			_gongSebul.Scale           = _gongBaseScale;
		}

		// ---------------------------
		// Animasi Gambang
		// ---------------------------
		if (_gambang.Visible)
		{
			// 1) Rotasi 90 => horizontal
			float[,] matGam = new float[3,3];
			Transformasi.Matrix3x3Identity(matGam);
			Vector2 gambangPivotNow = _gambangBasePivot;

			_tf.RotationClockwise(matGam, 90f, gambangPivotNow);

			_gambang.Position        = gambangPivotNow;
			_gambang.RotationDegrees = 90f;
			_gambang.Scale           = new Vector2(0.6f, 0.6f);

			// 2) Animasi pemukul
			//    fromPos => lebih jauh (100,150?), 
			//    toPos => (0,80) di bawah bilah?

			float approachTime = 2f;
			if (time < approachTime)
			{
				float alpha = time / approachTime; 
				Vector2 fromPos = new Vector2(100, 150); 
				Vector2 toPos   = new Vector2(0, -100);

				Vector2 lerpOffset = fromPos.Lerp(toPos, alpha);

				float[,] matMallet = new float[3,3];
				Transformasi.Matrix3x3Identity(matMallet);
				Vector2 pivotMallet = Vector2.Zero;
				_tf.Translation(matMallet, lerpOffset.X, lerpOffset.Y, ref pivotMallet);

				// Belum memukul, hanya bergerak ke (0,80)
				_gambang.MalletOffset = pivotMallet;
			}
			else
			{
				// Sudah sampai => mulailah animasi memukul
				float t2 = time - approachTime;
				float amplitude = 10f;
				float yOff = amplitude*Mathf.Sin(t2*5f);

				// Titik dasar pemukul di (0,80), lalu naik-turun ±yOff
				// => final = (0,80 - yOff)
				float[,] matMallet2 = new float[3,3];
				Transformasi.Matrix3x3Identity(matMallet2);

				Vector2 pivotMallet2 = new Vector2(0,-100);
				_tf.Translation(matMallet2, 0, -yOff, ref pivotMallet2);

				_gambang.MalletOffset = pivotMallet2;
			}
		}
	}

	public override void _Draw()
	{
		var marginPoints = _bentukDasar.Margin();
		GraphicsUtils.PutPixelAll(this, marginPoints, GraphicsUtils.DrawStyle.DotDot, Colors.Black);
	}

	// Tombol2
	private void _on_BtnBatik_pressed()    { ShowMotif("batik"); }
	private void _on_BtnStupa_pressed()    { ShowMotif("stupa"); }
	private void _on_BtnGong_pressed()     { ShowMotif("gong"); }
	private void _on_BtnGambang_pressed()  { ShowMotif("gambang"); }

	private void ShowMotif(string motif)
	{
		_batikKawung.Visible    = (motif == "batik");
		_stupaBorobudur.Visible = (motif == "stupa");
		_gongSebul.Visible      = (motif == "gong");
		_gambang.Visible        = (motif == "gambang");
	}
}
