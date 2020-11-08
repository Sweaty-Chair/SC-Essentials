using UnityEngine;

namespace SweatyChair.UI
{

    /// <summary>
    /// Instantiate panel depeneds on platforms, this is for panel prefabs very different acrooss platform
    /// </summary>
    public class PanelInstantiatorOnPlatforms : MonoBehaviour
    {

        [Header("Prefabs")]
        [SerializeField] private GameObject _panelStandaloneMacPrefab;
        [SerializeField] private GameObject _panelStandaloneWindowsPrefab, _panelIOSPrefab, _panelAndroidPrefab;

        [Header("Settings")]
        [Tooltip("Setup the size of new panel as the size of canvas, otherwise using the prefab default size")]
        [SerializeField] private bool _followCanvasSize = true;
        [Tooltip("Replace this game object from the new panel, otherwise create as children")]
        [SerializeField] private bool _replaceCurrentGameObject = true;
        [Tooltip("The new panel name, is not set then simply using the prefab name")]
        [SerializeField] private string _objectName = "";

        private void Awake()
        {
            Transform parentTF = _replaceCurrentGameObject ? transform.parent : transform;

#if UNITY_STANDALONE_OSX
			GameObject panelGO = Instantiate(_panelStandaloneMacPrefab, parentTF);
#elif UNITY_STANDALONE_WIN
            GameObject panelGO = Instantiate(_panelStandaloneWindowsPrefab, parentTF);
#elif UNITY_IOS
            GameObject panelGO = Instantiate(_panelIOSPrefab, parentTF);
#elif UNITY_ANDROID
			GameObject panelGO = Instantiate(_panelIOSPrefab, parentTF);
#endif
            if (!string.IsNullOrEmpty(_objectName))
                panelGO.name = _objectName;

#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID

            panelGO.transform.localScale = Vector3.one;

            // Set up our rect transform if need be
            if (_followCanvasSize) {
                Canvas panelCanvas = panelGO.GetComponent<Canvas>();
                if (panelCanvas != null) {
                    RectTransform canvasRT = panelCanvas.transform as RectTransform;
                    canvasRT.anchorMin = Vector2.zero;
                    canvasRT.anchorMax = Vector2.one;
                    canvasRT.offsetMin = canvasRT.offsetMax = Vector2.zero;
                }
            }

#endif

        }

    }

}