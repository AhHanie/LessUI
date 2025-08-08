using FluentAssertions;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    /// <summary>
    /// Integration tests that verify Canvas works properly with other UI elements.
    ///  DO NOT USE OR TEST THE INTERNALS OF Verse.Text, Verse.Widgets as it's Unity code which cannot be executed outside of Unity
    /// </summary>
    public class CanvasTests
    {
        [Fact]
        public void Canvas_WithFillGrid_ShouldCalculateCellDimensionsCorrectly()
        {
            var canvasRect = new Rect(0f, 0f, 600f, 400f);
            var canvas = new Canvas(canvasRect);
            var fillGrid = new FillGrid(3, 2); // 3 columns, 2 rows
            fillGrid.ColumnSpacing = 10f;
            fillGrid.RowSpacing = 15f;
            fillGrid.Padding = 20f;

            canvas.AddChild(fillGrid);

            fillGrid.Width.Should().Be(600f); // Should fill canvas width

            // Available width for cells: 600 - (2 * 20 padding) - (2 * 10 column spacing) = 540
            // Cell width: 540 / 3 = 180
            fillGrid.CellWidth.Should().Be(180f);
        }

        [Fact]
        public void Canvas_UpdateRect_WithFillGrid_ShouldRecalculateCellSizes()
        {
            
            var initialRect = new Rect(0f, 0f, 400f, 300f);
            var canvas = new Canvas(initialRect);
            var fillGrid = new FillGrid(4, 1); // 4 columns
            canvas.AddChild(fillGrid);

            var initialCellWidth = fillGrid.CellWidth; // 400 / 4 = 100

            var newRect = new Rect(0f, 0f, 800f, 300f); // Double the width
            canvas.UpdateRect(newRect);

            fillGrid.Width.Should().Be(800f);
            fillGrid.CellWidth.Should().Be(200f); // 800 / 4 = 200
            fillGrid.CellWidth.Should().NotBe(initialCellWidth);
        }

        [Fact]
        public void Canvas_MultipleUpdates_ShouldHandleCorrectly()
        {
            
            var canvas = new Canvas(new Rect(0f, 0f, 200f, 100f));
            var fillGrid = new FillGrid(2, 1);
            canvas.AddChild(fillGrid);

            canvas.UpdateRect(new Rect(0f, 0f, 400f, 200f));
            fillGrid.Width.Should().Be(400f);
            fillGrid.CellWidth.Should().Be(200f);

            canvas.UpdateRect(new Rect(10f, 20f, 600f, 300f));
            canvas.X.Should().Be(10f);
            canvas.Y.Should().Be(20f);
            fillGrid.Width.Should().Be(600f);
            fillGrid.CellWidth.Should().Be(300f);

            canvas.UpdateRect(new Rect(0f, 0f, 300f, 150f));
            fillGrid.Width.Should().Be(300f);
            fillGrid.CellWidth.Should().Be(150f);
        }
    }
}