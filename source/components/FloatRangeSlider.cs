using UnityEngine;
using Verse;

namespace LessUI
{
    public class FloatRangeSlider : UIElement
    {
        private float _min = 0f;
        private float _max = 100f;
        private FloatRange _range = new FloatRange(0f, 100f);
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

        public float LowerValue
        {
            get => _range.min;
            set
            {
                float newValue = ClampToRange(value, _min, _max);
                if (newValue > _range.max - _gap)
                {
                    newValue = _range.max - _gap;
                }

                if (!Mathf.Approximately(_range.min, newValue))
                {
                    _range.min = newValue;
                }
            }
        }

        public float UpperValue
        {
            get => _range.max;
            set
            {
                float newValue = ClampToRange(value, _min, _max);
                if (newValue < _range.min + _gap)
                {
                    newValue = _range.min + _gap;
                }

                if (!Mathf.Approximately(_range.max, newValue))
                {
                    _range.max = newValue;
                }
            }
        }

        public FloatRange Range
        {
            get => _range;
            set => _range = value;
        }

        public float RangeSpan => _range.max - _range.min;

        public bool IsValidRange => _range.min <= _range.max;

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
            FloatRange? range = null,
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
            _range = range ?? new FloatRange(0f, 100f);
            _labelKey = labelKey ?? "";
            _tooltip = tooltip ?? "";
            _valueStyle = valueStyle ?? ToStringStyle.FloatTwo;
            _gap = gap ?? 0f;
            _sliderLabelFont = sliderLabelFont ?? GameFont.Small;
            _sliderLabelColor = sliderLabelColor;
            _roundTo = roundTo ?? 0f;
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
            CalculateElementSize();

            var rect = CreateRect();

            Widgets.FloatRange(rect, _controlId, ref _range, _min, _max, _labelKey, _valueStyle, _gap, _sliderLabelFont, _sliderLabelColor, _roundTo);

            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }

        public void SetValues(float lowerValue, float upperValue)
        {
            float clampedLower = ClampToRange(lowerValue, _min, _max);
            float clampedUpper = ClampToRange(upperValue, _min, _max);

            if (clampedUpper - clampedLower < _gap)
            {
                float midpoint = (clampedLower + clampedUpper) / 2f;
                clampedLower = midpoint - _gap / 2f;
                clampedUpper = midpoint + _gap / 2f;

                clampedLower = ClampToRange(clampedLower, _min, _max - _gap);
                clampedUpper = ClampToRange(clampedUpper, _min + _gap, _max);
            }

            _range = new FloatRange(clampedLower, clampedUpper);
        }

        public void SetToFullRange()
        {
            SetValues(_min, _max);
        }

        public void SetToSingleValue(float value)
        {
            float clampedValue = ClampToRange(value, _min, _max);
            _range = new FloatRange(clampedValue, clampedValue);
        }

        private float ClampToRange(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}