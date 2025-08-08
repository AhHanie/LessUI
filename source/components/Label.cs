using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's label functionality that provides a user-friendly API
    /// for displaying text in the UI.
    /// </summary>
    public class Label : UIElement
    {
        private StrongBox<string> _text;
        private float? _maxWidth;
        private bool _wordWrap = true;
        private string _tooltip;

        /// <summary>
        /// Gets or sets the text content to display in the label.
        /// </summary>
        public string Content
        {
            get => _text.Value;
            set => _text.Value = value;
        }

        /// <summary>
        /// Gets or sets the text content to display in the label.
        /// Alias for Content property for backward compatibility.
        /// </summary>
        public string Text
        {
            get => _text.Value;
            set => Content = value;
        }

        /// <summary>
        /// Gets or sets the tooltip text to display when hovering over the label.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        /// <summary>
        /// Gets or sets whether the text should wrap to multiple lines when it exceeds the available width.
        /// </summary>
        public bool WordWrap
        {
            get => _wordWrap;
            set => _wordWrap = value;
        }

        /// <summary>
        /// Gets or sets the maximum width for the label. When set, text will wrap to fit within this width.
        /// </summary>
        public float? MaxWidth
        {
            get => _maxWidth;
            set => _maxWidth = value;
        }

        /// <summary>
        /// Gets whether the label has empty or whitespace-only text.
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(_text.Value);

        /// <summary>
        /// Creates a new label with content-based sizing.
        /// </summary>
        /// <param name="text">The StrongBox containing the text to display</param>
        /// <param name="options">Optional UI element options</param>
        /// <exception cref="ArgumentNullException">Thrown when text is null</exception>
        public Label(StrongBox<string> text, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        /// Creates a new label with fixed dimensions.
        /// </summary>
        /// <param name="text">The StrongBox containing the text to display</param>
        /// <param name="width">Fixed width of the label</param>
        /// <param name="height">Fixed height of the label</param>
        /// <param name="options">Optional UI element options</param>
        /// <exception cref="ArgumentNullException">Thrown when text is null</exception>
        public Label(StrongBox<string> text, float width, float height, UIElementOptions options = null) : base(width, height, options)
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        /// Creates a new label with content-based sizing as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="text">The StrongBox containing the text to display</param>
        /// <param name="options">Optional UI element options</param>
        /// <exception cref="ArgumentNullException">Thrown when text is null</exception>
        public Label(UIElement parent, StrongBox<string> text, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
            if (parent != null)
            {
                parent.AddChild(this);
            }
        }

        /// <summary>
        /// Calculates the dynamic size of the label based on its text content.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            // Handle empty text
            if (IsEmpty)
            {
                Width = 1f;
                Height = GetLineHeight();
                return;
            }

            float availableWidth = _maxWidth ?? float.MaxValue;

            // Store current text settings
            var originalWordWrap = Verse.Text.WordWrap;

            try
            {
                // Set word wrap for calculation
                Verse.Text.WordWrap = _wordWrap;

                float calculatedWidth;
                float calculatedHeight;

                if (_wordWrap && _maxWidth.HasValue)
                {
                    // Calculate with word wrapping and max width constraint
                    calculatedWidth = _maxWidth.Value;
                    calculatedHeight = Verse.Text.CalcHeight(_text.Value, calculatedWidth);
                }
                else if (_wordWrap)
                {
                    // Calculate optimal width for wrapping (use a reasonable default max width)
                    float defaultMaxWidth = 300f; // Default max width for auto-sizing
                    var singleLineWidth = GetTextWidth(_text.Value);

                    if (singleLineWidth <= defaultMaxWidth)
                    {
                        // Text fits on one line
                        calculatedWidth = singleLineWidth;
                        calculatedHeight = GetLineHeight();
                    }
                    else
                    {
                        // Text needs wrapping
                        calculatedWidth = defaultMaxWidth;
                        calculatedHeight = Verse.Text.CalcHeight(_text.Value, calculatedWidth);
                    }
                }
                else
                {
                    // No word wrap - single line
                    calculatedWidth = GetTextWidth(_text.Value);
                    calculatedHeight = GetLineHeight();
                }

                Width = Math.Max(1f, calculatedWidth);
                Height = Math.Max(1f, calculatedHeight);
            }
            finally
            {
                // Restore original word wrap setting
                Verse.Text.WordWrap = originalWordWrap;
            }
        }

        /// <summary>
        /// Renders the label.
        /// </summary>
        protected override void RenderElement()
        {
            if (IsEmpty) return;

            // Store current text settings
            var originalWordWrap = Verse.Text.WordWrap;

            try
            {
                // Set text properties for rendering
                Verse.Text.WordWrap = _wordWrap;

                // Create rect for rendering
                var rect = CreateRect();

                // Render the label
                Widgets.Label(rect, _text.Value);

                // Handle tooltip if present
                if (!string.IsNullOrEmpty(_tooltip))
                {
                    TooltipHandler.TipRegion(rect, _tooltip);
                }
            }
            finally
            {
                // Restore original text settings
                Verse.Text.WordWrap = originalWordWrap;
            }
        }

        /// <summary>
        /// Creates a Unity Rect from the label's position and size.
        /// </summary>
        /// <returns>A Rect representing the label's bounds</returns>
        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }

        /// <summary>
        /// Gets the width of the specified text without word wrapping.
        /// </summary>
        /// <param name="text">The text to measure</param>
        /// <returns>The width of the text</returns>
        private float GetTextWidth(string text)
        {
            if (string.IsNullOrEmpty(text)) return 1f;

            var originalWordWrap = Verse.Text.WordWrap;
            try
            {
                Verse.Text.WordWrap = false;
                var size = Verse.Text.CalcSize(text);
                return size.x;
            }
            finally
            {
                Verse.Text.WordWrap = originalWordWrap;
            }
        }

        /// <summary>
        /// Gets the height of a single line of text.
        /// </summary>
        /// <returns>The line height</returns>
        private float GetLineHeight()
        {
            return Verse.Text.LineHeight;
        }
    }
}