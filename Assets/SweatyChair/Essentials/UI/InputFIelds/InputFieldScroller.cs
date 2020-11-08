using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

namespace SweatyChair.UI
{

	[ExecuteInEditMode]
	[RequireComponent(typeof(InputField))]
	public class InputFieldScroller : UIBehaviour
	{

		[Tooltip("The default row count in  InputField, this will be ignored if a ScrollRect is assigned")]
		[Range(1, 50)] [SerializeField] private int _minRowCount = 1;

		[Tooltip("Scroll rect parent")]
		[SerializeField] private ScrollRect _scrollRect = null;

		private InputField _inputField;
		private RectTransform _rectTransform, _scrollRectRectTransform;

		// Layout
		private LayoutElement _layoutElement;
		private HorizontalOrVerticalLayoutGroup _parentLayoutGroup;

		private float _canvasScaleFactor = 1;

		private float _minHeight, _currentHeight;

		protected override void Awake()
		{
			_inputField = GetComponent<InputField>();
			_inputField.onValueChanged.AddListener(new UnityAction<string>(ResizeInput));
			_rectTransform = GetComponent<RectTransform>();
			_scrollRectRectTransform = _scrollRect.GetComponent<RectTransform>();
			CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();
			if (canvasScaler)
				_canvasScaleFactor = canvasScaler.scaleFactor;
			_layoutElement = GetComponent<LayoutElement>();
			_parentLayoutGroup = transform.parent.GetComponent<HorizontalOrVerticalLayoutGroup>();
			if (_scrollRect != null && _scrollRect.horizontal) // Force input field horizontally overflow if scrol rect can scroll horizontally
				_inputField.textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
		}

		protected override void Start()
		{
			ResizeInput();
		}

		private void Update()
		{
			if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
				ScrollForCaret(); // Scroll up/down if caret is out of screen
		}

		// Resize input field recttransform
		public void ResizeInput()
		{
			ResizeInput(_inputField.text);
		}

		private void ResizeInput(string text)
		{
			// Current text settings
			TextGenerationSettings settings = _inputField.textComponent.GetGenerationSettings(_inputField.textComponent.rectTransform.rect.size);
			settings.generateOutOfBounds = false;
			settings.scaleFactor = _canvasScaleFactor; // HACK: scale factor of settings not following the global scale factor... make sure it do

			Vector2 scrollRectSize = _scrollRectRectTransform.rect.size;

			if (_scrollRect != null && _scrollRect.horizontal) {
				// Get text padding (min max horizontal offset for width calculation)
				float horizontalOffset = _inputField.placeholder.rectTransform.offsetMin.x - _inputField.placeholder.rectTransform.offsetMax.x + _inputField.textComponent.fontSize;
				// Preferred text rect width
				float preferredWidth = (new TextGenerator().GetPreferredWidth(text, settings) / _canvasScaleFactor) + horizontalOffset;
				float minWidth;
				// Default text rect width (fit to scroll parent or expand to fit text)
				if (_scrollRect) {
					minWidth = scrollRectSize.x; // Width using the content width
				} else {
					minWidth = ((new TextGenerator().GetPreferredWidth("", settings)) / _canvasScaleFactor) + horizontalOffset;
				}
				// Current text rect width
				float currentWidth = _inputField.textComponent.rectTransform.rect.width;
				// Check if need to resize width
				if (Mathf.Abs(currentWidth - preferredWidth) > Mathf.Epsilon) {
					float newWidth = Mathf.Max(preferredWidth, minWidth); // At least min width
					if (_parentLayoutGroup && _layoutElement)
						_layoutElement.preferredHeight = newWidth;
					else
						_rectTransform.sizeDelta = new Vector2(newWidth, _rectTransform.rect.height);
				}
				// Scroll to right if at the right most
				//if (gameObject.activeInHierarchy && _inputField.caretPosition == _inputField.text.Length && _inputField.text.Length > 0 && _inputField.text[_inputField.text.Length - 1] == '\n')
				//StartCoroutine(ScrollToRightCoroutine());
			}

			// Get text padding (min max vertical offset for height calculation)
			float vecticalOffset = _inputField.placeholder.rectTransform.offsetMin.y - _inputField.placeholder.rectTransform.offsetMax.y + _inputField.textComponent.fontSize / 2;
			// Preferred text rect height
			float preferredHeight = (new TextGenerator().GetPreferredHeight(text, settings) / _canvasScaleFactor) + vecticalOffset;
			// Default text rect height (fit to scroll parent or expand to fit text)
			if (_scrollRect) {
				_minHeight = scrollRectSize.y;
			} else {
				_minHeight = ((new TextGenerator().GetPreferredHeight("", settings) * _minRowCount) / _canvasScaleFactor) + vecticalOffset;
			}
			// Current text rect height
			float currentHeight = _inputField.textComponent.rectTransform.rect.height;
			_currentHeight = Mathf.Max(preferredHeight, _minHeight); // At least min height, H
			// Check if need to resize height
			if (Mathf.Abs(currentHeight - preferredHeight) > Mathf.Epsilon) {
				if (_parentLayoutGroup && _layoutElement)
					_layoutElement.preferredHeight = _currentHeight;
				else
					_rectTransform.sizeDelta = new Vector2(_rectTransform.rect.width, _currentHeight);
			}

			if (_inputField.text.Length > 0 && _inputField.text[_inputField.text.Length - 1] == '\n') // Scroll for caret only when a return pressed
				ScrollForCaret();
		}

