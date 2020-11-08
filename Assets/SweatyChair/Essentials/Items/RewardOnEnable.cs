using UnityEngine;

namespace SweatyChair
{

	/// <summary>
	/// Reward player on enable, used mainly in tutorial.
	/// </summary>
	public class RewardOnEnable : MonoBehaviour
	{

		[SerializeField] private Reward _reward;
		[SerializeField] private string _eventName;

		private void OnEnable()
		{
			_reward.Obtain(_eventName);
		}

	}

}