namespace Godot.Utils;

using Godot;
using Godot.Core;
using System.Collections.Generic;

public static class NodeUtils
{
	public static void DisposeAndNull(RefCounted obj, string objName)
	{
		GD.Print($"{objName} is null: {obj == null}");
		obj?.Dispose();
		obj = null;
		GD.Print($"{objName} is null after dispose and null: {obj == null}");
	}

	public static void RemoveAndDisposeNode(Node parent, Node child)
	{
		if (parent != null && child != null)
		{
			parent.RemoveChild(child);
			child.Dispose();
			child = null; // This won't affect the original reference, but it's good practice
		}
	}

	public static List<Vector2> CheckPrimitif(Primitif primitif)
	{
		if (primitif == null)
		{
			GD.PrintErr("Node Primitif belum di-assign!");
			return new List<Vector2>();
		}
		return null; // Indicate that primitif is not null
	}
}
