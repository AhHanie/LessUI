using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LessUI
{
    public class Row : UIElement
    {
        private float _horizontalSpacing = 2f;

        public float HorizontalSpacing
        {
            get => _horizontalSpacing;
            set
            {
                if (_horizontalSpacing != value)
                {
                    _horizontalSpacing = value;
                    InvalidateLayout();
                }
            }
        }

        public Row(
            float? horizontalSpacing = null,
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
            _horizontalSpacing = horizontalSpacing ?? 2f;
        }

        public Row(
            List<UIElement> children,
            float? horizontalSpacing = null,
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
            _horizontalSpacing = horizontalSpacing ?? 2f;
        }

        protected override float ComputeIntrinsicWidthFromChildren()
        {
            if (!Children.Any()) return 0f;

            float totalWidth = Children.Sum(child => child.IntrinsicSize.width);
            if (Children.Count > 1)
            {
                totalWidth += _horizontalSpacing * (Children.Count - 1);
            }
            return totalWidth;
        }

        protected override float ComputeIntrinsicHeightFromChildren()
        {
            if (!Children.Any()) return 0f;
            return Children.Max(child => child.IntrinsicSize.height);
        }

        protected override void LayoutChildren()
        {
            if (!Children.Any()) return;

            float currentX = ComputedX;
            var availableHeight = ComputedHeight;

            foreach (var child in Children)
            {
                child.X = currentX;
                child.Y = ComputedY;

                var childContainingBlock = new Size(child.IntrinsicSize.width, availableHeight);
                child.ResolveLayout(childContainingBlock);

                currentX += child.ComputedWidth + _horizontalSpacing;
            }
        }
    }
}