using System;
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
            set => _verticalSpacing = value; 
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

        protected override float CalculateContentWidthFromChildren()
        {
            float width = Children.Max(child => child.Width);
            return Math.Max(1f, width);
        }

        protected override float CalculateContentHeightFromChildren()
        {
            float height = Children.Sum(child => child.Height);
            if (Children.Count > 1)
            {
                height += _verticalSpacing * (Children.Count - 1);
            }
            return Math.Max(1f, height);
        }

        protected override void LayoutChildren()
        {
            float currentY = Y;

            foreach (var child in Children)
            {
                child.X = X;
                child.Y = currentY;

                currentY += child.Height + _verticalSpacing;
            }
        }
    }
}