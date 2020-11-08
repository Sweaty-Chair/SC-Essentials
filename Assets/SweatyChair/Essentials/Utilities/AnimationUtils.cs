using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweatyChair
{
	public static class AnimationUtils
	{

		#region Play Animation

		public static IEnumerator Play(Animation animation, string clipName, bool useTimeScale = true, Action onComplete = null)
		{
			if (!useTimeScale) {
				AnimationState _currState = animation[clipName];
				bool isPlaying = true;
				float _progressTime = 0f;
				float _timeAtLastFrame = 0f;
				float _timeAtCurrentFrame = 0f;
				float deltaTime = 0f;

				animation.Play(clipName);
				_timeAtLastFrame = Time.realtimeSinceStartup;

				while (isPlaying) {
					if (!animation)
						break;

					_timeAtCurrentFrame = Time.realtimeSinceStartup;
					deltaTime = _timeAtCurrentFrame - _timeAtLastFrame;
					_timeAtLastFrame = _timeAtCurrentFrame;

					_progressTime += deltaTime;
					_currState.normalizedTime = _progressTime / _currState.length;
					animation.Sample();

					if (_progressTime >= _currState.length) {
						if (_currState.wrapMode != WrapMode.Loop)
							isPlaying = false;
						else
							_progressTime = 0.0f;
					}

					yield return new WaitForEndOfFrame();
				}

				yield return null;

				if (onComplete != null)
					onComplete();
			} else
				animation.Play(clipName);
		}

		#endregion

		#region Create New Clip

		public static AnimationClip CreateNewLegacyClip(string name)
		{
			AnimationClip newClip = new AnimationClip();
			newClip.name = name;
			newClip.legacy = true;
			newClip.wrapMode = WrapMode.Loop;

			return newClip;
		}

		#endregion

		#region Get Managing Animation

		public static Animation GetManagingAnimation(Transform childTF)
		{
			return childTF?.GetComponentInParent<Animation>();
		}

		#endregion

		#region Get Animation Relative Path

		/// <summary>
		/// Gets the local path from a child bone to the parent animation component
		/// </summary>
		/// <param name="childTF">The transform of the child bone</param>
		/// <returns>The path of our object</returns>
		public static string GetAnimationRelativePath(Transform childTF)
		{
			// Get our animation in our component
			Animation animation = GetManagingAnimation(childTF);

			if (animation != null) {
				// If our animation is not equal to null, return our relative path
				return GetAnimationRelativePath(animation, childTF);
			} else {
				// Otherwise return an empty string
				return string.Empty;
			}
		}
		public static string GetAnimationRelativePath(Animation animation, Transform childTF)
		{
			return GetAnimationRelativePath(animation.transform, childTF);
		}
		public static string GetAnimationRelativePath(Transform rootTF, Transform childTF)
		{
			// If our transforms are null, return nothing
			if (rootTF == null || childTF == null) { return string.Empty; }
			// If our selected bone is not a child of our root. return just the root transform path
			if (!childTF.IsChildOf(rootTF)) { return rootTF.name; }
			// Otherwise, return our child object converted to local path
			return childTF.ToPath(rootTF);
		}

		#endregion

		#region Get State Safely

		public static AnimationState GetAnimationStateSafely(this Animation anim, string animationName)
		{
			if (anim != null) {
				foreach (AnimationState clip in anim) {
					if (clip.name == animationName)
						return clip;
				}
			}
			return null;
		}

		#endregion

		#region Remove All Animation Clips

		public static void RemoveAllAnimationClips(this Animation anim)
		{
			if (anim == null)
				return;

			// Go through all of our animation states and remove our animations from the object
			List<AnimationState> allAnimationStates = new List<AnimationState>();
			foreach (AnimationState state in anim) {
				allAnimationStates.Add(state);
			}

			// Once we have all animation states, we begin removing all our states from our object
			for (int i = 0; i < allAnimationStates.Count; i++) {
				anim.RemoveClip(allAnimationStates[i].clip);
			}
		}

		#endregion

		#region Sample and Preview Animation

		public static void SampleAndPreviewAnimation(this Animation animation, float time, bool forceAllAnimationStatesActive = false)
		{
			if (animation == null) { return; }

			// Iterate through all of our animation states, and sample all animations at our time
			foreach (AnimationState animState in animation) {

				if (animState != null) {

					// Then force sample our animation
					if (animState.enabled || forceAllAnimationStatesActive) {

						// Set our animation time
						animState.time = time;

						// Force our state on and sample it
						animState.enabled = true;
						animation.Sample();
					}
				}
			}
		}

		public static void SampleAndPreviewAnimation(this Animation animation, string animationName, float time)
		{
			if (animation == null) { return; }

			// Get our animation state from our animation, using the provided name 
			AnimationState animationState = animation.GetAnimationStateSafely(animationName);

			// If our state is not equal to null
			if (animationState != null) {
				// Then sample our animation correctly
				// Force our Wrap mode to avoid issues where our frames get funky at the end
				animationState.wrapMode = WrapMode.Clamp;

				animationState.time = time;
				animationState.weight = 1;
				animationState.layer = 100;

				animationState.enabled = true;
				animation.Sample();
				animationState.enabled = false;
			} else {
				Debug.LogWarningFormat("Could not get animation state '{0}', off of animation '{1}'", animationName, animation.gameObject.name);
			}

		}

		#endregion

	}
}