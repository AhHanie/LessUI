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
            set
            {
                if (_text != value)
                {
                    _text = value;
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

        protected override Size ComputeIntrinsicSize()
        {
            float intrinsicWidth, intrinsicHeight;

            if (IsEmpty)
            {
                intrinsicWidth = 60f;
                intrinsicHeight = 30f;
            }
            else
            {
                var originalWordWrap = Verse.Text.WordWrap;
                try
                {
                    Verse.Text.WordWrap = false;
                    var textSize = Verse.Text.CalcSize(_text);

                    intrinsicWidth = textSize.x + 20f;
                    intrinsicHeight = Math.Max(textSize.y + 10f, 30f);
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
            return ComputedRect;
        }
    }
}