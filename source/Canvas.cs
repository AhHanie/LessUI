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

        public override void Render()
        {
            if (NeedsLayout || HasChildrenNeedingLayout())
            {
                RunIntrinsicSizePhase();
                RunLayoutResolutionPhase();
            }

            RunPaintPhase();
        }

        private void RunIntrinsicSizePhase()
        {
            CalculateIntrinsicSize();
        }

        private void RunLayoutResolutionPhase()
        {
            var canvasSize = new Size(_rect.width, _rect.height);
            ResolveLayout(canvasSize);
        }

        private void RunPaintPhase()
        {
            if (_drawMenuSection)
            {
                Widgets.DrawMenuSection(_rect);
            }

            Widgets.BeginGroup(_rect);

            try
            {
                Paint();
            }
            finally
            {
                Widgets.EndGroup();
            }
        }

        protected override Size ComputeIntrinsicSize()
        {
            return new Size(_rect.width, _rect.height);
        }

        protected override Size ComputeResolvedSize(Size availableSize)
        {
            return new Size(_rect.width, _rect.height);
        }

        protected override void LayoutChildren()
        {
            var containingBlockSize = new Size(ComputedWidth, ComputedHeight);

            foreach (var child in Children)
            {
                child.X = 0;
                child.Y = 0;

                child.ResolveLayout(containingBlockSize);
            }
        }

        private bool HasChildrenNeedingLayout()
        {
            return HasChildrenNeedingLayoutRecursive(this);
        }

        private bool HasChildrenNeedingLayoutRecursive(UIElement element)
        {
            if (element.NeedsLayout) return true;

            foreach (var child in element.Children)
            {
                if (HasChildrenNeedingLayoutRecursive(child))
                    return true;
            }

            return false;
        }

        protected override void PaintElement()
        {
        }

        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }
    }
}