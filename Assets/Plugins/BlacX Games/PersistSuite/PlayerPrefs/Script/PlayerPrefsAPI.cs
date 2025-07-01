using System.Collections;
using System.Collections.Generic;

namespace BlacXGames.PersistSuite.PlayerPrefs
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
            
            public static string FullName { get => UnityEngine.PlayerPrefs.GetString(fullname); set => UnityEngine.PlayerPrefs.SetString(fullname, value); }
            public static string FirstName { get => UnityEngine.PlayerPrefs.GetString(firstname); set => UnityEngine.PlayerPrefs.SetString(firstname, value); }
            public static string LastName { get => UnityEngine.PlayerPrefs.GetString(lastname); set => UnityEngine.PlayerPrefs.SetString(lastname, value); }
            public static string DateOfBirth { get => UnityEngine.PlayerPrefs.GetString(dateofbirth); set => UnityEngine.PlayerPrefs.SetString(dateofbirth, value); }
            public static string Gender { get => UnityEngine.PlayerPrefs.GetString(gender); set => UnityEngine.PlayerPrefs.SetString(gender, value); }
        }
        public static class Currency
        {
            private const string coin = "_coin";
            private const string gem = "_gem";
            
            public static string Coin { get => UnityEngine.PlayerPrefs.GetString(coin); set => UnityEngine.PlayerPrefs.SetString(coin, value); }
            public static string Gem { get => UnityEngine.PlayerPrefs.GetString(gem); set => UnityEngine.PlayerPrefs.SetString(gem, value); }
        }
        public static class Setting
        {
            private const string music = "_music";
            private const string sound = "_sound";
            private const string vibration = "_vibration";
            
            public static bool Music { get => bool.Parse(UnityEngine.PlayerPrefs.GetString(music)); set => UnityEngine.PlayerPrefs.SetString(music, value.ToString()); }
            public static bool Sound { get => bool.Parse(UnityEngine.PlayerPrefs.GetString(sound)); set => UnityEngine.PlayerPrefs.SetString(sound, value.ToString()); }
            public static bool Vibration { get => bool.Parse(UnityEngine.PlayerPrefs.GetString(vibration)); set => UnityEngine.PlayerPrefs.SetString(vibration, value.ToString()); }
        }
    }
}