		private void ScrollForCaret()
		{
			// Scroll to up/down if caret is out of viewport
			if (_currentHeight > _minHeight && gameObject.activeInHierarchy && _inputField.text.Length > 0) { // Only when there's text

				// Get the caret position related to content
				TextGenerator txtGen = _inputField.textComponent.cachedTextGenerator;
				float caretPosition = _currentHeight / 2;
				if (_inputField.caretPosition > 0 && txtGen.characters.Count > _inputField.caretPosition - 1) {
					UICharInfo charInfo = txtGen.characters[_inputField.caretPosition - 1];
					caretPosition = charInfo.cursorPos.y / _inputField.textComponent.pixelsPerUnit;
				}

				// Get the viewport threshold
				float lineHeight = _inputField.textComponent.fontSize * 1.5f; // Ignore line spacing for now
				float scrolledUpOffset = _scrollRect.content.localPosition.y;

				if (_inputField.text.Length >= _inputField.caretPosition && _inputField.caretPosition > 0 && (_inputField.text[_inputField.caretPosition - 1] == '\n' || _inputField.caretPosition == _inputField.text.Length)) // Add a line if in a new line or end of input
					caretPosition -= lineHeight;

				float bottomThreshold = _currentHeight / 2 - _minHeight - scrolledUpOffset + lineHeight; // H/2 - h - dy + lh
				float topThreshold = Mathf.Min(_currentHeight / 2, _currentHeight / 2 - scrolledUpOffset); // H/2 - dy

				//print("H=" + _currentHeight + ",h=" + _minHeight + ",dy=" + scrolledUpOffset + ",cp=" + caretPosition + ",bt=" + bottomThreshold + ",tt=" + topThreshold + (_inputField.caretPosition > 0 ? (",lc=" + _inputField.text[_inputField.caretPosition - 1] + "(" + (int)(_inputField.text[_inputField.caretPosition - 1]) + ")") : ""));

				if (caretPosition <= bottomThreshold) {
					StartCoroutine(ScrollVerticalCoroutine((_currentHeight / 2 + caretPosition - 2 * _inputField.textComponent.fontSize) / (_currentHeight - _minHeight))); // (H/2 + cp - 2fh) / (H - h)
				} else if (caretPosition >= topThreshold) {
					StartCoroutine(ScrollVerticalCoroutine(1 - (_currentHeight / 2 - caretPosition) / (_currentHeight - _minHeight))); // 1 - (H/2 - cp + 2fh) / (H - h)
				}
			}
		}

		// Update scroll rect horizontal position (after Layout was rebuilt)
		private IEnumerator ScrollToRightCoroutine()
		{
			_scrollRect.StopMovement();
			yield return new WaitForEndOfFrame();
			if (_scrollRect != null)
				_scrollRect.horizontalNormalizedPosition = 0;
		}

		// Update scroll rect vertical position (after Layout was rebuilt)
		private IEnumerator ScrollVerticalCoroutine(float verticalPosition)
		{
			//print("ScrollVerticalCoroutine(" + verticalPosition + ")");
			if (_scrollRect != null) {
				_scrollRect.StopMovement();
				_scrollRect.enabled = false; // Hack: the interia of the scroll rect would pop the position back, disable 
				yield return new WaitForEndOfFrame();
				_scrollRect.verticalNormalizedPosition = verticalPosition;
				yield return new WaitForSeconds(0.5f);
				_scrollRect.enabled = true;
			}
		}

		// Update scroll rect vertical position (after Layout was rebuilt)
		private IEnumerator ScrollToBottomCoroutine()
		{
			//print("ScrollToBottomCoroutine");
			_scrollRect.StopMovement();
			yield return new WaitForEndOfFrame();
			if (_scrollRect != null)
				_scrollRect.verticalNormalizedPosition = 0;
		}

	}

}