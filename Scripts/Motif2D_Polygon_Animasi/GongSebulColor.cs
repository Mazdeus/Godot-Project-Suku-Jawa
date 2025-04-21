using Godot;
using System;
using System.Collections.Generic;
using Godot.Core;
using Godot.Utils;

/// Gong Sebul versi berwarna, dengan half‑ellipse dan trapezoid yang valid untuk DrawPolygon.
public partial class GongSebulColor : Node2D
{
	private bentukdasar _bentukDasar;

	public override void _Ready()
	{
		_bentukDasar = new bentukdasar();
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		// 1) Hitung anchor (titik acuan)
		int cx      = (ScreenUtils.MarginLeft  + ScreenUtils.MarginRight)  / 2;
		int cy      = (ScreenUtils.MarginTop   + ScreenUtils.MarginBottom) / 2;
		float ax    = cx;
		float ay    = cy - 125f;  // setengah tinggi ~250

		// 2) Pegangan atas (cokelat muda)
		float w     = 10f, h = 30f;
		DrawRect(
			new Rect2(ax - w/2f, ay, w, h),
			new Color("D2B48C"),  // tan / cokelat muda
			true
		);

		// 3) Hitung endpoint busur
		float arcCx = ax, arcCy = ay + 50f;
		int   arcRx = 30, arcRy = 20;
		Vector2 leftArc  = new Vector2(arcCx - arcRx, arcCy);
		Vector2 rightArc = new Vector2(arcCx + arcRx, arcCy);

		// 4) Buat polygon half‑ellipse (busur) dari leftArc → rightArc
		int segs = 32;
		var arcPts = new List<Vector2>(segs + 2);
		arcPts.Add(leftArc);
		for (int i = segs; i >= 0; i--)
		{
			float t = (i / (float)segs) * Mathf.Pi; // π..0
			float x = arcCx + arcRx * Mathf.Cos(t);
			float y = arcCy + arcRy * Mathf.Sin(t);
			arcPts.Add(new Vector2(x, y));
		}
		arcPts.Add(rightArc);

		// 5) Lingkaran bawah
		float cirCx = ax, cirCy = ay + 200f;
		int   cirR  = 50;
		Vector2 leftCir  = new Vector2(cirCx - cirR, cirCy);
		Vector2 rightCir = new Vector2(cirCx + cirR, cirCy);

		// 6) Fill BODY (trapesium) cokelat muda
		var bodyPoly = new Vector2[] { leftArc, rightArc, rightCir, leftCir };
		var tan      = new Color("D2B48C");
		var bodyCols = new Color[bodyPoly.Length];
		for (int i = 0; i < bodyCols.Length; i++)
			bodyCols[i] = tan;
		DrawPolygon(bodyPoly, bodyCols);

		// 7) Fill busur kuning
		var arcArr    = arcPts.ToArray();
		var yellow    = new Color("FFD95B");
		var yellowCols= new Color[arcArr.Length];
		for (int i = 0; i < yellowCols.Length; i++)
			yellowCols[i] = yellow;
		DrawPolygon(arcArr, yellowCols);

		// 8) Fill lingkaran bawah keemasan (tanpa outline)
		DrawCircle(new Vector2(cirCx, cirCy), cirR, new Color("FFCA28"));

		// 9) Outline: pegangan, busur, dan sisi
		// Pegangan outline
		DrawRect(
			new Rect2(ax - w/2f, ay, w, h),
			Colors.Transparent,
			false,
			2f
		);
		// Busur outline
		for (int i = 1; i < arcPts.Count; i++)
			DrawLine(arcPts[i - 1], arcPts[i], Colors.Black, 2f);
		// Sisi kiri & kanan
		DrawLine(leftArc,  leftCir,  Colors.Black, 2f);
		DrawLine(rightArc, rightCir, Colors.Black, 2f);
	}
}
