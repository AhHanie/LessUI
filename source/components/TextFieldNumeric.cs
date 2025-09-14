using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class TextFieldNumeric<T> : UIElement where T : struct
    {
        private T _value = default(T);
        private string _buffer = "";
        private T _min;
        private T _max;
        private string _tooltip = "";
        private string _label = "";
        private bool _changed = false;

        public T Value
        {
            get => _value;
            set => _value = value;
        }

        public string Buffer
        {
            get => _buffer;
            set => _buffer = value ?? "";
        }

        public T Min
        {
            get => _min;
            set => _min = value;
        }

        public T Max
        {
            get => _max;
            set => _max = value;
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

        public bool Changed
        {
            get => _changed;
            set => _changed = value;
        }

        public bool IsEmpty => string.IsNullOrWhiteSpace(_label);

        public TextFieldNumeric(
            T? value = null,
            T? min = null,
            T? max = null,
            string buffer = null,
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
            SetDefaultMinMax();

            _value = value ?? default(T);
            _min = min ?? _min;
            _max = max ?? _max;
            _buffer = buffer ?? "";
            _label = label ?? "";
            _tooltip = tooltip ?? "";
        }

        private void SetDefaultMinMax()
        {
            if (typeof(T) == typeof(int))
            {
                _min = (T)(object)0;
                _max = (T)(object)1000000000;
            }
            else if (typeof(T) == typeof(float))
            {
                _min = (T)(object)0f;
                _max = (T)(object)1000000000f;
            }
            else if (typeof(T) == typeof(double))
            {
                _min = (T)(object)0.0;
                _max = (T)(object)1000000000.0;
            }
            else
            {
                _min = default(T);
                _max = (T)(object)1000000000;
            }
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
                    Height = GetLineHeight();
                }
                else
                {
                    Height = GetLineHeight() + GetLineHeight();
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
                Verse.Text.Anchor = TextAnchor.UpperLeft;
                Widgets.Label(labelRect, _label);
                Verse.Text.Anchor = originalAnchor;
            }

            var textFieldRect = new Rect(rect.x, rect.y + labelHeight, rect.width, rect.height - labelHeight);

            T originalValue = _value;

            Widgets.TextFieldNumeric(textFieldRect, ref _value, ref _buffer, ConvertToFloat(_min), ConvertToFloat(_max));

            if (!_value.Equals(originalValue))
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

        private float ConvertToFloat(T value)
        {
            return Convert.ToSingle(value);
        }
    }
}