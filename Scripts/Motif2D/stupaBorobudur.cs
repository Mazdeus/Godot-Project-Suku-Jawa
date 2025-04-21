using Godot;
using System;
using System.Collections.Generic;
using Godot.Core;
using Godot.Utils;

/// Node2D untuk menggambar Stupa Borobudur menggunakan bentuk-bentuk primitif
/// yang tersedia di Primitif.cs melalui kelas bentukdasar.
/// 
/// Struktur stupa:
/// 1) Base (alas) 4 layer: 
///     - Layer 1: Persegi panjang
///     - Layer 2: Trapesium
///     - Layer 3: Trapesium terbalik
///     - Layer 4: Persegi panjang
/// 2) Bell (badan stupa) dengan sisi kiri dan kanan yang lebih lurus serta bagian atas melengkung
/// 3) Diamond pattern di dalam bell (menggunakan grid offset dan point-in-polygon)
/// 4) Ring
/// 5) Spire
/// 
/// Fungsi-fungsi gambar seperti Persegi, PersegiPanjang, dan Polygon sudah diambil dari
/// file Primitif.cs melalui _bentukDasar.
public partial class stupaBorobudur : Node2D
{
	private bentukdasar _bentukDasar;
	private List<Vector2> _bellPoints; // Titik-titik untuk bagian bell
	
	public override void _Ready()
	{
		// Inisialisasi kelas bentukdasar, yang menggunakan fungsi-fungsi dari Primitif.cs
		_bentukDasar = new bentukdasar();
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		DrawStupaManualPolygon();
	}

	/// Fungsi utama menggambar stupa Borobudur.
	/// Bagian stupa meliputi:
	/// 1) Base (alas) 4 layer:
	///    - Layer 1: Persegi panjang
	///    - Layer 2: Trapesium (bagian atas lebih sempit)
	///    - Layer 3: Trapesium terbalik (bagian atas lebih lebar)
	///    - Layer 4: Persegi panjang
	/// 2) Bell (badan stupa) dengan sisi yang lebih lurus dan puncak melengkung
	/// 3) Diamond pattern di dalam bell
	/// 4) Ring
	/// 5) Spire
	private void DrawStupaManualPolygon()
	{
		// Ambil margin layar dari ScreenUtils
		int left   = ScreenUtils.MarginLeft;
		int right  = ScreenUtils.MarginRight;
		int bottom = ScreenUtils.MarginBottom;

		// Tentukan ukuran stupa dan posisi horizontal (centerX)
		float totalWidth = 400;
		float centerX = (left + right) / 2f;
		float baseY = bottom - 20;  // Posisi awal base, 20px di atas margin bawah

		// ============== BAGIAN BASE (Alas) 4 LAYER ==============
		// Layer 1: Persegi panjang
		var baseLayer1 = new List<Vector2>()
		{
			new Vector2(centerX - 150, baseY),
			new Vector2(centerX + 150, baseY),
			new Vector2(centerX + 150, baseY - 20),
			new Vector2(centerX - 150, baseY - 20)
		};
		DrawPolygon(baseLayer1, Colors.Black);
		baseY -= 20;

		// Layer 2: Trapesium (bawah lebar, atas lebih sempit)
		var baseLayer2 = new List<Vector2>()
		{
			new Vector2(centerX - 130, baseY),          // Bawah kiri
			new Vector2(centerX + 130, baseY),          // Bawah kanan
			new Vector2(centerX + 110, baseY - 15),       // Atas kanan
			new Vector2(centerX - 110, baseY - 15)        // Atas kiri
		};
		DrawPolygon(baseLayer2, Colors.Black);
		baseY -= 15;

		// Layer 3: Trapesium terbalik (bawah lebih sempit, atas lebih lebar)
		var baseLayer3 = new List<Vector2>()
		{
			new Vector2(centerX - 110, baseY),          // Bawah kiri
			new Vector2(centerX + 110, baseY),          // Bawah kanan
			new Vector2(centerX + 130, baseY - 15),       // Atas kanan
			new Vector2(centerX - 130, baseY - 15)        // Atas kiri
		};
		DrawPolygon(baseLayer3, Colors.Black);
		baseY -= 15;

		// Layer 4: Persegi panjang
		var baseLayer4 = new List<Vector2>()
		{
			new Vector2(centerX - 100, baseY),
			new Vector2(centerX + 100, baseY),
			new Vector2(centerX + 100, baseY - 10),
			new Vector2(centerX - 100, baseY - 10)
		};
		DrawPolygon(baseLayer4, Colors.Black);
		baseY -= 10;

		// ============== BAGIAN BELL (Badan Stupa) ==============
		// Sisi lebih tegak, puncak lebih melengkung
		float bellHeight = 140;
		_bellPoints = new List<Vector2>()
		{
			// Kiri-bawah (lebih tegak)
			new Vector2(centerX - 90, baseY),
			new Vector2(centerX - 88, baseY - 25), // tegak
			new Vector2(centerX - 86, baseY - 35),
			new Vector2(centerX - 84, baseY - 45),
			new Vector2(centerX - 82, baseY - 55),
			new Vector2(centerX - 80, baseY - 65),
			new Vector2(centerX - 78, baseY - 75),
			new Vector2(centerX - 76, baseY - 85),
			new Vector2(centerX - 74, baseY - 95),
			new Vector2(centerX - 72, baseY - 105),
			new Vector2(centerX - 70, baseY - 115),
			new Vector2(centerX - 68, baseY - 125),
			new Vector2(centerX - 64, baseY - 135),
			new Vector2(centerX - 60, baseY - 140),

			// Bagian atas melengkung (kiri puncak -> tengah -> kanan puncak)
			new Vector2(centerX - 60, baseY - bellHeight - 0),
			new Vector2(centerX - 50, baseY - bellHeight - 0),
			new Vector2(centerX - 25, baseY - bellHeight - 0),
			new Vector2(centerX,      baseY - bellHeight - 0),
			new Vector2(centerX + 25, baseY - bellHeight - 0),
			new Vector2(centerX + 50, baseY - bellHeight - 0),
			new Vector2(centerX + 60, baseY - bellHeight - 0),

			// Sisi kanan (cermin sisi kiri)
			new Vector2(centerX + 60, baseY - 140),
			new Vector2(centerX + 64, baseY - 135),
			new Vector2(centerX + 68, baseY - 125),
			new Vector2(centerX + 70, baseY - 115),
			new Vector2(centerX + 72, baseY - 105),
			new Vector2(centerX + 74, baseY - 95),
			new Vector2(centerX + 76, baseY - 85),
			new Vector2(centerX + 78, baseY - 75),
			new Vector2(centerX + 80, baseY - 65),
			new Vector2(centerX + 82, baseY - 55),
			new Vector2(centerX + 84, baseY - 45),
			new Vector2(centerX + 86, baseY - 35),
			new Vector2(centerX + 88, baseY - 25),
			new Vector2(centerX + 90, baseY),
		};
		DrawPolygon(_bellPoints, Colors.Black);

		baseY -= bellHeight;

		// ============== BAGIAN RING ==============
		float ringHeight = 15f;
		var ring = new List<Vector2>()
		{
			new Vector2(centerX - 60, baseY),
			new Vector2(centerX + 60, baseY),
			new Vector2(centerX + 50, baseY - ringHeight),
			new Vector2(centerX - 50, baseY - ringHeight)
		};
		DrawPolygon(ring, Colors.Black);
		baseY -= ringHeight;

		// ============== BAGIAN SPIRE ==============
		float spireHeight = 60;
		var spire = new List<Vector2>()
		{
			new Vector2(centerX - 30, baseY),
			new Vector2(centerX + 30, baseY),
			new Vector2(centerX + 10, baseY - spireHeight),
			new Vector2(centerX - 10, baseY - spireHeight)
		};
		DrawPolygon(spire, Colors.Black);
		baseY -= spireHeight;

		// ============== BAGIAN DIAMOND (di dalam bell) ==============
		DrawDiamondsInsideBell(8, 6);
	}

