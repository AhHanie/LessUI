using UnityEngine;
using Verse;

namespace LessUI
{
    public class Canvas : UIElement
    {
        private Rect _rect;
        private bool _drawMenuSection = true;

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
        public bool DrawMenuSection
        {
            get => _drawMenuSection;
            set => _drawMenuSection = value;
        }

        public Canvas(
            Rect? rect = null,
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

        public override void Render()
        {
            if (_drawMenuSection)
            {
                Widgets.DrawMenuSection(_rect);
            }
            Widgets.BeginGroup(_rect);

            try
            {
                CalculateElementSize();
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
                Widgets.EndGroup();
            }
        }

        protected override void LayoutChildren()
        {
        }

        protected override void RenderElement()
        {
        }

        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }
    }
}