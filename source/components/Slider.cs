using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class Slider : UIElement
    {
        private float _value = 0f;
        private float _min = 0f;
        private float _max = 100f;
        private string _tooltip = "";
        private string _label = "";
        private float _roundTo = -1f;
        private bool _changed = false;

        public float Value
        {
            get => _value;
            set => _value = Mathf.Clamp(value, _min, _max);
        }

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

        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        public string Label
        {
            get => _label;
            set => _label = value;
        }

        public float RoundTo
        {
            get => _roundTo;
            set => _roundTo = value;
        }

        public bool Changed
        {
            get => _changed;
            set => _changed = value;
        }

        public float Range => _max - _min;

        public bool IsAtMin => Mathf.Approximately(_value, _min);

        public bool IsAtMax => Mathf.Approximately(_value, _max);

        public bool IsEmpty => string.IsNullOrWhiteSpace(_label);

        public float Percentage
        {
            get
            {
                if (Mathf.Approximately(Range, 0f))
                    return 0f;
                return (_value - _min) / Range;
            }
        }

        public Slider(
            float? value = null,
            float? min = null,
            float? max = null,
            string label = null,
            string tooltip = null,
            float? roundTo = null,
            float? x = null,
            float? y = null,
            float? width = null,
            float? height = null,
            SizeMode? widthMode = null,
            SizeMode? heightMode = null,
            Align? alignment = null,
            bool? showBorders = null,
            Color? borderColor = null,
            int? borderThickness = null)
            : base(x, y, width, height, widthMode, heightMode, alignment, showBorders, borderColor, borderThickness)
        {
            _min = min ?? 0f;
            _max = max ?? 100f;
            _value = Mathf.Clamp(value ?? 0f, _min, _max);
            _label = label ?? "";
            _tooltip = tooltip ?? "";
            _roundTo = roundTo ?? -1f;
        }

        public void SetToPercentage(float percentage)
        {
            float clampedPercentage = Mathf.Clamp01(percentage);
            Value = _min + (Range * clampedPercentage);
        }

        protected override void CalculateElementSize()
        {
            base.CalculateElementSize();

            if (WidthMode == SizeMode.Content)
            {
                WidthCalculated = true;
                if (IsEmpty)
                {
                    Width = 120f;
                }
                else
                {
                    var originalWordWrap = Verse.Text.WordWrap;
                    Verse.Text.WordWrap = false;
                    var textSize = Verse.Text.CalcSize(_label);
                    Width = Math.Max(120f, textSize.x);
                    Verse.Text.WordWrap = originalWordWrap;
                }
            }

            if (HeightMode == SizeMode.Content)
            {
                HeightCalculated = true;
                if (IsEmpty)
                {
                    Height = 22f;
                }
                else
                {
                    Height = GetLineHeight() + 22f;
                }
            }
        }

        protected override void RenderElement()
        {
            var rect = CreateRect();
            var labelHeight = IsEmpty ? 0f : GetLineHeight();

            if (!IsEmpty)
            {
                var labelRect = new Rect(rect.x, rect.y, rect.width, labelHeight);
                var originalAnchor = Verse.Text.Anchor;
                Verse.Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(labelRect, _label);
                Verse.Text.Anchor = originalAnchor;
            }

            var sliderRect = new Rect(rect.x, rect.y + labelHeight, rect.width, rect.height - labelHeight);

            var originalValue = _value;

            _value = Widgets.HorizontalSlider(sliderRect, _value, _min, _max, middleAlignment: true,
                label: null, leftAlignedLabel: null, rightAlignedLabel: null, roundTo: _roundTo);

            if (!Mathf.Approximately(_value, originalValue))
            {
                _changed = true;
            }
            else if (_changed)
            {
                _changed = false;
            }

            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }

        private float GetLineHeight()
        {
            return Verse.Text.LineHeight;
        }
    }
}