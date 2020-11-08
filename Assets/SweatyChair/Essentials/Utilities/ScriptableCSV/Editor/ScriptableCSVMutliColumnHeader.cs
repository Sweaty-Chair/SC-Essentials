using UnityEditor.IMGUI.Controls;

namespace SweatyChair.ScriptableCSV
{

	public class ScriptableCSVMutliColumnHeader : MultiColumnHeader
	{

		#region Constructor

		public ScriptableCSVMutliColumnHeader(MultiColumnHeaderState state) : base(state) { }

		#endregion

		#region ColumnHeaderClicked

		protected override void ColumnHeaderClicked(MultiColumnHeaderState.Column column, int columnIndex)
		{
			// If this column has just been selected, force our selection to be ascending and init our index
			if (state.sortedColumnIndex != columnIndex) {
				column.sortedAscending = true;
				state.sortedColumnIndex = columnIndex;

			} else {
				// Otherwise, if we are already sorted descending, we reset our column, otherwise we sort descending. Bascially this just lets us toggle our sorting off completely
				if (!column.sortedAscending)
					state.sortedColumnIndex = -1;
				else
					column.sortedAscending = false;

			}

			OnSortingChanged();
		}

		#endregion

	}

}
