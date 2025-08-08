using System.Collections.Generic;
using UnityEngine;
using Verse;
using System.Linq;
using System;

namespace LessUI
{
    /// <summary>
    /// Base class for all UI elements in the LessUI framework.
    /// Provides basic functionality for positioning, parent-child relationships, and rendering.
    /// </summary>
    public class UIElement
    {
        private float _width;
        private float _height;
        private UIElement _parent;

        // Cycle detection fields
        private bool _isCalculatingSize = false;

        // Border properties
        private bool _showBorders = false;
        private Color _borderColor = Color.white;
        private int _borderThickness = 1;

        public float X { get; set; }
        public float Y { get; set; }
        public Align Alignment { get; set; } = Align.UpperLeft;

        /// <summary>
        /// Gets or sets the vertical spacing between children when using default line layout.
        /// </summary>
        public float VerticalSpacing { get; set; } = 2f;

        public float Width
        {
            get => _width;
            set => _width = value;
        }

        public float Height
        {
            get => _height;
            set => _height = value;
        }

        public SizeMode WidthMode { get; set; }
        public SizeMode HeightMode { get; set; }
        public List<UIElement> Children { get; }

        /// <summary>
        /// Gets or sets whether borders should be automatically drawn for this element.
        /// When enabled, borders will be drawn after the element content.
        /// </summary>
        public bool ShowBorders
        {
            get => _showBorders;
            set => _showBorders = value;
        }

        /// <summary>
        /// Gets or sets the color of the automatic borders.
        /// </summary>
        public Color BorderColor
        {
            get => _borderColor;
            set => _borderColor = value;
        }

        /// <summary>
        /// Gets or sets the thickness of the automatic borders.
        /// </summary>
        public int BorderThickness
        {
            get => _borderThickness;
            set => _borderThickness = value;
        }

