using System;
using System.Linq;

namespace LessUI
{
    /// <summary>
    /// A container that arranges its children in a horizontal row layout.
    /// Children are positioned side by side with configurable spacing.
    /// </summary>
    public class Row : UIElement
    {
        private float _horizontalSpacing = 2f;

        /// <summary>
        /// Gets or sets the horizontal spacing between children.
        /// </summary>
        public float HorizontalSpacing
        {
            get => _horizontalSpacing;
            set => _horizontalSpacing = value;
        }

        /// <summary>
        /// Creates a new row with default content-based sizing and default spacing.
        /// </summary>
        /// <param name="options">Optional UI element options</param>
        public Row(UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            // Use default HorizontalSpacing (2f)
        }

        /// <summary>
        /// Creates a new row with custom horizontal spacing between children.
        /// </summary>
        /// <param name="spacing">The horizontal spacing between children</param>
        /// <param name="options">Optional UI element options</param>
        public Row(float spacing, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _horizontalSpacing = spacing;
        }

        /// <summary>
        /// Calculates the dynamic size of the row based on its children.
        /// Width is the sum of all children widths plus spacing between them.
        /// Height is determined by the tallest child.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            base.CalculateDynamicSize();

            foreach (var child in Children)
            {
                child.CalculateDynamicSize();
            }

            base.CalculateDynamicSize();

            if (!Children.Any())
            {
                // Return default size when no children
                Width = 50f;
                Height = 30f;
                return;
            }

            // Width is the sum of all children widths plus spacing
            float width = Children.Sum(child => child.Width);
            if (Children.Count > 1)
            {
                width += _horizontalSpacing * (Children.Count - 1);
            }

            // Height is the maximum height of all children
            float height = Children.Max(child => child.Height);

            Width = Math.Max(1f, width);
            Height = Math.Max(1f, height);
        }

        /// <summary>
        /// Overrides the default layout to position children horizontally side by side.
        /// </summary>
        protected override void LayoutChildren()
        {
            float currentX = X;

            foreach (var child in Children)
            {
                child.X = currentX;
                child.Y = Y;

                currentX += child.Width + _horizontalSpacing;
            }
        }
    }
}