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
            set
            {
                if (_texture != value)
                {
                    _texture = value;
                    InvalidateLayout();
                }
            }
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

        protected override Size ComputeIntrinsicSize()
        {
            float intrinsicWidth, intrinsicHeight;

            if (IsEmpty)
            {
                intrinsicWidth = 32f;
                intrinsicHeight = 32f;
            }
            else
            {
                intrinsicWidth = _texture.width;
                intrinsicHeight = _texture.height;
            }

            return new Size(Math.Max(1f, intrinsicWidth), Math.Max(1f, intrinsicHeight));
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
            return ComputedRect;
        }
    }
}