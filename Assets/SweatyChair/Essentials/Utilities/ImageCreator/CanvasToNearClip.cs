using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasToNearClip : MonoBehaviour
{

	#region Const

	private const float EPSILON = 0.001f;

	#endregion

	#region Variables

	private Canvas _canvas;

	#endregion

	#region OnEnable

	private void OnEnable()
	{
		_canvas = GetComponent<Canvas>();
	}

	#endregion

	#region Update

	private void Update()
	{
		// If we are rendering with a screenspace Camera, and our camera is not null, set our canvas distance to be our near clip distance + an epsilon to avoid Floating point errors
		if (_canvas.renderMode == RenderMode.ScreenSpaceCamera) {
			if (_canvas.worldCamera != null) {

				// If we are not a matching plane distance, we update our distance
				if (!Mathf.Approximately(_canvas.planeDistance, _canvas.worldCamera.nearClipPlane + EPSILON))
					_canvas.planeDistance = _canvas.worldCamera.nearClipPlane + EPSILON;
			}

		}
	}

	#endregion

}
