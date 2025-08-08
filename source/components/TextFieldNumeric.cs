using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's numeric text field functionality that provides a user-friendly API
    /// for creating interactive numeric input controls in the UI.
    /// </summary>
    /// <typeparam name="T">The numeric type (int, float, double, etc.)</typeparam>
    public class TextFieldNumeric<T> : UIElement where T : struct
    {
        private T _value;
        private string _buffer;
        private T _min;
        private T _max;
        private string _tooltip;

        /// <summary>
        /// Gets or sets the numeric value of the text field.
        /// </summary>
        public T Value
        {
            get => _value;
            set => _value = value;
        }

        /// <summary>
        /// Gets or sets the string buffer used internally by RimWorld for text editing.
        /// </summary>
        public string Buffer
        {
            get => _buffer;
            set => _buffer = value ?? "";
        }

        /// <summary>
        /// Gets or sets the minimum allowed value.
        /// </summary>
        public T Min
        {
            get => _min;
            set => _min = value;
        }

        /// <summary>
        /// Gets or sets the maximum allowed value.
        /// </summary>
        public T Max
        {
            get => _max;
            set => _max = value;
        }

        /// <summary>
        /// Gets or sets the tooltip text to display when hovering over the text field.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        /// <summary>
        /// Event callback that is invoked when the numeric value changes.
        /// The parameter contains the new numeric value.
        /// </summary>
        public Action<T> OnValueChanged { get; set; }

        /// <summary>
        /// Creates a new numeric text field with content-based sizing.
        /// </summary>
        /// <param name="value">The initial numeric value</param>
        /// <param name="buffer">The initial string buffer</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public TextFieldNumeric(T value, string buffer, Action<T> onValueChanged = null, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _value = value;
            _buffer = buffer ?? "";
            OnValueChanged = onValueChanged;
            SetDefaultMinMax();
        }

        /// <summary>
        /// Creates a new numeric text field with specified width sizing mode.
        /// </summary>
        /// <param name="value">The initial numeric value</param>
        /// <param name="buffer">The initial string buffer</param>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public TextFieldNumeric(T value, string buffer, SizeMode widthMode, Action<T> onValueChanged = null, UIElementOptions options = null) : base(widthMode, SizeMode.Content, options)
        {
            _value = value;
            _buffer = buffer ?? "";
            OnValueChanged = onValueChanged;
            SetDefaultMinMax();
        }

        /// <summary>
        /// Creates a new numeric text field with minimum and maximum values.
        /// </summary>
        /// <param name="value">The initial numeric value</param>
        /// <param name="buffer">The initial string buffer</param>
        /// <param name="min">The minimum allowed value</param>
        /// <param name="max">The maximum allowed value</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public TextFieldNumeric(T value, string buffer, T min, T max, Action<T> onValueChanged = null, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _value = value;
            _buffer = buffer ?? "";
            _min = min;
            _max = max;
            OnValueChanged = onValueChanged;
        }

        /// <summary>
        /// Creates a new numeric text field with fixed dimensions.
        /// </summary>
        /// <param name="value">The initial numeric value</param>
        /// <param name="buffer">The initial string buffer</param>
        /// <param name="width">Fixed width of the text field</param>
        /// <param name="height">Fixed height of the text field</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public TextFieldNumeric(T value, string buffer, float width, float height, Action<T> onValueChanged = null, UIElementOptions options = null) : base(width, height, options)
        {
            _value = value;
            _buffer = buffer ?? "";
            OnValueChanged = onValueChanged;
            SetDefaultMinMax();
        }

        /// <summary>
        /// Creates a new numeric text field with content-based sizing as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="value">The initial numeric value</param>
        /// <param name="buffer">The initial string buffer</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public TextFieldNumeric(UIElement parent, T value, string buffer, Action<T> onValueChanged = null, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _value = value;
            _buffer = buffer ?? "";
            OnValueChanged = onValueChanged;
            SetDefaultMinMax();
            if (parent != null)
            {
                parent.AddChild(this);
            }
        }

        /// <summary>
        /// Creates a new numeric text field with specified width sizing mode as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="value">The initial numeric value</param>
        /// <param name="buffer">The initial string buffer</param>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public TextFieldNumeric(UIElement parent, T value, string buffer, SizeMode widthMode, Action<T> onValueChanged = null, UIElementOptions options = null) : base(widthMode, SizeMode.Content, options)
        {
            _value = value;
            _buffer = buffer ?? "";
            OnValueChanged = onValueChanged;
            SetDefaultMinMax();
            if (parent != null)
            {
                parent.AddChild(this);
            }
        }

        /// <summary>
        /// Sets default min and max values based on the numeric type.
        /// </summary>
        private void SetDefaultMinMax()
        {
            // Default to 0 and 1 billion, matching RimWorld's defaults
            if (typeof(T) == typeof(int))
            {
                _min = (T)(object)0;
                _max = (T)(object)1000000000;
            }
            else if (typeof(T) == typeof(float))
            {
                _min = (T)(object)0f;
                _max = (T)(object)1000000000f;
            }
            else if (typeof(T) == typeof(double))
            {
                _min = (T)(object)0.0;
                _max = (T)(object)1000000000.0;
            }
            else
            {
                // For other types, try to set reasonable defaults
                _min = default(T);
                _max = (T)(object)1000000000;
            }
        }

        /// <summary>
        /// Calculates the dynamic size of the numeric text field.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            // Height based on line height
            float height = GetLineHeight();

            // Handle Fill mode
            if (WidthMode == SizeMode.Fill)
            {
                Height = Math.Max(1f, height);
                return;
            }

            // Default width for numeric text field (Content mode)
            float width = 120f;

            width = Math.Max(1f, width);
            Height = Math.Max(1f, height);
        }

        /// <summary>
        /// Renders the numeric text field using RimWorld's UI system.
        /// </summary>
        protected override void RenderElement()
        {
            // Create rect for rendering
            var rect = CreateRect();

            // Store original value to detect changes
            T originalValue = _value;

            // Render the numeric text field using RimWorld's TextFieldNumeric
            Widgets.TextFieldNumeric(rect, ref _value, ref _buffer, ConvertToFloat(_min), ConvertToFloat(_max));

            // If value changed, trigger callback
            if (!_value.Equals(originalValue))
            {
                OnValueChanged?.Invoke(_value);
            }

            // Handle tooltip if present
            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        /// <summary>
        /// Creates a Unity Rect from the text field's position and size.
        /// </summary>
        /// <returns>A Rect representing the text field's bounds</returns>
        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }

        /// <summary>
        /// Gets the height of a single line of text.
        /// </summary>
        /// <returns>The line height</returns>
        private float GetLineHeight()
        {
            return Verse.Text.LineHeight;
        }

        /// <summary>
        /// Converts a numeric value to float for RimWorld's TextFieldNumeric method.
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The value as a float</returns>
        private float ConvertToFloat(T value)
        {
            return Convert.ToSingle(value);
        }
    }
}