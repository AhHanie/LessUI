using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's slider functionality that provides a user-friendly API
    /// for creating labeled slider controls in the UI, with labels rendered above the slider.
    /// </summary>
    public class LabeledSlider : UIElement
    {
        private string _label;
        private float _value;
        private float _min;
        private float _max;
        private string _tooltip;
        private string _format;
        private float _roundTo;
        private string _leftAlignedLabel;
        private string _rightAlignedLabel;

        /// <summary>
        /// Gets or sets the main label text to display above the center of the slider.
        /// </summary>
        public string Label
        {
            get => _label;
            set => _label = value;
        }

        /// <summary>
        /// Gets or sets the current value of the slider.
        /// </summary>
        public float Value
        {
            get => _value;
            set
            {
                var clampedValue = Mathf.Clamp(value, _min, _max);
                if (Math.Abs(_value - clampedValue) > float.Epsilon)
                {
                    _value = clampedValue;
                    OnValueChanged?.Invoke(_value);
                }
            }
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
                    Value = _min;
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
                    Value = _max;
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
        /// Gets or sets the format string for displaying the slider value.
        /// </summary>
        public string Format
        {
            get => _format;
            set => _format = value;
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
        /// Gets or sets the left-aligned label text to display above the left side of the slider.
        /// </summary>
        public string LeftAlignedLabel
        {
            get => _leftAlignedLabel;
            set => _leftAlignedLabel = value;
        }

        /// <summary>
        /// Gets or sets the right-aligned label text to display above the right side of the slider.
        /// </summary>
        public string RightAlignedLabel
        {
            get => _rightAlignedLabel;
            set => _rightAlignedLabel = value;
        }

        /// <summary>
        /// Gets whether the main label is empty or whitespace-only.
        /// </summary>
        public bool IsLabelEmpty => string.IsNullOrWhiteSpace(_label);

        /// <summary>
        /// Gets whether any labels (main, left, or right) are present.
        /// </summary>
        public bool HasAnyLabels => !IsLabelEmpty || !string.IsNullOrWhiteSpace(_leftAlignedLabel) || !string.IsNullOrWhiteSpace(_rightAlignedLabel);

        /// <summary>
        /// Event callback that is invoked when the slider value changes.
        /// </summary>
        public Action<float> OnValueChanged { get; set; }

        /// <summary>
        /// Creates a new labeled slider with content-based sizing.
        /// </summary>
        /// <param name="label">The main label text to display</param>
        /// <param name="value">The initial value of the slider</param>
        /// <param name="min">The minimum value (default: 0)</param>
        /// <param name="max">The maximum value (default: 100)</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledSlider(string label, float value, float min = 0f, float max = 100f, Action<float> onValueChanged = null, UIElementOptions options = null)
            : base(SizeMode.Content, SizeMode.Content, options)
        {
            _label = label ?? "";
            _min = min;
            _max = max;
            _roundTo = -1f;
            OnValueChanged = onValueChanged;
            Value = value; // Use property to ensure clamping
        }

        /// <summary>
        /// Creates a new labeled slider with fixed dimensions.
        /// </summary>
        /// <param name="label">The main label text to display</param>
        /// <param name="value">The initial value of the slider</param>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="width">Fixed width of the slider</param>
        /// <param name="height">Fixed height of the slider</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledSlider(string label, float value, float min, float max, float width, float height, Action<float> onValueChanged = null, UIElementOptions options = null)
            : base(width, height, options)
        {
            _label = label ?? "";
            _min = min;
            _max = max;
            _roundTo = -1f;
            OnValueChanged = onValueChanged;
            Value = value; // Use property to ensure clamping
        }

        /// <summary>
        /// Creates a new labeled slider with specified width sizing mode.
        /// </summary>
        /// <param name="label">The main label text to display</param>
        /// <param name="value">The initial value of the slider</param>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledSlider(string label, float value, float min, float max, SizeMode widthMode, Action<float> onValueChanged = null, UIElementOptions options = null)
            : base(widthMode, SizeMode.Content, options)
        {
            _label = label ?? "";
            _min = min;
            _max = max;
            _roundTo = -1f;
            OnValueChanged = onValueChanged;
            Value = value; // Use property to ensure clamping
        }

        /// <summary>
        /// Creates a new labeled slider with content-based sizing as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="label">The main label text to display</param>
        /// <param name="value">The initial value of the slider</param>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledSlider(UIElement parent, string label, float value, float min, float max, Action<float> onValueChanged = null, UIElementOptions options = null)
            : base(SizeMode.Content, SizeMode.Content, options)
        {
            _label = label ?? "";
            _min = min;
            _max = max;
            _roundTo = -1f;
            OnValueChanged = onValueChanged;
            Value = value; // Use property to ensure clamping
            parent?.AddChild(this);
        }

        /// <summary>
        /// Creates a new labeled slider with specified width sizing mode as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="label">The main label text to display</param>
        /// <param name="value">The initial value of the slider</param>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="onValueChanged">Callback function to execute when value changes</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledSlider(UIElement parent, string label, float value, float min, float max, SizeMode widthMode, Action<float> onValueChanged = null, UIElementOptions options = null)
            : base(widthMode, SizeMode.Content, options)
        {
            _label = label ?? "";
            _min = min;
            _max = max;
            _roundTo = -1f;
            OnValueChanged = onValueChanged;
            Value = value; // Use property to ensure clamping
            parent?.AddChild(this);
        }

        /// <summary>
        /// Calculates the dynamic size of the labeled slider based on its content.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            base.CalculateDynamicSize();
            var sliderMinWidth = 120f; // Standard slider width
            var sliderHeight = 22f; // Standard slider height
            var labelHeight = HasAnyLabels ? Text.LineHeight : 0f;
            var totalHeight = sliderHeight + labelHeight;

            if (WidthMode == SizeMode.Fill)
            {
                Height = totalHeight;
                return;
            }

            // For content mode, calculate the minimum width needed for labels
            float labelWidth = 0f;
            if (!IsLabelEmpty)
            {
                labelWidth = Math.Max(labelWidth, Text.CalcSize(_label).x);
            }
            if (!string.IsNullOrEmpty(_leftAlignedLabel))
            {
                labelWidth = Math.Max(labelWidth, Text.CalcSize(_leftAlignedLabel).x);
            }
            if (!string.IsNullOrEmpty(_rightAlignedLabel))
            {
                labelWidth = Math.Max(labelWidth, Text.CalcSize(_rightAlignedLabel).x);
            }

            var totalWidth = Math.Max(sliderMinWidth, labelWidth);
            Width = totalWidth;
            Height = totalHeight;
        }

        /// <summary>
        /// Creates a Unity Rect from the labeled slider's position and size.
        /// </summary>
        /// <returns>A Rect representing the labeled slider's bounds</returns>
        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }

        protected override void RenderElement()
        {
            var rect = CreateRect();
            var labelHeight = HasAnyLabels ? Text.LineHeight : 0f;

            // Render labels above the slider if any are present
            if (HasAnyLabels)
            {
                var labelRect = new Rect(rect.x, rect.y, rect.width, labelHeight);
                RenderLabels(labelRect);
            }

            // Render the slider below the labels
            var sliderRect = new Rect(rect.x, rect.y + labelHeight, rect.width, rect.height - labelHeight);

            var originalValue = _value;
            _value = Widgets.HorizontalSlider(sliderRect, _value, _min, _max, middleAlignment: true,
                label: null, leftAlignedLabel: null, rightAlignedLabel: null, roundTo: _roundTo);

            if (!Mathf.Approximately(_value, originalValue))
            {
                OnValueChanged?.Invoke(_value);
            }

            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        /// <summary>
        /// Renders the labels above the slider track.
        /// </summary>
        /// <param name="labelRect">The rect area for rendering labels</param>
        private void RenderLabels(Rect labelRect)
        {
            // Render left-aligned label
            if (!string.IsNullOrEmpty(_leftAlignedLabel))
            {
                var leftLabelRect = new Rect(labelRect.x, labelRect.y, labelRect.width * 0.3f, labelRect.height);
                Text.Anchor = TextAnchor.UpperLeft;
                Widgets.Label(leftLabelRect, _leftAlignedLabel);
            }

            // Render center label (main label)
            if (!IsLabelEmpty)
            {
                var centerLabelRect = new Rect(labelRect.x + labelRect.width * 0.3f, labelRect.y, labelRect.width * 0.4f, labelRect.height);
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(centerLabelRect, _label);
            }

            // Render right-aligned label
            if (!string.IsNullOrEmpty(_rightAlignedLabel))
            {
                var rightLabelRect = new Rect(labelRect.x + labelRect.width * 0.7f, labelRect.y, labelRect.width * 0.3f, labelRect.height);
                Text.Anchor = TextAnchor.UpperRight;
                Widgets.Label(rightLabelRect, _rightAlignedLabel);
            }

            // Reset text anchor
            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}