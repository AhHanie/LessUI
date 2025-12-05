using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class Icon : UIElement
    {
        private Texture2D _texture = null;
        private float _scale = 1f;
        private Color _color = Color.white;
        private string _tooltip = "";

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

        public float Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    InvalidateLayout();
                }
            }
        }

        public Color Color
        {
            get => _color;
            set => _color = value;
        }

        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        public bool IsEmpty => _texture == null;

        public Icon(
            Texture2D texture = null,
            float? scale = null,
            Color? color = null,
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
            _texture = texture;
            _scale = scale ?? 1f;
            _color = color ?? Color.white;
            _tooltip = tooltip ?? "";
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
                intrinsicWidth = _texture.width * _scale;
                intrinsicHeight = _texture.height * _scale;
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
            if (IsEmpty) return;

            var rect = ComputedRect;
            var originalColor = GUI.color;

            try
            {
                GUI.color = _color;
                Widgets.DrawTextureFitted(rect, _texture, 1f);

                if (!string.IsNullOrEmpty(_tooltip))
                {
                    TooltipHandler.TipRegion(rect, _tooltip);
                }
            }
            finally
            {
                GUI.color = originalColor;
            }
        }

        public Rect CreateRect()
        {
            return ComputedRect;
        }
    }
}
