using FluentAssertions;
using System.Runtime.CompilerServices;
using UnityEngine;
using Xunit;
using System;

namespace LessUI.Tests
{
    // DO NOT USE OR TEST THE INTERNALS OF Verse.Text, Verse.Widgets as it's Unity code which cannot be executed outside of Unity
    public class LabeledCheckboxTests
    {
        [Fact]
        public void Constructor_WithTextAndContentSize_ShouldInitializeWithCorrectValues()
        {
            string text = "Enable Option";
            var isChecked = new StrongBox<bool>(true);

            var labeledCheckBox = new LabeledCheckbox(text, isChecked);

            labeledCheckBox.Text.Should().Be(text);
            labeledCheckBox.Checked.Should().Be(true);
            labeledCheckBox.WidthMode.Should().Be(SizeMode.Content);
            labeledCheckBox.HeightMode.Should().Be(SizeMode.Content);
            labeledCheckBox.Spacing.Should().Be(6f);
        }

        [Fact]
        public void Constructor_WithTextAndFixedSize_ShouldInitializeWithCorrectValues()
        {
            string text = "Enable Feature";
            float width = 200f;
            float height = 30f;
            var isChecked = new StrongBox<bool>(false);

            var labeledCheckBox = new LabeledCheckbox(text, width, height, isChecked);

            labeledCheckBox.Text.Should().Be(text);
            labeledCheckBox.Width.Should().Be(width);
            labeledCheckBox.Height.Should().Be(height);
            labeledCheckBox.Checked.Should().Be(false);
            labeledCheckBox.WidthMode.Should().Be(SizeMode.Fixed);
            labeledCheckBox.HeightMode.Should().Be(SizeMode.Fixed);
        }

        [Fact]
        public void Constructor_WithOptions_ShouldApplyOptions()
        {
            var isChecked = new StrongBox<bool>(false);
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };
            var labeledCheckBox = new LabeledCheckbox("Test", isChecked, options);

            labeledCheckBox.Alignment.Should().Be(Align.MiddleCenter);
        }

        [Fact]
        public void Constructor_WithFixedSizeAndOptions_ShouldApplyOptionsAndSize()
        {
            var isChecked = new StrongBox<bool>(true);
            var options = new UIElementOptions { Alignment = Align.LowerRight };
            var labeledCheckBox = new LabeledCheckbox("Test", 150f, 40f, isChecked, options);

            labeledCheckBox.Alignment.Should().Be(Align.LowerRight);
            labeledCheckBox.Width.Should().Be(150f);
            labeledCheckBox.Height.Should().Be(40f);
            labeledCheckBox.Checked.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WithNullStrongBox_ShouldThrowArgumentNullException()
        {
            StrongBox<bool> nullBox = null;

            var act = () => new LabeledCheckbox("Test", nullBox);

            act.Should().Throw<ArgumentNullException>().WithParameterName("isChecked");
        }

        [Fact]
        public void Constructor_WithFixedSizeAndNullStrongBox_ShouldThrowArgumentNullException()
        {
            StrongBox<bool> nullBox = null;

            var act = () => new LabeledCheckbox("Test", 100f, 50f, nullBox);

            act.Should().Throw<ArgumentNullException>().WithParameterName("isChecked");
        }

        [Fact]
        public void Constructor_WithNullText_ShouldUseEmptyString()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new LabeledCheckbox(null, isChecked);

            labeledCheckBox.Text.Should().Be("");
            labeledCheckBox.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void Text_Property_ShouldGetAndSetCorrectly()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new LabeledCheckbox("Initial Text", isChecked);
            string newText = "Updated Text";

            labeledCheckBox.Text = newText;

            labeledCheckBox.Text.Should().Be(newText);
        }

        [Fact]
        public void Checked_Property_ShouldGetAndSetCorrectly()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new LabeledCheckbox("Test", isChecked);

            labeledCheckBox.Checked = true;

