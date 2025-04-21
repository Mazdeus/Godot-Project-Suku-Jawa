using Godot;
using System;
using System.Collections.Generic;
using Godot.Core;
using Godot.Utils;

public partial class gambang : Node2D
{
	// Properti publik untuk mengatur offset pemukul (untuk animasi)
	public Vector2 MalletOffset { get; set; } = Vector2.Zero;

	private bentukdasar _bentukDasar;

	// Kumpulan titik untuk setiap elemen gambang (alat musik)
	private List<List<Vector2>> _barPoints;            
	private List<List<Vector2>> _barLeftCirclePoints;  
	private List<List<Vector2>> _barRightCirclePoints; 
	private List<Vector2> _rodLeftPoints;              
	private List<Vector2> _rodRightPoints;             

	// Dua pemukul (mallet) yang akan dianimasikan
	private List<Vector2> _mallet1CirclePoints;
	private List<Vector2> _mallet1HandlePoints;
	private List<Vector2> _mallet2CirclePoints;
	private List<Vector2> _mallet2HandlePoints;

	// Konstanta jumlah bilah
	private const int BAR_COUNT = 7;
	
	public override void _Ready()
	{
		_bentukDasar = new bentukdasar();

		// Inisialisasi list
		_barPoints            = new List<List<Vector2>>();
		_barLeftCirclePoints  = new List<List<Vector2>>();
		_barRightCirclePoints = new List<List<Vector2>>();
		_rodLeftPoints        = new List<Vector2>();
		_rodRightPoints       = new List<Vector2>();

		_mallet1CirclePoints  = new List<Vector2>();
		_mallet1HandlePoints  = new List<Vector2>();
		_mallet2CirclePoints  = new List<Vector2>();
		_mallet2HandlePoints  = new List<Vector2>();
	}

	public override void _Process(double delta)
	{
		QueueRedraw();  // Minta redraw setiap frame
	}

