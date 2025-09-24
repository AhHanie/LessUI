using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class IntRangeSlider : UIElement
    {
        private int _min = 0;
        private int _max = 100;
        private StrongBox<IntRange> _range = null;
        private string _tooltip = "";
        private int _controlId;

        public int Min
        {
            get => _min;
            set => _min = value;
        }

        public int Max
        {
            get => _max;
            set => _max = value;
        }

        public IntRange RangeValue
        {
            get => _range.Value;
        }

        public int LowerValue
        {
            get => RangeValue.min;
            set
            {
                int newValue = ClampToRange(value, _min, _max);
                if (newValue > RangeValue.max)
                {
                    newValue = RangeValue.max;
                }

                if (RangeValue.min != newValue)
                {
                    _range.Value.min = newValue;
                }
            }
        }

        public int UpperValue
        {
            get => RangeValue.max;
            set
            {
                int newValue = ClampToRange(value, _min, _max);
                if (newValue < RangeValue.min)
                {
                    newValue = RangeValue.min;
                }

                if (RangeValue.max != newValue)
                {
                    _range.Value.max = newValue;
                }
            }
        }

        public StrongBox<IntRange> Range
        {
            get => _range;
            set => _range = value;
        }

        public int RangeSpan => RangeValue.max - RangeValue.min;

        public bool IsValidRange => RangeValue.min <= RangeValue.max;

        public int TotalRange => _max - _min;

        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        public IntRangeSlider(
            int? min = null,
            int? max = null,
            StrongBox<IntRange> range = null,
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
            _range = range ?? new StrongBox<IntRange>(new IntRange(0, 100));
            _tooltip = tooltip ?? "";
            _controlId = GetHashCode();
        }

        protected override Size ComputeIntrinsicSize()
        {
            return new Size(200f, 31f);
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

            Widgets.IntRange(rect, _controlId, ref _range.Value, _min, _max);

            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        public Rect CreateRect()
        {
            return ComputedRect;
        }

        private int ClampToRange(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}