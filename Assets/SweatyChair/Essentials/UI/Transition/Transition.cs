using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace SweatyChair
{

	public enum TransitionType
	{
		Default,
		SceneChange,
		Flash,
		IEnumerator
	}

	public enum TransitionVisual
	{
		FullFill,
		Vignette,
		AnimatedPanel,
	}

	/// <summary>
	/// GameData Instance for a stackable Transition Effect which can load scenes / Fade full Panels in and out
	/// </summary>
	public class Transition
	{

		#region Generic Variables

		public TransitionType type = TransitionType.Default;
		public int priority = (int)Priority.None;       //We keep priority as an int as it allows us to have finer control over priorities such as between medium and high (5-8)

		//Base Transition Variables
		public TransitionVisual transitionImage = TransitionVisual.FullFill;
		public bool fadeOut = true;

		// Visual Settings
		public float duration = 1;
		public bool realtime = false;

		public Color transitionColour = Color.white;
		public EasingFunction.Ease easeType = EasingFunction.Ease.Linear;

		//Actions
		public UnityAction onBegan = null;
		public UnityAction onCompleted = null;

		#endregion

		#region Scene Transition Variables

		public string sceneName = string.Empty;
		public LoadSceneMode loadSceneMode = LoadSceneMode.Single;

		//Action
		public UnityAction onSceneLoaded = null;

		#endregion

		#region Flash Transition Variables

		public UnityAction onHalfway = null;

		#endregion

		#region IEnumerator Transition Variables

		public IEnumerator yieldRoutine = null;

		#endregion

		#region Show

		public void Show()
		{
			TransitionManager.Show(this);
		}

		#endregion

		#region Utility

		public override string ToString()
		{
			return string.Format("[Transition: type={0}, priority={1}, duration={2}, fadeOut={3}, transitionColour={4}, realtime={5}, onBeginAction={6}, onCompleteAction={7}, sceneName={8}, loadSceneMode={9}, onSceneLoadedAction={10}, onHalfwayAction={11}]", type, priority, duration, fadeOut, transitionColour, realtime, onBegan, onCompleted, sceneName, loadSceneMode, onSceneLoaded, onHalfway);
		}

		#endregion

	}

}