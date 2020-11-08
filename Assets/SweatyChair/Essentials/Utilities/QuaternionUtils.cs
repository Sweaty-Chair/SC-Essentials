using UnityEngine;

public static class QuaternionUtils  {

	/// <summary>
	/// A variant of rotate around which returns a quaternion rather than directly modifying the value
	/// </summary>
	/// <param name="objRotation"></param>
	/// <param name="axis"></param>
	/// <param name="angle"></param>
	/// <returns></returns>
	public static Quaternion RotateAround(Quaternion objRotation, Vector3 axis, float angle)
	{
		Quaternion rot = Quaternion.AngleAxis(angle, axis);			// get the desired rotation
		Quaternion ReturnRotation = objRotation * (Quaternion.Inverse(objRotation) * rot * objRotation);
		return ReturnRotation * Quaternion.Inverse(objRotation);
	}

}
