using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SweatyChair
{
	
	public class ShopDatabase : Database<ShopData>
	{

		private static Dictionary<ItemType, List<ShopData>> _dataDictByItemType = new Dictionary<ItemType, List<ShopData>>();

		protected override void PostDatabaseRead()
		{
			foreach (ShopData sd in _dataDict.Values) {
				ItemType itemType = sd.item.type;
				if (!_dataDictByItemType.ContainsKey(itemType))
					_dataDictByItemType.Add(itemType, new List<ShopData>());
				_dataDictByItemType[itemType].Add(sd);
			}
		}

		public static ShopData GetData(int id)
		{
			return DatabaseManager.GetData<ShopData>(id);
		}

		public static ShopData GetDataByProductId(string productId)
		{
			var dataList = DatabaseManager.GetDatas<ShopData>();
			foreach (ShopData shopData in dataList) {
				if (shopData.productId == productId)
					return shopData;
			}
			return null;
		}

		public static List<ShopData> GetDatasByItemType(ItemType itemType)
		{
			if (_dataDictByItemType.ContainsKey(itemType))
				return _dataDictByItemType[itemType];
			return new List<ShopData>();
		}

		public static ShopData GetDataByRewardString(string rewardString)
		{
			var dataList = DatabaseManager.GetDatas<ShopData>();
			foreach (ShopData shopData in dataList) {
				if (shopData.item.typeString == rewardString)
					return shopData;
			}
			return null;
		}

		public static ShopData GetCheapestData(List<ShopData> list)
		{
			List<ShopData> dataList = list.Where(x => x.cost.type != ItemType.RewardedVideo).ToList();
			if (dataList == null || dataList.Count < 1) {
				Debug.LogErrorFormat("ShopDatabase:GetCheapestItem - not found");
				return null;
			}
			return dataList.OrderBy(x => x.cost.amount).ToList()[0];
		}

		#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/Shop/Print Database")]
		private static void PrintShopDatabase()
		{
			var dataList = DatabaseManager.GetDatas<ShopData>();
			DebugUtils.Log(dataList, "dataList");
		}

		#endif

	}

}