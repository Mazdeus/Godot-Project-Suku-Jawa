using Godot;
using System;
using System.Collections.Generic;
using Godot.Core;
using Godot.Utils;

public partial class batikKawung : Node2D
{
	private bentukdasar _bentukDasar;

	//--------------------------------------------------------------------
	// 1) Parameter Manual untuk Posisi Elips
	//--------------------------------------------------------------------
	// Pusat elips (berdasarkan offset manual dari pusat sel).
	// Contoh: (40, -40) untuk elips pada "45°" (top-right), dan seterusnya.
	private Vector2[] ellipseCenterOffsets = new Vector2[]
	{
		new Vector2(40, -40),   // 45° (top-right)
		new Vector2(-40, -40),  // 135° (top-left)
		new Vector2(-40, 40),   // 225° (bottom-left)
		new Vector2(40, 40)     // 315° (bottom-right)
	};

	// Sudut rotasi untuk tiap elips, sehingga sumbu elips mengikuti arah yang diinginkan.
	private float[] ellipseRotations = new float[] { 45f, 135f, 225f, 315f };

	//--------------------------------------------------------------------
	// 2) Parameter Lingkaran Kecil di Dalam Elips (Outline)
	//--------------------------------------------------------------------
	// Masing-masing elips memiliki array offset untuk menempatkan lingkaran kecil outline relatif ke pusat elips.
	// Nilai offset ini (misalnya, (10,0) dan (-10,0)) akan diputar sesuai dengan rotasi elips.
	private Vector2[][] smallCircleManualOffsets = new Vector2[][]
	{
		new Vector2[] { new Vector2(10, 0), new Vector2(-10, 0) },  // Untuk elips pada 45° (top-right)
		new Vector2[] { new Vector2(10, 0), new Vector2(-10, 0) },  // Untuk elips pada 135° (top-left)
		new Vector2[] { new Vector2(10, 0), new Vector2(-10, 0) },  // Untuk elips pada 225° (bottom-left)
		new Vector2[] { new Vector2(10, 0), new Vector2(-10, 0) }   // Untuk elips pada 315° (bottom-right)
	};

	//--------------------------------------------------------------------
	// 3) Parameter Belah Ketupat (Diamond) Manual
	//--------------------------------------------------------------------
	// Posisi sudut-sudut diamond relatif terhadap pusat sel.
	private Vector2[] diamondManualOffsets = new Vector2[]
	{
		new Vector2(92, -40),   // Corner atas
		new Vector2(125, 0),    // Corner kanan
		new Vector2(92, 40),    // Corner bawah
		new Vector2(60, 0)    // Corner kiri
	};

	public override void _Ready()
	{
		_bentukDasar = new bentukdasar();
	}

	public override void _Process(double delta)
	{
		// Pastikan motif selalu di-redraw setiap frame
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

		// Atur ukuran sel
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

	/// Menggambar satu sel motif Kawung:
	/// 1) Lingkaran pusat.
	/// 2) Empat elips (dengan outline lingkaran kecil di dalamnya) yang posisinya ditentukan secara manual,
	///    muncul pada sudut 45°, 135°, 225°, dan 315°.
	/// 3) Belah ketupat (diamond) yang diatur secara manual di antara komponen kawung.
	private void DrawOneKawungCell(float cx, float cy, int cellSize)
	{
		//----------------------------------------------------------------
		// (A) LINGKARAN PUSAT
		//----------------------------------------------------------------
		float centerCircleRadius = cellSize * 0.10f;
		var centerCirclePoints = _bentukDasar.Lingkaran(new Vector2(cx, cy), (int)centerCircleRadius);
		GraphicsUtils.PutPixelAll(this, centerCirclePoints, GraphicsUtils.DrawStyle.DotDot, Colors.Black);

		//----------------------------------------------------------------
		// (B) EMPAT ELIPS + LINGKARAN KECIL (OUTLINE) DI DALAMNYA
		//----------------------------------------------------------------
		// Parameter untuk elips: gunakan nilai yang sama untuk keempat elips
		int rx = (int)(cellSize * 0.15f);
		int ry = (int)(cellSize * 0.25f);

		// Simpan pusat elips untuk referensi (jika perlu)
		List<Vector2> ellipseCenters = new List<Vector2>();

		for (int i = 0; i < ellipseCenterOffsets.Length; i++)
		{
			Vector2 offset = ellipseCenterOffsets[i];
			float angleDeg = ellipseRotations[i];

			// 1) Tentukan pusat elips secara manual
			Vector2 eCenter = new Vector2(cx, cy) + offset;
			ellipseCenters.Add(eCenter);

			// 2) Gambar outline elips
			List<Vector2> ellipsePoints = _bentukDasar.Elips(eCenter, rx, ry);
			RotatePointsAroundPivot(ellipsePoints, eCenter, angleDeg);
			GraphicsUtils.PutPixelAll(this, ellipsePoints, GraphicsUtils.DrawStyle.DotDot, Colors.Black);

			// 3) Gambar lingkaran kecil (outline) di dalam elips
			Vector2[] smallOffsets = smallCircleManualOffsets[i];
			foreach (Vector2 off in smallOffsets)
			{
				// Rotasi offset kecil agar sejajar dengan orientasi elips
				Vector2 rotatedOff = RotatePoint(off, Vector2.Zero, angleDeg);
				Vector2 smallCenter = eCenter + rotatedOff;
				int smallRadius = rx / 3;  // Atur ukuran sesuai keinginan

				// Gambar outline lingkaran kecil (tanpa fill) menggunakan _bentukDasar.Lingkaran
				var smallCirclePoints = _bentukDasar.Lingkaran(smallCenter, smallRadius);
				GraphicsUtils.PutPixelAll(this, smallCirclePoints, GraphicsUtils.DrawStyle.DotDot, Colors.Black);
			}
		}

		//----------------------------------------------------------------
		// (C) BELAH KETUPAT (DIAMOND) MANUAL
		//----------------------------------------------------------------
		// Anda bisa atur posisi belah ketupat dengan mengubah diamondManualOffsets
		List<Vector2> diamondPoints = new List<Vector2>();
		foreach (var off in diamondManualOffsets)
		{
			Vector2 corner = new Vector2(cx + off.X, cy + off.Y);
			diamondPoints.Add(corner);
		}
		var diamondEdges = _bentukDasar.Polygon(diamondPoints);
		GraphicsUtils.PutPixelAll(this, diamondEdges, GraphicsUtils.DrawStyle.DotDot, Colors.Black);
	}

	/// Memutar sekumpulan titik (points) di sekitar pivot sebesar angleDeg.
	private void RotatePointsAroundPivot(List<Vector2> points, Vector2 pivot, float angleDeg)
	{
		for (int i = 0; i < points.Count; i++)
		{
			points[i] = RotatePoint(points[i], pivot, angleDeg);
		}
	}

	/// Memutar satu titik (pt) di sekitar pivot sebesar angleDeg derajat.
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

	/// Fungsi FillCircle masih disediakan jika Anda butuh menggambar lingkaran fill,
	/// tapi untuk lingkaran kecil di elips, kita menggunakan outline (bukan fill).
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
}
