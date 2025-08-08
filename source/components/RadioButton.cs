using System;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's radio button functionality that provides a user-friendly API
    /// for creating interactive radio button controls in the UI.
    /// </summary>
    public class RadioButton : UIElement
    {
        private bool _selected;
        private bool _disabled;
        private string _tooltip;

        /// <summary>
        /// Gets or sets whether the radio button is currently selected.
        /// </summary>
        public bool Selected
        {
            get => _selected;
            set => _selected = value;
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
        /// Gets or sets the tooltip text to display when hovering over the radio button.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        /// <summary>
        /// Event callback that is invoked when the radio button is clicked.
        /// </summary>
        public Action OnClick { get; set; }

        /// <summary>
        /// Creates a new radio button with content-based sizing.
        /// </summary>
        /// <param name="selected">Whether the radio button is initially selected</param>
        /// <param name="onClick">Callback function to execute when clicked</param>
        /// <param name="options">Optional UI element options</param>
        public RadioButton(bool selected, Action onClick, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _selected = selected;
            _disabled = false;
            OnClick = onClick;
        }

        /// <summary>
        /// Creates a new radio button with fixed dimensions.
        /// </summary>
        /// <param name="width">Fixed width of the radio button</param>
        /// <param name="height">Fixed height of the radio button</param>
        /// <param name="selected">Whether the radio button is initially selected</param>
        /// <param name="onClick">Callback function to execute when clicked</param>
        /// <param name="options">Optional UI element options</param>
        public RadioButton(float width, float height, bool selected, Action onClick, UIElementOptions options = null) : base(width, height, options)
        {
            _selected = selected;
            _disabled = false;
            OnClick = onClick;
        }

        /// <summary>
        /// Creates a new radio button as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent UI element</param>
        /// <param name="selected">Whether the radio button is initially selected</param>
        /// <param name="onClick">Callback function to execute when clicked</param>
        /// <param name="options">Optional UI element options</param>
        public RadioButton(UIElement parent, bool selected, Action onClick, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _selected = selected;
            _disabled = false;
            if (parent != null)
            {
                parent.AddChild(this);
            }
            OnClick = onClick;
        }

        /// <summary>
        /// Calculates the dynamic size of the radio button.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            // Radio button without text is just the radio circle (approximately 24 pixels)
            Width = 24f;
            Height = GetLineHeight();
        }

        /// <summary>
        /// Renders the radio button using RimWorld's UI system.
        /// </summary>
        protected override void RenderElement()
        {
            // Create rect for rendering
            var rect = CreateRect();

            // Store original selected state
            bool originalSelected = _selected;

            bool wasClicked;

            // Render the radio button without text label using RadioButtonLabeled with empty string
            wasClicked = Widgets.RadioButtonLabeled(rect, "", _selected, _disabled);

            // If clicked and not disabled, update selection state and trigger callback
            if (wasClicked && !_disabled)
            {
                // For radio buttons, clicking should always select them
                _selected = true;
                OnClick?.Invoke();
            }

            // Handle tooltip if present
            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        /// <summary>
        /// Creates a Unity Rect from the radio button's position and size.
        /// </summary>
        /// <returns>A Rect representing the radio button's bounds</returns>
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