using System;
using System.Runtime.CompilerServices;
using FluentAssertions;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    // DO NOT USE OR TEST THE INTERNALS OF Verse.Text, Verse.Widgets as it's Unity code which cannot be executed outside of Unity
    public class LabelTests
    {
        [Fact]
        public void Constructor_WithTextAndSize_ShouldInitializeWithCorrectValues()
        {
            var textBox = new StrongBox<string>("Hello World");
            float width = 200f;
            float height = 50f;

            var label = new Label(textBox, width, height);

            label.Text.Should().Be("Hello World");
            label.Width.Should().Be(width);
            label.Height.Should().Be(height);
            label.WidthMode.Should().Be(SizeMode.Fixed);
            label.HeightMode.Should().Be(SizeMode.Fixed);
        }

        [Fact]
        public void Constructor_WithParent_ShouldSetParentCorrectly()
        {
            var parent = new UIElement(SizeMode.Fixed, SizeMode.Fixed);
            parent.Width = 300f;
            parent.Height = 200f;
            var textBox = new StrongBox<string>("Child Label");

            var label = new Label(parent, textBox);

            label.Parent.Should().Be(parent);
            label.Text.Should().Be("Child Label");
            parent.Children.Should().Contain(label);
        }

        [Fact]
        public void Constructor_WithNullTextBox_ShouldThrowArgumentNullException()
        {
            Action act = () => new Label(null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("text");
        }

        [Fact]
        public void Constructor_WithNullTextBoxAndSize_ShouldThrowArgumentNullException()
        {
            Action act = () => new Label(null, 100f, 50f);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("text");
        }

        [Fact]
        public void Constructor_WithParentAndNullTextBox_ShouldThrowArgumentNullException()
        {
            var parent = new UIElement(SizeMode.Fixed, SizeMode.Fixed);

            Action act = () => new Label(parent, null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("text");
        }

        [Fact]
        public void Text_Property_ShouldGetAndSetCorrectly()
        {
            var textBox = new StrongBox<string>("Initial Text");
            var label = new Label(textBox);
            string newText = "Updated Text";

            label.Text = newText;

            label.Text.Should().Be(newText);
            textBox.Value.Should().Be(newText); // Verify the StrongBox value is updated
        }

        [Fact]
        public void Content_Property_ShouldGetAndSetCorrectly()
        {
            var textBox = new StrongBox<string>("Initial Content");
            var label = new Label(textBox);
            string newContent = "Updated Content";

            label.Content = newContent;

            label.Content.Should().Be(newContent);
            label.Text.Should().Be(newContent); // Text should return same value as Content
            textBox.Value.Should().Be(newContent); // Verify the StrongBox value is updated
        }

        [Fact]
        public void StrongBox_ValueChange_ShouldReflectInTextProperty()
        {
            var textBox = new StrongBox<string>("Initial Text");
            var label = new Label(textBox);

            // Directly modify the StrongBox value
            textBox.Value = "Modified via StrongBox";

            label.Text.Should().Be("Modified via StrongBox");
            label.Content.Should().Be("Modified via StrongBox");
        }

        [Fact]
        public void Tooltip_Property_ShouldGetAndSetCorrectly()
        {
            var textBox = new StrongBox<string>("Test");
            var label = new Label(textBox);
            string tooltip = "This is a tooltip";

            label.Tooltip = tooltip;

            label.Tooltip.Should().Be(tooltip);
        }

        [Fact]
        public void WordWrap_Property_ShouldGetAndSetCorrectly()
        {
            var textBox = new StrongBox<string>("Test");
            var label = new Label(textBox);

            label.WordWrap = false;

            label.WordWrap.Should().BeFalse();
        }

        [Fact]
        public void MaxWidth_Property_ShouldGetAndSetCorrectly()
        {
            var textBox = new StrongBox<string>("Test");
            var label = new Label(textBox);
            float maxWidth = 150f;

            label.MaxWidth = maxWidth;

            label.MaxWidth.Should().Be(maxWidth);
        }

        [Fact]
        public void Render_ShouldCallRenderElement()
        {
            var textBox = new StrongBox<string>("Test");
            var label = new MockLabel(textBox);

            label.Render();

            label.RenderElementCalled.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_ShouldSetCorrectTextProperties()
        {
            var textBox = new StrongBox<string>("Test");
            var label = new MockLabel(textBox);
            label.WordWrap = false;

            label.PublicRenderElement();

            label.WordWrapSet.Should().BeFalse();
            label.WidgetsLabelCalled.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_WithTooltip_ShouldSetTooltipRegion()
        {
            var textBox = new StrongBox<string>("Test");
            var label = new MockLabel(textBox);
            label.Tooltip = "Test tooltip";

            label.PublicRenderElement();

            label.TooltipSet.Should().Be("Test tooltip");
        }

        [Fact]
        public void RenderElement_WithoutTooltip_ShouldNotSetTooltipRegion()
        {
            var textBox = new StrongBox<string>("Test");
            var label = new MockLabel(textBox);
            label.Tooltip = null;

            label.PublicRenderElement();

            label.TooltipSet.Should().BeNull();
        }

        [Fact]
        public void CreateRect_ShouldReturnCorrectRect()
        {
            var textBox = new StrongBox<string>("Test");
            var label = new Label(textBox);
            label.X = 10f;
            label.Y = 20f;
            label.Width = 100f;
            label.Height = 50f;

            var rect = label.CreateRect();

            rect.x.Should().Be(10f);
            rect.y.Should().Be(20f);
            rect.width.Should().Be(100f);
            rect.height.Should().Be(50f);
        }

        [Fact]
        public void IsEmpty_WithEmptyString_ShouldReturnTrue()
        {
            var textBox = new StrongBox<string>("");
            var label = new Label(textBox);

            label.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_WithNullString_ShouldReturnTrue()
        {
            var textBox = new StrongBox<string>("test");
            var label = new Label(textBox);
            label.Content = null;

            label.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_WithWhitespaceString_ShouldReturnTrue()
        {
            var textBox = new StrongBox<string>("   ");
            var label = new Label(textBox);

            label.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_WithValidText_ShouldReturnFalse()
        {
            var textBox = new StrongBox<string>("Hello");
            var label = new Label(textBox);

            label.IsEmpty.Should().BeFalse();
        }

        [Fact]
        public void IsEmpty_AfterStrongBoxValueChange_ShouldReflectCorrectly()
        {
            var textBox = new StrongBox<string>("Initial");
            var label = new Label(textBox);

            label.IsEmpty.Should().BeFalse();

            // Change via StrongBox
            textBox.Value = "";
            label.IsEmpty.Should().BeTrue();

            // Change via property
            label.Text = "Not empty";
            label.IsEmpty.Should().BeFalse();
        }

        // Mock class for testing protected methods
        private class MockLabel : Label
        {
            public bool RenderElementCalled { get; private set; }
            public bool WidgetsLabelCalled { get; private set; }
            public TextAnchor TextAnchorSet { get; private set; }
            public bool WordWrapSet { get; private set; }
            public string TooltipSet { get; private set; }

            public MockLabel(StrongBox<string> text) : base(text) { }

            public void PublicRenderElement()
            {
                RenderElement();
            }

            protected override void RenderElement()
            {
                RenderElementCalled = true;

                // Mock the RimWorld UI calls
                WordWrapSet = WordWrap;
                WidgetsLabelCalled = true;

                if (!string.IsNullOrEmpty(Tooltip))
                {
                    TooltipSet = Tooltip;
                }
            }
        }
    }
}