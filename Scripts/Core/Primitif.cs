namespace Godot.Core;

using Godot;
using Godot.Utils;
using System;
using System.Collections.Generic;

public partial class Primitif: RefCounted
{
	public List<Vector2> LineDDA(float xa, float ya, float xb, float yb)
	{
		float dx = xb - xa;
		float dy = yb - ya;
		float steps;
		float xIncrement;
		float yIncrement;
		float x = xa;
		float y = ya;

		List<Vector2> res = new List<Vector2>();

		if (Mathf.Abs(dx) > Mathf.Abs(dy))
		{
			steps = Mathf.Abs(dx);
		}
		else
		{
			steps = Mathf.Abs(dy);
		}

		xIncrement = dx / steps;
		yIncrement = dy / steps;

		res.Add(new Vector2(Mathf.Round(x), Mathf.Round(y)));

		for (int k = 0; k < steps; k++)
		{
			x += xIncrement;
			y += yIncrement;
			res.Add(new Vector2(Mathf.Round(x), Mathf.Round(y))); 
		}

		return res;
	}

	public List<Vector2> LineBresenham(float xa, float ya, float xb, float yb)
	{
		List<Vector2> res = new List<Vector2>();
		int x1 = (int)xa;
		int y1 = (int)ya;
		int x2 = (int)xb;
		int y2 = (int)yb;

		int dx = Math.Abs(x2 - x1);
		int dy = Math.Abs(y2 - y1);
		int sx = (x1 < x2) ? 1 : -1;
		int sy = (y1 < y2) ? 1 : -1;
		int err = dx - dy;

		while (true)
		{
			res.Add(new Vector2(x1, y1));
			if (x1 == x2 && y1 == y2) break;
			int e2 = 2 * err;
			if (e2 > -dy) { err -= dy; x1 += sx; }
			if (e2 < dx) { err += dx; y1 += sy; }
		}
		return res;
	}

	public List<Vector2> Margin()
	{
		List<Vector2> res = new List<Vector2>();
		res.AddRange(LineBresenham(ScreenUtils.MarginLeft, ScreenUtils.MarginTop, ScreenUtils.MarginRight, ScreenUtils.MarginTop));
		res.AddRange(LineBresenham(ScreenUtils.MarginLeft, ScreenUtils.MarginBottom, ScreenUtils.MarginRight, ScreenUtils.MarginBottom));
		res.AddRange(LineBresenham(ScreenUtils.MarginLeft, ScreenUtils.MarginTop, ScreenUtils.MarginLeft, ScreenUtils.MarginBottom));
		res.AddRange(LineBresenham(ScreenUtils.MarginRight, ScreenUtils.MarginTop, ScreenUtils.MarginRight, ScreenUtils.MarginBottom));
		return res;
	}

	public List<Vector2> Persegi(float x, float y, float ukuran)
	{
		List<Vector2> res = new List<Vector2>();
		res.AddRange(LineBresenham(x, y, x + ukuran, y));
		res.AddRange(LineBresenham(x + ukuran, y, x + ukuran, y + ukuran)); // No offset here
		res.AddRange(LineBresenham(x + ukuran, y + ukuran, x, y + ukuran)); // No offset here
		res.AddRange(LineBresenham(x, y + ukuran, x, y)); // No offset here
		return res;
	}

	public List<Vector2> PersegiPanjang(float x, float y, float panjang, float lebar)
	{
		List<Vector2> res = new List<Vector2>();
		res.AddRange(LineBresenham(x, y, x + panjang, y));
		res.AddRange(LineBresenham(x + panjang, y, x + panjang, y + lebar)); // No offset here
		res.AddRange(LineBresenham(x + panjang, y + lebar, x, y + lebar)); // No offset here
		res.AddRange(LineBresenham(x, y + lebar, x, y)); // No offset here
		return res;
	}

	public List<Vector2> SegitigaSiku(Vector2 titikAwal, int alas, int tinggi)
	{
		List<Vector2> res = new List<Vector2>();
		res.AddRange(LineBresenham(titikAwal.X, titikAwal.Y, titikAwal.X, titikAwal.Y + tinggi)); // sisi tinggi
		res.AddRange(LineBresenham(titikAwal.X, titikAwal.Y + tinggi, titikAwal.X + alas, titikAwal.Y + tinggi)); // sisi alas
		res.AddRange(LineBresenham(titikAwal.X, titikAwal.Y, titikAwal.X + alas, titikAwal.Y + tinggi)); // sisi miring
		return res;
	}

