using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's labeled checkbox functionality that provides a user-friendly API
    /// for creating interactive checkbox controls with text labels in the UI.
    /// </summary>
    public class LabeledCheckbox : UIElement
    {
        private StrongBox<bool> _checkedBox;
        private string _text;
        private string _tooltip;
        private float _spacing = 6f; // Default spacing between checkbox and text

        /// <summary>
        /// Gets or sets whether the checkbox is currently checked.
        /// </summary>
        public bool Checked
        {
            get => _checkedBox.Value;
            set => _checkedBox.Value = value;
        }

        /// <summary>
        /// Gets or sets the text content to display next to the checkbox.
        /// </summary>
        public string Text
        {
            get => _text;
            set => _text = value;
        }

        /// <summary>
        /// Gets or sets the tooltip text to display when hovering over the checkbox.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        /// <summary>
        /// Gets or sets the spacing between the checkbox and the text label.
        /// </summary>
        public float Spacing
        {
            get => _spacing;
            set => _spacing = value;
        }

        /// <summary>
        /// Gets whether the labeled checkbox has empty or whitespace-only text.
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(_text);

        /// <summary>
        /// Creates a new labeled checkbox with content-based sizing.
        /// </summary>
        /// <param name="text">The text to display next to the checkbox</param>
        /// <param name="isChecked">StrongBox containing the checkbox state that will be updated automatically</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledCheckbox(string text, StrongBox<bool> isChecked, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _text = text ?? "";
            _checkedBox = isChecked ?? throw new ArgumentNullException(nameof(isChecked));
        }

        /// <summary>
        /// Creates a new labeled checkbox with fixed dimensions.
        /// </summary>
        /// <param name="text">The text to display next to the checkbox</param>
        /// <param name="width">Fixed width of the labeled checkbox</param>
        /// <param name="height">Fixed height of the labeled checkbox</param>
        /// <param name="isChecked">StrongBox containing the checkbox state that will be updated automatically</param>
        /// <param name="options">Optional UI element options</param>
        public LabeledCheckbox(string text, float width, float height, StrongBox<bool> isChecked, UIElementOptions options = null) : base(width, height, options)
        {
            _text = text ?? "";
            _checkedBox = isChecked ?? throw new ArgumentNullException(nameof(isChecked));
        }

        /// <summary>
        /// Calculates the dynamic size of the labeled checkbox based on its text content.
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

            // Total width: checkbox (24) + spacing + text width
            float totalWidth = 24f + _spacing + textWidth;

            // Height is the maximum of checkbox height and text line height
            float totalHeight = Math.Max(24f, GetLineHeight());

            Width = Math.Max(1f, totalWidth);
            Height = Math.Max(1f, totalHeight);
        }

        /// <summary>
        /// Renders the labeled checkbox using RimWorld's UI system.
        /// </summary>
        protected override void RenderElement()
        {
            if (IsEmpty)
            {
                // If no text, just render a checkbox
                RenderCheckboxOnly();
                return;
            }

            // Create rect for rendering
            var rect = CreateRect();

            // Render the labeled checkbox using RimWorld's CheckboxLabeled
            Widgets.CheckboxLabeled(rect, _text, ref _checkedBox.Value);

            // Handle tooltip if present
            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        /// <summary>
        /// Renders just the checkbox when there's no text.
        /// </summary>
        private void RenderCheckboxOnly()
        {
            // Create rect for just the checkbox
            var checkboxRect = new Rect(X, Y, 24f, 24f);

            // Render the checkbox
            Widgets.Checkbox(checkboxRect.x, checkboxRect.y, ref _checkedBox.Value);

            // Handle tooltip if present
            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(checkboxRect, _tooltip);
            }
        }

        /// <summary>
        /// Creates a Unity Rect from the labeled checkbox's position and size.
        /// </summary>
        /// <returns>A Rect representing the labeled checkbox's bounds</returns>
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