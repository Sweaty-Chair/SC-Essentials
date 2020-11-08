using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	/// <summary>
	/// Follows a target in canvas.
	/// Author: RV
	/// </summary>
	public class PanelFollower : MonoBehaviour
	{

		#region Variable

		[Tooltip("A target to follow, can be a game object in game.")]
		[SerializeField] private Transform _targetTF;
		[Tooltip("The camera the following refernced to, uses main camera by default if not set.")]
		[SerializeField] private Camera _camera;
		[SerializeField] private Vector3 _offset;
		[SerializeField] private bool _useCanvasGroup = true;

		private Canvas _parentCanvas;
		private Canvas _selfCanvas;
		private CanvasGroup _canvasGroup;
		private Transform _cameraTF;

		private Vector3 _position;

		public Vector3 targetPosition { get { return _targetTF != null ? _targetTF.position + _offset : _position + _offset; } }

		private Transform _myTF;
		public Transform myTF {
			get {
				if (_myTF == null)
					_myTF = transform;
				return _myTF;
			}
		}

		#endregion

		private void Start()
		{
			_parentCanvas = transform.parent?.GetComponentInParent<Canvas>();

			// Simple optimization trick, Any panel that uses panel follower should not be pixel perfect as this tanks framerate whenever the panel moves
			if (_useCanvasGroup) {
				_parentCanvas.overridePixelPerfect = true;
				_parentCanvas.pixelPerfect = false;
			}

			// If we use a a canvas group add that component, otherwise add a canvas
			if (_useCanvasGroup) {
				_canvasGroup = myTF.GetOrAddComponent<CanvasGroup>();
			} else {
				// Canvas require grapohic raycaster for interaction
				_selfCanvas = myTF.GetOrAddComponent<Canvas>();
				_selfCanvas.overridePixelPerfect = true;
				_selfCanvas.pixelPerfect = false;
				myTF.GetOrAddComponent<GraphicRaycaster>();
			}


			if (_camera == null)
				_camera = Camera.main;
			_cameraTF = _camera?.transform;

			UpdateRender();
		}

		public void SetTarget(Transform targetTF)
		{
			_targetTF = targetTF;
		}

		public void SetRenderCamera(Camera camera)
		{
			_camera = camera;
		}

		public void SetPosition(Vector3 position)
		{
			_targetTF = null;
			_position = position;
		}

		public void SetOffset(Vector3 offset)
		{
			_offset = offset;
		}

		private void LateUpdate()
		{
			UpdateRender();
		}

		#region Update Render

		private void UpdateRender()
		{
			if (_camera == null)
				return;

			// Toggle our visibility if our camera is not even looking in the direction of our object. Fixes issues with panels showing even when object is behind camera
			bool isLookingInDirection = Vector3.Dot(_cameraTF.forward, (_cameraTF.position - targetPosition).normalized) < 0;

			if (_useCanvasGroup) {
				_canvasGroup.alpha = isLookingInDirection ? 1 : 0;
			} else {
				_selfCanvas.enabled = isLookingInDirection;
			}


			if (!isLookingInDirection)
				return;

			switch (_parentCanvas.renderMode) {
			case RenderMode.ScreenSpaceOverlay:
			default:
				myTF.position = _camera.WorldToScreenPoint(targetPosition); // Set Position
				myTF.localRotation = Quaternion.identity; // Set our local rotation back to original
				myTF.localScale = Vector3.one; //Set our scale back to original
				break;
			case RenderMode.ScreenSpaceCamera:
				myTF.position = targetPosition; // Set our position
				myTF.localRotation = Quaternion.identity; // Set our rotation back to original
				SetScale(); // Set our scale based off distance
				break;
			case RenderMode.WorldSpace:
				myTF.position = targetPosition; // Set our position
				SetScale(); // Set our scale based off distance
				LookWithCamera(); // Set our rotation to look towards camera
				break;
			}
		}

		private void SetScale()
		{
			// Set our scale based off distance from our frustrum
			float distanceToMain = Vector3.Distance(_cameraTF.position, _targetTF.position);

			// If we are orthographic. Do orthographic, else use our perspective
			float camHeight = _camera.orthographic ? _camera.orthographicSize * 2 : 2.0f * distanceToMain * Mathf.Tan(Mathf.Deg2Rad * (_camera.fieldOfView * 0.5f));

			// Set our local scale to the inverse of our parents scale so we always maintain a scale of 1,1,1 regardless of parent size
			Vector3 panelParentScale = myTF.parent.lossyScale;
			Vector3 scale = new Vector3(1 / panelParentScale.x, 1 / panelParentScale.y, 1 / panelParentScale.z);

			// Then use the inverse scale in our perspective algorithm
			myTF.localScale = scale * (camHeight / Screen.height);
		}

		private void LookWithCamera()
		{
			// Set our forward to match the forward of our camera
			myTF.forward = _cameraTF.forward;
		}

		public void Toggle(bool show)
		{
			gameObject.SetActive(show);
		}

		#endregion

	}

}