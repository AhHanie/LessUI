using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's horizontal slider functionality that provides a user-friendly API
    /// for creating interactive slider controls in the UI.
    /// </summary>
    public class Slider : UIElement
    {
        private float _value;
        private float _min;
        private float _max;
        private string _tooltip;
        private float _roundTo = -1f;

        /// <summary>
        /// Gets or sets the current value of the slider.
        /// </summary>
        public float Value
        {
            get => _value;
            set => _value = Mathf.Clamp(value, _min, _max);
        }

        /// <summary>
        /// Gets or sets the minimum value of the slider.
        /// </summary>
        public float Min
        {
            get => _min;
            set
            {
                _min = value;
                if (_value < _min)
                {
                    _value = _min;
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum value of the slider.
        /// </summary>
        public float Max
        {
            get => _max;
            set
            {
                _max = value;
                if (_value > _max)
                {
                    _value = _max;
                }
            }
        }

        /// <summary>
        /// Gets or sets the tooltip text to display when hovering over the slider.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        /// <summary>
        /// Gets or sets the rounding precision for the slider value. -1 disables rounding.
        /// </summary>
        public float RoundTo
        {
            get => _roundTo;
            set => _roundTo = value;
        }

        /// <summary>
        /// Gets the range between the maximum and minimum values.
        /// </summary>
        public float Range => _max - _min;

        /// <summary>
        /// Gets whether the slider is currently at its minimum value.
        /// </summary>
        public bool IsAtMin => Mathf.Approximately(_value, _min);

        /// <summary>
        /// Gets whether the slider is currently at its maximum value.
        /// </summary>
        public bool IsAtMax => Mathf.Approximately(_value, _max);

        /// <summary>
        /// Gets the current value as a percentage between 0 and 1.
        /// </summary>
        public float Percentage
        {
            get
            {
                if (Mathf.Approximately(Range, 0f))
                    return 0f;
                return (_value - _min) / Range;
            }
        }

        /// <summary>
        /// Event callback that is invoked when the slider value changes.
        /// The parameter contains the new slider value.
        /// </summary>
        public Action<float> OnValueChanged { get; set; }

        /// <summary>
        /// Creates a new slider with content-based sizing.
        /// </summary>
        /// <param name="value">The initial value of the slider</param>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">The maximum value of the slider</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public Slider(float value, float min, float max, Action<float> onValueChanged = null, UIElementOptions options = null)
            : base(SizeMode.Content, SizeMode.Content, options)
        {
            _min = min;
            _max = max;
            _value = Mathf.Clamp(value, min, max);
            OnValueChanged = onValueChanged;
        }

        /// <summary>
        /// Creates a new slider with fixed dimensions.
        /// </summary>
        /// <param name="value">The initial value of the slider</param>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">The maximum value of the slider</param>
        /// <param name="width">Fixed width of the slider</param>
        /// <param name="height">Fixed height of the slider</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public Slider(float value, float min, float max, float width, float height, Action<float> onValueChanged = null, UIElementOptions options = null)
            : base(width, height, options)
        {
            _min = min;
            _max = max;
            _value = Mathf.Clamp(value, min, max);
            OnValueChanged = onValueChanged;
        }

        /// <summary>
        /// Creates a new slider with specified width sizing mode.
        /// </summary>
        /// <param name="value">The initial value of the slider</param>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">The maximum value of the slider</param>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public Slider(float value, float min, float max, SizeMode widthMode, Action<float> onValueChanged = null, UIElementOptions options = null)
            : base(widthMode, SizeMode.Content, options)
        {
            _min = min;
            _max = max;
            _value = Mathf.Clamp(value, min, max);
            OnValueChanged = onValueChanged;
        }

        /// <summary>
        /// Creates a new slider with content-based sizing as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="value">The initial value of the slider</param>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">The maximum value of the slider</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public Slider(UIElement parent, float value, float min, float max, Action<float> onValueChanged = null, UIElementOptions options = null)
            : base(SizeMode.Content, SizeMode.Content, options)
        {
            _min = min;
            _max = max;
            _value = Mathf.Clamp(value, min, max);
            OnValueChanged = onValueChanged;
            parent?.AddChild(this);
        }

        /// <summary>
        /// Creates a new slider with specified width sizing mode as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="value">The initial value of the slider</param>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">The maximum value of the slider</param>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public Slider(UIElement parent, float value, float min, float max, SizeMode widthMode, Action<float> onValueChanged = null, UIElementOptions options = null)
            : base(widthMode, SizeMode.Content, options)
        {
            _min = min;
            _max = max;
            _value = Mathf.Clamp(value, min, max);
            OnValueChanged = onValueChanged;
            parent?.AddChild(this);
        }

        /// <summary>
        /// Sets the slider value to the specified percentage (0.0 to 1.0).
        /// </summary>
        /// <param name="percentage">The percentage value (0.0 to 1.0)</param>
        public void SetToPercentage(float percentage)
        {
            float clampedPercentage = Mathf.Clamp01(percentage);
            Value = _min + (Range * clampedPercentage);
        }

        /// <summary>
        /// Calculates the dynamic size of the slider based on its content.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            // Default slider size - reasonable width and standard height similar to RimWorld's slider
            Width = 120f;
            Height = 22f;
        }

        /// <summary>
        /// Renders the slider using RimWorld's native HorizontalSlider widget.
        /// </summary>
        protected override void RenderElement()
        {
            var rect = CreateRect();

            // Store original value to detect changes
            var originalValue = _value;

            // Use RimWorld's native HorizontalSlider widget
            _value = Widgets.HorizontalSlider(rect, _value, _min, _max, middleAlignment: true,
                label: null, leftAlignedLabel: null, rightAlignedLabel: null, roundTo: _roundTo);

            // If value changed, trigger callback
            if (!Mathf.Approximately(_value, originalValue))
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
        /// Creates a Unity Rect from the slider's position and size.
        /// </summary>
        /// <returns>A Rect representing the slider's bounds</returns>
        private Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }
    }
}