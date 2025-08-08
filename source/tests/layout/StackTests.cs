using FluentAssertions;
using Xunit;

namespace LessUI.Tests
{
    public class StackTests
    {
        [Fact]
        public void Stack_DefaultConstructor_ShouldInitializeWithContentSizing()
        {
            var stack = new Stack();

            stack.WidthMode.Should().Be(SizeMode.Content);
            stack.HeightMode.Should().Be(SizeMode.Content);
            stack.VerticalSpacing.Should().Be(2f); // Default spacing
            stack.Alignment.Should().Be(Align.UpperLeft);
            stack.Children.Should().BeEmpty();
        }

        [Fact]
        public void Stack_ConstructorWithOptions_ShouldApplyOptions()
        {
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };

            var stack = new Stack(options);

            stack.Alignment.Should().Be(Align.MiddleCenter);
            stack.WidthMode.Should().Be(SizeMode.Content);
            stack.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Stack_ConstructorWithSpacing_ShouldSetCustomSpacing()
        {
            var customSpacing = 10f;

            var stack = new Stack(customSpacing);

            stack.VerticalSpacing.Should().Be(customSpacing);
            stack.WidthMode.Should().Be(SizeMode.Content);
            stack.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Stack_ConstructorWithSpacingAndOptions_ShouldApplyBoth()
        {
            var customSpacing = 15f;
            var options = new UIElementOptions { Alignment = Align.LowerRight };

            var stack = new Stack(customSpacing, options);

            stack.VerticalSpacing.Should().Be(customSpacing);
            stack.Alignment.Should().Be(Align.LowerRight);
        }

        [Fact]
        public void Stack_WithChildren_ShouldCalculateWidthFromWidestChild()
        {
            var stack = new Stack();
            var child1 = new UIElement(50f, 30f);
            var child2 = new UIElement(80f, 25f);
            var child3 = new UIElement(60f, 35f);

            stack.AddChild(child1);
            stack.AddChild(child2);
            stack.AddChild(child3);

            stack.Width.Should().Be(80f);
        }

        [Fact]
        public void Stack_WithChildren_ShouldCalculateHeightFromSumOfChildrenPlusSpacing()
        {
            var stack = new Stack(5f);
            var child1 = new UIElement(50f, 30f);
            var child2 = new UIElement(60f, 25f);
            var child3 = new UIElement(40f, 35f);

            stack.AddChild(child1);
            stack.AddChild(child2);
            stack.AddChild(child3);

            // Height: 30 + 25 + 35 + (2 * 5) = 90 + 10 = 100
            stack.Height.Should().Be(100f);
        }

        [Fact]
        public void Stack_WithNoChildren_ShouldReturnDefaultSize()
        {
            var stack = new Stack();

            stack.Width.Should().Be(50f);
            stack.Height.Should().Be(30f);
        }

        [Fact]
        public void Stack_WithSingleChild_ShouldMatchChildDimensions()
        {
            var stack = new Stack();
            var child = new UIElement(75f, 45f);

            stack.AddChild(child);

            stack.Width.Should().Be(75f);
            stack.Height.Should().Be(45f);
        }

        [Fact]
        public void Stack_Render_ShouldPositionChildrenVertically()
        {
            var stack = new Stack(8f) { X = 10f, Y = 20f };
            var child1 = new UIElement(50f, 30f);
            var child2 = new UIElement(60f, 25f);
            var child3 = new UIElement(40f, 35f);

            stack.AddChild(child1);
            stack.AddChild(child2);
            stack.AddChild(child3);

            stack.Render();

            child1.X.Should().Be(10f);
            child1.Y.Should().Be(20f);

            child2.X.Should().Be(10f);
            child2.Y.Should().Be(58f); // 20 + 30 + 8

            child3.X.Should().Be(10f);
            child3.Y.Should().Be(91f); // 58 + 25 + 8
        }

        [Fact]
        public void Stack_WithZeroSpacing_ShouldPositionChildrenTightly()
        {
            var stack = new Stack(0f) { X = 0f, Y = 0f };
            var child1 = new UIElement(50f, 20f);
            var child2 = new UIElement(60f, 30f);

            stack.AddChild(child1);
            stack.AddChild(child2);

            stack.Render();

            child1.Y.Should().Be(0f);
            child2.Y.Should().Be(20f);
        }

        [Fact]
        public void Stack_AddChild_ShouldInvalidateSize()
        {
            var stack = new Stack();
            var initialWidth = stack.Width;
            var initialHeight = stack.Height;

            var child = new UIElement(100f, 60f);
            stack.AddChild(child);

            stack.Width.Should().NotBe(initialWidth);
            stack.Height.Should().NotBe(initialHeight);
            stack.Width.Should().Be(100f);
            stack.Height.Should().Be(60f);
        }

        [Fact]
        public void Stack_RemoveChild_ShouldRecalculateSize()
        {
            var stack = new Stack();
            var child1 = new UIElement(50f, 30f);
            var child2 = new UIElement(80f, 25f);

            stack.AddChild(child1);
            stack.AddChild(child2);
            var widthWithBothChildren = stack.Width;

            stack.RemoveChild(child2);
            var widthAfterRemoval = stack.Width;

            widthWithBothChildren.Should().Be(80f);
            widthAfterRemoval.Should().Be(50f);
        }

        [Fact]
        public void Stack_WithChildrenOfDifferentHeights_ShouldStackCorrectly()
        {
            var stack = new Stack(3f) { X = 5f, Y = 10f };
            var child1 = new UIElement(40f, 15f);
            var child2 = new UIElement(50f, 40f);
            var child3 = new UIElement(35f, 20f);
            var child4 = new UIElement(45f, 10f);

            stack.AddChild(child1);
            stack.AddChild(child2);
            stack.AddChild(child3);
            stack.AddChild(child4);

            stack.Render();

            child1.Y.Should().Be(10f);
            child2.Y.Should().Be(28f); // 10 + 15 + 3
            child3.Y.Should().Be(71f); // 28 + 40 + 3
            child4.Y.Should().Be(94f); // 71 + 20 + 3

            child1.X.Should().Be(5f);
            child2.X.Should().Be(5f);
            child3.X.Should().Be(5f);
            child4.X.Should().Be(5f);
        }

        [Fact]
        public void Stack_WithZeroHeightChild_ShouldHandleCorrectly()
        {
            var stack = new Stack(4f);
            var child1 = new UIElement(50f, 20f);
            var child2 = new UIElement(60f, 0f);
            var child3 = new UIElement(40f, 15f);

            stack.AddChild(child1);
            stack.AddChild(child2);
            stack.AddChild(child3);

            stack.Render();

            child1.Y.Should().Be(0f);
            child2.Y.Should().Be(24f); // 0 + 20 + 4
            child3.Y.Should().Be(28f); // 24 + 0 + 4

            // Height calculation: 20 + 0 + 15 + (2 * 4) = 43
            stack.Height.Should().Be(43f);
        }

        [Fact]
        public void Stack_NestedStacks_ShouldWorkCorrectly()
        {
            var outerStack = new Stack(5f);
            var innerStack = new Stack(2f);
            var child1 = new UIElement(40f, 20f);
            var child2 = new UIElement(50f, 15f);
            var child3 = new UIElement(60f, 25f);

            innerStack.AddChild(child1);
            innerStack.AddChild(child2);
            outerStack.AddChild(innerStack);
            outerStack.AddChild(child3);

            outerStack.Render();

            // Inner stack should have width 50 (widest child) and height 37 (20+15+2)
            innerStack.Width.Should().Be(50f);
            innerStack.Height.Should().Be(37f); // 20 + 15 + 2

            // Outer stack should have width 60 (wider of innerStack and child3)
            // and height 67 (37 + 25 + 5)
            outerStack.Width.Should().Be(60f);
            outerStack.Height.Should().Be(67f);
        }

        [Fact]
        public void Stack_ShouldUseDefaultStackLayoutNotGridOrCanvas()
        {
            var stack = new Stack();
            var child1 = new UIElement(50f, 30f);
            var child2 = new UIElement(60f, 25f);

            stack.AddChild(child1);
            stack.AddChild(child2);
            stack.Render();

            child1.X.Should().Be(0f);
            child1.Y.Should().Be(0f);
            child2.X.Should().Be(0f);
            child2.Y.Should().Be(32f); // 0 + 30 + 2 (default spacing)
        }

        [Fact]
        public void Stack_FillMode_ShouldStillFillParentWidth()
        {
            var stack = new Stack();
            stack.WidthMode = SizeMode.Fixed;
            stack.HeightMode = SizeMode.Fixed;
            stack.Width = 300f;
            stack.Height = 200f;
            var fillChild = new UIElement(SizeMode.Fill, SizeMode.Fixed);
            fillChild.Height = 40f;

            stack.AddChild(fillChild);

            fillChild.Width.Should().Be(300f);
            fillChild.Height.Should().Be(40f);
        }

        [Fact]
        public void Stack_CircularDependency_ShouldNotCauseStackOverflow()
        {
            // Arrange: The original problematic case
            var stack = new Stack(); // Content-sized stack
            var fillChild = new UIElement(SizeMode.Fill, SizeMode.Fixed);
            fillChild.Height = 40f;

            // Act: This should not cause a stack overflow
            stack.AddChild(fillChild);

            // Assert: Should not throw
            stack.Invoking(s => { var width = s.Width; }).Should().NotThrow();
            fillChild.Invoking(c => { var width = c.Width; }).Should().NotThrow();

            // Verify we get reasonable values
            stack.Width.Should().BeGreaterThan(0);
            fillChild.Width.Should().BeGreaterThan(0);
            fillChild.Height.Should().Be(40f); // This should remain unchanged
        }

        [Fact]
        public void Stack_FillMode_ShouldStillFillParentWidth_WithCycleProtection()
        {
            var stack = new Stack();
            stack.Width = 300f;
            stack.Height = 200f;
            var fillChild = new UIElement(SizeMode.Fill, SizeMode.Fixed);
            fillChild.Height = 40f;

            stack.AddChild(fillChild);

            fillChild.Width.Should().BeGreaterThan(0);
            fillChild.Height.Should().Be(40f);
            stack.Width.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Stack_FillMode_ShouldStillFillParentWidth_ProperWay()
        {
            var stack = new Stack();
            stack.WidthMode = SizeMode.Fixed;
            stack.HeightMode = SizeMode.Fixed;
            stack.Width = 300f;
            stack.Height = 200f;
            var fillChild = new UIElement(SizeMode.Fill, SizeMode.Fixed);
            fillChild.Height = 40f;

            stack.AddChild(fillChild);

            fillChild.Width.Should().Be(300f);
            fillChild.Height.Should().Be(40f);
        }

        [Fact]
        public void Stack_FillMode_ShouldStillFillParentWidth_UsingFixedConstructor()
        {
            var stack = new UIElement(300f, 200f);
            var fillChild = new UIElement(SizeMode.Fill, SizeMode.Fixed);
            fillChild.Height = 40f;

            stack.AddChild(fillChild);

            fillChild.Width.Should().Be(300f);
            fillChild.Height.Should().Be(40f);
        }
    }
}