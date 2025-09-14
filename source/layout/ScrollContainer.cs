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
            set => _padding = value;
        }

        public float VerticalSpacing
        {
            get => _verticalSpacing;
            set => _verticalSpacing = value;
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

        protected override float CalculateContentWidthFromChildren()
        {
            float width = Children.Max(child => child.Width);
            width += _padding * 2f;
            return Math.Max(1f, width);
        }

        protected override float CalculateContentHeightFromChildren()
        {
            float height = Children.Sum(child => child.Height);
            if (Children.Count > 1)
            {
                height += VerticalSpacing * (Children.Count - 1);
            }
            height += _padding * 2f;

            return Math.Max(1f, height);
        }

        public Rect GetRect()
        {
            return new Rect(X, Y, Width, Height);
        }

        public Rect CalculateScrollRect()
        {
            var containerRect = GetRect();
            float paddedX = containerRect.x + _padding;
            float paddedY = containerRect.y + _padding;
            float paddedWidth = Mathf.Max(1f, containerRect.width - (_padding * 2f));
            float paddedHeight = Mathf.Max(1f, containerRect.height - (_padding * 2f));

            return new Rect(paddedX, paddedY, paddedWidth, paddedHeight);
        }

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

        public override void Render()
        {
            CalculateFillSize();

            RenderElement();

            LayoutChildren();

            var viewRect = CalculateViewRect();
            var scrollRect = CalculateScrollRect();

            var containerRect = GetRect();
            Widgets.DrawMenuSection(containerRect);

            Widgets.BeginScrollView(scrollRect, ref _scrollPositionBox.Value, viewRect, _showScrollbars);

            foreach (var child in Children)
            {
                child.Render();
            }

            CalculateContentSizeFromChildren();

            if (ShowBorders)
            {
                DrawBorders(BorderColor, BorderThickness);
            }

            Widgets.EndScrollView();
        }

        protected override void RenderElement()
        {
        }
    }
}