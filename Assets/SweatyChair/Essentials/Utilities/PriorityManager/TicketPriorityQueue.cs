using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Creates a queue based off a priority, to store a value, Returns an Identifier back so that we are able to remove data safely, even if it changes
/// </summary>
/// <typeparam name="T"></typeparam>
public class TicketPriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
{

	#region Variables

	public T value { get { return GetValue(); } }   //Return the last item in our array, which ends up being the data with the highest priority

	private Dictionary<int, PriorityPair<T>> _idDictionary;
	private List<PriorityPair<T>> _data;    //List of Keyvalue pair, with the Key being the ID of our data, and value, is our data we want to sort by

	private T _defaultValue;

	private Func<PriorityPair<T>, T> _sortingOverride;

	#endregion

	#region Constructor

	public TicketPriorityQueue(T defaultValue, Func<PriorityPair<T>, T> customSort = null)
	{
		_idDictionary = new Dictionary<int, PriorityPair<T>>();
		_data = new List<PriorityPair<T>>();
		_defaultValue = defaultValue;

		_sortingOverride = customSort;

		Reset();
	}

	#endregion

	#region Add to queue

	public int Add(int identifier, float priority, T item)
	{
		int id = 0; //Initialize our initial counter to 0, just cause

		if (_idDictionary.Count != 0) {

			//If our ID already exists in the dictionary
			if (_idDictionary.ContainsKey(identifier)) {    //If we already contain that identifier, we modify its value. and return

				//Update our data stored with the new data provided
				PriorityPair<T> pair = _idDictionary[identifier];   //Get our current value from the dictionary
				SortandRemoveFromQueue(pair);                       //Remove our data from the queue and sort.

				pair = new PriorityPair<T>(priority, item);         //Get our new data passed into our function
				_idDictionary[identifier] = pair;
				SortAndAddToQueue(pair);                            //Then add to our queue and sort appropriately

				return identifier;                                  //And return our identifier

			}
			else {
				//If our ID does not already exist in the dictionary, Add new data
				List<int> keys = _idDictionary.Keys.ToList();   //Get all our active keys so we are able to find the next ID
				int max = keys.Max() + 2;                       //Get the max number in our array
				List<int> missingNumbers = Enumerable.Range(0, max).Except(keys).ToList();  //We then get all missing / reuseable numbers from our data
				id = missingNumbers.Min();  //Then we get the smallest number in our List, to reuse for our ID

			}
		}

		PriorityPair<T> finalPair = new PriorityPair<T>(priority, item);
		_idDictionary.Add(id, finalPair);    //Add our ID and reference to the dictionary
		SortAndAddToQueue(finalPair);        //Then we add our new pair to the queue and sort

		return id;

	}

	private void SortAndAddToQueue(PriorityPair<T> item)
	{
		_data.Add(item);

		IOrderedEnumerable<PriorityPair<T>> sortedData = _data.OrderBy(data => data.priority);

		// Do our custom sorting if we are going to
		if (_sortingOverride != null)
			sortedData = sortedData.ThenBy(_sortingOverride);

		_data = sortedData.ToList();
	}

	#endregion

	#region Remove from queue

	public int Remove(int identifier)
	{
		//If our dictionary contains the identifier
		if (_idDictionary.ContainsKey(identifier)) {

			PriorityPair<T> pair = _idDictionary[identifier];   //Get our current value from the dictionary
			SortandRemoveFromQueue(pair);                       //Remove our data from the queue and sort.

			_idDictionary.Remove(identifier);   //Then we remove ourself
		}

		return -1;
	}

	private void SortandRemoveFromQueue(PriorityPair<T> item)
	{
		if (_data.Contains(item)) {
			_data.Remove(item);
		}
	}

	#endregion

	#region Get Value

	private T GetValue()
	{
		return _data[_data.Count - 1].item;
	}

	#endregion

	#region Reset

	public void Reset()
	{
		_idDictionary.Clear();
		_data.Clear();
		//Add default
		_data.Add(new PriorityPair<T>(-1, _defaultValue));
	}

	#endregion

	#region Get Enumerator

	public IEnumerator<T> GetEnumerator()
	{
		return _data.Select(data => data.item).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	#endregion
}

#region PriorityPair

public class PriorityPair<T>
{

	public float priority;
	public T item;

	public PriorityPair(float priority, T item)
	{
		this.priority = priority;
		this.item = item;
	}
}

#endregion
