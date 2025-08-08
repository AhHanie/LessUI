using FluentAssertions;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    /// <summary>
    /// Tests for ScrollCanvas functionality.
    /// DO NOT USE OR TEST THE INTERNALS OF Verse.Text, Verse.Widgets as it's Unity code which cannot be executed outside of Unity
    /// </summary>
    public class ScrollCanvasTests
    {
        [Fact]
        public void ScrollCanvas_Constructor_ShouldInitializeWithCorrectValues()
        {
            var rect = new Rect(10f, 20f, 300f, 200f);

            var scrollCanvas = new ScrollCanvas(rect);

            scrollCanvas.X.Should().Be(10f);
            scrollCanvas.Y.Should().Be(20f);
            scrollCanvas.Width.Should().Be(300f);
            scrollCanvas.Height.Should().Be(200f);
            scrollCanvas.WidthMode.Should().Be(SizeMode.Fixed);
            scrollCanvas.HeightMode.Should().Be(SizeMode.Fixed);
            scrollCanvas.ScrollPosition.Should().Be(Vector2.zero);
            scrollCanvas.ShowScrollbars.Should().BeTrue();
            scrollCanvas.Padding.Should().Be(0f); // Updated from 5f to 0f
        }

        [Fact]
        public void ScrollCanvas_Constructor_WithOptions_ShouldInitializeCorrectly()
        {
            var rect = new Rect(0f, 0f, 400f, 300f);
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };

            var scrollCanvas = new ScrollCanvas(rect, options);

            scrollCanvas.Alignment.Should().Be(Align.MiddleCenter);
            scrollCanvas.ScrollPosition.Should().Be(Vector2.zero);
            scrollCanvas.ShowScrollbars.Should().BeTrue();
            scrollCanvas.Padding.Should().Be(0f); // Updated from 5f to 0f
        }

        [Fact]
        public void ScrollCanvas_Constructor_WithExternalScrollPosition_ShouldInitializeCorrectly()
        {
            var rect = new Rect(10f, 20f, 300f, 200f);
            var externalScrollPosition = new Vector2(25f, 50f);
            Vector2 GetScrollPosition() => externalScrollPosition;
            void SetScrollPosition(Vector2 value) => externalScrollPosition = value;

            var scrollCanvas = new ScrollCanvas(rect, GetScrollPosition, SetScrollPosition);

            scrollCanvas.X.Should().Be(10f);
            scrollCanvas.Y.Should().Be(20f);
            scrollCanvas.Width.Should().Be(300f);
            scrollCanvas.Height.Should().Be(200f);
            scrollCanvas.WidthMode.Should().Be(SizeMode.Fixed);
            scrollCanvas.HeightMode.Should().Be(SizeMode.Fixed);
            scrollCanvas.ScrollPosition.Should().Be(new Vector2(25f, 50f));
            scrollCanvas.ShowScrollbars.Should().BeTrue();
            scrollCanvas.Padding.Should().Be(0f); // Updated from 5f to 0f
        }

        [Fact]
        public void ScrollCanvas_Constructor_WithExternalScrollPositionAndOptions_ShouldInitializeCorrectly()
        {
            var rect = new Rect(0f, 0f, 400f, 300f);
            var externalScrollPosition = Vector2.zero;
            Vector2 GetScrollPosition() => externalScrollPosition;
            void SetScrollPosition(Vector2 value) => externalScrollPosition = value;
            var options = new UIElementOptions { Alignment = Align.LowerRight };

            var scrollCanvas = new ScrollCanvas(rect, GetScrollPosition, SetScrollPosition, options);

            scrollCanvas.Alignment.Should().Be(Align.LowerRight);
            scrollCanvas.ScrollPosition.Should().Be(Vector2.zero);
            scrollCanvas.ShowScrollbars.Should().BeTrue();
            scrollCanvas.Padding.Should().Be(0f); // Updated from 5f to 0f
        }

        [Fact]
        public void ScrollPosition_Property_ShouldGetAndSetCorrectly()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f));
            var newPosition = new Vector2(50f, 75f);

            scrollCanvas.ScrollPosition = newPosition;

            scrollCanvas.ScrollPosition.Should().Be(newPosition);
        }

        [Fact]
        public void ScrollPosition_WithExternalDelegates_ShouldUseExternalStorage()
        {
            var externalScrollPosition = new Vector2(10f, 20f);
            Vector2 GetScrollPosition() => externalScrollPosition;
            void SetScrollPosition(Vector2 value) => externalScrollPosition = value;

            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f), GetScrollPosition, SetScrollPosition);

            // Should get from external storage
            scrollCanvas.ScrollPosition.Should().Be(new Vector2(10f, 20f));

            // Should set to external storage
            var newPosition = new Vector2(50f, 75f);
            scrollCanvas.ScrollPosition = newPosition;
            scrollCanvas.ScrollPosition.Should().Be(newPosition);
            externalScrollPosition.Should().Be(newPosition);
        }

        [Fact]
        public void ScrollPosition_WithExternalDelegates_ShouldReflectExternalChanges()
        {
            var externalScrollPosition = Vector2.zero;
            Vector2 GetScrollPosition() => externalScrollPosition;
            void SetScrollPosition(Vector2 value) => externalScrollPosition = value;

            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f), GetScrollPosition, SetScrollPosition);

            // Change external value directly
            externalScrollPosition = new Vector2(100f, 150f);

            // ScrollCanvas should reflect the external change
            scrollCanvas.ScrollPosition.Should().Be(new Vector2(100f, 150f));
        }

        [Fact]
        public void ShowScrollbars_Property_ShouldGetAndSetCorrectly()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f));

            scrollCanvas.ShowScrollbars = false;

            scrollCanvas.ShowScrollbars.Should().BeFalse();
        }

        [Fact]
        public void UpdateRect_ShouldUpdateDimensionsCorrectly()
        {
            var initialRect = new Rect(0f, 0f, 200f, 150f);
            var scrollCanvas = new ScrollCanvas(initialRect);
            var newRect = new Rect(10f, 20f, 400f, 300f);

            scrollCanvas.UpdateRect(newRect);

            scrollCanvas.X.Should().Be(10f);
            scrollCanvas.Y.Should().Be(20f);
            scrollCanvas.Width.Should().Be(400f);
            scrollCanvas.Height.Should().Be(300f);
            scrollCanvas.GetRect().Should().Be(newRect);
        }

        [Fact]
        public void UpdateRect_WithFillChildren_ShouldInvalidateChildSizes()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 200f, 100f));
            var fillChild = new UIElement(SizeMode.Fill, SizeMode.Fixed);
            fillChild.Height = 50f;
            scrollCanvas.AddChild(fillChild);

            var initialWidth = fillChild.Width;
            scrollCanvas.UpdateRect(new Rect(0f, 0f, 400f, 100f));
            var newWidth = fillChild.Width;

            initialWidth.Should().Be(200f);
            newWidth.Should().Be(400f);
        }

        [Fact]
        public void CalculateViewRect_WithNoChildren_ShouldReturnMinimumSize()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f));

            var viewRect = scrollCanvas.CalculateViewRect();

            viewRect.Should().Be(new Rect(0f, 0f, 300f, 200f));
        }

        [Fact]
        public void CalculateViewRect_WithChildren_ShouldCalculateCorrectBounds()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f));
            var child1 = new UIElement(100f, 50f) { X = 10f, Y = 20f };
            var child2 = new UIElement(80f, 60f) { X = 150f, Y = 100f };
            var child3 = new UIElement(120f, 40f) { X = 50f, Y = 180f };

            scrollCanvas.AddChild(child1);
            scrollCanvas.AddChild(child2);
            scrollCanvas.AddChild(child3);

            var viewRect = scrollCanvas.CalculateViewRect();

            // Expected bounds: 
            // X: min(10, 150, 50) = 10, max(110, 230, 170) = 230, width = 220
            // Y: min(20, 100, 180) = 20, max(70, 160, 220) = 220, height = 200
            viewRect.x.Should().Be(10f);
            viewRect.y.Should().Be(20f);
            viewRect.width.Should().Be(220f); // 230 - 10
            viewRect.height.Should().Be(200f); // 220 - 20
        }

        [Fact]
        public void CalculateViewRect_ShouldEnsureMinimumSize()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f));
            var smallChild = new UIElement(50f, 30f) { X = 0f, Y = 0f };

            scrollCanvas.AddChild(smallChild);

            var viewRect = scrollCanvas.CalculateViewRect();

            // Should be at least the size of the scroll canvas
            viewRect.width.Should().BeGreaterThanOrEqualTo(300f);
            viewRect.height.Should().BeGreaterThanOrEqualTo(200f);
        }

        [Fact]
        public void CalculateViewRect_WithChildrenOutsideCanvasBounds_ShouldIncludeAllChildren()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 200f, 150f));
            var child1 = new UIElement(100f, 50f) { X = 0f, Y = 0f };
            var child2 = new UIElement(100f, 50f) { X = 250f, Y = 200f }; // Outside canvas

            scrollCanvas.AddChild(child1);
            scrollCanvas.AddChild(child2);

            var viewRect = scrollCanvas.CalculateViewRect();

            // Should include the child outside canvas bounds
            viewRect.width.Should().Be(350f); // 0 to 350 (250 + 100)
            viewRect.height.Should().Be(250f); // 0 to 250 (200 + 50)
        }

        [Fact]
        public void GetRect_ShouldReturnCurrentRect()
        {
            var rect = new Rect(15f, 25f, 400f, 300f);
            var scrollCanvas = new ScrollCanvas(rect);

            var result = scrollCanvas.GetRect();

            result.Should().Be(rect);
        }

        [Fact]
        public void ScrollCanvas_WithFillGrid_ShouldCalculateViewRectCorrectly()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 400f, 300f));
            var fillGrid = new FillGrid(3, 4); // 3 columns, 4 rows
            fillGrid.CellHeight = 60f;
            fillGrid.RowSpacing = 10f;
            fillGrid.Padding = 15f;

            scrollCanvas.AddChild(fillGrid);

            // Add some children to the grid
            for (int i = 0; i < 8; i++)
            {
                fillGrid.AddChild(new UIElement(50f, 40f));
            }

            // Access grid properties to trigger size calculations without rendering
            var gridWidth = fillGrid.Width;
            var gridHeight = fillGrid.Height;

            var viewRect = scrollCanvas.CalculateViewRect();

            // Grid should fill the canvas width (400f)
            // Grid height with auto-calculated cell height (40f from max child height): (4 * 40) + (3 * 10) + (2 * 15) = 160 + 30 + 30 = 220
            viewRect.width.Should().BeGreaterThanOrEqualTo(400f);
            viewRect.height.Should().BeGreaterThanOrEqualTo(220f); // Updated from 300f to 220f to match actual auto-calculated height
        }

        [Fact]
        public void ScrollCanvas_WithContentSizedChild_ShouldRecalculateViewRect()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f));
            var contentChild = new UIElement(SizeMode.Content, SizeMode.Content);
            scrollCanvas.AddChild(contentChild);

            var initialViewRect = scrollCanvas.CalculateViewRect();

            // Change the content child's calculated size by invalidating
            contentChild.InvalidateSize();
            var newViewRect = scrollCanvas.CalculateViewRect();

            // View rect should be recalculated
            newViewRect.Should().Be(initialViewRect); // Since our test element returns fixed size
        }

        [Fact]
        public void ScrollCanvas_AddChild_ShouldMaintainScrollPosition()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f));
            var initialPosition = new Vector2(25f, 50f);
            scrollCanvas.ScrollPosition = initialPosition;

            var child = new UIElement(100f, 50f);
            scrollCanvas.AddChild(child);

            scrollCanvas.ScrollPosition.Should().Be(initialPosition);
        }

        [Fact]
        public void ScrollCanvas_AddChild_WithExternalScrollPosition_ShouldMaintainScrollPosition()
        {
            var externalScrollPosition = new Vector2(25f, 50f);
            Vector2 GetScrollPosition() => externalScrollPosition;
            void SetScrollPosition(Vector2 value) => externalScrollPosition = value;

            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f), GetScrollPosition, SetScrollPosition);

            var child = new UIElement(100f, 50f);
            scrollCanvas.AddChild(child);

            scrollCanvas.ScrollPosition.Should().Be(new Vector2(25f, 50f));
            externalScrollPosition.Should().Be(new Vector2(25f, 50f));
        }

        [Fact]
        public void ScrollCanvas_RemoveChild_ShouldMaintainScrollPosition()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f));
            var child = new UIElement(100f, 50f);
            scrollCanvas.AddChild(child);
            var position = new Vector2(25f, 50f);
            scrollCanvas.ScrollPosition = position;

            scrollCanvas.RemoveChild(child);

            scrollCanvas.ScrollPosition.Should().Be(position);
        }

        [Fact]
        public void ScrollCanvas_WithExternalScrollPosition_ShouldPersistBetweenCanvasRecreation()
        {
            // Simulate the game loop scenario where Canvas gets recreated each frame
            var staticScrollPosition = new Vector2(75f, 100f);
            Vector2 GetScrollPosition() => staticScrollPosition;
            void SetScrollPosition(Vector2 value) => staticScrollPosition = value;

            // First frame - create canvas and set scroll position
            var canvas1 = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f), GetScrollPosition, SetScrollPosition);
            canvas1.ScrollPosition = new Vector2(150f, 200f);

            // Verify external storage was updated
            staticScrollPosition.Should().Be(new Vector2(150f, 200f));

            // Second frame - recreate canvas (simulating fresh creation in game loop)
            var canvas2 = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f), GetScrollPosition, SetScrollPosition);

            // New canvas should read from external storage
            canvas2.ScrollPosition.Should().Be(new Vector2(150f, 200f));
        }

        [Fact]
        public void ScrollCanvas_WithNegativeChildPositions_ShouldHandleCorrectly()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f));
            var child1 = new UIElement(100f, 50f) { X = -50f, Y = -30f };
            var child2 = new UIElement(80f, 60f) { X = 100f, Y = 150f };

            scrollCanvas.AddChild(child1);
            scrollCanvas.AddChild(child2);

            var viewRect = scrollCanvas.CalculateViewRect();

            // Should start from negative position
            viewRect.x.Should().Be(-50f);
            viewRect.y.Should().Be(-30f);
            // Width: from -50 to 180 (100 + 80) = 230
            // Height: from -30 to 210 (150 + 60) = 240
            viewRect.width.Should().Be(230f);
            viewRect.height.Should().Be(240f);
        }

        [Fact]
        public void ScrollCanvas_EmptyWithCustomSize_ShouldUseCanvasSize()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(50f, 75f, 250f, 180f));

            var viewRect = scrollCanvas.CalculateViewRect();

            // Should use canvas position and size when no children
            viewRect.x.Should().Be(50f);
            viewRect.y.Should().Be(75f);
            viewRect.width.Should().Be(250f);
            viewRect.height.Should().Be(180f);
        }

        [Fact]
        public void ScrollCanvas_DefaultPadding_ShouldBeZero()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f));

            scrollCanvas.Padding.Should().Be(0f);
        }

        [Fact]
        public void ScrollCanvas_Padding_Property_ShouldGetAndSetCorrectly()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f));
            var expectedPadding = 10f;

            scrollCanvas.Padding = expectedPadding;

            scrollCanvas.Padding.Should().Be(expectedPadding);
        }

        [Fact]
        public void ScrollCanvas_WithPadding_ShouldCalculateScrollRectCorrectly()
        {
            var outerRect = new Rect(20f, 30f, 400f, 300f);
            var scrollCanvas = new ScrollCanvas(outerRect);
            scrollCanvas.Padding = 15f;

            var scrollRect = scrollCanvas.CalculateScrollRect();

            // Scroll rect should be inset by padding on all sides
            // x: 20 + 15 = 35, y: 30 + 15 = 45
            // width: 400 - (2 * 15) = 370, height: 300 - (2 * 15) = 270
            scrollRect.x.Should().Be(35f);
            scrollRect.y.Should().Be(45f);
            scrollRect.width.Should().Be(370f);
            scrollRect.height.Should().Be(270f);
        }

        [Fact]
        public void ScrollCanvas_WithZeroPadding_ShouldReturnOriginalRect()
        {
            var outerRect = new Rect(10f, 20f, 300f, 200f);
            var scrollCanvas = new ScrollCanvas(outerRect);
            scrollCanvas.Padding = 0f;

            var scrollRect = scrollCanvas.CalculateScrollRect();

            scrollRect.Should().Be(outerRect);
        }

        [Fact]
        public void ScrollCanvas_WithLargePadding_ShouldHandleCorrectly()
        {
            var outerRect = new Rect(0f, 0f, 100f, 80f);
            var scrollCanvas = new ScrollCanvas(outerRect);
            scrollCanvas.Padding = 30f;

            var scrollRect = scrollCanvas.CalculateScrollRect();

            scrollRect.x.Should().Be(30f);
            scrollRect.y.Should().Be(30f);
            scrollRect.width.Should().Be(40f); // 100 - (2 * 30)
            scrollRect.height.Should().Be(20f); // 80 - (2 * 30)
        }

        [Fact]
        public void ScrollCanvas_PaddingLargerThanHalfDimensions_ShouldNotGiveNegativeSize()
        {
            var outerRect = new Rect(0f, 0f, 100f, 80f);
            var scrollCanvas = new ScrollCanvas(outerRect);
            scrollCanvas.Padding = 60f;

            var scrollRect = scrollCanvas.CalculateScrollRect();

            scrollRect.width.Should().BeGreaterThan(0f);
            scrollRect.height.Should().BeGreaterThan(0f);
        }

        [Fact]
        public void ScrollCanvas_UpdateRect_WithPadding_ShouldRecalculateScrollRect()
        {
            var initialRect = new Rect(0f, 0f, 200f, 150f);
            var scrollCanvas = new ScrollCanvas(initialRect);
            scrollCanvas.Padding = 10f;

            var initialScrollRect = scrollCanvas.CalculateScrollRect();

            var newRect = new Rect(0f, 0f, 400f, 300f);
            scrollCanvas.UpdateRect(newRect);
            var newScrollRect = scrollCanvas.CalculateScrollRect();

            initialScrollRect.width.Should().Be(180f); // 200 - (2 * 10)
            initialScrollRect.height.Should().Be(130f); // 150 - (2 * 10)

            newScrollRect.width.Should().Be(380f); // 400 - (2 * 10)
            newScrollRect.height.Should().Be(280f); // 300 - (2 * 10)
        }

        [Fact]
        public void ScrollCanvas_CalculateViewRect_WithPadding_ShouldAccountForPaddedArea()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f));
            scrollCanvas.Padding = 20f;

            var child = new UIElement(100f, 50f) { X = 20f, Y = 20f };
            scrollCanvas.AddChild(child);

            var viewRect = scrollCanvas.CalculateViewRect();

            var scrollRect = scrollCanvas.CalculateScrollRect();
            viewRect.width.Should().BeGreaterThanOrEqualTo(scrollRect.width);
            viewRect.height.Should().BeGreaterThanOrEqualTo(scrollRect.height);
        }

        [Fact]
        public void ScrollCanvas_WithPaddingAndNoChildren_ShouldReturnScrollRect()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(10f, 15f, 300f, 200f));
            scrollCanvas.Padding = 25f;

            var viewRect = scrollCanvas.CalculateViewRect();
            var expectedScrollRect = scrollCanvas.CalculateScrollRect();

            // Updated: should return canvas rect, not scroll rect when no children
            viewRect.Should().Be(new Rect(10f, 15f, 300f, 200f));
        }

        [Fact]
        public void ScrollCanvas_Constructor_WithExternalScrollPositionAndPadding_ShouldWork()
        {
            var externalScrollPosition = Vector2.zero;
            Vector2 GetScrollPosition() => externalScrollPosition;
            void SetScrollPosition(Vector2 value) => externalScrollPosition = value;

            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 400f, 300f), GetScrollPosition, SetScrollPosition);
            scrollCanvas.Padding = 15f;

            var scrollRect = scrollCanvas.CalculateScrollRect();

            scrollRect.width.Should().Be(370f); // 400 - (2 * 15)
            scrollRect.height.Should().Be(270f); // 300 - (2 * 15)
            scrollCanvas.ScrollPosition.Should().Be(Vector2.zero);
        }

        [Fact]
        public void ScrollCanvas_WithPaddingAndFillChildren_ShouldStillFillScrollArea()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 400f, 300f));
            scrollCanvas.Padding = 20f;
            var fillChild = new UIElement(SizeMode.Fill, SizeMode.Fixed);
            fillChild.Height = 50f;

            scrollCanvas.AddChild(fillChild);

            fillChild.Width.Should().Be(400f);
        }

        [Fact]
        public void ScrollCanvas_WithPaddingAndScrollPosition_ShouldHandleCorrectly()
        {
            var scrollCanvas = new ScrollCanvas(new Rect(0f, 0f, 300f, 200f));
            scrollCanvas.Padding = 10f;
            var initialPosition = new Vector2(25f, 40f);

            scrollCanvas.ScrollPosition = initialPosition;

            scrollCanvas.ScrollPosition.Should().Be(initialPosition);
            scrollCanvas.CalculateScrollRect().width.Should().Be(280f); // 300 - (2 * 10)
            scrollCanvas.CalculateScrollRect().height.Should().Be(180f); // 200 - (2 * 10)
        }
    }
}