using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's IntRange widget that provides a user-friendly API
    /// for creating integer range sliders in the UI.
    /// </summary>
    public class IntRangeSlider : UIElement
    {
        private int _min;
        private int _max;
        private string _tooltip;
        private int _controlId;

        /// <summary>
        /// Gets or sets the minimum value of the range slider.
        /// </summary>
        public int Min
        {
            get => _min;
            set => _min = value;
        }

        /// <summary>
        /// Gets or sets the maximum value of the range slider.
        /// </summary>
        public int Max
        {
            get => _max;
            set => _max = value;
        }

        /// <summary>
        /// Gets or sets the lower bound value of the range.
        /// </summary>
        public int LowerValue
        {
            get => RangeBox.Value.min;
            set
            {
                int newValue = ClampToRange(value, _min, _max);
                if (newValue > RangeBox.Value.max)
                {
                    newValue = RangeBox.Value.max;
                }

                if (RangeBox.Value.min != newValue)
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
        public int UpperValue
        {
            get => RangeBox.Value.max;
            set
            {
                int newValue = ClampToRange(value, _min, _max);
                if (newValue < RangeBox.Value.min)
                {
                    newValue = RangeBox.Value.min;
                }

                if (RangeBox.Value.max != newValue)
                {
                    var currentRange = RangeBox.Value;
                    currentRange.max = newValue;
                    RangeBox.Value = currentRange;
                }
            }
        }

        /// <summary>
        /// Gets the StrongBox containing the IntRange value, allowing for shared references.
        /// </summary>
        public StrongBox<IntRange> RangeBox { get; }

        /// <summary>
        /// Gets the span of the current range (upper value - lower value).
        /// </summary>
        public int Range => RangeBox.Value.max - RangeBox.Value.min;

        /// <summary>
        /// Gets whether the current range is valid (lower value <= upper value).
        /// </summary>
        public bool IsValidRange => RangeBox.Value.min <= RangeBox.Value.max;

        /// <summary>
        /// Gets the total possible range span (max - min).
        /// </summary>
        public int MinRange => _max - _min;

        /// <summary>
        /// Gets or sets the tooltip text for the range slider.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        /// <summary>
        /// Initializes a new instance of the IntRangeSlider with content-based sizing.
        /// </summary>
        /// <param name="min">The minimum value of the range</param>
        /// <param name="max">The maximum value of the range</param>
        /// <param name="range">A StrongBox containing the IntRange for shared reference</param>
        /// <exception cref="ArgumentNullException">Thrown when range is null</exception>
        public IntRangeSlider(int min, int max, StrongBox<IntRange> range)
            : base(SizeMode.Content, SizeMode.Content)
        {
            _min = min;
            _max = max;

            RangeBox = range ?? throw new ArgumentNullException(nameof(range));
            _controlId = GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the IntRangeSlider with fixed sizing.
        /// </summary>
        /// <param name="min">The minimum value of the range</param>
        /// <param name="max">The maximum value of the range</param>
        /// <param name="width">The fixed width of the slider</param>
        /// <param name="height">The fixed height of the slider</param>
        /// <param name="range">A StrongBox containing the IntRange for shared reference</param>
        /// <exception cref="ArgumentNullException">Thrown when range is null</exception>
        public IntRangeSlider(int min, int max, float width, float height, StrongBox<IntRange> range)
            : base(width, height)
        {
            _min = min;
            _max = max;

            RangeBox = range ?? throw new ArgumentNullException(nameof(range));
            _controlId = GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the IntRangeSlider with specified width mode.
        /// </summary>
        /// <param name="min">The minimum value of the range</param>
        /// <param name="max">The maximum value of the range</param>
        /// <param name="widthMode">The width sizing mode</param>
        /// <param name="range">A StrongBox containing the IntRange for shared reference</param>
        /// <param name="options">Additional UI element options</param>
        /// <exception cref="ArgumentNullException">Thrown when range is null</exception>
        public IntRangeSlider(int min, int max, SizeMode widthMode, StrongBox<IntRange> range, UIElementOptions options = null)
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
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            // RimWorld's IntRange widget default size - reasonable width and standard height
            Width = 200f;
            Height = 200f;
        }

        /// <summary>
        /// Renders the integer range slider using RimWorld's native IntRange widget.
        /// </summary>
        protected override void RenderElement()
        {
            var rect = CreateRect();

            // Use RimWorld's native IntRange widget
            Widgets.IntRange(rect, _controlId, ref RangeBox.Value, _min, _max);

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
        public void SetValues(int lowerValue, int upperValue)
        {
            int clampedLower = ClampToRange(lowerValue, _min, _max);
            int clampedUpper = ClampToRange(upperValue, _min, _max);

            if (clampedLower > clampedUpper)
            {
                clampedLower = clampedUpper;
            }

            RangeBox.Value = new IntRange(clampedLower, clampedUpper);
        }

        /// <summary>
        /// Clamps a value to the specified range.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum allowed value</param>
        /// <param name="max">The maximum allowed value</param>
        /// <returns>The clamped value</returns>
        private int ClampToRange(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}