using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class Line : UIElement
    {
        private LineType _lineType = LineType.Horizontal;
        private float _thickness = 1f;
        private Color _color = Color.white;
        private string _tooltip = "";

        public LineType LineType
        {
            get => _lineType;
            set => _lineType = value;
        }

        public float Thickness
        {
            get => _thickness;
            set => _thickness = Mathf.Max(0.1f, value);
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

        public Line(
            LineType? lineType = null,
            float? thickness = null,
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
            _lineType = lineType ?? LineType.Horizontal;
            Thickness = thickness ?? 1f;
            _color = color ?? Color.white;
            _tooltip = tooltip ?? "";
        }

        protected override void CalculateElementSize()
        {
            base.CalculateElementSize();

            if (WidthMode == SizeMode.Content)
            {
                WidthCalculated = true;
                Width = (_lineType == LineType.Horizontal) ? 100f : Math.Max(0.1f, _thickness);
            }

            if (HeightMode == SizeMode.Content)
            {
                HeightCalculated = true;
                Height = (_lineType == LineType.Horizontal) ? Math.Max(0.1f, _thickness) : 100f;
            }
        }

        protected override void RenderElement()
        {
            var rect = CreateRect();
            var originalColor = GUI.color;

            try
            {
                GUI.color = _color;

                if (_lineType == LineType.Horizontal)
                {
                    var lineY = CalculateHorizontalLineY(rect);
                    Widgets.DrawLineHorizontal(rect.x, lineY, rect.width);
                }
                else
                {
                    var lineX = CalculateVerticalLineX(rect);
                    DrawVerticalLine(lineX, rect.y, rect.height);
                }
            }
            finally
            {
                GUI.color = originalColor;
            }

            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        protected float CalculateHorizontalLineY(Rect rect)
        {
            switch (Alignment)
            {
                case Align.UpperLeft:
                case Align.UpperCenter:
                case Align.UpperRight:
                    return rect.y + (_thickness / 2f);

                case Align.MiddleLeft:
                case Align.MiddleCenter:
                case Align.MiddleRight:
                    return rect.y + (rect.height - _thickness) / 2f + (_thickness / 2f);

                case Align.LowerLeft:
                case Align.LowerCenter:
                case Align.LowerRight:
                    return rect.y + rect.height - (_thickness / 2f);

                default:
                    return rect.y + (_thickness / 2f);
            }
        }

        protected float CalculateVerticalLineX(Rect rect)
        {
            switch (Alignment)
            {
                case Align.UpperLeft:
                case Align.MiddleLeft:
                case Align.LowerLeft:
                    return rect.x + (_thickness / 2f);

                case Align.UpperCenter:
                case Align.MiddleCenter:
                case Align.LowerCenter:
                    return rect.x + (rect.width - _thickness) / 2f;

                case Align.UpperRight:
                case Align.MiddleRight:
                case Align.LowerRight:
                    return rect.x + rect.width - (_thickness / 2f);

                default:
                    return rect.x + (_thickness / 2f);
            }
        }

        private void DrawVerticalLine(float x, float y, float height)
        {
            var lineRect = new Rect(x - (_thickness / 2f), y, _thickness, height);
            GUI.DrawTexture(lineRect, BaseContent.WhiteTex);
        }

        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }
    }
}