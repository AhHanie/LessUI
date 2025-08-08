using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A root container that bridges between Unity's Rect system and LessUI's UIElement system.
    /// Acts as the top-level parent for all UI elements and handles Unity-specific rendering setup.
    /// </summary>
    public class Canvas : UIElement
    {
        private Rect _rect;

        /// <summary>
        /// Creates a new Canvas from a Unity Rect.
        /// The Canvas will have fixed dimensions matching the rect and act as the root element.
        /// </summary>
        /// <param name="rect">The Unity Rect that defines the canvas bounds</param>
        /// <param name="options">Optional UI element options</param>
        public Canvas(Rect rect, UIElementOptions options = null) : base(rect.width, rect.height, options)
        {
            _rect = rect;
            X = rect.x;
            Y = rect.y;
            // Canvas is always fixed size since it's based on a Unity Rect
            WidthMode = SizeMode.Fixed;
            HeightMode = SizeMode.Fixed;
            Width = rect.width;
            Height = rect.height;
        }

        /// <summary>
        /// Updates the canvas to use a new Unity Rect.
        /// This will update the canvas dimensions and invalidate child sizes if needed.
        /// </summary>
        /// <param name="rect">The new Unity Rect</param>
        public void UpdateRect(Rect rect)
        {
            _rect = rect;
            X = rect.x;
            Y = rect.y;

            // Update dimensions
            Width = rect.width;
            Height = rect.height;
        }

        /// <summary>
        /// Gets the current Unity Rect that represents this canvas.
        /// </summary>
        /// <returns>The Unity Rect</returns>
        public Rect GetRect()
        {
            return _rect;
        }

        /// <summary>
        /// Renders the canvas and all its children.
        /// This handles Unity-specific setup and cleanup around the standard UIElement rendering.
        /// </summary>
        public override void Render()
        {
            // Handle Unity drawing setup
            Widgets.DrawMenuSection(_rect);
            Widgets.BeginGroup(_rect);

            try
            {
                // Use base rendering which calls RenderElement(), LayoutChildren(), then renders children
                base.Render();
            }
            finally
            {
                // Ensure EndGroup is always called
                Widgets.EndGroup();
            }
        }

        /// <summary>
        /// Canvas acts as a coordinate system provider, not a layout container.
        /// Children are positioned absolutely within the canvas.
        /// Override LayoutChildren to do nothing since children handle their own positioning.
        /// </summary>
        protected override void LayoutChildren()
        {
            // Do nothing - Canvas doesn't impose layout on children
            // Children position themselves absolutely within the canvas coordinate system
        }

        /// <summary>
        /// Canvas doesn't render itself, only its children and Unity setup.
        /// Override the base RenderElement to do nothing.
        /// </summary>
        protected override void RenderElement()
        {
            // Canvas doesn't render itself - it just provides the coordinate system
            // and handles Unity setup/cleanup
        }
    }
}