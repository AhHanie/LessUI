using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's FloatRange widget that provides a user-friendly API
    /// for creating float range sliders in the UI.
    /// </summary>
    public class FloatRangeSlider : UIElement
    {
        private float _min;
        private float _max;
        private string _tooltip;
        private string _labelKey;
        private ToStringStyle _valueStyle = ToStringStyle.FloatTwo;
        private float _gap = 0f;
        private GameFont _sliderLabelFont = GameFont.Small;
        private Color? _sliderLabelColor = null;
        private float _roundTo = 0f;
        private int _controlId;

        /// <summary>
        /// Gets or sets the minimum value of the range slider.
        /// </summary>
        public float Min
        {
            get => _min;
            set => _min = value;
        }

        /// <summary>
        /// Gets or sets the maximum value of the range slider.
        /// </summary>
        public float Max
        {
            get => _max;
            set => _max = value;
        }

        /// <summary>
        /// Gets or sets the lower bound value of the range.
        /// </summary>
        public float LowerValue
        {
            get => RangeBox.Value.min;
            set
            {
                float newValue = ClampToRange(value, _min, _max);
                if (newValue > RangeBox.Value.max - _gap)
                {
                    newValue = RangeBox.Value.max - _gap;
                }

                if (!Mathf.Approximately(RangeBox.Value.min, newValue))
                {
                    var currentRange = RangeBox.Value;
                    currentRange.min = newValue;
                    RangeBox.Value = currentRange;
                }
            }
        }

        /// <summary>
        /// Gets or sets the upper bound value of the range.
        /// </summary>
        public float UpperValue
        {
            get => RangeBox.Value.max;
            set
            {
                float newValue = ClampToRange(value, _min, _max);
                if (newValue < RangeBox.Value.min + _gap)
                {
                    newValue = RangeBox.Value.min + _gap;
                }

                if (!Mathf.Approximately(RangeBox.Value.max, newValue))
                {
                    var currentRange = RangeBox.Value;
                    currentRange.max = newValue;
                    RangeBox.Value = currentRange;
                }
            }
        }

        /// <summary>
        /// Gets the StrongBox containing the FloatRange value, allowing for shared references.
        /// </summary>
        public StrongBox<FloatRange> RangeBox { get; }

        /// <summary>
        /// Gets the span of the current range (upper value - lower value).
        /// </summary>
        public float Range => RangeBox.Value.max - RangeBox.Value.min;

        /// <summary>
        /// Gets whether the current range is valid (lower value <= upper value).
        /// </summary>
        public bool IsValidRange => RangeBox.Value.min <= RangeBox.Value.max;

        /// <summary>
        /// Gets the total possible range span (max - min).
        /// </summary>
        public float TotalRange => _max - _min;

        /// <summary>
        /// Gets or sets the tooltip text for the range slider.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        /// <summary>
        /// Gets or sets the label key for the range slider display.
        /// </summary>
        public string LabelKey
        {
            get => _labelKey;
            set => _labelKey = value;
        }

        /// <summary>
        /// Gets or sets the string formatting style for the range values.
        /// </summary>
        public ToStringStyle ValueStyle
        {
            get => _valueStyle;
            set => _valueStyle = value;
        }

        /// <summary>
        /// Gets or sets the minimum gap that must be maintained between min and max values.
        /// </summary>
        public float Gap
        {
            get => _gap;
            set => _gap = value;
        }

        /// <summary>
        /// Gets or sets the font used for the slider label.
        /// </summary>
        public GameFont SliderLabelFont
        {
            get => _sliderLabelFont;
            set => _sliderLabelFont = value;
        }

        /// <summary>
        /// Gets or sets the color used for the slider label.
        /// </summary>
        public Color? SliderLabelColor
        {
            get => _sliderLabelColor;
            set => _sliderLabelColor = value;
        }

        /// <summary>
        /// Gets or sets the rounding precision for slider values. 0 means no rounding.
        /// </summary>
        public float RoundTo
        {
            get => _roundTo;
            set => _roundTo = value;
        }

        /// <summary>
        /// Initializes a new instance of the FloatRangeSlider with content-based sizing.
        /// </summary>
        /// <param name="min">The minimum value of the range</param>
        /// <param name="max">The maximum value of the range</param>
        /// <param name="range">A StrongBox containing the FloatRange for shared reference</param>
        /// <exception cref="ArgumentNullException">Thrown when range is null</exception>
        public FloatRangeSlider(float min, float max, StrongBox<FloatRange> range)
            : base(SizeMode.Content, SizeMode.Content)
        {
            _min = min;
            _max = max;

            RangeBox = range ?? throw new ArgumentNullException(nameof(range));
            _controlId = GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the FloatRangeSlider with fixed sizing.
        /// </summary>
        /// <param name="min">The minimum value of the range</param>
        /// <param name="max">The maximum value of the range</param>
        /// <param name="width">The fixed width of the slider</param>
        /// <param name="height">The fixed height of the slider</param>
        /// <param name="range">A StrongBox containing the FloatRange for shared reference</param>
        /// <exception cref="ArgumentNullException">Thrown when range is null</exception>
        public FloatRangeSlider(float min, float max, float width, float height, StrongBox<FloatRange> range)
            : base(width, height)
        {
            _min = min;
            _max = max;

            RangeBox = range ?? throw new ArgumentNullException(nameof(range));
            _controlId = GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the FloatRangeSlider with specified width mode.
        /// </summary>
        /// <param name="min">The minimum value of the range</param>
        /// <param name="max">The maximum value of the range</param>
        /// <param name="widthMode">The width sizing mode</param>
        /// <param name="range">A StrongBox containing the FloatRange for shared reference</param>
        /// <param name="options">Additional UI element options</param>
        /// <exception cref="ArgumentNullException">Thrown when range is null</exception>
        public FloatRangeSlider(float min, float max, SizeMode widthMode, StrongBox<FloatRange> range, UIElementOptions options = null)
            : base(widthMode, SizeMode.Content, options)
        {
            _min = min;
            _max = max;

            RangeBox = range ?? throw new ArgumentNullException(nameof(range));
            _controlId = GetHashCode();
        }

        /// <summary>
        /// Calculates the dynamic size of the range slider based on its content.
        /// </summary>
        public override void CalculateDynamicSize()
        {
            // RimWorld's FloatRange widget default size - reasonable width and standard height
            Width = 200f;
            Height = 31f; // RangeControlIdealHeight from Widgets
        }

        /// <summary>
        /// Renders the float range slider using RimWorld's native FloatRange widget.
        /// </summary>
        protected override void RenderElement()
        {
            var rect = CreateRect();

            // Use RimWorld's native FloatRange widget
            Widgets.FloatRange(rect, _controlId, ref RangeBox.Value, _min, _max, _labelKey, _valueStyle, _gap, _sliderLabelFont, _sliderLabelColor, _roundTo);

            // Handle tooltip if present
            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        /// <summary>
        /// Creates a Unity Rect from the range slider's position and size.
        /// </summary>
        /// <returns>A Rect representing the range slider's bounds</returns>
        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }

        /// <summary>
        /// Sets both lower and upper values simultaneously.
        /// </summary>
        /// <param name="lowerValue">The new lower bound value</param>
        /// <param name="upperValue">The new upper bound value</param>
        public void SetValues(float lowerValue, float upperValue)
        {
            float clampedLower = ClampToRange(lowerValue, _min, _max);
            float clampedUpper = ClampToRange(upperValue, _min, _max);

            // Ensure gap is maintained
            if (clampedUpper - clampedLower < _gap)
            {
                float midpoint = (clampedLower + clampedUpper) / 2f;
                clampedLower = midpoint - _gap / 2f;
                clampedUpper = midpoint + _gap / 2f;

                // Re-clamp after gap adjustment
                clampedLower = ClampToRange(clampedLower, _min, _max - _gap);
                clampedUpper = ClampToRange(clampedUpper, _min + _gap, _max);
            }

            RangeBox.Value = new FloatRange(clampedLower, clampedUpper);
        }

        /// <summary>
        /// Sets the range to span the entire possible range.
        /// </summary>
        public void SetToFullRange()
        {
            SetValues(_min, _max);
        }

        /// <summary>
        /// Sets the range to a single point at the specified value.
        /// </summary>
        /// <param name="value">The value to set both min and max to</param>
        public void SetToSingleValue(float value)
        {
            float clampedValue = ClampToRange(value, _min, _max);
            RangeBox.Value = new FloatRange(clampedValue, clampedValue);
        }

        /// <summary>
        /// Clamps a value to the specified range.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum allowed value</param>
        /// <param name="max">The maximum allowed value</param>
        /// <returns>The clamped value</returns>
        private float ClampToRange(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}