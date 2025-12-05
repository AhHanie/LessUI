using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LessUI
{
    public class Stack : UIElement
    {
        private float _verticalSpacing = 2f;

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

        public Stack(
            float? verticalSpacing = null,
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
            _verticalSpacing = verticalSpacing ?? 2f;
        }

        public Stack(
            List<UIElement> children,
            float? verticalSpacing = null,
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
            _verticalSpacing = verticalSpacing ?? 2f;
        }

        protected override float ComputeIntrinsicWidthFromChildren()
        {
            if (!Children.Any()) return 0f;
            return Children.Max(child => child.IntrinsicSize.width);
        }

        protected override float ComputeIntrinsicHeightFromChildren()
        {
            if (!Children.Any()) return 0f;

            float totalHeight = Children.Sum(child => child.IntrinsicSize.height);
            if (Children.Count > 1)
            {
                totalHeight += _verticalSpacing * (Children.Count - 1);
            }
            return totalHeight;
        }

        protected override void LayoutChildren()
        {
            if (!Children.Any()) return;

            // First pass: calculate space needed for non-Fill children and count Fill children
            float totalSpacingHeight = (Children.Count - 1) * _verticalSpacing;
            float fixedHeight = 0f;
            int fillChildrenCount = 0;

            foreach (var child in Children)
            {
                if (child.HeightMode == SizeMode.Fill)
                {
                    fillChildrenCount++;
                }
                else
                {
                    fixedHeight += child.IntrinsicSize.height;
                }
            }

            // Calculate remaining height for Fill children
            float remainingHeight = ComputedHeight - fixedHeight - totalSpacingHeight;
            float fillChildHeight = fillChildrenCount > 0
                ? Mathf.Max(0f, remainingHeight / fillChildrenCount)
                : 0f;

            // Second pass: layout children
            float currentY = ComputedY;
            var availableWidth = ComputedWidth;

            foreach (var child in Children)
            {
                child.X = ComputedX;
                child.Y = currentY;

                float childAvailableHeight = child.HeightMode == SizeMode.Fill
                    ? fillChildHeight
                    : child.IntrinsicSize.height;

                var childContainingBlock = new Size(availableWidth, childAvailableHeight);
                child.ResolveLayout(childContainingBlock);

                currentY += child.ComputedHeight + _verticalSpacing;
            }
        }
    }
}