	public override void _Draw()
	{
		// -------------------------------------------------------------------
		// 1) Menentukan parameter untuk bilah paling atas & perbedaan lebar
		// -------------------------------------------------------------------
		int marginCenterX = (ScreenUtils.MarginLeft + ScreenUtils.MarginRight) / 2;
		float bottomBarWidth = 100f;  
		float startX = marginCenterX - (bottomBarWidth / 2f); 
		float startY = 50f;          
		float barHeight = 20f;       
		float topBarWidth = 60f;     
		float gapBetweenBars = 5f;   

		float widthDiff = bottomBarWidth - topBarWidth; // 40
		float widthStep = widthDiff / (BAR_COUNT - 1);   // ~6.67
		float xShiftPerBar = 2f;       

		// -------------------------------------------------------------------
		// 2) Bangun bilah-bilah gambang + lingkaran kiri/kanan
		// -------------------------------------------------------------------
		for (int i = 0; i < BAR_COUNT; i++)
		{
			float currentBarWidth = topBarWidth + (i * widthStep);
			float currentX = startX - (i * xShiftPerBar);
			float currentY = startY + i * (barHeight + gapBetweenBars);

			// Bentuk bilah (persegi panjang)
			var barShape = _bentukDasar.PersegiPanjang(currentX, currentY, currentBarWidth, barHeight);
			_barPoints.Add(barShape);

			// Lingkaran sisi kiri
			float circleRadius = 5f;
			var leftCircleCenter = new Vector2(currentX + circleRadius, currentY + barHeight / 2f);
			var leftCircle = _bentukDasar.Lingkaran(leftCircleCenter, (int)circleRadius);
			_barLeftCirclePoints.Add(leftCircle);

			// Lingkaran sisi kanan
			var rightCircleCenter = new Vector2(currentX + currentBarWidth - circleRadius, currentY + barHeight / 2f);
			var rightCircle = _bentukDasar.Lingkaran(rightCircleCenter, (int)circleRadius);
			_barRightCirclePoints.Add(rightCircle);
		}

		// -------------------------------------------------------------------
		// 3) Buat garis rod kiri/kanan (menghubungkan lingkaran antar bilah)
		// -------------------------------------------------------------------
		for (int i = 0; i < BAR_COUNT - 1; i++)
		{
			Vector2 leftTop = GetCircleCenter(_barLeftCirclePoints[i]);
			Vector2 leftBottom = GetCircleCenter(_barLeftCirclePoints[i + 1]);
			_rodLeftPoints.AddRange(_bentukDasar.LineBresenham(leftTop.X, leftTop.Y, leftBottom.X, leftBottom.Y));

			Vector2 rightTop = GetCircleCenter(_barRightCirclePoints[i]);
			Vector2 rightBottom = GetCircleCenter(_barRightCirclePoints[i + 1]);
			_rodRightPoints.AddRange(_bentukDasar.LineBresenham(rightTop.X, rightTop.Y, rightBottom.X, rightBottom.Y));
		}

		// -------------------------------------------------------------------
		// 4) Tentukan posisi “terakhir” bilah (supaya pemukul berada di dekatnya)
		// -------------------------------------------------------------------
		// Bilah terakhir = index BAR_COUNT - 1
		float lastBarTop = startY + (BAR_COUNT - 1) * (barHeight + gapBetweenBars);
		float lastBarWidth = topBarWidth + ((BAR_COUNT - 1) * widthStep);
		float lastBarX = startX - ((BAR_COUNT - 1) * xShiftPerBar);
		float lastBarBottomY = lastBarTop + barHeight;

		// Untuk menempatkan pemukul di sebelah kanan alat musik gambang,
		// kita hitung tepi kanan bilah terakhir, lalu tambahkan offset horizontal.
		float malletBaseX = lastBarX + lastBarWidth + 20f;  // gunakan lastBarX + lastBarWidth (tepi kanan) + offset 20px
		float malletBaseY = lastBarBottomY + 40f;  // posisikan secara vertikal, misal 40px di bawah bilah terakhir

		// -------------------------------------------------------------------
		// 5) Gambar dua pemukul (mallet) 
		//    Gunakan MalletOffset untuk animasi (dipicu dari Karya2)
		// -------------------------------------------------------------------
		float malletRadius = 10f;
		float handleLength = 50f;
		float handleThickness = 10f;

		// Pemukul 1 
		Vector2 mallet1CenterBase = new Vector2(malletBaseX, malletBaseY); 
		Vector2 mallet1Center = mallet1CenterBase + MalletOffset;  
		_mallet1CirclePoints = _bentukDasar.Lingkaran(mallet1Center, (int)malletRadius);

		Vector2 mallet1HandlePos = new Vector2(mallet1Center.X + malletRadius, mallet1Center.Y - handleThickness / 2);
		_mallet1HandlePoints = _bentukDasar.PersegiPanjang(mallet1HandlePos.X, mallet1HandlePos.Y, handleLength, handleThickness);

		// Pemukul 2 (diletakkan sedikit lebih ke bawah)
		Vector2 mallet2CenterBase = new Vector2(malletBaseX, malletBaseY + 40f);
		Vector2 mallet2Center = mallet2CenterBase + MalletOffset;
		_mallet2CirclePoints = _bentukDasar.Lingkaran(mallet2Center, (int)malletRadius);

		Vector2 mallet2HandlePos = new Vector2(mallet2Center.X + malletRadius, mallet2Center.Y - handleThickness / 2);
		_mallet2HandlePoints = _bentukDasar.PersegiPanjang(mallet2HandlePos.X, mallet2HandlePos.Y, handleLength, handleThickness);

		// -------------------------------------------------------------------
		// 6) Gambar semua komponen ke Node2D
		// -------------------------------------------------------------------
		var c = Colors.Black;
		// Gambar bilah
		for (int i = 0; i < BAR_COUNT; i++)
		{
			GraphicsUtils.PutPixelAll(this, _barPoints[i], GraphicsUtils.DrawStyle.DotDot, c);
			GraphicsUtils.PutPixelAll(this, _barLeftCirclePoints[i], GraphicsUtils.DrawStyle.DotDot, c);
			GraphicsUtils.PutPixelAll(this, _barRightCirclePoints[i], GraphicsUtils.DrawStyle.DotDot, c);
		}
		// Gambar rod kiri & rod kanan
		GraphicsUtils.PutPixelAll(this, _rodLeftPoints, GraphicsUtils.DrawStyle.DotDot, c);
		GraphicsUtils.PutPixelAll(this, _rodRightPoints, GraphicsUtils.DrawStyle.DotDot, c);

		// Gambar pemukul
		GraphicsUtils.PutPixelAll(this, _mallet1CirclePoints, GraphicsUtils.DrawStyle.DotDot, c);
		GraphicsUtils.PutPixelAll(this, _mallet1HandlePoints, GraphicsUtils.DrawStyle.DotDot, c);
		GraphicsUtils.PutPixelAll(this, _mallet2CirclePoints, GraphicsUtils.DrawStyle.DotDot, c);
		GraphicsUtils.PutPixelAll(this, _mallet2HandlePoints, GraphicsUtils.DrawStyle.DotDot, c);
	}

	// Fungsi bantu untuk mengambil titik pusat lingkaran
	private Vector2 GetCircleCenter(List<Vector2> circlePoints)
	{
		if (circlePoints.Count == 0)
			return Vector2.Zero;

		float minX = circlePoints[0].X;
		float maxX = circlePoints[0].X;
		float minY = circlePoints[0].Y;
		float maxY = circlePoints[0].Y;

		for (int i = 1; i < circlePoints.Count; i++)
		{
			var pt = circlePoints[i];
			if (pt.X < minX) minX = pt.X;
			if (pt.X > maxX) maxX = pt.X;
			if (pt.Y < minY) minY = pt.Y;
			if (pt.Y > maxY) maxY = pt.Y;
		}

		float centerX = (minX + maxX) / 2f;
		float centerY = (minY + maxY) / 2f;
		return new Vector2(centerX, centerY);
	}
}
