using SweatyChair.ScriptableCSV;
using UnityEngine;
using UnityEngine.Serialization;

namespace SweatyChair
{

	public abstract class SOData : ScriptableObject
	{

		#region Variables

		// ID field
		[ScriptableCSVColumn(30, 30)]
		public int id;

		[Header("DEBUG")]
		[FormerlySerializedAs("DEBUG_isLoadable")]
		[ScriptableCSVColumn(40, 40)]
		public bool load = true;

		#endregion

		#region Constructor

		public SOData() { }

		#endregion

	}

}
