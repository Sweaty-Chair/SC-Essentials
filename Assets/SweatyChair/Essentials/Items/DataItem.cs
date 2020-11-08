namespace SweatyChair
{
	
	public abstract class DataItem : Item
	{
	
		public BaseData data { get; protected set; }

		public DataItem(BaseData data)
		{
			this.data = data;
			amount = 1;
		}

		public DataItem(BaseData data, int amount)
		{
			this.data = data;
			this.amount = amount;
		}

		public override string ToString()
		{
			return string.Format("[DataItem: data={0} amount:{1}]", data, amount);
		}

	}

}