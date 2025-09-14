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
            set => _text = value;
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
            set => _wordWrap = value;
        }

        public float? MaxWidth
        {
            get => _maxWidth;
            set => _maxWidth = value;
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

        protected override void CalculateElementSize()
        {
            base.CalculateElementSize();

            if (WidthMode == SizeMode.Content)
            {
                WidthCalculated = true;

                if (IsEmpty)
                {
                    Width = 1f;
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
                            Width = _maxWidth.Value;
                        }
                        else if (_wordWrap)
                        {
                            float defaultMaxWidth = 300f;
                            var singleLineWidth = GetTextWidth(_text);

                            if (singleLineWidth <= defaultMaxWidth)
                            {
                                Width = singleLineWidth;
                            }
                            else
                            {
                                Width = defaultMaxWidth;
                            }
                        }
                        else
                        {
                            Width = GetTextWidth(_text);
                        }

                        Width = Math.Max(1f, Width);
                    }
                    finally
                    {
                        Verse.Text.WordWrap = originalWordWrap;
                    }
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
                    var originalWordWrap = Verse.Text.WordWrap;

                    try
                    {
                        Verse.Text.WordWrap = _wordWrap;

                        if (_wordWrap && (_maxWidth.HasValue || Width > 0))
                        {
                            float widthForHeight = _maxWidth ?? Width;
                            Height = Verse.Text.CalcHeight(_text, widthForHeight);
                        }
                        else
                        {
                            Height = GetLineHeight();
                        }

                        Height = Math.Max(1f, Height);
                    }
                    finally
                    {
                        Verse.Text.WordWrap = originalWordWrap;
                    }
                }
            }
        }

        protected override void RenderElement()
        {
            if (IsEmpty) return;

            var originalWordWrap = Verse.Text.WordWrap;

            try
            {
                Verse.Text.WordWrap = _wordWrap;

                var rect = CreateRect();

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
            return new Rect(X, Y, Width, Height);
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