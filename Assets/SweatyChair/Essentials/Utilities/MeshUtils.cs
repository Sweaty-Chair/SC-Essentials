using System.Collections.Generic;
using UnityEngine;

namespace SweatyChair
{

	public static class MeshUtils
	{

		#region Set Pivot of mesh

		public static void SetMeshPivot(Mesh mesh, Vector3 normalizedPivotPos)
		{
			//Recalculate our local bounds so we can get the min and max size of our local mesh
			mesh.RecalculateBounds();
			Bounds meshBounds = mesh.bounds;

			//Go through our mesh and get the absolute local position of our mesh
			Vector3 offset = Vector3.zero + meshBounds.min;
			offset += Vector3.Scale(meshBounds.size, normalizedPivotPos);

			//Create a copy of our vertices for editing
			Vector3[] vertices = mesh.vertices;
			//Then minus our offset
			for (int i = 0; i < vertices.Length; i++) {
				vertices[i] -= offset;
			}

			//Then re-assign our vertices
			mesh.vertices = vertices;
			mesh.RecalculateBounds();
		}

		public static void SetPivot(this Mesh mesh, Vector3 normalizedPivotPos)
		{
			SetMeshPivot(mesh, normalizedPivotPos);
		}

		#endregion

		#region Generate Simple Meshes

		public static Mesh GenerateLineMesh(Vector3 point1, Vector3 point2)
		{
			Vector3 direction = point2 - point1.normalized;

			//Assign our vertices
			Vector3[] meshVertices = new Vector3[4];
			meshVertices[0] = point1;
			meshVertices[3] = point1 + (direction * 0.01f);
			meshVertices[1] = point2;
			meshVertices[2] = point2 - (direction * 0.01f);

			//Assign our triangles
			int[] meshTriangles = new int[] { 0, 1, 2, 0, 2, 3 };

			//Create our new Mesh
			Mesh mesh = new Mesh();
			mesh.vertices = meshVertices;
			mesh.triangles = meshTriangles;

			//Recalculate our data
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			//Return our mesh
			return mesh;
		}

		#endregion

		#region Apply Transformation Matrix

		/// <summary>
		/// Applies a given transformation matrix onto a meshes vertices.
		/// </summary>
		/// <param name="mesh"></param>
		/// <param name="transformMatrix"></param>
		public static void ApplyTransformationMatrix(this Mesh mesh, Matrix4x4 transformMatrix)
		{
			// https://answers.unity.com/questions/1243954/how-to-scale-meshbindpose-correctly.html
			// Cache all of our data
			List<Vector3> vertices = new List<Vector3>();
			List<Matrix4x4> bindPoses = new List<Matrix4x4>();
			List<BoneWeight> boneWeights = new List<BoneWeight>();

			mesh.GetBindposes(bindPoses);
			mesh.GetVertices(vertices);
			mesh.GetBoneWeights(boneWeights);

			// Check if we have a bind pose to respect
			bool hasBoneWeight = boneWeights != null && boneWeights.Count > 0;

			// Go through also setting all of our bone weights - If we have any
			if (hasBoneWeight) {

				// We want to preserve our initial object bindpose scale because of scale degradation over time if we transform our bindposes too many times.
				for (int i = 0; i < bindPoses.Count; i++) {
					bindPoses[i] = transformMatrix * bindPoses[i];
				}

				// TODO: FIGURE OUT HOW TO DO BELOW. SCALE DOWN THE BIND POSE, WHILE ENFORCING THE ORIGINAL ROTATION AND SCALE VALUES FROM THE PREVIOUS BIND POSE

				//// Transform our Vertices into World space,
				//for (int i = 0; i < mesh.vertexCount; i++) {
				//	vertices[i] = CalculateVertexBindpose(bindPoses, boneWeights[i], vertices[i]);
				//	vertices[i] = transformMatrix.MultiplyPoint3x4(vertices[i]);
				//}

				//// We want to preserve our initial object bindpose scale because of scale degradation over time if we transform our bindposes too many times.
				//for (int i = 0; i < bindPoses.Count; i++) {

				//	// Decompose our matrix to get our component parts
				//	bindPoses[i].DecomposeMatrix(out Vector3 position, out Quaternion rotation, out Vector3 scale);

				//	bindPoses[i] = Matrix4x4.TRS(transformMatrix.MultiplyPoint(position), rotation.normalized, scale);
				//}

				//// Go through all of our vertices again, and translate them back into object space
				//for (int i = 0; i < mesh.vertexCount; i++)
				//	vertices[i] = CalculateInverseVertexBindpose(bindPoses, boneWeights[i], vertices[i]);

			}
			else {
				for (int i = 0; i < mesh.vertexCount; i++)
					vertices[i] = transformMatrix.MultiplyPoint3x4(vertices[i]);
			}

			// Set our Vertice array, and force an update of all needed stuff
			mesh.SetVertices(vertices);
			mesh.bindposes = bindPoses.ToArray();   // Annoying linq here, but not worth optimizing atm
			mesh.UploadMeshData(false);

			mesh.RecalculateBounds();
		}

