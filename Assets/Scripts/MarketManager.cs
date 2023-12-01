using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    public static MarketManager _Instance;
    public static MarketManager Instance { get { return _Instance ? _Instance : _Instance = FindObjectOfType<MarketManager>(); } }

    public string[] keys;

    public virtual void Buy(string key, Action success, Action<string> error)
    {
        if (string.IsNullOrEmpty(key))
        {
            error.Invoke("error");
        }
        else
        {
            success.Invoke();

        }

    }

}
