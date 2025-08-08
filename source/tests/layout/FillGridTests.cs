using FluentAssertions;
using Xunit;

namespace LessUI.Tests
{
    public class FillGridTests
    {
        [Fact]
        public void FillGrid_Constructor_ShouldInitializeWithFillWidth()
        {
            // Arrange & Act
            var fillGrid = new FillGrid(3, 2);

            // Assert
            fillGrid.Columns.Should().Be(3);
            fillGrid.Rows.Should().Be(2);
            fillGrid.WidthMode.Should().Be(SizeMode.Fill);
            fillGrid.HeightMode.Should().Be(SizeMode.Content);
            fillGrid.Alignment.Should().Be(Align.UpperLeft); // Default alignment
        }

        [Fact]
        public void FillGrid_Constructor_WithOptions_ShouldInitializeWithOptionsAndFillWidth()
        {
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };

            var fillGrid = new FillGrid(3, 2, options);

            fillGrid.Columns.Should().Be(3);
            fillGrid.Rows.Should().Be(2);
            fillGrid.WidthMode.Should().Be(SizeMode.Fill);
            fillGrid.HeightMode.Should().Be(SizeMode.Content);
            fillGrid.Alignment.Should().Be(Align.MiddleCenter);
        }

        [Fact]
        public void FillGrid_ShouldFillParentWidth()
        {
            var parent = new UIElement(200f, 100f);
            var fillGrid = new FillGrid(3, 2);

            parent.AddChild(fillGrid);

            fillGrid.Width.Should().Be(200f);
        }

        [Fact]
        public void FillGrid_ShouldCalculateCellWidthCorrectly()
        {
            var parent = new UIElement(200f, 100f);
            var fillGrid = new FillGrid(4, 2) { ColumnSpacing = 5f, Padding = 10f };

            parent.AddChild(fillGrid);

            // Available width: 200 - (2 * 10) - (3 * 5) = 200 - 20 - 15 = 165
            // Cell width: 165 / 4 = 41.25
            fillGrid.CellWidth.Should().Be(41.25f);
        }

        [Fact]
        public void FillGrid_ShouldCalculateCellHeightFromChildren()
        {
            var parent = new UIElement(200f, 100f);
            var fillGrid = new FillGrid(2, 2);
            var child1 = new UIElement(30f, 40f);
            var child2 = new UIElement(25f, 50f);

            parent.AddChild(fillGrid);
            fillGrid.AddChild(child1);
            fillGrid.AddChild(child2);

            // Cell height should be max of children: 50f
            fillGrid.CellHeight.Should().Be(50f);
        }

        [Fact]
        public void FillGrid_ShouldRecalculateWhenParentWidthChanges()
        {
            var parent = new UIElement(200f, 100f);
            var fillGrid = new FillGrid(2, 1);
            parent.AddChild(fillGrid);
            var initialCellWidth = fillGrid.CellWidth;

            parent.Width = 300f;
            fillGrid.OnParentSet();

            fillGrid.CellWidth.Should().NotBe(initialCellWidth);
            fillGrid.CellWidth.Should().Be(150f); // 300 / 2 = 150
        }

        [Fact]
        public void FillGrid_ShouldUpdateCellWidthWhenSpacingChanges()
        {
            var parent = new UIElement(200f, 100f);
            var fillGrid = new FillGrid(2, 1);
            parent.AddChild(fillGrid);
            var initialCellWidth = fillGrid.CellWidth;

            fillGrid.ColumnSpacing = 10f;

            // Available width: 200 - (0 * 2) - (1 * 10) = 190
            // Cell width: 190 / 2 = 95
            fillGrid.CellWidth.Should().Be(95f);
            fillGrid.CellWidth.Should().NotBe(initialCellWidth);
        }

        [Fact]
        public void FillGrid_ShouldUpdateCellWidthWhenPaddingChanges()
        {
            var parent = new UIElement(200f, 100f);
            var fillGrid = new FillGrid(2, 1);
            parent.AddChild(fillGrid);
            var initialCellWidth = fillGrid.CellWidth;

            fillGrid.Padding = 15f;

            // Available width: 200 - (2 * 15) - (1 * 0) = 170
            // Cell width: 170 / 2 = 85
            fillGrid.CellWidth.Should().Be(85f);
            fillGrid.CellWidth.Should().NotBe(initialCellWidth);
        }

