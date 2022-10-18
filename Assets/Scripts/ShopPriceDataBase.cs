using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShopPriceDataBase : MonoBehaviour
{
	[SerializeField] protected List<ItemPriceData> shopPricesList = new List<ItemPriceData>();
	public int[] shopPrices;

	private void Awake()
	{
		PrepareData();
	}

	public abstract void PrepareData();


	public ItemPriceData GetItemPriceData()
    {
		foreach (var ItemData in shopPricesList)
		{
			if (name.Contains(ItemData.name))
				return ItemData;
		}

		return null;
	}

}
