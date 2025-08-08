using FluentAssertions;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    public class GridTests
    {
        [Fact]
        public void Grid_WithFixedCellSize_ShouldCalculateCorrectSize()
        {
            var grid = new Grid(3, 2, 50f, 30f) { ColumnSpacing = 5f, RowSpacing = 10f, Padding = 8f };

            var width = grid.Width;
            var height = grid.Height;

            // Width: (3 * 50) + (2 * 5) + (2 * 8) = 150 + 10 + 16 = 176
            width.Should().Be(176f);
            // Height: (2 * 30) + (1 * 10) + (2 * 8) = 60 + 10 + 16 = 86
            height.Should().Be(86f);
        }

        [Fact]
        public void Grid_WithAutoSizing_ShouldSizeBasedOnChildren()
        {
            var grid = new Grid(2);
            var child1 = new UIElement(40f, 25f);
            var child2 = new UIElement(60f, 35f);
            var child3 = new UIElement(30f, 20f);

            grid.AddChild(child1);
            grid.AddChild(child2);
            grid.AddChild(child3);

            // Cell size should be max of children: 60x35
            // 2 columns, 2 rows (3 children in 2 columns = 2 rows)
            // Width: (2 * 60) + (1 * 0) + (2 * 0) = 120
            grid.Width.Should().Be(120f);
            // Height: (2 * 35) + (1 * 0) + (2 * 0) = 70
            grid.Height.Should().Be(70f);
        }

        [Fact]
        public void Grid_ShouldPositionChildrenCorrectly()
        {
            var grid = new Grid(2, 2, 50f, 30f) { X = 10f, Y = 20f, ColumnSpacing = 5f, RowSpacing = 8f, Padding = 3f };
            var child1 = new UIElement(40f, 25f);
            var child2 = new UIElement(40f, 25f);
            var child3 = new UIElement(40f, 25f);

            grid.AddChild(child1);
            grid.AddChild(child2);
            grid.AddChild(child3);
            grid.Render();

            // Child 1 at (0,0): X = 10 + 3 = 13, Y = 20 + 3 = 23
            child1.X.Should().Be(13f);
            child1.Y.Should().Be(23f);

            // Child 2 at (1,0): X = 10 + 3 + 55 = 68, Y = 20 + 3 = 23
            child2.X.Should().Be(68f); // 13 + 50 + 5
            child2.Y.Should().Be(23f);

            // Child 3 at (0,1): X = 10 + 3 = 13, Y = 20 + 3 + 38 = 61
            child3.X.Should().Be(13f);
            child3.Y.Should().Be(61f); // 23 + 30 + 8
        }

        [Fact]
        public void Grid_GetChildAt_ShouldReturnCorrectChild()
        {
            var grid = new Grid(2, 2, 50f, 30f);
            var child1 = new UIElement(40f, 25f);
            var child2 = new UIElement(40f, 25f);
            var child3 = new UIElement(40f, 25f);

            grid.AddChild(child1);
            grid.AddChild(child2);
            grid.AddChild(child3);

            grid.GetChildAt(0, 0).Should().Be(child1);
            grid.GetChildAt(1, 0).Should().Be(child2);
            grid.GetChildAt(0, 1).Should().Be(child3);
            grid.GetChildAt(1, 1).Should().BeNull();
        }

        [Fact]
        public void Grid_GetPositionOfChild_ShouldReturnCorrectPosition()
        {
            var grid = new Grid(3, 2, 50f, 30f);
            var child1 = new UIElement(40f, 25f);
            var child2 = new UIElement(40f, 25f);
            var child3 = new UIElement(40f, 25f);
            var child4 = new UIElement(40f, 25f);

            grid.AddChild(child1);
            grid.AddChild(child2);
            grid.AddChild(child3);
            grid.AddChild(child4);

            grid.GetPositionOfChild(child1).Should().Be((0, 0));
            grid.GetPositionOfChild(child2).Should().Be((1, 0));
            grid.GetPositionOfChild(child3).Should().Be((2, 0));
            grid.GetPositionOfChild(child4).Should().Be((0, 1));
        }

        [Fact]
        public void Grid_InsertChildAt_ShouldInsertAtCorrectPosition()
        {
            var grid = new Grid(2, 2, 50f, 30f);
            var child1 = new UIElement(40f, 25f);
            var child2 = new UIElement(40f, 25f);
            var newChild = new UIElement(40f, 25f);

            grid.AddChild(child1);
            grid.AddChild(child2);

            grid.InsertChildAt(newChild, 1, 0);

            grid.GetChildAt(0, 0).Should().Be(child1);
            grid.GetChildAt(1, 0).Should().Be(newChild);
            grid.GetChildAt(0, 1).Should().Be(child2);
        }

        [Fact]
        public void Grid_MaxCapacity_ShouldReturnCorrectValue()
        {
            var fixedGrid = new Grid(3, 2, 50f, 30f);
            var unlimitedGrid = new Grid(3, 0, 50f, 30f);

            fixedGrid.MaxCapacity.Should().Be(6); // 3 * 2
            unlimitedGrid.MaxCapacity.Should().Be(-1);
        }

        [Fact]
        public void Grid_AvailableCells_ShouldCalculateCorrectly()
        {
            var grid = new Grid(2, 2, 50f, 30f);
            var child1 = new UIElement(40f, 25f);
            var child2 = new UIElement(40f, 25f);

            grid.AddChild(child1);
            var available1 = grid.AvailableCells;

            grid.AddChild(child2);
            var available2 = grid.AvailableCells;

            available1.Should().Be(3); // 4 - 1
            available2.Should().Be(2); // 4 - 2
        }

        [Fact]
        public void Grid_IsFull_ShouldReturnCorrectValue()
        {
            var grid = new Grid(2, 1, 50f, 30f);
            var child1 = new UIElement(40f, 25f);
            var child2 = new UIElement(40f, 25f);

            grid.IsFull.Should().BeFalse();

            grid.AddChild(child1);
            grid.IsFull.Should().BeFalse();

            grid.AddChild(child2);
            grid.IsFull.Should().BeTrue();
        }

        [Fact]
        public void Grid_ActualRows_ShouldCalculateCorrectly()
        {
            var grid = new Grid(3);
            var child1 = new UIElement(40f, 25f);
            var child2 = new UIElement(40f, 25f);
            var child3 = new UIElement(40f, 25f);
            var child4 = new UIElement(40f, 25f);

            grid.ActualRows.Should().Be(0);

            grid.AddChild(child1);
            grid.ActualRows.Should().Be(1); // 1 child = 1 row

            grid.AddChild(child2);
            grid.AddChild(child3);
            grid.ActualRows.Should().Be(1); // 3 children in 3 columns = 1 row

            grid.AddChild(child4);
            grid.ActualRows.Should().Be(2); // 4 children in 3 columns = 2 rows
        }

        [Fact]
        public void Grid_UnlimitedRows_ShouldGrowAsNeeded()
        {
            var grid = new Grid(2, 0, 50f, 30f);

            for (int i = 0; i < 5; i++)
            {
                grid.AddChild(new UIElement(40f, 25f));
            }

            grid.ActualRows.Should().Be(3); // 5 children / 2 columns = 3 rows (ceiling)

            // Height: (3 * 30) + (2 * 0) + (2 * 0) = 90 (assuming no spacing/padding)
            grid.Height.Should().Be(90f);
        }

        [Fact]
        public void Grid_Constructor_WithColumns_ShouldInitializeCorrectly()
        {
            var grid = new Grid(3);

            grid.Columns.Should().Be(3);
            grid.Rows.Should().Be(0);
            grid.WidthMode.Should().Be(SizeMode.Content);
            grid.HeightMode.Should().Be(SizeMode.Content);
            grid.Alignment.Should().Be(Align.UpperLeft);
        }

        [Fact]
        public void Grid_Constructor_WithColumnsAndOptions_ShouldInitializeCorrectly()
        {
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };

            var grid = new Grid(3, 0, options);

            grid.Columns.Should().Be(3);
            grid.Rows.Should().Be(0);
            grid.WidthMode.Should().Be(SizeMode.Content);
            grid.HeightMode.Should().Be(SizeMode.Content);
            grid.Alignment.Should().Be(Align.MiddleCenter);
        }

        [Fact]
        public void Grid_Constructor_WithColumnsAndRows_ShouldInitializeCorrectly()
        {
            var grid = new Grid(3, 2);

            grid.Columns.Should().Be(3);
            grid.Rows.Should().Be(2);
            grid.WidthMode.Should().Be(SizeMode.Content);
            grid.HeightMode.Should().Be(SizeMode.Content);
            grid.Alignment.Should().Be(Align.UpperLeft);
        }

        [Fact]
        public void Grid_Constructor_WithColumnsRowsAndOptions_ShouldInitializeCorrectly()
        {
            var options = new UIElementOptions { Alignment = Align.LowerRight };

            var grid = new Grid(3, 2, options);

            grid.Columns.Should().Be(3);
            grid.Rows.Should().Be(2);
            grid.WidthMode.Should().Be(SizeMode.Content);
            grid.HeightMode.Should().Be(SizeMode.Content);
            grid.Alignment.Should().Be(Align.LowerRight);
        }

        [Fact]
        public void Grid_Constructor_WithFixedCellSize_ShouldInitializeCorrectly()
        {
            var grid = new Grid(3, 2, 50f, 30f);

            grid.Columns.Should().Be(3);
            grid.Rows.Should().Be(2);
            grid.CellWidth.Should().Be(50f);
            grid.CellHeight.Should().Be(30f);
            grid.WidthMode.Should().Be(SizeMode.Content);
            grid.HeightMode.Should().Be(SizeMode.Content);
            grid.Alignment.Should().Be(Align.UpperLeft);
        }

        [Fact]
        public void Grid_Constructor_WithFixedCellSizeAndOptions_ShouldInitializeCorrectly()
        {
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };

            var grid = new Grid(3, 2, 50f, 30f, options);

            grid.Columns.Should().Be(3);
            grid.Rows.Should().Be(2);
            grid.CellWidth.Should().Be(50f);
            grid.CellHeight.Should().Be(30f);
            grid.WidthMode.Should().Be(SizeMode.Content);
            grid.HeightMode.Should().Be(SizeMode.Content);
            grid.Alignment.Should().Be(Align.MiddleCenter);
        }

        [Fact]
        public void Grid_GetChildAt_WithInvalidPosition_ShouldReturnNull()
        {
            var grid = new Grid(2, 2, 50f, 30f);
            var child = new UIElement(40f, 25f);
            grid.AddChild(child);

            grid.GetChildAt(5, 5).Should().BeNull();
            grid.GetChildAt(1, 1).Should().BeNull();
        }

        [Fact]
        public void Grid_GetPositionOfChild_WithNonExistentChild_ShouldReturnNegativeOne()
        {
            var grid = new Grid(2, 2, 50f, 30f);
            var child = new UIElement(40f, 25f);
            var outsideChild = new UIElement(30f, 20f);

            grid.AddChild(child);

            grid.GetPositionOfChild(outsideChild).Should().Be((-1, -1));
        }

        [Fact]
        public void Grid_InsertChildAt_WithNullChild_ShouldNotThrow()
        {
            var grid = new Grid(2, 2, 50f, 30f);

            grid.Invoking(g => g.InsertChildAt(null, 0, 0)).Should().NotThrow();
        }

        [Fact]
        public void Grid_InsertChildAt_WithInvalidPosition_ShouldClampToValidRange()
        {
            var grid = new Grid(2, 2, 50f, 30f);
            var child = new UIElement(40f, 25f);

            grid.InsertChildAt(child, -1, -1); // Should clamp to (0, 0)

            grid.GetChildAt(0, 0).Should().Be(child);
        }

        [Fact]
        public void Grid_Render_ShouldLayoutChildrenBeforeRendering()
        {
            var grid = new Grid(2, 2, 50f, 30f);
            var child1 = new UIElement(40f, 25f);
            var child2 = new UIElement(40f, 25f);

            grid.AddChild(child1);
            grid.AddChild(child2);

            grid.Render();

            child1.X.Should().Be(0f); // Grid X + Padding + (0 * (CellWidth + ColumnSpacing))
            child1.Y.Should().Be(0f); // Grid Y + Padding + (0 * (CellHeight + RowSpacing))
            child2.X.Should().Be(50f); // Grid X + Padding + (1 * (CellWidth + ColumnSpacing))
            child2.Y.Should().Be(0f); // Grid Y + Padding + (0 * (CellHeight + RowSpacing))
        }

        [Fact]
        public void Grid_WithRowLimit_ShouldNotPositionChildrenBeyondLimit()
        {
            var grid = new Grid(2, 1, 50f, 30f);
            var child1 = new UIElement(40f, 25f);
            var child2 = new UIElement(40f, 25f);
            var child3 = new UIElement(40f, 25f); // This should be ignored for positioning

            grid.AddChild(child1);
            grid.AddChild(child2);
            grid.AddChild(child3);

            grid.Render();

            // First two children should be positioned
            child1.X.Should().Be(0f);
            child2.X.Should().Be(50f);

            // Third child should still have default position since grid won't position it
            child3.X.Should().Be(0f); // From AddChild default positioning
        }

        [Fact]
        public void Grid_WithAutoCalculateCellSize_ShouldCalculateCellSizeFromChildrenWhenAccessingSize()
        {
            var grid = new Grid(2);
            var child1 = new UIElement(60f, 40f);
            var child2 = new UIElement(80f, 30f);
            var child3 = new UIElement(50f, 45f);

            grid.AddChild(child1);
            grid.AddChild(child2);
            grid.AddChild(child3);

            var width = grid.Width;
            var height = grid.Height;

            grid.CellWidth.Should().Be(80f, "CellWidth should be set to the maximum child width");
            grid.CellHeight.Should().Be(45f, "CellHeight should be set to the maximum child height");
        }

        [Fact]
        public void Grid_WithAutoCalculateCellSize_WithNoChildren_ShouldSetDefaultCellSize()
        {
            var grid = new Grid(2);

            var width = grid.Width;
            var height = grid.Height;

            grid.CellWidth.Should().Be(50f, "CellWidth should use default value when no children");
            grid.CellHeight.Should().Be(30f, "CellHeight should use default value when no children");
        }

        [Fact]
        public void Grid_WithFixedCellSize_ShouldNotRecalculateCellSizeFromChildren()
        {
            var grid = new Grid(2, 2, 100f, 60f);
            var originalCellWidth = grid.CellWidth;
            var originalCellHeight = grid.CellHeight;

            var child1 = new UIElement(150f, 80f);
            var child2 = new UIElement(200f, 90f);

            grid.AddChild(child1);
            grid.AddChild(child2);

            var width = grid.Width;
            var height = grid.Height;

            grid.CellWidth.Should().Be(originalCellWidth, "CellWidth should not change when auto-calculation is disabled");
            grid.CellHeight.Should().Be(originalCellHeight, "CellHeight should not change when auto-calculation is disabled");
        }

        [Fact]
        public void Grid_WithAutoCalculateCellSize_ShouldRecalculateWhenChildrenChange()
        {
            var grid = new Grid(2);
            var child1 = new UIElement(50f, 30f);
            grid.AddChild(child1);

            var initialWidth = grid.Width;
            var initialCellWidth = grid.CellWidth;
            var initialCellHeight = grid.CellHeight;

            var child2 = new UIElement(100f, 60f);
            grid.AddChild(child2);

            var newWidth = grid.Width;

            grid.CellWidth.Should().Be(100f, "CellWidth should be updated to the new maximum child width");
            grid.CellHeight.Should().Be(60f, "CellHeight should be updated to the new maximum child height");
            grid.CellWidth.Should().NotBe(initialCellWidth, "CellWidth should have changed from initial value");
            grid.CellHeight.Should().NotBe(initialCellHeight, "CellHeight should have changed from initial value");
        }

        [Fact]
        public void Grid_WithAutoCalculateCellSize_ShouldRecalculateWhenChildSizeChanges()
        {
            var grid = new Grid(2);
            var child1 = new UIElement(50f, 30f);
            var child2 = new UIElement(60f, 35f);

            grid.AddChild(child1);
            grid.AddChild(child2);

            var initialWidth = grid.Width;
            var initialCellWidth = grid.CellWidth;
            var initialCellHeight = grid.CellHeight;

            child1.Width = 120f;
            child1.Height = 80f;

            grid.InvalidateSize();
            var newWidth = grid.Width;

            grid.CellWidth.Should().Be(120f, "CellWidth should be updated to reflect the changed child width");
            grid.CellHeight.Should().Be(80f, "CellHeight should be updated to reflect the changed child height");
        }

        [Fact]
        public void Grid_WithAutoCalculateCellSize_MultipleChildrenSameDimensions_ShouldUseCommonSize()
        {
            var grid = new Grid(3);
            var child1 = new UIElement(75f, 45f);
            var child2 = new UIElement(75f, 45f);
            var child3 = new UIElement(75f, 45f);

            grid.AddChild(child1);
            grid.AddChild(child2);
            grid.AddChild(child3);

            var width = grid.Width;

            grid.CellWidth.Should().Be(75f, "CellWidth should be the common width when all children have same dimensions");
            grid.CellHeight.Should().Be(45f, "CellHeight should be the common height when all children have same dimensions");
        }

        [Fact]
        public void Grid_WithAutoCalculateCellSize_SingleChild_ShouldMatchChildSize()
        {
            var grid = new Grid(1);
            var child = new UIElement(95f, 65f);

            grid.AddChild(child);

            var width = grid.Width;

            grid.CellWidth.Should().Be(95f, "CellWidth should match the single child's width");
            grid.CellHeight.Should().Be(65f, "CellHeight should match the single child's height");
        }

        [Fact]
        public void Grid_ChildWithUpperLeftAlignment_ShouldPositionAtTopLeftOfCell()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(50f, 30f) { Alignment = Align.UpperLeft };

            grid.AddChild(child);
            grid.Render();

            child.X.Should().Be(10f);
            child.Y.Should().Be(20f);
        }

        [Fact]
        public void Grid_ChildWithUpperCenterAlignment_ShouldPositionAtTopCenterOfCell()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(50f, 30f) { Alignment = Align.UpperCenter };

            grid.AddChild(child);
            grid.Render();

            child.X.Should().Be(35f);
            child.Y.Should().Be(20f);
        }

        [Fact]
        public void Grid_ChildWithUpperRightAlignment_ShouldPositionAtTopRightOfCell()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(50f, 30f) { Alignment = Align.UpperRight };

            grid.AddChild(child);
            grid.Render();

            child.X.Should().Be(60f);
            child.Y.Should().Be(20f);
        }

        [Fact]
        public void Grid_ChildWithMiddleLeftAlignment_ShouldPositionAtMiddleLeftOfCell()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(50f, 30f) { Alignment = Align.MiddleLeft };

            grid.AddChild(child);
            grid.Render();

            child.X.Should().Be(10f);
            child.Y.Should().Be(35f);
        }

        [Fact]
        public void Grid_ChildWithMiddleCenterAlignment_ShouldPositionAtCenterOfCell()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(50f, 30f) { Alignment = Align.MiddleCenter };

            grid.AddChild(child);
            grid.Render();

            child.X.Should().Be(35f);
            child.Y.Should().Be(35f);
        }

        [Fact]
        public void Grid_ChildWithMiddleRightAlignment_ShouldPositionAtMiddleRightOfCell()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(50f, 30f) { Alignment = Align.MiddleRight };

            grid.AddChild(child);
            grid.Render();

            child.X.Should().Be(60f);
            child.Y.Should().Be(35f);
        }

        [Fact]
        public void Grid_ChildWithLowerLeftAlignment_ShouldPositionAtBottomLeftOfCell()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(50f, 30f) { Alignment = Align.LowerLeft };

            grid.AddChild(child);
            grid.Render();

            child.X.Should().Be(10f);
            child.Y.Should().Be(50f);
        }

        [Fact]
        public void Grid_ChildWithLowerCenterAlignment_ShouldPositionAtBottomCenterOfCell()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(50f, 30f) { Alignment = Align.LowerCenter };

            grid.AddChild(child);
            grid.Render();

            child.X.Should().Be(35f);
            child.Y.Should().Be(50f);
        }

        [Fact]
        public void Grid_ChildWithLowerRightAlignment_ShouldPositionAtBottomRightOfCell()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(50f, 30f) { Alignment = Align.LowerRight };

            grid.AddChild(child);
            grid.Render();

            child.X.Should().Be(60f);
            child.Y.Should().Be(50f);
        }

        [Fact]
        public void Grid_ChildLargerThanCellWidth_ShouldPositionAtLeftEdgeOfCell()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(150f, 30f) { Alignment = Align.MiddleCenter };

            grid.AddChild(child);
            grid.Render();

            child.X.Should().Be(10f);
            child.Y.Should().Be(35f);
        }

        [Fact]
        public void Grid_ChildLargerThanCellHeight_ShouldPositionAtTopEdgeOfCell()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(50f, 80f) { Alignment = Align.MiddleCenter };

            grid.AddChild(child);
            grid.Render();

            child.X.Should().Be(35f);
            child.Y.Should().Be(20f);
        }

        [Fact]
        public void Grid_ChildLargerThanCellBothDimensions_ShouldPositionAtTopLeftOfCell()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(150f, 80f) { Alignment = Align.MiddleCenter };

            grid.AddChild(child);
            grid.Render();

            child.X.Should().Be(10f);
            child.Y.Should().Be(20f);
        }

        [Fact]
        public void Grid_ChildExactlySameAsCell_ShouldPositionAtTopLeftOfCell()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(100f, 60f) { Alignment = Align.MiddleCenter };

            grid.AddChild(child);
            grid.Render();

            child.X.Should().Be(10f);
            child.Y.Should().Be(20f);
        }

        [Fact]
        public void Grid_MultipleChildrenWithDifferentAlignments_ShouldPositionCorrectly()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 0f, Y = 0f };
            var child1 = new UIElement(40f, 20f) { Alignment = Align.UpperLeft };
            var child2 = new UIElement(40f, 20f) { Alignment = Align.MiddleCenter };
            var child3 = new UIElement(40f, 20f) { Alignment = Align.LowerRight };

            grid.AddChild(child1);
            grid.AddChild(child2);
            grid.AddChild(child3);
            grid.Render();

            child1.X.Should().Be(0f);
            child1.Y.Should().Be(0f);

            child2.X.Should().Be(130f);
            child2.Y.Should().Be(20f);

            child3.X.Should().Be(60f);
            child3.Y.Should().Be(100f);
        }

        [Fact]
        public void Grid_WithSpacingAndPadding_ShouldAlignWithinCorrectCellBounds()
        {
            var grid = new Grid(2, 2, 100f, 60f)
            {
                X = 5f,
                Y = 10f,
                ColumnSpacing = 10f,
                RowSpacing = 15f,
                Padding = 8f
            };
            var child = new UIElement(40f, 20f) { Alignment = Align.MiddleCenter };

            grid.AddChild(child);
            grid.Render();

            child.X.Should().Be(43f);
            child.Y.Should().Be(38f);
        }

        [Fact]
        public void Grid_ChildWithZeroWidth_ShouldNotCrash()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(0f, 30f) { Alignment = Align.MiddleCenter };

            grid.AddChild(child);
            grid.Invoking(g => g.Render()).Should().NotThrow();

            child.X.Should().Be(60f);
            child.Y.Should().Be(35f);
        }

        [Fact]
        public void Grid_ChildWithZeroHeight_ShouldNotCrash()
        {
            var grid = new Grid(2, 2, 100f, 60f) { X = 10f, Y = 20f };
            var child = new UIElement(50f, 0f) { Alignment = Align.MiddleCenter };

            grid.AddChild(child);
            grid.Invoking(g => g.Render()).Should().NotThrow();

            child.X.Should().Be(35f);
            child.Y.Should().Be(50f);
        }

        [Fact]
        public void Grid_AutoCalculatingCellSize_ShouldAlignCorrectlyAfterCellSizeCalculation()
        {
            var grid = new Grid(2, 2);
            var child1 = new UIElement(80f, 50f) { Alignment = Align.UpperLeft };
            var child2 = new UIElement(60f, 40f) { Alignment = Align.LowerRight };

            grid.AddChild(child1);
            grid.AddChild(child2);
            grid.Render();

            child1.X.Should().Be(0f);
            child1.Y.Should().Be(0f);

            child2.X.Should().Be(100f);
            child2.Y.Should().Be(10f);
        }

        [Fact]
        public void Grid_ShouldNotUseDefaultLineLayout()
        {
            var grid = new Grid(2, 2, 50f, 30f) { X = 10f, Y = 20f };
            var child1 = new UIElement(40f, 25f);
            var child2 = new UIElement(40f, 25f);

            grid.AddChild(child1);
            grid.AddChild(child2);

            grid.Render();

            child1.X.Should().Be(10f);
            child1.Y.Should().Be(20f);

            child2.X.Should().Be(60f); // Grid's second cell position (10 + 50)
            child2.Y.Should().Be(20f);
        }

        [Fact]
        public void Grid_ChildrenWithFillHeight_ShouldFillCellHeight()
        {
            // Arrange
            var grid = new Grid(2, 2, 100f, 60f);
            var child1 = new UIElement(SizeMode.Fixed, SizeMode.Fill);
            child1.Width = 50f;
            var child2 = new UIElement(SizeMode.Fixed, SizeMode.Fill);
            child2.Width = 40f;

            // Act
            grid.AddChild(child1);
            grid.AddChild(child2);

            // Assert
            child1.Height.Should().Be(60f); // Should fill cell height
            child2.Height.Should().Be(60f); // Should fill cell height
        }

        [Fact]
        public void Grid_WhenCellWidthChanges_ShouldInvalidateFillWidthChildren()
        {
            // Arrange
            var grid = new Grid(2, 2, 100f, 50f);
            var fillChild = new UIElement(SizeMode.Fill, SizeMode.Fixed);
            fillChild.Height = 30f;
            var contentChild = new UIElement(40f, 25f);

            grid.AddChild(fillChild);
            grid.AddChild(contentChild);

            var initialWidth = fillChild.Width;

            // Act
            grid.CellWidth = 150f;
            var newWidth = fillChild.Width;

            // Assert
            initialWidth.Should().Be(100f);
            newWidth.Should().Be(150f);
        }

        [Fact]
        public void Grid_WhenCellHeightChanges_ShouldInvalidateFillHeightChildren()
        {
            // Arrange
            var grid = new Grid(2, 2, 100f, 50f);
            var fillChild = new UIElement(SizeMode.Fixed, SizeMode.Fill);
            fillChild.Width = 80f;
            var contentChild = new UIElement(40f, 25f);

            grid.AddChild(fillChild);
            grid.AddChild(contentChild);

            var initialHeight = fillChild.Height;

            // Act
            grid.CellHeight = 80f;
            var newHeight = fillChild.Height;

            // Assert
            initialHeight.Should().Be(50f);
            newHeight.Should().Be(80f);
        }

        [Fact]
        public void Grid_FillChildrenShouldNotAffectNonFillChildren()
        {
            // Arrange
            var grid = new Grid(2, 2, 100f, 50f);
            var fillChild = new UIElement(SizeMode.Fill, SizeMode.Fill);
            var fixedChild = new UIElement(60f, 30f);
            var contentChild = new UIElement(SizeMode.Content, SizeMode.Content);

            // Act
            grid.AddChild(fillChild);
            grid.AddChild(fixedChild);
            grid.AddChild(contentChild);

            // Assert
            fillChild.Width.Should().Be(100f); // Cell width
            fillChild.Height.Should().Be(50f); // Cell height
            fixedChild.Width.Should().Be(60f); // Original fixed width
            fixedChild.Height.Should().Be(30f); // Original fixed height
            contentChild.Width.Should().Be(50f); // Default content width
            contentChild.Height.Should().Be(30f); // Default content height
        }

        [Fact]
        public void RegularUIElement_FillMode_ShouldStillFillParentWidthHeight()
        {
            // Ensure the base behavior still works for non-Grid parents
            var parent = new UIElement(200f, 150f);
            var fillChild = new UIElement(SizeMode.Fill, SizeMode.Fill);

            parent.AddChild(fillChild);

            fillChild.Width.Should().Be(200f);
            fillChild.Height.Should().Be(150f);
        }
    }
}