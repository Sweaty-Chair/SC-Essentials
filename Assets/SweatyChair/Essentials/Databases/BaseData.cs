namespace SweatyChair
{

	/// <summary>
	/// Base abstract class of the data stored in different databases, each Database class contain a list of this.
	/// Unity 2019.4 notes: Unity introduced GameData.Util namespace and breaking the original class name 'Data', use 'BaseData' for now.
	/// </summary>
	public abstract class BaseData
	{

		// Generic ID field
		public int id { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseData"/> class. It is empty waiting to be feed data by <see cref="FeedData(string[] data)"/> method.
		/// </summary>
		public BaseData()
		{
		}

		/// <summary>
		/// Abstract method that feeds the data to this data class.
		/// </summary>
		public abstract bool FeedData(string[] data);

	}

}