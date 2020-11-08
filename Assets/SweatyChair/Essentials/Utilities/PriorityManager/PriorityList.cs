using System.Collections;
using System.Collections.Generic;
using System;

public class PriorityList<T> : IEnumerable<T> where T : IComparable<T>
{
	private List<T> data;

	public PriorityList()
	{
		this.data = new List<T>();
	}

	public int Add(T item)
	{
		if (data.Count == 0 || item.CompareTo(data[0]) <= 0)
		{
			data.Insert(0, item);
			return 0;
		}

		if (item.CompareTo(data[data.Count - 1]) >= 0)
		{
			data.Insert(data.Count, item);
			return data.Count - 1;
		}

		int i1 = 0;
		int i2 = data.Count - 1;

		while (i1 < i2 - 1)
		{
			int i = (i1 + i2) / 2;
			if (item.CompareTo(data[i]) > 0)
			{
				i1 = i;
			}
			else
			{
				i2 = i;
			}
		}

		data.Insert(i1 + 1, item);
		return i1 + 1;
	}

	public T this[int index]
	{
		get { return data[index]; }
	}

	public void RemoveAt(int index)
	{
		data.RemoveAt(index);
	}

	public void Remove(T item)
	{
		data.Remove(item);
	}

	public void RemoveAll(Predicate<T> predicate)
	{
		data.RemoveAll(predicate);
	}

	public bool Contains(T item)
	{
		return data.Contains(item);
	}

	public bool Exists(Predicate<T> predicate)
	{
		return data.Exists(predicate);
	}

	public int Count
	{
		get { return data.Count; }
	}

	public T Find(Predicate<T> predicate)
	{
		return data.Find(predicate);
	}

	public T FindLast(Predicate<T> predicate)
	{
		return data.FindLast(predicate);
	}

	public int FindIndex(Predicate<T> predicate)
	{
		return data.FindIndex(predicate);
	}

	public void Clear()
	{
		data.Clear();
	}

	public IEnumerator<T> GetEnumerator()
	{
		return data.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public override string ToString()
	{
		string s = "";
		for (int i = 0; i < data.Count; i++)
		{
			s += data[i].ToString() + " ";
		}
		return s;
	}

	public bool CheckConsistent()
	{
		for (int i = 0; i < data.Count - 2; i++)
		{
			T t1 = data[i];
			T t2 = data[i + 1];
			if (t1.CompareTo(t2) > 0)
				return false;
		}

		return true;
	}

}