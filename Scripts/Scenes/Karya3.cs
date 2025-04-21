// File: Scripts/Scenes/Karya3.cs
// Scene: 050_Motif2D_Polygon_Animasi (Berwarna)
//
// – Empat motif versi **berwarna** (fill polygon / draw‑circle).
// – Animasi identik dengan Karya 2 tetapi dipadukan dengan pewarnaan:
//     Batik = multi‑hue kawung, Stupa = layer coklat‑batu, dll.
// – Masih tanpa interaksi; fokus pada estetika warna & fill polygon.
// --------------------------------------------------------------------

using Godot;
using System;
using Godot.Utils;   // ScreenUtils, dsb.
using Godot.Core;    // Transformasi
using System.Collections.Generic;

/// Karya3: Mirip Karya2, tetapi seluruh motif digambar dengan fill berwarna
/// (bukan lagi titik/line saja).
public partial class Karya3 : Node2D
{
	private bentukdasar _bentukDasar;

	// Empat motif versi "berwarna"
	private BatikKawungColor    _batikKawung;
	private StupaBorobudurColor _stupaBorobudur;
	private GongSebulColor      _gongSebul;
	private GambangColor        _gambang;

	// Variabel waktu animasi
	private float time = 0f;

	// Transformasi manual (Matrix3x3)
	private Transformasi _tf = new Transformasi();

	// ----------------------
	// Base/pivot transform
	// ----------------------
	// Batik
	private Vector2 _batikBasePivot;
	private float   _batikBaseRot;
	private Vector2 _batikBaseScale;

	// Stupa
	private Vector2 _stupaBasePivot;
	private float   _stupaBaseRot;
	private Vector2 _stupaBaseScale;

	// Gong
	private Vector2 _gongBasePivot;
	private Vector2 _gongBaseScale;
	private Vector2 corner1 = new Vector2(0, 100);
	private Vector2 corner2 = new Vector2(400, 100);
	private Vector2 corner3 = new Vector2(400, 250);
	private Vector2 corner4 = new Vector2(0, 250);
	private float   cycleTime = 8f; // total 8 detik

	// Gambang
	private Vector2 _gambangBasePivot;

	public override void _Ready()
	{
		ScreenUtils.Initialize(GetViewport());
		_bentukDasar = new bentukdasar();

		// Buat instance motif "Color"
		_batikKawung    = new BatikKawungColor();
		_stupaBorobudur = new StupaBorobudurColor();
		_gongSebul      = new GongSebulColor();
		_gambang        = new GambangColor();

		// Tambahkan ke scene
		AddChild(_batikKawung);
		AddChild(_stupaBorobudur);
		AddChild(_gongSebul);
		AddChild(_gambang);

		// Hitung titik tengah margin
		float centerX = (ScreenUtils.MarginLeft + ScreenUtils.MarginRight) / 2f;
		float centerY = (ScreenUtils.MarginTop  + ScreenUtils.MarginBottom) / 2f;

		// 1) Batik Kawung
		float batikW = 400f;
		float batikH = 400f;
		float batikScaleVal = 0.7f;

		_batikKawung.Scale    = new Vector2(batikScaleVal, batikScaleVal);
		_batikKawung.Position = new Vector2(
			centerX - (batikW*batikScaleVal)/2f,
			centerY - (batikH*batikScaleVal)/2f
		);
		_batikBasePivot = _batikKawung.Position;
		_batikBaseRot   = _batikKawung.RotationDegrees;
		_batikBaseScale = _batikKawung.Scale;

		// 2) Stupa Borobudur
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

		// 3) Gong Sebul
		float gongScaleVal = 0.5f;
		_gongSebul.Scale = new Vector2(gongScaleVal, gongScaleVal);
		_gongSebul.Position = corner1; // mulai di corner1
		_gongBasePivot = _gongSebul.Position;
		_gongBaseScale = _gongSebul.Scale;

		// 4) Gambang
		float gambangScaleVal = 0.6f;
		_gambang.Scale    = new Vector2(gambangScaleVal, gambangScaleVal);
		_gambang.Position = new Vector2(500, 100);
		_gambangBasePivot = _gambang.Position;

		// Tampilkan motif default
		ShowMotif("batik");
	}

