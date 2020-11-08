using UnityEngine;

namespace SweatyChair
{

	public class PermanentLoader : MonoBehaviour
	{

		private static bool isAdded = false;

		[SerializeField] private GameObject _permanentPrefab;

		private void Awake()
		{
			if (!isAdded)
				Instantiate(_permanentPrefab);
			isAdded = true;
			Destroy(gameObject);
		}

	}

}