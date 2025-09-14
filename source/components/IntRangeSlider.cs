using UnityEngine;
using Verse;

namespace LessUI
{
    public class IntRangeSlider : UIElement
    {
        private int _min = 0;
        private int _max = 100;
        private IntRange _range = new IntRange(0, 100);
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

        public int LowerValue
        {
            get => _range.min;
            set
            {
                int newValue = ClampToRange(value, _min, _max);
                if (newValue > _range.max)
                {
                    newValue = _range.max;
                }

                if (_range.min != newValue)
                {
                    _range.min = newValue;
                }
            }
        }

        public int UpperValue
        {
            get => _range.max;
            set
            {
                int newValue = ClampToRange(value, _min, _max);
                if (newValue < _range.min)
                {
                    newValue = _range.min;
                }

                if (_range.max != newValue)
                {
                    _range.max = newValue;
                }
            }
        }

        public IntRange Range
        {
            get => _range;
            set => _range = value;
        }

        public int RangeSpan => _range.max - _range.min;

        public bool IsValidRange => _range.min <= _range.max;

        public int TotalRange => _max - _min;

        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        public IntRangeSlider(
            int? min = null,
            int? max = null,
            IntRange? range = null,
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
            _range = range ?? new IntRange(0, 100);
            _tooltip = tooltip ?? "";
            _controlId = GetHashCode();
        }

        protected override void CalculateElementSize()
        {
            base.CalculateElementSize();

            if (WidthMode == SizeMode.Content)
            {
                WidthCalculated = true;
                Width = 200f;
            }

            if (HeightMode == SizeMode.Content)
            {
                HeightCalculated = true;
                Height = 31f;
            }
        }

        protected override void RenderElement()
        {
            var rect = CreateRect();

            Widgets.IntRange(rect, _controlId, ref _range, _min, _max);

            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }

        public void SetValues(int lowerValue, int upperValue)
        {
            int clampedLower = ClampToRange(lowerValue, _min, _max);
            int clampedUpper = ClampToRange(upperValue, _min, _max);

            if (clampedLower > clampedUpper)
            {
                clampedLower = clampedUpper;
            }

            _range = new IntRange(clampedLower, clampedUpper);
        }

        public void SetToFullRange()
        {
            SetValues(_min, _max);
        }

        public void SetToSingleValue(int value)
        {
            int clampedValue = ClampToRange(value, _min, _max);
            _range = new IntRange(clampedValue, clampedValue);
        }

        private int ClampToRange(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}