	public List<Vector2> TrapesiumSiku(Vector2 titikAwal, int panjangAtas, int panjangBawah, int tinggi)
	{
		List<Vector2> res = new List<Vector2>();
		res.AddRange(LineBresenham(titikAwal.X, titikAwal.Y, titikAwal.X + panjangAtas, titikAwal.Y)); // sisi atas
		res.AddRange(LineBresenham(titikAwal.X, titikAwal.Y, titikAwal.X, titikAwal.Y + tinggi)); // sisi kiri
		res.AddRange(LineBresenham(titikAwal.X, titikAwal.Y + tinggi, titikAwal.X + panjangBawah, titikAwal.Y + tinggi)); // sisi bawah
		res.AddRange(LineBresenham(titikAwal.X + panjangAtas, titikAwal.Y, titikAwal.X + panjangBawah, titikAwal.Y + tinggi)); // sisi miring
		return res;
	}

	public List<Vector2> BelahKetupat(float xMulai, float yMulai, float diagonalX, float diagonalY)
	{
		List<Vector2> res = new List<Vector2>();
		float setengahDiagonalX = diagonalX / 2;
		float setengahDiagonalY = diagonalY / 2;
		
		// Kiri ke Atas
		res.AddRange(LineBresenham(xMulai, yMulai, xMulai + setengahDiagonalX, yMulai + setengahDiagonalY));
		// Atas ke Kanan
		res.AddRange(LineBresenham(xMulai + setengahDiagonalX, yMulai + setengahDiagonalY, xMulai + diagonalX, yMulai));
		// Kanan ke Bawah
		res.AddRange(LineBresenham(xMulai + diagonalX, yMulai, xMulai + setengahDiagonalX, yMulai - setengahDiagonalY));
		// Bawah ke Kiri
		res.AddRange(LineBresenham(xMulai + setengahDiagonalX, yMulai - setengahDiagonalY, xMulai, yMulai));

		return res;
	}

	public List<Vector2> LayangLayang(float xMulai, float yMulai, float diagonalX, float diagonalY)
	{
		List<Vector2> res = new List<Vector2>();
		float setengahDiagonalX = diagonalX / 2;
		float setengahDiagonalY1 = diagonalY * 1 / 4;
		float setengahDiagonalY2 = diagonalY - setengahDiagonalY1;

		// Kiri ke Atas
		res.AddRange(LineBresenham(xMulai, yMulai, xMulai + setengahDiagonalX, yMulai + setengahDiagonalY1));
		// Atas ke Kanan
		res.AddRange(LineBresenham(xMulai + setengahDiagonalX, yMulai + setengahDiagonalY1, xMulai + diagonalX, yMulai));
		// Kanan ke Bawah
		res.AddRange(LineBresenham(xMulai + diagonalX, yMulai, xMulai + setengahDiagonalX, yMulai - setengahDiagonalY2));
		// Bawah ke Kiri
		res.AddRange(LineBresenham(xMulai + setengahDiagonalX, yMulai - setengahDiagonalY2, xMulai, yMulai));
		return res;
	}

	public List<Vector2> TrapesiumSamaKaki(Vector2 titikAwal, int panjangAtas, int panjangBawah, int tinggi)
	{
		List<Vector2> res = new List<Vector2>();
		int selisih = (panjangBawah - panjangAtas) / 2;
		int awalBawahX = Mathf.FloorToInt(titikAwal.X - selisih); // Use Mathf.FloorToInt for clarity
		res.AddRange(LineBresenham(titikAwal.X, titikAwal.Y, titikAwal.X + panjangAtas, titikAwal.Y)); // sisi atas
		res.AddRange(LineBresenham(titikAwal.X, titikAwal.Y, awalBawahX, titikAwal.Y + tinggi)); // sisi kiri
		res.AddRange(LineBresenham(awalBawahX, titikAwal.Y + tinggi, awalBawahX + panjangBawah, titikAwal.Y + tinggi)); // sisi bawah
		res.AddRange(LineBresenham(titikAwal.X + panjangAtas, titikAwal.Y, awalBawahX + panjangBawah, titikAwal.Y + tinggi)); // sisi kanan
		return res;
	}

