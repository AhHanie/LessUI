using System;
using System.Runtime.CompilerServices;
using FluentAssertions;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    // DO NOT USE OR TEST THE INTERNALS OF Verse.Text, Verse.Widgets as it's Unity code which cannot be executed outside of Unity
    public class LabeledRadioButtonTests
    {
        [Fact]
        public void Constructor_ContentBased_ShouldInitializeWithCorrectValues()
        {
            var selectedBox = new StrongBox<bool>(true);
            var clickedBox = new StrongBox<bool>(false);
            string text = "Test Radio Button";

            var labeledRadioButton = new LabeledRadioButton(text, selectedBox, clickedBox);

            labeledRadioButton.WidthMode.Should().Be(SizeMode.Content);
            labeledRadioButton.HeightMode.Should().Be(SizeMode.Content);
            labeledRadioButton.Text.Should().Be(text);
            labeledRadioButton.Selected.Should().BeTrue();
            labeledRadioButton.Disabled.Should().BeFalse();
            labeledRadioButton.Spacing.Should().Be(6f);
        }

        [Fact]
        public void Constructor_FixedSize_ShouldInitializeWithCorrectValues()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            string text = "Fixed Size Radio";
            float width = 200f;
            float height = 40f;

            var labeledRadioButton = new LabeledRadioButton(text, width, height, selectedBox, clickedBox);

            labeledRadioButton.Width.Should().Be(width);
            labeledRadioButton.Height.Should().Be(height);
            labeledRadioButton.WidthMode.Should().Be(SizeMode.Fixed);
            labeledRadioButton.HeightMode.Should().Be(SizeMode.Fixed);
            labeledRadioButton.Text.Should().Be(text);
            labeledRadioButton.Selected.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WithNullText_ShouldInitializeWithEmptyString()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);

            var labeledRadioButton = new LabeledRadioButton(null, selectedBox, clickedBox);

            labeledRadioButton.Text.Should().Be("");
            labeledRadioButton.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WithOptions_ShouldApplyOptions()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };

            var labeledRadioButton = new LabeledRadioButton("Test", selectedBox, clickedBox, options);

            labeledRadioButton.Alignment.Should().Be(Align.MiddleCenter);
        }

        [Fact]
        public void Selected_Property_ShouldGetAndSetCorrectly()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var labeledRadioButton = new LabeledRadioButton("Test", selectedBox, clickedBox);

            labeledRadioButton.Selected = true;

            labeledRadioButton.Selected.Should().BeTrue();
            selectedBox.Value.Should().BeTrue();
        }

        [Fact]
        public void Disabled_Property_ShouldGetAndSetCorrectly()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var labeledRadioButton = new LabeledRadioButton("Test", selectedBox, clickedBox);

            labeledRadioButton.Disabled = true;

            labeledRadioButton.Disabled.Should().BeTrue();
        }

        [Fact]
        public void Text_Property_ShouldGetAndSetCorrectly()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var labeledRadioButton = new LabeledRadioButton("Initial", selectedBox, clickedBox);
            string newText = "Updated Text";

            labeledRadioButton.Text = newText;

            labeledRadioButton.Text.Should().Be(newText);
        }

        [Fact]
        public void Text_PropertySetWithDifferentValue_ShouldInvalidateSize()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var mockRadioButton = new MockLabeledRadioButton("Initial", selectedBox, clickedBox);

            mockRadioButton.Text = "Different Text";

            mockRadioButton.InvalidateSizeCalled.Should().BeTrue();
        }

        [Fact]
        public void Text_PropertySetWithSameValue_ShouldNotInvalidateSize()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var mockRadioButton = new MockLabeledRadioButton("Same Text", selectedBox, clickedBox);
            mockRadioButton.ResetInvalidateSizeFlag();

            mockRadioButton.Text = "Same Text";

            mockRadioButton.InvalidateSizeCalled.Should().BeFalse();
        }

        [Fact]
        public void Tooltip_Property_ShouldGetAndSetCorrectly()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var labeledRadioButton = new LabeledRadioButton("Test", selectedBox, clickedBox);
            string tooltip = "This is a tooltip";

            labeledRadioButton.Tooltip = tooltip;

            labeledRadioButton.Tooltip.Should().Be(tooltip);
        }

        [Fact]
        public void Spacing_Property_ShouldGetAndSetCorrectly()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var labeledRadioButton = new LabeledRadioButton("Test", selectedBox, clickedBox);
            float newSpacing = 10f;

            labeledRadioButton.Spacing = newSpacing;

            labeledRadioButton.Spacing.Should().Be(newSpacing);
        }

        [Fact]
        public void Spacing_PropertySetWithDifferentValue_ShouldInvalidateSize()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var mockRadioButton = new MockLabeledRadioButton("Test", selectedBox, clickedBox);

            mockRadioButton.Spacing = 12f;

            mockRadioButton.InvalidateSizeCalled.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_WithEmptyText_ShouldReturnTrue()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var labeledRadioButton = new LabeledRadioButton("", selectedBox, clickedBox);

            labeledRadioButton.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_WithWhitespaceText_ShouldReturnTrue()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var labeledRadioButton = new LabeledRadioButton("   ", selectedBox, clickedBox);

            labeledRadioButton.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_WithNullText_ShouldReturnTrue()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var labeledRadioButton = new LabeledRadioButton(null, selectedBox, clickedBox);

            labeledRadioButton.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_WithValidText_ShouldReturnFalse()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var labeledRadioButton = new LabeledRadioButton("Valid Text", selectedBox, clickedBox);

            labeledRadioButton.IsEmpty.Should().BeFalse();
        }

        [Fact]
        public void CreateRect_ShouldReturnCorrectRect()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var labeledRadioButton = new LabeledRadioButton("Test", selectedBox, clickedBox);
            labeledRadioButton.X = 15f;
            labeledRadioButton.Y = 25f;
            labeledRadioButton.Width = 120f;
            labeledRadioButton.Height = 35f;

            var rect = labeledRadioButton.CreateRect();

            rect.x.Should().Be(15f);
            rect.y.Should().Be(25f);
            rect.width.Should().Be(120f);
            rect.height.Should().Be(35f);
        }

        [Fact]
        public void Render_ShouldCallRenderElement()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var mockRadioButton = new MockLabeledRadioButton("Test", selectedBox, clickedBox);

            mockRadioButton.Render();

            mockRadioButton.RenderElementCalled.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_WithText_ShouldCallRadioButtonLabeled()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var mockRadioButton = new MockLabeledRadioButton("Test Radio", selectedBox, clickedBox);

            mockRadioButton.PublicRenderElement();

            mockRadioButton.WidgetsRadioButtonLabeledCalled.Should().BeTrue();
            mockRadioButton.RenderRadioButtonOnlyCalled.Should().BeFalse();
            mockRadioButton.RenderedText.Should().Be("Test Radio");
            mockRadioButton.RenderedSelected.Should().BeFalse();
            mockRadioButton.RenderedDisabled.Should().BeFalse();
        }

        [Fact]
        public void RenderElement_WithEmptyText_ShouldCallRenderRadioButtonOnly()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var mockRadioButton = new MockLabeledRadioButton("", selectedBox, clickedBox);

            mockRadioButton.PublicRenderElement();

            mockRadioButton.RenderRadioButtonOnlyCalled.Should().BeTrue();
            mockRadioButton.WidgetsRadioButtonLabeledCalled.Should().BeFalse();
        }

        [Fact]
        public void RenderElement_WithSelectedState_ShouldPassCorrectState()
        {
            var selectedBox = new StrongBox<bool>(true);
            var clickedBox = new StrongBox<bool>(false);
            var mockRadioButton = new MockLabeledRadioButton("Test", selectedBox, clickedBox);
            mockRadioButton.Disabled = true;

            mockRadioButton.PublicRenderElement();

            mockRadioButton.RenderedSelected.Should().BeTrue();
            mockRadioButton.RenderedDisabled.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_WithTooltip_ShouldSetTooltipRegion()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var mockRadioButton = new MockLabeledRadioButton("Test", selectedBox, clickedBox);
            mockRadioButton.Tooltip = "Test tooltip";

            mockRadioButton.PublicRenderElement();

            mockRadioButton.TooltipSet.Should().Be("Test tooltip");
        }

        [Fact]
        public void RenderElement_WithoutTooltip_ShouldNotSetTooltipRegion()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var mockRadioButton = new MockLabeledRadioButton("Test", selectedBox, clickedBox);
            mockRadioButton.Tooltip = null;

            mockRadioButton.PublicRenderElement();

            mockRadioButton.TooltipSet.Should().BeNull();
        }

        [Fact]
        public void RenderRadioButtonOnly_WithTooltip_ShouldSetTooltipRegion()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var mockRadioButton = new MockLabeledRadioButton("", selectedBox, clickedBox);
            mockRadioButton.Tooltip = "Empty text tooltip";

            mockRadioButton.PublicRenderElement();

            mockRadioButton.RenderRadioButtonOnlyCalled.Should().BeTrue();
            mockRadioButton.TooltipSet.Should().Be("Empty text tooltip");
        }

        [Fact]
        public void RenderElement_ShouldUpdateClickedBox()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var mockRadioButton = new MockLabeledRadioButton("Test", selectedBox, clickedBox);
            mockRadioButton.SimulateClick(true);

            mockRadioButton.PublicRenderElement();

            clickedBox.Value.Should().BeTrue();
        }

        [Fact]
        public void StrongBoxes_ShouldSynchronizeWithProperties()
        {
            var selectedBox = new StrongBox<bool>(true);
            var clickedBox = new StrongBox<bool>(false);
            var labeledRadioButton = new LabeledRadioButton("Test", selectedBox, clickedBox);

            labeledRadioButton.Selected.Should().BeTrue();

            labeledRadioButton.Selected = false;
            selectedBox.Value.Should().BeFalse();

            selectedBox.Value = true;
            labeledRadioButton.Selected.Should().BeTrue();
        }

        [Fact]
        public void LabeledRadioButton_ShouldRenderWithoutCrashing()
        {
            var selectedBox = new StrongBox<bool>(false);
            var clickedBox = new StrongBox<bool>(false);
            var mockRadioButton = new MockLabeledRadioButton("Test", selectedBox, clickedBox);

            mockRadioButton.Invoking(rb => rb.Render()).Should().NotThrow();
            mockRadioButton.RenderElementCalled.Should().BeTrue();
        }

        private class MockLabeledRadioButton : LabeledRadioButton
        {
            public bool RenderElementCalled { get; private set; }
            public bool WidgetsRadioButtonLabeledCalled { get; private set; }
            public bool RenderRadioButtonOnlyCalled { get; private set; }
            public bool InvalidateSizeCalled { get; private set; }
            public string RenderedText { get; private set; }
            public bool RenderedSelected { get; private set; }
            public bool RenderedDisabled { get; private set; }
            public string TooltipSet { get; private set; }
            public float MockTextWidth { get; set; } = 50f;
            public float MockLineHeight { get; set; } = 20f;
            private bool _simulatedClickResult = false;

            public MockLabeledRadioButton(string text, StrongBox<bool> selectedBox, StrongBox<bool> clickedBox, UIElementOptions options = null)
                : base(text, selectedBox, clickedBox, options) { }

            public void PublicRenderElement()
            {
                RenderElement();
            }

            public (float width, float height) PublicCalculateDynamicSize()
            {
                return CalculateDynamicSize();
            }

            public void ResetInvalidateSizeFlag()
            {
                InvalidateSizeCalled = false;
            }

            public void SimulateClick(bool clicked)
            {
                _simulatedClickResult = clicked;
            }

            protected override void RenderElement()
            {
                RenderElementCalled = true;

                if (IsEmpty)
                {
                    RenderRadioButtonOnlyCalled = true;
                    RenderedText = "";
                }
                else
                {
                    WidgetsRadioButtonLabeledCalled = true;
                    RenderedText = Text;
                }

                RenderedSelected = Selected;
                RenderedDisabled = Disabled;

                var clickedBoxField = typeof(LabeledRadioButton).GetField("_clickedBox",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var clickedBox = (StrongBox<bool>)clickedBoxField.GetValue(this);
                clickedBox.Value = _simulatedClickResult;

                if (!string.IsNullOrEmpty(Tooltip))
                {
                    TooltipSet = Tooltip;
                }
            }

            public override void InvalidateSize()
            {
                InvalidateSizeCalled = true;
                base.InvalidateSize();
            }

            protected float GetTextWidth(string text)
            {
                return string.IsNullOrEmpty(text) ? 0f : MockTextWidth;
            }

            protected float GetLineHeight()
            {
                return MockLineHeight;
            }
        }
    }
}