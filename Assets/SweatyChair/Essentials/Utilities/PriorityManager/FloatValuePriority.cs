using System.Collections;
using System.Collections.Generic;
using System;

public class FloatValuePriority : IComparable<FloatValuePriority>
{
	public float val;
	public int priority;

	public FloatValuePriority(float value, int rating){
		val = value;
		priority = rating;
	}

	public int CompareTo(FloatValuePriority other)
	{
		// If other is not a valid object reference, this instance is greater.
		if (other == null) return 1;
		// return higher priority
		return priority.CompareTo(other.priority);
	}
}