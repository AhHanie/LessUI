using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's button functionality that provides a user-friendly API
    /// for creating interactive button controls in the UI.
    /// </summary>
    public class Button : UIElement
    {
        private string _text;
        private string _tooltip;
        private bool _disabled;
        private StrongBox<bool> _clickedBox;

        /// <summary>
        /// Gets or sets the text content to display on the button.
        /// </summary>
        public string Text
        {
            get => _text;
            set => _text = value;
        }

        /// <summary>
        /// Gets or sets the tooltip text to display when hovering over the button.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        /// <summary>
        /// Gets or sets whether the button is disabled and cannot be interacted with.
        /// </summary>
        public bool Disabled
        {
            get => _disabled;
            set => _disabled = value;
        }

        /// <summary>
        /// Gets whether the button has empty or whitespace-only text.
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(_text);

        /// <summary>
        /// Gets or sets whether the button has been clicked.
        /// Modifies the underlying StrongBox value.
        /// </summary>
        public bool Clicked
        {
            get => _clickedBox.Value;
            set => _clickedBox.Value = value;
        }

        /// <summary>
        /// Gets or sets the StrongBox containing the clicked state.
        /// This allows external code to share the same clicked state reference.
        /// </summary>
        public StrongBox<bool> ClickedBox
        {
            get => _clickedBox;
            set => _clickedBox = value ?? new StrongBox<bool>(false);
        }

        /// <summary>
        /// Creates a new button with content-based sizing.
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="clicked">StrongBox containing the clicked state</param>
        /// <param name="options">Optional UI element options</param>
        public Button(string text, StrongBox<bool> clicked, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _text = text ?? "";
            _clickedBox = clicked ?? throw new ArgumentNullException(nameof(clicked));
            _disabled = false;
        }

        /// <summary>
        /// Creates a new button with fixed dimensions.
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="width">Fixed width of the button</param>
        /// <param name="height">Fixed height of the button</param>
        /// <param name="clicked">StrongBox containing the clicked state</param>
        /// <param name="options">Optional UI element options</param>
        public Button(string text, float width, float height, StrongBox<bool> clicked, UIElementOptions options = null) : base(width, height, options)
        {
            _text = text ?? "";
            _clickedBox = clicked ?? throw new ArgumentNullException(nameof(clicked));
            _disabled = false;
        }

        /// <summary>
        /// Calculates the dynamic size of the button based on its text content.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            // Handle empty text
            if (IsEmpty)
            {
                Width = 60f;
                Height = 30f;
            }

            // Store current text settings
            var originalWordWrap = Verse.Text.WordWrap;

            try
            {
                // Set text properties for calculation
                Verse.Text.WordWrap = false;

                // Calculate text size
                var textSize = Verse.Text.CalcSize(_text);

                // Add padding around text (typical button padding)
                float buttonWidth = textSize.x + 20f; // 10px padding on each side
                float buttonHeight = Math.Max(textSize.y + 10f, 30f); // 5px padding top/bottom, minimum 30px height

                Width = Math.Max(1f, buttonWidth);
                Height = Math.Max(1f, buttonHeight);
            }
            finally
            {
                // Restore original text settings
                Verse.Text.WordWrap = originalWordWrap;
            }
        }

        /// <summary>
        /// Renders the button using RimWorld's UI system.
        /// </summary>
        protected override void RenderElement()
        {
            var rect = CreateRect();

            // Render the button and capture whether it was clicked
            bool wasClicked = Widgets.ButtonText(rect, _text ?? "", drawBackground: true, doMouseoverSound: true, active: !_disabled);

            // If clicked and not disabled, set the clicked state to true
            if (wasClicked && !_disabled)
            {
                _clickedBox.Value = true;
            }

            if (!wasClicked && Clicked)
            {
                _clickedBox.Value = false;
            }

            // Handle tooltip if present
            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        /// <summary>
        /// Creates a Unity Rect from the button's position and size.
        /// </summary>
        /// <returns>A Rect representing the button's bounds</returns>
        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }
    }
}