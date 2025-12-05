using System;
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

        private Padding _padding = Padding.Zero;
        private Color? _backgroundColor = null;
        private Texture2D _backgroundTexture = null;
        private float _cornerRadius = 0f;

        private bool _showDropShadow = false;
        private Color _dropShadowColor = new Color(0f, 0f, 0f, 0.25f);
        private Vector2 _dropShadowOffset = new Vector2(2f, 2f);
        private float _dropShadowBlur = 4f;

        private static readonly Dictionary<int, Texture2D> _filledCornerCache = new Dictionary<int, Texture2D>();
        private static readonly Dictionary<Tuple<int, int>, Texture2D> _ringCornerCache = new Dictionary<Tuple<int, int>, Texture2D>();

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

        public Rect ContentRect
        {
            get
            {
                float contentWidth = Mathf.Max(0f, ComputedWidth - _padding.Horizontal);
                float contentHeight = Mathf.Max(0f, ComputedHeight - _padding.Vertical);

                return new Rect(
                    ComputedX + _padding.Left,
                    ComputedY + _padding.Top,
                    contentWidth,
                    contentHeight);
            }
        }

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

        public Padding Padding
        {
            get => _padding;
            set
            {
                if (!_padding.Equals(value))
                {
                    _padding = value;
                    InvalidateLayout();
                }
            }
        }

        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set => _backgroundColor = value;
        }

        public Texture2D BackgroundTexture
        {
            get => _backgroundTexture;
            set => _backgroundTexture = value;
        }

        public float CornerRadius
        {
            get => _cornerRadius;
            set => _cornerRadius = Mathf.Max(0f, value);
        }

        public bool ShowDropShadow
        {
            get => _showDropShadow;
            set => _showDropShadow = value;
        }

        public Color DropShadowColor
        {
            get => _dropShadowColor;
            set => _dropShadowColor = value;
        }

        public Vector2 DropShadowOffset
        {
            get => _dropShadowOffset;
            set => _dropShadowOffset = value;
        }

        public float DropShadowBlur
        {
            get => _dropShadowBlur;
            set => _dropShadowBlur = Mathf.Max(0f, value);
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
           int? borderThickness = null,
           Padding? padding = null,
            Color? backgroundColor = null,
            Texture2D backgroundTexture = null,
            float? cornerRadius = null,
            bool? showDropShadow = null,
            Color? dropShadowColor = null,
            Vector2? dropShadowOffset = null,
            float? dropShadowBlur = null)
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
            if (padding.HasValue) _padding = padding.Value;
            if (backgroundColor.HasValue) _backgroundColor = backgroundColor.Value;
            if (backgroundTexture != null) _backgroundTexture = backgroundTexture;
            if (cornerRadius.HasValue) _cornerRadius = Mathf.Max(0f, cornerRadius.Value);
            if (showDropShadow.HasValue) _showDropShadow = showDropShadow.Value;
            if (dropShadowColor.HasValue) _dropShadowColor = dropShadowColor.Value;
            if (dropShadowOffset.HasValue) _dropShadowOffset = dropShadowOffset.Value;
            if (dropShadowBlur.HasValue) _dropShadowBlur = Mathf.Max(0f, dropShadowBlur.Value);
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
            int? borderThickness = null,
            Padding? padding = null,
            Color? backgroundColor = null,
            Texture2D backgroundTexture = null,
            float? cornerRadius = null,
            bool? showDropShadow = null,
            Color? dropShadowColor = null,
            Vector2? dropShadowOffset = null,
            float? dropShadowBlur = null) : this(x, y, width, height, widthMode, heightMode, alignment, showBorders, borderColor, borderThickness, padding, backgroundColor, backgroundTexture, cornerRadius, showDropShadow, dropShadowColor, dropShadowOffset, dropShadowBlur)
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
            float intrinsicWidth = _padding.Horizontal;
            float intrinsicHeight = _padding.Vertical;

            if (Children.Any())
            {
                intrinsicWidth += ComputeIntrinsicWidthFromChildren();
                intrinsicHeight += ComputeIntrinsicHeightFromChildren();
            }
            else
            {
                intrinsicWidth = Mathf.Max(intrinsicWidth, _width > 0 ? _width : 10f + _padding.Horizontal);
                intrinsicHeight = Mathf.Max(intrinsicHeight, _height > 0 ? _height : 10f + _padding.Vertical);
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
            float contentWidth = Mathf.Max(0f, ComputedWidth - _padding.Horizontal);
            float contentHeight = Mathf.Max(0f, ComputedHeight - _padding.Vertical);

            var childAvailableSize = new Size(contentWidth, contentHeight);
            float offsetX = _padding.Left;
            float offsetY = _padding.Top;

            foreach (var child in Children)
            {
                var originalX = child.X;
                var originalY = child.Y;

                child.X = originalX + offsetX;
                child.Y = originalY + offsetY;

                child.ResolveLayout(childAvailableSize);

                child.X = originalX;
                child.Y = originalY;
            }
        }

        public virtual void Paint()
        {
            bool shadowDrawn = DrawBackground();

            PaintElement();

            foreach (var child in Children)
            {
                child.Paint();
            }

            if (_showBorders)
            {
                DrawBorders(_borderColor, _borderThickness, _cornerRadius, !shadowDrawn && _showDropShadow);
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
            DrawBorders(Color.white, 1, _cornerRadius, false);
        }

        public void DrawBorders(Color color)
        {
            DrawBorders(color, 1, _cornerRadius, false);
        }

        public void DrawBorders(Color color, int thickness, float cornerRadius = 0f, bool drawShadow = false)
        {
            if (thickness <= 0 || ComputedWidth <= 0 || ComputedHeight <= 0)
            {
                return;
            }

            DrawBorderInternal(ComputedRect, color, thickness, cornerRadius, drawShadow);
        }

        protected virtual void DrawBorderInternal(Rect rect, Color color, int thickness, float cornerRadius = 0f, bool drawShadow = false)
        {
            float clampedRadius = Mathf.Min(Mathf.Max(0f, cornerRadius), Mathf.Min(rect.width, rect.height) / 2f);
            int intRadius = Mathf.CeilToInt(clampedRadius);
            int clampedThickness = Mathf.Max(1, thickness);

            if (drawShadow)
            {
                DrawShadow(rect, clampedRadius);
            }

            if (intRadius <= 0)
            {
                DrawSquareBorder(rect, color, clampedThickness);
                return;
            }

            var originalColor = GUI.color;
            GUI.color = color;

            GUI.BeginGroup(rect);

            try
            {
                float width = rect.width;
                float height = rect.height;
                float innerWidth = Mathf.Max(0f, width - (2 * intRadius));
                float innerHeight = Mathf.Max(0f, height - (2 * intRadius));

                var cornerTexture = GetRingCornerTexture(intRadius, clampedThickness);

                GUI.DrawTexture(new Rect(intRadius, 0, innerWidth, clampedThickness), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(intRadius, height - clampedThickness, innerWidth, clampedThickness), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(0, intRadius, clampedThickness, innerHeight), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(width - clampedThickness, intRadius, clampedThickness, innerHeight), Texture2D.whiteTexture);

                DrawCorner(cornerTexture, new Rect(0, 0, intRadius, intRadius), new Rect(0f, 0f, 0.5f, 0.5f));
                DrawCorner(cornerTexture, new Rect(width - intRadius, 0, intRadius, intRadius), new Rect(0.5f, 0f, 0.5f, 0.5f));
                DrawCorner(cornerTexture, new Rect(0, height - intRadius, intRadius, intRadius), new Rect(0f, 0.5f, 0.5f, 0.5f));
                DrawCorner(cornerTexture, new Rect(width - intRadius, height - intRadius, intRadius, intRadius), new Rect(0.5f, 0.5f, 0.5f, 0.5f));
            }
            finally
            {
                GUI.EndGroup();
                GUI.color = originalColor;
            }
        }

        protected virtual bool DrawBackground()
        {
            bool shadowDrawn = false;

            if (_showDropShadow)
            {
                DrawShadow(ComputedRect, _cornerRadius);
                shadowDrawn = true;
            }

            if (!_backgroundColor.HasValue && _backgroundTexture == null)
            {
                return shadowDrawn;
            }

            float radius = Mathf.Min(Mathf.Max(0f, _cornerRadius), Mathf.Min(ComputedWidth, ComputedHeight) / 2f);
            var color = _backgroundColor ?? Color.white;

            DrawRoundedRect(ComputedRect, color, radius, _backgroundTexture);

            return shadowDrawn;
        }

        private void DrawRoundedRect(Rect rect, Color color, float cornerRadius, Texture2D texture = null)
        {
            float clampedRadius = Mathf.Min(Mathf.Max(0f, cornerRadius), Mathf.Min(rect.width, rect.height) / 2f);
            var originalColor = GUI.color;
            GUI.color = color;

            if (clampedRadius <= 0f)
            {
                GUI.DrawTexture(rect, texture ?? Texture2D.whiteTexture, ScaleMode.StretchToFill);
                GUI.color = originalColor;
                return;
            }

            int intRadius = Mathf.CeilToInt(clampedRadius);
            var cornerTexture = GetFilledCornerTexture(intRadius);

            GUI.BeginGroup(rect);

            try
            {
                float width = rect.width;
                float height = rect.height;
                float innerWidth = Mathf.Max(0f, width - (2 * intRadius));
                float innerHeight = Mathf.Max(0f, height - (2 * intRadius));

                DrawFill(texture, new Rect(intRadius, intRadius, innerWidth, innerHeight));
                DrawFill(texture, new Rect(intRadius, 0, innerWidth, intRadius));
                DrawFill(texture, new Rect(intRadius, height - intRadius, innerWidth, intRadius));
                DrawFill(texture, new Rect(0, intRadius, intRadius, innerHeight));
                DrawFill(texture, new Rect(width - intRadius, intRadius, intRadius, innerHeight));

                DrawCorner(cornerTexture, new Rect(0, 0, intRadius, intRadius), new Rect(0f, 0f, 0.5f, 0.5f));
                DrawCorner(cornerTexture, new Rect(width - intRadius, 0, intRadius, intRadius), new Rect(0.5f, 0f, 0.5f, 0.5f));
                DrawCorner(cornerTexture, new Rect(0, height - intRadius, intRadius, intRadius), new Rect(0f, 0.5f, 0.5f, 0.5f));
                DrawCorner(cornerTexture, new Rect(width - intRadius, height - intRadius, intRadius, intRadius), new Rect(0.5f, 0.5f, 0.5f, 0.5f));
            }
            finally
            {
                GUI.EndGroup();
                GUI.color = originalColor;
            }
        }

        private void DrawFill(Texture2D texture, Rect rect)
        {
            if (rect.width <= 0f || rect.height <= 0f)
            {
                return;
            }

            GUI.DrawTexture(rect, texture ?? Texture2D.whiteTexture, ScaleMode.StretchToFill);
        }

        private void DrawSquareBorder(Rect rect, Color color, int thickness)
        {
            var originalColor = GUI.color;

            GUI.color = color;
            var topRect = new Rect(rect.x, rect.y, rect.width, thickness);
            GUI.DrawTexture(topRect, Texture2D.whiteTexture);

            var bottomRect = new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness);
            GUI.DrawTexture(bottomRect, Texture2D.whiteTexture);

            var leftRect = new Rect(rect.x, rect.y, thickness, rect.height);
            GUI.DrawTexture(leftRect, Texture2D.whiteTexture);

            var rightRect = new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height);
            GUI.DrawTexture(rightRect, Texture2D.whiteTexture);

            GUI.color = originalColor;
        }

        private void DrawCorner(Texture2D texture, Rect drawRect, Rect texCoords)
        {
            if (drawRect.width <= 0f || drawRect.height <= 0f)
            {
                return;
            }

            GUI.DrawTextureWithTexCoords(drawRect, texture, texCoords);
        }

        private Texture2D GetFilledCornerTexture(int radius)
        {
            if (radius <= 0)
            {
                return Texture2D.whiteTexture;
            }

            if (_filledCornerCache.TryGetValue(radius, out var cachedTexture))
            {
                return cachedTexture;
            }

            var texture = CreateCornerTexture(radius, radius, true);
            _filledCornerCache[radius] = texture;
            return texture;
        }

        private Texture2D GetRingCornerTexture(int radius, int thickness)
        {
            int clampedThickness = Mathf.Clamp(thickness, 1, radius);
            var key = Tuple.Create(radius, clampedThickness);

            if (_ringCornerCache.TryGetValue(key, out var cachedTexture))
            {
                return cachedTexture;
            }

            var texture = CreateCornerTexture(radius, clampedThickness, false);
            _ringCornerCache[key] = texture;
            return texture;
        }

        private Texture2D CreateCornerTexture(int radius, int thickness, bool filled)
        {
            int size = Math.Max(1, radius * 2);
            var texture = new Texture2D(size, size, TextureFormat.ARGB32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                hideFlags = HideFlags.HideAndDontSave
            };

            float center = radius;
            float outerRadius = radius;
            float innerRadius = filled ? 0f : Mathf.Max(0f, radius - thickness);

            var pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center + 0.5f;
                    float dy = y - center + 0.5f;
                    float distance = Mathf.Sqrt((dx * dx) + (dy * dy));

                    float alpha = 0f;
                    if (filled)
                    {
                        alpha = distance <= outerRadius ? 1f : 0f;
                    }
                    else if (distance <= outerRadius && distance >= innerRadius)
                    {
                        alpha = 1f;
                    }

                    pixels[(y * size) + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        private void DrawShadow(Rect rect, float cornerRadius)
        {
            if (rect.width <= 0f || rect.height <= 0f)
            {
                return;
            }

            var shadowRect = new Rect(
                rect.x + _dropShadowOffset.x - (_dropShadowBlur * 0.5f),
                rect.y + _dropShadowOffset.y - (_dropShadowBlur * 0.5f),
                rect.width + _dropShadowBlur,
                rect.height + _dropShadowBlur);

            float shadowRadius = Mathf.Max(0f, cornerRadius + (_dropShadowBlur * 0.5f));

            DrawRoundedRect(shadowRect, _dropShadowColor, shadowRadius, null);
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