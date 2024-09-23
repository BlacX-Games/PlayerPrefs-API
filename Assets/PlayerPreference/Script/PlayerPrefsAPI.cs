using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsAPI
{
    private const string fullname = "_flname";
    private const string firstname = "_fname";
    private const string lastname = "_lname";
    private const string dateofbirth = "_dob";
    private const string coin = "_coin";
    private const string gem = "_gem";
    
    public static string FullName { get => PlayerPrefs.GetString(fullname); set => PlayerPrefs.SetString(fullname, value); }
    public static string FirstName { get => PlayerPrefs.GetString(firstname); set => PlayerPrefs.SetString(firstname, value); }
    public static string LastName { get => PlayerPrefs.GetString(lastname); set => PlayerPrefs.SetString(lastname, value); }
    public static string DateOfBirth { get => PlayerPrefs.GetString(dateofbirth); set => PlayerPrefs.SetString(dateofbirth, value); }
    public static string Coin { get => PlayerPrefs.GetString(coin); set => PlayerPrefs.SetString(coin, value); }
    public static string Gem { get => PlayerPrefs.GetString(gem); set => PlayerPrefs.SetString(gem, value); }
}
