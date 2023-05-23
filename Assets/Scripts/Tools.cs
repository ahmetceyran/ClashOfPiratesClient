namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Tools : MonoBehaviour
    {

        public static Color GenerateRandomColor()
        {
            return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }

        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("0x", "").Replace("#", "");
            byte a = 255;
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return (Color)(new Color32(r, g, b, a));
        }

        public static string ColorToHex(Color color)
        {
            Color32 color32 = (Color32)color;
            return color32.r.ToString("X2") + color32.g.ToString("X2") + color32.b.ToString("X2");
        }

    }
}