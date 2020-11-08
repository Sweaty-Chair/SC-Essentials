using UnityEngine;

public class ToggleRandomObjectInList : MonoBehaviour
{

	#region Variables

	public GameObject[] gameObjects;

	#endregion

	#region OnEnable

	void OnEnable()
	{
		// If our Array is null or empty, Populate our array with our current children
		if (gameObjects == null || gameObjects.Length == 0)
			CacheChildren();

		// Then toggle our child
		ToggleRandomChild();
	}

	private void CacheChildren()
	{
		// Create a new array as our child count
		gameObjects = new GameObject[transform.childCount];

		int childIndex = 0;
		foreach (Transform child in transform) {
			gameObjects[childIndex++] = child.gameObject;
		}
	}

	#endregion

	#region Toggle Random Object In List

	public void ToggleRandomChild()
	{
		int selectedChild = Random.Range(0, gameObjects.Length);

		// Go through each of our children, toggling them on or off
		for (int i = 0; i < gameObjects.Length; i++)
			gameObjects[i].SetActive(i == selectedChild);
	}

	#endregion

}