	/// Fungsi pembantu untuk menggambar outline poligon.
	private void DrawPolygon(List<Vector2> points, Color color)
	{
		var edges = _bentukDasar.Polygon(points);
		GraphicsUtils.PutPixelAll(this, edges, GraphicsUtils.DrawStyle.DotDot, color);
	}

	/// Membuat grid diamond di bounding box bell dengan pola offset (brick pattern) dan
	/// memeriksa apakah titik berada di dalam poligon bell (menggunakan ray casting).
	private void DrawDiamondsInsideBell(int cols, int rows)
	{
		if (_bellPoints == null || _bellPoints.Count < 3)
			return;

		// Hitung bounding box poligon bell
		float minX = float.MaxValue, maxX = float.MinValue;
		float minY = float.MaxValue, maxY = float.MinValue;
		foreach (var p in _bellPoints)
		{
			if (p.X < minX) minX = p.X;
			if (p.X > maxX) maxX = p.X;
			if (p.Y < minY) minY = p.Y;
			if (p.Y > maxY) maxY = p.Y;
		}

		float margin = 20f;
		float left = minX;
		float right = maxX;
		float top = minY;
		float bottom = maxY;

		float usableWidth = (right - margin) - (left + margin);
		float usableHeight = (bottom - margin) - (top + margin);

		float stepX = usableWidth / (cols - 1);
		float stepY = usableHeight / (rows - 1);

		for (int r = 0; r < rows; r++)
		{
			// Brick pattern offset: baris ganjil digeser setengah stepX
			float offsetX = (r % 2 == 1) ? stepX / 2f : 0f;
			for (int c = 0; c < cols; c++)
			{
				float dx = left + margin + offsetX + c * stepX;
				float dy = top + margin + r * stepY;
				if (IsPointInPolygon(_bellPoints, dx, dy))
				{
					DrawOneDiamond(dx, dy, 10f);
				}
			}
		}
	}

	/// Ray casting algorithm untuk mengecek apakah titik (px, py) berada di dalam poligon.
	private bool IsPointInPolygon(List<Vector2> polygon, float px, float py)
	{
		bool inside = false;
		int count = polygon.Count;
		for (int i = 0, j = count - 1; i < count; j = i++)
		{
			float xi = polygon[i].X, yi = polygon[i].Y;
			float xj = polygon[j].X, yj = polygon[j].Y;
			bool intersect = ((yi > py) != (yj > py)) &&
				(px < (xj - xi) * (py - yi) / (yj - yi) + xi);
			if (intersect)
				inside = !inside;
		}
		return inside;
	}

	/// Menggambar satu diamond (belah ketupat) di titik (cx, cy) dengan ukuran tertentu.
	private void DrawOneDiamond(float cx, float cy, float size)
	{
		float half = size / 2f;
		var diamondPoints = new List<Vector2>()
		{
			new Vector2(cx,       cy - half),
			new Vector2(cx + half, cy),
			new Vector2(cx,       cy + half),
			new Vector2(cx - half, cy)
		};

		var edges = _bentukDasar.Polygon(diamondPoints);
		GraphicsUtils.PutPixelAll(this, edges, GraphicsUtils.DrawStyle.DotDot, Colors.Black);
	}
}
