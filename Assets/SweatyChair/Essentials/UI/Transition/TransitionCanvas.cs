using SweatyChair.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair
{

	public class TransitionCanvas : MonoBehaviour
	{

		#region Variables

		[Header("General")]
		[SerializeField] private UIFader _canvasFader;
		[SerializeField] private Image _mainImage;

		private Transition _transitionData;

		#endregion

		#region Set

		public void Set(Transition transition)
		{
			_transitionData = transition;

			if (_transitionData != null) {

				// Ensure we are enabled and have correct ordering
				gameObject.SetActive(true);
				GetComponent<Canvas>().sortingOrder = transition.priority;

				// Set our UI Colours
				if (_mainImage != null)
					_mainImage.color = _transitionData.transitionColour;

				// Then Begin our Transitions
				switch (transition.type) {

				case TransitionType.Default:
					BeginDefault();
					break;

				case TransitionType.IEnumerator:
					BeginIEnumeratorTransition();
					break;

				case TransitionType.SceneChange:
					BeginSceneChange();
					break;

				case TransitionType.Flash:
					BeginFlash();
					break;

				default:
					Debug.LogErrorFormat("[Transition Canvas] - Unable to play Transition for TransitionType '{0}', We do not have a case for this");
					BeginDefault();
					break;
				}
			} else {
				// Destroy ourselves
				Destroy(gameObject);
			}
		}

		#endregion

		#region Default Transition

		protected void BeginDefault()
		{
			StartCoroutine(PlayDefaultTransitionRoutine());
		}

		private IEnumerator PlayDefaultTransitionRoutine()
		{
			// Set our Canvas Fader to the opposite immediately so we have correct starting values
			_canvasFader.Fade(!_transitionData.fadeOut, true);

			_transitionData.onBegan?.Invoke();

			// Begin our proper Fade
			_canvasFader.Fade(_transitionData.fadeOut, _transitionData.duration, _transitionData.easeType, _transitionData.realtime);

			// Wait for our timer
			yield return WaitForSeconds(_transitionData.duration, _transitionData.realtime);

			// Then Invoke our complete event, then Destroy ourselves as we are no longer needed
			_transitionData.onCompleted?.Invoke();

			Destroy(gameObject); // We then destroy self
		}

		#endregion

		#region IEnumerator Transition

		protected void BeginIEnumeratorTransition()
		{
			StartGenericIEnumeratorTransition(OnIEnumeratorTransitionBegin(), OnIEnumeratorTransitionHalfway(), OnIEnumeratorTransitionComplete());
		}

		protected IEnumerator OnIEnumeratorTransitionBegin()
		{
			_transitionData.onBegan?.Invoke();
			yield break;
		}

		protected IEnumerator OnIEnumeratorTransitionHalfway()
		{
			yield return _transitionData.yieldRoutine;
			yield break;
		}

		protected IEnumerator OnIEnumeratorTransitionComplete()
		{
			_transitionData.onCompleted?.Invoke();
			yield break;
		}

		#endregion

		#region Scene Load Transition

		protected void BeginSceneChange()
		{
			StartGenericIEnumeratorTransition(OnSceneTransitionBegin(), OnSceneTranstionHalfway(), OnSceneTransitionComplete());
		}

		private IEnumerator OnSceneTransitionBegin()
		{
			_transitionData.onBegan?.Invoke();
			yield break;
		}

		private IEnumerator OnSceneTranstionHalfway()
		{
			yield return GameSceneManager.LoadSceneAsync(_transitionData.sceneName, _transitionData.loadSceneMode);
			_transitionData.onSceneLoaded?.Invoke();
			yield break;
		}

		private IEnumerator OnSceneTransitionComplete()
		{
			_transitionData.onCompleted?.Invoke();
			yield break;
		}

		#endregion

		#region Flash Transition

		protected void BeginFlash()
		{
			StartGenericIEnumeratorTransition(OnBeginFlashRoutine(), OnHalfwayFlashRoutine(), OnFinishFlashRoutine());
		}

		private IEnumerator OnBeginFlashRoutine()
		{
			_transitionData.onBegan?.Invoke();
			yield break;
		}

		private IEnumerator OnHalfwayFlashRoutine()
		{
			_transitionData.onHalfway?.Invoke();
			yield break;
		}

		private IEnumerator OnFinishFlashRoutine()
		{
			_transitionData.onCompleted?.Invoke();
			yield break;
		}

		#endregion

		#region Generic IEnumerator Routine

		private void StartGenericIEnumeratorTransition(IEnumerator onBeginYield, IEnumerator onHalfwayYield, IEnumerator onCompleteYield)
		{
			StartCoroutine(PlayGenericIEnumeratorTransitionRoutine(onBeginYield, onHalfwayYield, onCompleteYield));
		}
		private IEnumerator PlayGenericIEnumeratorTransitionRoutine(IEnumerator onBeginYield, IEnumerator onHalfwayYield, IEnumerator onCompleteYield)
		{
			float halfTransitionDuration = _transitionData.duration / 2f;
			_canvasFader.Fade(true, true);

			// yield our initial data
			if (onBeginYield != null)
				yield return onBeginYield;

			_canvasFader.Fade(false, halfTransitionDuration, _transitionData.easeType, _transitionData.realtime);
			yield return WaitForSeconds(halfTransitionDuration, _transitionData.realtime);

			// yield our initial data
			if (onHalfwayYield != null)
				yield return onHalfwayYield;


			// Then fade our object back out
			_canvasFader.Fade(true, halfTransitionDuration, _transitionData.easeType, _transitionData.realtime);
			yield return WaitForSeconds(halfTransitionDuration, _transitionData.realtime);


			// yield our initial data
			if (onCompleteYield != null)
				yield return onCompleteYield;


			Destroy(gameObject); // We then destroy ourselves
		}

		#endregion

		#region Utility

		private IEnumerator WaitForSeconds(float duration, bool realtime)
		{
			if (realtime)
				yield return new WaitForSecondsRealtime(duration);
			else
				yield return new WaitForSeconds(duration);
		}

		#endregion

	}

}