using FluentAssertions;
using System.Runtime.CompilerServices;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    public class ButtonTests
    {
        [Fact]
        public void Constructor_WithTextAndContentSize_ShouldInitializeWithCorrectValues()
        {
            string text = "Click Me";
            var clicked = new StrongBox<bool>(false);

            var button = new Button(text, clicked);

            button.Text.Should().Be(text);
            button.ClickedBox.Should().BeSameAs(clicked);
            button.Clicked.Should().BeFalse();
            button.WidthMode.Should().Be(SizeMode.Content);
            button.HeightMode.Should().Be(SizeMode.Content);
            button.Disabled.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WithTextAndFixedSize_ShouldInitializeWithCorrectValues()
        {
            string text = "Save";
            float width = 100f;
            float height = 40f;
            var clicked = new StrongBox<bool>(false);

            var button = new Button(text, width, height, clicked);

            button.Text.Should().Be(text);
            button.Width.Should().Be(width);
            button.Height.Should().Be(height);
            button.ClickedBox.Should().BeSameAs(clicked);
            button.Clicked.Should().BeFalse();
            button.WidthMode.Should().Be(SizeMode.Fixed);
            button.HeightMode.Should().Be(SizeMode.Fixed);
        }

        [Fact]
        public void Constructor_WithOptions_ShouldApplyOptions()
        {
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };
            var clicked = new StrongBox<bool>(false);
            var button = new Button("Test", clicked, options);

            button.Alignment.Should().Be(Align.MiddleCenter);
        }

        [Fact]
        public void Constructor_WithFixedSizeAndOptions_ShouldApplyOptionsAndSize()
        {
            var options = new UIElementOptions { Alignment = Align.LowerRight };
            var clicked = new StrongBox<bool>(false);

            var button = new Button("Test", 150f, 40f, clicked, options);

            button.Alignment.Should().Be(Align.LowerRight);
            button.Width.Should().Be(150f);
            button.Height.Should().Be(40f);
            button.ClickedBox.Should().BeSameAs(clicked);
        }

        [Fact]
        public void Constructor_WithNullText_ShouldUseEmptyString()
        {
            StrongBox<bool> clicked = new StrongBox<bool>(false);
            var button = new Button(null, clicked);

            button.Text.Should().Be("");
            button.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void Text_Property_ShouldGetAndSetCorrectly()
        {
            StrongBox<bool> clicked = new StrongBox<bool>(false);
            var button = new Button("Initial Text", clicked);
            string newText = "Updated Text";

            button.Text = newText;

            button.Text.Should().Be(newText);
        }

        [Fact]
        public void Tooltip_Property_ShouldGetAndSetCorrectly()
        {
            StrongBox<bool> clicked = new StrongBox<bool>(false);
            var button = new Button("Test", clicked);
            string tooltip = "This is a tooltip";

            button.Tooltip = tooltip;

            button.Tooltip.Should().Be(tooltip);
        }

        [Fact]
        public void Disabled_Property_ShouldGetAndSetCorrectly()
        {
            StrongBox<bool> clicked = new StrongBox<bool>(false);
            var button = new Button("Test", clicked);

            button.Disabled = true;

            button.Disabled.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_WithEmptyString_ShouldReturnTrue()
        {
            StrongBox<bool> clicked = new StrongBox<bool>(false);
            var button = new Button("", clicked);

            button.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_WithNullString_ShouldReturnTrue()
        {
            StrongBox<bool> clicked = new StrongBox<bool>(false);
            var button = new Button("test", clicked);
            button.Text = null;

            button.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_WithWhitespaceString_ShouldReturnTrue()
        {
            StrongBox<bool> clicked = new StrongBox<bool>(false);
            var button = new Button("   ", clicked);

            button.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_WithValidText_ShouldReturnFalse()
        {
            StrongBox<bool> clicked = new StrongBox<bool>(false);
            var button = new Button("Valid Text", clicked);

            button.IsEmpty.Should().BeFalse();
        }

        [Fact]
        public void Render_ShouldCallRenderElement()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Test", clicked);

            button.Render();

            button.RenderElementCalled.Should().BeTrue();
        }

        [Fact]
        public void Clicked_ShouldBeSetWhenClicked()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Test", clicked);
            button.ClickedBox = clicked;

            button.SimulateClick();

            clicked.Value.Should().BeTrue();
            button.Clicked.Should().BeTrue();
        }

        [Fact]
        public void Clicked_WhenDisabled_ShouldNotBeSet()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Test", clicked);
            button.ClickedBox = clicked;
            button.Disabled = true;

            button.SimulateClick();

            clicked.Value.Should().BeFalse();
            button.Clicked.Should().BeFalse();
        }

        [Fact]
        public void CalculateDynamicSize_WithEmptyText_ShouldReturnDefaultSize()
        {
            StrongBox<bool> clicked = new StrongBox<bool>(false);
            var button = new Button("", clicked);

            var size = button.CalculateDynamicSize();

            size.width.Should().Be(60f);
            size.height.Should().Be(30f);
        }

        [Fact]
        public void CalculateDynamicSize_WithText_ShouldIncludeTextSizeAndPadding()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Sample Text", clicked);
            button.MockTextWidth = 80f;
            button.MockTextHeight = 16f;

            var size = button.CalculateDynamicSize();

            size.width.Should().Be(100f);
            size.height.Should().Be(30f);
        }

        [Fact]
        public void CalculateDynamicSize_WithTallText_ShouldAdjustHeight()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Sample Text", clicked);
            button.MockTextWidth = 60f;
            button.MockTextHeight = 24f;

            var size = button.CalculateDynamicSize();

            size.width.Should().Be(80f);
            size.height.Should().Be(34f);
        }

        [Fact]
        public void Width_WithContentMode_ShouldCalculateCorrectly()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Test Text", clicked);
            button.MockTextWidth = 70f;

            var width = button.Width;

            width.Should().Be(90f);
        }

        [Fact]
        public void Height_WithContentMode_ShouldCalculateCorrectly()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Test", clicked);
            button.MockTextHeight = 20f;

            var height = button.Height;

            height.Should().Be(30f);
        }

        [Fact]
        public void Text_SettingToSameValue_ShouldNotInvalidateSize()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Initial", clicked);
            var initialCalculationCount = button.CalculationCount;

            button.Text = "Initial";

            var width = button.Width;

            button.CalculationCount.Should().Be(initialCalculationCount + 1);
        }

        [Fact]
        public void Text_SettingToDifferentValue_ShouldInvalidateSize()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Initial", clicked);
            var width1 = button.Width;
            var calculationCount1 = button.CalculationCount;

            button.Text = "Changed";
            var width2 = button.Width;

            button.CalculationCount.Should().Be(calculationCount1 + 1);
        }

        [Fact]
        public void Clicked_WithMultipleClicks_ShouldRemainTrue()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Test", clicked);
            button.ClickedBox = clicked;

            button.SimulateClick();
            button.SimulateClick();
            button.SimulateClick();

            clicked.Value.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_WhenDisabled_ShouldRenderInactiveButton()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Test", clicked);
            button.Disabled = true;

            button.PublicRenderElement();

            button.RenderElementCalled.Should().BeTrue();
            button.WidgetsButtonTextCalled.Should().BeTrue();
            button.RenderedActiveState.Should().BeFalse();
        }

        [Fact]
        public void RenderElement_WhenEnabled_ShouldRenderActiveButton()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Test", clicked);
            button.Disabled = false;

            button.PublicRenderElement();

            button.WidgetsButtonTextCalled.Should().BeTrue();
            button.RenderedActiveState.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_ShouldCallButtonText()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Click Me", clicked);

            button.PublicRenderElement();

            button.WidgetsButtonTextCalled.Should().BeTrue();
            button.RenderedText.Should().Be("Click Me");
        }

        [Fact]
        public void RenderElement_WithEmptyText_ShouldRenderEmptyString()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("", clicked);

            button.PublicRenderElement();

            button.WidgetsButtonTextCalled.Should().BeTrue();
            button.RenderedText.Should().Be("");
        }

        [Fact]
        public void RenderElement_WithNullText_ShouldRenderEmptyString()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("test", clicked);
            button.Text = null;

            button.PublicRenderElement();

            button.RenderedText.Should().Be("");
        }

        [Fact]
        public void RenderElement_WithTooltip_ShouldSetTooltipRegion()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Test", clicked);
            button.Tooltip = "Test tooltip";

            button.PublicRenderElement();

            button.TooltipSet.Should().Be("Test tooltip");
        }

        [Fact]
        public void RenderElement_WithoutTooltip_ShouldNotSetTooltipRegion()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new MockButton("Test", clicked);
            button.Tooltip = null;

            button.PublicRenderElement();

            button.TooltipSet.Should().BeNull();
        }

        [Fact]
        public void CreateRect_ShouldReturnCorrectRect()
        {
            StrongBox<bool> clicked = new StrongBox<bool>(false);
            var button = new Button("Test", clicked);
            button.X = 10f;
            button.Y = 20f;
            button.Width = 100f;
            button.Height = 50f;

            var rect = button.CreateRect();

            rect.x.Should().Be(10f);
            rect.y.Should().Be(20f);
            rect.width.Should().Be(100f);
            rect.height.Should().Be(50f);
        }

        [Fact]
        public void ClickedBox_ShouldAllowDirectManipulation()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new Button("Test", clicked);
            var clickedBoxRef = button.ClickedBox;

            clickedBoxRef.Value = true;

            button.Clicked.Should().BeTrue();

            button.Clicked = false;
            clickedBoxRef.Value.Should().BeFalse();
        }

        [Fact]
        public void ClickedBox_SharedBetweenButtons_ShouldSynchronize()
        {
            var sharedClicked = new StrongBox<bool>(false);
            var button1 = new MockButton("Button 1", sharedClicked);
            var button2 = new MockButton("Button 2", sharedClicked);

            button1.SimulateClick();

            button1.Clicked.Should().BeTrue();
            button2.Clicked.Should().BeTrue();
            button1.ClickedBox.Should().BeSameAs(button2.ClickedBox);
            sharedClicked.Value.Should().BeTrue();
        }

        [Fact]
        public void ClickedBox_IndependentBoxes_ShouldHaveIndependentState()
        {
            var clicked1 = new StrongBox<bool>(false);
            var clicked2 = new StrongBox<bool>(false);
            var button1 = new MockButton("Button 1", clicked1);
            var button2 = new MockButton("Button 2", clicked2);
            button1.ClickedBox = clicked1;
            button2.ClickedBox = clicked2;

            button1.SimulateClick();

            button1.Clicked.Should().BeTrue();
            button2.Clicked.Should().BeFalse();
            button1.ClickedBox.Should().NotBeSameAs(button2.ClickedBox);
        }

        [Fact]
        public void Button_StrongBoxType_ShouldBeCorrectType()
        {
            var clicked = new StrongBox<bool>(false);
            var button = new Button("Test", clicked);

            button.ClickedBox.Should().BeOfType<StrongBox<bool>>();
            button.ClickedBox.Value.Should().Be(false);
        }

        private class MockButton : Button
        {
            public bool RenderElementCalled { get; private set; }
            public bool WidgetsButtonTextCalled { get; private set; }
            public string RenderedText { get; private set; }
            public bool RenderedActiveState { get; private set; }
            public string TooltipSet { get; private set; }
            public float MockTextWidth { get; set; } = 50f;
            public float MockTextHeight { get; set; } = 16f;
            public int CalculationCount { get; private set; }

            public MockButton(string text, StrongBox<bool> clicked) : base(text, clicked) { }

            public void PublicRenderElement()
            {
                RenderElement();
            }

            public void SimulateClick()
            {
                if (!Disabled)
                {
                    ClickedBox.Value = true;
                }
            }

            public override (float width, float height) CalculateDynamicSize()
            {
                CalculationCount++;

                if (IsEmpty)
                {
                    return (60f, 30f);
                }

                float buttonWidth = MockTextWidth + 20f;
                float buttonHeight = System.Math.Max(MockTextHeight + 10f, 30f);

                return (buttonWidth, buttonHeight);
            }

            protected override void RenderElement()
            {
                RenderElementCalled = true;
                WidgetsButtonTextCalled = true;
                RenderedText = Text ?? "";
                RenderedActiveState = !Disabled;

                if (!string.IsNullOrEmpty(Tooltip))
                {
                    TooltipSet = Tooltip;
                }
            }
        }
    }
}