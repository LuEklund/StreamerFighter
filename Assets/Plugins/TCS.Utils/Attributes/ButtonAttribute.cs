namespace TCS.Utils {
    /// <summary>
    /// Apply to a parameterless method to draw a button in the Inspector.
    /// Optional: label override, mode gating, enable-if condition, and height.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class ButtonAttribute : Attribute
    {
        public enum Mode
        {
            Always,
            EditorOnly,
            PlayModeOnly
        }

        /// <summary>Label shown on the button. If null/empty, uses the method name in spaced Title Case.</summary>
        public readonly string Label;

        /// <summary>When the button should be enabled/rendered.</summary>
        public readonly Mode ShowMode;

        /// <summary>Optional: Name of a bool field/property/method used to enable/disable the button.</summary>
        public readonly string EnableIf;

        /// <summary>Optional: Button height in pixels (defaults to EditorGUIUtility.singleLineHeight * 1.3f).</summary>
        public readonly float Height;

        public ButtonAttribute(
            string label = null,
            Mode showMode = Mode.Always,
            string enableIf = null,
            float height = 0f)
        {
            Label = label;
            ShowMode = showMode;
            EnableIf = enableIf;
            Height = height;
        }
    }
}