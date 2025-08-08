using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's text entry functionality that provides a user-friendly API
    /// for creating interactive text input controls in the UI.
    /// </summary>
    public class TextEntry : UIElement
    {
        private string _text;
        private string _tooltip;
        private int _lineCount = 1;
        private int? _maxLength;

        /// <summary>
        /// Gets or sets the text content of the text entry.
        /// </summary>
        public string Text
        {
            get => _text;
            set => _text = value ?? "";
        }

        /// <summary>
        /// Gets or sets the tooltip text to display when hovering over the text entry.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
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
        /// Gets whether this text entry is configured for multi-line input.
        /// </summary>
        public bool IsMultiLine => _lineCount > 1;

        /// <summary>
        /// Event callback that is invoked when the text content changes.
        /// The parameter contains the new text value.
        /// </summary>
        public Action<string> OnTextChanged { get; set; }

        /// <summary>
        /// Creates a new text entry with content-based sizing.
        /// </summary>
        /// <param name="text">The initial text content</param>
        /// <param name="onTextChanged">Callback function to execute when text changes</param>
        /// <param name="options">Optional UI element options</param>
        public TextEntry(string text, Action<string> onTextChanged = null, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _text = text ?? "";
            OnTextChanged = onTextChanged;
        }

        /// <summary>
        /// Creates a new text entry with specified width sizing mode.
        /// </summary>
        /// <param name="text">The initial text content</param>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="onTextChanged">Callback function to execute when text changes</param>
        /// <param name="options">Optional UI element options</param>
        public TextEntry(string text, SizeMode widthMode, Action<string> onTextChanged = null, UIElementOptions options = null) : base(widthMode, SizeMode.Content, options)
        {
            _text = text ?? "";
            OnTextChanged = onTextChanged;
        }

        /// <summary>
        /// Creates a new text entry with fixed dimensions.
        /// </summary>
        /// <param name="text">The initial text content</param>
        /// <param name="width">Fixed width of the text entry</param>
        /// <param name="height">Fixed height of the text entry</param>
        /// <param name="onTextChanged">Callback function to execute when text changes</param>
        /// <param name="options">Optional UI element options</param>
        public TextEntry(string text, float width, float height, Action<string> onTextChanged = null, UIElementOptions options = null) : base(width, height, options)
        {
            _text = text ?? "";
            OnTextChanged = onTextChanged;
        }

        /// <summary>
        /// Creates a new text entry with content-based sizing as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="text">The initial text content</param>
        /// <param name="onTextChanged">Callback function to execute when text changes</param>
        /// <param name="options">Optional UI element options</param>
        public TextEntry(UIElement parent, string text, Action<string> onTextChanged = null, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _text = text ?? "";
            OnTextChanged = onTextChanged;
            if (parent != null)
            {
                parent.AddChild(this);
            }
        }

        /// <summary>
        /// Creates a new text entry with specified width sizing mode as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="text">The initial text content</param>
        /// <param name="widthMode">The width sizing mode (Content or Fill)</param>
        /// <param name="onTextChanged">Callback function to execute when text changes</param>
        /// <param name="options">Optional UI element options</param>
        public TextEntry(UIElement parent, string text, SizeMode widthMode, Action<string> onTextChanged = null, UIElementOptions options = null) : base(widthMode, SizeMode.Content, options)
        {
            _text = text ?? "";
            OnTextChanged = onTextChanged;
            if (parent != null)
            {
                parent.AddChild(this);
            }
        }

        /// <summary>
        /// Calculates the dynamic size of the text entry.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            // Height based on line count
            float height = GetLineHeight() * _lineCount;

            // Handle Fill mode
            if (WidthMode == SizeMode.Fill)
            {
                Height = Math.Max(1f, height);
                return;
            }

            // Default width for text entry (Content mode)
            float width = 150f;

            Width = Math.Max(1f, width);
            Height = Math.Max(1f, height);
        }

        /// <summary>
        /// Renders the text entry using RimWorld's UI system.
        /// </summary>
        protected override void RenderElement()
        {
            // Create rect for rendering
            var rect = CreateRect();

            // Store original text to detect changes
            string originalText = _text;
            string newText;

            // Render appropriate text control based on line count
            if (IsMultiLine)
            {
                newText = Widgets.TextArea(rect, _text);
            }
            else
            {
                newText = Widgets.TextField(rect, _text);
            }

            // Apply max length if set
            if (_maxLength.HasValue && newText.Length > _maxLength.Value)
            {
                newText = newText.Substring(0, _maxLength.Value);
            }

            // Update text and trigger callback if changed
            if (newText != originalText)
            {
                _text = newText;
                OnTextChanged?.Invoke(_text);
            }

            // Handle tooltip if present
            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        /// <summary>
        /// Creates a Unity Rect from the text entry's position and size.
        /// </summary>
        /// <returns>A Rect representing the text entry's bounds</returns>
        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
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