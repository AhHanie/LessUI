using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class Label : UIElement
    {
        private string _text = "";
        private float? _maxWidth = null;
        private bool _wordWrap = true;
        private string _tooltip = "";

        public string Content
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    InvalidateLayout();
                }
            }
        }

        public string Text
        {
            get => _text;
            set => Content = value;
        }

        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        public bool WordWrap
        {
            get => _wordWrap;
            set
            {
                if (_wordWrap != value)
                {
                    _wordWrap = value;
                    InvalidateLayout();
                }
            }
        }

        public float? MaxWidth
        {
            get => _maxWidth;
            set
            {
                if (_maxWidth != value)
                {
                    _maxWidth = value;
                    InvalidateLayout();
                }
            }
        }

        public bool IsEmpty => string.IsNullOrWhiteSpace(_text);

        public Label(
            string text = null,
            float? maxWidth = null,
            bool? wordWrap = null,
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
            _text = text ?? "";
            _maxWidth = maxWidth;
            _wordWrap = wordWrap ?? true;
            _tooltip = tooltip ?? "";
        }

        protected override Size ComputeIntrinsicSize()
        {
            float intrinsicWidth, intrinsicHeight;

            if (IsEmpty)
            {
                intrinsicWidth = 1f;
                intrinsicHeight = GetLineHeight();
            }
            else
            {
                float availableWidth = _maxWidth ?? float.MaxValue;
                var originalWordWrap = Verse.Text.WordWrap;

                try
                {
                    Verse.Text.WordWrap = _wordWrap;

                    if (_wordWrap && _maxWidth.HasValue)
                    {
                        intrinsicWidth = _maxWidth.Value;
                        intrinsicHeight = Verse.Text.CalcHeight(_text, intrinsicWidth);
                    }
                    else if (_wordWrap)
                    {
                        float defaultMaxWidth = 300f;
                        var singleLineWidth = GetTextWidth(_text);

                        if (singleLineWidth <= defaultMaxWidth)
                        {
                            intrinsicWidth = singleLineWidth;
                            intrinsicHeight = GetLineHeight();
                        }
                        else
                        {
                            intrinsicWidth = defaultMaxWidth;
                            intrinsicHeight = Verse.Text.CalcHeight(_text, intrinsicWidth);
                        }
                    }
                    else
                    {
                        intrinsicWidth = GetTextWidth(_text);
                        intrinsicHeight = GetLineHeight();
                    }
                }
                finally
                {
                    Verse.Text.WordWrap = originalWordWrap;
                }
            }

            return new Size(Math.Max(1f, intrinsicWidth), Math.Max(1f, intrinsicHeight));
        }

        protected override Size ComputeResolvedSize(Size availableSize)
        {
            float resolvedWidth = ComputeResolvedWidth(availableSize.width);
            float resolvedHeight = ComputeResolvedHeight(availableSize.height);

            if (_wordWrap && WidthMode != SizeMode.Content && !IsEmpty)
            {
                var originalWordWrap = Verse.Text.WordWrap;
                try
                {
                    Verse.Text.WordWrap = true;
                    resolvedHeight = Verse.Text.CalcHeight(_text, resolvedWidth);
                }
                finally
                {
                    Verse.Text.WordWrap = originalWordWrap;
                }
            }

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
            if (IsEmpty) return;

            var originalWordWrap = Verse.Text.WordWrap;

            try
            {
                Verse.Text.WordWrap = _wordWrap;

                var rect = ComputedRect;

                Widgets.Label(rect, _text);

                if (!string.IsNullOrEmpty(_tooltip))
                {
                    TooltipHandler.TipRegion(rect, _tooltip);
                }
            }
            finally
            {
                Verse.Text.WordWrap = originalWordWrap;
            }
        }

        public Rect CreateRect()
        {
            return ComputedRect;
        }

        private float GetTextWidth(string text)
        {
            if (string.IsNullOrEmpty(text)) return 1f;

            var originalWordWrap = Verse.Text.WordWrap;
            try
            {
                Verse.Text.WordWrap = false;
                var size = Verse.Text.CalcSize(text);
                return size.x;
            }
            finally
            {
                Verse.Text.WordWrap = originalWordWrap;
            }
        }

        private float GetLineHeight()
        {
            return Verse.Text.LineHeight;
        }
    }
}