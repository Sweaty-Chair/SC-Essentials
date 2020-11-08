using SweatyChair.UI;
using UnityEngine;

namespace SweatyChair
{

	public class TransitionPanel : SingletonPanel<TransitionPanel>
	{

		#region Variables

		[Header("Required")]
		[SerializeField] private Transform _holderObject = null;

		[Header("Transition Objects")]
		[SerializeField] private TransitionCanvas _defaultTransitionObj = null;
		[SerializeField] private TransitionCanvas _vignetteTransitionObj = null;
		[SerializeField] private TransitionCanvas _animatedTransitionObj = null;

		[Header("Settings")]
		[SerializeField] private bool _debugMode = false;

		#endregion

		#region OnAwake / Destroy

		public void Awake()
		{
			// Init our selves
			Init();
		}

		public void OnDestroy()
		{
			Reset();
		}

		#endregion

		#region Init / Reset

		public override void Init()
		{
			base.Init();
			DontDestroyOnLoad(gameObject);

			//Subscribe to events
			TransitionManager.transitionShowEvent -= Show;
			TransitionManager.transitionShowEvent += Show;

			// Reset all of our Child Panels, to ensure they are all hidden
			_defaultTransitionObj.gameObject.SetActive(false);
			_vignetteTransitionObj.gameObject.SetActive(false);
			_animatedTransitionObj.gameObject.SetActive(false);
		}

		public override void Reset()
		{
			base.Reset();

			TransitionManager.transitionShowEvent -= Show;
		}

		#endregion

		#region Show Panel

		public void Show(Transition transition)
		{
			TransitionCanvas transitionInstance;

			switch (transition.transitionImage) {
				case TransitionVisual.Vignette:
					transitionInstance = Instantiate(_vignetteTransitionObj, _holderObject);
					break;

				case TransitionVisual.AnimatedPanel:
					transitionInstance = Instantiate(_animatedTransitionObj, _holderObject);
					break;

				case TransitionVisual.FullFill:
				default:
					transitionInstance = Instantiate(_defaultTransitionObj, _holderObject);
					break;
			}

			// Make sure our transition is last in our list
			transitionInstance.transform.SetAsLastSibling();

			// Then we Initialize our Transition Instance
			transitionInstance.Set(transition);
		}

		#endregion

	}

}
