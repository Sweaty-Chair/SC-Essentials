using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweatyChair
{

	public class ToggleChildrenOneByOne : MonoBehaviour
	{

		#region Variables

		[Header("Settings")]
		[SerializeField] private bool _runOnEnable = true;
		[SerializeField] private float _startDelay = 0;

		[SerializeField] private bool _ignoreTimeScale = false;

		[SerializeField] private bool _toggleChildrenOn = true;
		[SerializeField] private float _toggleDelay = 0.1f;

		[SerializeField] private bool _includeInactive = true;
		[SerializeField] private bool _looping = false;

		[SerializeField] private List<GameObject> _ignoreChildGOList = new List<GameObject>();

		private IEnumerator _toggleRoutine;
		private IEnumerator _waitRoutine;

		#endregion

		#region IgnoreList

		public void AddToIgnoreList(GameObject childGO)
		{
			// If our child is not null, and it is actually a child of our object
			if (childGO != null && childGO.transform.parent != null && childGO.transform.parent == transform) {

				// Then if our ignore list doesn't already have our object, add it
				if (!_ignoreChildGOList.Contains(childGO))
					_ignoreChildGOList.Add(childGO);

			}
		}

		public void RemoveFromIgnoreList(GameObject childGO)
		{
			// If our child is not null, and it is actually a child of our object
			if (childGO != null && childGO.transform.parent != null && childGO.transform.parent == transform) {

				// Then if our ignore list has our object, remove it
				if (_ignoreChildGOList.Contains(childGO))
					_ignoreChildGOList.Remove(childGO);

			}
		}

		public void SetIgnoreList(List<GameObject> ignoreChildGOList = null)
		{
			if (_ignoreChildGOList == null)
				_ignoreChildGOList.Clear();
			else
				_ignoreChildGOList = ignoreChildGOList;
		}

		#endregion

		#region OnEnable

		private void OnEnable()
		{
			//Toggle children on enable if we say it should
			if (_runOnEnable) {
				ToggleChildren();
			}
		}

		private void OnDisable()
		{
			//If our routine is running, cancel it
			if (_toggleRoutine != null) {
				StopCoroutine(_toggleRoutine);
			}

			if (_waitRoutine != null) {
				StopCoroutine(_waitRoutine);
			}
		}

		#endregion

		#region Toggle Children

		public void ToggleChildren()
		{
			ToggleChildren(_toggleChildrenOn, _toggleDelay, _startDelay);
		}

		public void ToggleChildren(bool value, float toggleDelay, float startDelay)
		{
			if (startDelay > 0) {
				ToggleChildrenInternal(!value, 0);
				_waitRoutine = TimeManager.Invoke(() => { ToggleChildrenInternal(value, toggleDelay); }, startDelay);
			} else
				ToggleChildrenInternal(value, toggleDelay);
		}

		public void ToggleChildrenInternal(bool value, float toggleDelay)
		{
			GameObject[] childrenObjs = transform.GetGameObjectsInChildren(_includeInactive);

			// Toggle all our children instantly to the inverse of what we will be setting it to
			ToggleChildrenRoutine(childrenObjs, !value, 0);

			// Then toggle all children on
			_toggleRoutine = ToggleChildrenRoutine(childrenObjs, value, toggleDelay);
			StartCoroutine(_toggleRoutine);
		}

		private IEnumerator ToggleChildrenRoutine(GameObject[] childrenObjs, bool value, float toggleDelay)
		{

			//For each of our children objects
			for (int i = 0; i < childrenObjs.Length; i++) {

				if (_ignoreChildGOList.Contains(childrenObjs[i]))
					continue;

				//Toggle them to thier new state
				childrenObjs[i].SetActive(value);

				//Wait for seconds if our delay is not 0 or less
				if (toggleDelay > 0) {
					if (_ignoreTimeScale)
						yield return new WaitForSecondsRealtime(toggleDelay);
					else
						yield return new WaitForSeconds(toggleDelay);
				}
			}

			if (_looping) {
				ToggleChildren();
			}

		}

		#endregion
	}

}