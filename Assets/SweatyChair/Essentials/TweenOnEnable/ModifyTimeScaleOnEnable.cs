using SweatyChair;
using UnityEngine;

public class ModifyTimeScaleOnEnable : MonoBehaviour
{

	#region Variables

	[Header("Settings")]
	[SerializeField] private float _targetTimeScale = 0;
	[SerializeField] private Priority _priority = Priority.Medium;

	private int _timescaleModifier = -1;

	#endregion

	#region OnEnable / OnDisable

	private void OnEnable()
	{
		TimeScaleManager.SetTimescale(ref _timescaleModifier, Priority.Medium, _targetTimeScale);
	}

	private void OnDisable()
	{
		TimeScaleManager.RemoveTimescale(ref _timescaleModifier);
	}

	#endregion

}
