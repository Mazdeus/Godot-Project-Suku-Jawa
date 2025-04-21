// File: Scripts/Scenes/Karya1.cs
// Scene: 050_Motif2D
//
// – Empat motif hitam‑putih bertema budaya Jawa:
//     1) Batik Kawung        2) Stupa Borobudur
//     3) Gong Sebul          4) Gambang
// – Hanya men‑render bentuk‑bentuk dasar (garis & titik) tanpa animasi.
// – Tombol UI / hot‑key memilih motif yang ingin ditampilkan.
// --------------------------------------------------------------------


using Godot;
using System;
using Godot.Core;
using Godot.Utils;

public partial class Karya1 : Node2D
{
	// Referensi ke skrip motif
	private batikKawung _batikKawung;
	private stupaBorobudur _stupaBorobudur;
	private gongSebul _gongSebul;
	private gambang _gambang;
	
	// Untuk menggambar margin
	private bentukdasar _bentukDasar;

	public override void _Ready()
	{
		ScreenUtils.Initialize(GetViewport());

		_bentukDasar = new bentukdasar();

		// Instansiasi BatikKawung
		_batikKawung = new batikKawung();
		AddChild(_batikKawung);

		// Instansiasi Stupa Borobudur
		_stupaBorobudur = new stupaBorobudur();
		AddChild(_stupaBorobudur);

		// Instansiasi GongSebul (motif baru)
		_gongSebul = new gongSebul();
		AddChild(_gongSebul);
		
		// Inisialisasi Gambang
		_gambang = new gambang();
		AddChild(_gambang);
		
		// Tampilkan batik kawung secara default
		_batikKawung.Visible = true;
		_stupaBorobudur.Visible = false;
		_gongSebul.Visible = false;
		_gambang.Visible = false;
	}

	public override void _Process(double delta)
	{
		// Gunakan QueueRedraw() untuk menggambar ulang
		this.QueueRedraw();
	}

	public override void _Draw()
	{
		// Gambar margin (batas) layar
		var marginPoints = _bentukDasar.Margin();
		GraphicsUtils.PutPixelAll(this, marginPoints, GraphicsUtils.DrawStyle.DotDot, Colors.Black);
	}

	private void _on_BtnBatik_pressed()
	{
		_batikKawung.Visible = true;
		_stupaBorobudur.Visible = false;
		_gongSebul.Visible = false;
		_gambang.Visible = false;
	}

	private void _on_BtnStupa_pressed()
	{
		_batikKawung.Visible = false;
		_stupaBorobudur.Visible = true;
		_gongSebul.Visible = false;
		_gambang.Visible = false;
	}

	private void _on_BtnGong_pressed()
	{
		_batikKawung.Visible = false;
		_stupaBorobudur.Visible = false;
		_gongSebul.Visible = true;
		_gambang.Visible = false;
	}
	
	private void _on_BtnGambang_pressed()
	{
		_batikKawung.Visible = false;
		_stupaBorobudur.Visible = false;
		_gongSebul.Visible = false;
		_gambang.Visible = true;
	}
}
