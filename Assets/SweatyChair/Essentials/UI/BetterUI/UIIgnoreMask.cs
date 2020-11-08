using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[DisallowMultipleComponent]
public class UIIgnoreMask : MonoBehaviour
{
	#region Variables

	[NonSerialized] private MaskableGraphic _cachedGraphic;
	[NonSerialized] private bool _cachedMaskableAttr;

	#endregion

	private void OnEnable()
	{
		_cachedGraphic = GetComponent<MaskableGraphic>();

		if (_cachedGraphic != null) {
			_cachedMaskableAttr = _cachedGraphic.maskable;
			_cachedGraphic.maskable = false;
		}
	}

	private void OnDisable()
	{
		if (_cachedGraphic != null)
			_cachedGraphic.maskable = _cachedMaskableAttr;
	}

}