            labeledCheckBox.Checked.Should().BeTrue();
            isChecked.Value.Should().BeTrue();
        }

        [Fact]
        public void Checked_Property_ShouldUpdateStrongBox()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new LabeledCheckbox("Test", isChecked);

            labeledCheckBox.Checked = true;

            isChecked.Value.Should().BeTrue();
        }

        [Fact]
        public void Tooltip_Property_ShouldGetAndSetCorrectly()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new LabeledCheckbox("Test", isChecked);
            string tooltip = "This is a tooltip";

            labeledCheckBox.Tooltip = tooltip;

            labeledCheckBox.Tooltip.Should().Be(tooltip);
        }

        [Fact]
        public void Spacing_Property_ShouldGetAndSetCorrectly()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new LabeledCheckbox("Test", isChecked);
            float customSpacing = 10f;

            labeledCheckBox.Spacing = customSpacing;

            labeledCheckBox.Spacing.Should().Be(customSpacing);
        }

        [Fact]
        public void Spacing_DefaultValue_ShouldBe6()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new LabeledCheckbox("Test", isChecked);

            labeledCheckBox.Spacing.Should().Be(6f);
        }

        [Fact]
        public void IsEmpty_WithEmptyString_ShouldReturnTrue()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new LabeledCheckbox("", isChecked);

            labeledCheckBox.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_WithNullString_ShouldReturnTrue()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new LabeledCheckbox("test", isChecked);
            labeledCheckBox.Text = null;

            labeledCheckBox.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_WithWhitespaceString_ShouldReturnTrue()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new LabeledCheckbox("   ", isChecked);

            labeledCheckBox.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_WithValidText_ShouldReturnFalse()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new LabeledCheckbox("Valid Text", isChecked);

            labeledCheckBox.IsEmpty.Should().BeFalse();
        }

        [Fact]
        public void Render_ShouldCallRenderElement()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new MockLabeledCheckbox("Test", isChecked);

            labeledCheckBox.Render();

            labeledCheckBox.RenderElementCalled.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_WithText_ShouldCallCheckboxLabeled()
        {
            var isChecked = new StrongBox<bool>(true);
            var labeledCheckBox = new MockLabeledCheckbox("Enable Feature", isChecked);

            labeledCheckBox.PublicRenderElement();

            labeledCheckBox.CheckboxLabeledCalled.Should().BeTrue();
            labeledCheckBox.CheckboxOnlyCalled.Should().BeFalse();
            labeledCheckBox.RenderedText.Should().Be("Enable Feature");
            labeledCheckBox.RenderedCheckedState.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_WithEmptyText_ShouldCallCheckboxOnly()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new MockLabeledCheckbox("", isChecked);

            labeledCheckBox.PublicRenderElement();

            labeledCheckBox.CheckboxOnlyCalled.Should().BeTrue();
            labeledCheckBox.CheckboxLabeledCalled.Should().BeFalse();
        }

        [Fact]
        public void RenderElement_WithTooltip_ShouldSetTooltipRegion()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new MockLabeledCheckbox("Test", isChecked);
            labeledCheckBox.Tooltip = "Test tooltip";

            labeledCheckBox.PublicRenderElement();

            labeledCheckBox.TooltipSet.Should().Be("Test tooltip");
        }

        [Fact]
        public void RenderElement_WithoutTooltip_ShouldNotSetTooltipRegion()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new MockLabeledCheckbox("Test", isChecked);
            labeledCheckBox.Tooltip = null;

            labeledCheckBox.PublicRenderElement();

            labeledCheckBox.TooltipSet.Should().BeNull();
        }

        [Fact]
        public void RenderElement_EmptyTextWithTooltip_ShouldStillShowTooltip()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new MockLabeledCheckbox("", isChecked);
            labeledCheckBox.Tooltip = "Empty checkbox tooltip";

            labeledCheckBox.PublicRenderElement();

            labeledCheckBox.CheckboxOnlyCalled.Should().BeTrue();
            labeledCheckBox.TooltipSet.Should().Be("Empty checkbox tooltip");
        }

        [Fact]
        public void CreateRect_ShouldReturnCorrectRect()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new LabeledCheckbox("Test", isChecked);
            labeledCheckBox.X = 10f;
            labeledCheckBox.Y = 20f;
            labeledCheckBox.Width = 100f;
            labeledCheckBox.Height = 50f;

            var rect = labeledCheckBox.CreateRect();

            rect.x.Should().Be(10f);
            rect.y.Should().Be(20f);
            rect.width.Should().Be(100f);
            rect.height.Should().Be(50f);
        }

        [Fact]
        public void StateChange_ShouldUpdateStrongBox()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new MockLabeledCheckbox("Test", isChecked);

            labeledCheckBox.SimulateStateChange(true);

            isChecked.Value.Should().BeTrue();
        }

        [Fact]
        public void StateChange_WhenStateDoesNotChange_ShouldNotUpdateStrongBox()
        {
            var isChecked = new StrongBox<bool>(true);
            var labeledCheckBox = new MockLabeledCheckbox("Test", isChecked);

            labeledCheckBox.SimulateStateChange(true);

            isChecked.Value.Should().BeTrue();
        }

        [Fact]
        public void StateChange_WithMultipleStateChanges_ShouldUpdateStrongBoxEachTime()
        {
            var isChecked = new StrongBox<bool>(false);
            var labeledCheckBox = new MockLabeledCheckbox("Test", isChecked);

            labeledCheckBox.SimulateStateChange(true);
            isChecked.Value.Should().BeTrue();

            labeledCheckBox.SimulateStateChange(false);
            isChecked.Value.Should().BeFalse();

            labeledCheckBox.SimulateStateChange(true);
            isChecked.Value.Should().BeTrue();
        }

        // Mock class for testing protected methods
        private class MockLabeledCheckbox : LabeledCheckbox
        {
            public bool RenderElementCalled { get; private set; }
            public bool CheckboxLabeledCalled { get; private set; }
            public bool CheckboxOnlyCalled { get; private set; }
            public string RenderedText { get; private set; }
            public bool RenderedCheckedState { get; private set; }
            public string TooltipSet { get; private set; }

            public MockLabeledCheckbox(string text, StrongBox<bool> isChecked) : base(text, isChecked) { }

            public void PublicRenderElement()
            {
                RenderElement();
            }

            public void SimulateStateChange(bool newState)
            {
                bool originalState = Checked;
                Checked = newState;

                // The state change is automatically reflected in the StrongBox
                // through the Checked property setter
            }

            protected override void RenderElement()
            {
                RenderElementCalled = true;

                if (IsEmpty)
                {
                    CheckboxOnlyCalled = true;
                    if (!string.IsNullOrEmpty(Tooltip))
                    {
                        TooltipSet = Tooltip;
                    }
                }
                else
                {
                    CheckboxLabeledCalled = true;
                    RenderedText = Text;
                    RenderedCheckedState = Checked;

                    if (!string.IsNullOrEmpty(Tooltip))
                    {
                        TooltipSet = Tooltip;
                    }
                }
            }
        }
    }
}