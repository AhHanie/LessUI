using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A UI component that renders a straight line, either horizontally or vertically.
    /// The line is drawn in the center of its container by default, but can be positioned
    /// using the Alignment property for precise control.
    /// </summary>
    public class Line : UIElement
    {
        private LineType _lineType;
        private float _thickness = 1f;
        private Color _color = Color.white;
        private string _tooltip;

        /// <summary>
        /// Gets or sets the type of line (Horizontal or Vertical).
        /// </summary>
        public LineType LineType
        {
            get => _lineType;
            set => _lineType = value;
        }

        /// <summary>
        /// Gets or sets the thickness of the line in pixels.
        /// Values less than or equal to zero will be clamped to 0.1f.
        /// </summary>
        public float Thickness
        {
            get => _thickness;
            set => _thickness = Mathf.Max(0.1f, value);
        }

        /// <summary>
        /// Gets or sets the color of the line.
        /// </summary>
        public Color Color
        {
            get => _color;
            set => _color = value;
        }

        /// <summary>
        /// Gets or sets the tooltip text to display when hovering over the line.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        /// <summary>
        /// Creates a new line with content-based sizing.
        /// </summary>
        /// <param name="lineType">The type of line (Horizontal or Vertical)</param>
        /// <param name="thickness">The thickness of the line in pixels (default: 1f)</param>
        /// <param name="options">Optional UI element options</param>
        public Line(LineType lineType, float thickness = 1f, UIElementOptions options = null)
            : base(SizeMode.Content, SizeMode.Content, options)
        {
            _lineType = lineType;
            Thickness = thickness;
        }

        /// <summary>
        /// Creates a new line with fixed dimensions.
        /// </summary>
        /// <param name="lineType">The type of line (Horizontal or Vertical)</param>
        /// <param name="thickness">The thickness of the line in pixels</param>
        /// <param name="width">Fixed width of the line</param>
        /// <param name="height">Fixed height of the line</param>
        /// <param name="options">Optional UI element options</param>
        public Line(LineType lineType, float thickness, float width, float height, UIElementOptions options = null)
            : base(width, height, options)
        {
            _lineType = lineType;
            Thickness = thickness;
        }

        /// <summary>
        /// Creates a new line with specified width sizing mode.
        /// </summary>
        /// <param name="lineType">The type of line (Horizontal or Vertical)</param>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="thickness">The thickness of the line in pixels (default: 1f)</param>
        /// <param name="options">Optional UI element options</param>
        public Line(LineType lineType, SizeMode widthMode, float thickness = 1f, UIElementOptions options = null)
            : base(widthMode, SizeMode.Content, options)
        {
            _lineType = lineType;
            Thickness = thickness;
        }

        /// <summary>
        /// Creates a new line with content-based sizing as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="lineType">The type of line (Horizontal or Vertical)</param>
        /// <param name="thickness">The thickness of the line in pixels (default: 1f)</param>
        /// <param name="options">Optional UI element options</param>
        public Line(UIElement parent, LineType lineType, float thickness = 1f, UIElementOptions options = null)
            : base(SizeMode.Content, SizeMode.Content, options)
        {
            _lineType = lineType;
            Thickness = thickness;
            parent?.AddChild(this);
        }

        /// <summary>
        /// Creates a new line with specified width sizing mode as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="lineType">The type of line (Horizontal or Vertical)</param>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="thickness">The thickness of the line in pixels (default: 1f)</param>
        /// <param name="options">Optional UI element options</param>
        public Line(UIElement parent, LineType lineType, SizeMode widthMode, float thickness = 1f, UIElementOptions options = null)
            : base(widthMode, SizeMode.Content, options)
        {
            _lineType = lineType;
            Thickness = thickness;
            parent?.AddChild(this);
        }

        /// <summary>
        /// Calculates the dynamic size of the line based on its type and thickness.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            // IMPORTANT: Call base first to handle Fill/Fixed modes
            base.CalculateDynamicSize();

            // Only override for Content mode (base.CalculateDynamicSize handles Fill/Fixed)
            if (WidthMode == SizeMode.Content)
            {
                Width = (_lineType == LineType.Horizontal) ? 100f : Math.Max(0.1f, _thickness);
            }

            if (HeightMode == SizeMode.Content)
            {
                Height = (_lineType == LineType.Horizontal) ? Math.Max(0.1f, _thickness) : 100f;
            }
        }

        /// <summary>
        /// Renders the line using RimWorld's drawing utilities.
        /// </summary>
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

        /// <summary>
        /// Calculates the Y coordinate for a horizontal line based on alignment.
        /// </summary>
        /// <param name="rect">The container rect</param>
        /// <returns>The Y coordinate where the line should be drawn</returns>
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

        /// <summary>
        /// Calculates the X coordinate for a vertical line based on alignment.
        /// </summary>
        /// <param name="rect">The container rect</param>
        /// <returns>The X coordinate where the line should be drawn</returns>
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

        /// <summary>
        /// Draws a vertical line since Widgets may not have a DrawLineVertical method.
        /// </summary>
        /// <param name="x">X coordinate of the line</param>
        /// <param name="y">Y coordinate where the line starts</param>
        /// <param name="height">Height of the line</param>
        private void DrawVerticalLine(float x, float y, float height)
        {
            var lineRect = new Rect(x - (_thickness / 2f), y, _thickness, height);
            GUI.DrawTexture(lineRect, BaseContent.WhiteTex);
        }

        /// <summary>
        /// Creates a Unity Rect from the line's position and size.
        /// </summary>
        /// <returns>A Rect representing the line's bounds</returns>
        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }
    }
}