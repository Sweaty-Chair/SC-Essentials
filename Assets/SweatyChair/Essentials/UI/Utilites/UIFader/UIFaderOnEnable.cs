using UnityEngine;

namespace SweatyChair.UI
{

	[RequireComponent(typeof(UIFader))]
	public class UIFaderOnEnable : MonoBehaviour
	{

		#region Variables

		[Header("Settings")]
		[SerializeField] private bool _fadeOutOnAwake = true;
		[SerializeField] private bool _fadeOutOnDisable = true;

		private UIFader _uiFader;

		#endregion

		#region Start

		private void Awake()
		{
			if (_uiFader == null)
				_uiFader = GetComponent<UIFader>();

			if (_uiFader != null && _fadeOutOnAwake)
				_uiFader.FadeOut(true);
		}

		#endregion

		#region OnEnable / OnDisable

		private void OnEnable()
		{
			if (_uiFader == null)
				_uiFader = GetComponent<UIFader>();

			if (_uiFader != null)
				_uiFader.FadeIn();
		}

		private void OnDisable()
		{
			if (_uiFader == null)
				_uiFader = GetComponent<UIFader>();

			if (_uiFader != null && _fadeOutOnDisable)
				_uiFader.FadeOut(true);
		}

		#endregion

	}

}
