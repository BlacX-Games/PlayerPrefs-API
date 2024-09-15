using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsAPI
{
    private const string name = "_name";
    private const string coin = "_coin";
    private const string gem = "_gem";
    
    public static string Name { get => PlayerPrefs.GetString(name); set => PlayerPrefs.SetString(name, value); }
    public static string Coin { get => PlayerPrefs.GetString(coin); set => PlayerPrefs.SetString(coin, value); }
    public static string Gem { get => PlayerPrefs.GetString(gem); set => PlayerPrefs.SetString(gem, value); }
}
