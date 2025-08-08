using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// An empty UI element that takes up space without rendering any content.
    /// Useful as a spacer, placeholder, or for creating layout gaps.
    /// </summary>
    public class Empty : UIElement
    {
        /// <summary>
        /// Creates a new empty element with content-based sizing.
        /// Will use default dimensions when no constraints are provided.
        /// </summary>
        /// <param name="options">Optional UI element options</param>
        public Empty(UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
        }

        /// <summary>
        /// Creates a new empty element with fixed dimensions.
        /// </summary>
        /// <param name="width">Fixed width of the empty element</param>
        /// <param name="height">Fixed height of the empty element</param>
        /// <param name="options">Optional UI element options</param>
        public Empty(float width, float height, UIElementOptions options = null) : base(width, height, options)
        {
        }

        /// <summary>
        /// Creates a new empty element with specified sizing modes.
        /// </summary>
        /// <param name="widthMode">Width sizing mode (Fixed, Content, or Fill)</param>
        /// <param name="heightMode">Height sizing mode (Fixed, Content, or Fill)</param>
        /// <param name="options">Optional UI element options</param>
        public Empty(SizeMode widthMode, SizeMode heightMode, UIElementOptions options = null) : base(widthMode, heightMode, options)
        {
        }

        /// <summary>
        /// Creates a new empty element with specified width sizing mode and content-based height.
        /// </summary>
        /// <param name="widthMode">Width sizing mode (Content or Fill)</param>
        /// <param name="options">Optional UI element options</param>
        public Empty(SizeMode widthMode, UIElementOptions options = null) : base(widthMode, SizeMode.Content, options)
        {
        }

        /// <summary>
        /// Creates a new empty element as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="options">Optional UI element options</param>
        public Empty(UIElement parent, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            parent?.AddChild(this);
        }

        /// <summary>
        /// Creates a new empty element with fixed dimensions as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="width">Fixed width of the empty element</param>
        /// <param name="height">Fixed height of the empty element</param>
        /// <param name="options">Optional UI element options</param>
        public Empty(UIElement parent, float width, float height, UIElementOptions options = null) : base(width, height, options)
        {
            parent?.AddChild(this);
        }

        /// <summary>
        /// Creates a new empty element with specified sizing modes as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="widthMode">Width sizing mode (Fixed, Content, or Fill)</param>
        /// <param name="heightMode">Height sizing mode (Fixed, Content, or Fill)</param>
        /// <param name="options">Optional UI element options</param>
        public Empty(UIElement parent, SizeMode widthMode, SizeMode heightMode, UIElementOptions options = null) : base(widthMode, heightMode, options)
        {
            parent?.AddChild(this);
        }

        /// <summary>
        /// Calculates the dynamic size of the empty element.
        /// Provides reasonable default dimensions for content-based sizing.
        /// </summary>
        public override void CalculateDynamicSize()
        {
            base.CalculateDynamicSize();

            // For content mode, provide default spacer dimensions
            if (WidthMode == SizeMode.Content && Width == 0)
            {
                Width = 10f; // Default spacer width
            }

            if (HeightMode == SizeMode.Content && Height == 0)
            {
                Height = 10f; // Default spacer height
            }
        }

        /// <summary>
        /// Empty elements don't render anything - this method intentionally does nothing.
        /// The element will still take up space in layouts but won't draw any content.
        /// </summary>
        protected override void RenderElement()
        {
            // Intentionally empty - this element takes space but renders nothing
        }

        /// <summary>
        /// Creates a Unity Rect from the empty element's position and size.
        /// Useful for debugging or if you need to know the bounds of the empty space.
        /// </summary>
        /// <returns>A Rect representing the empty element's bounds</returns>
        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }
    }
}