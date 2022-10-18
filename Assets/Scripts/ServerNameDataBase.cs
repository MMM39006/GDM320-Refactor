using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Defective.JSON;

public class ServerNameDataBase : ShopPriceDataBase
{
    [SerializeField] string url;

    public override void PrepareData()
    {
        StartCoroutine(SendWebRequest());
    }

    IEnumerator SendWebRequest()
    {
        var webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        var downloadedText = webRequest.downloadHandler.text;
        var jsonObject = new JSONObject(downloadedText);
        foreach (var json in jsonObject.list)
        {
            var UserNam = "";
            json.GetField(ref UserNam, "UserNam");

            var UserID = 0;
            json.GetField(ref UserID, "UserID");

            var newItemData = new ItemPriceData();
            newItemData.name = UserNam;
            newItemData.price = UserID;

            shopPricesList.Add(newItemData);
        }
    }
}