	public List<Vector2> JajarGenjang(Vector2 titikAwal, int alas, int tinggi, int jarakBeda)
	{
		List<Vector2> res = new List<Vector2>();
		res.AddRange(LineBresenham(titikAwal.X, titikAwal.Y, titikAwal.X + alas, titikAwal.Y)); // sisi atas
		res.AddRange(LineBresenham(titikAwal.X, titikAwal.Y, titikAwal.X - jarakBeda, titikAwal.Y + tinggi)); // sisi kiri
		res.AddRange(LineBresenham(titikAwal.X + alas, titikAwal.Y, titikAwal.X - jarakBeda + alas, titikAwal.Y + tinggi)); // sisi kanan
		res.AddRange(LineBresenham(titikAwal.X - jarakBeda, titikAwal.Y + tinggi, titikAwal.X - jarakBeda + alas, titikAwal.Y + tinggi)); // sisi bawah
		return res;
	}

	public List<Vector2> CircleMidPoint(int xCenter, int yCenter, int radius)
	{
		List<Vector2> points = new List<Vector2>();
		int x = 0;
		int y = radius;
		int p = 1 - radius;

		CirclePlotPoints(xCenter, yCenter, x, y, points);

		while (x < y)
		{
			x++;
			if (p < 0)
			{
				p += 2 * x + 1;
			}
			else
			{
				y--;
				p += 2 * (x - y) + 1;
			}
			CirclePlotPoints(xCenter, yCenter, x, y, points);
		}
		return points;
	}

	private void CirclePlotPoints(int xCenter, int yCenter, int x, int y, List<Vector2> points)
	{
		points.Add(new Vector2(xCenter + x, yCenter + y));
		points.Add(new Vector2(xCenter - x, yCenter + y));
		points.Add(new Vector2(xCenter + x, yCenter - y));
		points.Add(new Vector2(xCenter - x, yCenter - y));
		points.Add(new Vector2(xCenter + y, yCenter + x));
		points.Add(new Vector2(xCenter - y, yCenter + x));
		points.Add(new Vector2(xCenter + y, yCenter - x));
		points.Add(new Vector2(xCenter - y, yCenter - x));
	}

	public List<Vector2> EllipseMidpoint(int xCenter, int yCenter, int rx, int ry)
	{
		List<Vector2> points = new List<Vector2>();

		int rx2 = rx * rx;
		int ry2 = ry * ry;
		int twoRx2 = 2 * rx2;
		int twoRy2 = 2 * ry2;
		int x = 0;
		int y = ry;
		int p;
		int px = 0;
		int py = twoRx2 * y;

		// Region 1
		p = (int)(ry2 - (rx2 * ry) + (0.25 * rx2));
		while (px < py)
		{
			EllipsePlotPoints(xCenter, yCenter, x, y, points);
			x++;
			px += twoRy2;
			if (p < 0)
			{
				p += ry2 + px;
			}
			else
			{
				y--;
				py -= twoRx2;
				p += ry2 + px - py;
			}
		}

		// Region 2
		p = (int)(ry2 * (x + 0.5) * (x + 0.5) + rx2 * (y - 1) * (y - 1) - rx2 * ry2);
		while (y >= 0)
		{
			EllipsePlotPoints(xCenter, yCenter, x, y, points);
			y--;
			py -= twoRx2;
			if (p > 0)
			{
				p += rx2 - py;
			}
			else
			{
				x++;
				px += twoRy2;
				p += rx2 - py + px;
			}
		}
		return points;
	}

	private void EllipsePlotPoints(int xCenter, int yCenter, int x, int y, List<Vector2> points)
	{
		points.Add(new Vector2(xCenter + x, yCenter + y));
		points.Add(new Vector2(xCenter - x, yCenter + y));
		points.Add(new Vector2(xCenter + x, yCenter - y));
		points.Add(new Vector2(xCenter - x, yCenter - y));
	}
	
}
