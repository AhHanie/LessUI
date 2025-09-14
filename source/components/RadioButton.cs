using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class RadioButton : UIElement
    {
        private bool _selected = false;
        private bool _disabled = false;
        private string _tooltip = "";
        private string _label = "";
        private bool _clicked = false;

        public bool Selected
        {
            get => _selected;
            set => _selected = value;
        }

        public bool Disabled
        {
            get => _disabled;
            set => _disabled = value;
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

        public bool Clicked
        {
            get => _clicked;
            set => _clicked = value;
        }

        public bool IsEmpty => string.IsNullOrWhiteSpace(_label);

        public RadioButton(
            string label = null,
            bool? selected = null,
            bool? disabled = null,
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
            _label = label ?? "";
            _selected = selected ?? false;
            _disabled = disabled ?? false;
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
                    Width = 24f;
                }
                else
                {
                    var originalWordWrap = Verse.Text.WordWrap;
                    Verse.Text.WordWrap = false;
                    var textSize = Verse.Text.CalcSize(_label);
                    Width = 24f + 6f + textSize.x;
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
                    Height = Math.Max(24f, GetLineHeight());
                }
            }
        }

        protected override void RenderElement()
        {
            var rect = CreateRect();

            bool wasClicked = Widgets.RadioButtonLabeled(rect, _label ?? "", _selected, _disabled);

            if (wasClicked && !_disabled)
            {
                _selected = true;
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

        private float GetLineHeight()
        {
            return Verse.Text.LineHeight;
        }
    }
}