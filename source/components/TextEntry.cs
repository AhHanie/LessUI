using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class TextEntry : UIElement
    {
        private StrongBox<string> _text = null;
        private string _tooltip = "";
        private string _label = "";
        private int _lineCount = 1;
        private int? _maxLength = null;
        private bool _changed = false;

        public StrongBox<string> Text
        {
            get => _text;
            set => _text = value;
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

        public int LineCount
        {
            get => _lineCount;
            set
            {
                int newLineCount = Math.Max(1, value);
                if (_lineCount != newLineCount)
                {
                    _lineCount = newLineCount;
                    InvalidateLayout();
                }
            }
        }

        public int? MaxLength
        {
            get => _maxLength;
            set => _maxLength = value;
        }

        public bool Changed
        {
            get => _changed;
            set => _changed = value;
        }

        public bool IsMultiLine => _lineCount > 1;

        public bool IsEmpty => string.IsNullOrWhiteSpace(_label);

        public TextEntry(
            StrongBox<string> text = null,
            string label = null,
            string tooltip = null,
            int? lineCount = null,
            int? maxLength = null,
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
            _text = text ?? new StrongBox<string>("");
            _label = label ?? "";
            _tooltip = tooltip ?? "";
            _lineCount = Math.Max(1, lineCount ?? 1);
            _maxLength = maxLength;
        }

        protected override Size ComputeIntrinsicSize()
        {
            float intrinsicWidth, intrinsicHeight;

            if (IsEmpty)
            {
                intrinsicWidth = 150f;
                intrinsicHeight = Math.Max(1f, GetLineHeight() * _lineCount);
            }
            else
            {
                var originalWordWrap = Verse.Text.WordWrap;
                try
                {
                    Verse.Text.WordWrap = false;
                    var textSize = Verse.Text.CalcSize(_label);
                    intrinsicWidth = Math.Max(150f, textSize.x);
                    var textEntryHeight = Math.Max(1f, GetLineHeight() * _lineCount);
                    intrinsicHeight = GetLineHeight() + textEntryHeight;
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

            var textEntryRect = new Rect(rect.x, rect.y + labelHeight, rect.width, rect.height - labelHeight);

            string originalText = _text.Value;
            string newText;

            if (IsMultiLine)
            {
                newText = Widgets.TextArea(textEntryRect, _text.Value);
            }
            else
            {
                newText = Widgets.TextField(textEntryRect, _text.Value);
            }

            if (_maxLength.HasValue && newText.Length > _maxLength.Value)
            {
                newText = newText.Substring(0, _maxLength.Value);
            }

            if (newText != originalText)
            {
                _text.Value = newText;
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