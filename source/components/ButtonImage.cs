using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A wrapper around RimWorld's image button functionality that provides a user-friendly API
    /// for creating interactive image button controls in the UI.
    /// </summary>
    public class ButtonImage : UIElement
    {
        private Texture2D _texture;
        private string _tooltip;
        private bool _disabled;
        private StrongBox<bool> _clickedBox;

        /// <summary>
        /// Gets or sets the texture to display on the button.
        /// </summary>
        public Texture2D Texture
        {
            get => _texture;
            set => _texture = value;
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
        /// Gets or sets the StrongBox that holds the clicked state of the button.
        /// </summary>
        public StrongBox<bool> ClickedBox
        {
            get => _clickedBox;
            set => _clickedBox = value ?? new StrongBox<bool>(false);
        }

        /// <summary>
        /// Gets or sets whether the button has been clicked.
        /// This is a convenience property that accesses the Value of the ClickedBox.
        /// </summary>
        public bool Clicked
        {
            get => _clickedBox?.Value ?? false;
            set
            {
                if (_clickedBox != null)
                {
                    _clickedBox.Value = value;
                }
            }
        }

        /// <summary>
        /// Creates a new image button with content-based sizing.
        /// </summary>
        /// <param name="texture">The texture to display on the button</param>
        /// <param name="clicked">StrongBox to track the clicked state of the button</param>
        /// <param name="options">Optional UI element options</param>
        public ButtonImage(Texture2D texture, StrongBox<bool> clicked, UIElementOptions options = null) : base(SizeMode.Content, SizeMode.Content, options)
        {
            _texture = texture;
            _clickedBox = clicked ?? throw new ArgumentNullException(nameof(clicked));
            _disabled = false;
        }

        /// <summary>
        /// Creates a new image button with fixed dimensions.
        /// </summary>
        /// <param name="texture">The texture to display on the button</param>
        /// <param name="width">Fixed width of the button</param>
        /// <param name="height">Fixed height of the button</param>
        /// <param name="clicked">StrongBox to track the clicked state of the button</param>
        /// <param name="options">Optional UI element options</param>
        public ButtonImage(Texture2D texture, float width, float height, StrongBox<bool> clicked, UIElementOptions options = null) : base(width, height, options)
        {
            _texture = texture;
            _clickedBox = clicked ?? throw new ArgumentNullException(nameof(clicked));
            _disabled = false;
        }

        /// <summary>
        /// Calculates the dynamic size of the button based on its texture dimensions.
        /// </summary>
        /// <returns>A tuple containing the calculated width and height</returns>
        public override void CalculateDynamicSize()
        {
            // Handle null texture
            if (_texture == null)
            {
                Width = 32f;
                Height = 32f;
            }

            Width = Math.Max(1f, _texture.width);
            Height = Math.Max(1f, _texture.height);
        }

        /// <summary>
        /// Renders the image button using RimWorld's UI system.
        /// </summary>
        protected override void RenderElement()
        {
            // Create rect for rendering
            var rect = CreateRect();

            // Render the image button
            bool wasClicked = Widgets.ButtonImage(rect, _texture);

            // If clicked and not disabled, set clicked state to true
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