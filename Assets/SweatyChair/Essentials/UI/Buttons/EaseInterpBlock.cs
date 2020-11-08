namespace UnityEngine.UI
{

	[System.Serializable]
	public struct EaseInterpBlock
	{

		[SerializeField] private EaseData _normalEase;
		[SerializeField] private EaseData _highlightedEase;
		[SerializeField] private EaseData _pressedEase;

		public EaseData normalEase { get { return _normalEase; } set { _normalEase = value; } }
		public EaseData highlightedEase { get { return _highlightedEase; } set { _highlightedEase = value; } }
		public EaseData pressedEase { get { return _pressedEase; } set { _pressedEase = value; } }

		public static EaseInterpBlock defaultCurveBlock {
			get {
				var c = new EaseInterpBlock {
					_normalEase = EaseData.defaultNormalData,
					_highlightedEase = EaseData.defaultHighlightData,
					_pressedEase = EaseData.defaultPressData,
				};
				return c;
			}
		}

	}

	[System.Serializable]
	public struct EaseData
	{

		[SerializeField] private float _endScale;
		[SerializeField] private EasingFunction.Ease _easeType;
		[SerializeField] private float _animationTime;

		public float endScale { get { return _endScale; } set { _endScale = value; } }
		public EasingFunction.Ease easeType { get { return _easeType; } set { _easeType = value; } }
		public float animationTime { get { return _animationTime; } set { _animationTime = value; } }

		public static EaseData defaultNormalData {
			get {
				var data = new EaseData {
					endScale = 1f,
					easeType = EasingFunction.Ease.EaseOutElastic,
					animationTime = 0.25f
				};
				return data;
			}
		}

		public static EaseData defaultHighlightData {
			get {
				var data = new EaseData {
					endScale = 1.1f,
					easeType = EasingFunction.Ease.EaseOutElastic,
					animationTime = 0.25f
				};
				return data;
			}
		}

		public static EaseData defaultPressData {
			get {
				var data = new EaseData {
					endScale = 0.9f,
					easeType = EasingFunction.Ease.EaseOutCubic,
					animationTime = 0.2f
				};
				return data;
			}
		}
	}

}