using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class Checkbox : UIElement
    {
        private string _tooltip = "";
        private StrongBox<bool> _checked;
        private string _label = "";

        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        public StrongBox<bool> Checked
        {
            get => _checked;
            set => _checked = value;
        }

        public string Label
        {
            get => _label;
            set
            {
                if (_label != value)
                {
                    _label = value;
                    InvalidateLayout();
                }
            }
        }

        public bool IsEmpty => string.IsNullOrWhiteSpace(_label);

        public Checkbox(
            string label = null,
            StrongBox<bool> isChecked = null,
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
            _checked = isChecked ?? new StrongBox<bool>(false);
            _tooltip = tooltip ?? "";
        }

        protected override Size ComputeIntrinsicSize()
        {
            float intrinsicWidth, intrinsicHeight;

            if (IsEmpty)
            {
                intrinsicWidth = 24f;
                intrinsicHeight = 24f;
            }
            else
            {
                var originalWordWrap = Verse.Text.WordWrap;
                try
                {
                    Verse.Text.WordWrap = false;
                    var textSize = Verse.Text.CalcSize(_label);
                    intrinsicWidth = 24f + 4f + textSize.x;
                    intrinsicHeight = 24f;
                }
                finally
                {
                    Verse.Text.WordWrap = originalWordWrap;
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

            if (IsEmpty)
            {
                Widgets.Checkbox(rect.x, rect.y, ref _checked.Value);
            }
            else
            {
                Widgets.CheckboxLabeled(rect, _label, ref _checked.Value);
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
    }
}