		public static void ApplyTransformationMatrix(this SkinnedMeshRenderer skinnedMeshRenderer, Matrix4x4 transformationMatrix)
		{
			// Apply our Transformation matrix to our skinned mesh
			skinnedMeshRenderer.sharedMesh.ApplyTransformationMatrix(transformationMatrix);

			// Then apply our transformation matrix to our bones
			// Get our root joint, Which we just assume to be the youngest shared parent of our bones
			Transform skeletonRootJoint = SkinnedMeshUtils.GetRootSkeletonTransform(skinnedMeshRenderer);

			// Then Start Transforming our bone positions and scales
			Transform[] boneArray = skeletonRootJoint.GetComponentsInChildren<Transform>();
			for (int i = 0; i < boneArray.Length; i++) {
				if (boneArray[i] == skeletonRootJoint)
					boneArray[i].localScale = Vector3.one;

				boneArray[i].localPosition = transformationMatrix.MultiplyPoint(boneArray[i].localPosition);
			}
		}

		#region Utility

		private static Vector3 CalculateTransformedVertex(List<Matrix4x4> bindPoses, BoneWeight weights, Vector3 vertOPos, Matrix4x4 transformationMatrix)
		{
			Vector3 finalVertPos = CalculateVertexBindpose(bindPoses, weights, vertOPos);
			finalVertPos = transformationMatrix.MultiplyPoint3x4(finalVertPos);
			return CalculateInverseVertexBindpose(bindPoses, weights, finalVertPos);
		}

		private static Vector3 CalculateVertexBindpose(List<Matrix4x4> bindPoses, BoneWeight weights, Vector3 vertOPos)
		{
			Vector3 finalPos = Vector3.zero;

			if (weights.weight0 > 0)
				finalPos += bindPoses[weights.boneIndex0].MultiplyPoint3x4(vertOPos) * weights.weight0;

			if (weights.weight1 > 0)
				finalPos += bindPoses[weights.boneIndex1].MultiplyPoint3x4(vertOPos) * weights.weight1;

			if (weights.weight2 > 0)
				finalPos += bindPoses[weights.boneIndex2].MultiplyPoint3x4(vertOPos) * weights.weight2;

			if (weights.weight3 > 0)
				finalPos += bindPoses[weights.boneIndex3].MultiplyPoint3x4(vertOPos) * weights.weight3;

			return finalPos;
		}

		private static Vector3 CalculateInverseVertexBindpose(List<Matrix4x4> bindPoses, BoneWeight weights, Vector3 vertWPos)
		{
			Vector3 finalPos = Vector3.zero;

			if (weights.weight0 > 0)
				finalPos += bindPoses[weights.boneIndex0].inverse.MultiplyPoint3x4(vertWPos) * weights.weight0;

			if (weights.weight1 > 0)
				finalPos += bindPoses[weights.boneIndex1].inverse.MultiplyPoint3x4(vertWPos) * weights.weight1;

			if (weights.weight2 > 0)
				finalPos += bindPoses[weights.boneIndex2].inverse.MultiplyPoint3x4(vertWPos) * weights.weight2;

			if (weights.weight3 > 0)
				finalPos += bindPoses[weights.boneIndex3].inverse.MultiplyPoint3x4(vertWPos) * weights.weight3;

			return finalPos;
		}

		#endregion

		#endregion

	}

}