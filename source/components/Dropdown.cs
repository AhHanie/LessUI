using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class Dropdown<T> : UIElement
    {
        private List<T> _items = new List<T>();
        private StrongBox<T> _selectedItem = new StrongBox<T>(default(T));
        private Func<T, string> _displayTextFunc = item => item?.ToString() ?? "";
        private string _tooltip = "";
        private string _placeholder = "";

        public List<T> Items
        {
            get => _items;
            set
            {
                _items = value ?? new List<T>();
                if (!_items.Contains(SelectedItem))
                {
                    SelectedItem = default(T);
                }
                InvalidateLayout();
            }
        }

        public T SelectedItem
        {
            get => _selectedItem.Value;
            set
            {
                if (_items.Contains(value) || EqualityComparer<T>.Default.Equals(value, default(T)))
                {
                    _selectedItem.Value = value;
                }
            }
        }

        public Func<T, string> DisplayTextFunc
        {
            get => _displayTextFunc;
            set
            {
                _displayTextFunc = value ?? (item => item?.ToString() ?? "");
                InvalidateLayout();
            }
        }

        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        public string Placeholder
        {
            get => _placeholder;
            set
            {
                if (_placeholder != value)
                {
                    _placeholder = value ?? "";
                    InvalidateLayout();
                }
            }
        }

        public string SelectedDisplayText
        {
            get
            {
                if (EqualityComparer<T>.Default.Equals(SelectedItem, default(T)))
                {
                    return string.IsNullOrEmpty(_placeholder) ? "Select..." : _placeholder;
                }
                return _displayTextFunc(SelectedItem);
            }
        }

        public bool HasSelection => !EqualityComparer<T>.Default.Equals(SelectedItem, default(T));

        public Dropdown(
            List<T> items = null,
            StrongBox<T> selectedItem = null,
            Func<T, string> displayTextFunc = null,
            string tooltip = null,
            string placeholder = null,
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
            _items = items ?? new List<T>();
            _selectedItem = selectedItem ?? new StrongBox<T>(default(T));
            _displayTextFunc = displayTextFunc ?? (item => item?.ToString() ?? "");
            _tooltip = tooltip ?? "";
            _placeholder = placeholder ?? "";
        }

        protected override Size ComputeIntrinsicSize()
        {
            float intrinsicWidth = 150f;
            float intrinsicHeight = 30f;

            if (_items.Any())
            {
                float maxItemWidth = _items.Max(item => GetTextWidth(_displayTextFunc(item)));
                intrinsicWidth = Math.Max(intrinsicWidth, maxItemWidth + 30f);
            }

            if (!string.IsNullOrEmpty(_placeholder))
            {
                float placeholderWidth = GetTextWidth(_placeholder);
                intrinsicWidth = Math.Max(intrinsicWidth, placeholderWidth + 30f);
            }

            return new Size(intrinsicWidth, intrinsicHeight);
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

            if (Widgets.ButtonText(rect, SelectedDisplayText))
            {
                ShowDropdownMenu();
            }

            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        private void ShowDropdownMenu()
        {
            if (!_items.Any()) return;

            var options = new List<FloatMenuOption>();

            foreach (var item in _items)
            {
                var displayText = _displayTextFunc(item);
                var option = new FloatMenuOption(displayText, () => {
                    SelectedItem = item;
                });
                options.Add(option);
            }

            Find.WindowStack.Add(new FloatMenu(options));
        }

        public Rect CreateRect()
        {
            return ComputedRect;
        }

        private float GetTextWidth(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0f;

            var originalWordWrap = Text.WordWrap;
            try
            {
                Text.WordWrap = false;
                var size = Text.CalcSize(text);
                return size.x;
            }
            finally
            {
                Text.WordWrap = originalWordWrap;
            }
        }

        public void ClearSelection()
        {
            SelectedItem = default(T);
        }

        public void SelectFirst()
        {
            if (_items.Any())
            {
                SelectedItem = _items.First();
            }
        }

        public void SelectLast()
        {
            if (_items.Any())
            {
                SelectedItem = _items.Last();
            }
        }

        public int GetSelectedIndex()
        {
            return _items.IndexOf(SelectedItem);
        }

        public void SelectByIndex(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                SelectedItem = _items[index];
            }
        }
    }
}