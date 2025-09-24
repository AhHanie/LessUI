using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class TextFieldNumeric<T> : UIElement where T : struct
    {
        private StrongBox<T> _value = null;
        private string _buffer = "";
        private T _min;
        private T _max;
        private string _tooltip = "";
        private string _label = "";
        private bool _changed = false;

        public StrongBox<T> Value
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

        public bool IsEmpty => string.IsNullOrWhiteSpace(_label);

        public TextFieldNumeric(
            StrongBox<T> value = null,
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

            _value = value ?? new StrongBox<T>(default(T));
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

        protected override Size ComputeIntrinsicSize()
        {
            float intrinsicWidth, intrinsicHeight;

            if (IsEmpty)
            {
                intrinsicWidth = 120f;
                intrinsicHeight = GetLineHeight();
            }
            else
            {
                var originalWordWrap = Verse.Text.WordWrap;
                try
                {
                    Verse.Text.WordWrap = false;
                    var textSize = Verse.Text.CalcSize(_label);
                    intrinsicWidth = Math.Max(120f, textSize.x);
                    intrinsicHeight = GetLineHeight() + GetLineHeight();
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
                Verse.Text.Anchor = TextAnchor.UpperLeft;
                Widgets.Label(labelRect, _label);
                Verse.Text.Anchor = originalAnchor;
            }

            var textFieldRect = new Rect(rect.x, rect.y + labelHeight, rect.width, rect.height - labelHeight);

            T originalValue = _value.Value;

            Widgets.TextFieldNumeric(textFieldRect, ref _value.Value, ref _buffer, ConvertToFloat(_min), ConvertToFloat(_max));

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
            return ComputedRect;
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