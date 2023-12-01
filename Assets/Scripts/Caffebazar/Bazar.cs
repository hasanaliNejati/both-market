using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class Bazar : MarketManager, IStoreListener

{
    private IStoreController m_StoreController;


    struct BuyRequest
    {
        public Action success;
        public Action<string> failure;

        public BuyRequest( Action success, Action<string> failure)
        {
            this.success = success;
            this.failure = failure;
        }
    }

    Dictionary<string,BuyRequest> requests = new Dictionary<string,BuyRequest>();

    bool initialized = false;

    private void Start()
    {
        if (Instance == this)
        {
            Init();
            transform.parent = null;
            DontDestroyOnLoad(this);
        }
    }

    public void Init()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //Your products IDs. They should match the ids of your products in your store.
        //Add products that will be purchasable and indicate its type.
        foreach (var item in keys)
        {
            builder.AddProduct(item, ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        initialized = true;
        m_StoreController = controller;

    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        StartCoroutine(Retry());
    }

    IEnumerator Retry()
    {
        yield return new WaitForSeconds(5);
        Init();
    }



    public override void Buy(string key, Action success, Action<string> error)
    {
        if (requests.ContainsKey(key))
            requests[key] = new BuyRequest(success, error);
        else
            requests.Add(key, new BuyRequest(success, error));
        m_StoreController.InitiatePurchase(key);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        if(requests.ContainsKey(product.definition.id))
            requests[product.definition.id].failure.Invoke(failureReason.ToString());
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (requests.ContainsKey(purchaseEvent.purchasedProduct.definition.id))
        {
            requests[purchaseEvent.purchasedProduct.definition.id].success.Invoke();
            requests.Remove(purchaseEvent.purchasedProduct.definition.id);
        }
        //We return Complete, informing IAP that the processing on our side is done and   the transaction can be closed.
        return PurchaseProcessingResult.Complete;
    }
}
