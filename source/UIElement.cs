using System.Collections.Generic;
using UnityEngine;
using Verse;
using System.Linq;

namespace LessUI
{
    public class UIElement
    {
        private float _width;
        private float _height;
        private UIElement _parent;
        private bool _showBorders = false;
        private Color _borderColor = Color.white;
        private int _borderThickness = 1;

        private bool _needsLayout = true;
        private bool _layoutInProgress = false;
        private Rect _computedRect = Rect.zero;
        private Size _intrinsicSize = Size.zero;
        private Size _availableSize = Size.zero;

        public float X { get; set; }
        public float Y { get; set; }
        public Align Alignment { get; set; } = Align.UpperLeft;

        public float Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    InvalidateLayout();
                }
            }
        }

        public float Height
        {
            get => _height;
            set
            {
                if (_height != value)
                {
                    _height = value;
                    InvalidateLayout();
                }
            }
        }

        public float ComputedWidth => _computedRect.width;
        public float ComputedHeight => _computedRect.height;
        public float ComputedX => _computedRect.x;
        public float ComputedY => _computedRect.y;
        public Rect ComputedRect => _computedRect;

        public Size IntrinsicSize => _intrinsicSize;

        public Size AvailableSize => _availableSize;

        public SizeMode WidthMode { get; set; }
        public SizeMode HeightMode { get; set; }
        public List<UIElement> Children { get; }

        public bool ShowBorders
        {
            get => _showBorders;
            set => _showBorders = value;
        }

        public Color BorderColor
        {
            get => _borderColor;
            set => _borderColor = value;
        }

        public int BorderThickness
        {
            get => _borderThickness;
            set => _borderThickness = value;
        }

        public UIElement Parent
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    InvalidateLayout();
                    OnParentSet();
                }
            }
        }

        public bool NeedsLayout => _needsLayout;
        public bool IsLayoutInProgress => _layoutInProgress;

        public UIElement(
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
        {
            X = x ?? 0;
            Y = y ?? 0;
            _width = width ?? 0;
            _height = height ?? 0;
            WidthMode = widthMode ?? SizeMode.Fixed;
            HeightMode = heightMode ?? SizeMode.Fixed;
            Alignment = alignment ?? Align.UpperLeft;
            Children = new List<UIElement>();
            _parent = null;

            if (showBorders.HasValue) _showBorders = showBorders.Value;
            if (borderColor.HasValue) _borderColor = borderColor.Value;
            if (borderThickness.HasValue) _borderThickness = borderThickness.Value;
        }

        public UIElement(List<UIElement> children,
            float? x = null,
            float? y = null,
            float? width = null,
            float? height = null,
            SizeMode? widthMode = null,
            SizeMode? heightMode = null,
            Align? alignment = null,
            bool? showBorders = null,
            Color? borderColor = null,
            int? borderThickness = null) : this(x, y, width, height, widthMode, heightMode, alignment, showBorders, borderColor, borderThickness)
        {
            if (children != null)
            {
                foreach (var child in children)
                {
                    AddChild(child);
                }
            }
        }

        public void InvalidateLayout()
        {
            if (_needsLayout) return;

            _needsLayout = true;

            _parent?.InvalidateLayout();
        }

        public void InvalidateLayoutRecursive()
        {
            _needsLayout = true;
            foreach (var child in Children)
            {
                child.InvalidateLayoutRecursive();
            }
        }

        public virtual void CalculateIntrinsicSize()
        {
            if (_layoutInProgress) return;
            _layoutInProgress = true;

            try
            {
                foreach (var child in Children)
                {
                    child.CalculateIntrinsicSize();
                }

                _intrinsicSize = ComputeIntrinsicSize();
            }
            finally
            {
                _layoutInProgress = false;
            }
        }

        protected virtual Size ComputeIntrinsicSize()
        {
            float intrinsicWidth = 0f;
            float intrinsicHeight = 0f;

            if (Children.Any())
            {
                intrinsicWidth = ComputeIntrinsicWidthFromChildren();
                intrinsicHeight = ComputeIntrinsicHeightFromChildren();
            }
            else
            {
                intrinsicWidth = _width > 0 ? _width : 10f;
                intrinsicHeight = _height > 0 ? _height : 10f;
            }

            return new Size(intrinsicWidth, intrinsicHeight);
        }

        protected virtual float ComputeIntrinsicWidthFromChildren()
        {
            return Children.Sum(child => child.IntrinsicSize.width);
        }

        protected virtual float ComputeIntrinsicHeightFromChildren()
        {
            return Children.Any() ? Children.Max(child => child.IntrinsicSize.height) : 0f;
        }

        public virtual void ResolveLayout(Size availableSize)
        {
            _availableSize = availableSize;

            var resolvedSize = ComputeResolvedSize(availableSize);

            _computedRect = new Rect(X, Y, resolvedSize.width, resolvedSize.height);

            LayoutChildren();

            _needsLayout = false;
        }

        protected virtual Size ComputeResolvedSize(Size availableSize)
        {
            float resolvedWidth = ComputeResolvedWidth(availableSize.width);
            float resolvedHeight = ComputeResolvedHeight(availableSize.height);

            return new Size(resolvedWidth, resolvedHeight);
        }

        protected virtual float ComputeResolvedWidth(float availableWidth)
        {
            switch (WidthMode)
            {
                case SizeMode.Fixed:
                    return _width > 0 ? _width : _intrinsicSize.width;

                case SizeMode.Content:
                    return _intrinsicSize.width;

                case SizeMode.Fill:
                    return availableWidth;

                default:
                    return _intrinsicSize.width;
            }
        }

        protected virtual float ComputeResolvedHeight(float availableHeight)
        {
            switch (HeightMode)
            {
                case SizeMode.Fixed:
                    return _height > 0 ? _height : _intrinsicSize.height;

                case SizeMode.Content:
                    return _intrinsicSize.height;

                case SizeMode.Fill:
                    return availableHeight;

                default:
                    return _intrinsicSize.height;
            }
        }

        protected virtual void LayoutChildren()
        {
            var childAvailableSize = new Size(ComputedWidth, ComputedHeight);

            foreach (var child in Children)
            {
                child.ResolveLayout(childAvailableSize);
            }
        }

        public virtual void Paint()
        {
            PaintElement();

            foreach (var child in Children)
            {
                child.Paint();
            }

            if (_showBorders)
            {
                DrawBorders(_borderColor, _borderThickness);
            }
        }

        protected virtual void PaintElement()
        {
        }

        public virtual void Render()
        {

            if (NeedsLayout)
            {
                CalculateIntrinsicSize();

                var availableSize = new Size(_width > 0 ? _width : 800f, _height > 0 ? _height : 600f);
                ResolveLayout(availableSize);
            }

            Paint();
        }

        public void DrawBorders()
        {
            DrawBorders(Color.white, 1);
        }

        public void DrawBorders(Color color)
        {
            DrawBorders(color, 1);
        }

        public void DrawBorders(Color color, int thickness)
        {
            if (thickness <= 0 || ComputedWidth <= 0 || ComputedHeight <= 0)
            {
                return;
            }

            DrawBorderInternal(ComputedRect, color, thickness);
        }

        protected virtual void DrawBorderInternal(Rect rect, Color color, int thickness)
        {
            var originalColor = GUI.color;

            GUI.color = color;
            var topRect = new Rect(rect.x, rect.y, rect.width, thickness);
            GUI.DrawTexture(topRect, Texture2D.whiteTexture);
            GUI.color = originalColor;

            GUI.color = color;
            var bottomRect = new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness);
            GUI.DrawTexture(bottomRect, Texture2D.whiteTexture);
            GUI.color = originalColor;

            GUI.color = color;
            var leftRect = new Rect(rect.x, rect.y, thickness, rect.height);
            GUI.DrawTexture(leftRect, Texture2D.whiteTexture);
            GUI.color = originalColor;

            GUI.color = color;
            var rightRect = new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height);
            GUI.DrawTexture(rightRect, Texture2D.whiteTexture);
            GUI.color = originalColor;
        }

        public virtual UIElement AddChild(UIElement child)
        {
            if (child == null)
            {
                return this;
            }

            if (child.Parent != null)
            {
                child.Parent.RemoveChild(child);
            }

            Children.Add(child);
            child.Parent = this;
            InvalidateLayout();
            return this;
        }

        public virtual UIElement RemoveChild(UIElement child)
        {
            if (child == null)
            {
                return this;
            }

            if (Children.Remove(child))
            {
                child.Parent = null;
                InvalidateLayout();
            }

            return this;
        }

        public virtual void OnParentSet()
        {
        }
    }
}