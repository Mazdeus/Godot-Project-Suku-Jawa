namespace Godot.Utils;

using Godot;
using System;
using System.Numerics;

public static class MatrixUtils
{
	public static Matrix4x4 CreateTranslation(float x, float y, float z = 0f)
	{
		Matrix4x4 matrix = Matrix4x4.Identity;
		matrix.M14 = x;
		matrix.M24 = y;
		return matrix;
	}

	public static Matrix4x4 CreateScale(float x, float y)
	{
		Matrix4x4 matrix = Matrix4x4.Identity;
		matrix.M11 = x;
		matrix.M22 = y;
		return matrix;
	}

	public static Matrix4x4 CreateRotationZ(float radians)
	{
		Matrix4x4 matrix = Matrix4x4.Identity;
		matrix.M11 = MathF.Cos(radians);
		matrix.M12 = MathF.Sin(radians);
		matrix.M21 = -MathF.Sin(radians);
		matrix.M22 = MathF.Cos(radians);
		return matrix;
	}

	// Add other matrix utility functions as needed
	// For example:
	public static Matrix4x4 CreateShearing(float x, float y)
	{
		Matrix4x4 matrix = Matrix4x4.Identity;
		matrix.M12 = x;
		matrix.M21 = y;
		return matrix;
	}

	public static Matrix4x4 CreateReflectionX()
	{
		Matrix4x4 matrix = Matrix4x4.Identity;
		matrix.M22 = -1;
		return matrix;
	}

	public static Matrix4x4 CreateReflectionY()
	{
		Matrix4x4 matrix = Matrix4x4.Identity;
		matrix.M11 = -1;
		return matrix;
	}

	public static Matrix4x4 CreateReflectionOrigin()
	{
		Matrix4x4 matrix = Matrix4x4.Identity;
		matrix.M11 = -1;
		matrix.M22 = -1;
		return matrix;
	}
}
