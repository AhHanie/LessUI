using FluentAssertions;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    // DO NOT USE OR TEST THE INTERNALS OF Verse.Text, Verse.Widgets as it's Unity code which cannot be executed outside of Unity
    public class LabeledTextEntryTests
    {
        [Fact]
        public void Constructor_WithLabelAndContentSize_ShouldInitializeWithCorrectValues()
        {
            string label = "Name:";
            StrongBox<string> initialValue = new StrongBox<string>("John");

            var labeledTextEntry = new LabeledTextEntry(label, initialValue);

            labeledTextEntry.Label.Should().Be(label);
            labeledTextEntry.Value.Should().Be("John");
            labeledTextEntry.WidthMode.Should().Be(SizeMode.Content);
            labeledTextEntry.HeightMode.Should().Be(SizeMode.Content);
            labeledTextEntry.Spacing.Should().Be(6f);
            labeledTextEntry.LineCount.Should().Be(1);
            labeledTextEntry.TextLineHeightProvider.Should().BeOfType<TextLineHeightProvider>();
            labeledTextEntry.TextWidthProvider.Should().BeOfType<TextWidthProvider>();
        }

        [Fact]
        public void Constructor_WithLabelAndContentSize_WithNullStrongBox_ShouldThrowNullArgumentException()
        {
            string label = "Name:";

            var func = () => new LabeledTextEntry(label, null);

            func.Should().Throw<ArgumentNullException>().WithParameterName("value");
        }

        [Fact]
        public void Constructor_WithLabelAndFixedSize_ShouldInitializeWithCorrectValues()
        {
            string label = "Description:";
            StrongBox<string> initialValue = new StrongBox<string>("Test");
            float width = 300f;
            float height = 40f;

            var labeledTextEntry = new LabeledTextEntry(label, initialValue, width, height);

            labeledTextEntry.Label.Should().Be(label);
            labeledTextEntry.Value.Should().Be("Test");
            labeledTextEntry.Width.Should().Be(width);
            labeledTextEntry.Height.Should().Be(height);
            labeledTextEntry.WidthMode.Should().Be(SizeMode.Fixed);
            labeledTextEntry.HeightMode.Should().Be(SizeMode.Fixed);
            labeledTextEntry.TextLineHeightProvider.Should().BeOfType<TextLineHeightProvider>();
            labeledTextEntry.TextWidthProvider.Should().BeOfType<TextWidthProvider>();
        }

        [Fact]
        public void Constructor_WithLabelAndFixedSize_WithNullStrongBox_ShouldThrowNullArgumentException()
        {
            string label = "Name:";
            float width = 300f;
            float height = 40f;

            var func = () => new LabeledTextEntry(label, null, width, height);

            func.Should().Throw<ArgumentNullException>().WithParameterName("value");
        }

        [Fact]
        public void Constructor_WithNullLabel_ShouldUseEmptyString()
        {
            StrongBox<string> initialValue = new StrongBox<string>("Value");
            var labeledTextEntry = new LabeledTextEntry(null, initialValue);

            labeledTextEntry.Label.Should().Be("");
            labeledTextEntry.IsLabelEmpty.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WithSizeMode_ShouldInitializeWithCorrectValues()
        {
            string label = "Description:";
            StrongBox<string> initialValue = new StrongBox<string>("Test");

            var labeledTextEntry = new LabeledTextEntry(label, initialValue, SizeMode.Fill);

            labeledTextEntry.Label.Should().Be(label);
            labeledTextEntry.Value.Should().Be("Test");
            labeledTextEntry.WidthMode.Should().Be(SizeMode.Fill);
            labeledTextEntry.TextLineHeightProvider.Should().BeOfType<TextLineHeightProvider>();
            labeledTextEntry.TextWidthProvider.Should().BeOfType<TextWidthProvider>();
        }

        [Fact]
        public void InvalidateSizeCalled_WhenOnParentSetCalled_WithSizeModeWidthFillAndParent() 
        {
            string label = "Description:";
            StrongBox<string> initialValue = new StrongBox<string>("Test");
            UIElement elem = new UIElement(200, 200);

            var labeledTextEntry = new MockLabeledTextEntry(label, initialValue, SizeMode.Fill);
            elem.AddChild(labeledTextEntry);

            labeledTextEntry.InvalidateSizeCalls.Should().Be(1);
        }

        [Fact]
        public void CalculateDynamicSize_WithFillModeAndNoParent_ShouldReturnZeroWidthAndCalculatedHeight()
        {
            var value = new StrongBox<string>("Test");
            var labeledTextEntry = new MockLabeledTextEntry("Label:", value, SizeMode.Fill);
            labeledTextEntry.LineCount = 2;

            var result = labeledTextEntry.CalculateDynamicSize();

            result.width.Should().Be(0f);
            result.height.Should().Be(120f);
        }

        [Fact]
        public void CalculateDynamicSize_WithFillModeAndParentWithEmptyLabel_ShouldReturnParentWidthAndCalculatedHeight()
        {
            Console.WriteLine("HELLO WOLRD");
            var value = new StrongBox<string>("Test");
            var labeledTextEntry = new MockLabeledTextEntry("", value, SizeMode.Fill);
            var parent = new UIElement(300f, 200f);
            parent.AddChild(labeledTextEntry);
            labeledTextEntry.LineCount = 1;

            var result = labeledTextEntry.CalculateDynamicSize();

            result.width.Should().Be(300f);
            result.height.Should().Be(60f);
        }

        [Fact]
        public void CalculateDynamicSize_WithFillModeAndParentWithLabel_ShouldReturnRemainingWidthAfterLabelAndSpacing()
        {
            var value = new StrongBox<string>("Test");
            var labeledTextEntry = new MockLabeledTextEntry("Name:", value, SizeMode.Fill);
            var parent = new UIElement(300f, 200f);
            parent.AddChild(labeledTextEntry);
            labeledTextEntry.Spacing = 10f;

            var result = labeledTextEntry.CalculateDynamicSize();

            result.width.Should().Be(240f);
            result.height.Should().Be(60f);
        }

        [Fact]
        public void CalculateDynamicSize_WithFillModeAndInsufficientParentWidth_ShouldReturnZeroWidth()
        {
            var value = new StrongBox<string>("Test");
            var labeledTextEntry = new MockLabeledTextEntry("Very Long Label Text:", value, SizeMode.Fill);
            var parent = new UIElement(30f, 200f);
            parent.AddChild(labeledTextEntry);
            labeledTextEntry.Spacing = 10f;

            var result = labeledTextEntry.CalculateDynamicSize();

            result.width.Should().Be(0f);
            result.height.Should().Be(60f);
        }

        [Fact]
        public void CalculateDynamicSize_WithContentModeAndEmptyLabel_ShouldReturnTextEntryWidthAndHeight()
        {
            var value = new StrongBox<string>("Test");
            var labeledTextEntry = new MockLabeledTextEntry("", value, SizeMode.Content);
            labeledTextEntry.LineCount = 3;

            var result = labeledTextEntry.CalculateDynamicSize();

            result.width.Should().Be(120f);
            result.height.Should().Be(180f);
        }

        [Fact]
        public void CalculateDynamicSize_WithContentModeAndLabel_ShouldReturnLabelPlusSpacingPlusTextEntryWidth()
        {
            var value = new StrongBox<string>("Test");
            var labeledTextEntry = new MockLabeledTextEntry("Description:", value, SizeMode.Content);
            labeledTextEntry.Spacing = 8f;
            labeledTextEntry.LineCount = 1;

            var result = labeledTextEntry.CalculateDynamicSize();

            result.width.Should().Be(248f);
            result.height.Should().Be(60f);
        }

        [Fact]
        public void CalculateDynamicSize_WithLineCountSetToZero_ShouldUseMinimumLineCount()
        {
            var value = new StrongBox<string>("Test");
            var labeledTextEntry = new MockLabeledTextEntry("Label:", value, SizeMode.Content);

            labeledTextEntry.LineCount = 0;

            var result = labeledTextEntry.CalculateDynamicSize();

            labeledTextEntry.LineCount.Should().Be(1);
            result.height.Should().Be(60f);
        }

        [Fact]
        public void CalculateDynamicSize_WithNegativeLineHeight_ShouldClampHeightToMinimum()
        {
            var value = new StrongBox<string>("Test");
            var labeledTextEntry = new MockLabeledTextEntry("Label:", value, SizeMode.Content);
            labeledTextEntry.TextLineHeightProvider = new MockNegativeTextLineHeightProvider();
            labeledTextEntry.LineCount = 2;

            var result = labeledTextEntry.CalculateDynamicSize();

            result.height.Should().Be(1f);
        }

        [Fact]
        public void CalculateDynamicSize_WithZeroSpacingAndSmallComponents_ShouldStillClampToMinimum()
        {
            var value = new StrongBox<string>("Test");
            var labeledTextEntry = new MockLabeledTextEntry("", value, SizeMode.Content);
            labeledTextEntry.Spacing = 0f;

            var result = labeledTextEntry.CalculateDynamicSize();

            result.width.Should().Be(120f);
            result.height.Should().Be(60f);
        }

        [Fact]
        public void CalculateDynamicSize_WithWhitespaceOnlyLabel_ShouldTreatAsEmptyLabel()
        {
            var value = new StrongBox<string>("Test");
            var labeledTextEntry = new MockLabeledTextEntry("   \t\n  ", value, SizeMode.Content);

            var result = labeledTextEntry.CalculateDynamicSize();

            result.width.Should().Be(120f);
            labeledTextEntry.IsLabelEmpty.Should().BeTrue();
        }

        [Fact]
        public void CalculateDynamicSize_WithMultipleLineCount_ShouldScaleHeightCorrectly()
        {
            var value = new StrongBox<string>("Multi\nLine\nText");
            var labeledTextEntry = new MockLabeledTextEntry("Label:", value, SizeMode.Content);
            labeledTextEntry.LineCount = 5;

            var result = labeledTextEntry.CalculateDynamicSize();

            result.height.Should().Be(300f);
        }

        private class MockLabeledTextEntry : LabeledTextEntry
        {
            public int InvalidateSizeCalls { get; set; } = 0;
            public MockLabeledTextEntry(string label, StrongBox<string> value, SizeMode widthMode, UIElementOptions options = null)
                : base(label, value, widthMode, options)
            {
                TextLineHeightProvider = new MockTextLineHeightProvider();
                TextWidthProvider = new MockTextWidthProvider();
            }

            public MockLabeledTextEntry(string label, StrongBox<string> value, UIElementOptions options = null)
                : base(label, value, options)
            {
                TextLineHeightProvider = new MockTextLineHeightProvider();
                TextWidthProvider = new MockTextWidthProvider();
            }

            public override void InvalidateSize()
            {
                InvalidateSizeCalls++;
                base.InvalidateSize();
            }
        }

        private class MockTextLineHeightProvider : TextLineHeightProvider
        {
            public override float GetLineHeight()
            {
                return 60f;
            }
        }

        private class MockNegativeTextLineHeightProvider : TextLineHeightProvider
        {
            public override float GetLineHeight()
            {
                return -120f;
            }
        }

        private class MockTextWidthProvider : TextWidthProvider
        {
            public override float GetTextWidth(string text)
            {
                if (string.IsNullOrEmpty(text)) return 0f;

                return text.Length * 10f;
            }
        }

        private class MockZeroWidthProvider : TextWidthProvider
        {
            public override float GetTextWidth(string text)
            {
                return 0f;
            }
        }
    }
}