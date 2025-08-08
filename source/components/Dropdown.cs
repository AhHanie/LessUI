using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's dropdown functionality that provides a user-friendly API
    /// for creating dropdown selection controls in the UI.
    /// </summary>
    /// <typeparam name="T">The type of items in the dropdown</typeparam>
    public class Dropdown<T> : UIElement
    {
        private List<T> _items;
        private T _selectedItem;
        private Func<T, string> _displayTextFunc;
        private string _tooltip;
        private string _placeholder;
        private StrongBox<T> _selectedItemBox;

        /// <summary>
        /// Gets or sets the list of items in the dropdown.
        /// </summary>
        public List<T> Items
        {
            get => _items;
            set
            {
                _items = value ?? new List<T>();
                var currentSelection = _selectedItemBox.Value;
                if (!_items.Contains(currentSelection))
                {
                    _selectedItemBox.Value = default(T);
                    _selectedItem = default(T);
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected item.
        /// </summary>
        public T SelectedItem
        {
            get => _selectedItemBox.Value;
            set
            {
                if (_items.Contains(value) || EqualityComparer<T>.Default.Equals(value, default(T)))
                {
                    var oldValue = _selectedItemBox.Value;
                    _selectedItemBox.Value = value;
                    _selectedItem = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the function used to convert items to display text.
        /// </summary>
        public Func<T, string> DisplayTextFunc
        {
            get => _displayTextFunc;
            set => _displayTextFunc = value ?? (item => item?.ToString() ?? "");
        }

        /// <summary>
        /// Gets or sets the tooltip text to display when hovering over the dropdown.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        /// <summary>
        /// Gets or sets the placeholder text to display when no item is selected.
        /// </summary>
        public string Placeholder
        {
            get => _placeholder;
            set => _placeholder = value ?? "";
        }

        /// <summary>
        /// Gets the display text for the currently selected item.
        /// </summary>
        public string SelectedDisplayText
        {
            get
            {
                var selectedItem = _selectedItemBox.Value;
                if (EqualityComparer<T>.Default.Equals(selectedItem, default(T)))
                {
                    return string.IsNullOrEmpty(_placeholder) ? "Select..." : _placeholder;
                }
                return _displayTextFunc(selectedItem);
            }
        }

        /// <summary>
        /// Gets whether an item is currently selected.
        /// </summary>
        public bool HasSelection => !EqualityComparer<T>.Default.Equals(_selectedItemBox.Value, default(T));

        /// <summary>
        /// Gets the StrongBox containing the selected item.
        /// </summary>
        public StrongBox<T> SelectedItemBox => _selectedItemBox;

        /// <summary>
        /// Creates a new dropdown with content-based sizing.
        /// </summary>
        /// <param name="items">The list of items in the dropdown</param>
        /// <param name="selectedItem">StrongBox containing the selected item that will be updated automatically</param>
        /// <param name="displayTextFunc">Function to convert items to display text</param>
        /// <param name="options">Optional UI element options</param>
        public Dropdown(List<T> items, StrongBox<T> selectedItem, Func<T, string> displayTextFunc = null, UIElementOptions options = null)
            : base(SizeMode.Content, SizeMode.Content, options)
        {
            _items = items ?? new List<T>();
            _selectedItemBox = selectedItem ?? throw new ArgumentNullException(nameof(selectedItem));
            _displayTextFunc = displayTextFunc ?? (item => item?.ToString() ?? "");
            _selectedItem = _selectedItemBox.Value;
            _placeholder = "Select...";
        }

        /// <summary>
        /// Creates a new dropdown with fixed dimensions.
        /// </summary>
        /// <param name="items">The list of items in the dropdown</param>
        /// <param name="width">Fixed width of the dropdown</param>
        /// <param name="height">Fixed height of the dropdown</param>
        /// <param name="selectedItem">StrongBox containing the selected item that will be updated automatically</param>
        /// <param name="displayTextFunc">Function to convert items to display text</param>
        /// <param name="options">Optional UI element options</param>
        public Dropdown(List<T> items, float width, float height, StrongBox<T> selectedItem, Func<T, string> displayTextFunc = null, UIElementOptions options = null)
            : base(width, height, options)
        {
            _items = items ?? new List<T>();
            _selectedItemBox = selectedItem ?? throw new ArgumentNullException(nameof(selectedItem));
            _displayTextFunc = displayTextFunc ?? (item => item?.ToString() ?? "");
            _selectedItem = _selectedItemBox.Value;
            _placeholder = "Select...";
        }

        /// <summary>
        /// Creates a new dropdown with specified width sizing mode.
        /// </summary>
        /// <param name="items">The list of items in the dropdown</param>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="selectedItem">StrongBox containing the selected item that will be updated automatically</param>
        /// <param name="displayTextFunc">Function to convert items to display text</param>
        /// <param name="options">Optional UI element options</param>
        public Dropdown(List<T> items, SizeMode widthMode, StrongBox<T> selectedItem, Func<T, string> displayTextFunc = null, UIElementOptions options = null)
            : base(widthMode, SizeMode.Content, options)
        {
            _items = items ?? new List<T>();
            _selectedItemBox = selectedItem ?? throw new ArgumentNullException(nameof(selectedItem));
            _displayTextFunc = displayTextFunc ?? (item => item?.ToString() ?? "");
            _selectedItem = _selectedItemBox.Value;
            _placeholder = "Select...";
        }

        /// <summary>
        /// Calculates the dynamic size of the dropdown based on its content.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            base.CalculateDynamicSize();
            float height = 30f;

            if (WidthMode == SizeMode.Fill)
            {
                Height = height;
                return;
            }

            float width = 150f;

            if (_items.Any())
            {
                float maxItemWidth = _items.Max(item => GetTextWidth(_displayTextFunc(item)));
                width = Math.Max(width, maxItemWidth + 30f);
            }

            if (!string.IsNullOrEmpty(_placeholder))
            {
                float placeholderWidth = GetTextWidth(_placeholder);
                width = Math.Max(width, placeholderWidth + 30f);
            }

            Width = width;
            Height = height;
        }

        /// <summary>
        /// Renders the dropdown using RimWorld's UI system.
        /// </summary>
        protected override void RenderElement()
        {
            var rect = CreateRect();

            if (Widgets.ButtonText(rect, SelectedDisplayText))
            {
                ShowDropdownMenu();
            }

            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        /// <summary>
        /// Shows the dropdown menu using RimWorld's FloatMenu system.
        /// </summary>
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

        /// <summary>
        /// Creates a Unity Rect from the dropdown's position and size.
        /// </summary>
        /// <returns>A Rect representing the dropdown's bounds</returns>
        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }

        /// <summary>
        /// Gets the width of the specified text.
        /// </summary>
        /// <param name="text">The text to measure</param>
        /// <returns>The width of the text</returns>
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

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        public void ClearSelection()
        {
            SelectedItem = default(T);
        }

        /// <summary>
        /// Selects the first item in the dropdown.
        /// </summary>
        public void SelectFirst()
        {
            if (_items.Any())
            {
                SelectedItem = _items.First();
            }
        }

        /// <summary>
        /// Selects the last item in the dropdown.
        /// </summary>
        public void SelectLast()
        {
            if (_items.Any())
            {
                SelectedItem = _items.Last();
            }
        }

        /// <summary>
        /// Gets the index of the currently selected item.
        /// </summary>
        /// <returns>The index of the selected item, or -1 if no item is selected</returns>
        public int GetSelectedIndex()
        {
            return _items.IndexOf(_selectedItemBox.Value);
        }

        /// <summary>
        /// Selects an item by its index in the dropdown.
        /// </summary>
        /// <param name="index">The index of the item to select</param>
        public void SelectByIndex(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                SelectedItem = _items[index];
            }
        }
    }
}