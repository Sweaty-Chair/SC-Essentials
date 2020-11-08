using UnityEditor;
using UnityEngine;

namespace SweatyChair.InputSystem
{

	[CustomEditor(typeof(ScreenAreaControl), true)]
	public class ScreenAreaControlInspector : Editor
	{

		#region Variables

		protected ScreenAreaControl _target;

		protected virtual string iconLocation { get { return ""; } }

		protected Rect screenViewRect { get { return SceneView.currentDrawingSceneView.position; } }

		#endregion

		#region OnEnable / OnDisable

		protected virtual void OnEnable() {
			_target = (ScreenAreaControl)target;
		}

		protected virtual void OnDisable() {
			_target = null;
		}

		#endregion

		#region OnSceneGUI

		protected void OnSceneGUI() {
			Handles.BeginGUI();

			Color oldColor = GUI.color;
			Color oldBackgroundColor = GUI.backgroundColor;

			DrawInfluenceArea();

			if (!string.IsNullOrEmpty(iconLocation)) {
				DrawInputTypeIcon();
			}

			//Return our colour back to normal
			GUI.color = oldColor;
			GUI.backgroundColor = oldBackgroundColor;

			Handles.EndGUI();
		}

		/// <summary>
		/// Draws the current controls viewport affect region to the Scene Viewport
		/// </summary>
		protected void DrawInfluenceArea() {
			//Get our viewport rect from our min and max
			Vector2 viewportRegionCenter = new Vector2(_target.viewportRegionMin.x, (1 - _target.viewportRegionMax.y));
			Vector2 viewportRegionSize = (_target.viewportRegionMax - _target.viewportRegionMin);

			viewportRegionCenter.Scale(new Vector2(screenViewRect.width, screenViewRect.height));
			viewportRegionSize.Scale(new Vector2(screenViewRect.width, screenViewRect.height));

			//Create our rect
			Rect viewportRect = new Rect(viewportRegionCenter, viewportRegionSize);

			//Get a reference to our original GUI Colour

			GUI.color = new Color(1, 1, 1, 0.5f);
			GUI.backgroundColor = new Color(0.3f, 1, 1);

			//Then display it to the screen
			GUI.Box(viewportRect, Texture2D.whiteTexture);
		}

		protected void DrawInputTypeIcon() {
			//Set our GUI Colour
			GUI.color = new Color(1, 1, 1, 1);
			GUI.backgroundColor = new Color(0, 0, 0, 1);

			//Get our viewport rect from our min and max
			Vector2 viewportRegionSize = (_target.viewportRegionMax - _target.viewportRegionMin) * 0.3f;
			Vector2 viewportRegionCenter = ((_target.viewportRegionMin + _target.viewportRegionMax) * 0.5f) - (viewportRegionSize * 0.5f);

			viewportRegionCenter.y = 1 - (viewportRegionCenter.y + viewportRegionSize.y);

			viewportRegionCenter.Scale(new Vector2(screenViewRect.width, screenViewRect.height));
			viewportRegionSize.Scale(new Vector2(screenViewRect.width, screenViewRect.height));

			Rect iconRect = new Rect();
			iconRect.center = viewportRegionCenter;
			iconRect.size = viewportRegionSize;

			Texture icon = (Texture)EditorGUIUtility.Load(iconLocation);
			GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
		}

		#endregion

	}

}
