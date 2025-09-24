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

            float currentY = ComputedY;
            var availableWidth = ComputedWidth;

            foreach (var child in Children)
            {
                child.X = ComputedX;
                child.Y = currentY;

                var childContainingBlock = new Size(availableWidth, child.IntrinsicSize.height);
                child.ResolveLayout(childContainingBlock);

                currentY += child.ComputedHeight + _verticalSpacing;
            }
        }
    }
}