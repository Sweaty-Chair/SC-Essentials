using UnityEngine;

namespace SweatyChair
{

	// Sourced from https://answers.unity.com/questions/609385/type-for-layer-selection.html

	/// <summary>
	/// Specifies a single Unity Layer
	/// </summary>
	[System.Serializable]
	public struct Layer
	{

		#region Variables

		[SerializeField] private int m_LayerIndex;
		public int layerIndex
		{
			get { return m_LayerIndex; }
			set { Set(value); }
		}

		public int mask
		{
			get { return 1 << m_LayerIndex; }
		}

		#endregion

		#region Set

		public void Set(int index)
		{
			// Do error Checking
			if (index >= 0 && index < 32)
				m_LayerIndex = index;
			else
				Debug.LogError($"Cannot set layer. Layer index '{index}' is out of bounds of Layer Array");
		}

		#endregion

		#region Utility

		public string GetLayerName()
		{
			return LayerMask.LayerToName(m_LayerIndex);
		}

		#endregion

		#region Implicit Operators

		public static implicit operator int(Layer layer)
		{
			return layer.m_LayerIndex;
		}

		public static implicit operator Layer(int intVal)
		{
			Layer layer;
			layer.m_LayerIndex = intVal;
			return layer;
		}

		#endregion

	}

}
