using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class SliderInt : UIElement
    {
        private StrongBox<int> _value = null;
        private int _min = 0;
        private int _max = 100;
        private string _tooltip = "";
        private string _label = "";
        private bool _changed = false;

        public StrongBox<int> Value
        {
            get => _value;
            set => _value = value;
        }

        public int ActualValue
        {
            get => _value.Value;
        }

        public int Min
        {
            get => _min;
            set
            {
                _min = value;
                if (ActualValue < _min)
                {
                    _value.Value = _min;
                }
            }
        }

        public int Max
        {
            get => _max;
            set
            {
                _max = value;
                if (ActualValue > _max)
                {
                    _value.Value = _max;
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
            set
            {
                if (_label != value)
                {
                    _label = value;
                    InvalidateLayout();
                }
            }
        }

        public bool Changed
        {
            get => _changed;
            set => _changed = value;
        }

        public int Range => _max - _min;

        public bool IsEmpty => string.IsNullOrWhiteSpace(_label);

        public float Percentage
        {
            get
            {
                if (Range == 0)
                    return 0f;
                return (float)(ActualValue - _min) / Range;
            }
        }

        public SliderInt(
            StrongBox<int> value = null,
            int? min = null,
            int? max = null,
            string label = null,
            string tooltip = null,
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
            _min = min ?? 0;
            _max = max ?? 100;
            _value = value ?? new StrongBox<int>(0);
            _label = label ?? "";
            _tooltip = tooltip ?? "";
        }

        public void SetToPercentage(float percentage)
        {
            float clampedPercentage = Mathf.Clamp01(percentage);
            int newValue = Mathf.RoundToInt(_min + (Range * clampedPercentage));
            Value.Value = newValue;
        }

        protected override Size ComputeIntrinsicSize()
        {
            float intrinsicWidth, intrinsicHeight;

            if (IsEmpty)
            {
                intrinsicWidth = 120f;
                intrinsicHeight = 22f;
            }
            else
            {
                var originalWordWrap = Verse.Text.WordWrap;
                try
                {
                    Verse.Text.WordWrap = false;
                    var textSize = Verse.Text.CalcSize(_label);
                    intrinsicWidth = Math.Max(120f, textSize.x);
                    intrinsicHeight = GetLineHeight() + 22f;
                }
                finally
                {
                    Verse.Text.WordWrap = originalWordWrap;
                }
            }

            return new Size(intrinsicWidth, intrinsicHeight);
        }

        protected override Size ComputeResolvedSize(Size availableSize)
        {
            float resolvedWidth = ComputeResolvedWidth(availableSize.width);
            float resolvedHeight = ComputeResolvedHeight(availableSize.height);

            return new Size(resolvedWidth, resolvedHeight);
        }

        protected override float ComputeResolvedWidth(float availableWidth)
        {
            switch (WidthMode)
            {
                case SizeMode.Fixed:
                    return Width > 0 ? Width : IntrinsicSize.width;

                case SizeMode.Content:
                    return IntrinsicSize.width;

                case SizeMode.Fill:
                    return availableWidth;

                default:
                    return IntrinsicSize.width;
            }
        }

        protected override float ComputeResolvedHeight(float availableHeight)
        {
            switch (HeightMode)
            {
                case SizeMode.Fixed:
                    return Height > 0 ? Height : IntrinsicSize.height;

                case SizeMode.Content:
                    return IntrinsicSize.height;

                case SizeMode.Fill:
                    return availableHeight;

                default:
                    return IntrinsicSize.height;
            }
        }

        protected override void LayoutChildren()
        {
        }

        protected override void PaintElement()
        {
            var rect = ComputedRect;
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

            var originalValue = ActualValue;

            float floatValue = (float)_value.Value;
            float newFloatValue = Widgets.HorizontalSlider(sliderRect, floatValue, (float)_min, (float)_max,
                middleAlignment: true, label: null, leftAlignedLabel: null, rightAlignedLabel: null, roundTo: 1f);

            _value.Value = Mathf.RoundToInt(newFloatValue);

            if (ActualValue != originalValue)
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
            return ComputedRect;
        }

        private float GetLineHeight()
        {
            return Verse.Text.LineHeight;
        }
    }
}