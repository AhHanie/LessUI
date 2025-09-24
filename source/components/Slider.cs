using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class Slider : UIElement
    {
        private StrongBox<float> _value = null;
        private float _min = 0f;
        private float _max = 100f;
        private string _tooltip = "";
        private string _label = "";
        private float _roundTo = -1f;
        private bool _changed = false;

        public StrongBox<float> Value
        {
            get => _value;
            set => _value = value;
        }

        public float ActualValue
        {
            get => _value.Value;
        }

        public float Min
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

        public float Max
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

        public bool IsEmpty => string.IsNullOrWhiteSpace(_label);

        public float Percentage
        {
            get
            {
                if (Mathf.Approximately(Range, 0f))
                    return 0f;
                return (ActualValue - _min) / Range;
            }
        }

        public Slider(
            StrongBox<float> value = null,
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
            _value = value ?? new StrongBox<float>(0f);
            _label = label ?? "";
            _tooltip = tooltip ?? "";
            _roundTo = roundTo ?? -1f;
        }

        public void SetToPercentage(float percentage)
        {
            float clampedPercentage = Mathf.Clamp01(percentage);
            Value.Value = ActualValue + (Range * clampedPercentage);
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

            _value.Value = Widgets.HorizontalSlider(sliderRect, _value.Value, _min, _max, middleAlignment: true,
                label: null, leftAlignedLabel: null, rightAlignedLabel: null, roundTo: _roundTo);

            if (!Mathf.Approximately(ActualValue, originalValue))
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