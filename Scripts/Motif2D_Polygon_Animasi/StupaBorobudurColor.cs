using Godot;
using System;
using System.Collections.Generic;

/// Stupa Borobudur berwarna. 
/// Kita siapkan polygons di _Draw(), lalu panggil DrawColoredPolygon.
public partial class StupaBorobudurColor : Node2D
{
	private List<Vector2> _bellPoints;

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		DrawStupa();
	}

	private void DrawStupa()
	{
		// Contoh penempatan stupa => langsung di Node2D.Position
		// Kita fokus fill polygon base-layer, dsb.

		// Base Y => ~250
		float baseY = 250;
		float centerX = 0; 
		// Karena Node2D (Position) sudah diatur di Karya3,
		// di sini kita cukup gambar relatif (local coords).

		// 1) Layer 1 (persegi panjang)
		var layer1 = new Vector2[] {
			new Vector2(centerX - 150, baseY),
			new Vector2(centerX + 150, baseY),
			new Vector2(centerX + 150, baseY - 20),
			new Vector2(centerX - 150, baseY - 20)
		};
		DrawPolygon(layer1, new Color[] { new Color("8F6A3A") }); // coklat tua
		baseY -= 20;

		// 2) Layer 2 (trapesium)
		var layer2 = new Vector2[] {
			new Vector2(centerX - 130, baseY),
			new Vector2(centerX + 130, baseY),
			new Vector2(centerX + 110, baseY - 15),
			new Vector2(centerX - 110, baseY - 15)
		};
		DrawPolygon(layer2, new Color[] { new Color("9F7B4B") });
		baseY -= 15;

		// 3) Layer 3 (trapesium terbalik)
		var layer3 = new Vector2[] {
			new Vector2(centerX - 110, baseY),
			new Vector2(centerX + 110, baseY),
			new Vector2(centerX + 130, baseY - 15),
			new Vector2(centerX - 130, baseY - 15)
		};
		DrawPolygon(layer3, new Color[] { new Color("9F7B4B") });
		baseY -= 15;

		// 4) Layer 4 (persegi panjang)
		var layer4 = new Vector2[] {
			new Vector2(centerX - 100, baseY),
			new Vector2(centerX + 100, baseY),
			new Vector2(centerX + 100, baseY - 10),
			new Vector2(centerX - 100, baseY - 10)
		};
		DrawPolygon(layer4, new Color[] { new Color("B48B5E") });
		baseY -= 10;

		// ============ Bell (badan stupa) ============
		float bellHeight = 140;
		// Buat list titik => fill
		_bellPoints = new List<Vector2>() {
			// kiri
			new Vector2(centerX - 90, baseY),
			new Vector2(centerX - 88, baseY - 25),
			new Vector2(centerX - 60, baseY - 140),
			new Vector2(centerX - 60, baseY - bellHeight - 0),
			new Vector2(centerX - 50, baseY - bellHeight - 0),
			new Vector2(centerX,      baseY - bellHeight - 0),
			new Vector2(centerX + 50, baseY - bellHeight - 0),
			new Vector2(centerX + 60, baseY - bellHeight - 0),
			new Vector2(centerX + 60, baseY - 140),
			new Vector2(centerX + 88, baseY - 25),
			new Vector2(centerX + 90, baseY),
		};
		DrawPolygon(_bellPoints.ToArray(), new Color[] { new Color("D1C7A1") });
		baseY -= bellHeight;

		// ============ Ring ============
		float ringHeight = 15f;
		var ring = new Vector2[] {
			new Vector2(centerX - 60, baseY),
			new Vector2(centerX + 60, baseY),
			new Vector2(centerX + 50, baseY - ringHeight),
			new Vector2(centerX - 50, baseY - ringHeight)
		};
		DrawPolygon(ring, new Color[] { new Color("6E6243") });
		baseY -= ringHeight;

		// ============ Spire ============
		float spireHeight = 60;
		var spire = new Vector2[] {
			new Vector2(centerX - 30, baseY),
			new Vector2(centerX + 30, baseY),
			new Vector2(centerX + 10, baseY - spireHeight),
			new Vector2(centerX - 10, baseY - spireHeight)
		};
		DrawPolygon(spire, new Color[] { new Color("51442C") });
		baseY -= spireHeight;

		// ============ Diamond di dalam bell (opsional) ============
		DrawDiamondsInsideBell();
	}

	private void DrawDiamondsInsideBell()
	{
		if (_bellPoints == null || _bellPoints.Count < 3)
			return;

		// bounding box poligon
		float minX = float.MaxValue, maxX = float.MinValue;
		float minY = float.MaxValue, maxY = float.MinValue;
		foreach (var p in _bellPoints)
		{
			if (p.X < minX) minX = p.X;
			if (p.X > maxX) maxX = p.X;
			if (p.Y < minY) minY = p.Y;
			if (p.Y > maxY) maxY = p.Y;
		}

		// Buat grid
		int cols = 8, rows = 6;
		float margin = 20f;
		float usableWidth  = (maxX - margin) - (minX + margin);
		float usableHeight = (maxY - margin) - (minY + margin);
		float stepX = usableWidth /(cols -1);
		float stepY = usableHeight/(rows -1);

		for (int r=0; r<rows; r++)
		{
			float offsetX = (r%2 ==1)? stepX/2f :0;
			for (int c=0; c<cols; c++)
			{
				float dx = (minX + margin + offsetX + c*stepX);
				float dy = (minY + margin + r*stepY);
				if (IsPointInPolygon(_bellPoints, dx, dy))
				{
					DrawOneDiamond(new Vector2(dx, dy), 10f);
				}
			}
		}
	}

	private bool IsPointInPolygon(List<Vector2> poly, float px, float py)
	{
		bool inside = false;
		int count = poly.Count;
		for (int i=0,j=count-1; i<count; j=i++)
		{
			float xi=poly[i].X, yi=poly[i].Y;
			float xj=poly[j].X, yj=poly[j].Y;
			bool intersect = ((yi>py)!=(yj>py)) 
							 && (px < (xj - xi)*(py - yi)/(yj - yi)+ xi);
			if (intersect) inside=!inside;
		}
		return inside;
	}

	private void DrawOneDiamond(Vector2 center, float size)
	{
		float half = size/2f;
		var points = new Vector2[] {
			new Vector2(center.X,       center.Y - half),
			new Vector2(center.X+ half, center.Y),
			new Vector2(center.X,       center.Y + half),
			new Vector2(center.X- half, center.Y)
		};
		DrawPolygon(points, new Color[] { new Color("000000") }); // isian diamond hitam
	}
}
