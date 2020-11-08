using UnityEngine;

public static class ComponentUtils
{

	public static T GetOrAddComponent<T>(this Component cmpt) where T : Component
	{
		T component = cmpt.GetComponent<T>(); //Get component of type T
		if (component == null) {
			component = cmpt.gameObject.AddComponent<T>();   //If our object is null, add it
		}
		return component;   //Return our new component
	}

	#region Turn Off All Monobehaviour

	public static void TurnOffAllMono(GameObject gameObj)
	{
		MonoBehaviour[] allBehaviours = gameObj.GetComponentsInChildren<MonoBehaviour>(true);
		for (int i = 0; i < allBehaviours.Length; i++) {
			allBehaviours[i].enabled = false;
		}
	}

	#endregion

}