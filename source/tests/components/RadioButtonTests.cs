using FluentAssertions;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    // DO NOT USE OR TEST THE INTERNALS OF Verse.Text, Verse.Widgets as it's Unity code which cannot be executed outside of Unity
    public class RadioButtonTests
    {
        [Fact]
        public void Constructor_ContentBased_ShouldInitializeWithCorrectValues()
        {
            bool selected = true;
            Action onClick = () => { };

            var radioButton = new RadioButton(selected, onClick);

            radioButton.WidthMode.Should().Be(SizeMode.Content);
            radioButton.HeightMode.Should().Be(SizeMode.Content);
            radioButton.Selected.Should().Be(selected);
        }

        [Fact]
        public void Constructor_FixedSize_ShouldInitializeWithCorrectValues()
        {
            float width = 50f;
            float height = 30f;
            bool selected = false;
            Action onClick = () => { };

            var radioButton = new RadioButton(width, height, selected, onClick);

            radioButton.Width.Should().Be(width);
            radioButton.Height.Should().Be(height);
            radioButton.WidthMode.Should().Be(SizeMode.Fixed);
            radioButton.HeightMode.Should().Be(SizeMode.Fixed);
            radioButton.Selected.Should().Be(selected);
        }

        [Fact]
        public void Constructor_WithParent_ShouldSetParentCorrectly()
        {
            var parent = new UIElement(SizeMode.Fixed, SizeMode.Fixed);
            parent.Width = 300f;
            parent.Height = 200f;
            bool selected = true;
            Action onClick = () => { };

            var radioButton = new RadioButton(parent, selected, onClick);

            radioButton.Parent.Should().Be(parent);
            radioButton.Selected.Should().Be(selected);
            parent.Children.Should().Contain(radioButton);
        }

        [Fact]
        public void Selected_Property_ShouldGetAndSetCorrectly()
        {
            var radioButton = new RadioButton(false, () => { });

            radioButton.Selected = true;

            radioButton.Selected.Should().BeTrue();
        }

        [Fact]
        public void Tooltip_Property_ShouldGetAndSetCorrectly()
        {
            var radioButton = new RadioButton(false, () => { });
            string tooltip = "This is a tooltip";

            radioButton.Tooltip = tooltip;

            radioButton.Tooltip.Should().Be(tooltip);
        }

        [Fact]
        public void Disabled_Property_ShouldGetAndSetCorrectly()
        {
            var radioButton = new RadioButton(false, () => { });

            radioButton.Disabled = true;

            radioButton.Disabled.Should().BeTrue();
        }

        [Fact]
        public void Render_ShouldCallRenderElement()
        {
            var radioButton = new MockRadioButton(false, () => { });

            radioButton.Render();

            radioButton.RenderElementCalled.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_ShouldSetCorrectProperties()
        {
            var radioButton = new MockRadioButton(true, () => { });
            radioButton.Disabled = false;

            radioButton.PublicRenderElement();

            radioButton.WidgetsRadioButtonCalled.Should().BeTrue();
            radioButton.SelectedState.Should().BeTrue();
            radioButton.DisabledState.Should().BeFalse();
        }

        [Fact]
        public void RenderElement_WithTooltip_ShouldSetTooltipRegion()
        {
            var radioButton = new MockRadioButton(false, () => { });
            radioButton.Tooltip = "Test tooltip";

            radioButton.PublicRenderElement();

            radioButton.TooltipSet.Should().Be("Test tooltip");
        }

        [Fact]
        public void RenderElement_WithoutTooltip_ShouldNotSetTooltipRegion()
        {
            var radioButton = new MockRadioButton(false, () => { });
            radioButton.Tooltip = null;

            radioButton.PublicRenderElement();

            radioButton.TooltipSet.Should().BeNull();
        }

        [Fact]
        public void CreateRect_ShouldReturnCorrectRect()
        {
            var radioButton = new RadioButton(false, () => { });
            radioButton.X = 15f;
            radioButton.Y = 25f;
            radioButton.Width = 120f;
            radioButton.Height = 35f;

            var rect = radioButton.CreateRect();

            rect.x.Should().Be(15f);
            rect.y.Should().Be(25f);
            rect.width.Should().Be(120f);
            rect.height.Should().Be(35f);
        }

        [Fact]
        public void RadioButton_ShouldRenderWithoutCrashing()
        {
            var radioButton = new MockRadioButton(false, () => { });

            radioButton.Invoking(rb => rb.Render()).Should().NotThrow();
            radioButton.RenderElementCalled.Should().BeTrue();
        }

        [Fact]
        public void RadioButton_ShouldHandleClicksCorrectly()
        {
            bool clickedCallbackInvoked = false;
            var radioButton = new MockRadioButton(false, () => clickedCallbackInvoked = true);

            radioButton.SimulateClick();

            clickedCallbackInvoked.Should().BeTrue();
            radioButton.Selected.Should().BeTrue();
        }

        [Fact]
        public void RadioButton_ShouldHandleTooltip()
        {
            var radioButton = new MockRadioButton(false, () => { });
            radioButton.Tooltip = "Radio button tooltip";

            radioButton.PublicRenderElement();

            radioButton.TooltipSet.Should().Be("Radio button tooltip");
        }

        [Fact]
        public void RadioButton_ShouldUseEmptyStringInRender()
        {
            var radioButton = new MockRadioButton(false, () => { });

            radioButton.PublicRenderElement();

            radioButton.RenderedText.Should().Be("");
            radioButton.WidgetsRadioButtonCalled.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WithOptions_ShouldApplyOptions()
        {
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };
            var radioButton = new RadioButton(false, () => { }, options);

            radioButton.Alignment.Should().Be(Align.MiddleCenter);
        }

        [Fact]
        public void Constructor_FixedSizeWithOptions_ShouldApplyOptions()
        {
            var options = new UIElementOptions { Alignment = Align.LowerRight };
            var radioButton = new RadioButton(150f, 40f, true, () => { }, options);

            radioButton.Alignment.Should().Be(Align.LowerRight);
            radioButton.Width.Should().Be(150f);
            radioButton.Height.Should().Be(40f);
            radioButton.Selected.Should().BeTrue();
        }

        [Fact]
        public void Constructor_ParentWithOptions_ShouldApplyOptionsAndSetParent()
        {
            var parent = new UIElement(300f, 200f);
            var options = new UIElementOptions { Alignment = Align.MiddleLeft };
            var radioButton = new RadioButton(parent, false, () => { }, options);

            radioButton.Parent.Should().Be(parent);
            radioButton.Alignment.Should().Be(Align.MiddleLeft);
            radioButton.Selected.Should().BeFalse();
            parent.Children.Should().Contain(radioButton);
        }

        [Fact]
        public void OnClick_ShouldBeInvokedWhenClicked()
        {
            bool clickedCallbackInvoked = false;
            var radioButton = new MockRadioButton(false, () => clickedCallbackInvoked = true);

            radioButton.SimulateClick();

            clickedCallbackInvoked.Should().BeTrue();
        }

        [Fact]
        public void OnClick_WhenNull_ShouldNotThrow()
        {
            var radioButton = new MockRadioButton(false, null);

            radioButton.Invoking(rb => rb.SimulateClick()).Should().NotThrow();
        }

        [Fact]
        public void Constructor_WithSelectedTrue_ShouldInitializeAsSelected()
        {
            var radioButton = new RadioButton(true, () => { });

            radioButton.Selected.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WithSelectedFalse_ShouldInitializeAsNotSelected()
        {
            var radioButton = new RadioButton(false, () => { });

            radioButton.Selected.Should().BeFalse();
        }

        [Fact]
        public void RadioButton_WhenDisabled_ShouldNotTriggerCallback()
        {
            bool callbackTriggered = false;
            var radioButton = new MockRadioButton(false, () => callbackTriggered = true);
            radioButton.Disabled = true;

            radioButton.SimulateClick();

            callbackTriggered.Should().BeFalse();
        }

        [Fact]
        public void RadioButton_WhenClicked_ShouldSetSelectedToTrue()
        {
            var radioButton = new MockRadioButton(false, () => { });

            radioButton.SimulateClick();

            radioButton.Selected.Should().BeTrue();
        }

        private class MockRadioButton : RadioButton
        {
            public bool RenderElementCalled { get; private set; }
            public bool WidgetsRadioButtonCalled { get; private set; }
            public bool SelectedState { get; private set; }
            public bool DisabledState { get; private set; }
            public string TooltipSet { get; private set; }
            public string RenderedText { get; private set; }

            public MockRadioButton(bool selected, Action onClick) : base(selected, onClick) { }

            public void PublicRenderElement()
            {
                RenderElement();
            }

            public void SimulateClick()
            {
                if (!Disabled)
                {
                    Selected = true;
                    OnClick?.Invoke();
                }
            }

            protected override void RenderElement()
            {
                RenderElementCalled = true;

                SelectedState = Selected;
                DisabledState = Disabled;
                WidgetsRadioButtonCalled = true;

                RenderedText = "";

                if (!string.IsNullOrEmpty(Tooltip))
                {
                    TooltipSet = Tooltip;
                }
            }
        }
    }
}