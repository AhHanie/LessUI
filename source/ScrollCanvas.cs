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
        private bool _drawMenuSection = true;

        private Rect _contentBounds = Rect.zero;

        public Vector2 ScrollPosition
        {
            get => _scrollPosition.Value;
            set => _scrollPosition.Value = value;
        }

        public StrongBox<Vector2> ScrollPositionBox
        {
            get => _scrollPosition;
            set => _scrollPosition = value;
        }

        public bool ShowScrollbars
        {
            get => _showScrollbars;
            set => _showScrollbars = value;
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
                if (_rect != value)
                {
                    _rect = value;
                    X = value.x;
                    Y = value.y;
                    Width = value.width;
                    Height = value.height;
                    InvalidateLayout();
                }
            }
        }

        public ScrollCanvas(
            Rect? rect = null,
            bool? showScrollbars = null,
            StrongBox<Vector2> scrollPositionBox = null,
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
            _scrollPosition = scrollPositionBox ?? new StrongBox<Vector2>(Vector2.zero);

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

        protected override Size ComputeIntrinsicSize()
        {
            float intrinsicWidth = _rect.width > 0 ? _rect.width : Width;
            float intrinsicHeight = _rect.height > 0 ? _rect.height : Height;

            return new Size(intrinsicWidth, intrinsicHeight);
        }

        protected override Size ComputeResolvedSize(Size availableSize)
        {
            float resolvedWidth = ComputeResolvedWidth(availableSize.width);
            float resolvedHeight = ComputeResolvedHeight(availableSize.height);

            return new Size(resolvedWidth, resolvedHeight);
        }

        protected override void LayoutChildren()
        {
            var contentArea = CalculateScrollArea();
            var availableSize = new Size(contentArea.width, contentArea.height);

            foreach (var child in Children)
            {
                child.X = 0f;
                child.Y = 0f;
                child.ResolveLayout(availableSize);
            }

            CalculateContentBounds();
        }

        private void CalculateContentBounds()
        {
            var contentArea = CalculateScrollArea();

            if (Children.Count == 0)
            {
                _contentBounds = new Rect(0, 0, contentArea.width, contentArea.height);
                return;
            }

            float maxX = 0f;
            float maxY = 0f;

            foreach (var child in Children)
            {
                maxX = Mathf.Max(maxX, child.ComputedX + child.ComputedWidth);
                maxY = Mathf.Max(maxY, child.ComputedY + child.ComputedHeight);
            }

            float contentWidth = maxX;
            float contentHeight = maxY;

            bool needsVerticalScrollbar = maxY > contentArea.height;
            bool needsHorizontalScrollbar = maxX > contentArea.width;
            bool contentFitsHorizontally = maxX <= contentArea.width;
            bool contentFitsVertically = maxY <= contentArea.height;

            if (needsVerticalScrollbar && contentFitsHorizontally)
            {
                contentWidth = Mathf.Max(1f, contentArea.width - 35f);
            }
            else if (needsHorizontalScrollbar && contentFitsVertically)
            {
                contentHeight = Mathf.Max(1f, contentArea.height - 35f);
            }
            else if (!needsVerticalScrollbar && !needsHorizontalScrollbar)
            {
                contentWidth = contentArea.width;
                contentHeight = contentArea.height;
            }

            _contentBounds = new Rect(0, 0, contentWidth, contentHeight);
        }

        private Rect CalculateScrollArea()
        {
            float inset = 2f;
            return new Rect(
                ComputedX + inset,
                ComputedY + inset,
                ComputedWidth - (inset * 2f),
                ComputedHeight - (inset * 2f)
            );
        }

        public override void Paint()
        {
            var scrollArea = CalculateScrollArea();
            var currentScrollPosition = ScrollPosition;

            if (_drawMenuSection)
            {
                Widgets.DrawMenuSection(ComputedRect);
            }

            Widgets.BeginScrollView(scrollArea, ref currentScrollPosition, _contentBounds, _showScrollbars);

            try
            {
                foreach (var child in Children)
                {
                    child.Paint();
                }
            }
            finally
            {
                ScrollPosition = currentScrollPosition;
                Widgets.EndScrollView();
            }

            if (ShowBorders)
            {
                DrawBorders(BorderColor, BorderThickness);
            }
        }

        protected override void PaintElement()
        {
        }

        public Rect GetContentBounds()
        {
            return _contentBounds;
        }

        public Rect GetContentArea()
        {
            return CalculateScrollArea();
        }

        public bool ContentOverflows
        {
            get
            {
                var scrollArea = CalculateScrollArea();
                return _contentBounds.width > scrollArea.width || _contentBounds.height > scrollArea.height;
            }
        }

        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }
    }
}