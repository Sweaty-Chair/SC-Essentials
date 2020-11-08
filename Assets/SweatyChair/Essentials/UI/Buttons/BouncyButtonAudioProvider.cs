using UnityEngine;

namespace SweatyChair.UI
{
	/// <summary>
	/// Abstract class to inherit off of when needing a custom solution to play Sound Effects using Bouncy Button.
	/// </summary>
	public abstract class BouncyButtonAudioProvider : ScriptableObject
	{

		#region Play Audio Methods

		public abstract void PlaySFX();

		#endregion

	}

}
