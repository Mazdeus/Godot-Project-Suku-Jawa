namespace Godot.Scene;

using Godot;
using System;

public partial class BtnBack : Button
{
	private AudioStreamPlayer _btnPlayer;

	public override void _Ready()
	{
		_btnPlayer = new AudioStreamPlayer {
			Stream = GD.Load<AudioStream>("res://Assets/Audio/Btn.mp3")
		};
		CallDeferred("add_child", _btnPlayer);
	}

	private void _on_BtnBack_pressed()
	{
		_btnPlayer.Play();

		// schedule scene‑change 0.2s later
		var t = GetTree().CreateTimer(0.2f);
		t.Timeout += () => {
			GetTree().ChangeSceneToFile("res://Scenes/Welcome.tscn");
		};
	}
}
