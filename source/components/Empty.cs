using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class Empty : UIElement
    {
        public Empty(
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
        }

        protected override void CalculateElementSize()
        {
            base.CalculateElementSize();

            if (WidthMode == SizeMode.Content)
            {
                WidthCalculated = true;
                Width = 10f;
            }

            if (HeightMode == SizeMode.Content)
            {
                HeightCalculated = true;
                Height = 10f;
            }
        }

        protected override void RenderElement()
        {
            CreateRect();
        }

        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }
    }
}