using UnityEngine;

namespace SweatyChair
{

	public static class RectTransformUtils
	{

#if UNITY_EDITOR
		[UnityEditor.MenuItem("CONTEXT/RectTransform/Print Transform Position")]
		private static void CopyTransformPath(UnityEditor.MenuCommand menuCommand)
		{
			Debug.Log((menuCommand.context as Transform).position);
		}
#endif

	}

}