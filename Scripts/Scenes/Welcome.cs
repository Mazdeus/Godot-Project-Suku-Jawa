using Godot;
using System;

public partial class Welcome : Control  
{
	private AudioStreamPlayer _btnPlayer;
	
	public override void _Ready()
	{
		GD.Print("Test Scene Loaded");

		_btnPlayer = new AudioStreamPlayer {
			Stream = GD.Load<AudioStream>("res://Assets/Audio/Btn2.mp3")
		};

		// instead of Root.AddChild(_btnPlayer):
		GetTree().Root.CallDeferred("add_child", _btnPlayer);
	}

	private void PlayBtnSound() 
	{
		_btnPlayer.Play();
	}
	
	private void _on_BtnKarya1_pressed()
	{   
		PlayBtnSound();
		if (GetTree().ChangeSceneToFile("res://Scenes/Karya1.tscn") != Error.Ok)
		{
			GD.Print("Scene Tidak Ada");
		}
	}

	private void _on_BtnKarya2_pressed()
	{
		PlayBtnSound();
		if (GetTree().ChangeSceneToFile("res://Scenes/Karya2.tscn") != Error.Ok)
		{
			GD.Print("Scene Tidak Ada");
		}
	}

	private void _on_BtnKarya3_pressed()
	{
		PlayBtnSound();
		if (GetTree().ChangeSceneToFile("res://Scenes/Karya3.tscn") != Error.Ok)
		{
			GD.Print("Scene Tidak Ada");
		}
	}

	private void _on_BtnKarya4_pressed()
	{
		PlayBtnSound();
		if (GetTree().ChangeSceneToFile("res://Scenes/Karya4.tscn") != Error.Ok)
		{
			GD.Print("Scene Tidak Ada");
		}
	}

	private void _on_BtnAbout_pressed()
	{
		PlayBtnSound();
		if (GetTree().ChangeSceneToFile("res://Scenes/About.tscn") != Error.Ok)
		{
			GD.Print("Scene Tidak Ada");
		}
	}

	private void _on_BtnGuide_pressed()
	{
		PlayBtnSound();
		if (GetTree().ChangeSceneToFile("res://Scenes/Guide.tscn") != Error.Ok)
		{
			GD.Print("Scene Tidak Ada");
		}
	}

	private void _on_BtnExit_pressed()
	{
		PlayBtnSound();
		GetTree().Quit();
	}
}
