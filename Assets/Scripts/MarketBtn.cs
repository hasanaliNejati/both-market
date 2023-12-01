using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MarketBtn : MonoBehaviour
{

    public string key;

    public UnityEvent purchasSuccessfuly;
    public UnityEvent purchasFailed;

    public void Click()
    {

        MarketManager.Instance.Buy(key, () =>
        {
            print("ok");
            purchasSuccessfuly.Invoke();
        }, (string s) =>
        {
            print("no" + s);
            purchasFailed.Invoke();
        });
    }

}
