using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's labeled numeric text field functionality that provides a user-friendly API
    /// for creating interactive numeric input controls with text labels in the UI.
    /// </summary>
    /// <typeparam name="T">The numeric type (int, float, double, etc.)</typeparam>
    public class LabeledTextFieldNumeric<T> : UIElement where T : struct
    {
        private T _value;
        private string _buffer;
        private T _min;
        private T _max;
        private string _label;
        private string _tooltip;
        private float _spacing = 6f; // Default spacing between label and text field

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
        /// Gets or sets the label text to display next to the numeric text field.
        /// </summary>
        public string Label
        {
            get => _label;
            set => _label = value;
        }

        /// <summary>
        /// Gets or sets the tooltip text to display when hovering over the labeled numeric text field.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        /// <summary>
        /// Gets or sets the spacing between the label and the numeric text field.
        /// </summary>
        public float Spacing
        {
            get => _spacing;
            set => _spacing = value;
        }

        /// <summary>
        /// Gets whether the labeled numeric text field has empty or whitespace-only label text.
        /// </summary>
        public bool IsLabelEmpty => string.IsNullOrWhiteSpace(_label);

        /// <summary>
        /// Event callback that is invoked when the numeric value changes.
        /// The parameter contains the new numeric value.
        /// </summary>
        public Action<T> OnValueChanged { get; set; }

        /// <summary>
        /// Creates a new labeled numeric text field with content-based sizing.
        /// </summary>
        /// <param name="label">The label text to display</param>
        /// <param name="value">The initial numeric value</param>
        /// <param name="buffer">The initial string buffer</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledTextFieldNumeric(string label, T value, string buffer, Action<T> onValueChanged = null, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _label = label ?? "";
            _value = value;
            _buffer = buffer ?? "";
            OnValueChanged = onValueChanged;
            SetDefaultMinMax();
        }

        /// <summary>
        /// Creates a new labeled numeric text field with specified width sizing mode.
        /// </summary>
        /// <param name="label">The label text to display</param>
        /// <param name="value">The initial numeric value</param>
        /// <param name="buffer">The initial string buffer</param>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledTextFieldNumeric(string label, T value, string buffer, SizeMode widthMode, Action<T> onValueChanged = null, UIElementOptions options = null) : base(widthMode, SizeMode.Content, options)
        {
            _label = label ?? "";
            _value = value;
            _buffer = buffer ?? "";
            OnValueChanged = onValueChanged;
            SetDefaultMinMax();
        }

        /// <summary>
        /// Creates a new labeled numeric text field with minimum and maximum values.
        /// </summary>
        /// <param name="label">The label text to display</param>
        /// <param name="value">The initial numeric value</param>
        /// <param name="buffer">The initial string buffer</param>
        /// <param name="min">The minimum allowed value</param>
        /// <param name="max">The maximum allowed value</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledTextFieldNumeric(string label, T value, string buffer, T min, T max, Action<T> onValueChanged = null, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _label = label ?? "";
            _value = value;
            _buffer = buffer ?? "";
            _min = min;
            _max = max;
            OnValueChanged = onValueChanged;
        }

        /// <summary>
        /// Creates a new labeled numeric text field with fixed dimensions.
        /// </summary>
        /// <param name="label">The label text to display</param>
        /// <param name="value">The initial numeric value</param>
        /// <param name="buffer">The initial string buffer</param>
        /// <param name="width">Fixed width of the labeled numeric text field</param>
        /// <param name="height">Fixed height of the labeled numeric text field</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledTextFieldNumeric(string label, T value, string buffer, float width, float height, Action<T> onValueChanged = null, UIElementOptions options = null) : base(width, height, options)
        {
            _label = label ?? "";
            _value = value;
            _buffer = buffer ?? "";
            OnValueChanged = onValueChanged;
            SetDefaultMinMax();
        }

        /// <summary>
        /// Creates a new labeled numeric text field with content-based sizing as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="label">The label text to display</param>
        /// <param name="value">The initial numeric value</param>
        /// <param name="buffer">The initial string buffer</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledTextFieldNumeric(UIElement parent, string label, T value, string buffer, Action<T> onValueChanged = null, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _label = label ?? "";
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
        /// Creates a new labeled numeric text field with specified width sizing mode as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="label">The label text to display</param>
        /// <param name="value">The initial numeric value</param>
        /// <param name="buffer">The initial string buffer</param>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledTextFieldNumeric(UIElement parent, string label, T value, string buffer, SizeMode widthMode, Action<T> onValueChanged = null, UIElementOptions options = null) : base(widthMode, SizeMode.Content, options)
        {
            _label = label ?? "";
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
        /// Calculates the dynamic size of the labeled numeric text field based on its content.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            // Height is based on line height
            float totalHeight = GetLineHeight();

            // Handle Fill mode
            if (WidthMode == SizeMode.Fill)
            {
                Height = Math.Max(1f, totalHeight);
                return;
            }

            // Handle empty label (Content mode)
            if (IsLabelEmpty)
            {
                // Just numeric text field size
                float textFieldWidth = GetTextFieldWidth();
                Width = Math.Max(1f, textFieldWidth);
                Height = Math.Max(1f, totalHeight);
                return;
            }

            // Calculate label width
            float labelWidth = GetTextWidth(_label);

            // Calculate text field width
            float textFieldWidthContent = GetTextFieldWidth();

            // Total width: label + spacing + text field
            float totalWidth = labelWidth + _spacing + textFieldWidthContent;

            Width = Math.Max(1f, totalWidth);
            Height = Math.Max(1f, totalHeight);
        }

        /// <summary>
        /// Gets the width for the text field portion, taking into account the sizing mode.
        /// </summary>
        /// <returns>The calculated text field width</returns>
        protected virtual float GetTextFieldWidth()
        {
            if (WidthMode == SizeMode.Fill)
            {
                if (Parent == null) return 0f;

                if (IsLabelEmpty)
                {
                    return Width; // Use our own width, not parent's width
                }

                float labelWidth = GetTextWidth(_label);
                return Math.Max(0f, Width - labelWidth - _spacing); // Use our own width
            }

            return 120f; // Default text field width for content mode
        }

        /// <summary>
        /// Renders the labeled numeric text field using RimWorld's UI system.
        /// </summary>
        protected override void RenderElement()
        {
            if (IsLabelEmpty)
            {
                // If no label, just render numeric text field
                RenderTextFieldOnly();
                return;
            }

            // Calculate label width and text field width
            float labelWidth = GetTextWidth(_label);
            float textFieldWidth = GetTextFieldWidth();

            // Create rects for label and text field
            var labelRect = new Rect(X, Y, labelWidth, Height);
            var textFieldRect = new Rect(X + labelWidth + _spacing, Y, textFieldWidth, Height);

            // Render the label
            Widgets.Label(labelRect, _label);

            // Store original value to detect changes
            T originalValue = _value;

            // Render the numeric text field using RimWorld's TextFieldNumeric
            Widgets.TextFieldNumeric(textFieldRect, ref _value, ref _buffer, ConvertToFloat(_min), ConvertToFloat(_max));

            // If value changed, trigger callback
            if (!_value.Equals(originalValue))
            {
                OnValueChanged?.Invoke(_value);
            }

            // Handle tooltip if present
            if (!string.IsNullOrEmpty(_tooltip))
            {
                var fullRect = CreateRect();
                TooltipHandler.TipRegion(fullRect, _tooltip);
            }
        }

        /// <summary>
        /// Renders just the numeric text field when there's no label.
        /// </summary>
        private void RenderTextFieldOnly()
        {
            // Create rect for just the text field
            var textFieldRect = CreateRect();

            // Store original value to detect changes
            T originalValue = _value;

            // Render the numeric text field
            Widgets.TextFieldNumeric(textFieldRect, ref _value, ref _buffer, ConvertToFloat(_min), ConvertToFloat(_max));

            // If value changed, trigger callback
            if (!_value.Equals(originalValue))
            {
                OnValueChanged?.Invoke(_value);
            }

            // Handle tooltip if present
            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(textFieldRect, _tooltip);
            }
        }

        /// <summary>
        /// Creates a Unity Rect from the labeled numeric text field's position and size.
        /// </summary>
        /// <returns>A Rect representing the labeled numeric text field's bounds</returns>
        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }

        /// <summary>
        /// Gets the width of the specified text.
        /// </summary>
        /// <param name="text">The text to measure</param>
        /// <returns>The width of the text</returns>
        private float GetTextWidth(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0f;

            var originalWordWrap = Verse.Text.WordWrap;
            try
            {
                Verse.Text.WordWrap = false;
                var size = Verse.Text.CalcSize(text);
                return size.x;
            }
            finally
            {
                Verse.Text.WordWrap = originalWordWrap;
            }
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