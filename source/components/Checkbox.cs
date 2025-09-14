using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class Checkbox : UIElement
    {
        private string _tooltip = "";
        private bool _checked = false;
        private string _label = "";

        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        public bool Checked
        {
            get => _checked;
            set => _checked = value;
        }

        public string Label
        {
            get => _label;
            set => _label = value;
        }

        public bool IsEmpty => string.IsNullOrWhiteSpace(_label);

        public Checkbox(
            string label = null,
            bool? isChecked = null,
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
            _label = label ?? "";
            _checked = isChecked ?? false;
            _tooltip = tooltip ?? "";
        }

        protected override void CalculateElementSize()
        {
            base.CalculateElementSize();

            if (WidthMode == SizeMode.Content)
            {
                WidthCalculated = true;
                if (IsEmpty)
                {
                    Width = 24f;
                }
                else
                {
                    var originalWordWrap = Verse.Text.WordWrap;
                    Verse.Text.WordWrap = false;
                    var textSize = Verse.Text.CalcSize(_label);
                    Width = 24f + 4f + textSize.x;
                    Verse.Text.WordWrap = originalWordWrap;
                }
            }

            if (HeightMode == SizeMode.Content)
            {
                HeightCalculated = true;
                Height = 24f;
            }
        }

        protected override void RenderElement()
        {
            var rect = CreateRect();

            if (IsEmpty)
            {
                Widgets.Checkbox(rect.x, rect.y, ref _checked);
            }
            else
            {
                Widgets.CheckboxLabeled(rect, _label, ref _checked);
            }

            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }
    }
}