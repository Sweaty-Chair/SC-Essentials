// A Shameless code rip from Unitys' Post processing system, In cases which we do not have / want access to that package

using System;
using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// A <see cref="ParameterOverride{T}"/> that holds a <c>string</c> value.
	/// </summary>
	[Serializable]
	public sealed class StringParameter : ParameterOverride<string> { }

	/// <summary>
	/// A <see cref="ParameterOverride{T}"/> that holds a <c>float</c> value.
	/// </summary>
	[Serializable]
	public sealed class FloatParameter : ParameterOverride<float> { }

	/// <summary>
	/// A <see cref="ParameterOverride{T}"/> that holds a <c>int</c> value.
	/// </summary>
	[Serializable]
	public sealed class IntParameter : ParameterOverride<int> { }

	/// <summary>
	/// A <see cref="ParameterOverride{T}"/> that holds a <c>bool</c> value.
	/// </summary>
	[Serializable]
	public sealed class BoolParameter : ParameterOverride<bool> { }

	/// <summary>
	/// A <see cref="ParameterOverride{T}"/> that holds a <see cref="Color"/> value.
	/// </summary>
	[Serializable]
	public sealed class ColorParameter : ParameterOverride<Color> { }

	/// <summary>
	/// A <see cref="ParameterOverride{T}"/> that holds a <see cref="Vector2"/> value.
	/// </summary>
	[Serializable]
	public sealed class Vector2Parameter : ParameterOverride<Vector2> { }

	/// <summary>
	/// A <see cref="ParameterOverride{T}"/> that holds a <see cref="Vector3"/> value.
	/// </summary>
	[Serializable]
	public sealed class Vector3Parameter : ParameterOverride<Vector3> { }

	/// <summary>
	/// A <see cref="ParameterOverride{T}"/> that holds a <see cref="Vector4"/> value.
	/// </summary>
	[Serializable]
	public sealed class Vector4Parameter : ParameterOverride<Vector4> { }

	/// <summary>
	/// A <see cref="ParameterOverride{T}"/> that holds a <see cref="AnimationCurve"/> value.
	/// </summary>
	[Serializable]
	public sealed class AnimationCurveParameter : ParameterOverride<AnimationCurve> { }

	/// <summary>
	/// A <see cref="ParameterOverride{T}"/> that holds a <see cref="Texture"/> value.
	/// </summary>
	[Serializable]
	public sealed class TextureParameter : ParameterOverride<Texture> { }

	/// <summary>
	/// A <see cref="ParameterOverride{T}"/> that holds a <see cref="GameObject"/> value.
	/// </summary>
	[Serializable]
	public sealed class GameObjectParameter : ParameterOverride<GameObject> { }

}