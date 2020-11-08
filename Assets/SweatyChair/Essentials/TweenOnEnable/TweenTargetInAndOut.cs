using UnityEngine;
using UnityEngine.Serialization;

public class TweenTargetInAndOut : MonoBehaviour
{
    [SerializeField] private GameObject _target;

    [SerializeField] private Vector3 _startOffset = Vector3.zero;
    [Range(0, float.MaxValue)]
    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private float _waitTime = 0;
    [SerializeField] private LeanTweenType _tweenType = LeanTweenType.linear;
    [SerializeField] private LeanTweenType _loopType = LeanTweenType.once;
    [SerializeField] private bool _ignoreTimeScale = false;
    [SerializeField] private bool _disableOnComplete = true;

    private Vector3 _initLocalPosition;

    // Public Getters
    public float duration => _duration;

    private void Awake()
    {
        if (_target == null)
            _target = gameObject;

        _initLocalPosition = _target.transform.localPosition;
    }

    [ContextMenu("Execute")]
    private void OnEnable()
    {
        TweenInAndOut();
    }

    public void TweenInAndOut()
    {
        _target.SetActive(true);
        _target.transform.localPosition = _initLocalPosition + _startOffset;
        LeanTween.moveLocal(_target, _initLocalPosition, _duration)
                 .setEase(_tweenType)
                 .setLoopType(_loopType)
                 .setIgnoreTimeScale(_ignoreTimeScale)
                 .setOnComplete(ComeBack);
    }


    private void ComeBack()
    {
        LeanTween.moveLocal(_target, _initLocalPosition + _startOffset, _duration)
                 .setEase(_tweenType)
                 .setLoopType(_loopType)
                 .setIgnoreTimeScale(_ignoreTimeScale)
                 .setDelay(_waitTime)
                 .setOnComplete(CheckComplete);
    }

    private void CheckComplete()
    {
        if (_disableOnComplete)
        {
            _target.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (_loopType != LeanTweenType.once)
            LeanTween.cancel(_target);
    }

}