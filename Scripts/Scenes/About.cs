// About.cs
using Godot;
using System;

public partial class About : Control
{
	public override void _Ready()
	{
		// Initialization code here
	}

	private void _on_BtnBack_pressed()
	{
		if (GetTree().ChangeSceneToFile("res://Scenes/Welcome.tscn") != Error.Ok)
		{
			GD.Print("Scene Tidak Ada");
		}
	}
}
