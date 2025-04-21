namespace Godot.Core;

using Godot;
using Godot.Utils;
using System;
using System.Collections.Generic;
using System.Numerics; // Import System.Numerics for Matrix4x4

public partial class TransformasiFast : RefCounted
{
	// Use Matrix4x4 from System.Numerics
	private List<Godot.Vector2> _transformedPoints = new List<Godot.Vector2>(); 

	public static Matrix4x4 Identity()
	{
		return Matrix4x4.Identity;
	}

	public List<Godot.Vector2> GetTransformPoint(Matrix4x4 m, List<Godot.Vector2> res)
	{
		if (res == null)
		{
			GD.PrintErr("Error: res is null in GetTransformPoint");
			return new List<Godot.Vector2>(); // Or handle the error appropriately
		}
		_transformedPoints.Clear();
		foreach (Godot.Vector2 point in res)
		{
			// Convert Godot.Vector2 to Godot.Vector3 with W=1
			Godot.Vector3 tempPoint = new Godot.Vector3(point.X, point.Y, 1);

			// PrintUtils.PrintMatrix(m, "Matrix before multiplication:"); // Debug the matrix

			// Manually perform matrix-vector multiplication
			Godot.Vector3 transformedPoint3D = new Godot.Vector3(
				m.M11 * tempPoint.X + m.M12 * tempPoint.Y + m.M13 * tempPoint.Z + m.M14,
				m.M21 * tempPoint.X + m.M22 * tempPoint.Y + m.M23 * tempPoint.Z + m.M24,
				m.M31 * tempPoint.X + m.M32 * tempPoint.Y + m.M33 * tempPoint.Z + m.M34
			);

			// PrintUtils.PrintGodot.Vector3(transformedPoint3D, "Transformed Point 3D");

			// Convert back to Godot.Vector2
			Godot.Vector2 transformedPoint = new Godot.Vector2(transformedPoint3D.X, transformedPoint3D.Y);

			_transformedPoints.Add(transformedPoint);
		}
		return new List<Godot.Vector2>(_transformedPoints);
	}

	public void Translation(ref Matrix4x4 matrix, float x, float y)
	{
		matrix = MatrixUtils.CreateTranslation(x, y) * matrix; // Pre-multiplication
	}

	public void Scaling(ref Matrix4x4 matrix, float x, float y, Godot.Vector2 coord)
	{
		if (coord.X != 0 && coord.Y != 0)
		{
			Translation(ref matrix, -coord.X, -coord.Y);
			matrix = MatrixUtils.CreateScale(x, y) * matrix; // Pre-multiplication
			Translation(ref matrix, coord.X, coord.Y);
		}
		else
		{
			matrix = MatrixUtils.CreateScale(x, y) * matrix; // Pre-multiplication
		}
	}

	public void RotationClockwise(ref Matrix4x4 matrix, float radians, Godot.Vector2 coord) // Use radians directly
	{
		if (coord.X != 0 && coord.Y != 0)
		{
			Translation(ref matrix, -coord.X, -coord.Y);
			matrix = MatrixUtils.CreateRotationZ(radians) * matrix; // Pre-multiplication
			Translation(ref matrix, coord.X, coord.Y);
		}
		else
		{
			matrix = MatrixUtils.CreateRotationZ(radians) * matrix; // Pre-multiplication
		}
	}

	public void RotationCounterClockwise(ref Matrix4x4 matrix, float radians, Godot.Vector2 coord) // Use radians directly
	{
		if (coord.X != 0 && coord.Y != 0)
		{
			Translation(ref matrix, -coord.X, -coord.Y);
			matrix = MatrixUtils.CreateRotationZ(-radians) * matrix; // Pre-multiplication
			Translation(ref matrix, coord.X, coord.Y);
		}
		else
		{
			matrix = MatrixUtils.CreateRotationZ(-radians) * matrix; // Pre-multiplication
		}
	}

	public void Shearing(ref Matrix4x4 matrix, float x, float y, Godot.Vector2 coord)
	{
		Matrix4x4 shearingMatrix = Matrix4x4.Identity;
		shearingMatrix.M12 = x;
		shearingMatrix.M21 = y;

		if (coord.X != 0 && coord.Y != 0)
		{
			Translation(ref matrix, -coord.X, -coord.Y);
			matrix = shearingMatrix * matrix; // Pre-multiplication
			Translation(ref matrix, coord.X, coord.Y);
		}
		else
		{
			matrix = shearingMatrix * matrix; // Pre-multiplication
		}
	}

	public void ReflectionToX(ref Matrix4x4 matrix) // Remove ref Godot.Vector2 coord
	{
		Matrix4x4 reflectionMatrix = Matrix4x4.Identity;
		reflectionMatrix.M22 = -1; // Reflection along x-axis
		matrix = reflectionMatrix * matrix; // Pre-multiplication
		// Remove: coord.Y = -coord.Y;
	}

	public void ReflectionToY(ref Matrix4x4 matrix)  // Remove ref Godot.Vector2 coord
	{
		Matrix4x4 reflectionMatrix = Matrix4x4.Identity;
		reflectionMatrix.M11 = -1; // Reflection along y-axis
		matrix = reflectionMatrix * matrix; // Pre-multiplication
		// Remove: coord.X = -coord.X; 
	}

	public void ReflectionToOrigin(ref Matrix4x4 matrix) // Remove ref Godot.Vector2 coord
	{
		Matrix4x4 reflectionMatrix = Matrix4x4.Identity;
		reflectionMatrix.M11 = -1; // Reflection through origin
		reflectionMatrix.M22 = -1; 
		matrix = reflectionMatrix * matrix; // Pre-multiplication
		// Remove: coord.X = -coord.X; 
		// Remove: coord.Y = -coord.Y;
	}
}
