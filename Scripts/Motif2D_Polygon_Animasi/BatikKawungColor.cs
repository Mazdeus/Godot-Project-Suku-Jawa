using Godot;
using System;
using System.Collections.Generic;
using Godot.Core;
using Godot.Utils;

public partial class BatikKawungColor : Node2D
{
	private bentukdasar _bentukDasar;

	//--------------------------------------------------------------------
	// 1) Parameter Manual untuk Posisi Elips
	//--------------------------------------------------------------------
	private Vector2[] ellipseCenterOffsets = new Vector2[]
	{
		new Vector2(40, -40),   // Untuk elips pada 45° (top-right)
		new Vector2(-40, -40),  // Untuk elips pada 135° (top-left)
		new Vector2(-40, 40),   // Untuk elips pada 225° (bottom-left)
		new Vector2(40, 40)     // Untuk elips pada 315° (bottom-right)
	};

	// Sudut rotasi untuk tiap elips (dalam derajat)
	private float[] ellipseRotations = new float[] { 45f, 135f, 225f, 315f };

	//--------------------------------------------------------------------
	// 2) Parameter Lingkaran Kecil di Dalam Elips (Fill)
	//--------------------------------------------------------------------
	private Vector2[][] smallCircleManualOffsets = new Vector2[][]
	{
		new Vector2[] { new Vector2(10, 0), new Vector2(-10, 0) },
		new Vector2[] { new Vector2(10, 0), new Vector2(-10, 0) },
		new Vector2[] { new Vector2(10, 0), new Vector2(-10, 0) },
		new Vector2[] { new Vector2(10, 0), new Vector2(-10, 0) }
	};

	//--------------------------------------------------------------------
	// 3) Parameter Belah Ketupat (Diamond) Manual
	//--------------------------------------------------------------------
	private Vector2[] diamondManualOffsets = new Vector2[]
	{
		new Vector2(92, -40),   // Corner atas
		new Vector2(125, 0),    // Corner kanan
		new Vector2(92, 40),    // Corner bawah
		new Vector2(60, 0)      // Corner kiri
	};

