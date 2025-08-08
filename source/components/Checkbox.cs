using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's checkbox functionality that provides a user-friendly API
    /// for creating interactive checkbox controls in the UI.
    /// </summary>
    public class Checkbox : UIElement
    {
        private string _tooltip;
        private StrongBox<bool> _checkedBox;

        /// <summary>
        /// Gets or sets the tooltip text to display when hovering over the checkbox.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => _tooltip = value;
        }

        public bool Checked
        {
            get => _checkedBox.Value;
            set => _checkedBox.Value = value;
        }

        /// <summary>
        /// Creates a new checkbox with content-based sizing.
        /// </summary>
        /// <param name="checked">StrongBox containing the checkbox state that will be updated automatically</param>
        /// <param name="options">Optional UI element options</param>
        public Checkbox(StrongBox<bool> isChecked, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _checkedBox = isChecked ?? throw new ArgumentNullException(nameof(isChecked));
        }

        /// <summary>
        /// Creates a new checkbox with fixed dimensions.
        /// </summary>
        /// <param name="width">Fixed width of the checkbox</param>
        /// <param name="height">Fixed height of the checkbox</param>
        /// <param name="checked">StrongBox containing the checkbox state that will be updated automatically</param>
        /// <param name="options">Optional UI element options</param>
        public Checkbox(float width, float height, StrongBox<bool> isChecked, UIElementOptions options = null) : base(width, height, options)
        {
            _checkedBox = isChecked ?? throw new ArgumentNullException(nameof(isChecked));
        }

        /// <summary>
        /// Calculates the dynamic size of the checkbox.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            // Checkbox is just the checkbox square (approximately 24x24 pixels)
            Width = 24f;
            Height = 24f;
        }

        /// <summary>
        /// Renders the checkbox using RimWorld's UI system.
        /// </summary>
        protected override void RenderElement()
        {
            // Create rect for rendering
            var rect = CreateRect();

            // Store original checked state to detect changes

            // Render the checkbox and let it modify the _checked state
            Widgets.Checkbox(rect.x, rect.y, ref _checkedBox.Value);
            
            // Handle tooltip if present
            if (!string.IsNullOrEmpty(_tooltip))
            {
                TooltipHandler.TipRegion(rect, _tooltip);
            }
        }

        /// <summary>
        /// Creates a Unity Rect from the checkbox's position and size.
        /// </summary>
        /// <returns>A Rect representing the checkbox's bounds</returns>
        public Rect CreateRect()
        {
            return new Rect(X, Y, Width, Height);
        }
    }
}