	public override void _Process(double delta)
	{
		time += (float)delta;

		// Batas pergerakan
		float offset = 10f;
		float minX = ScreenUtils.MarginLeft   + offset;
		float maxX = ScreenUtils.MarginRight  - offset;
		float minY = ScreenUtils.MarginTop    + offset;
		float maxY = ScreenUtils.MarginBottom - offset;

		// ---------------------------
		// Animasi Batik
		// ---------------------------
		if (_batikKawung.Visible)
		{
			float amplitude = 10f;
			float dx = Mathf.Sin(time) * amplitude; 
			float dy = Mathf.Cos(time * 0.5f) * amplitude;

			float[,] matBatik = new float[3,3];
			Transformasi.Matrix3x3Identity(matBatik);

			Vector2 pivotNow = _batikBasePivot;
			// translasi
			_tf.Translation(matBatik, dx, dy, ref pivotNow);
			// clamp
			pivotNow.X = Mathf.Clamp(pivotNow.X, minX, maxX);
			pivotNow.Y = Mathf.Clamp(pivotNow.Y, minY, maxY);

			// rotasi ±5 derajat
			float rotAngle = Mathf.Sin(time*0.5f)*5f;
			_tf.RotationClockwise(matBatik, rotAngle, pivotNow);

			// scale: ±5%
			float scaleFactor = 1f + 0.05f*Mathf.Sin(time*2f);
			_tf.Scaling(matBatik, scaleFactor, scaleFactor, pivotNow);

			// apply
			_batikKawung.Position        = pivotNow;
			_batikKawung.RotationDegrees = _batikBaseRot + rotAngle;
			_batikKawung.Scale = new Vector2(_batikBaseScale.X * scaleFactor,
											 _batikBaseScale.Y * scaleFactor);
		}

		// ---------------------------
		// Animasi Stupa
		// ---------------------------
		if (_stupaBorobudur.Visible)
		{
			float stupaRot = _stupaBaseRot + time*5f;
			float dy = Mathf.Sin(time*2f)*8f;

			float[,] matStupa = new float[3,3];
			Transformasi.Matrix3x3Identity(matStupa);

			Vector2 pivotStupa = _stupaBasePivot;

			// translasi vertikal
			_tf.Translation(matStupa, 0, dy, ref pivotStupa);
			// clamp
			pivotStupa.X = Mathf.Clamp(pivotStupa.X, minX, maxX);
			pivotStupa.Y = Mathf.Clamp(pivotStupa.Y, minY, maxY);

			// rotasi
			float deltaRot = stupaRot - _stupaBaseRot;
			_tf.RotationClockwise(matStupa, deltaRot, pivotStupa);

			// apply
			_stupaBorobudur.Position        = pivotStupa;
			_stupaBorobudur.RotationDegrees = stupaRot;
			_stupaBorobudur.Scale           = _stupaBaseScale;
		}

		// ---------------------------
		// Animasi Gong
		// ---------------------------
		if (_gongSebul.Visible)
		{
			float segmentTime = cycleTime / 4f;
			float currentMod  = time % cycleTime;
			int   segment     = (int)(currentMod / segmentTime);
			float t           = (currentMod % segmentTime)/ segmentTime;

			Vector2 fromPos, toPos;
			switch(segment)
			{
				case 0: fromPos = corner1; toPos = corner2; break;
				case 1: fromPos = corner2; toPos = corner3; break;
				case 2: fromPos = corner3; toPos = corner4; break;
				default: fromPos = corner4; toPos = corner1; break;
			}
			Vector2 lerpPos = fromPos.Lerp(toPos, t);

			float goy    = 5f*Mathf.Sin(time*10f);
			float rotGoy = 5f*Mathf.Sin(time*10f);

			float[,] matGong = new float[3,3];
			Transformasi.Matrix3x3Identity(matGong);

			Vector2 pivotGong = _gongBasePivot;

			float dx = (lerpPos.X - _gongBasePivot.X);
			float dy = (lerpPos.Y - _gongBasePivot.Y) + goy;

			_tf.Translation(matGong, dx, dy, ref pivotGong);
			_tf.RotationClockwise(matGong, rotGoy, pivotGong);

			// apply
			_gongSebul.Position        = pivotGong;
			_gongSebul.RotationDegrees = rotGoy;
			_gongSebul.Scale           = _gongBaseScale;
		}

		// ---------------------------
		// Animasi Gambang
		// ---------------------------
		if (_gambang.Visible)
		{
			float[,] matGam = new float[3,3];
			Transformasi.Matrix3x3Identity(matGam);

			Vector2 pivotGambang = _gambangBasePivot;
			// Rotasi 90 derajat
			_tf.RotationClockwise(matGam, 90f, pivotGambang);

			_gambang.Position        = pivotGambang;
			_gambang.RotationDegrees = 90f;
			_gambang.Scale           = new Vector2(0.6f, 0.6f);

			// Pemukul
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

				_gambang.MalletOffset = pivotMallet;
			}
			else
			{
				float t2 = time - approachTime;
				float amplitude = 10f;
				float yOff = amplitude*Mathf.Sin(t2*5f);

				float[,] matMallet2 = new float[3,3];
				Transformasi.Matrix3x3Identity(matMallet2);
				Vector2 pivotMallet2 = new Vector2(0, -100);
				_tf.Translation(matMallet2, 0, -yOff, ref pivotMallet2);

				_gambang.MalletOffset = pivotMallet2;
			}
		}
	}

	public override void _Draw()
	{
		// (Opsional) gambar margin/border
		// Bisa digambar warna hitam
		// Atau dihilangkan, terserah preferensi
		var marginPoints = _bentukDasar.Margin();
		GraphicsUtils.PutPixelAll(this, marginPoints, GraphicsUtils.DrawStyle.DotDot, Colors.Black);
	}

	// Tombol2 (UI)
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
