using Godot;
using System;
using System.Collections.Generic;

/// Gambang berwarna + pemukul, dengan rod (garis penghubung)
public partial class GambangColor : Node2D
{
	// Offset pemukul, diubah di Karya3
	public Vector2 MalletOffset { get; set; } = Vector2.Zero;

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		// 1) Gambar 7 bilah + catat posisi lingkaran kiri/kanan
		float barHeight      = 20f;
		float startX         = 0;
		float startY         = 0;
		float bottomBarWidth = 100f;
		float topBarWidth    = 60f;
		float gap            = 5f;
		float widthDiff      = bottomBarWidth - topBarWidth; // 40
		float widthStep      = widthDiff / 6f;               // ~6.67
		float xShiftPerBar   = 2f;

		float circleRadius   = 5f;
		var leftCenters      = new List<Vector2>();
		var rightCenters     = new List<Vector2>();

		for (int i = 0; i < 7; i++)
		{
			float currentW = topBarWidth + i * widthStep;
			float x        = startX - i * xShiftPerBar;
			float y        = startY + i * (barHeight + gap);

			// Gambar bilah
			var rect = new Rect2(x, y, currentW, barHeight);
			DrawRect(rect, new Color("9CBBE9"), true);

			// Lingkaran kiri
			Vector2 leftC = new Vector2(x + circleRadius, y + barHeight / 2f);
			DrawCircle(leftC, circleRadius, Colors.Black);
			leftCenters.Add(leftC);

			// Lingkaran kanan
			Vector2 rightC = new Vector2(x + currentW - circleRadius, y + barHeight / 2f);
			DrawCircle(rightC, circleRadius, Colors.Black);
			rightCenters.Add(rightC);
		}

		// 2) Gambar rod (garis) menghubungkan lingkaran kiri & kanan
		for (int i = 0; i < leftCenters.Count - 1; i++)
		{
			// Rod kiri
			DrawLine(leftCenters[i], leftCenters[i + 1], Colors.Black, 2f);
			// Rod kanan
			DrawLine(rightCenters[i], rightCenters[i + 1], Colors.Black, 2f);
		}

		// 3) Gambar dua pemukul (mallet)
		float malletRadius    = 10f;
		float handleLength    = 50f;
		float handleThickness = 10f;

		// Basis pemukul 1: posisikan di bawah bilah terakhir
		Vector2 base1 = new Vector2(
			startX + bottomBarWidth / 2f + 20f,
			startY + 6 * (barHeight + gap) + barHeight + 40f
		);
		Vector2 m1 = base1 + MalletOffset;
		DrawCircle(m1, malletRadius, new Color("F66F6F"));

		// Gagang pemukul 1
		var h1 = new Rect2(
			m1.X + malletRadius,
			m1.Y - handleThickness / 2f,
			handleLength,
			handleThickness
		);
		DrawRect(h1, new Color("3E2723"), true);

		// Basis pemukul 2 (40px di bawah pemukul 1)
		Vector2 base2 = base1 + new Vector2(0, 40f);
		Vector2 m2    = base2 + MalletOffset;
		DrawCircle(m2, malletRadius, new Color("F66F6F"));

		// Gagang pemukul 2
		var h2 = new Rect2(
			m2.X + malletRadius,
			m2.Y - handleThickness / 2f,
			handleLength,
			handleThickness
		);
		DrawRect(h2, new Color("3E2723"), true);
	}
}
