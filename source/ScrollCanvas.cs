using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class ScrollCanvas : UIElement
    {
        private Rect _rect;
        private StrongBox<Vector2> _scrollPosition;
        private bool _showScrollbars = true;
        private float _padding = 0f;
        private bool _drawMenuSection = true;

        private const float SCROLLBAR_WIDTH = 16f;
        private const float SCROLLBAR_HEIGHT = 16f;

        public Vector2 ScrollPosition
        {
            get => _scrollPosition.Value;
            set => _scrollPosition.Value = value;
        }

        public bool ShowScrollbars
        {
            get => _showScrollbars;
            set => _showScrollbars = value;
        }

        public float Padding
        {
            get => _padding;
            set => _padding = value;
        }

        public bool DrawMenuSection
        {
            get => _drawMenuSection;
            set => _drawMenuSection = value;
        }

        public Rect Rect
        {
            get => _rect;
            set
            {
                _rect = value;
                X = value.x;
                Y = value.y;
                Width = value.width;
                Height = value.height;
            }
        }

        public ScrollCanvas(
            Rect? rect = null,
            bool? showScrollbars = null,
            float? padding = null,
            StrongBox<Vector2> scrollPosition = null,
            float? x = null,
            float? y = null,
            float? width = null,
            float? height = null,
            SizeMode? widthMode = null,
            SizeMode? heightMode = null,
            Align? alignment = null,
            bool? showBorders = null,
            Color? borderColor = null,
            int? borderThickness = null,
            bool? drawMenuSection = true)
            : base(x, y, width, height, widthMode, heightMode, alignment, showBorders, borderColor, borderThickness)
        {
            _showScrollbars = showScrollbars ?? true;
            _padding = padding ?? 0f;
            _scrollPosition = scrollPosition ?? new StrongBox<Vector2>(Vector2.zero);

            WidthMode = SizeMode.Fixed;
            HeightMode = SizeMode.Fixed;

            if (rect.HasValue)
            {
                Rect = rect.Value;
            }

            if (drawMenuSection.HasValue)
            {
                _drawMenuSection = drawMenuSection.Value;
            }
        }

        protected override void CalculateElementSize()
        {
            base.CalculateElementSize();

            if (WidthMode == SizeMode.Content)
            {
                WidthCalculated = true;
                Width = _rect.width;
            }

            if (HeightMode == SizeMode.Content)
            {
                HeightCalculated = true;
                Height = _rect.height;
            }
        }

        public Rect CalculateScrollRect()
        {
            float paddedX = _rect.x + _padding;
            float paddedY = _rect.y + _padding;
            float paddedWidth = Mathf.Max(1f, _rect.width - (_padding * 2f));
            float paddedHeight = Mathf.Max(1f, _rect.height - (_padding * 2f));

            return new Rect(paddedX, paddedY, paddedWidth, paddedHeight);
        }

        public Rect CalculateViewRect()
        {
            if (Children.Count == 0)
            {
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

            float viewX = minX;
            float viewY = minY;
            float viewWidth = contentWidth;
            float viewHeight = contentHeight;

            bool contentFitsInCanvas = minX >= _rect.x && maxX <= (_rect.x + _rect.width) &&
                                     minY >= _rect.y && maxY <= (_rect.y + _rect.height);

            if (contentFitsInCanvas && (contentWidth < _rect.width || contentHeight < _rect.height))
            {
                viewX = _rect.x;
                viewY = _rect.y;
                viewWidth = Mathf.Max(contentWidth, _rect.width);
                viewHeight = Mathf.Max(contentHeight, _rect.height);
            }

            return new Rect(viewX, viewY, viewWidth, viewHeight);
        }

        public override void Render()
        {
            CalculateElementSize();

            var viewRect = CalculateViewRect();
            var scrollRect = CalculateScrollRect();
            var currentScrollPosition = ScrollPosition;

            if (_drawMenuSection)
            {
                Widgets.DrawMenuSection(_rect);
            }
            Widgets.BeginScrollView(scrollRect, ref currentScrollPosition, viewRect, _showScrollbars);

            ScrollPosition = currentScrollPosition;

            try
            {
                RenderElement();
                LayoutChildren();

                foreach (var child in Children)
                {
                    child.Render();
                }

                if (ShowBorders)
                {
                    DrawBorders(BorderColor, BorderThickness);
                }
            }
            finally
            {
                Widgets.EndScrollView();
            }
        }

        protected override void LayoutChildren()
        {
        }

        protected override void RenderElement()
        {
        }

        public override float CalculateFillWidth(UIElement child)
        {
            float availableWidth = Width - (_padding * 2f);

            if (_showScrollbars)
            {
                availableWidth -= SCROLLBAR_WIDTH;
            }

            return Math.Max(1f, availableWidth);
        }

        public override float CalculateFillHeight(UIElement child)
        {
            float availableHeight = Height - (_padding * 2f);

            if (_showScrollbars)
            {
                availableHeight -= SCROLLBAR_HEIGHT;
            }

            return Math.Max(1f, availableHeight);
        }

        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }
    }
}