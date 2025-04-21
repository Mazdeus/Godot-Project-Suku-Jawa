using Godot;
using System;
using System.Collections.Generic;
using Godot.Core;
using Godot.Utils;

public partial class gongSebul : Node2D
{
	private bentukdasar _bentukDasar;

	// Kumpulan titik
	private List<Vector2> _topRectPoints;      
	private List<Vector2> _arcPoints;          
	private List<Vector2> _leftSidePoints;     
	private List<Vector2> _rightSidePoints;    
	private List<Vector2> _bottomCirclePoints; 
	
	public override void _Ready()
	{
		_bentukDasar = new bentukdasar();
		
		_topRectPoints      = new List<Vector2>();
		_arcPoints          = new List<Vector2>();
		_leftSidePoints     = new List<Vector2>();
		_rightSidePoints    = new List<Vector2>();
		_bottomCirclePoints = new List<Vector2>();
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		//========================================================
		// 1) Hitung titik pusat margin (agar bentuk di tengah)
		//    ScreenUtils menyediakan MarginLeft, MarginRight, dsb.
		//========================================================
		int marginCenterX = (ScreenUtils.MarginLeft + ScreenUtils.MarginRight) / 2;
		int marginCenterY = (ScreenUtils.MarginTop + ScreenUtils.MarginBottom) / 2;

		//========================================================
		// 2) Tentukan "anchor" (titik acuan) untuk meletakkan
		//    keseluruhan bentuk. 
		//    Estimasi tinggi total ~250 px (rect 30, arc 20, jarak
		//    ke lingkaran 100, lingkaran diameter 100).
		//    Kita posisikan anchor di 125 px di atas marginCenterY.
		//========================================================
		float anchorX = marginCenterX; 
		float anchorY = marginCenterY - 125; // setengah dari total 250

		//========================================================
		// 3) Persegi panjang atas (pegangan)
		//    Letakkan top rect di anchorY, tepat di tengah sumbu X
		//    Lebar=10, tinggi=30
		//========================================================
		float rectWidth  = 10;
		float rectHeight = 30;
		float rectLeft   = anchorX - rectWidth / 2f; // agar center
		float rectTop    = anchorY;

		// Hati-hati, bentukdasar.PersegiPanjang() minta float x, y, panjang, lebar
		_topRectPoints = _bentukDasar.PersegiPanjang(
			rectLeft, 
			rectTop, 
			rectWidth,   // panjang (sumbu X)
			rectHeight   // lebar  (sumbu Y)
		);

		// Batas bawah persegi => anchorY + 30

		//========================================================
		// 4) Arc/busur di atas badan
		//    Letakkan center arc di (anchorX, anchorY + 50)
		//    radiusX=30, radiusY=20
		//    Lalu ambil Y <= centerArcY agar hanya setengah atas
		//========================================================
		float arcCenterX  = anchorX;
		float arcCenterY  = anchorY + 50;
		float arcRadiusX  = 30;
		float arcRadiusY  = 20;

		var fullArc = _bentukDasar.Elips(new Vector2(arcCenterX, arcCenterY), (int)arcRadiusX, (int)arcRadiusY);
		_arcPoints.Clear();
		foreach (var pt in fullArc)
		{
			if (pt.Y <= arcCenterY) // hanya bagian atas
				_arcPoints.Add(pt);
		}

		//========================================================
		// 5) Lingkaran di bagian bawah
		//    Center: (anchorX, anchorY + 200)
		//    radius=50 => diameter 100 px
		//========================================================
		float circleCenterX = anchorX;
		float circleCenterY = anchorY + 200;  
		float circleRadius  = 50;

		_bottomCirclePoints = _bentukDasar.Lingkaran(new Vector2(circleCenterX, circleCenterY), (int)circleRadius);

		//========================================================
		// 6) Tarik sisi kiri & kanan dari arc ke tepi lingkaran
		//    Arc left:  (arcCenterX - arcRadiusX, arcCenterY) => (anchorX - 30, anchorY + 50)
		//    Circle left boundary: (anchorX - 50, anchorY + 200)
		//========================================================
		float arcLeftX = arcCenterX - arcRadiusX;   //  anchorX - 30
		float arcLeftY = arcCenterY;                //  anchorY + 50
		float arcRightX = arcCenterX + arcRadiusX;  //  anchorX + 30
		float arcRightY = arcCenterY;               //  anchorY + 50

		float circleLeftX  = circleCenterX - circleRadius;  // anchorX - 50
		float circleLeftY  = circleCenterY;                 // anchorY + 200
		float circleRightX = circleCenterX + circleRadius;  // anchorX + 50
		float circleRightY = circleCenterY;                 // anchorY + 200

		_leftSidePoints = _bentukDasar.LineBresenham(
			arcLeftX, arcLeftY, 
			circleLeftX, circleLeftY
		);
		_rightSidePoints = _bentukDasar.LineBresenham(
			arcRightX, arcRightY,
			circleRightX, circleRightY
		);

		//========================================================
		// 7) Gambar semua dengan warna & gaya yang diinginkan
		//========================================================
		Color drawColor = Colors.Black;

		GraphicsUtils.PutPixelAll(this, _topRectPoints,      GraphicsUtils.DrawStyle.DotDot, drawColor);
		GraphicsUtils.PutPixelAll(this, _arcPoints,          GraphicsUtils.DrawStyle.DotDot, drawColor);
		GraphicsUtils.PutPixelAll(this, _leftSidePoints,     GraphicsUtils.DrawStyle.DotDot, drawColor);
		GraphicsUtils.PutPixelAll(this, _rightSidePoints,    GraphicsUtils.DrawStyle.DotDot, drawColor);
		GraphicsUtils.PutPixelAll(this, _bottomCirclePoints, GraphicsUtils.DrawStyle.DotDot, drawColor);
	}
}
