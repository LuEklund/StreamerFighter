using UnityEngine;
namespace TwitchLib.Unity {
    public static class ColorExtension {
        public static Color ToUnity(this System.Drawing.Color color)
            => new(color.R, color.G, color.B, color.A);
    }
}