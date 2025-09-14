using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class Button : UIElement
    {
        private string _text = "";
        private string _tooltip = "";
        private bool _disabled = false;
        private bool _clicked = false;

        public string Text
        {
            get => _text;
            set => _text = value;
        }

        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        public bool Disabled
        {
            get => _disabled;
            set => _disabled = value;
        }

        public bool IsEmpty => string.IsNullOrWhiteSpace(_text);

        public bool Clicked
        {
            get => _clicked;
            set => _clicked = value;
        }

        public Button(
            string text = null,
            string tooltip = null,
            bool? disabled = null,
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
            _tooltip = tooltip ?? "";
            _disabled = disabled ?? false;
        }

        protected override void CalculateElementSize()
        {
            base.CalculateElementSize();

            if (WidthMode == SizeMode.Content)
            {
                WidthCalculated = true;
                if (IsEmpty)
                {
                    Width = 60f;
                }
                else
                {
                    var originalWordWrap = Verse.Text.WordWrap;
                    Verse.Text.WordWrap = false;
                    var textSize = Verse.Text.CalcSize(_text);
                    float buttonWidth = textSize.x + 20f;
                    Width = Math.Max(1f, buttonWidth);
                    Verse.Text.WordWrap = originalWordWrap;
                }
            }

            if (HeightMode == SizeMode.Content)
            {
                HeightCalculated = true;
                if (IsEmpty)
                {
                    Height = 30f;
                }
                else
                {
                    var originalWordWrap = Verse.Text.WordWrap;
                    Verse.Text.WordWrap = false;
                    var textSize = Verse.Text.CalcSize(_text);
                    float buttonHeight = Math.Max(textSize.y + 10f, 30f);
                    Height = Math.Max(1f, buttonHeight);
                    Verse.Text.WordWrap = originalWordWrap;
                }
            }
        }

        protected override void RenderElement()
        {
            var rect = CreateRect();

            bool wasClicked = Widgets.ButtonText(rect, _text ?? "", drawBackground: true, doMouseoverSound: true, active: !_disabled);

            if (wasClicked && !_disabled)
            {
                _clicked = true;
            }

            if (!wasClicked && Clicked)
            {
                _clicked = false;
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
    }
}