using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class ScrollContainer : UIElement
    {
        private StrongBox<Vector2> _scrollPositionBox;
        private bool _showScrollbars = true;
        private float _padding = 0f;
        private float _verticalSpacing = 2f;
        private Rect _contentBounds = Rect.zero;

        public Vector2 ScrollPosition
        {
            get => _scrollPositionBox.Value;
            set => _scrollPositionBox.Value = value;
        }

        public StrongBox<Vector2> ScrollPositionBox => _scrollPositionBox;

        public bool ShowScrollbars
        {
            get => _showScrollbars;
            set => _showScrollbars = value;
        }

        public float Padding
        {
            get => _padding;
            set
            {
                if (_padding != value)
                {
                    _padding = value;
                    InvalidateLayout();
                }
            }
        }

        public float VerticalSpacing
        {
            get => _verticalSpacing;
            set
            {
                if (_verticalSpacing != value)
                {
                    _verticalSpacing = value;
                    InvalidateLayout();
                }
            }
        }

        public ScrollContainer(
            bool? showScrollbars = null,
            float? padding = null,
            float? verticalSpacing = null,
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
            int? borderThickness = null)
            : base(x, y, width, height, widthMode, heightMode, alignment, showBorders, borderColor, borderThickness)
        {
            _showScrollbars = showScrollbars ?? true;
            _padding = padding ?? 0f;
            _verticalSpacing = verticalSpacing ?? 2f;
            _scrollPositionBox = scrollPosition ?? new StrongBox<Vector2>(Vector2.zero);
        }

        public ScrollContainer(
            List<UIElement> children,
            bool? showScrollbars = null,
            float? padding = null,
            float? verticalSpacing = null,
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
            int? borderThickness = null)
            : base(children, x, y, width, height, widthMode, heightMode, alignment, showBorders, borderColor, borderThickness)
        {
            _showScrollbars = showScrollbars ?? true;
            _padding = padding ?? 0f;
            _verticalSpacing = verticalSpacing ?? 2f;
            _scrollPositionBox = scrollPosition ?? new StrongBox<Vector2>(Vector2.zero);
        }

        protected override Size ComputeIntrinsicSize()
        {
            foreach (var child in Children)
            {
                if (child.NeedsLayout)
                {
                    child.CalculateIntrinsicSize();
                }
            }

            var contentSize = CalculateContentSize();
            return contentSize;
        }

        private Size CalculateContentSize()
        {
            if (!Children.Any())
            {
                return new Size(_padding * 2f, _padding * 2f);
            }

            float contentWidth = Children.Max(child => child.IntrinsicSize.width) + (_padding * 2f);
            float contentHeight = Children.Sum(child => child.IntrinsicSize.height);

            if (Children.Count > 1)
            {
                contentHeight += _verticalSpacing * (Children.Count - 1);
            }
            contentHeight += _padding * 2f;

            return new Size(Math.Max(1f, contentWidth), Math.Max(1f, contentHeight));
        }

        protected override Size ComputeResolvedSize(Size availableSize)
        {
            float resolvedWidth = ComputeResolvedWidth(availableSize.width);
            float resolvedHeight = ComputeResolvedHeight(availableSize.height);

            return new Size(resolvedWidth, resolvedHeight);
        }

        protected override float ComputeResolvedWidth(float availableWidth)
        {
            switch (WidthMode)
            {
                case SizeMode.Fixed:
                    return Width > 0 ? Width : IntrinsicSize.width;

                case SizeMode.Content:
                    return IntrinsicSize.width;

                case SizeMode.Fill:
                    return availableWidth;

                default:
                    return IntrinsicSize.width;
            }
        }

        protected override float ComputeResolvedHeight(float availableHeight)
        {
            switch (HeightMode)
            {
                case SizeMode.Fixed:
                    return Height > 0 ? Height : IntrinsicSize.height;

                case SizeMode.Content:
                    return IntrinsicSize.height;

                case SizeMode.Fill:
                    return availableHeight;

                default:
                    return IntrinsicSize.height;
            }
        }

        protected override void LayoutChildren()
        {
            var contentArea = CalculateContentArea();
            var availableWidth = contentArea.width;

            float currentY = contentArea.y;

            foreach (var child in Children)
            {
                child.X = contentArea.x;
                child.Y = currentY;

                var childContainingBlock = new Size(availableWidth, child.IntrinsicSize.height);
                child.ResolveLayout(childContainingBlock);

                currentY += child.ComputedHeight + _verticalSpacing;
            }

            CalculateContentBounds();
        }

        private Rect CalculateContentArea()
        {
            float contentX = ComputedX + _padding;
            float contentY = ComputedY + _padding;
            float contentWidth = Mathf.Max(1f, ComputedWidth - (_padding * 2f));
            float contentHeight = Mathf.Max(1f, ComputedHeight - (_padding * 2f));

            return new Rect(contentX, contentY, contentWidth, contentHeight);
        }

        private void CalculateContentBounds()
        {
            var contentArea = CalculateContentArea();

            if (Children.Count == 0)
            {
                _contentBounds = contentArea;
                return;
            }

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (var child in Children)
            {
                minX = Mathf.Min(minX, child.ComputedX);
                minY = Mathf.Min(minY, child.ComputedY);
                maxX = Mathf.Max(maxX, child.ComputedX + child.ComputedWidth);
                maxY = Mathf.Max(maxY, child.ComputedY + child.ComputedHeight);
            }

            float contentWidth = maxX - minX;
            float contentHeight = maxY - minY;

            contentWidth = Mathf.Max(contentWidth, contentArea.width);
            contentHeight = Mathf.Max(contentHeight, contentArea.height);

            _contentBounds = new Rect(minX, minY, contentWidth, contentHeight);
        }

        protected override void PaintElement()
        {
            var contentArea = CalculateContentArea();
            var containerRect = ComputedRect;

            Widgets.DrawMenuSection(containerRect);

            Widgets.BeginScrollView(contentArea, ref _scrollPositionBox.Value, _contentBounds, _showScrollbars);

            try
            {
                foreach (var child in Children)
                {
                    child.Paint();
                }
            }
            finally
            {
                Widgets.EndScrollView();
            }
        }

        public Rect GetRect()
        {
            return ComputedRect;
        }

        public Rect GetContentBounds()
        {
            return _contentBounds;
        }

        public Rect GetContentArea()
        {
            return CalculateContentArea();
        }

        public bool ContentOverflows
        {
            get
            {
                var contentArea = CalculateContentArea();
                return _contentBounds.width > contentArea.width || _contentBounds.height > contentArea.height;
            }
        }
    }
}