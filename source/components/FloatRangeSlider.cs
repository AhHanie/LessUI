using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class FloatRangeSlider : UIElement
    {
        private float _min = 0f;
        private float _max = 100f;
        private StrongBox<FloatRange> _range = null;
        private string _tooltip = "";
        private string _labelKey = "";
        private ToStringStyle _valueStyle = ToStringStyle.FloatTwo;
        private float _gap = 0f;
        private GameFont _sliderLabelFont = GameFont.Small;
        private Color? _sliderLabelColor = null;
        private float _roundTo = 0f;
        private int _controlId;

        public float Min
        {
            get => _min;
            set => _min = value;
        }

        public float Max
        {
            get => _max;
            set => _max = value;
        }

        public FloatRange RangeValue
        {
            get => _range.Value;
        }

        public float LowerValue
        {
            get => RangeValue.min;
            set
            {
                float newValue = ClampToRange(value, _min, _max);
                if (newValue > RangeValue.max - _gap)
                {
                    newValue = RangeValue.max - _gap;
                }

                if (!Mathf.Approximately(RangeValue.min, newValue))
                {
                    _range.Value.min = newValue;
                }
            }
        }

        public float UpperValue
        {
            get => RangeValue.max;
            set
            {
                float newValue = ClampToRange(value, _min, _max);
                if (newValue < RangeValue.min + _gap)
                {
                    newValue = RangeValue.min + _gap;
                }

                if (!Mathf.Approximately(RangeValue.max, newValue))
                {
                    _range.Value.max = newValue;
                }
            }
        }

        public StrongBox<FloatRange> Range
        {
            get => _range;
            set => _range = value;
        }

        public float RangeSpan => RangeValue.max - RangeValue.min;

        public bool IsValidRange => RangeValue.min <= RangeValue.max;

        public float TotalRange => _max - _min;

        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        public string LabelKey
        {
            get => _labelKey;
            set => _labelKey = value;
        }

        public ToStringStyle ValueStyle
        {
            get => _valueStyle;
            set => _valueStyle = value;
        }

        public float Gap
        {
            get => _gap;
            set => _gap = value;
        }

        public GameFont SliderLabelFont
        {
            get => _sliderLabelFont;
            set => _sliderLabelFont = value;
        }

        public Color? SliderLabelColor
        {
            get => _sliderLabelColor;
            set => _sliderLabelColor = value;
        }

        public float RoundTo
        {
            get => _roundTo;
            set => _roundTo = value;
        }

        public FloatRangeSlider(
            float? min = null,
            float? max = null,
            StrongBox<FloatRange> range = null,
            string labelKey = null,
            string tooltip = null,
            ToStringStyle? valueStyle = null,
            float? gap = null,
            GameFont? sliderLabelFont = null,
            Color? sliderLabelColor = null,
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
            _range = range ?? new StrongBox<FloatRange>(new FloatRange(0f, 100f));
            _labelKey = labelKey ?? "";
            _tooltip = tooltip ?? "";
            _valueStyle = valueStyle ?? ToStringStyle.FloatTwo;
            _gap = gap ?? 0f;
            _sliderLabelFont = sliderLabelFont ?? GameFont.Small;
            _sliderLabelColor = sliderLabelColor;
            _roundTo = roundTo ?? 0f;
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

            Widgets.FloatRange(rect, _controlId, ref _range.Value, _min, _max, _labelKey, _valueStyle, _gap, _sliderLabelFont, _sliderLabelColor, _roundTo);

            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        public Rect CreateRect()
        {
            return ComputedRect;
        }

        private float ClampToRange(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}