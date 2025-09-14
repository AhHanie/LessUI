using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class TextEntry : UIElement
    {
        private string _text = "";
        private string _tooltip = "";
        private string _label = "";
        private int _lineCount = 1;
        private int? _maxLength = null;
        private bool _changed = false;

        public string Text
        {
            get => _text;
            set => _text = value ?? "";
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

        public int LineCount
        {
            get => _lineCount;
            set => _lineCount = Math.Max(1, value);
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
            string text = null,
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
            _text = text ?? "";
            _label = label ?? "";
            _tooltip = tooltip ?? "";
            _lineCount = Math.Max(1, lineCount ?? 1);
            _maxLength = maxLength;
        }

        protected override void CalculateElementSize()
        {
            base.CalculateElementSize();

            if (WidthMode == SizeMode.Content)
            {
                WidthCalculated = true;
                if (IsEmpty)
                {
                    Width = 150f;
                }
                else
                {
                    var originalWordWrap = Verse.Text.WordWrap;
                    Verse.Text.WordWrap = false;
                    var textSize = Verse.Text.CalcSize(_label);
                    Width = Math.Max(150f, textSize.x); // Use larger of default text entry width or label width
                    Verse.Text.WordWrap = originalWordWrap;
                }
            }

            if (HeightMode == SizeMode.Content)
            {
                HeightCalculated = true;
                var textEntryHeight = Math.Max(1f, GetLineHeight() * _lineCount);
                if (IsEmpty)
                {
                    Height = textEntryHeight;
                }
                else
                {
                    Height = GetLineHeight() + textEntryHeight; // Label height + text entry height
                }
            }
        }

        protected override void RenderElement()
        {
            var rect = CreateRect();
            var labelHeight = IsEmpty ? 0f : GetLineHeight();

            // Render label above the text entry if present
            if (!IsEmpty)
            {
                var labelRect = new Rect(rect.x, rect.y, rect.width, labelHeight);
                var originalAnchor = Verse.Text.Anchor;
                Verse.Text.Anchor = TextAnchor.UpperLeft;
                Widgets.Label(labelRect, _label);
                Verse.Text.Anchor = originalAnchor;
            }

            // Render the text entry below the label (or at the top if no label)
            var textEntryRect = new Rect(rect.x, rect.y + labelHeight, rect.width, rect.height - labelHeight);

            string originalText = _text;
            string newText;

            if (IsMultiLine)
            {
                newText = Widgets.TextArea(textEntryRect, _text);
            }
            else
            {
                newText = Widgets.TextField(textEntryRect, _text);
            }

            if (_maxLength.HasValue && newText.Length > _maxLength.Value)
            {
                newText = newText.Substring(0, _maxLength.Value);
            }

            if (newText != originalText)
            {
                _text = newText;
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
    }
}