using FluentAssertions;
using Xunit;

namespace LessUI.Tests
{
    public class RowTests
    {
        [Fact]
        public void Row_DefaultConstructor_ShouldInitializeWithContentSizing()
        {
            var row = new Row();

            row.WidthMode.Should().Be(SizeMode.Content);
            row.HeightMode.Should().Be(SizeMode.Content);
            row.HorizontalSpacing.Should().Be(2f);
            row.Alignment.Should().Be(Align.UpperLeft);
            row.Children.Should().BeEmpty();
        }

        [Fact]
        public void Row_ConstructorWithOptions_ShouldApplyOptions()
        {
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };

            var row = new Row(options);

            row.Alignment.Should().Be(Align.MiddleCenter);
            row.WidthMode.Should().Be(SizeMode.Content);
            row.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Row_ConstructorWithSpacing_ShouldSetCustomSpacing()
        {
            var customSpacing = 10f;

            var row = new Row(customSpacing);

            row.HorizontalSpacing.Should().Be(customSpacing);
            row.WidthMode.Should().Be(SizeMode.Content);
            row.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Row_ConstructorWithSpacingAndOptions_ShouldApplyBoth()
        {
            var customSpacing = 15f;
            var options = new UIElementOptions { Alignment = Align.LowerRight };

            var row = new Row(customSpacing, options);

            row.HorizontalSpacing.Should().Be(customSpacing);
            row.Alignment.Should().Be(Align.LowerRight);
        }

        [Fact]
        public void Row_WithChildren_ShouldCalculateWidthFromSumOfChildrenPlusSpacing()
        {
            var row = new Row(5f);
            var child1 = new UIElement(30f, 50f);
            var child2 = new UIElement(25f, 60f);
            var child3 = new UIElement(35f, 40f);

            row.AddChild(child1);
            row.AddChild(child2);
            row.AddChild(child3);

            row.Width.Should().Be(100f);
        }

        [Fact]
        public void Row_WithChildren_ShouldCalculateHeightFromTallestChild()
        {
            var row = new Row();
            var child1 = new UIElement(30f, 50f);
            var child2 = new UIElement(25f, 80f);
            var child3 = new UIElement(35f, 60f);

            row.AddChild(child1);
            row.AddChild(child2);
            row.AddChild(child3);

            row.Height.Should().Be(80f);
        }

        [Fact]
        public void Row_WithNoChildren_ShouldReturnDefaultSize()
        {
            var row = new Row();

            row.Width.Should().Be(50f);
            row.Height.Should().Be(30f);
        }

        [Fact]
        public void Row_WithSingleChild_ShouldMatchChildDimensions()
        {
            var row = new Row();
            var child = new UIElement(75f, 45f);

            row.AddChild(child);

            row.Width.Should().Be(75f);
            row.Height.Should().Be(45f);
        }

        [Fact]
        public void Row_Render_ShouldPositionChildrenHorizontally()
        {
            var row = new Row(8f) { X = 10f, Y = 20f };
            var child1 = new UIElement(30f, 50f);
            var child2 = new UIElement(25f, 60f);
            var child3 = new UIElement(35f, 40f);

            row.AddChild(child1);
            row.AddChild(child2);
            row.AddChild(child3);

            row.Render();

            child1.X.Should().Be(10f);
            child1.Y.Should().Be(20f);

            child2.X.Should().Be(48f);
            child2.Y.Should().Be(20f);

            child3.X.Should().Be(81f);
            child3.Y.Should().Be(20f);
        }

        [Fact]
        public void Row_WithZeroSpacing_ShouldPositionChildrenTightly()
        {
            var row = new Row(0f) { X = 0f, Y = 0f };
            var child1 = new UIElement(20f, 50f);
            var child2 = new UIElement(30f, 60f);

            row.AddChild(child1);
            row.AddChild(child2);

            row.Render();

            child1.X.Should().Be(0f);
            child2.X.Should().Be(20f);
        }

        [Fact]
        public void Row_AddChild_ShouldInvalidateSize()
        {
            var row = new Row();
            var initialWidth = row.Width;
            var initialHeight = row.Height;

            var child = new UIElement(60f, 100f);
            row.AddChild(child);

            row.Width.Should().NotBe(initialWidth);
            row.Height.Should().NotBe(initialHeight);
            row.Width.Should().Be(60f);
            row.Height.Should().Be(100f);
        }

        [Fact]
        public void Row_RemoveChild_ShouldRecalculateSize()
        {
            var row = new Row();
            var child1 = new UIElement(30f, 50f);
            var child2 = new UIElement(25f, 80f);

            row.AddChild(child1);
            row.AddChild(child2);
            var heightWithBothChildren = row.Height;

            row.RemoveChild(child2);
            var heightAfterRemoval = row.Height;

            heightWithBothChildren.Should().Be(80f);
            heightAfterRemoval.Should().Be(50f);
        }

        [Fact]
        public void Row_WithChildrenOfDifferentWidths_ShouldArrangeCorrectly()
        {
            var row = new Row(3f) { X = 5f, Y = 10f };
            var child1 = new UIElement(15f, 40f);
            var child2 = new UIElement(40f, 50f);
            var child3 = new UIElement(20f, 35f);
            var child4 = new UIElement(10f, 45f);

            row.AddChild(child1);
            row.AddChild(child2);
            row.AddChild(child3);
            row.AddChild(child4);

            row.Render();

            child1.X.Should().Be(5f);
            child2.X.Should().Be(23f);
            child3.X.Should().Be(66f);
            child4.X.Should().Be(89f);

            child1.Y.Should().Be(10f);
            child2.Y.Should().Be(10f);
            child3.Y.Should().Be(10f);
            child4.Y.Should().Be(10f);
        }

        [Fact]
        public void Row_WithZeroWidthChild_ShouldHandleCorrectly()
        {
            var row = new Row(4f);
            var child1 = new UIElement(20f, 50f);
            var child2 = new UIElement(0f, 60f);
            var child3 = new UIElement(15f, 40f);

            row.AddChild(child1);
            row.AddChild(child2);
            row.AddChild(child3);

            row.Render();

            child1.X.Should().Be(0f);
            child2.X.Should().Be(24f);
            child3.X.Should().Be(28f);

            row.Width.Should().Be(43f);
        }

        [Fact]
        public void Row_NestedRows_ShouldWorkCorrectly()
        {
            var outerRow = new Row(5f);
            var innerRow = new Row(2f);
            var child1 = new UIElement(20f, 40f);
            var child2 = new UIElement(15f, 50f);
            var child3 = new UIElement(25f, 60f);

            innerRow.AddChild(child1);
            innerRow.AddChild(child2);
            outerRow.AddChild(innerRow);
            outerRow.AddChild(child3);

            outerRow.Render();

            innerRow.Width.Should().Be(37f);
            innerRow.Height.Should().Be(50f);

            outerRow.Width.Should().Be(67f);
            outerRow.Height.Should().Be(60f);
        }

        [Fact]
        public void Row_ShouldUseHorizontalLayoutNotVertical()
        {
            var row = new Row();
            var child1 = new UIElement(30f, 50f);
            var child2 = new UIElement(25f, 60f);

            row.AddChild(child1);
            row.AddChild(child2);
            row.Render();

            child1.X.Should().Be(0f);
            child1.Y.Should().Be(0f);
            child2.X.Should().Be(32f);
            child2.Y.Should().Be(0f);
        }

        [Fact]
        public void HorizontalSpacing_Property_ShouldGetAndSetCorrectly()
        {
            var row = new Row();
            var newSpacing = 12f;

            row.HorizontalSpacing = newSpacing;

            row.HorizontalSpacing.Should().Be(newSpacing);
        }

        [Fact]
        public void HorizontalSpacing_DefaultValue_ShouldBe2()
        {
            var row = new Row();

            row.HorizontalSpacing.Should().Be(2f);
        }

        [Fact]
        public void Row_WithLargeSpacing_ShouldCalculateWidthCorrectly()
        {
            var row = new Row(20f);
            var child1 = new UIElement(10f, 30f);
            var child2 = new UIElement(15f, 25f);
            var child3 = new UIElement(12f, 40f);

            row.AddChild(child1);
            row.AddChild(child2);
            row.AddChild(child3);

            row.Width.Should().Be(77f);
        }

        [Fact]
        public void Row_WithNegativeSpacing_ShouldHandleCorrectly()
        {
            var row = new Row(-5f);
            var child1 = new UIElement(20f, 30f);
            var child2 = new UIElement(25f, 35f);

            row.AddChild(child1);
            row.AddChild(child2);

            row.Render();

            child1.X.Should().Be(0f);
            child2.X.Should().Be(15f);

            row.Width.Should().Be(40f);
        }

        [Fact]
        public void Row_ChangingSpacing_ShouldRecalculateSize()
        {
            var row = new Row(2f);
            var child1 = new UIElement(20f, 30f);
            var child2 = new UIElement(15f, 25f);

            row.AddChild(child1);
            row.AddChild(child2);

            var initialWidth = row.Width;

            row.HorizontalSpacing = 10f;

            var newWidth = row.Width;

            initialWidth.Should().Be(37f);
            newWidth.Should().Be(45f);
        }

        [Fact]
        public void Row_WithIdenticalChildren_ShouldSpaceEvenly()
        {
            var row = new Row(5f) { X = 10f, Y = 20f };
            var child1 = new UIElement(25f, 30f);
            var child2 = new UIElement(25f, 30f);
            var child3 = new UIElement(25f, 30f);

            row.AddChild(child1);
            row.AddChild(child2);
            row.AddChild(child3);

            row.Render();

            child1.X.Should().Be(10f);
            child2.X.Should().Be(40f);
            child3.X.Should().Be(70f);

            row.Width.Should().Be(85f);
            row.Height.Should().Be(30f);
        }

        [Fact]
        public void Row_WithVaryingHeights_ShouldAlignToSameY()
        {
            var row = new Row() { X = 5f, Y = 15f };
            var child1 = new UIElement(20f, 10f);
            var child2 = new UIElement(15f, 50f);
            var child3 = new UIElement(30f, 25f);

            row.AddChild(child1);
            row.AddChild(child2);
            row.AddChild(child3);

            row.Render();

            child1.Y.Should().Be(15f);
            child2.Y.Should().Be(15f);
            child3.Y.Should().Be(15f);

            row.Height.Should().Be(50f);
        }

        [Fact]
        public void Row_MixedWithStack_ShouldWorkCorrectly()
        {
            var stack = new Stack(8f);
            var row1 = new Row(5f);
            var row2 = new Row(3f);

            var child1 = new UIElement(20f, 25f);
            var child2 = new UIElement(15f, 20f);
            var child3 = new UIElement(30f, 30f);
            var child4 = new UIElement(25f, 35f);

            row1.AddChild(child1);
            row1.AddChild(child2);
            row2.AddChild(child3);
            row2.AddChild(child4);
            stack.AddChild(row1);
            stack.AddChild(row2);

            stack.Render();

            row1.Width.Should().Be(40f);
            row1.Height.Should().Be(25f);
            row2.Width.Should().Be(58f);
            row2.Height.Should().Be(35f);
            stack.Width.Should().Be(58f);
            stack.Height.Should().Be(68f);
        }
    }
}