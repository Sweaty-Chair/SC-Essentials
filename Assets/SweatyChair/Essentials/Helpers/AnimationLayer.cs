using UnityEngine;

namespace SweatyChair
{

	[RequireComponent(typeof(Animation))]
	public class AnimationLayer : MonoBehaviour
	{

		[SerializeField] private AnimationClip _clip;
		[SerializeField] private int _layer = 2;

		private void Awake()
		{
			// Get our Animation Component
			Animation anim = GetComponent<Animation>();

			if (anim == null || _clip == null)
				return;

			// Otherwise set up our layer, and destroy our script
			anim[_clip.name].layer = _layer;
			Destroy(this);
		}

	}

}