        public UIElement Parent
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    OnParentSet();
                }
            }
        }

        public UIElement(float width, float height, UIElementOptions options = null)
        {
            X = 0;
            Y = 0;
            _width = width;
            _height = height;
            WidthMode = SizeMode.Fixed;
            HeightMode = SizeMode.Fixed;
            Children = new List<UIElement>();
            _parent = null;

            ApplyOptions(options);
        }

        public UIElement(SizeMode widthMode, SizeMode heightMode, UIElementOptions options = null)
        {
            X = 0;
            Y = 0;
            _width = 0;
            _height = 0;
            WidthMode = widthMode;
            HeightMode = heightMode;
            Children = new List<UIElement>();
            _parent = null;

            ApplyOptions(options);
        }

        /// <summary>
        /// Called when the parent of this element is set. Override in derived classes to react to parent changes.
        /// </summary>
        public virtual void OnParentSet()
        {
            // Base implementation does nothing - sizes will be recalculated on next render
        }

        /// <summary>
        /// Applies the options to this UI element.
        /// </summary>
        /// <param name="options">The options to apply, or null to use defaults.</param>
        private void ApplyOptions(UIElementOptions options)
        {
            if (options != null)
            {
                Alignment = options.Alignment;
            }
        }

        /// <summary>
        /// Calculates the width that a child with Fill width mode should have.
        /// Override this method in derived classes to provide custom fill behavior.
        /// </summary>
        /// <param name="child">The child element requesting fill width</param>
        /// <returns>The width the child should fill</returns>
        protected virtual float CalculateFillWidth(UIElement child)
        {
            // Cycle detection: if we're calculating our own size, return fallback
            if (_isCalculatingSize)
            {
                return 50f; // Fallback width to break the cycle
            }
            return Width;
        }

        /// <summary>
        /// Calculates the height that a child with Fill height mode should have.
        /// Override this method in derived classes to provide custom fill behavior.
        /// </summary>
        /// <param name="child">The child element requesting fill height</param>
        /// <returns>The height the child should fill</returns>
        protected virtual float CalculateFillHeight(UIElement child)
        {
            // Cycle detection: if we're calculating our own size, return fallback
            if (_isCalculatingSize)
            {
                return 30f; // Fallback height to break the cycle
            }
            return Height;
        }

        /// <summary>
        /// Calculates and updates the dynamic size of the element based on its content and size modes.
        /// This method handles all sizing logic including Fixed, Content, and Fill modes.
        /// It directly updates the _width and _height fields.
        /// </summary>
        public virtual void CalculateDynamicSize()
        {
            // Prevent infinite recursion during size calculation
            if (_isCalculatingSize)
            {
                return;
            }

            _isCalculatingSize = true;
            try
            {
                // Calculate width based on WidthMode
                switch (WidthMode)
                {
                    case SizeMode.Fixed:
                        // Width is already set, nothing to do
                        break;

                    case SizeMode.Content:
                        _width = CalculateContentWidth();
                        break;

                    case SizeMode.Fill:
                        if (Parent != null)
                        {
                            _width = Parent.CalculateFillWidth(this);
                        }
                        break;
                }

                // Calculate height based on HeightMode
                switch (HeightMode)
                {
                    case SizeMode.Fixed:
                        // Height is already set, nothing to do
                        break;

                    case SizeMode.Content:
                        _height = CalculateContentHeight();
                        break;

                    case SizeMode.Fill:
                        if (Parent != null)
                        {
                            _height = Parent.CalculateFillHeight(this);
                        }
                        break;
                }
            }
            finally
            {
                _isCalculatingSize = false;
            }
        }

        /// <summary>
        /// Calculates the width needed to contain all children.
        /// Override this method in derived classes to provide custom content width calculation.
        /// </summary>
        /// <returns>The calculated content width</returns>
        protected virtual float CalculateContentWidth()
        {
            if (!Children.Any())
            {
                return 50f; // Default width when no children
            }

            // Calculate width - max of all children
            float width = Children.Max(child => child.Width);
            return Math.Max(1f, width);
        }

        /// <summary>
        /// Calculates the height needed to contain all children.
        /// Override this method in derived classes to provide custom content height calculation.
        /// </summary>
        /// <returns>The calculated content height</returns>
        protected virtual float CalculateContentHeight()
        {
            if (!Children.Any())
            {
                return 30f; // Default height when no children
            }

            // Calculate height - sum of all children plus spacing
            float height = Children.Sum(child => child.Height);
            if (Children.Count > 1)
            {
                height += VerticalSpacing * (Children.Count - 1);
            }

            return Math.Max(1f, height);
        }

        /// <summary>
        /// Renders this UI element and all of its children.
        /// First calculates dynamic sizing, then calls RenderElement() for this element, 
        /// then layouts and renders all children. Finally draws borders if ShowBorders is enabled.
        /// </summary>
        public virtual void Render()
        {
            // Always calculate dynamic size before rendering
            CalculateDynamicSize();

            RenderElement();

            // Layout children before rendering them
            LayoutChildren();

            foreach (var child in Children)
            {
                child.Render();
            }

            // Draw automatic borders AFTER content is rendered to ensure they appear on top
            if (_showBorders)
            {
                DrawBorders(_borderColor, _borderThickness);
            }
        }

        /// <summary>
        /// Layouts the children of this element. Override in derived classes to provide custom layout behavior.
        /// The default implementation arranges children in a vertical line with spacing.
        /// </summary>
        protected virtual void LayoutChildren()
        {
            float currentY = Y;

            foreach (var child in Children)
            {
                child.X = X;
                child.Y = currentY;

                currentY += child.Height + VerticalSpacing;
            }
        }

        /// <summary>
        /// Renders this specific UI element. Override this method in derived classes
        /// to implement custom rendering logic.
        /// </summary>
        protected virtual void RenderElement()
        {
            // Base implementation does nothing - override in derived classes
        }

        /// <summary>
        /// Draws borders around this UI element with default white color and thickness of 1.
        /// IMPORTANT: Call this at the END of your RenderElement() method to ensure borders appear on top of content.
        /// Alternatively, use the automatic border system by setting ShowBorders = true.
        /// </summary>
        public void DrawBorders()
        {
            DrawBorders(Color.white, 1);
        }

        /// <summary>
        /// Draws borders around this UI element with the specified color and default thickness of 1.
        /// IMPORTANT: Call this at the END of your RenderElement() method to ensure borders appear on top of content.
        /// Alternatively, use the automatic border system by setting ShowBorders = true and BorderColor.
        /// </summary>
        /// <param name="color">The color of the border</param>
        public void DrawBorders(Color color)
        {
            DrawBorders(color, 1);
        }

        /// <summary>
        /// Draws borders around this UI element with the specified color and thickness.
        /// IMPORTANT: Call this at the END of your RenderElement() method to ensure borders appear on top of content.
        /// Alternatively, use the automatic border system by setting ShowBorders = true, BorderColor, and BorderThickness.
        /// </summary>
        /// <param name="color">The color of the border</param>
        /// <param name="thickness">The thickness of the border in pixels</param>
        public void DrawBorders(Color color, int thickness)
        {
            // Don't draw border if thickness is invalid or element has no area
            if (thickness <= 0 || Width <= 0 || Height <= 0)
            {
                return;
            }

            var borderRect = new Rect(X, Y, Width, Height);
            DrawBorderInternal(borderRect, color, thickness);
        }

        /// <summary>
        /// Internal method for drawing borders. Can be overridden in derived classes for custom border rendering
        /// or in test classes to capture border drawing calls.
        /// </summary>
        /// <param name="rect">The rectangle to draw the border around</param>
        /// <param name="color">The color of the border</param>
        /// <param name="thickness">The thickness of the border</param>
        protected virtual void DrawBorderInternal(Rect rect, Color color, int thickness)
        {
            // Store the original GUI color to restore it immediately after each draw call
            var originalColor = GUI.color;

            // Draw border as four separate rectangles to avoid color bleeding issues
            // This approach ensures GUI.color changes don't affect other elements

            // Top border
            GUI.color = color;
            var topRect = new Rect(rect.x, rect.y, rect.width, thickness);
            GUI.DrawTexture(topRect, Texture2D.whiteTexture);
            GUI.color = originalColor;

            // Bottom border  
            GUI.color = color;
            var bottomRect = new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness);
            GUI.DrawTexture(bottomRect, Texture2D.whiteTexture);
            GUI.color = originalColor;

            // Left border
            GUI.color = color;
            var leftRect = new Rect(rect.x, rect.y, thickness, rect.height);
            GUI.DrawTexture(leftRect, Texture2D.whiteTexture);
            GUI.color = originalColor;

            // Right border
            GUI.color = color;
            var rightRect = new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height);
            GUI.DrawTexture(rightRect, Texture2D.whiteTexture);
            GUI.color = originalColor;
        }

        /// <summary>
        /// Adds a child element to this UI element.
        /// If the child already has a parent, it will be removed from the old parent first.
        /// </summary>
        /// <param name="child">The child element to add.</param>
        public virtual void AddChild(UIElement child)
        {
            if (child == null) return;

            // Remove from old parent if it exists
            if (child.Parent != null)
            {
                child.Parent.RemoveChild(child);
            }

            // Add to this element's children
            Children.Add(child);
            child.Parent = this;
        }

        /// <summary>
        /// Removes a child element from this UI element.
        /// The child's parent property will be set to null.
        /// </summary>
        /// <param name="child">The child element to remove.</param>
        public virtual void RemoveChild(UIElement child)
        {
            if (child == null) return;

            if (Children.Remove(child))
            {
                child.Parent = null;
            }
        }
    }
}