        [Fact]
        public void FillGrid_WithoutParent_ShouldNotCrash()
        {
            var fillGrid = new FillGrid(2, 1);

            fillGrid.WidthMode.Should().Be(SizeMode.Fill);
            // Should not crash when accessing properties without parent
            fillGrid.Invoking(g => { var width = g.Width; }).Should().NotThrow();
        }

        [Fact]
        public void FillGrid_AddingChildren_ShouldRecalculateCellHeight()
        {
            var parent = new UIElement(200f, 100f);
            var fillGrid = new FillGrid(2, 1);
            parent.AddChild(fillGrid);
            var child1 = new UIElement(30f, 40f);

            fillGrid.AddChild(child1);
            var heightAfterFirstChild = fillGrid.CellHeight;

            var child2 = new UIElement(25f, 60f); // Taller child
            fillGrid.AddChild(child2);
            var heightAfterSecondChild = fillGrid.CellHeight;

            heightAfterFirstChild.Should().Be(40f);
            heightAfterSecondChild.Should().Be(60f);
        }

        [Fact]
        public void FillGrid_ShouldPositionChildrenCorrectly()
        {
            var parent = new UIElement(210f, 100f);
            var fillGrid = new FillGrid(2, 2) { X = 10f, Y = 20f, ColumnSpacing = 5f, RowSpacing = 8f, Padding = 3f };
            parent.AddChild(fillGrid);

            var child1 = new UIElement(40f, 25f);
            var child2 = new UIElement(40f, 25f);
            var child3 = new UIElement(40f, 25f);

            fillGrid.AddChild(child1);
            fillGrid.AddChild(child2);
            fillGrid.AddChild(child3);
            fillGrid.Render();

            // Assert
            // Available width: 210 - (2 * 3) - (1 * 5) = 210 - 6 - 5 = 199
            // Cell width: 199 / 2 = 99.5
            fillGrid.CellWidth.Should().Be(99.5f);

            // Child 1 at (0,0): X = 10 + 3 = 13, Y = 20 + 3 = 23
            child1.X.Should().Be(13f);
            child1.Y.Should().Be(23f);

            // Child 2 at (1,0): X = 10 + 3 + 99.5 + 5 = 117.5, Y = 20 + 3 = 23
            child2.X.Should().Be(117.5f);
            child2.Y.Should().Be(23f);

            // Child 3 at (0,1): X = 10 + 3 = 13, Y = 20 + 3 + CellHeight + 8
            child3.X.Should().Be(13f);
            // CellHeight should be 25f (max of children), so Y = 23 + 25 + 8 = 56
            child3.Y.Should().Be(56f);
        }

        [Fact]
        public void FillGrid_DefaultCellHeight_ShouldBe30()
        {
            var parent = new UIElement(200f, 100f);
            var fillGrid = new FillGrid(2, 1);

            parent.AddChild(fillGrid);

            // With no children, should use default cell height
            fillGrid.CellHeight.Should().Be(30f);
        }

        [Fact]
        public void FillGrid_UnlimitedRows_ShouldWork()
        {
            var parent = new UIElement(200f, 100f);
            var fillGrid = new FillGrid(2, 0);
            parent.AddChild(fillGrid);

            for (int i = 0; i < 5; i++)
            {
                fillGrid.AddChild(new UIElement(40f, 25f));
            }

            // Assert
            fillGrid.ActualRows.Should().Be(3); // 5 children / 2 columns = 3 rows (ceiling)
            fillGrid.CellWidth.Should().Be(100f); // 200 / 2 = 100
        }

        [Fact]
        public void FillGrid_ShouldSupportAlignment()
        {
            var parent = new UIElement(200f, 100f);
            var fillGrid = new FillGrid(2, 1);
            var child1 = new UIElement(50f, 30f) { Alignment = Align.UpperCenter };
            var child2 = new UIElement(40f, 25f) { Alignment = Align.LowerRight };

            parent.AddChild(fillGrid);
            fillGrid.AddChild(child1);
            fillGrid.AddChild(child2);
            fillGrid.Render();

            child1.X.Should().Be(25f);
            child1.Y.Should().Be(0f);

            child2.X.Should().Be(160f);
            child2.Y.Should().Be(5f);
        }
    }
}