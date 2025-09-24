using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    public enum ColorPickerMode
    {
        HSVWheel,
        Palette,
        Temperature,
        Combined
    }

    public class Window_ColorPicker : Window
    {
        private StrongBox<Color> colorBox;
        private ColorPickerMode mode;
        private List<Color> paletteColors;
        private float colorValue = 1f;

        private bool wheelDragging = false;
        private bool temperatureDragging = false;

        private string[] colorTextBuffers = new string[6];
        private Color colorTextBuffer = Color.white;
        private string previousFocusedControl = "";

        public override Vector2 InitialSize => new Vector2(400f, 450f);

        public Window_ColorPicker(StrongBox<Color> colorBox, ColorPickerMode mode = ColorPickerMode.Combined, List<Color> paletteColors = null)
        {
            this.colorBox = colorBox;
            this.mode = mode;
            this.paletteColors = paletteColors ?? GetDefaultPalette();
            this.colorTextBuffer = colorBox.Value;
            this.colorValue = Mathf.Max(colorBox.Value.r, colorBox.Value.g, colorBox.Value.b);

            doCloseButton = false;
            doCloseX = true;
            closeOnCancel = true;
            closeOnAccept = false;

            for (int i = 0; i < colorTextBuffers.Length; i++)
            {
                colorTextBuffers[i] = "";
            }
        }

        private List<Color> GetDefaultPalette()
        {
            return new List<Color>
            {
                Color.white, Color.black, Color.gray, new Color(0.5f, 0.5f, 0.5f),
                Color.red, new Color(1f, 0.5f, 0.5f), new Color(0.5f, 0f, 0f), new Color(1f, 0.8f, 0.8f),
                Color.green, new Color(0.5f, 1f, 0.5f), new Color(0f, 0.5f, 0f), new Color(0.8f, 1f, 0.8f),
                Color.blue, new Color(0.5f, 0.5f, 1f), new Color(0f, 0f, 0.5f), new Color(0.8f, 0.8f, 1f),
                Color.yellow, new Color(1f, 1f, 0.5f), new Color(0.5f, 0.5f, 0f), new Color(1f, 1f, 0.8f),
                Color.cyan, new Color(0.5f, 1f, 1f), new Color(0f, 0.5f, 0.5f), new Color(0.8f, 1f, 1f),
                Color.magenta, new Color(1f, 0.5f, 1f), new Color(0.5f, 0f, 0.5f), new Color(1f, 0.8f, 1f)
            };
        }

        public override void DoWindowContents(Rect inRect)
        {
            var originalColor = colorBox.Value;
            float currentY = inRect.y;
            float sectionSpacing = 15f;

            Text.Font = GameFont.Medium;
            var titleHeight = Text.CalcHeight("Select Color", inRect.width);
            Widgets.Label(new Rect(inRect.x, currentY, inRect.width, titleHeight), "Select Color");
            Text.Font = GameFont.Small;
            currentY += titleHeight + sectionSpacing;

            switch (mode)
            {
                case ColorPickerMode.HSVWheel:
                    PaintHSVWheel(new Rect(inRect.x, currentY, inRect.width, inRect.height - currentY - 40f));
                    break;

                case ColorPickerMode.Palette:
                    PaintPalette(new Rect(inRect.x, currentY, inRect.width, inRect.height - currentY - 40f));
                    break;

                case ColorPickerMode.Temperature:
                    PaintTemperature(new Rect(inRect.x, currentY, inRect.width, 40f));
                    break;

                case ColorPickerMode.Combined:
                    PaintCombined(new Rect(inRect.x, currentY, inRect.width, inRect.height - currentY - 40f));
                    break;
            }

            var buttonY = inRect.yMax - 35f;
            var buttonWidth = 80f;
            var buttonSpacing = 10f;

            if (Widgets.ButtonText(new Rect(inRect.xMax - buttonWidth, buttonY, buttonWidth, 30f), "OK"))
            {
                Close();
            }

            if (Widgets.ButtonText(new Rect(inRect.xMax - buttonWidth * 2 - buttonSpacing, buttonY, buttonWidth, 30f), "Cancel"))
            {
                colorBox.Value = originalColor;
                Close();
            }
        }

        private void PaintHSVWheel(Rect rect)
        {
            float size = Math.Min(rect.width, rect.height - 50f);
            var wheelRect = new Rect(
                rect.x + (rect.width - size) / 2f,
                rect.y,
                size,
                size
            );

            string controlName = $"ColorWheel_{GetHashCode()}";
            Color tempColor = colorBox.Value;
            Widgets.HSVColorWheel(wheelRect, ref tempColor, ref wheelDragging, colorValue, controlName);
            colorBox.Value = tempColor;

            var sliderRect = new Rect(rect.x, wheelRect.yMax + 10f, rect.width, 30f);
            colorValue = Widgets.HorizontalSlider(sliderRect, colorValue, 0f, 1f, middleAlignment: true, "Brightness");
        }

        private void PaintPalette(Rect rect)
        {
            if (paletteColors == null || !paletteColors.Any())
                return;

            float height;
            Color tempColor = colorBox.Value;
            bool changed = Widgets.ColorSelector(rect, ref tempColor, paletteColors, out height);
            colorBox.Value = tempColor;
        }

        private void PaintTemperature(Rect rect)
        {
            Color tempColor = colorBox.Value;
            Widgets.ColorTemperatureBar(rect, ref tempColor, ref temperatureDragging, colorValue);
            colorBox.Value = tempColor;
        }

        private void PaintCombined(Rect rect)
        {
            float currentY = rect.y;
            float sectionSpacing = 10f;

            float wheelSize = Math.Min(200f, rect.width);
            var wheelRect = new Rect(
                rect.x + (rect.width - wheelSize) / 2f,
                currentY,
                wheelSize,
                wheelSize
            );

            string controlName = $"ColorWheel_{GetHashCode()}";
            Color tempColor = colorBox.Value;
            Widgets.HSVColorWheel(wheelRect, ref tempColor, ref wheelDragging, colorValue, controlName);
            colorBox.Value = tempColor;
            currentY += wheelSize + sectionSpacing;

            // Value slider
            if (currentY + 30f <= rect.yMax)
            {
                var sliderRect = new Rect(rect.x, currentY, rect.width, 30f);
                colorValue = Widgets.HorizontalSlider(sliderRect, colorValue, 0f, 1f, middleAlignment: true, "Brightness");
                currentY += 30f + sectionSpacing;
            }

            float tempHeight = 40f;
            if (currentY + tempHeight <= rect.yMax)
            {
                var tempRect = new Rect(rect.x, currentY, rect.width, tempHeight);
                Color tempColor2 = colorBox.Value;
                Widgets.ColorTemperatureBar(tempRect, ref tempColor2, ref temperatureDragging, colorValue);
                colorBox.Value = tempColor2;
                currentY += tempHeight + sectionSpacing;
            }

            if (currentY < rect.yMax - 50f)
            {
                var textRect = new Rect(rect.x, currentY, rect.width, rect.yMax - currentY);
                var aggregator = new RectAggregator(textRect, GetHashCode(), new Vector2(2f, 2f));

                Color tempColor3 = colorBox.Value;
                Widgets.ColorTextfields(
                    ref aggregator,
                    ref tempColor3,
                    ref colorTextBuffers,
                    ref colorTextBuffer,
                    previousFocusedControl,
                    $"ColorText_{GetHashCode()}"
                );
                colorBox.Value = tempColor3;
            }
        }
    }

    public class ColorPicker : UIElement
    {
        private StrongBox<Color> _colorBox = new StrongBox<Color>(Color.white);
        private ColorPickerMode _mode = ColorPickerMode.Combined;
        private string _tooltip = "";
        private string _buttonText = "Select Color";
        private List<Color> _paletteColors = new List<Color>();
        private bool _showColorSwatch = true;
        private float _swatchSize = 20f;

        public Color Color
        {
            get => _colorBox.Value;
            set
            {
                if (_colorBox.Value != value)
                {
                    _colorBox.Value = value;
                    InvalidateLayout();
                }
            }
        }

        public StrongBox<Color> ColorBox
        {
            get => _colorBox;
            set => _colorBox = value ?? new StrongBox<Color>(Color.white);
        }

        public ColorPickerMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                }
            }
        }

        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        public string ButtonText
        {
            get => _buttonText;
            set
            {
                if (_buttonText != value)
                {
                    _buttonText = value ?? "Select Color";
                    InvalidateLayout();
                }
            }
        }

        public List<Color> PaletteColors
        {
            get => _paletteColors;
            set => _paletteColors = value ?? new List<Color>();
        }

        public bool ShowColorSwatch
        {
            get => _showColorSwatch;
            set
            {
                if (_showColorSwatch != value)
                {
                    _showColorSwatch = value;
                    InvalidateLayout();
                }
            }
        }

        public float SwatchSize
        {
            get => _swatchSize;
            set
            {
                float newSize = Mathf.Max(10f, value);
                if (_swatchSize != newSize)
                {
                    _swatchSize = newSize;
                    InvalidateLayout();
                }
            }
        }

        public ColorPicker(
            Color? color = null,
            StrongBox<Color> colorBox = null,
            ColorPickerMode? mode = null,
            string buttonText = null,
            List<Color> paletteColors = null,
            bool? showColorSwatch = null,
            float? swatchSize = null,
            string tooltip = null,
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
            _colorBox = colorBox ?? new StrongBox<Color>(color ?? Color.white);
            _mode = mode ?? ColorPickerMode.Combined;
            _buttonText = buttonText ?? "Select Color";
            _paletteColors = paletteColors ?? new List<Color>();
            _showColorSwatch = showColorSwatch ?? true;
            _swatchSize = swatchSize ?? 20f;
            _tooltip = tooltip ?? "";
        }

        protected override Size ComputeIntrinsicSize()
        {
            var originalWordWrap = Verse.Text.WordWrap;
            try
            {
                Verse.Text.WordWrap = false;
                var textSize = Verse.Text.CalcSize(_buttonText);

                float intrinsicWidth = textSize.x + 20f;
                float intrinsicHeight = Math.Max(textSize.y + 10f, 30f);

                if (_showColorSwatch)
                {
                    intrinsicWidth += _swatchSize + 8f;
                    intrinsicHeight = Math.Max(intrinsicHeight, _swatchSize + 10f);
                }

                return new Size(Math.Max(1f, intrinsicWidth), Math.Max(1f, intrinsicHeight));
            }
            finally
            {
                Verse.Text.WordWrap = originalWordWrap;
            }
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
            var originalColor = _colorBox.Value;

            Widgets.DrawButtonGraphic(rect);

            if (Widgets.ButtonInvisible(rect))
            {
                OpenColorPickerWindow();
            }

            if (_showColorSwatch)
            {
                var swatchRect = new Rect(
                    rect.x + 8f,
                    rect.y + (rect.height - _swatchSize) / 2f,
                    _swatchSize,
                    _swatchSize
                );

                GUI.color = _colorBox.Value;
                GUI.DrawTexture(swatchRect, BaseContent.WhiteTex);
                GUI.color = Color.white;

                Widgets.DrawBox(swatchRect, 1);
            }

            var textRect = new Rect(rect);
            if (_showColorSwatch)
            {
                textRect.x += _swatchSize + 16f;
                textRect.width -= _swatchSize + 16f;
            }

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(textRect, _buttonText);
            Text.Anchor = TextAnchor.UpperLeft;

            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        private void OpenColorPickerWindow()
        {
            var window = new Window_ColorPicker(_colorBox, _mode, _paletteColors);
            Find.WindowStack.Add(window);
        }

        public Rect CreateRect()
        {
            return ComputedRect;
        }

        public void SetMode(ColorPickerMode mode)
        {
            Mode = mode;
        }

        public void SetPalette(List<Color> colors)
        {
            PaletteColors = colors;
        }
    }
}