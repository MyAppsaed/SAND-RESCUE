using System;
using UnityEngine;

namespace SandRescue
{
    [Serializable]
    public struct BilingualText
    {
        [Tooltip("Stable key used by a future Unity Localization String Table.")]
        public string key;
        [TextArea] public string arabicFallback;
        [TextArea] public string englishFallback;

        public string Get(Language language) => language == Language.Arabic ? arabicFallback : englishFallback;
    }

    public enum Language { Arabic, English }

    public static class SandRescueLocalization
    {
        public static Language Current => (Language)PlayerPrefs.GetInt("sand_rescue_language", (int)Language.Arabic);
        public static void SetLanguage(Language language) => PlayerPrefs.SetInt("sand_rescue_language", (int)language);
        public static string Resolve(BilingualText text) => text.Get(Current);
    }
}
