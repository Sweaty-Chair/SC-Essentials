using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace SweatyChair
{

	public static class TransitionManager
	{

		#region Actions

		public static event UnityAction<Transition> transitionShowEvent;

		#endregion

		#region Constructor

		static TransitionManager() { }

		#endregion

		#region Show Transition

		public static void Show(Transition transition)
		{
			//Then call our event to initialize our transition
			transitionShowEvent?.Invoke(transition);

			if (transitionShowEvent == null)
				Debug.LogError("[Transition Manager] - Unable to use transition panel, as there is nothing listening to our Show Event. Make sure we have a Transition Panel in the scene.");
		}

		#endregion

		#region Default Transitions

		public static void FadeTransition(float duration, Priority priority, bool fadeOut, TransitionVisual transitionImage, Color colour, EasingFunction.Ease easingType, UnityAction onCompleteCallback = null, bool realtime = false)
		{
			FadeTransition(duration, (int)priority, fadeOut, transitionImage, colour, easingType, onCompleteCallback, realtime);
		}

		public static void FadeTransition(float duration, int priority, bool fadeOut, TransitionVisual transitionImage, Color colour, EasingFunction.Ease easingType, UnityAction onCompleteCallback = null, bool realtime = false)
		{
			new Transition() {
				//General
				type = TransitionType.Default,
				priority = priority,
				duration = duration,
				fadeOut = fadeOut,
				transitionImage = transitionImage,
				transitionColour = colour,
				easeType = easingType,
				onCompleted = onCompleteCallback,
				realtime = realtime
			}.Show();
		}


		public static void FadeInOutTransition(float duration, Priority priority, TransitionVisual transitionImage, Color colour, EasingFunction.Ease easingType, UnityAction onHalfwayCallback = null, UnityAction onCompleteCallback = null, bool realtime = false)
		{
			FadeInOutTransition(duration, (int)priority, transitionImage, colour, easingType, onHalfwayCallback, onCompleteCallback, realtime);
		}

		public static void FadeInOutTransition(float duration, int priority, TransitionVisual transitionImage, Color colour, EasingFunction.Ease easingType, UnityAction onHalfwayCallback = null, UnityAction onCompleteCallback = null, bool realtime = false)
		{
			new Transition {
				//General
				type = TransitionType.Flash,
				priority = priority,
				duration = duration,
				transitionImage = transitionImage,
				transitionColour = colour,
				easeType = easingType,
				onCompleted = onCompleteCallback,
				realtime = realtime,

				//Flash Transition
				onHalfway = onHalfwayCallback

			}.Show();
		}


		public static void LoadSceneTransition(string sceneName, LoadSceneMode loadMode, float duration, TransitionVisual transitionImage, Color colour, EasingFunction.Ease easingType, UnityAction onSceneLoadedCallback = null, UnityAction onCompletedCallback = null, bool realtime = false)
		{
			new Transition {
				//General
				type = TransitionType.SceneChange,
				priority = (int)Priority.Highest,
				duration = duration,
				transitionImage = transitionImage,
				transitionColour = colour,
				easeType = easingType,
				realtime = realtime,

				//Scene Stuff
				sceneName = sceneName,
				loadSceneMode = loadMode,
				onSceneLoaded = onSceneLoadedCallback,
				onCompleted = onCompletedCallback

			}.Show();
		}

		public static void FadeEnumeratorTransition(float duration, Priority priority, TransitionVisual transitionImage, Color colour, EasingFunction.Ease easingType, IEnumerator yieldRoutine, UnityAction onCompleteCallback = null, bool realtime = false)
		{
			FadeEnumeratorTransition(duration, (int)priority, transitionImage, colour, easingType, yieldRoutine, onCompleteCallback, realtime);
		}

		public static void FadeEnumeratorTransition(float duration, int priority, TransitionVisual transitionImage, Color colour, EasingFunction.Ease easingType, IEnumerator yieldRoutine, UnityAction onCompleteCallback = null, bool realtime = false)
		{
			new Transition {
				//General
				type = TransitionType.IEnumerator,
				priority = priority,
				duration = duration,
				transitionImage = transitionImage,
				transitionColour = colour,
				easeType = easingType,
				onCompleted = onCompleteCallback,
				realtime = realtime,

				//Enumerator Transition
				yieldRoutine = yieldRoutine

			}.Show();
		}

		#endregion

	}
}
