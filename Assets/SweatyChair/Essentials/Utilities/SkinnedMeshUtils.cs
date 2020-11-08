using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SweatyChair
{

	public static class SkinnedMeshUtils
	{

		#region Bone Weights

		public static Color[] BoneWeightsToColour(Mesh mesh)
		{
			if (mesh.boneWeights == null || mesh.boneWeights.Length == 0) { return new Color[0]; }
			return mesh.boneWeights.Select(boneWeight => new Color(boneWeight.weight0, boneWeight.weight1, boneWeight.weight2, boneWeight.weight3)).ToArray();
		}

		public static Mesh TransferBoneWeightsToVertColour(Mesh originalMesh)
		{
			Mesh tempMeshCopy = Object.Instantiate(originalMesh);
			tempMeshCopy.colors = BoneWeightsToColour(tempMeshCopy);
			return tempMeshCopy;
		}

		#endregion

		#region ResetToBindPose

		public static void ResetToBindPose(this SkinnedMeshRenderer skinnedMesh)
		{
			ResetSkinnedMeshToBindPose(skinnedMesh);
		}

		public static void ResetSkinnedMeshToBindPose(SkinnedMeshRenderer skinnedMesh)
		{
			Transform[] boneArray = skinnedMesh.bones;
			Matrix4x4[] bindPoseArray = skinnedMesh.sharedMesh.bindposes;

			Dictionary<Transform, Matrix4x4> bindPoseMap = new Dictionary<Transform, Matrix4x4>();
			for (int i = 0; i < boneArray.Length; i++) {
				if (boneArray[i] != null && !bindPoseMap.ContainsKey(boneArray[i])) {
					bindPoseMap.Add(boneArray[i], bindPoseArray[i]);
				}
			}

			foreach (var kvp in bindPoseMap) {
				Transform boneTF = kvp.Key;
				Matrix4x4 bindPose = kvp.Value;

				// Recreate the local transform matrix of the bone
				Matrix4x4 localMatrix = bindPoseMap.ContainsKey(boneTF.parent) ? (bindPose * bindPoseMap[boneTF.parent].inverse).inverse : bindPose.inverse;


				// Recreate local transform from that matrix
				boneTF.localPosition = localMatrix.MultiplyPoint(Vector3.zero);
				boneTF.localRotation = Quaternion.LookRotation(localMatrix.GetColumn(2), localMatrix.GetColumn(1));
				boneTF.localScale = new Vector3(localMatrix.GetColumn(0).magnitude, localMatrix.GetColumn(1).magnitude, localMatrix.GetColumn(2).magnitude);
			}
		}

		#endregion

		#region Get Root bone

		/// <summary>
		/// Returns the highest skinned bone in the hierarchy in our skinned renderer bone array
		/// </summary>
		/// <param name="renderer"></param>
		/// <returns></returns>
		public static Transform GetRootSkinnedBone(this SkinnedMeshRenderer renderer)
		{
			//If our renderer or our bones are null. Return null
			if (renderer == null || renderer.bones == null) { return null; }

			//Otherwise traverse up our hierarchy and find our parent bone
			Transform[] bones = renderer.bones;

			Transform rootBone = bones[0];

			while (bones.Contains(rootBone)) {
				rootBone = rootBone.parent;
			}

			return rootBone;
		}

		/// <summary>
		/// Returns the highest shared transform of all of our bones in our skeleton
		/// </summary>
		/// <param name="renderer"></param>
		/// <returns></returns>
		public static Transform GetRootSkeletonTransform(this SkinnedMeshRenderer renderer)
		{
			// Check if our renderer is null
			if (renderer == null)
				return null;

			// Then check if we have any bones in our hierarchy
			if (renderer.bones == null || renderer.bones.Length <= 0)
				return renderer.transform;

			// Finally actually attempt to find our shared root skeleton
			Transform rootBone = renderer.bones[0];
			for (int i = 1; i < renderer.bones.Length; i++)
				rootBone = TransformUtils.GetYoungestSharedParent(rootBone, renderer.bones[i]);

			return rootBone;
		}

		#endregion

		#region Get Skeleton Path

		public static string GetRelativeBonePath(SkinnedMeshRenderer renderer, Transform bone)
		{
			Transform root = GetRootSkinnedBone(renderer);

			//If our bone is not a child of the root. Return empty
			if (!bone.IsChildOf(root)) { return string.Empty; }

			string path = bone.name;
			while (bone.parent != null && bone.parent != root) {
				bone = bone.parent;
				path = bone.name + "/" + path;
			}

			return path;
		}

		#endregion

		#region Get if bone is Skinned

		public static bool IsBoneWeighted(this SkinnedMeshRenderer skinRenderer, Transform bone)
		{
			// First we check if either is null
			if (skinRenderer == null || skinRenderer.sharedMesh == null || bone == null)
				return false;

			// Then check if our skinned bone is even a part of our root skeleton
			if (!skinRenderer.bones.Contains(bone))
				return false;

			// Finally, all we need now is the index our skinned bone is at, so we can check if there are any weights
			int boneIndex = System.Array.IndexOf(skinRenderer.bones, bone);

			// Now we loop through all of our mesh and check if we have any bone weightings for that bone!
			List<BoneWeight> boneWeights = new List<BoneWeight>();
			skinRenderer.sharedMesh.GetBoneWeights(boneWeights);

			bool hasWeight = false;

			for (int i = 0; i < boneWeights.Count; i++) {

				// Check if any of our bone weights match and weight is not 0
				hasWeight |= (boneIndex == boneWeights[i].boneIndex0 && !Mathf.Approximately(boneWeights[i].weight0, 0));
				hasWeight |= (boneIndex == boneWeights[i].boneIndex1 && !Mathf.Approximately(boneWeights[i].weight1, 0));
				hasWeight |= (boneIndex == boneWeights[i].boneIndex2 && !Mathf.Approximately(boneWeights[i].weight2, 0));
				hasWeight |= (boneIndex == boneWeights[i].boneIndex3 && !Mathf.Approximately(boneWeights[i].weight3, 0));

				if (hasWeight)
					break;
			}

			return hasWeight;
		}

		#endregion

		#region Remove Skinned Bone

		/// <summary>
		/// Removes all unused bones from a skinned mesh renderer. WARNING, THIS WILL AFFECT ALL INSTANCES OF THE MESH.
		/// If this behaviour is not desired, you must duplicate and manage the lifetime of the meshes yourself.
		/// </summary>
		/// <param name="skinRenderer"></param>
		public static void RemoveUnusedBones(this SkinnedMeshRenderer skinRenderer)
		{
			// Go through all of our bones, and remove all unused ones
			Transform[] skinnedBoneCopy = skinRenderer.bones;
			for (int i = 0; i < skinnedBoneCopy.Length; i++) {
				// Check if this bone is actually bound to anything
				if (!skinRenderer.IsBoneWeighted(skinnedBoneCopy[i])) {

					// Remove our bone from our mesh data
					skinRenderer.RemoveSkinnedBone(skinnedBoneCopy[i]);
					// Then delete our joint from our skeleton
					TransformUtils.DestroyTransformWithoutAffectingChildren(skinnedBoneCopy[i], false);
				}
			}
		}

		public static void RemoveUnusedBonesAndTransforms(this SkinnedMeshRenderer skinRenderer)
		{
			Transform rootBone = skinRenderer.GetRootSkinnedBone();

			// Then Go through all of our transforms in our children, and remove the transform if it is not in our bones list or it is not skinned
			Transform[] allChildBones = rootBone.GetComponentsInChildren<Transform>();
			for (int i = 0; i < allChildBones.Length; i++) {

				if (allChildBones[i] == rootBone)
					continue;

				// Go through all of our child bones, and check if we have this bone in our bone list
				if (!skinRenderer.IsBoneWeighted(allChildBones[i])) {

					// Remove the bone from our mesh data
					skinRenderer.RemoveSkinnedBone(allChildBones[i]);
					// Then Delete the joint from our skeleton
					TransformUtils.DestroyTransformWithoutAffectingChildren(allChildBones[i], false);
				}
			}
		}

		/// <summary>
		/// Removes a bone from a skinned mesh. WARNING, THIS WILL AFFECT ALL INSTANCES OF THE MESH.
		/// If this behaviour is not desired, you must duplicate and manage the lifetime of the mesh yourself.
		/// </summary>
		/// <param name="skinRenderer"></param>
		/// <param name="skinnedBone"></param>
		/// <param name="forceInstanceMesh"></param>
		public static void RemoveSkinnedBone(this SkinnedMeshRenderer skinRenderer, Transform skinnedBone)
		{
			// First we check if either is null
			if (skinRenderer == null || skinRenderer.sharedMesh == null || skinnedBone == null)
				return;

			// Then check if our skinned bone is even a part of our root skeleton
			if (!skinRenderer.bones.Contains(skinnedBone))
				return;

			// Finally, all we need now is the index our skinned bone is at,then we can work on removing it from the skeleton
			int boneIndex = System.Array.IndexOf(skinRenderer.bones, skinnedBone);
			Mesh tempMesh = skinRenderer.sharedMesh;

			// First we clear bone from our bindpose data
			List<Matrix4x4> bindPoses = new List<Matrix4x4>();
			tempMesh.GetBindposes(bindPoses);
			bindPoses.RemoveAt(boneIndex);
			tempMesh.bindposes = bindPoses.ToArray();

			// Now we loop through all of our mesh and remove all our bone weightings for that bone!
			List<BoneWeight> boneWeights = new List<BoneWeight>();
			tempMesh.GetBoneWeights(boneWeights);
			for (int i = 0; i < boneWeights.Count; i++) {

				int affectedBoneIndex = -1;
				// Go through all of our bone indexes and check if our bone appears
				if (boneWeights[i].boneIndex0 == boneIndex)
					affectedBoneIndex = 0;
				else if (boneWeights[i].boneIndex1 == boneIndex)
					affectedBoneIndex = 1;
				else if (boneWeights[i].boneIndex2 == boneIndex)
					affectedBoneIndex = 2;
				else if (boneWeights[i].boneIndex3 == boneIndex)
					affectedBoneIndex = 3;

				// Create a new bone weight to store our data in
				BoneWeight newBoneWeight = new BoneWeight();

				// Then if our affected bone index is not -1, then we modify our weights
				if (affectedBoneIndex != -1) {

					// NOTE: This code is going to look a bit convoluted, and overdone. This is to keep Unitys' best mesh practices.
					//"Each vertex can be affected by up to 4 different bones. The bone weights should be in descending order (most significant first) and add up to 1" (https://docs.unity3d.com/ScriptReference/Mesh-boneWeights.html)

					// Get how many of our bones affect this vertice, and the total bone weight of this vertice minus our affected bone
					int affectedBoneCount = 0;
					float totalWeight = 0;

					if (affectedBoneIndex != 0 && !Mathf.Approximately(boneWeights[i].weight0, 0)) {
						totalWeight += boneWeights[i].weight0;
						affectedBoneCount++;

					}
					else if (affectedBoneIndex != 1 && !Mathf.Approximately(boneWeights[i].weight1, 0)) {
						totalWeight += boneWeights[i].weight1;
						affectedBoneCount++;

					}
					else if (affectedBoneIndex != 2 && !Mathf.Approximately(boneWeights[i].weight2, 0)) {
						totalWeight += boneWeights[i].weight2;
						affectedBoneCount++;

					}
					else if (affectedBoneIndex != 3 && !Mathf.Approximately(boneWeights[i].weight3, 0)) {
						totalWeight += boneWeights[i].weight3;
						affectedBoneCount++;
					}

					// Then we remove our transform from this bone weighting. 
					// We first have to get our bone weight remaining to divy up to the remaining bones to keep our total weight at 1.
					// Then we also have to shift our bone weights up if we removed a bone weight in the beginning or middle of our total
					float remainingBoneWeight = (1 - totalWeight) / affectedBoneCount;

					// Bone 0 - If our bone index is less than our affected index, just modify our current weight. Otherwise we get our next bone index and our next bone weight
					newBoneWeight.boneIndex0 = (0 < affectedBoneIndex) ? boneWeights[i].boneIndex0 : boneWeights[i].boneIndex1;
					newBoneWeight.weight0 = ((0 < affectedBoneIndex) ? boneWeights[i].weight0 : boneWeights[i].weight1) + remainingBoneWeight;

					// Bone 1
					if (affectedBoneCount > 1) {
						newBoneWeight.boneIndex1 = (1 < affectedBoneIndex) ? boneWeights[i].boneIndex1 : boneWeights[i].boneIndex2;
						newBoneWeight.weight1 = ((1 < affectedBoneIndex) ? boneWeights[i].weight1 : boneWeights[i].weight2) + remainingBoneWeight;
					}

					// Bone 2
					if (affectedBoneCount > 2) {
						newBoneWeight.boneIndex2 = (2 < affectedBoneIndex) ? boneWeights[i].boneIndex2 : boneWeights[i].boneIndex3;
						newBoneWeight.weight2 = ((2 < affectedBoneIndex) ? boneWeights[i].weight2 : boneWeights[i].weight3) + remainingBoneWeight;
					}

					// Bone 3
					if (affectedBoneCount > 3) {
						newBoneWeight.boneIndex3 = (3 < affectedBoneIndex) ? boneWeights[i].boneIndex3 : 0;
						newBoneWeight.weight3 = ((3 < affectedBoneIndex) ? boneWeights[i].weight3 : 0);
					}
				}
				else {
					// Bone 0
					newBoneWeight.boneIndex0 = boneWeights[i].boneIndex0;
					newBoneWeight.weight0 = boneWeights[i].weight0;
					// Bone 1
					newBoneWeight.boneIndex1 = boneWeights[i].boneIndex1;
					newBoneWeight.weight1 = boneWeights[i].weight1;
					// Bone 2
					newBoneWeight.boneIndex2 = boneWeights[i].boneIndex2;
					newBoneWeight.weight2 = boneWeights[i].weight2;
					// Bone3
					newBoneWeight.boneIndex3 = boneWeights[i].boneIndex3;
					newBoneWeight.weight3 = boneWeights[i].weight3;
				}

				// Finally shift all our bone indexes down afterwards
				newBoneWeight.boneIndex0 = (newBoneWeight.boneIndex0 > boneIndex) ? newBoneWeight.boneIndex0 - 1 : newBoneWeight.boneIndex0;
				newBoneWeight.boneIndex1 = (newBoneWeight.boneIndex1 > boneIndex) ? newBoneWeight.boneIndex1 - 1 : newBoneWeight.boneIndex1;
				newBoneWeight.boneIndex2 = (newBoneWeight.boneIndex2 > boneIndex) ? newBoneWeight.boneIndex2 - 1 : newBoneWeight.boneIndex2;
				newBoneWeight.boneIndex3 = (newBoneWeight.boneIndex3 > boneIndex) ? newBoneWeight.boneIndex3 - 1 : newBoneWeight.boneIndex3;

				// Finally assign our new bone weight back to our array
				boneWeights[i] = newBoneWeight;
			}

			// Set our bone Weights
			tempMesh.boneWeights = boneWeights.ToArray();

			// Finally we remove our bone from our skinned mesh as well
			List<Transform> skinnedBones = new List<Transform>(skinRenderer.bones);
			skinnedBones.Remove(skinnedBone);
			skinRenderer.bones = skinnedBones.ToArray();

			// Then Recalculate our root bone
			Transform newRootBone = GetRootSkinnedBone(skinRenderer);
			skinRenderer.rootBone = newRootBone;
		}

		#endregion

		#region Enforce Bind Pose

		/// <summary>
		/// Enforces bind pose to be based off local position. WARNING, THIS WILL AFFECT ALL INSTANCES OF THE MESH.
		/// If this behaviour is not desired, you must duplicate and manage the lifetime of the mesh yourself.
		/// </summary>
		/// <param name="skinRenderer"></param>
		/// <param name="skinnedBone"></param>
		/// <param name="forceInstanceMesh"></param>
		public static void EnforceBindPose(this SkinnedMeshRenderer skinRenderer, Transform skinnedBone)
		{
			// First we check if either is null
			if (skinRenderer == null || skinRenderer.sharedMesh == null || skinnedBone == null)
				return;

			// Then check if our skinned bone is even a part of our root skeleton
			if (!skinRenderer.bones.Contains(skinnedBone))
				return;

			Mesh tempMesh = skinRenderer.sharedMesh;

			// First we get our bindpose data
			List<Matrix4x4> bindPoses = new List<Matrix4x4>();
			tempMesh.GetBindposes(bindPoses);

			Transform rootBone = GetRootSkinnedBone(skinRenderer);

			// Now Enforce our correct bind pose for each bone
			for (int i = 0; i < bindPoses.Count; i++) {
				bindPoses[i] = skinRenderer.bones[i].localToWorldMatrix * rootBone.localToWorldMatrix;
			}

			// Set our bind pose
			tempMesh.bindposes = bindPoses.ToArray();
		}

		#endregion

		#region Create Isolated Skinned Mesh

		/// <summary>
		/// Generates a copy of the provided skinned mesh but with a completely stripped hierarchy to only contain the transforms which are required.
		/// </summary>
		/// <param name="skinnedMeshRenderer"></param>
		/// <returns></returns>
		public static GameObject InstantiateIsolatedSkinnedMesh(SkinnedMeshRenderer skinnedMeshRenderer)
		{
			// First validate our data
			if (skinnedMeshRenderer == null)
				return null;

			// Create a parent object to hold our object and our mesh that we instantiate
			Transform parentRendererTransform = skinnedMeshRenderer.transform.parent;
			string parentName = (parentRendererTransform != null) ? parentRendererTransform.name : skinnedMeshRenderer.gameObject.name;

			// Generate our parent Obj
			GameObject isolatedParent = new GameObject(parentName);
			if (parentRendererTransform != null)
				isolatedParent.transform.SetTransformWithoutAffectingChildren(parentRendererTransform.localPosition, parentRendererTransform.localRotation, parentRendererTransform.localScale);

			// Create a copy of our Skinned Mesh Renderer - Which ends up being that we just duplicate our GameObject, and remove all children
			GameObject isolatedSkinRenderer = Object.Instantiate(skinnedMeshRenderer.gameObject, isolatedParent.transform, false);
			isolatedSkinRenderer.transform.DestroyAllChildren();

			// Generate a Hashset of all of our transforms, and just go through and Generate our hierarchy
			Transform oldSkeletonRoot = skinnedMeshRenderer.GetRootSkeletonTransform();

			GenerateDuplicateTransformTree(oldSkeletonRoot, skinnedMeshRenderer.bones, isolatedParent.transform, out Transform[] newBones);

			// Then Set our skinned mesh copy to reference it's new bones
			SkinnedMeshRenderer renderer = isolatedSkinRenderer.GetComponent<SkinnedMeshRenderer>();
			renderer.bones = newBones;

			return isolatedParent;
		}

		private static void GenerateDuplicateTransformTree(Transform rootTransfom, Transform[] leafTransforms, Transform newRoot, out Transform[] newLeafTransforms)
		{
			// Init our out
			newLeafTransforms = new Transform[leafTransforms.Length];

			// We do this in two separate passes. One to create all required transforms, then the next to set parenting and the TRS of the other transforms
			Dictionary<Transform, Transform> linkedTransformDict = new Dictionary<Transform, Transform>();

			int boneCount = 0;

			// First pass, creating all of our transforms, and setting up our transform dictionary
			for (int i = 0; i < leafTransforms.Length; i++) {

				// Iterate upwards through our list until we find a transform we have already created, or we reach our root or null
				Transform tfPrefab = leafTransforms[i];
				while (tfPrefab != null && !linkedTransformDict.ContainsKey(tfPrefab)) {

					// Generate our Transform
					Transform tfInstance = new GameObject(tfPrefab.name).transform;

					// Link it in our dictionary
					linkedTransformDict.Add(tfPrefab, tfInstance);

					// If we are our root transform, break out of our while
					if (tfPrefab == rootTransfom)
						break;

					// Then Attempt to set our prefab to our parent to continue our loop
					tfPrefab = tfPrefab.parent;
				}

				// Get our item from our dict
				newLeafTransforms[i] = linkedTransformDict[leafTransforms[i]];
			}

			// Then go through and Set our parents and our local pos, rot and scale
			KeyValuePair<Transform, Transform>[] flattenedPairList = linkedTransformDict.ToArray();
			for (int i = 0; i < flattenedPairList.Length; i++) {

				// Get our two data
				Transform oldData = flattenedPairList[i].Key;
				Transform newData = flattenedPairList[i].Value;

				if (oldData != rootTransfom) {
					// Get our new parent from our list
					if (linkedTransformDict.TryGetValue(oldData.parent, out Transform parent))
						newData.SetParent(parent);
				}
				else {
					newData.SetParent(newRoot);
				}

				// Then do our transform
				newData.localPosition = oldData.localPosition;
				newData.localRotation = oldData.localRotation;
				newData.localScale = oldData.localScale;
			}
		}

		#endregion

	}

}
