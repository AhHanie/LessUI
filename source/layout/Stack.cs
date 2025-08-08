using System;
using System.Linq;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A container that arranges its children in a vertical stack layout.
    /// Children are positioned one below the other with configurable spacing.
    /// This is similar to the default UIElement behavior but made explicit as a layout container.
    /// </summary>
    public class Stack : UIElement
    {
        /// <summary>
        /// Creates a new stack with default content-based sizing and default spacing.
        /// </summary>
        /// <param name="options">Optional UI element options</param>
        public Stack(UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            // Use default VerticalSpacing (2f) from UIElement
        }

        /// <summary>
        /// Creates a new stack with custom vertical spacing between children.
        /// </summary>
        /// <param name="spacing">The vertical spacing between children</param>
        /// <param name="options">Optional UI element options</param>
        public Stack(float spacing, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            VerticalSpacing = spacing;
        }

        /// <summary>
        /// Creates a new stack with specified width sizing mode and content-based height.
        /// </summary>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="options">Optional UI element options</param>
        public Stack(SizeMode widthMode, UIElementOptions options = null) : base(widthMode, SizeMode.Content, options)
        {
            // Use default VerticalSpacing (2f) from UIElement
        }

        /// <summary>
        /// Creates a new stack with specified width sizing mode, content-based height, and custom spacing.
        /// </summary>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="spacing">The vertical spacing between children</param>
        /// <param name="options">Optional UI element options</param>
        public Stack(SizeMode widthMode, float spacing, UIElementOptions options = null) : base(widthMode, SizeMode.Content, options)
        {
            VerticalSpacing = spacing;
        }

        /// <summary>
        /// Creates a new stack with specified sizing modes for both width and height.
        /// </summary>
        /// <param name="widthMode">The width sizing mode (Fixed, Content, or Fill)</param>
        /// <param name="heightMode">The height sizing mode (Fixed, Content, or Fill)</param>
        /// <param name="options">Optional UI element options</param>
        public Stack(SizeMode widthMode, SizeMode heightMode, UIElementOptions options = null) : base(widthMode, heightMode, options)
        {
            // Use default VerticalSpacing (2f) from UIElement
        }

        /// <summary>
        /// Creates a new stack with specified sizing modes and custom spacing.
        /// </summary>
        /// <param name="widthMode">The width sizing mode (Fixed, Content, or Fill)</param>
        /// <param name="heightMode">The height sizing mode (Fixed, Content, or Fill)</param>
        /// <param name="spacing">The vertical spacing between children</param>
        /// <param name="options">Optional UI element options</param>
        public Stack(SizeMode widthMode, SizeMode heightMode, float spacing, UIElementOptions options = null) : base(widthMode, heightMode, options)
        {
            VerticalSpacing = spacing;
        }

        /// <summary>
        /// Calculates the dynamic size of the stack based on its children.
        /// This ensures children are sized before the stack calculates its own content size.
        /// </summary>
        public override void CalculateDynamicSize()
        {
            base.CalculateDynamicSize();
            // Ensure all children are sized before we calculate our content dimensions
            foreach (var child in Children)
            {
                child.CalculateDynamicSize();
            }

            // Let the base class handle Fill/Content/Fixed logic properly
            base.CalculateDynamicSize();
        }

        /// <summary>
        /// Calculates the width needed to contain all children.
        /// Width is determined by the widest child.
        /// </summary>
        /// <returns>The calculated content width</returns>
        protected override float CalculateContentWidth()
        {
            if (!Children.Any())
            {
                return 50f; // Default width when no children
            }

            // Width is the maximum width of all children
            float width = Children.Max(child => child.Width);
            return Math.Max(1f, width);
        }

        /// <summary>
        /// Calculates the height needed to contain all children.
        /// Height is the sum of all children heights plus spacing between them.
        /// </summary>
        /// <returns>The calculated content height</returns>
        protected override float CalculateContentHeight()
        {
            if (!Children.Any())
            {
                return 30f; // Default height when no children
            }

            // Height is the sum of all children heights plus spacing
            float height = Children.Sum(child => child.Height);
            if (Children.Count > 1)
            {
                height += VerticalSpacing * (Children.Count - 1);
            }

            return Math.Max(1f, height);
        }

        /// <summary>
        /// Stack uses the default UIElement layout behavior which positions children
        /// vertically one below the other with VerticalSpacing between them.
        /// We don't need to override LayoutChildren since UIElement already does what we want.
        /// </summary>
    }
}