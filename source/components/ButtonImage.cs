using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class ButtonImage : UIElement
    {
        private Texture2D _texture = null;
        private string _tooltip = "";
        private bool _disabled = false;
        private bool _clicked = false;

        public Texture2D Texture
        {
            get => _texture;
            set => _texture = value;
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

        public bool IsEmpty => _texture == null;

        public bool Clicked
        {
            get => _clicked;
            set => _clicked = value;
        }

        public ButtonImage(
            Texture2D texture = null,
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
            _texture = texture;
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
                    Width = 32f;
                }
                else
                {
                    Width = Math.Max(1f, _texture.width);
                }
            }

            if (HeightMode == SizeMode.Content)
            {
                HeightCalculated = true;
                if (IsEmpty)
                {
                    Height = 32f;
                }
                else
                {
                    Height = Math.Max(1f, _texture.height);
                }
            }
        }

        protected override void RenderElement()
        {
            var rect = CreateRect();

            bool wasClicked = Widgets.ButtonImage(rect, _texture);

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