	//--------------------------------------------------------------------
	// 4) Variabel Warna untuk Fill Setiap Komponen
	//--------------------------------------------------------------------
	private Color centralCircleColor = Colors.Red;
	private Color[] ellipseColors = new Color[] { Colors.Blue, Colors.Green, Colors.Orange, Colors.Purple };
	private Color[] smallCircleColors = new Color[] { Colors.Yellow, Colors.Cyan, Colors.Magenta, Colors.Lime };
	private Color diamondColor = Colors.White;

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
		DrawKawungPattern();
	}

	/// Menggambar grid motif Kawung di area margin.
	private void DrawKawungPattern()
	{
		int left = ScreenUtils.MarginLeft;
		int top = ScreenUtils.MarginTop;
		int right = ScreenUtils.MarginRight;
		int bottom = ScreenUtils.MarginBottom;

		int cellSize = 180;
		int widthAvailable = right - left;
		int heightAvailable = bottom - top;
		int cols = widthAvailable / cellSize;
		int rows = heightAvailable / cellSize;

		for (int r = 0; r < rows; r++)
		{
			for (int c = 0; c < cols; c++)
			{
				float centerX = left + c * cellSize + cellSize / 2f;
				float centerY = top + r * cellSize + cellSize / 2f;
				DrawOneKawungCell(centerX, centerY, cellSize);
			}
		}
	}

	/// Menggambar satu sel motif Kawung dengan fill warna-warni:
	/// (A) Fill lingkaran pusat.
	/// (B) Fill empat elips (diisi) beserta lingkaran kecil (fill) di dalamnya.
	///     Pengisian elips mengikuti rotasi yang diatur.
	/// (C) Fill diamond (belah ketupat) dengan warna berbeda.
	private void DrawOneKawungCell(float cx, float cy, int cellSize)
	{
		//----------------------------------------------------------------
		// (A) Fill Lingkaran Pusat
		//----------------------------------------------------------------
		float centerCircleRadius = cellSize * 0.10f;
		FillCircle(new Vector2(cx, cy), (int)centerCircleRadius, centralCircleColor);

		//----------------------------------------------------------------
		// (B) Fill Empat Elips + Lingkaran Kecil di Dalamnya
		//----------------------------------------------------------------
		int rx = (int)(cellSize * 0.15f);
		int ry = (int)(cellSize * 0.25f);
		List<Vector2> ellipseCenters = new List<Vector2>();

		for (int i = 0; i < ellipseCenterOffsets.Length; i++)
		{
			Vector2 offset = ellipseCenterOffsets[i];
			float angleDeg = ellipseRotations[i];

			// Tentukan pusat elips secara manual.
			Vector2 eCenter = new Vector2(cx, cy) + offset;
			ellipseCenters.Add(eCenter);

			// Fill elips dengan fill yang mengikuti rotasi.
			FillRotatedEllipse(eCenter, rx, ry, angleDeg, ellipseColors[i]);

			// (Opsional) Gambar outline elips.
			List<Vector2> ellipseOutline = _bentukDasar.Elips(eCenter, rx, ry);
			RotatePointsAroundPivot(ellipseOutline, eCenter, angleDeg);
			GraphicsUtils.PutPixelAll(this, ellipseOutline, GraphicsUtils.DrawStyle.DotDot, Colors.Black);

			// Fill lingkaran kecil di dalam elips.
			Vector2[] smallOffsets = smallCircleManualOffsets[i];
			foreach (Vector2 off in smallOffsets)
			{
				// Rotasi offset kecil agar selaras dengan orientasi elips.
				Vector2 rotatedOff = RotatePoint(off, Vector2.Zero, angleDeg);
				Vector2 smallCenter = eCenter + rotatedOff;
				int smallRadius = rx / 3;
				FillCircle(smallCenter, smallRadius, smallCircleColors[i]);
			}
		}

		//----------------------------------------------------------------
		// (C) Fill Diamond (Belah Ketupat) Manual
		//----------------------------------------------------------------
		List<Vector2> diamondPoints = new List<Vector2>();
		foreach (var off in diamondManualOffsets)
		{
			Vector2 corner = new Vector2(cx + off.X, cy + off.Y);
			diamondPoints.Add(corner);
		}
		FillPolygon(diamondPoints, diamondColor);
	}

	/// Mengisi sebuah lingkaran dengan warna menggunakan algoritma iterasi bounding box.
	private void FillCircle(Vector2 center, int radius, Color color)
	{
		int cx = (int)center.X;
		int cy = (int)center.Y;
		for (int yy = cy - radius; yy <= cy + radius; yy++)
		{
			for (int xx = cx - radius; xx <= cx + radius; xx++)
			{
				int dx = xx - cx;
				int dy = yy - cy;
				if (dx * dx + dy * dy <= radius * radius)
				{
					GraphicsUtils.PutPixel(this, xx, yy, color);
				}
			}
		}
	}

	/// Mengisi sebuah elips yang sudah dirotasi.
	/// Proses: untuk setiap pixel di bounding box, kita hitung koordinat lokal melalui
	/// rotasi balik, lalu uji apakah pixel tersebut berada di dalam elips standar.
	private void FillRotatedEllipse(Vector2 center, int rx, int ry, float angleDeg, Color color)
	{
		// Hitung bounding box "aman" menggunakan faktor sqrt(2)
		int bound = (int)Mathf.Ceil(Math.Max(rx, ry) * 1.42f);
		int left = (int)(center.X - bound);
		int right = (int)(center.X + bound);
		int top = (int)(center.Y - bound);
		int bottom = (int)(center.Y + bound);

		// Konversi sudut ke radian dan siapkan cos, sin.
		float rad = Mathf.DegToRad(angleDeg);
		float cos = Mathf.Cos(rad);
		float sin = Mathf.Sin(rad);

		for (int y = top; y <= bottom; y++)
		{
			for (int x = left; x <= right; x++)
			{
				// Translasi ke sistem koordinat elips
				float dx = x - center.X;
				float dy = y - center.Y;

				// Rotasi balik (-angleDeg) agar dapat uji persamaan elips tanpa rotasi.
				float xLocal = dx * cos + dy * sin;
				float yLocal = -dx * sin + dy * cos;

				if ((xLocal * xLocal) / (float)(rx * rx) + (yLocal * yLocal) / (float)(ry * ry) <= 1f)
				{
					GraphicsUtils.PutPixel(this, x, y, color);
				}
			}
		}
	}

	/// Mengisi sebuah poligon (misalnya diamond) dengan warna menggunakan algoritma scanline sederhana.
	private void FillPolygon(List<Vector2> points, Color color)
	{
		int minX = (int)points[0].X, maxX = (int)points[0].X;
		int minY = (int)points[0].Y, maxY = (int)points[0].Y;
		foreach (var pt in points)
		{
			if (pt.X < minX) minX = (int)pt.X;
			if (pt.X > maxX) maxX = (int)pt.X;
			if (pt.Y < minY) minY = (int)pt.Y;
			if (pt.Y > maxY) maxY = (int)pt.Y;
		}
		for (int y = minY; y <= maxY; y++)
		{
			for (int x = minX; x <= maxX; x++)
			{
				if (IsPointInPolygon(points, new Vector2(x, y)))
				{
					GraphicsUtils.PutPixel(this, x, y, color);
				}
			}
		}
	}

	/// Menguji apakah sebuah titik berada di dalam poligon menggunakan algoritma ray-casting.
	private bool IsPointInPolygon(List<Vector2> polygon, Vector2 point)
	{
		bool result = false;
		int j = polygon.Count - 1;
		for (int i = 0; i < polygon.Count; i++)
		{
			if ((polygon[i].Y < point.Y && polygon[j].Y >= point.Y ||
				 polygon[j].Y < point.Y && polygon[i].Y >= point.Y) &&
				(polygon[i].X + (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * 
				 (polygon[j].X - polygon[i].X) < point.X))
			{
				result = !result;
			}
			j = i;
		}
		return result;
	}

	/// Memutar sekumpulan titik di sekitar pivot sebesar angleDeg.
	private void RotatePointsAroundPivot(List<Vector2> points, Vector2 pivot, float angleDeg)
	{
		for (int i = 0; i < points.Count; i++)
		{
			points[i] = RotatePoint(points[i], pivot, angleDeg);
		}
	}

	/// Memutar satu titik di sekitar pivot sebesar angleDeg derajat.
	private Vector2 RotatePoint(Vector2 pt, Vector2 pivot, float angleDeg)
	{
		float rad = Mathf.DegToRad(angleDeg);
		float sin = Mathf.Sin(rad);
		float cos = Mathf.Cos(rad);
		float x = pt.X - pivot.X;
		float y = pt.Y - pivot.Y;
		float xNew = x * cos - y * sin;
		float yNew = x * sin + y * cos;
		xNew += pivot.X;
		yNew += pivot.Y;
		return new Vector2(xNew, yNew);
	}
}
