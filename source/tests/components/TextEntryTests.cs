using FluentAssertions;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    // DO NOT USE OR TEST THE INTERNALS OF Verse.Text, Verse.Widgets as it's Unity code which cannot be executed outside of Unity
    public class TextEntryTests
    {
        [Fact]
        public void Constructor_WithContentSize_ShouldInitializeWithCorrectValues()
        {
            string initialText = "Hello World";
            Action<string> onTextChanged = (text) => { };

            var textEntry = new TextEntry(initialText, onTextChanged);

            textEntry.Text.Should().Be(initialText);
            textEntry.OnTextChanged.Should().Be(onTextChanged);
            textEntry.WidthMode.Should().Be(SizeMode.Content);
            textEntry.HeightMode.Should().Be(SizeMode.Content);
            textEntry.LineCount.Should().Be(1);
            textEntry.MaxLength.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithFixedSize_ShouldInitializeWithCorrectValues()
        {
            string initialText = "Test";
            float width = 200f;
            float height = 30f;
            Action<string> onTextChanged = (text) => { };

            var textEntry = new TextEntry(initialText, width, height, onTextChanged);

            textEntry.Text.Should().Be(initialText);
            textEntry.Width.Should().Be(width);
            textEntry.Height.Should().Be(height);
            textEntry.OnTextChanged.Should().Be(onTextChanged);
            textEntry.WidthMode.Should().Be(SizeMode.Fixed);
            textEntry.HeightMode.Should().Be(SizeMode.Fixed);
        }

        [Fact]
        public void Constructor_WithParent_ShouldSetParentCorrectly()
        {
            var parent = new UIElement(SizeMode.Fixed, SizeMode.Fixed);
            parent.Width = 300f;
            parent.Height = 200f;
            string initialText = "Child Text Entry";
            Action<string> onTextChanged = (text) => { };

            var textEntry = new TextEntry(parent, initialText, onTextChanged);

            textEntry.Parent.Should().Be(parent);
            textEntry.Text.Should().Be(initialText);
            textEntry.OnTextChanged.Should().Be(onTextChanged);
            parent.Children.Should().Contain(textEntry);
        }

        [Fact]
        public void Text_Property_ShouldGetAndSetCorrectly()
        {
            var textEntry = new TextEntry("Initial Text");
            string newText = "Updated Text";

            textEntry.Text = newText;

            textEntry.Text.Should().Be(newText);
        }

        [Fact]
        public void LineCount_Property_ShouldGetAndSetCorrectly()
        {
            var textEntry = new TextEntry("Test");
            int newLineCount = 3;

            textEntry.LineCount = newLineCount;

            textEntry.LineCount.Should().Be(newLineCount);
        }

        [Fact]
        public void MaxLength_Property_ShouldGetAndSetCorrectly()
        {
            var textEntry = new TextEntry("Test");
            int maxLength = 50;

            textEntry.MaxLength = maxLength;

            textEntry.MaxLength.Should().Be(maxLength);
        }

        [Fact]
        public void Tooltip_Property_ShouldGetAndSetCorrectly()
        {
            var textEntry = new TextEntry("Test");
            string tooltip = "This is a tooltip";

            textEntry.Tooltip = tooltip;

            textEntry.Tooltip.Should().Be(tooltip);
        }

        [Fact]
        public void IsMultiLine_WithLineCountOne_ShouldReturnFalse()
        {
            var textEntry = new TextEntry("Test");
            textEntry.LineCount = 1;

            textEntry.IsMultiLine.Should().BeFalse();
        }

        [Fact]
        public void IsMultiLine_WithLineCountGreaterThanOne_ShouldReturnTrue()
        {
            var textEntry = new TextEntry("Test");
            textEntry.LineCount = 3;

            textEntry.IsMultiLine.Should().BeTrue();
        }

        [Fact]
        public void Render_ShouldCallRenderElement()
        {
            var textEntry = new MockTextEntry("Test");

            textEntry.Render();

            textEntry.RenderElementCalled.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_SingleLine_ShouldCallTextField()
        {
            var textEntry = new MockTextEntry("Test");
            textEntry.LineCount = 1;

            textEntry.PublicRenderElement();

            textEntry.WidgetsTextFieldCalled.Should().BeTrue();
            textEntry.WidgetsTextAreaCalled.Should().BeFalse();
            textEntry.RenderedText.Should().Be("Test");
        }

        [Fact]
        public void RenderElement_MultiLine_ShouldCallTextArea()
        {
            var textEntry = new MockTextEntry("Test");
            textEntry.LineCount = 3;

            textEntry.PublicRenderElement();

            textEntry.WidgetsTextAreaCalled.Should().BeTrue();
            textEntry.WidgetsTextFieldCalled.Should().BeFalse();
            textEntry.RenderedText.Should().Be("Test");
        }

        [Fact]
        public void RenderElement_WithTooltip_ShouldSetTooltipRegion()
        {
            var textEntry = new MockTextEntry("Test");
            textEntry.Tooltip = "Test tooltip";

            textEntry.PublicRenderElement();

            textEntry.TooltipSet.Should().Be("Test tooltip");
        }

        [Fact]
        public void RenderElement_WithoutTooltip_ShouldNotSetTooltipRegion()
        {
            var textEntry = new MockTextEntry("Test");
            textEntry.Tooltip = null;

            textEntry.PublicRenderElement();

            textEntry.TooltipSet.Should().BeNull();
        }

        [Fact]
        public void CreateRect_ShouldReturnCorrectRect()
        {
            var textEntry = new TextEntry("Test");
            textEntry.X = 10f;
            textEntry.Y = 20f;
            textEntry.Width = 100f;
            textEntry.Height = 50f;

            var rect = textEntry.CreateRect();

            rect.x.Should().Be(10f);
            rect.y.Should().Be(20f);
            rect.width.Should().Be(100f);
            rect.height.Should().Be(50f);
        }

        [Fact]
        public void OnTextChanged_ShouldBeInvokedWhenTextChanges()
        {
            bool callbackInvoked = false;
            string newText = "";
            var textEntry = new MockTextEntry("Initial");
            textEntry.OnTextChanged = (text) =>
            {
                callbackInvoked = true;
                newText = text;
            };

            textEntry.SimulateTextChange("Updated");

            callbackInvoked.Should().BeTrue();
            newText.Should().Be("Updated");
        }

        [Fact]
        public void OnTextChanged_WhenNull_ShouldNotThrow()
        {
            var textEntry = new MockTextEntry("Test");
            textEntry.OnTextChanged = null;

            textEntry.Invoking(te => te.SimulateTextChange("New")).Should().NotThrow();
        }

        [Fact]
        public void OnTextChanged_WhenTextDoesNotChange_ShouldNotBeInvoked()
        {
            bool callbackInvoked = false;
            var textEntry = new MockTextEntry("Same");
            textEntry.OnTextChanged = (text) => callbackInvoked = true;

            textEntry.SimulateTextChange("Same");

            callbackInvoked.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WithNullText_ShouldUseEmptyString()
        {
            var textEntry = new TextEntry(null);

            textEntry.Text.Should().Be("");
        }

        [Fact]
        public void Constructor_WithOptions_ShouldApplyOptions()
        {
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };

            var textEntry = new TextEntry("Test", null, options);

            textEntry.Alignment.Should().Be(Align.MiddleCenter);
        }

        [Fact]
        public void Constructor_WithFixedSizeAndOptions_ShouldApplyOptionsAndSize()
        {
            var options = new UIElementOptions { Alignment = Align.LowerRight };
            Action<string> onTextChanged = (text) => { };

            var textEntry = new TextEntry("Test", 150f, 40f, onTextChanged, options);

            textEntry.Alignment.Should().Be(Align.LowerRight);
            textEntry.Width.Should().Be(150f);
            textEntry.Height.Should().Be(40f);
            textEntry.OnTextChanged.Should().Be(onTextChanged);
        }

        [Fact]
        public void Constructor_WithParentAndOptions_ShouldApplyOptionsAndSetParent()
        {
            var parent = new UIElement(300f, 200f);
            var options = new UIElementOptions { Alignment = Align.MiddleLeft };
            Action<string> onTextChanged = (text) => { };

            var textEntry = new TextEntry(parent, "Test", onTextChanged, options);

            textEntry.Parent.Should().Be(parent);
            textEntry.Alignment.Should().Be(Align.MiddleLeft);
            textEntry.OnTextChanged.Should().Be(onTextChanged);
            parent.Children.Should().Contain(textEntry);
        }

        [Fact]
        public void CalculateDynamicSize_SingleLine_ShouldReturnSingleLineHeight()
        {
            var textEntry = new MockTextEntry("Test");
            textEntry.LineCount = 1;
            textEntry.MockLineHeight = 20f;

            var size = textEntry.CalculateDynamicSize();

            size.width.Should().Be(150f);
            size.height.Should().Be(20f);
        }

        [Fact]
        public void CalculateDynamicSize_MultiLine_ShouldReturnMultiLineHeight()
        {
            var textEntry = new MockTextEntry("Test");
            textEntry.LineCount = 3;
            textEntry.MockLineHeight = 20f;

            var size = textEntry.CalculateDynamicSize();

            size.width.Should().Be(150f);
            size.height.Should().Be(60f);
        }

        [Fact]
        public void Width_WithContentMode_ShouldCalculateCorrectly()
        {
            var textEntry = new MockTextEntry("Test Text");

            var width = textEntry.Width;

            width.Should().Be(150f);
        }

        [Fact]
        public void Height_WithContentMode_ShouldCalculateCorrectly()
        {
            var textEntry = new MockTextEntry("Test");
            textEntry.LineCount = 2;
            textEntry.MockLineHeight = 18f;

            var height = textEntry.Height;

            height.Should().Be(36f);
        }

        [Fact]
        public void Text_SettingToSameValue_ShouldNotInvalidateSize()
        {
            var textEntry = new MockTextEntry("Initial");
            var initialCalculationCount = textEntry.CalculationCount;

            textEntry.Text = "Initial";

            var width = textEntry.Width;

            textEntry.CalculationCount.Should().Be(initialCalculationCount + 1);
        }

        [Fact]
        public void Text_SettingToDifferentValue_ShouldInvalidateSize()
        {
            var textEntry = new MockTextEntry("Initial");
            var width1 = textEntry.Width;
            var calculationCount1 = textEntry.CalculationCount;

            textEntry.Text = "Changed";
            var width2 = textEntry.Width;

            textEntry.CalculationCount.Should().Be(calculationCount1 + 1);
        }

        [Fact]
        public void LineCount_SettingToDifferentValue_ShouldInvalidateSize()
        {
            var textEntry = new MockTextEntry("Test");
            var height1 = textEntry.Height;
            var calculationCount1 = textEntry.CalculationCount;

            textEntry.LineCount = 3;
            var height2 = textEntry.Height;

            textEntry.CalculationCount.Should().Be(calculationCount1 + 1);
        }

        [Fact]
        public void LineCount_SettingToZero_ShouldSetToOne()
        {
            var textEntry = new TextEntry("Test");

            textEntry.LineCount = 0;

            textEntry.LineCount.Should().Be(1);
        }

        [Fact]
        public void LineCount_SettingToNegative_ShouldSetToOne()
        {
            var textEntry = new TextEntry("Test");

            textEntry.LineCount = -5;

            textEntry.LineCount.Should().Be(1);
        }

        [Fact]
        public void Constructor_WithNullOnTextChanged_ShouldInitializeCorrectly()
        {
            var textEntry = new TextEntry("Test", null);

            textEntry.OnTextChanged.Should().BeNull();
            textEntry.Text.Should().Be("Test");
        }

        [Fact]
        public void OnTextChanged_WithMaxLength_ShouldTruncateText()
        {
            string receivedText = "";
            var textEntry = new MockTextEntry("Test");
            textEntry.MaxLength = 5;
            textEntry.OnTextChanged = (text) => receivedText = text;

            textEntry.SimulateTextChange("TooLongText");

            receivedText.Should().Be("TooLo");
            textEntry.Text.Should().Be("TooLo");
        }

        [Fact]
        public void OnTextChanged_WithoutMaxLength_ShouldNotTruncateText()
        {
            string receivedText = "";
            var textEntry = new MockTextEntry("Test");
            textEntry.MaxLength = null;
            textEntry.OnTextChanged = (text) => receivedText = text;

            textEntry.SimulateTextChange("VeryLongTextThatShouldNotBeTruncated");

            receivedText.Should().Be("VeryLongTextThatShouldNotBeTruncated");
            textEntry.Text.Should().Be("VeryLongTextThatShouldNotBeTruncated");
        }

        [Fact]
        public void OnTextChanged_WithMaxLengthZero_ShouldResultInEmptyText()
        {
            string receivedText = "not empty";
            var textEntry = new MockTextEntry("Test");
            textEntry.MaxLength = 0;
            textEntry.OnTextChanged = (text) => receivedText = text;

            textEntry.SimulateTextChange("AnyText");

            receivedText.Should().Be("");
            textEntry.Text.Should().Be("");
        }

        [Fact]
        public void Constructor_WithFillWidth_ShouldInitializeCorrectly()
        {
            string initialText = "Test";
            Action<string> onTextChanged = (text) => { };

            var textEntry = new TextEntry(initialText, SizeMode.Fill, onTextChanged);

            textEntry.Text.Should().Be(initialText);
            textEntry.OnTextChanged.Should().Be(onTextChanged);
            textEntry.WidthMode.Should().Be(SizeMode.Fill);
            textEntry.HeightMode.Should().Be(SizeMode.Content);
            textEntry.LineCount.Should().Be(1);
        }

        [Fact]
        public void Constructor_WithFillWidthAndOptions_ShouldInitializeCorrectly()
        {
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };
            var textEntry = new TextEntry("Test", SizeMode.Fill, null, options);

            textEntry.Alignment.Should().Be(Align.MiddleCenter);
            textEntry.WidthMode.Should().Be(SizeMode.Fill);
            textEntry.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Constructor_WithParentAndFillWidth_ShouldSetParentAndFillMode()
        {
            var parent = new UIElement(200f, 100f);
            var textEntry = new TextEntry(parent, "Test", SizeMode.Fill);

            textEntry.Parent.Should().Be(parent);
            textEntry.WidthMode.Should().Be(SizeMode.Fill);
            textEntry.HeightMode.Should().Be(SizeMode.Content);
            parent.Children.Should().Contain(textEntry);
        }

        [Fact]
        public void Constructor_WithParentFillWidthAndOptions_ShouldApplyAllSettings()
        {
            var parent = new UIElement(200f, 100f);
            var options = new UIElementOptions { Alignment = Align.LowerRight };
            var textEntry = new TextEntry(parent, "Test", SizeMode.Fill, null, options);

            textEntry.Parent.Should().Be(parent);
            textEntry.WidthMode.Should().Be(SizeMode.Fill);
            textEntry.Alignment.Should().Be(Align.LowerRight);
            parent.Children.Should().Contain(textEntry);
        }

        [Fact]
        public void Width_WithFillMode_ShouldFillParentWidth()
        {
            var parent = new UIElement(300f, 100f);
            var textEntry = new TextEntry("Test", SizeMode.Fill);
            parent.AddChild(textEntry);

            textEntry.Width.Should().Be(300f);
        }

        [Fact]
        public void Width_WithFillMode_WithoutParent_ShouldReturnZero()
        {
            var textEntry = new TextEntry("Test", SizeMode.Fill);

            textEntry.Width.Should().Be(0f);
        }

        [Fact]
        public void Width_WithFillMode_WhenParentWidthChanges_ShouldUpdate()
        {
            var parent = new UIElement(200f, 100f);
            var textEntry = new TextEntry("Test", SizeMode.Fill);
            parent.AddChild(textEntry);
            var initialWidth = textEntry.Width;

            parent.Width = 400f;
            var newWidth = textEntry.Width;

            initialWidth.Should().Be(200f);
            newWidth.Should().Be(400f);
        }

        [Fact]
        public void Height_WithFillMode_ShouldStillCalculateFromLineCount()
        {
            var parent = new UIElement(300f, 100f);
            var textEntry = new MockTextEntryFillMode("Test", SizeMode.Fill);
            textEntry.LineCount = 3;
            textEntry.MockLineHeight = 20f;
            parent.AddChild(textEntry);

            textEntry.Height.Should().Be(60f); // 3 * 20
        }

        [Fact]
        public void OnParentSet_WithFillMode_ShouldInvalidateSize()
        {
            var textEntry = new MockTextEntryFillMode("Test", SizeMode.Fill);
            var parent = new UIElement(250f, 150f);

            var widthBeforeParent = textEntry.Width;
            parent.AddChild(textEntry);
            var widthAfterParent = textEntry.Width;

            widthBeforeParent.Should().Be(0f);
            widthAfterParent.Should().Be(250f);
        }

        [Fact]
        public void CalculateDynamicSize_WithFillMode_ShouldReturnZeroWidthAndCalculatedHeight()
        {
            var textEntry = new MockTextEntryFillMode("Test", SizeMode.Fill);
            textEntry.LineCount = 2;
            textEntry.MockLineHeight = 18f;

            var size = textEntry.CalculateDynamicSize();

            size.width.Should().Be(0f); // Fill mode returns 0 for width calculation
            size.height.Should().Be(36f); // 2 * 18
        }

        [Fact]
        public void RenderElement_WithFillMode_ShouldUseCorrectWidth()
        {
            var parent = new UIElement(300f, 100f);
            var textEntry = new MockTextEntryFillMode("Test", SizeMode.Fill);
            parent.AddChild(textEntry);

            textEntry.PublicRenderElement();

            textEntry.RenderedWidth.Should().Be(300f);
            textEntry.RenderedHeight.Should().Be(20f); // MockLineHeight default
        }

        private class MockTextEntry : TextEntry
        {
            public bool RenderElementCalled { get; protected set; }
            public bool WidgetsTextFieldCalled { get; protected set; }
            public bool WidgetsTextAreaCalled { get; protected set; }
            public string RenderedText { get; protected set; }
            public string TooltipSet { get; protected set; }
            public float MockLineHeight { get; set; } = 20f;
            public int CalculationCount { get; protected set; }

            public MockTextEntry(string text, Action<string> onTextChanged = null) : base(text, onTextChanged) { }

            public void PublicRenderElement()
            {
                RenderElement();
            }

            public void SimulateTextChange(string newText)
            {
                string originalText = Text;

                if (MaxLength.HasValue && newText.Length > MaxLength.Value)
                {
                    newText = newText.Substring(0, MaxLength.Value);
                }

                Text = newText;

                if (newText != originalText)
                {
                    OnTextChanged?.Invoke(newText);
                }
            }

            public override (float width, float height) CalculateDynamicSize()
            {
                CalculationCount++;

                float width = 150f;
                float height = MockLineHeight * LineCount;

                return (width, height);
            }

            protected override void RenderElement()
            {
                RenderElementCalled = true;
                RenderedText = Text;

                if (IsMultiLine)
                {
                    WidgetsTextAreaCalled = true;
                }
                else
                {
                    WidgetsTextFieldCalled = true;
                }

                if (!string.IsNullOrEmpty(Tooltip))
                {
                    TooltipSet = Tooltip;
                }
            }
        }

        private class MockTextEntryFillMode : MockTextEntry
        {
            public float RenderedWidth { get; private set; }
            public float RenderedHeight { get; private set; }

            public MockTextEntryFillMode(string text, SizeMode widthMode, Action<string> onTextChanged = null)
                : base(text, onTextChanged)
            {
                WidthMode = widthMode;
            }

            public void PublicRenderElement()
            {
                RenderElement();
            }

            public override (float width, float height) CalculateDynamicSize()
            {
                CalculationCount++;

                if (WidthMode == SizeMode.Fill)
                {
                    return (0f, MockLineHeight * LineCount);
                }

                float width = 150f;
                float height = MockLineHeight * LineCount;
                return (width, height);
            }

            protected override void RenderElement()
            {
                RenderElementCalled = true;
                RenderedText = Text;

                var rect = CreateRect();
                RenderedWidth = rect.width;
                RenderedHeight = rect.height;

                if (IsMultiLine)
                {
                    WidgetsTextAreaCalled = true;
                }
                else
                {
                    WidgetsTextFieldCalled = true;
                }

                if (!string.IsNullOrEmpty(Tooltip))
                {
                    TooltipSet = Tooltip;
                }
            }
        }
    }
}