using System.Collections;
using System.Collections.Generic;
using Defective.JSON;
using UnityEngine;

public class LocalDataBase : ShopPriceDataBase
{
    [SerializeField] TextAsset jsonfile;

    public override void PrepareData()
    {
        var jsonObject = new JSONObject(jsonfile.text);
        foreach(var json in jsonObject.list)
        {
            var ItemName = "";
            json.GetField(ref ItemName, "ItemName");

            var Price = 0;
            json.GetField(ref Price, "Price");

            var newItemData = new ItemPriceData();
            newItemData.name = ItemName;
            newItemData.price = Price;

            shopPricesList.Add(newItemData);
        }
    }
}
