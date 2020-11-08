using UnityEngine;

namespace SweatyChair
{

	public static class CurveUtils
	{

		#region Probability Curve Evaluations

		public static float EvaluateProbabilityDistribution(this AnimationCurve curve)
		{
			return new CurveProbabilitySampler(curve).Sample();
		}

		#endregion

	}

}
