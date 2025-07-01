using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlacXGames.PersistSuite
{
    public class PlayerPrefsAPI
    {
        public static class Player
        {
            private const string fullname = "_flname";
            private const string firstname = "_fname";
            private const string lastname = "_lname";
            private const string dateofbirth = "_dob";
            private const string gender = "_gender";
            
            public static string FullName { get => PlayerPrefs.GetString(fullname); set => PlayerPrefs.SetString(fullname, value); }
            public static string FirstName { get => PlayerPrefs.GetString(firstname); set => PlayerPrefs.SetString(firstname, value); }
            public static string LastName { get => PlayerPrefs.GetString(lastname); set => PlayerPrefs.SetString(lastname, value); }
            public static string DateOfBirth { get => PlayerPrefs.GetString(dateofbirth); set => PlayerPrefs.SetString(dateofbirth, value); }
            public static string Gender { get => PlayerPrefs.GetString(gender); set => PlayerPrefs.SetString(gender, value); }
        }
        public static class Currency
        {
            private const string coin = "_coin";
            private const string gem = "_gem";
            
            public static string Coin { get => PlayerPrefs.GetString(coin); set => PlayerPrefs.SetString(coin, value); }
            public static string Gem { get => PlayerPrefs.GetString(gem); set => PlayerPrefs.SetString(gem, value); }
        }
        public static class Setting
        {
            private const string music = "_music";
            private const string sound = "_sound";
            private const string vibration = "_vibration";
            
            public static bool Music { get => bool.Parse(PlayerPrefs.GetString(music)); set => PlayerPrefs.SetString(music, value.ToString()); }
            public static bool Sound { get => bool.Parse(PlayerPrefs.GetString(sound)); set => PlayerPrefs.SetString(sound, value.ToString()); }
            public static bool Vibration { get => bool.Parse(PlayerPrefs.GetString(vibration)); set => PlayerPrefs.SetString(vibration, value.ToString()); }
        }
    }
}
