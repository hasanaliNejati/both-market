using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyketPlugin;
using System.Linq;

public class MyketCode : MarketManager
{


    [SerializeField] string marketKey = "";
    struct BuyRequest
    {
        public Action success;
        public Action<string> failure;

        public BuyRequest(Action success, Action<string> failure)
        {
            this.success = success;
            this.failure = failure;
        }
    }

    Dictionary<string, BuyRequest> requests = new Dictionary<string, BuyRequest>();


    bool initialized = false;

    private void Start()
    {
        IABEventManager.billingSupportedEvent += IABEventManager_billingSupportedEvent;
        IABEventManager.billingNotSupportedEvent += IABEventManager_billingNotSupportedEvent;
        IABEventManager.purchaseSucceededEvent += IABEventManager_purchaseSucceededEvent;
        IABEventManager.purchaseFailedEvent += IABEventManager_purchaseFailedEvent;
        


        Init();
    }

    private void IABEventManager_purchaseFailedEvent(string obj)
    {
        if (requests.Count >= 0)
        {
            string c_Key = requests.Last().Key;
            requests[c_Key].failure.Invoke(obj);
            requests.Remove(c_Key);  

        }
    }

    private void IABEventManager_purchaseSucceededEvent(MyketPurchase obj)
    {
        if (requests.ContainsKey(obj.ProductId))
        {
            requests[obj.ProductId].success.Invoke();
            MyketIAB.consumeProduct(obj.ProductId);
            requests.Remove(obj.ProductId);
        }
    }

    private void IABEventManager_billingNotSupportedEvent(string obj)
    {
        StartCoroutine(Retry());
    }

    IEnumerator Retry()
    {
        yield return new WaitForSeconds(5);
        Init();
    }


    private void IABEventManager_billingSupportedEvent()
    {
        initialized = true;

    }

    public void Init()
    {
        MyketIAB.init(marketKey);
    }


    public override void Buy(string key, Action success, Action<string> error)
    {
        if (requests.ContainsKey(key))
            requests[key] = new BuyRequest(success, error);
        else
            requests.Add(key, new BuyRequest(success, error));


        MyketIAB.purchaseProduct(key);
    }

}
