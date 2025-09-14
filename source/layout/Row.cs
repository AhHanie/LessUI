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
            set => _horizontalSpacing = value;
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

        protected override float CalculateContentWidthFromChildren()
        {
            float width = Children.Sum(child => child.Width);
            if (Children.Count > 1)
            {
                width += _horizontalSpacing * (Children.Count - 1);
            }

            return Math.Max(1f, width);
        }

        protected override float CalculateContentHeightFromChildren()
        {
            float height = Children.Max(child => child.Height);
            return Math.Max(1f, height);
        }

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