using System;
using System.Linq;
using UnityEngine;

namespace FortuneWheel.Scripts.Settings
{
    [Serializable]
    public class FortuneWheelAwardSettings
    {
        [Serializable]
        public class CurrencySprite
        {
            public string currency;
            public Sprite sprite;
        }

        [SerializeField] public CurrencySprite[] currencySprites;

        public Sprite GetSprite(string spriteCurrency)
        {
            return currencySprites.First(sprite => sprite.currency == spriteCurrency).sprite;
        }
    }
}