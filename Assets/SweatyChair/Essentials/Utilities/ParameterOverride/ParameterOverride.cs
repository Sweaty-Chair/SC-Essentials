namespace SweatyChair
{
	/// <summary>
	/// The base abstract class for all parameter override types.
	/// </summary>
	/// <seealso cref="ParameterOverride{T}"/>
	public abstract class ParameterOverride
	{
		/// <summary>
		/// The override state of this parameter.
		/// </summary>
		public bool overrideState;

		/// <summary>
		/// Returns the computed hash code for this parameter.
		/// </summary>
		/// <returns>A computed hash code</returns>
		public abstract int GetHash();

		/// <summary>
		/// Casts and returns the value stored in this parameter.
		/// </summary>
		/// <typeparam name="T">The type to cast to</typeparam>
		/// <returns>The value stored in this parameter</returns>
		public T GetValue<T>()
		{
			return ((ParameterOverride<T>)this).value;
		}

		/// <summary>
		/// This method is called right after the parent <see cref="PostProcessEffectSettings"/> has
		/// been initialized. This is used in case you need to access fields or properties that
		/// can't be accessed in the constructor of a <see cref="ScriptableObject"/>
		/// (<c>ParameterOverride</c> objects are generally declared and initialized in a
		/// <see cref="PostProcessEffectSettings"/>).
		/// </summary>
		/// <seealso cref="OnDisable"/>
		protected internal virtual void OnEnable()
		{
		}

		/// <summary>
		/// This method is called right before the parent <see cref="PostProcessEffectSettings"/>
		/// gets de-initialized.
		/// </summary>
		/// <seealso cref="OnEnable"/>
		protected internal virtual void OnDisable()
		{
		}

		internal abstract void SetValue(ParameterOverride parameter);
	}

	/// <summary>
	/// The base typed class for all parameter override types.
	/// </summary>
	/// <typeparam name="T">The type of value to store in this <c>ParameterOverride</c></typeparam>
	/// <remarks>
	/// Due to limitations with the serialization system in Unity you shouldn't use this class
	/// directly. Use one of the pre-flatten types (like <see cref="FloatParameter"/> or make your
	/// own by extending this class.
	/// </remarks>
	/// <example>
	/// This sample code shows how to make a custom parameter holding a <c>float</c>.
	/// <code>
	/// [Serializable]
	/// public sealed class FloatParameter : ParameterOverride&lt;float&gt;
	/// {
	///     public override void Interp(float from, float to, float t)
	///     {
	///         value = from + (to - from) * t;
	///     }
	/// }
	/// </code>
	/// </example>
	public abstract class ParameterOverride<T> : ParameterOverride
	{
		/// <summary>
		/// The value stored in this parameter.
		/// </summary>
		public T value;

		/// <summary>
		/// Creates a <c>ParameterOverride</c> with a default <see cref="value"/> and
		/// <see cref="ParameterOverride.overrideState"/> set to <c>false</c>.
		/// </summary>
		public ParameterOverride() : this(default(T), false) { }

		/// <summary>
		/// Creates a <c>ParameterOverride</c> with a given value and
		/// <see cref="ParameterOverride.overrideState"/> set to <c>false</c>.
		/// </summary>
		/// <param name="value">The value to set this parameter to</param>
		public ParameterOverride(T value) : this(value, false) { }

		/// <summary>
		/// Creates a <c>ParameterOverride</c> with a given value and override state.
		/// </summary>
		/// <param name="value">The value to set this parameter to</param>
		/// <param name="overrideState">The override state for this value</param>
		public ParameterOverride(T value, bool overrideState)
		{
			this.value = value;
			this.overrideState = overrideState;
		}

		/// <summary>
		/// Sets the value for this parameter to <paramref name="x"/> and mark the override state
		/// to <c>true</c>.
		/// </summary>
		/// <param name="x"></param>
		public void Override(T x)
		{
			overrideState = true;
			value = x;
		}

		internal override void SetValue(ParameterOverride parameter)
		{
			value = parameter.GetValue<T>();
		}

		/// <summary>
		/// Returns the computed hash code for this parameter.
		/// </summary>
		/// <returns>A computed hash code</returns>
		public override int GetHash()
		{
			unchecked {
				int hash = 17;
				hash = hash * 23 + overrideState.GetHashCode();
				hash = hash * 23 + value.GetHashCode();
				return hash;
			}
		}

		/// <summary>
		/// Implicit conversion between <see cref="ParameterOverride{T}"/> and its value type.
		/// </summary>
		/// <param name="prop">The parameter to implicitly cast</param>
		/// <returns>A value of type <typeparam name="T">.</typeparam></returns>
		public static implicit operator T(ParameterOverride<T> prop)
		{
			return prop.value;
		}

		#region Utility

		public override string ToString()
		{
			return $"{GetType()}: overrideState={overrideState}, value={value}";
		}

		#endregion
	}

}