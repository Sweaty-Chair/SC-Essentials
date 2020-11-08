using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweatyChair
{

	#region Plane Class

	public class RaycastPlane
	{

		//Plane GameData
		public List<Vector3> vertices { get; private set; }

		public Vector3 center { get; private set; }

		public Vector3 normalVector { get; private set; }
		public Vector3 upVector { get; private set; }
		public Vector3 rightVector { get; private set; }

		#region Constructor

		public RaycastPlane() {
			vertices = new List<Vector3> { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };

			RecalculateVectors();
			RecalculateCenter();
		}

		public RaycastPlane(Vector3 bottomRightVert, Vector3 bottomLeftVert, Vector3 topRightVert) {
			Vector3 center = ((bottomLeftVert + topRightVert) * 0.5f);

			vertices = new List<Vector3> { bottomRightVert, bottomLeftVert, center - (bottomRightVert - center), topRightVert };            //Store our vertices

			RecalculateVectors();
			RecalculateCenter();
		}

		public RaycastPlane(Vector3 bottomRightVert, Vector3 bottomLeftVert, Vector3 topLeftVert, Vector3 topRightVert) {
			vertices = new List<Vector3> { bottomRightVert, bottomLeftVert, topLeftVert, topRightVert };            //Store our vertices

			RecalculateVectors();
			RecalculateCenter();
		}

		#endregion

		#region Recalculate

		private void RecalculateVectors() {
			normalVector = RayCastUtils.GetPlaneNormalFromVertexes(vertices[0], vertices[1], vertices[2]);    //Calculate the surface normal of our plane given our vertexes

			upVector = (((vertices[2] + vertices[3]) / 2f) - ((vertices[0] + vertices[1]) / 2f)).normalized;
			rightVector = (((vertices[0] + vertices[3]) / 2f) - ((vertices[1] + vertices[2]) / 2f)).normalized;
		}

		private void RecalculateCenter() {
			center = (vertices[0] + vertices[1] + vertices[2] + vertices[3]) * 0.25f;          //Calculate the center of our plane
		}

		#endregion

		#region Resize

		public void UpdateVertices(Vector3 vertice1, Vector3 vertice2, Vector3 vertice3, Vector3 vertice4) {
			vertices[0] = vertice1;
			vertices[1] = vertice2;
			vertices[2] = vertice3;
			vertices[3] = vertice4;

			RecalculateCenter();
			RecalculateVectors();
		}

		#endregion

		#region Query

		public bool IsPointWithinPlane(Vector3 point) {
			float u1, v1, w1;
			float u2, v2, w2;
			GetBarycentric(point, vertices[0], vertices[1], vertices[2], out u1, out v1, out w1);
			GetBarycentric(point, vertices[0], vertices[2], vertices[3], out u2, out v2, out w2);

			return IsBarycentricInside(u1, v1, w1) || IsBarycentricInside(u2, v2, w2);
		}

		#endregion

		#region Barycentric

		private void GetBarycentric(Vector3 p, Vector3 a, Vector3 b, Vector3 c, out float u, out float v, out float w) {
			Vector3 v0 = b - a, v1 = c - a, v2 = p - a;
			float d00 = Vector3.Dot(v0, v0);
			float d01 = Vector3.Dot(v0, v1);
			float d11 = Vector3.Dot(v1, v1);
			float d20 = Vector3.Dot(v2, v0);
			float d21 = Vector3.Dot(v2, v1);
			float denom = d00 * d11 - d01 * d01;
			v = (d11 * d20 - d01 * d21) / denom;
			w = (d00 * d21 - d01 * d20) / denom;
			u = 1.0f - v - w;
		}

		private bool IsBarycentricInside(float u, float v, float w) {
			return (u >= 0.0f) && (u <= 1.0f) && (v >= 0.0f) && (v <= 1.0f) && (w >= 0.0f);
		}

		#endregion

		#region Editor Debugs

		public void DrawNormalsGizmo(float normalLength = 1f) {
			Gizmos.color = new Color(0, 0, 1);
			Gizmos.DrawLine(center, center + normalVector * normalLength);
		}

		public void DrawUpVectorGizmo(float normalLength = 1f) {
			Gizmos.color = new Color(0, 1, 0);
			Gizmos.DrawLine(center, center + upVector * normalLength);
		}

		public void DrawRightVectorGizmo(float normalLength = 1f) {
			Gizmos.color = new Color(1, 0, 0);
			Gizmos.DrawLine(center, center + rightVector * normalLength);
		}

		public void DrawOutlineGizmos(Color color) {
			Gizmos.color = color;

			Gizmos.DrawLine(vertices[0], vertices[1]);
			Gizmos.DrawLine(vertices[1], vertices[2]);
			Gizmos.DrawLine(vertices[2], vertices[3]);
			Gizmos.DrawLine(vertices[3], vertices[0]);
		}

		#endregion
	}

	#endregion

	#region Line Intersection Hit struct

	public struct LineIntersectionHit
	{
		public Ray ray;
		public Vector3 point;
		public Vector3 normal;
		public float distance;
		public float dotProduct;

		/// <summary>
		/// A Null hit struct filled with dummy data.
		/// </summary>
		public static LineIntersectionHit NullLineIntersectionHit {
			get {
				return new LineIntersectionHit {
					ray = new Ray(),
					point = Vector3.zero,
					normal = Vector3.zero,
					distance = 0,
					dotProduct = 0,
				};
			}
		}

	}

	#endregion

	public static class RayCastUtils
	{

		/// <summary>
		/// Returns a planes normal in world co-ordinates from three provided vertexes. (Make sure to follow left hand rule)
		/// </summary>
		/// <param name="point1">First vertex of our normal in world space</param>
		/// <param name="point2">Second vertex of our normal in world space</param>
		/// <param name="point3">Third vertex of our normal in world space</param>
		/// <returns></returns>
		public static Vector3 GetPlaneNormalFromVertexes(Vector3 point1, Vector3 point2, Vector3 point3) {
			Vector3 direction = Vector3.Cross(point2 - point1, point3 - point1);    //Calculate the cross product of our vertexes
			return Vector3.Normalize(direction);        //Return the normalized direction vector as our normal
		}

		#region Ray / Plane Intersection

		/// <summary>
		/// Raycasts given an origin and a direction and calculates whether our ray hits anywhere within our defined plane
		/// </summary>
		/// <param name="origin">The origin point of our ray</param>
		/// <param name="direction">The direction we are our ray towards</param>
		/// <param name="vertex1">The first vertex position of our plane</param>
		/// <param name="vertex2">The second vertex position of our plane</param>
		/// <param name="vertex3">The third vertex position of our plane</param>
		/// <param name="vertex4">The fourth vertex position of our plane</param>
		/// <param name="hit">The Line intersection hit with all data returned</param>
		/// <param name="hit"></param>
		/// <returns></returns>
		public static bool RaycastRayPlaneIntersection(Vector3 origin, Vector3 direction, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 vertex4, out LineIntersectionHit hit) {
			Ray ray = new Ray(origin, direction);
			return RaycastRayPlaneIntersection(ray, vertex1, vertex2, vertex3, vertex4, out hit);
		}

		/// <summary>
		/// Given an Ray, will raycast and calculate whether our ray hits anywhere within our defined plane
		/// </summary>
		/// <param name="ray">The ray that we are projecting from</param>
		/// <param name="vertex1">The first vertex position of our plane</param>
		/// <param name="vertex2">The second vertex position of our plane</param>
		/// <param name="vertex3">The third vertex position of our plane</param>
		/// <param name="vertex4">The fourth vertex position of our plane</param>
		/// <param name="hit">The Line intersection hit with all data returned</param>
		/// <returns></returns>
		public static bool RaycastRayPlaneIntersection(Ray ray, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 vertex4, out LineIntersectionHit hit) {
			RaycastPlane plane = new RaycastPlane(vertex1, vertex2, vertex3, vertex4);
			return RaycastRayPlaneIntersection(ray, plane, out hit);
		}

		/// <summary>
		/// Given an Ray, will raycast and calculate whether our ray hits anywhere within our defined plane
		/// </summary>
		/// <param name="ray">The ray that we are projecting from</param>
		/// <param name="vertex1">The first vertex position of our plane</param>
		/// <param name="vertex2">The second vertex position of our plane</param>
		/// <param name="vertex3">The third vertex position of our plane</param>
		/// <param name="vertex4">The fourth vertex position of our plane</param>
		/// <param name="hit">The Line intersection hit with all data returned</param>
		/// <returns></returns>
		public static bool RaycastRayPlaneIntersection(Ray ray, RaycastPlane plane, out LineIntersectionHit hit) {
			hit = LineIntersectionHit.NullLineIntersectionHit;  //Set our hit info to default values

			float hitDotProduct = Vector3.Dot(plane.normalVector, ray.direction);              //Find the dot product for our hit normal
			if (hitDotProduct < 0.0001f && hitDotProduct > -0.0001f) { return false; }  //If our plane doesnt intersect or intersects at an extremely shallow angle, return false.

			float hitDistance = Vector3.Dot(plane.center - ray.origin, plane.normalVector) / hitDotProduct; //Get the distance at which we collided

			if (hitDistance > 0.0001f) {        //If our ray is not coming fom within the plane and we are not hitting the plane with our reverse side of our ray.

				Vector3 hitPoint = ray.origin + (ray.direction * hitDistance);  //Calculate where in world space our point hit the plane

				if (!plane.IsPointWithinPlane(hitPoint)) { return false; }   //Calculate if our point is within the bounds of our plane

				//Populate our data
				hit.ray = ray;
				hit.point = hitPoint += (plane.normalVector * 0.001f);  //Add a small buffer to our raycast so it doesnt go behind our object due to floating point limitations
				hit.normal = plane.normalVector;
				hit.distance = hitDistance;
				hit.dotProduct = hitDotProduct;

				return true;    //Return that we hit our object

			} else {
				return false;   //We hit nothing
			}
		}

		#endregion

		#region Get Centroid Of Shapecast

		public static Vector3 GetCollisionCentroid(this RaycastHit hit, Ray ray) {
			return ray.origin + (ray.direction.normalized * hit.distance);
		}


		#endregion

	}
}
