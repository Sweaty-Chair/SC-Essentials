using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
[ExecuteInEditMode]
public class SortingGroupController : MonoBehaviour
{

	[SerializeField] private bool _runOnUpdate = true;
	[SerializeField] private int _offset = 30;
	[SerializeField] private int _scale = 100;

	private SortingGroup _sortingGroup;
	private Transform _transform;

	void Awake()
	{
		_sortingGroup = GetComponent<SortingGroup>();
		_transform = transform;
		#if !UNITY_EDITOR
		if (!_runOnUpdate)
			Destroy(this);
		#endif
	}
		
	void Update()
	{
		_sortingGroup.sortingOrder = _offset + Mathf.RoundToInt(-_transform.position.y * _scale);
	}

	#if UNITY_EDITOR

	[ContextMenu("Execute")]
	private void Execute()
	{
		_sortingGroup = GetComponent<SortingGroup>();
		_transform = transform;
		Update();
	}

	#endif

}