using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's labeled radio button functionality that provides a user-friendly API
    /// for creating interactive radio button controls with text labels in the UI.
    /// </summary>
    public class LabeledRadioButton : UIElement
    {
        private StrongBox<bool> _selectedBox;
        private StrongBox<bool> _clickedBox;
        private bool _disabled;
        private string _text;
        private string _tooltip;
        private float _spacing = 6f; // Default spacing between radio button and text

        /// <summary>
        /// Gets or sets whether the radio button is currently selected.
        /// </summary>
        public bool Selected
        {
            get => _selectedBox.Value;
            set => _selectedBox.Value = value;
        }

        /// <summary>
        /// Gets or sets whether the radio button is disabled and cannot be interacted with.
        /// </summary>
        public bool Disabled
        {
            get => _disabled;
            set => _disabled = value;
        }

        /// <summary>
        /// Gets or sets the text content to display next to the radio button.
        /// </summary>
        public string Text
        {
            get => _text;
            set => _text = value;
        }

        /// <summary>
        /// Gets or sets the tooltip text to display when hovering over the radio button.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        /// <summary>
        /// Gets or sets the spacing between the radio button and the text label.
        /// </summary>
        public float Spacing
        {
            get => _spacing;
            set => _spacing = value;
        }

        /// <summary>
        /// Gets whether the labeled radio button has empty or whitespace-only text.
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(_text);

        /// <summary>
        /// Creates a new labeled radio button with content-based sizing.
        /// </summary>
        /// <param name="text">The text to display next to the radio button</param>
        /// <param name="selected">Whether the radio button is initially selected</param>
        /// <param name="onClick">Callback function to execute when clicked</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledRadioButton(string text, StrongBox<bool> selected, StrongBox<bool> clicked, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _text = text ?? "";
            _selectedBox = selected ?? throw new ArgumentNullException(nameof(selected));
            _disabled = false;
            _clickedBox = clicked ?? throw new ArgumentNullException(nameof(clicked));
        }

        /// <summary>
        /// Creates a new labeled radio button with fixed dimensions.
        /// </summary>
        /// <param name="text">The text to display next to the radio button</param>
        /// <param name="width">Fixed width of the labeled radio button</param>
        /// <param name="height">Fixed height of the labeled radio button</param>
        /// <param name="selected">Whether the radio button is initially selected</param>
        /// <param name="onClick">Callback function to execute when clicked</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledRadioButton(string text, float width, float height, StrongBox<bool> selected, StrongBox<bool> clicked, UIElementOptions options = null) : base(width, height, options)
        {
            _text = text ?? "";
            _selectedBox = selected ?? throw new ArgumentNullException(nameof(selected));
            _disabled = false;
            _clickedBox = clicked ?? throw new ArgumentNullException(nameof(clicked));
        }

        /// <summary>
        /// Calculates the dynamic size of the labeled radio button based on its text content.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            // Handle empty text
            if (IsEmpty)
            {
                Width = 24f;
                Height = GetLineHeight();
                return;
            }

            // Calculate text width
            float textWidth = GetTextWidth(_text);

            // Total width: radio button (24) + spacing + text width
            float totalWidth = 24f + _spacing + textWidth;

            // Height is the maximum of radio button height and text line height
            float totalHeight = Math.Max(24f, GetLineHeight());

            Width = Math.Max(1f, totalWidth);
            Height = Math.Max(1f, totalHeight);
        }

        /// <summary>
        /// Renders the labeled radio button using RimWorld's UI system.
        /// </summary>
        protected override void RenderElement()
        {
            if (IsEmpty)
            {
                // If no text, just render a radio button
                RenderRadioButtonOnly();
                return;
            }

            // Create rect for rendering
            var rect = CreateRect();


            // Render the labeled radio button using RimWorld's RadioButtonLabeled
            _clickedBox.Value = Widgets.RadioButtonLabeled(rect, _text, _selectedBox.Value, _disabled);

            // Handle tooltip if present
            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        /// <summary>
        /// Renders just the radio button when there's no text.
        /// </summary>
        private void RenderRadioButtonOnly()
        {
            // Create rect for just the radio button
            var radioButtonRect = new Rect(X, Y, 24f, GetLineHeight());

            // Render the radio button without text using RadioButtonLabeled with empty string
            _clickedBox.Value = Widgets.RadioButtonLabeled(radioButtonRect, "", _selectedBox.Value, _disabled);

            // Handle tooltip if present
            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(radioButtonRect, _tooltip);
            }
        }

        /// <summary>
        /// Creates a Unity Rect from the labeled radio button's position and size.
        /// </summary>
        /// <returns>A Rect representing the labeled radio button's bounds</returns>
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