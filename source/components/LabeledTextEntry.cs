using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's text entry functionality that provides a user-friendly API
    /// for creating interactive text input controls with labels in the UI.
    /// </summary>
    public class LabeledTextEntry : UIElement
    {
        private string _label;
        private StrongBox<string> _valueBox;
        private string _tooltip;
        private float _spacing = 6f;
        private int _lineCount = 1;
        private int? _maxLength;
        private TextLineHeightProvider _heightLineProvider;
        private TextWidthProvider _textWidthProvider;

        /// <summary>
        /// Gets or sets the label text to display next to the text entry.
        /// </summary>
        public string Label
        {
            get => _label;
            set => _label = value;
        }

        /// <summary>
        /// Gets or sets the text content of the text entry.
        /// </summary>
        public string Value
        {
            get => _valueBox.Value;
            set => _valueBox.Value = value ?? "";
        }

        /// <summary>
        /// Gets or sets the tooltip text to display when hovering over the labeled text entry.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        /// <summary>
        /// Gets or sets the spacing between the label and the text entry.
        /// </summary>
        public float Spacing
        {
            get => _spacing;
            set => _spacing = value;
        }

        /// <summary>
        /// Gets or sets the number of lines for the text entry. 1 = single line, >1 = multi-line.
        /// </summary>
        public int LineCount
        {
            get => _lineCount;
            set => _lineCount = Math.Max(1, value);
        }

        /// <summary>
        /// Gets or sets the maximum length of text that can be entered. Null for no limit.
        /// </summary>
        public int? MaxLength
        {
            get => _maxLength;
            set => _maxLength = value;
        }

        /// <summary>
        /// Gets whether the labeled text entry has empty or whitespace-only label text.
        /// </summary>
        public bool IsLabelEmpty => string.IsNullOrWhiteSpace(_label);

        /// <summary>
        /// Gets whether this text entry is configured for multi-line input.
        /// </summary>
        public bool IsMultiLine => _lineCount > 1;

        public TextLineHeightProvider TextLineHeightProvider
        {
            get => _heightLineProvider;
            set => _heightLineProvider = value;
        }

        public TextWidthProvider TextWidthProvider
        {
            get => _textWidthProvider;
            set => _textWidthProvider = value;
        }

        /// <summary>
        /// Creates a new labeled text entry with content-based sizing.
        /// </summary>
        /// <param name="label">The label text to display</param>
        /// <param name="value">The initial text content</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledTextEntry(string label, StrongBox<string> value, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _label = label ?? "";
            _valueBox = value ?? throw new ArgumentNullException(nameof(value));
            TextLineHeightProvider = new TextLineHeightProvider();
            TextWidthProvider = new TextWidthProvider();
        }

        /// <summary>
        /// Creates a new labeled text entry with specified width sizing mode.
        /// </summary>
        /// <param name="label">The label text to display</param>
        /// <param name="value">The initial text content</param>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledTextEntry(string label, StrongBox<string> value, SizeMode widthMode, UIElementOptions options = null) : base(widthMode, SizeMode.Content, options)
        {
            _label = label ?? "";
            _valueBox = value ?? throw new ArgumentNullException(nameof(value));
            TextLineHeightProvider = new TextLineHeightProvider();
            TextWidthProvider = new TextWidthProvider();
        }

        /// <summary>
        /// Creates a new labeled text entry with fixed dimensions.
        /// </summary>
        /// <param name="label">The label text to display</param>
        /// <param name="value">The initial text content</param>
        /// <param name="width">Fixed width of the labeled text entry</param>
        /// <param name="height">Fixed height of the labeled text entry</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledTextEntry(string label, StrongBox<string> value, float width, float height, UIElementOptions options = null) : base(width, height, options)
        {
            _label = label ?? "";
            _valueBox = value ?? throw new ArgumentNullException(nameof(value));
            TextLineHeightProvider = new TextLineHeightProvider();
            TextWidthProvider = new TextWidthProvider();
        }

        /// <summary>
        /// Calculates the dynamic size of the labeled text entry based on its content.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            base.CalculateDynamicSize();
            // Height is based on line count
            float totalHeight = GetLineHeight() * _lineCount;

            // Handle Fill mode
            if (WidthMode == SizeMode.Fill)
            {
                Height = Math.Max(1f, totalHeight);
                return;
            }

            // Handle empty label (Content mode)
            if (IsLabelEmpty)
            {
                // Just text entry size
                float textEntryWidth = GetTextEntryWidth();
                Width = Math.Max(1f, textEntryWidth);
                Height = Math.Max(1f, totalHeight);
                return;
            }

            // Calculate label width
            float labelWidth = GetTextWidth(_label);

            // Calculate text entry width
            float textEntryWidthContent = GetTextEntryWidth();

            // Total width: label + spacing + text entry
            float totalWidth = labelWidth + _spacing + textEntryWidthContent;

            Width = Math.Max(1f, totalWidth);
            Height = Math.Max(1f, totalHeight);
        }

        /// <summary>
        /// Gets the width for the text entry portion, taking into account the sizing mode.
        /// </summary>
        /// <returns>The calculated text entry width</returns>
        private float GetTextEntryWidth()
        {
            if (WidthMode == SizeMode.Fill)
            {
                Console.WriteLine("B4");
                if (Parent == null) return 0f;
                Console.WriteLine("A4");
                if (IsLabelEmpty)
                {
                    return Parent.Width;
                }

                float labelWidth = GetTextWidth(_label);
                return Math.Max(0f, Parent.Width - labelWidth - _spacing);
            }

            return 120f; // Default text entry width for content mode
        }

        /// <summary>
        /// Renders the labeled text entry using RimWorld's UI system.
        /// </summary>
        protected override void RenderElement()
        {
            if (IsLabelEmpty)
            {
                // If no label, just render text entry
                RenderTextEntryOnly();
                return;
            }

            // Calculate label width and text entry width
            float labelWidth = GetTextWidth(_label);
            float textEntryWidth = GetTextEntryWidth();

            // Create rects for label and text entry
            var labelRect = new Rect(X, Y, labelWidth, Height);
            var textEntryRect = new Rect(X + labelWidth + _spacing, Y, textEntryWidth, Height);

            // Render the label
            Widgets.Label(labelRect, _label);

            // Render appropriate text control based on line count
            if (IsMultiLine)
            {
                _valueBox.Value = Widgets.TextArea(textEntryRect, _valueBox.Value);
            }
            else
            {
                _valueBox.Value = Widgets.TextField(textEntryRect, _valueBox.Value);
            }

            // Apply max length if set
            if (_maxLength.HasValue && _valueBox.Value.Length > _maxLength.Value)
            {
                _valueBox.Value = _valueBox.Value.Substring(0, _maxLength.Value);
            }

            // Handle tooltip if present
            if (!string.IsNullOrEmpty(_tooltip))
            {
                var fullRect = CreateRect();
                TooltipHandler.TipRegion(fullRect, _tooltip);
            }
        }

        /// <summary>
        /// Renders just the text entry when there's no label.
        /// </summary>
        private void RenderTextEntryOnly()
        {
            // Create rect for just the text entry
            var textEntryRect = CreateRect();

            // Render appropriate text control based on line count
            if (IsMultiLine)
            {
                _valueBox.Value = Widgets.TextArea(textEntryRect, _valueBox.Value);
            }
            else
            {
                _valueBox.Value = Widgets.TextField(textEntryRect, _valueBox.Value);
            }

            // Apply max length if set
            if (_maxLength.HasValue && _valueBox.Value.Length > _maxLength.Value)
            {
                _valueBox.Value = _valueBox.Value.Substring(0, _maxLength.Value);
            }

            // Handle tooltip if present
            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(textEntryRect, _tooltip);
            }
        }

        /// <summary>
        /// Creates a Unity Rect from the labeled text entry's position and size.
        /// </summary>
        /// <returns>A Rect representing the labeled text entry's bounds</returns>
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
            return TextWidthProvider.GetTextWidth(text);
        }

        /// <summary>
        /// Gets the height of a single line of text.
        /// </summary>
        /// <returns>The line height</returns>
        private float GetLineHeight()
        {
            return TextLineHeightProvider.GetLineHeight();
        }
    }
}