using System;
using UnityEngine;
using UnityEngine.Windows;
using Verse;

namespace LessUI
{
    public class Button : UIElement
    {
        private string _text = "";
        private string _tooltip = "";
        private bool _disabled = false;
        private bool _clicked = false;
        private Color _normalBackgroundColor = Color.white;
        private Color _hoverBackgroundColor = Color.white;
        private Color _pressedBackgroundColor = Color.white;
        private Color _disabledBackgroundColor = new Color(1f, 1f, 1f, 0.6f);
        private Color _normalTextColor = Color.white;
        private Color _hoverTextColor = Color.white;
        private Color _pressedTextColor = Color.white;
        private Color _disabledTextColor = new Color(0.8f, 0.8f, 0.8f, 0.6f);
        private GameFont _fontSize = GameFont.Small;
        private FontStyle _fontStyle = FontStyle.Normal;
        private float _cornerRadius = 0f;
        private Texture2D _icon = null;
        private Vector2 _iconSize = new Vector2(24f, 24f);
        private float _iconTextSpacing = 4f;

        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    InvalidateLayout();
                }
            }
        }

        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        public bool Disabled
        {
            get => _disabled;
            set => _disabled = value;
        }

        public bool IsEmpty => string.IsNullOrWhiteSpace(_text);

        public bool Clicked
        {
            get => _clicked;
            set => _clicked = value;
        }

        public Color NormalBackgroundColor
        {
            get => _normalBackgroundColor;
            set => _normalBackgroundColor = value;
        }

        public Color HoverBackgroundColor
        {
            get => _hoverBackgroundColor;
            set => _hoverBackgroundColor = value;
        }

        public Color PressedBackgroundColor
        {
            get => _pressedBackgroundColor;
            set => _pressedBackgroundColor = value;
        }

        public Color DisabledBackgroundColor
        {
            get => _disabledBackgroundColor;
            set => _disabledBackgroundColor = value;
        }

        public Color NormalTextColor
        {
            get => _normalTextColor;
            set => _normalTextColor = value;
        }

        public Color HoverTextColor
        {
            get => _hoverTextColor;
            set => _hoverTextColor = value;
        }

        public Color PressedTextColor
        {
            get => _pressedTextColor;
            set => _pressedTextColor = value;
        }

        public Color DisabledTextColor
        {
            get => _disabledTextColor;
            set => _disabledTextColor = value;
        }

        public GameFont FontSize
        {
            get => _fontSize;
            set => _fontSize = value;
        }

        public FontStyle FontStyle
        {
            get => _fontStyle;
            set => _fontStyle = value;
        }

        public float CornerRadius
        {
            get => _cornerRadius;
            set => _cornerRadius = Mathf.Max(0f, value);
        }

        public Texture2D Icon
        {
            get => _icon;
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    InvalidateLayout();
                }
            }
        }

        public Vector2 IconSize
        {
            get => _iconSize;
            set
            {
                if (_iconSize != value)
                {
                    _iconSize = value;
                    InvalidateLayout();
                }
            }
        }

        public float IconTextSpacing
        {
            get => _iconTextSpacing;
            set => _iconTextSpacing = Mathf.Max(0f, value);
        }

        public Button(
            string text = null,
            string tooltip = null,
            bool? disabled = null,
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
            _text = text ?? "";
            _tooltip = tooltip ?? "";
            _disabled = disabled ?? false;
        }

        protected override Size ComputeIntrinsicSize()
        {
            float intrinsicWidth, intrinsicHeight;

            float iconWidth = _icon != null ? _iconSize.x : 0f;
            float iconHeight = _icon != null ? _iconSize.y : 0f;

            if (IsEmpty)
            {
                intrinsicWidth = Math.Max(60f, iconWidth + 20f);
                intrinsicHeight = Math.Max(30f, iconHeight + 10f);
            }
            else
            {
                var originalWordWrap = Verse.Text.WordWrap;
                var originalFont = Verse.Text.Font;
                try
                {
                    Verse.Text.WordWrap = false;
                    Verse.Text.Font = _fontSize;
                    var textSize = Verse.Text.CalcSize(_text);

                    float spacing = (_icon != null && textSize.x > 0f) ? _iconTextSpacing : 0f;
                    intrinsicWidth = iconWidth + spacing + textSize.x + 20f;
                    intrinsicHeight = Math.Max(Math.Max(textSize.y, iconHeight) + 10f, 30f);
                }
                finally
                {
                    Verse.Text.WordWrap = originalWordWrap;
                    Verse.Text.Font = originalFont;
                }
            }

            return new Size(Math.Max(1f, intrinsicWidth), Math.Max(1f, intrinsicHeight));
        }

        protected override Size ComputeResolvedSize(Size availableSize)
        {
            float resolvedWidth = ComputeResolvedWidth(availableSize.width);
            float resolvedHeight = ComputeResolvedHeight(availableSize.height);

            return new Size(resolvedWidth, resolvedHeight);
        }

        protected override float ComputeResolvedWidth(float availableWidth)
        {
            switch (WidthMode)
            {
                case SizeMode.Fixed:
                    return Width > 0 ? Width : IntrinsicSize.width;

                case SizeMode.Content:
                    return IntrinsicSize.width;

                case SizeMode.Fill:
                    return availableWidth;

                default:
                    return IntrinsicSize.width;
            }
        }

        protected override float ComputeResolvedHeight(float availableHeight)
        {
            switch (HeightMode)
            {
                case SizeMode.Fixed:
                    return Height > 0 ? Height : IntrinsicSize.height;

                case SizeMode.Content:
                    return IntrinsicSize.height;

                case SizeMode.Fill:
                    return availableHeight;

                default:
                    return IntrinsicSize.height;
            }
        }

        protected override void LayoutChildren()
        {
        }

        protected override void PaintElement()
        {
            var rect = ComputedRect;
            bool isActive = !_disabled;
            bool isMouseOver = isActive && Mouse.IsOver(rect);
            bool isPressed = isMouseOver && UnityEngine.Input.GetMouseButton(0);

            DrawButton(rect, isMouseOver, isPressed, isActive);

            bool wasClicked = isActive && Widgets.ButtonInvisible(rect, doMouseoverSound: true);

            if (wasClicked)
            {
                _clicked = true;
            }
            else if (Clicked)
            {
                _clicked = false;
            }

            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        public Rect CreateRect()
        {
            return ComputedRect;
        }

        private void DrawButton(Rect rect, bool isMouseOver, bool isPressed, bool isActive)
        {
            var backgroundColor = ResolveBackgroundColor(isMouseOver, isPressed, isActive);
            var textColor = ResolveTextColor(isMouseOver, isPressed, isActive);

            DrawBackground(rect, backgroundColor, isMouseOver, isPressed);
            DrawContent(rect, textColor);
        }

        private Color ResolveBackgroundColor(bool isMouseOver, bool isPressed, bool isActive)
        {
            if (!isActive)
            {
                return _disabledBackgroundColor;
            }

            if (isPressed)
            {
                return _pressedBackgroundColor;
            }

            if (isMouseOver)
            {
                return _hoverBackgroundColor;
            }

            return _normalBackgroundColor;
        }

        private Color ResolveTextColor(bool isMouseOver, bool isPressed, bool isActive)
        {
            if (!isActive)
            {
                return _disabledTextColor;
            }

            if (isPressed)
            {
                return _pressedTextColor;
            }

            if (isMouseOver)
            {
                return _hoverTextColor;
            }

            return _normalTextColor;
        }

        private void DrawBackground(Rect rect, Color backgroundColor, bool isMouseOver, bool isPressed)
        {
            var originalColor = GUI.color;
            GUI.color = backgroundColor;

            Texture2D stateTexture = Widgets.ButtonBGAtlas;

            if (isPressed)
            {
                stateTexture = Widgets.ButtonBGAtlasClick ?? stateTexture;
            }
            else if (isMouseOver)
            {
                stateTexture = Widgets.ButtonBGAtlasMouseover ?? stateTexture;
            }

            float clampedRadius = Mathf.Min(Mathf.Max(0f, _cornerRadius), Mathf.Min(rect.width, rect.height) / 2f);

            if (clampedRadius <= 0f)
            {
                GUI.DrawTexture(rect, stateTexture);
                GUI.color = originalColor;
                return;
            }

            DrawRoundedRect(rect, stateTexture, clampedRadius);
            GUI.color = originalColor;
        }

        private void DrawRoundedRect(Rect rect, Texture2D texture, float cornerRadius)
        {
            int intRadius = Mathf.CeilToInt(cornerRadius);

            GUI.BeginGroup(rect);
            try
            {
                float width = rect.width;
                float height = rect.height;
                float innerWidth = Mathf.Max(0f, width - (2 * intRadius));
                float innerHeight = Mathf.Max(0f, height - (2 * intRadius));

                GUI.DrawTexture(new Rect(intRadius, intRadius, innerWidth, innerHeight), texture);
                GUI.DrawTexture(new Rect(intRadius, 0, innerWidth, intRadius), texture);
                GUI.DrawTexture(new Rect(intRadius, height - intRadius, innerWidth, intRadius), texture);
                GUI.DrawTexture(new Rect(0, intRadius, intRadius, innerHeight), texture);
                GUI.DrawTexture(new Rect(width - intRadius, intRadius, intRadius, innerHeight), texture);

                DrawCorner(texture, new Rect(0, 0, intRadius, intRadius), new Rect(0f, 0f, 0.5f, 0.5f));
                DrawCorner(texture, new Rect(width - intRadius, 0, intRadius, intRadius), new Rect(0.5f, 0f, 0.5f, 0.5f));
                DrawCorner(texture, new Rect(0, height - intRadius, intRadius, intRadius), new Rect(0f, 0.5f, 0.5f, 0.5f));
                DrawCorner(texture, new Rect(width - intRadius, height - intRadius, intRadius, intRadius), new Rect(0.5f, 0.5f, 0.5f, 0.5f));
            }
            finally
            {
                GUI.EndGroup();
            }
        }

        private void DrawCorner(Texture2D texture, Rect rect, Rect uv)
        {
            GUI.DrawTextureWithTexCoords(rect, texture, uv);
        }

        private void DrawContent(Rect rect, Color textColor)
        {
            var textRect = rect;

            if (_icon != null)
            {
                float iconWidth = Mathf.Min(_iconSize.x, rect.width);
                float iconHeight = Mathf.Min(_iconSize.y, rect.height);
                var iconRect = new Rect(rect.x + 10f, rect.y + ((rect.height - iconHeight) / 2f), iconWidth, iconHeight);
                GUI.DrawTexture(iconRect, _icon, ScaleMode.ScaleToFit);
                textRect.xMin = iconRect.xMax + _iconTextSpacing;
            }

            if (!IsEmpty)
            {
                var originalAnchor = Verse.Text.Anchor;
                var originalWordWrap = Verse.Text.WordWrap;
                var originalFont = Verse.Text.Font;
                var originalColor = GUI.color;

                Verse.Text.Anchor = TextAnchor.MiddleCenter;
                Verse.Text.WordWrap = false;
                Verse.Text.Font = _fontSize;

                var labelStyle = new GUIStyle(Verse.Text.CurFontStyle)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = _fontStyle,
                    wordWrap = false
                };

                GUI.color = textColor;
                GUI.Label(textRect, _text ?? string.Empty, labelStyle);

                Verse.Text.Anchor = originalAnchor;
                Verse.Text.WordWrap = originalWordWrap;
                Verse.Text.Font = originalFont;
                GUI.color = originalColor;
            }
        }
    }
}