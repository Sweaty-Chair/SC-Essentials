using UnityEditor;

namespace SweatyChair
{

	[CustomPropertyDrawer(typeof(StringParameter))]
	public sealed class StringParameterDrawer : ParameterOverrideDrawer { }

	[CustomPropertyDrawer(typeof(FloatParameter))]
	public sealed class FloatParameterDrawer : ParameterOverrideDrawer { }

	[CustomPropertyDrawer(typeof(IntParameter))]
	public sealed class IntParameterDrawer : ParameterOverrideDrawer { }

	[CustomPropertyDrawer(typeof(BoolParameter))]
	public sealed class BoolParameterDrawer : ParameterOverrideDrawer { }

	[CustomPropertyDrawer(typeof(ColorParameter))]
	public sealed class ColorParameterDrawer : ParameterOverrideDrawer { }

	[CustomPropertyDrawer(typeof(Vector2Parameter))]
	public sealed class Vector2ParameterDrawer : ParameterOverrideDrawer { }

	[CustomPropertyDrawer(typeof(Vector3Parameter))]
	public sealed class Vector3ParameterDrawer : ParameterOverrideDrawer { }

	[CustomPropertyDrawer(typeof(Vector4Parameter))]
	public sealed class Vector4ParameterDrawer : ParameterOverrideDrawer { }

	[CustomPropertyDrawer(typeof(AnimationCurveParameter))]
	public sealed class AnimationCurveParameterDrawer : ParameterOverrideDrawer { }

	[CustomPropertyDrawer(typeof(TextureParameter))]
	public sealed class TextureParameterDrawer : ParameterOverrideDrawer { }

	[CustomPropertyDrawer(typeof(GameObjectParameter))]
	public sealed class GameObjectParameterDrawer : ParameterOverrideDrawer { }

}
