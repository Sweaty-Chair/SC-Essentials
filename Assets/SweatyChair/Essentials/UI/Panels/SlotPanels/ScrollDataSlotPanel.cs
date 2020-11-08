namespace SweatyChair.UI
{

	/*****
	 * A scrollable panel that automatcially initialized for a given GameData and holds a number of DataSlots.
	 *****/
	public abstract class ScrollDataSlotPanel<T> : ScrollSlotPanel<T> where T : BaseData, new()
	{

		// Automatically get all data from database on awake if not specify
		protected virtual void Awake()
		{
			InitSlots(DatabaseManager.GetDatas<T>().ToArray());
		}

	}

}