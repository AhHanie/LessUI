using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A scrollable container that bridges between Unity's Rect system and LessUI's UIElement system.
    /// Acts as a scrollable parent for UI elements and handles Unity-specific scrolling setup.
    /// </summary>
    public class ScrollCanvas : UIElement
    {
        private Rect _rect;
        private Vector2 _scrollPosition;
        private bool _showScrollbars = true;
        private float _padding = 0f;
        private Func<Vector2> _getScrollPosition;
        private Action<Vector2> _setScrollPosition;

        // Typical Unity scrollbar dimensions
        private const float SCROLLBAR_WIDTH = 16f;
        private const float SCROLLBAR_HEIGHT = 16f;

        /// <summary>
        /// Gets or sets the current scroll position.
        /// Uses external getter/setter delegates if provided, otherwise uses internal field.
        /// </summary>
        public Vector2 ScrollPosition
        {
            get => _getScrollPosition?.Invoke() ?? _scrollPosition;
            set
            {
                if (_setScrollPosition != null)
                    _setScrollPosition(value);
                else
                    _scrollPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets whether scrollbars should be shown.
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
        /// Creates a new ScrollCanvas from a Unity Rect.
        /// The ScrollCanvas will have fixed dimensions matching the rect and act as a scrollable root element.
        /// </summary>
        /// <param name="rect">The Unity Rect that defines the canvas bounds</param>
        /// <param name="options">Optional UI element options</param>
        public ScrollCanvas(Rect rect, UIElementOptions options = null) : base(rect.width, rect.height, options)
        {
            _rect = rect;
            X = rect.x;
            Y = rect.y;
            // ScrollCanvas is always fixed size since it's based on a Unity Rect
            WidthMode = SizeMode.Fixed;
            HeightMode = SizeMode.Fixed;
            _scrollPosition = Vector2.zero;
            _getScrollPosition = null;
            _setScrollPosition = null;
            Padding = 0f; // Default padding is 0
        }

        /// <summary>
        /// Creates a new ScrollCanvas from a Unity Rect with external scroll position management.
        /// This allows the scroll position to persist between frame updates when the Canvas is recreated.
        /// </summary>
        /// <param name="rect">The Unity Rect that defines the canvas bounds</param>
        /// <param name="getScrollPosition">Delegate to get the current scroll position from external storage</param>
        /// <param name="setScrollPosition">Delegate to set the scroll position in external storage</param>
        /// <param name="options">Optional UI element options</param>
        public ScrollCanvas(Rect rect, Func<Vector2> getScrollPosition, Action<Vector2> setScrollPosition, UIElementOptions options = null)
            : base(rect.width, rect.height, options)
        {
            _rect = rect;
            X = rect.x;
            Y = rect.y;
            // ScrollCanvas is always fixed size since it's based on a Unity Rect
            WidthMode = SizeMode.Fixed;
            HeightMode = SizeMode.Fixed;
            _scrollPosition = Vector2.zero; // Fallback, won't be used if delegates are provided
            _getScrollPosition = getScrollPosition;
            _setScrollPosition = setScrollPosition;
            Padding = 0f; // Default padding is 0
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
        /// Calculates the scroll rect, which is the canvas rect inset by padding.
        /// This is the actual area where scrolling occurs.
        /// </summary>
        /// <returns>The calculated scroll rect</returns>
        public Rect CalculateScrollRect()
        {
            float paddedX = _rect.x + _padding;
            float paddedY = _rect.y + _padding;
            float paddedWidth = Mathf.Max(1f, _rect.width - (_padding * 2f));
            float paddedHeight = Mathf.Max(1f, _rect.height - (_padding * 2f));

            return new Rect(paddedX, paddedY, paddedWidth, paddedHeight);
        }

        /// <summary>
        /// Calculates the view rect based on all children's bounds.
        /// This determines the total scrollable area.
        /// </summary>
        /// <returns>The calculated view rect</returns>
        public Rect CalculateViewRect()
        {
            if (Children.Count == 0)
            {
                // Return canvas rect when no children - the natural content area should be the full canvas
                return _rect;
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

            // Determine the final view rect bounds
            float viewX = minX;
            float viewY = minY;
            float viewWidth = contentWidth;
            float viewHeight = contentHeight;

            // Only expand to canvas size if content is smaller AND fits entirely within canvas bounds
            bool contentFitsInCanvas = minX >= _rect.x && maxX <= (_rect.x + _rect.width) &&
                                     minY >= _rect.y && maxY <= (_rect.y + _rect.height);

            if (contentFitsInCanvas && (contentWidth < _rect.width || contentHeight < _rect.height))
            {
                // Content fits within canvas and is smaller - expand to ensure minimum canvas size
                viewX = _rect.x;
                viewY = _rect.y;
                viewWidth = Mathf.Max(contentWidth, _rect.width);
                viewHeight = Mathf.Max(contentHeight, _rect.height);
            }

            return new Rect(viewX, viewY, viewWidth, viewHeight);
        }

        /// <summary>
        /// Renders the scroll canvas and all its children.
        /// This handles Unity scrolling setup and cleanup around the standard UIElement rendering.
        /// </summary>
        public override void Render()
        {
            foreach (var child in Children)
            {
                child.CalculateDynamicSize();
            }

            var viewRect = CalculateViewRect();
            var scrollRect = CalculateScrollRect();
            var currentScrollPosition = ScrollPosition;

            // Handle Unity scrolling setup - draw border around full canvas but scroll within padded area
            Widgets.DrawMenuSection(_rect);
            Widgets.BeginScrollView(scrollRect, ref currentScrollPosition, viewRect, _showScrollbars);

            // Update scroll position through property (handles both internal and external cases)
            ScrollPosition = currentScrollPosition;

            try
            {
                // Use base rendering which calls RenderElement(), LayoutChildren(), then renders children
                base.Render();
            }
            finally
            {
                // Ensure EndScrollView is always called
                Widgets.EndScrollView();
            }
        }

        /// <summary>
        /// ScrollCanvas acts as a coordinate system provider, not a layout container.
        /// Children are positioned absolutely within the scroll area.
        /// Override LayoutChildren to do nothing since children handle their own positioning.
        /// </summary>
        protected override void LayoutChildren()
        {
            // Do nothing - ScrollCanvas doesn't impose layout on children
            // Children position themselves absolutely within the scrollable area
        }

        /// <summary>
        /// ScrollCanvas doesn't render itself, only its children and Unity scrolling setup.
        /// Override the base RenderElement to do nothing.
        /// </summary>
        protected override void RenderElement()
        {
            // ScrollCanvas doesn't render itself - it just provides scrolling
            // and handles Unity setup/cleanup
        }

        /// <summary>
        /// Calculates the width that a child with Fill width mode should have.
        /// Accounts for potential vertical scrollbar to prevent horizontal scrolling.
        /// </summary>
        /// <param name="child">The child element requesting fill width</param>
        /// <returns>The width the child should fill, accounting for scrollbars</returns>
        protected override float CalculateFillWidth(UIElement child)
        {
            // Start with the available content width (canvas width minus padding)
            float availableWidth = Width - (_padding * 2f);

            // If scrollbars are enabled, we need to be conservative and assume a vertical scrollbar might appear
            // This prevents horizontal scrollbars from appearing when vertical content overflows
            if (_showScrollbars)
            {
                availableWidth -= SCROLLBAR_WIDTH;
            }

            return Math.Max(1f, availableWidth);
        }

        /// <summary>
        /// Calculates the height that a child with Fill height mode should have.
        /// Accounts for potential horizontal scrollbar to prevent vertical scrolling issues.
        /// </summary>
        /// <param name="child">The child element requesting fill height</param>
        /// <returns>The height the child should fill, accounting for scrollbars</returns>
        protected override float CalculateFillHeight(UIElement child)
        {
            // Start with the available content height (canvas height minus padding)
            float availableHeight = Height - (_padding * 2f);

            // If scrollbars are enabled, we need to be conservative and assume a horizontal scrollbar might appear
            if (_showScrollbars)
            {
                availableHeight -= SCROLLBAR_HEIGHT;
            }

            return Math.Max(1f, availableHeight);
        }
    }
}