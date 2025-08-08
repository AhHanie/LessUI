using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A scrollable container that supports flexible sizing modes (Fixed, Content, Fill).
    /// Uses StrongBox for ref-like scroll position semantics compatible with C# 7.3.
    /// </summary>
    public class ScrollContainer : UIElement
    {
        private StrongBox<Vector2> _scrollPositionBox;
        private bool _showScrollbars = true;
        private float _padding = 0f;

        /// <summary>
        /// Gets or sets the current scroll position.
        /// Modifies the underlying StrongBox value.
        /// </summary>
        public Vector2 ScrollPosition
        {
            get => _scrollPositionBox.Value;
            set => _scrollPositionBox.Value = value;
        }

        /// <summary>
        /// Gets the StrongBox containing the scroll position.
        /// This allows external code to share the same scroll position reference.
        /// </summary>
        public StrongBox<Vector2> ScrollPositionBox => _scrollPositionBox;

        /// <summary>
        /// Gets or sets whether scrollbars should be shown when content overflows.
        /// </summary>
        public bool ShowScrollbars
        {
            get => _showScrollbars;
            set => _showScrollbars = value;
        }

        /// <summary>
        /// Gets or sets the padding inside the scroll area.
        /// This creates space between the scroll content and the scrollbars/borders.
        /// </summary>
        public float Padding
        {
            get => _padding;
            set => _padding = value;
        }

        /// <summary>
        /// Creates a new ScrollContainer with fixed dimensions and shared scroll position.
        /// </summary>
        /// <param name="width">Fixed width of the container</param>
        /// <param name="height">Fixed height of the container</param>
        /// <param name="scrollPositionBox">Shared StrongBox containing the scroll position</param>
        /// <param name="options">Optional UI element options</param>
        public ScrollContainer(float width, float height, StrongBox<Vector2> scrollPositionBox, UIElementOptions options = null)
            : base(width, height, options)
        {
            _scrollPositionBox = scrollPositionBox ?? new StrongBox<Vector2>(Vector2.zero);
            Padding = 0f;
            VerticalSpacing = 2f;
        }

        /// <summary>
        /// Creates a new ScrollContainer with specified sizing modes and shared scroll position.
        /// </summary>
        /// <param name="widthMode">Width sizing mode (Fixed, Content, Fill)</param>
        /// <param name="heightMode">Height sizing mode (Fixed, Content, Fill)</param>
        /// <param name="scrollPositionBox">Shared StrongBox containing the scroll position</param>
        /// <param name="options">Optional UI element options</param>
        public ScrollContainer(SizeMode widthMode, SizeMode heightMode, StrongBox<Vector2> scrollPositionBox, UIElementOptions options = null)
            : base(widthMode, heightMode, options)
        {
            _scrollPositionBox = scrollPositionBox ?? new StrongBox<Vector2>(Vector2.zero);
            Padding = 0f;
            VerticalSpacing = 2f;
        }

        /// <summary>
        /// Creates a new ScrollContainer with fixed dimensions as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="width">Fixed width of the container</param>
        /// <param name="height">Fixed height of the container</param>
        /// <param name="options">Optional UI element options</param>
        public ScrollContainer(UIElement parent, float width, float height, UIElementOptions options = null)
            : base(width, height, options)
        {
            _scrollPositionBox = new StrongBox<Vector2>(Vector2.zero);
            Padding = 0f;
            VerticalSpacing = 2f;
            parent?.AddChild(this);
        }

        /// <summary>
        /// Creates a new ScrollContainer with specified sizing modes as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="widthMode">Width sizing mode (Fixed, Content, Fill)</param>
        /// <param name="heightMode">Height sizing mode (Fixed, Content, Fill)</param>
        /// <param name="options">Optional UI element options</param>
        public ScrollContainer(UIElement parent, SizeMode widthMode, SizeMode heightMode, UIElementOptions options = null)
            : base(widthMode, heightMode, options)
        {
            _scrollPositionBox = new StrongBox<Vector2>(Vector2.zero);
            Padding = 0f;
            VerticalSpacing = 2f;
            parent?.AddChild(this);
        }

        /// <summary>
        /// Creates a new ScrollContainer with fixed dimensions and shared scroll position as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="width">Fixed width of the container</param>
        /// <param name="height">Fixed height of the container</param>
        /// <param name="scrollPositionBox">Shared StrongBox containing the scroll position</param>
        /// <param name="options">Optional UI element options</param>
        public ScrollContainer(UIElement parent, float width, float height, StrongBox<Vector2> scrollPositionBox, UIElementOptions options = null)
            : base(width, height, options)
        {
            _scrollPositionBox = scrollPositionBox ?? new StrongBox<Vector2>(Vector2.zero);
            Padding = 0f;
            VerticalSpacing = 2f;
            parent?.AddChild(this);
        }

        /// <summary>
        /// Calculates the dynamic size of the scroll container based on its content.
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
                Width = 50f;
                Height = 30f;
                return;
            }

            float width = Children.Max(child => child.Width);
            float height = Children.Sum(child => child.Height);
            if (Children.Count > 1)
            {
                height += VerticalSpacing * (Children.Count - 1);
            }

            Width = Math.Max(1f, width);
            Height = Math.Max(1f, height);
        }

        /// <summary>
        /// Gets the current Unity Rect that represents this container.
        /// </summary>
        /// <returns>The Unity Rect</returns>
        public Rect GetRect()
        {
            return new Rect(X, Y, Width, Height);
        }

        /// <summary>
        /// Calculates the scroll rect, which is the container rect inset by padding.
        /// This is the actual area where scrolling occurs.
        /// </summary>
        /// <returns>The calculated scroll rect</returns>
        public Rect CalculateScrollRect()
        {
            var containerRect = GetRect();
            float paddedX = containerRect.x + _padding;
            float paddedY = containerRect.y + _padding;
            float paddedWidth = Mathf.Max(1f, containerRect.width - (_padding * 2f));
            float paddedHeight = Mathf.Max(1f, containerRect.height - (_padding * 2f));

            return new Rect(paddedX, paddedY, paddedWidth, paddedHeight);
        }

        /// <summary>
        /// Calculates the view rect based on all children's bounds.
        /// This determines the total scrollable area.
        /// </summary>
        /// <returns>The calculated view rect</returns>
        public Rect CalculateViewRect()
        {
            Rect containerRect;
            if (Children.Count == 0)
            {
                containerRect = GetRect();
                return containerRect;
            }

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (var child in Children)
            {
                minX = Mathf.Min(minX, child.X);
                minY = Mathf.Min(minY, child.Y);
                maxX = Mathf.Max(maxX, child.X + child.Width);
                maxY = Mathf.Max(maxY, child.Y + child.Height);
            }

            float contentWidth = maxX - minX;
            float contentHeight = maxY - minY;

            float viewX = minX;
            float viewY = minY;
            float viewWidth = contentWidth;
            float viewHeight = contentHeight;

            containerRect = GetRect();
            bool contentFitsInContainer = minX >= containerRect.x && maxX <= (containerRect.x + containerRect.width) &&
                                        minY >= containerRect.y && maxY <= (containerRect.y + containerRect.height);

            if (contentFitsInContainer && (contentWidth < containerRect.width || contentHeight < containerRect.height))
            {
                viewX = containerRect.x;
                viewY = containerRect.y;
                viewWidth = Mathf.Max(contentWidth, containerRect.width);
                viewHeight = Mathf.Max(contentHeight, containerRect.height);
            }

            return new Rect(viewX, viewY, viewWidth, viewHeight);
        }

        /// <summary>
        /// Renders the scroll container and all its children.
        /// This handles Unity scrolling setup and cleanup around the standard UIElement rendering.
        /// </summary>
        public override void Render()
        {
            // First render this element (empty for ScrollContainer)
            RenderElement();

            // Layout children BEFORE calculating view rect so they are in correct positions
            LayoutChildren();

            // Now calculate view rect with properly positioned children
            var viewRect = CalculateViewRect();
            var scrollRect = CalculateScrollRect();
            var currentScrollPosition = _scrollPositionBox.Value;

            // Handle Unity scrolling setup - draw border around full container but scroll within padded area
            var containerRect = GetRect();
            Widgets.DrawMenuSection(containerRect);

            // Use the single scrollbar setting - pass by reference to StrongBox
            Widgets.BeginScrollView(scrollRect, ref _scrollPositionBox.Value, viewRect, _showScrollbars);

            try
            {
                // Render all children
                foreach (var child in Children)
                {
                    child.Render();
                }

                // Draw automatic borders if enabled
                if (ShowBorders)
                {
                    DrawBorders(BorderColor, BorderThickness);
                }
            }
            finally
            {
                // Ensure EndScrollView is always called
                Widgets.EndScrollView();
            }
        }

        /// <summary>
        /// ScrollContainer uses the default layout behavior for positioning children.
        /// This allows content-based sizing to work correctly while still supporting
        /// scrolling when content overflows.
        /// </summary>
        protected override void LayoutChildren()
        {
            base.LayoutChildren();
        }

        /// <summary>
        /// ScrollContainer doesn't render itself, only its children and Unity scrolling setup.
        /// Override the base RenderElement to do nothing.
        /// </summary>
        protected override void RenderElement()
        {
            // ScrollContainer doesn't render itself - it just provides scrolling
            // and handles Unity setup/cleanup
        }
    }
}