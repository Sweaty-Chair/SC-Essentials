using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("Layout/Image Fit In Parent", 142)]
	[ExecuteAlways]
	[RequireComponent(typeof(RectTransform), typeof(MaskableGraphic))]
	[DisallowMultipleComponent]
	/// <summary>
	/// Resizes a RectTransform to fit a specified aspect ratio.
	/// </summary>
	public class ImageFitInParent : UIBehaviour, ILayoutSelfController
	{

		#region Variables

		[System.NonSerialized] private Image _image;
		public Image image {
			get {
				if (_image == null) {
					_image = GetComponent<Image>();
				}
				return _image;
			}
		}

		[System.NonSerialized] private RawImage _rawImage;
		public RawImage rawImage {
			get {
				if (_rawImage == null) {
					_rawImage = GetComponent<RawImage>();
				}
				return _rawImage;
			}
		}

		public bool isUsingRawImage => image == null;
		public Texture currentTexture {
			get {
				return (!isUsingRawImage) ? image?.sprite?.texture : rawImage.texture;
			}
		}
		private Rect _currentTextureRect = new Rect();
		public Rect currentTextureRect {
			get {
				if (isUsingRawImage) {
					_currentTextureRect.Set(0, 0, _rawImage.texture.width, _rawImage.texture.height);
				} else {
					_currentTextureRect.Set(0, 0, _image.sprite.rect.width, _image.sprite.rect.height);
				}
				return _currentTextureRect;
			}
		}

		/// <summary>
		/// The aspect ratio to enforce. This means width divided by height.
		/// </summary>
		public float aspectRatio {
			get {
				if (currentTexture != null) {
					return currentTextureRect.width / currentTextureRect.height;
				} else {
					return 1;
				}
			}
		}

		[Header("Settings")]
		[SerializeField] private bool _fillInParent = false;
		[SerializeField] private bool _checkForImageChangeEveryFrame = false;

		[System.NonSerialized]
		private RectTransform m_Rect;
		private RectTransform rectTransform {
			get {
				if (m_Rect == null)
					m_Rect = GetComponent<RectTransform>();
				return m_Rect;
			}
		}

		// This "delayed" mechanism is required for case 1014834.
		private Vector2 _cachedSize;
		private bool m_DelayedSetDirty = false;

		private DrivenRectTransformTracker m_Tracker;

		#endregion

		#region Constructor

		protected ImageFitInParent() { }

		#endregion

		#region OnEnable / OnDisable

		protected override void OnEnable()
		{
			base.OnEnable();
			SetDirty();
		}

		protected override void OnDisable()
		{
			m_Tracker.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			base.OnDisable();
		}

		#endregion

		#region OnValidate

#if UNITY_EDITOR
		// OnValidate is only run on Editor, more in https://stackoverflow.com/questions/38613187/why-cant-i-override-onvalidate-in-unity3d

		protected override void OnValidate()
		{
			base.OnValidate();
			UpdateRect();
		}

#endif

		#endregion

		#region Update

		/// <summary>
		/// Update the rect based on the delayed dirty.
		/// Got around issue of calling onValidate from OnEnable function.
		/// </summary>
		protected virtual void Update()
		{
			if (m_DelayedSetDirty) {
				m_DelayedSetDirty = false;
				SetDirty();
			}

			// If our Image has changed, we should set ourselves dirty
			if (_checkForImageChangeEveryFrame && currentTexture != null &&
				(_cachedSize.x != currentTexture.width ||
				_cachedSize.y != currentTexture.height)) {
				SetDirty();
			}
		}

		#endregion

		#region Transform Updates

		/// <summary>
		/// Function called when this RectTransform or parent RectTransform has changed dimensions.
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			UpdateRect();
		}



		#endregion

		#region Update Rect

		public void ForceUpdate()
		{
			UpdateRect();
		}

		private void UpdateRect()
		{
			if (!IsActive())
				return;

			m_Tracker.Clear();

			m_Tracker.Add(this, rectTransform,
				DrivenTransformProperties.Anchors |
				DrivenTransformProperties.AnchoredPosition |
				DrivenTransformProperties.SizeDeltaX |
				DrivenTransformProperties.SizeDeltaY |
				DrivenTransformProperties.SizeDelta);

			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.anchoredPosition = Vector2.zero;

			Vector2 sizeDelta = Vector2.zero;
			Vector2 parentSize = GetParentSize();

			if (_fillInParent) {
				if ((parentSize.y * aspectRatio < parentSize.x)) {
					sizeDelta.y = GetSizeDeltaToProduceSize(parentSize.x / aspectRatio, 1);
				} else {
					sizeDelta.x = GetSizeDeltaToProduceSize(parentSize.y * aspectRatio, 0);
				}
			} else {
				if ((parentSize.y * aspectRatio < parentSize.x)) {
					sizeDelta.x = GetSizeDeltaToProduceSize(parentSize.y * aspectRatio, 0);
				} else {

					sizeDelta.y = GetSizeDeltaToProduceSize(parentSize.x / aspectRatio, 1);
				}
			}

			rectTransform.sizeDelta = sizeDelta;
		}

		#endregion

		#region Delta to produce size

		private float GetSizeDeltaToProduceSize(float size, int axis)
		{
			return size - GetParentSize()[axis] * (rectTransform.anchorMax[axis] - rectTransform.anchorMin[axis]);
		}

		private Vector2 GetParentSize()
		{
			RectTransform parent = rectTransform.parent as RectTransform;
			if (!parent)
				return Vector2.zero;
			return parent.rect.size;
		}

		#endregion

		#region Setting layout

		/// <summary>
		/// Method called by the layout system. Has no effect
		/// </summary>
		public virtual void SetLayoutHorizontal() { }

		/// <summary>
		/// Method called by the layout system. Has no effect
		/// </summary>
		public virtual void SetLayoutVertical() { }

		/// <summary>
		/// Mark the AspectRatioFitter as dirty.
		/// </summary>
		protected void SetDirty()
		{
			UpdateRect();
		}

		#endregion

		#region Editor

#if UNITY_EDITOR


		[ContextMenu("Force Image Update")]
		private void EDITOR_FORCE_UPDATE()
		{
			ForceUpdate();
		}

#endif

		#endregion

	}
}