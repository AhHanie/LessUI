using System.Collections.Generic;
using UnityEngine;
using Verse;
using System.Linq;
using System;

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
        private bool isCalculatingSize = false; 

        private bool widthCalculated = false;
        private bool heightCalculated = false;

        public float X { get; set; }
        public float Y { get; set; }
        public Align Alignment { get; set; } = Align.UpperLeft;

        public float Width
        {
            get => _width;
            set => _width = value;
        }

        public float Height
        {
            get => _height;
            set => _height = value;
        }

        public bool WidthCalculated
        {
            get => widthCalculated;
            set => widthCalculated = value;
        }
        public bool HeightCalculated
        {
            get => heightCalculated;
            set => heightCalculated = value;
        }

        public bool IsCalculatingSize
        {
            get => isCalculatingSize;
            set => isCalculatingSize = value;
        }

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
                    OnParentSet();
                }
            }
        }

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

            if (width.HasValue)
            {
                widthCalculated = true;
                WidthMode = SizeMode.Fixed;
            }
            if (height.HasValue)
            {
                heightCalculated = true;
                HeightMode = SizeMode.Fixed;
            }
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

        public virtual void OnParentSet()
        {
        }

        protected virtual void ApplyDefaultSettings()
        {

        }

        public virtual float CalculateFillWidth(UIElement child)
        {
            if (isCalculatingSize)
            {
                throw new Exception("Recursive cycle detected with size calculations");
            }
            return Width;
        }

        public virtual float CalculateFillHeight(UIElement child)
        {
            if (isCalculatingSize)
            {
                throw new Exception("Recursive cycle detected with size calculations");
            }
            return Height;
        }

        public virtual void CalculateFillSize()
        {
            if (WidthMode == SizeMode.Fill && Parent != null)
            {
                _width = Parent.CalculateFillWidth(this);
                widthCalculated = true;
            }

            if (HeightMode == SizeMode.Fill && Parent != null)
            {
                _height = Parent.CalculateFillHeight(this);
                heightCalculated = true;
            }
        }

        public virtual void CalculateContentSizeFromChildren()
        {
            if (WidthMode == SizeMode.Content)
            {
                _width = CalculateContentWidthFromChildren();
                widthCalculated = true;
            }

            if (HeightMode == SizeMode.Content)
            {
                _height = CalculateContentHeightFromChildren();
                heightCalculated = true;
            }
        }

        protected virtual void CalculateElementSize()
        {
            CalculateFillSize();

            if (Children.Any())
            {
                CalculateContentSizeFromChildren();
            }
        }

        protected virtual float CalculateContentWidthFromChildren()
        {
            float width = 0f;
            foreach (UIElement child in Children)
            {
                if (!child.WidthCalculated)
                {
                    child.CalculateElementSize();
                }
                width += child.Width;
            }
            return width;
        }

        protected virtual float CalculateContentHeightFromChildren()
        {
            float height = 0f;
            foreach (UIElement child in Children)
            {
                if (!child.HeightCalculated)
                {
                    child.CalculateElementSize();
                }
                height += child.Height;
            }
            return height;
        }

        public virtual void Render()
        {
            if (isCalculatingSize)
            {
                throw new Exception("Recursive cycle detected with size calculations");
            }
            isCalculatingSize = true;
            CalculateElementSize();
            isCalculatingSize = false;

            RenderElement();

            LayoutChildren();

            foreach (var child in Children)
            {
                child.Render();
            }

            if (_showBorders)
            {
                DrawBorders(_borderColor, _borderThickness);
            }
        }

        protected virtual void LayoutChildren()
        {
        }

        protected virtual void RenderElement()
        {
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
            if (thickness <= 0 || Width <= 0 || Height <= 0)
            {
                return;
            }

            var borderRect = new Rect(X, Y, Width, Height);
            DrawBorderInternal(borderRect, color, thickness);
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
            }

            return this;
        }
    }
}