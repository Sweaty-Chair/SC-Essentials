using UnityEngine;

// Code sourced from http://blog.s-schoener.com/2018-05-05-animation-curves/. Smart people math stuff

namespace SweatyChair
{

	/// <summary>
	/// Provides a numerically integrated version of a function.
	/// </summary>
	public class IntegrateFunc
	{
		private System.Func<float, float> _func;
		private float[] _values;
		private float _from, _to;

		/// <summary>
		/// Integrates a function on an interval. Use the steps parameter to control
		/// the precision of the numerical integration. Larger step values lead to
		/// better precision.
		/// </summary>
		public IntegrateFunc(System.Func<float, float> func,
							 float from, float to, int steps)
		{
			_values = new float[steps + 1];
			_func = func;
			_from = from;
			_to = to;
			ComputeValues();
		}

		private void ComputeValues()
		{
			int n = _values.Length;
			float segment = (_to - _from) / (n - 1);
			float lastY = _func(_from);
			float sum = 0;
			_values[0] = 0;
			for (int i = 1; i < n; i++) {
				float x = _from + i * segment;
				float nextY = _func(x);
				sum += segment * (nextY + lastY) / 2;
				lastY = nextY;
				_values[i] = sum;
			}
		}

		/// <summary>
		/// Evaluates the integrated function at any point in the interval.
		/// </summary>
		public float Evaluate(float x)
		{
			Debug.Assert(_from <= x && x <= _to);
			float t = Mathf.InverseLerp(_from, _to, x);
			int lower = (int)(t * _values.Length);
			int upper = (int)(t * _values.Length + .5f);
			if (lower == upper || upper >= _values.Length)
				return _values[lower];
			float innerT = Mathf.InverseLerp(lower, upper, t * _values.Length);
			return (1 - innerT) * _values[lower] + innerT * _values[upper];
		}

		/// <summary>
		/// Returns the total value integrated over the whole interval.
		/// </summary>
		public float Total
		{
			get {
				return _values[_values.Length - 1];
			}
		}
	}

	/// <summary>
	/// Utility to sample animation curves for probability Distributions
	/// </summary>
	public class CurveProbabilitySampler
	{
		private readonly AnimationCurve _densityCurve;
		private readonly IntegrateFunc _integratedDensity;

		public CurveProbabilitySampler(AnimationCurve curve, int integrationSteps = 100)
		{
			_densityCurve = curve;
			_integratedDensity = new IntegrateFunc(curve.Evaluate,
												   curve.keys[0].time,
												   curve.keys[curve.length - 1].time,
												   integrationSteps);
		}

		/// <summary>
		/// Takes a value s in [0, 1], scales it up to the interval
		/// [0, totalIntegratedValue] and computes its inverse.
		/// </summary>
		private float Invert(float s)
		{
			s *= _integratedDensity.Total;
			float lower = MinT;
			float upper = MaxT;
			const float precision = 0.00001f;
			while (upper - lower > precision) {
				float mid = (lower + upper) / 2f;
				float d = _integratedDensity.Evaluate(mid);
				if (d > s) {
					upper = mid;
				} else if (d < s) {
					lower = mid;
				} else {
					// unlikely :)
					return mid;
				}
			}

			return (lower + upper) / 2f;
		}

		public float TransformUnit(float unitValue)
		{
			return Invert(unitValue);
		}

		public float Sample()
		{
			return Invert(Random.value);
		}

		private float MinT
		{
			get { return _densityCurve.keys[0].time; }
		}

		private float MaxT
		{
			get { return _densityCurve.keys[_densityCurve.length - 1].time; }
		}
	}

}
