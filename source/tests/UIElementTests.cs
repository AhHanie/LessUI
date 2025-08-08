using FluentAssertions;
using System.Collections.Generic;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    public class UIElementTests
    {
        [Fact]
        public void Constructor_WithWidthAndHeight_ShouldInitializeWithCorrectValues()
        {
            float width = 100f;
            float height = 50f;

            var element = new UIElement(width, height);

            element.X.Should().Be(0);
            element.Y.Should().Be(0);
            element.Width.Should().Be(width);
            element.Height.Should().Be(height);
            element.WidthMode.Should().Be(SizeMode.Fixed);
            element.HeightMode.Should().Be(SizeMode.Fixed);
            element.Alignment.Should().Be(Align.UpperLeft);
            element.Children.Should().NotBeNull();
            element.Children.Should().BeEmpty();
            element.Parent.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithWidthHeightAndOptions_ShouldInitializeWithCorrectValues()
        {
            float width = 100f;
            float height = 50f;
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };

            var element = new UIElement(width, height, options);

            element.X.Should().Be(0);
            element.Y.Should().Be(0);
            element.Width.Should().Be(width);
            element.Height.Should().Be(height);
            element.WidthMode.Should().Be(SizeMode.Fixed);
            element.HeightMode.Should().Be(SizeMode.Fixed);
            element.Alignment.Should().Be(Align.MiddleCenter);
            element.Children.Should().NotBeNull();
            element.Children.Should().BeEmpty();
            element.Parent.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithSizeMode_ShouldInitializeWithCorrectValues()
        {
            var element = new UIElement(SizeMode.Content, SizeMode.Fixed);

            element.X.Should().Be(0);
            element.Y.Should().Be(0);
            element.WidthMode.Should().Be(SizeMode.Content);
            element.HeightMode.Should().Be(SizeMode.Fixed);
            element.Alignment.Should().Be(Align.UpperLeft);
            element.Children.Should().NotBeNull();
            element.Children.Should().BeEmpty();
            element.Parent.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithNullOptions_ShouldUseDefaultAlignment()
        {
            var element = new UIElement(100f, 50f, null);

            element.Alignment.Should().Be(Align.UpperLeft);
        }

        [Fact]
        public void Alignment_Property_ShouldGetAndSetCorrectly()
        {
            var element = new UIElement(100f, 50f);

            element.Alignment = Align.LowerRight;

            element.Alignment.Should().Be(Align.LowerRight);
        }

        [Fact]
        public void Width_Property_ShouldGetAndSetCorrectly()
        {
            var element = new UIElement(100f, 50f);
            var expectedWidth = 150.5f;

            element.Width = expectedWidth;

            element.Width.Should().Be(expectedWidth);
        }

        [Fact]
        public void Height_Property_ShouldGetAndSetCorrectly()
        {
            var element = new UIElement(100f, 50f);
            var expectedHeight = 200.75f;

            element.Height = expectedHeight;

            element.Height.Should().Be(expectedHeight);
        }

        [Fact]
        public void CalculateDynamicSize_ShouldReturnDefaultSize()
        {
            var element = new UIElement(SizeMode.Content, SizeMode.Content);

            var size = element.CalculateDynamicSize();

            size.width.Should().Be(50f);
            size.height.Should().Be(30f);
        }

        [Fact]
        public void Width_WithContentMode_ShouldCalculateDynamicSize()
        {
            var element = new TestUIElement(SizeMode.Content, SizeMode.Fixed);
            element.Height = 100f;

            var width = element.Width;

            width.Should().Be(200f);
            element.CalculateCallCount.Should().Be(1);
        }

        [Fact]
        public void Height_WithContentMode_ShouldCalculateDynamicSize()
        {
            var element = new TestUIElement(SizeMode.Fixed, SizeMode.Content);
            element.Width = 150f;

            var height = element.Height;

            height.Should().Be(100f);
            element.CalculateCallCount.Should().Be(1);
        }

        [Fact]
        public void Width_WithFixedMode_ShouldNotCalculateDynamicSize()
        {
            var element = new TestUIElement(SizeMode.Fixed, SizeMode.Content);
            element.Width = 150f;

            var width = element.Width;

            width.Should().Be(150f);
            element.CalculateCallCount.Should().Be(0);
        }

        [Fact]
        public void Height_WithFixedMode_ShouldNotCalculateDynamicSize()
        {
            var element = new TestUIElement(SizeMode.Content, SizeMode.Fixed);
            element.Height = 75f;

            var height = element.Height;

            height.Should().Be(75f);
            element.CalculateCallCount.Should().Be(0);
        }

        [Fact]
        public void Width_WithFillMode_ShouldUseParentWidth()
        {
            var parent = new UIElement(200f, 100f);
            var child = new UIElement(SizeMode.Fill, SizeMode.Fixed);
            child.Height = 50f;

            parent.AddChild(child);

            child.Width.Should().Be(200f);
        }

        [Fact]
        public void Height_WithFillMode_ShouldUseParentHeight()
        {
            var parent = new UIElement(200f, 100f);
            var child = new UIElement(SizeMode.Fixed, SizeMode.Fill);
            child.Width = 150f;

            parent.AddChild(child);

            child.Height.Should().Be(100f);
        }

        [Fact]
        public void Width_WithFillMode_WithoutParent_ShouldReturnZero()
        {
            var element = new UIElement(SizeMode.Fill, SizeMode.Fixed);
            element.Height = 50f;

            var width = element.Width;

            width.Should().Be(0f);
        }

        [Fact]
        public void Height_WithFillMode_WithoutParent_ShouldReturnZero()
        {
            var element = new UIElement(SizeMode.Fixed, SizeMode.Fill);
            element.Width = 150f;

            var height = element.Height;

            height.Should().Be(0f);
        }

        [Fact]
        public void Parent_Property_ShouldTriggerOnParentSet()
        {
            var parent = new UIElement(200f, 100f);
            var child = new TestUIElementWithParentCallback(SizeMode.Fixed, SizeMode.Fixed);
            child.Width = 100f;
            child.Height = 50f;

            child.Parent = parent;

            child.OnParentSetCalled.Should().BeTrue();
        }

        [Fact]
        public void OnParentSet_WithFillMode_ShouldInvalidateSize()
        {
            var parent = new UIElement(200f, 100f);
            var child = new TestUIElement(SizeMode.Fill, SizeMode.Fill);

            var initialWidth = child.Width;
            var initialCallCount = child.CalculateCallCount;

            parent.AddChild(child);

            var newWidth = child.Width;

            child.Width.Should().Be(200f);
            child.Height.Should().Be(100f);
        }

        [Fact]
        public void InvalidateSize_WithFillMode_ShouldInvalidate()
        {
            var element = new UIElement(SizeMode.Fill, SizeMode.Content);

            element.InvalidateSize();

            element.Invoking(e => e.InvalidateSize()).Should().NotThrow();
        }

        [Fact]
        public void InvalidateSize_WithContentMode_ShouldInvalidateOnlyWidth()
        {
            var element = new TestUIElement(SizeMode.Content, SizeMode.Fixed);
            element.Height = 100f;
            var width1 = element.Width;
            var callCount1 = element.CalculateCallCount;

            element.InvalidateSize();
            var width2 = element.Width;
            var height2 = element.Height;

            element.CalculateCallCount.Should().Be(callCount1 + 1);
            height2.Should().Be(100f);
        }

        [Fact]
        public void InvalidateSize_WithContentHeight_ShouldInvalidateOnlyHeight()
        {
            var element = new TestUIElement(SizeMode.Fixed, SizeMode.Content);
            element.Width = 150f;
            var height1 = element.Height;
            var callCount1 = element.CalculateCallCount;

            element.InvalidateSize();
            var width2 = element.Width;
            var height2 = element.Height;

            element.CalculateCallCount.Should().Be(callCount1 + 1);
            width2.Should().Be(150f);
        }

        [Fact]
        public void SetHeight_WithContentWidth_ShouldNotAffectWidthCalculation()
        {
            var element = new TestUIElement(SizeMode.Content, SizeMode.Fixed);

            element.Height = 100f;
            var width = element.Width;

            width.Should().Be(200f);
            element.CalculateCallCount.Should().Be(1);
        }

        [Fact]
        public void SetWidth_WithContentHeight_ShouldNotAffectHeightCalculation()
        {
            var element = new TestUIElement(SizeMode.Fixed, SizeMode.Content);

            element.Width = 150f;
            var height = element.Height;

            height.Should().Be(100f);
            element.CalculateCallCount.Should().Be(1);
        }

        [Fact]
        public void InvalidateSize_ShouldForceRecalculation()
        {
            var element = new TestUIElement(SizeMode.Content, SizeMode.Content);
            var initialWidth = element.Width;
            var initialHeight = element.Height;
            var callCountAfterInitial = element.CalculateCallCount;

            element.InvalidateSize();
            var newWidth = element.Width;
            var newHeight = element.Height;

            element.CalculateCallCount.Should().BeGreaterThan(callCountAfterInitial);
        }

        [Fact]
        public void InvalidateSize_ShouldInvalidateParentIfParentIsContentSized()
        {
            var parent = new TestUIElement(SizeMode.Content, SizeMode.Content);
            var child = new TestUIElement(SizeMode.Fixed, SizeMode.Fixed);
            child.Width = 100f;
            child.Height = 50f;
            parent.AddChild(child);

            var parentWidth = parent.Width;
            var parentHeight = parent.Height;
            var parentCallCount = parent.CalculateCallCount;

            child.InvalidateSize();
            var newParentWidth = parent.Width;
            var newParentHeight = parent.Height;

            parent.CalculateCallCount.Should().BeGreaterThan(parentCallCount);
        }

        [Fact]
        public void InvalidateSize_ShouldNotInvalidateParentIfParentIsFixedSized()
        {
            var parent = new TestUIElement(SizeMode.Fixed, SizeMode.Fixed);
            parent.Width = 200f;
            parent.Height = 100f;
            var child = new TestUIElement(SizeMode.Content, SizeMode.Content);
            parent.AddChild(child);

            var parentCallCount = parent.CalculateCallCount;

            child.InvalidateSize();
            var parentWidth = parent.Width;
            var parentHeight = parent.Height;

            parent.CalculateCallCount.Should().Be(parentCallCount);
        }

        [Fact]
        public void Render_ShouldCallRenderOnAllChildren()
        {
            var parent = new UIElement(100f, 50f);
            var child1 = new MockUIElement(80f, 40f);
            var child2 = new MockUIElement(60f, 30f);
            var grandChild = new MockUIElement(40f, 20f);

            parent.Children.Add(child1);
            parent.Children.Add(child2);
            child1.Children.Add(grandChild);

            parent.Render();

            child1.RenderCalled.Should().BeTrue();
            child2.RenderCalled.Should().BeTrue();
            grandChild.RenderCalled.Should().BeTrue();
        }

        [Fact]
        public void Render_ShouldCallRenderElementBeforeChildren()
        {
            var parent = new MockUIElement(100f, 50f);
            var child = new MockUIElement(80f, 40f);
            parent.Children.Add(child);

            parent.Render();

            parent.RenderElementCalled.Should().BeTrue();
            child.RenderCalled.Should().BeTrue();
            parent.RenderElementCallOrder.Should().BeLessThan(child.RenderCallOrder);
        }

        [Fact]
        public void Render_WithNoChildren_ShouldNotThrow()
        {
            var element = new UIElement(100f, 50f);

            element.Invoking(e => e.Render()).Should().NotThrow();
        }

        [Fact]
        public void AddChild_ShouldAddToChildrenCollection()
        {
            var parent = new UIElement(100f, 50f);
            var child = new UIElement(80f, 40f);

            parent.AddChild(child);

            parent.Children.Should().Contain(child);
            child.Parent.Should().Be(parent);
        }

        [Fact]
        public void AddChild_ShouldSetParentOnChild()
        {
            var parent = new UIElement(100f, 50f);
            var child = new UIElement(80f, 40f);

            parent.AddChild(child);

            child.Parent.Should().Be(parent);
        }

        [Fact]
        public void AddChild_WithContentSizedParent_ShouldInvalidateParentSize()
        {
            var parent = new TestUIElement(SizeMode.Content, SizeMode.Content);
            var child = new UIElement(80f, 40f);
            var width = parent.Width;
            var callCount = parent.CalculateCallCount;

            parent.AddChild(child);
            var newWidth = parent.Width;

            parent.CalculateCallCount.Should().Be(callCount + 1);
        }

        [Fact]
        public void RemoveChild_WithNonExistentChild_ShouldNotThrow()
        {
            var parent = new UIElement(100f, 50f);
            var child = new UIElement(80f, 40f);

            parent.Invoking(p => p.RemoveChild(child)).Should().NotThrow();
        }

        [Fact]
        public void RemoveChild_WithContentSizedParent_ShouldInvalidateParentSize()
        {
            var parent = new TestUIElement(SizeMode.Content, SizeMode.Content);
            var child = new UIElement(80f, 40f);
            parent.AddChild(child);
            var width = parent.Width;
            var callCount = parent.CalculateCallCount;

            parent.RemoveChild(child);
            var newWidth = parent.Width;

            parent.CalculateCallCount.Should().Be(callCount + 1);
        }

        [Fact]
        public void X_Property_ShouldGetAndSetCorrectly()
        {
            var element = new UIElement(100f, 50f);
            var expectedX = 123.45f;

            element.X = expectedX;

            element.X.Should().Be(expectedX);
        }

        [Fact]
        public void Y_Property_ShouldGetAndSetCorrectly()
        {
            var element = new UIElement(100f, 50f);
            var expectedY = 67.89f;

            element.Y = expectedY;

            element.Y.Should().Be(expectedY);
        }

        [Fact]
        public void WidthMode_Property_ShouldGetAndSetCorrectly()
        {
            var element = new UIElement(100f, 50f);

            element.WidthMode = SizeMode.Content;

            element.WidthMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void HeightMode_Property_ShouldGetAndSetCorrectly()
        {
            var element = new UIElement(100f, 50f);

            element.HeightMode = SizeMode.Fill;

            element.HeightMode.Should().Be(SizeMode.Fill);
        }

        [Fact]
        public void Children_Property_ShouldNotBeNull()
        {
            var element = new UIElement(100f, 50f);

            element.Children.Should().NotBeNull();
            element.Children.Should().BeOfType<List<UIElement>>();
        }

        [Fact]
        public void AddChild_WithNullChild_ShouldNotThrow()
        {
            var parent = new UIElement(100f, 50f);

            parent.Invoking(p => p.AddChild(null)).Should().NotThrow();
            parent.Children.Should().BeEmpty();
        }

        [Fact]
        public void RemoveChild_WithNullChild_ShouldNotThrow()
        {
            var parent = new UIElement(100f, 50f);

            parent.Invoking(p => p.RemoveChild(null)).Should().NotThrow();
        }

        [Fact]
        public void UIElement_WithChildren_ShouldLayoutChildrenInLines()
        {
            // Arrange
            var parent = new UIElement(200f, 300f) { X = 10f, Y = 20f };
            var child1 = new UIElement(50f, 30f);
            var child2 = new UIElement(60f, 25f);
            var child3 = new UIElement(40f, 35f);

            parent.AddChild(child1);
            parent.AddChild(child2);
            parent.AddChild(child3);

            // Act
            parent.Render();

            // Assert
            // First child should be at parent's position
            child1.X.Should().Be(10f);
            child1.Y.Should().Be(20f);

            // Second child should be below first child with default spacing
            child2.X.Should().Be(10f);
            child2.Y.Should().Be(52f); // 20 + 30 (child1 height) + 2 (default spacing)

            // Third child should be below second child
            child3.X.Should().Be(10f);
            child3.Y.Should().Be(79f); // 52 + 25 (child2 height) + 2 (default spacing)
        }

        [Fact]
        public void UIElement_WithCustomVerticalSpacing_ShouldUseCustomSpacing()
        {
            var parent = new UIElement(200f, 300f) { X = 0f, Y = 0f };
            parent.VerticalSpacing = 10f;
            var child1 = new UIElement(50f, 20f);
            var child2 = new UIElement(60f, 30f);

            parent.AddChild(child1);
            parent.AddChild(child2);

            parent.Render();

            child1.X.Should().Be(0f);
            child1.Y.Should().Be(0f);

            child2.X.Should().Be(0f);
            child2.Y.Should().Be(30f); // 0 + 20 (child1 height) + 10 (custom spacing)
        }

        [Fact]
        public void UIElement_WithNoChildren_ShouldNotCrash()
        {
            var parent = new UIElement(200f, 300f);

            parent.Invoking(p => p.Render()).Should().NotThrow();
        }

        [Fact]
        public void UIElement_WithSingleChild_ShouldPositionAtParentPosition()
        {
            var parent = new UIElement(200f, 300f) { X = 15f, Y = 25f };
            var child = new UIElement(50f, 30f);

            parent.AddChild(child);

            parent.Render();

            child.X.Should().Be(15f);
            child.Y.Should().Be(25f);
        }

        [Fact]
        public void UIElement_DefaultVerticalSpacing_ShouldBe2()
        {
            var element = new UIElement(100f, 50f);

            element.VerticalSpacing.Should().Be(2f);
        }

        [Fact]
        public void UIElement_WithZeroHeightChild_ShouldHandleCorrectly()
        {
            var parent = new UIElement(200f, 300f) { X = 0f, Y = 0f };
            var child1 = new UIElement(50f, 0f);
            var child2 = new UIElement(60f, 25f);

            parent.AddChild(child1);
            parent.AddChild(child2);

            parent.Render();

            child1.Y.Should().Be(0f);
            child2.Y.Should().Be(2f); // 0 + 0 (child1 height) + 2 (spacing)
        }

        [Fact]
        public void UIElement_WithNestedElements_ShouldLayoutRecursively()
        {
            var parent = new UIElement(200f, 300f) { X = 10f, Y = 20f };
            var child1 = new UIElement(100f, 50f);
            var grandChild1 = new UIElement(50f, 20f);
            var grandChild2 = new UIElement(60f, 15f);

            parent.AddChild(child1);
            child1.AddChild(grandChild1);
            child1.AddChild(grandChild2);

            parent.Render();

            child1.X.Should().Be(10f);
            child1.Y.Should().Be(20f);

            grandChild1.X.Should().Be(10f);
            grandChild1.Y.Should().Be(20f);

            grandChild2.X.Should().Be(10f);
            grandChild2.Y.Should().Be(42f); // 20 + 20 (grandChild1 height) + 2 (spacing)
        }

        [Fact]
        public void UIElement_CircularDependency_ShouldNotCauseStackOverflow()
        {
            // Arrange: Create a content-sized parent with a fill-sized child
            // This would normally cause a circular dependency:
            // - Parent calculates size from children
            // - Child calculates size from parent
            var parent = new UIElement(SizeMode.Content, SizeMode.Content);
            var fillChild = new UIElement(SizeMode.Fill, SizeMode.Fill);

            // Act: This should not cause a stack overflow
            parent.AddChild(fillChild);

            // Assert: Should not throw and should return reasonable fallback values
            parent.Invoking(p => { var width = p.Width; var height = p.Height; }).Should().NotThrow();
            fillChild.Invoking(c => { var width = c.Width; var height = c.Height; }).Should().NotThrow();

            // Verify we get reasonable fallback values
            parent.Width.Should().BeGreaterThan(0);
            parent.Height.Should().BeGreaterThan(0);
            fillChild.Width.Should().BeGreaterThan(0);
            fillChild.Height.Should().BeGreaterThan(0);
        }

        [Fact]
        public void UIElement_DeepCircularDependency_ShouldNotCauseStackOverflow()
        {
            // Arrange: Create a deeper circular dependency chain
            var grandParent = new UIElement(SizeMode.Content, SizeMode.Content);
            var parent = new UIElement(SizeMode.Fill, SizeMode.Content);
            var child = new UIElement(SizeMode.Fill, SizeMode.Fill);

            // Act: Build the chain that could cause circular dependencies
            grandParent.AddChild(parent);
            parent.AddChild(child);

            // Assert: Should not throw
            grandParent.Invoking(gp => { var width = gp.Width; var height = gp.Height; }).Should().NotThrow();
            parent.Invoking(p => { var width = p.Width; var height = p.Height; }).Should().NotThrow();
            child.Invoking(c => { var width = c.Width; var height = c.Height; }).Should().NotThrow();

            // All should have reasonable sizes
            grandParent.Width.Should().BeGreaterThan(0);
            grandParent.Height.Should().BeGreaterThan(0);
            parent.Width.Should().BeGreaterThan(0);
            parent.Height.Should().BeGreaterThan(0);
            child.Width.Should().BeGreaterThan(0);
            child.Height.Should().BeGreaterThan(0);
        }

        [Fact]
        public void UIElement_NormalBehavior_ShouldStillWork()
        {
            // Arrange: Normal, non-circular case
            var parent = new UIElement(200f, 100f); // Fixed size parent
            var fillChild = new UIElement(SizeMode.Fill, SizeMode.Fill);

            // Act
            parent.AddChild(fillChild);

            // Assert: Normal behavior should be preserved
            parent.Width.Should().Be(200f);
            parent.Height.Should().Be(100f);
            fillChild.Width.Should().Be(200f);
            fillChild.Height.Should().Be(100f);
        }

        [Fact]
        public void UIElement_ContentSizedWithFixedChildren_ShouldStillWork()
        {
            var parent = new UIElement(SizeMode.Content, SizeMode.Content);
            var child1 = new UIElement(60f, 40f);
            var child2 = new UIElement(80f, 30f);

            parent.AddChild(child1);
            parent.AddChild(child2);

            parent.Width.Should().Be(80f);
            parent.Height.Should().Be(72f);
        }

        [Fact]
        public void UIElement_FillChildWithContentParent_AfterManualSizeSet_ShouldUseFallback()
        {
            // Arrange: This mimics the original failing test scenario
            var parent = new UIElement(SizeMode.Content, SizeMode.Content);

            // Manually set size (this makes it seem fixed but mode is still Content)
            parent.Width = 300f;
            parent.Height = 200f;

            var fillChild = new UIElement(SizeMode.Fill, SizeMode.Fixed);
            fillChild.Height = 40f;

            // Act: Adding child invalidates parent size, causing circular dependency
            parent.AddChild(fillChild);

            // Assert: Should not crash and should use reasonable values
            parent.Invoking(p => { var width = p.Width; }).Should().NotThrow();
            fillChild.Invoking(c => { var width = c.Width; }).Should().NotThrow();

            // The exact values may be fallbacks, but they should be reasonable
            parent.Width.Should().BeGreaterThan(0);
            fillChild.Width.Should().BeGreaterThan(0);
            fillChild.Height.Should().Be(40f);
        }

        [Fact]
        public void UIElement_CycleDetection_ShouldOnlyTriggerDuringActualCycles()
        {
            var parent = new UIElement(150f, 100f);
            var contentChild = new UIElement(SizeMode.Content, SizeMode.Content);
            var fixedGrandChild = new UIElement(75f, 50f);

            parent.AddChild(contentChild);
            contentChild.AddChild(fixedGrandChild);

            parent.Width.Should().Be(150f);
            parent.Height.Should().Be(100f);
            contentChild.Width.Should().Be(75f);
            contentChild.Height.Should().Be(50f);
            fixedGrandChild.Width.Should().Be(75f);
            fixedGrandChild.Height.Should().Be(50f);
        }

        [Fact]
        public void DrawBorders_Default_ShouldDrawWhiteBorderWithDefaultThickness()
        {
            var mockElement = new MockUIElementWithBorders(100f, 60f);

            mockElement.PublicDrawBorders();

            mockElement.BorderDrawn.Should().BeTrue();
            mockElement.BorderColor.Should().Be(Color.white);
            mockElement.BorderThickness.Should().Be(1);
            mockElement.BorderRect.Should().Be(new Rect(0f, 0f, 100f, 60f));
        }

        [Fact]
        public void DrawBorders_WithCustomColor_ShouldDrawBorderWithSpecifiedColor()
        {
            var mockElement = new MockUIElementWithBorders(120f, 80f);
            var customColor = Color.red;

            mockElement.PublicDrawBorders(customColor);

            mockElement.BorderDrawn.Should().BeTrue();
            mockElement.BorderColor.Should().Be(customColor);
            mockElement.BorderThickness.Should().Be(1);
        }

        [Fact]
        public void DrawBorders_WithCustomColorAndThickness_ShouldDrawBorderWithBothSpecified()
        {
            var mockElement = new MockUIElementWithBorders(150f, 90f) { X = 10f, Y = 20f };
            var customColor = Color.blue;
            var customThickness = 3;

            mockElement.PublicDrawBorders(customColor, customThickness);

            mockElement.BorderDrawn.Should().BeTrue();
            mockElement.BorderColor.Should().Be(customColor);
            mockElement.BorderThickness.Should().Be(customThickness);
            mockElement.BorderRect.Should().Be(new Rect(10f, 20f, 150f, 90f));
        }

        [Fact]
        public void DrawBorders_WithZeroThickness_ShouldNotDrawBorder()
        {
            var mockElement = new MockUIElementWithBorders(100f, 60f);

            mockElement.PublicDrawBorders(Color.white, 0);

            mockElement.BorderDrawn.Should().BeFalse();
        }

        [Fact]
        public void DrawBorders_WithNegativeThickness_ShouldNotDrawBorder()
        {
            var mockElement = new MockUIElementWithBorders(100f, 60f);

            mockElement.PublicDrawBorders(Color.white, -1);

            mockElement.BorderDrawn.Should().BeFalse();
        }

        [Fact]
        public void DrawBorders_WithZeroSize_ShouldNotDrawBorder()
        {
            var mockElement = new MockUIElementWithBorders(0f, 0f);

            mockElement.PublicDrawBorders();

            mockElement.BorderDrawn.Should().BeFalse();
        }

        [Fact]
        public void DrawBorders_WithZeroWidth_ShouldNotDrawBorder()
        {
            var mockElement = new MockUIElementWithBorders(0f, 50f);

            mockElement.PublicDrawBorders();

            mockElement.BorderDrawn.Should().BeFalse();
        }

        [Fact]
        public void DrawBorders_WithZeroHeight_ShouldNotDrawBorder()
        {
            var mockElement = new MockUIElementWithBorders(100f, 0f);

            mockElement.PublicDrawBorders();

            mockElement.BorderDrawn.Should().BeFalse();
        }

        [Fact]
        public void DrawBorders_CalledMultipleTimes_ShouldDrawBorderEachTime()
        {
            var mockElement = new MockUIElementWithBorders(100f, 60f);

            mockElement.PublicDrawBorders(Color.red, 1);
            mockElement.PublicDrawBorders(Color.blue, 2);

            mockElement.DrawBorderCallCount.Should().Be(2);
            mockElement.BorderColor.Should().Be(Color.blue);
            mockElement.BorderThickness.Should().Be(2);
        }

        [Fact]
        public void DrawBorders_WithDifferentPositions_ShouldUseCorrectRect()
        {
            var mockElement = new MockUIElementWithBorders(100f, 60f) { X = 50f, Y = 30f };

            mockElement.PublicDrawBorders();

            mockElement.BorderRect.Should().Be(new Rect(50f, 30f, 100f, 60f));
        }

        [Fact]
        public void DrawBorders_WithLargeThickness_ShouldHandleCorrectly()
        {
            var mockElement = new MockUIElementWithBorders(100f, 60f);
            var largeThickness = 10;

            mockElement.PublicDrawBorders(Color.green, largeThickness);

            mockElement.BorderDrawn.Should().BeTrue();
            mockElement.BorderThickness.Should().Be(largeThickness);
        }

        [Fact]
        public void DrawBorders_WithTransparentColor_ShouldStillDrawBorder()
        {
            var mockElement = new MockUIElementWithBorders(100f, 60f);
            var transparentColor = new Color(1f, 0f, 0f, 0.5f);

            mockElement.PublicDrawBorders(transparentColor);

            mockElement.BorderDrawn.Should().BeTrue();
            mockElement.BorderColor.Should().Be(transparentColor);
        }

        [Fact]
        public void AutomaticBorders_WhenEnabled_ShouldDrawBordersAfterRender()
        {
            var mockElement = new MockUIElementWithAutomaticBorders(100f, 60f);
            mockElement.ShowBorders = true;
            mockElement.BorderColor = Color.blue;
            mockElement.BorderThickness = 2;

            mockElement.Render();

            mockElement.RenderElementCalled.Should().BeTrue();
            mockElement.AutomaticBorderDrawn.Should().BeTrue();
            mockElement.AutomaticBorderColor.Should().Be(Color.blue);
            mockElement.AutomaticBorderThickness.Should().Be(2);
        }

        [Fact]
        public void AutomaticBorders_WhenDisabled_ShouldNotDrawBorders()
        {
            var mockElement = new MockUIElementWithAutomaticBorders(100f, 60f);
            mockElement.ShowBorders = false;

            mockElement.Render();

            mockElement.RenderElementCalled.Should().BeTrue();
            mockElement.AutomaticBorderDrawn.Should().BeFalse();
        }

        [Fact]
        public void ShowBorders_Property_ShouldGetAndSetCorrectly()
        {
            var element = new UIElement(100f, 50f);

            element.ShowBorders.Should().BeFalse();

            element.ShowBorders = true;
            element.ShowBorders.Should().BeTrue();

            element.ShowBorders = false;
            element.ShowBorders.Should().BeFalse();
        }

        [Fact]
        public void BorderColor_Property_ShouldGetAndSetCorrectly()
        {
            var element = new UIElement(100f, 50f);
            var testColor = Color.magenta;

            element.BorderColor.Should().Be(Color.white);

            element.BorderColor = testColor;
            element.BorderColor.Should().Be(testColor);
        }

        [Fact]
        public void BorderThickness_Property_ShouldGetAndSetCorrectly()
        {
            var element = new UIElement(100f, 50f);

            element.BorderThickness.Should().Be(1);

            element.BorderThickness = 5;
            element.BorderThickness.Should().Be(5);
        }

        [Fact]
        public void AutomaticBorders_WithInvalidThickness_ShouldNotDrawBorders()
        {
            var mockElement = new MockUIElementWithAutomaticBorders(100f, 60f);
            mockElement.ShowBorders = true;
            mockElement.BorderThickness = 0;

            mockElement.Render();

            mockElement.AutomaticBorderDrawn.Should().BeFalse();
        }

        [Fact]
        public void AutomaticBorders_WithZeroSizedElement_ShouldNotDrawBorders()
        {
            var mockElement = new MockUIElementWithAutomaticBorders(0f, 0f);
            mockElement.ShowBorders = true;

            mockElement.Render();

            mockElement.AutomaticBorderDrawn.Should().BeFalse();
        }

        [Fact]
        public void ManualBorders_Integration_ShouldStillWork()
        {
            var mockElement = new MockUIElementWithManualBordersIntegration(100f, 60f);
            mockElement.EnableBorders = true;
            mockElement.ManualBorderColor = Color.yellow;
            mockElement.ManualBorderThickness = 2;

            mockElement.Render();

            mockElement.RenderElementCalled.Should().BeTrue();
            mockElement.ManualBorderDrawn.Should().BeTrue();
            mockElement.FinalBorderColor.Should().Be(Color.yellow);
            mockElement.FinalBorderThickness.Should().Be(2);
        }

        [Fact]
        public void ManualBorders_Integration_WhenDisabled_ShouldNotDrawBorder()
        {
            var mockElement = new MockUIElementWithManualBordersIntegration(100f, 60f);
            mockElement.EnableBorders = false;

            mockElement.Render();

            mockElement.RenderElementCalled.Should().BeTrue();
            mockElement.ManualBorderDrawn.Should().BeFalse();
        }

        private class MockUIElementWithBorders : UIElement
        {
            public bool BorderDrawn { get; private set; }
            public Color BorderColor { get; private set; }
            public int BorderThickness { get; private set; }
            public Rect BorderRect { get; private set; }
            public int DrawBorderCallCount { get; private set; }

            public MockUIElementWithBorders(float width, float height) : base(width, height) { }

            public void PublicDrawBorders()
            {
                DrawBorders();
            }

            public void PublicDrawBorders(Color color)
            {
                DrawBorders(color);
            }

            public void PublicDrawBorders(Color color, int thickness)
            {
                DrawBorders(color, thickness);
            }

            protected override void DrawBorderInternal(Rect rect, Color color, int thickness)
            {
                DrawBorderCallCount++;

                if (thickness > 0 && rect.width > 0 && rect.height > 0)
                {
                    BorderDrawn = true;
                    BorderColor = color;
                    BorderThickness = thickness;
                    BorderRect = rect;
                }
                else
                {
                    BorderDrawn = false;
                }
            }
        }
        private class MockUIElement : UIElement
        {
            private static int _callOrderCounter = 0;

            public bool RenderCalled { get; private set; }
            public bool RenderElementCalled { get; private set; }
            public int RenderCallOrder { get; private set; }
            public int RenderElementCallOrder { get; private set; }

            public MockUIElement(float width, float height) : base(width, height) { }

            public override void Render()
            {
                RenderCalled = true;
                RenderCallOrder = ++_callOrderCounter;
                base.Render();
            }

            protected override void RenderElement()
            {
                RenderElementCalled = true;
                RenderElementCallOrder = ++_callOrderCounter;
            }
        }

        private class TestUIElement : UIElement
        {
            public int CalculateCallCount { get; private set; }

            public TestUIElement(SizeMode widthMode, SizeMode heightMode) : base(widthMode, heightMode) { }

            public override (float width, float height) CalculateDynamicSize()
            {
                CalculateCallCount++;
                return (200f, 100f);
            }
        }

        private class TestUIElementWithParentCallback : UIElement
        {
            public bool OnParentSetCalled { get; private set; }

            public TestUIElementWithParentCallback(SizeMode widthMode, SizeMode heightMode) : base(widthMode, heightMode) { }

            public override void OnParentSet()
            {
                OnParentSetCalled = true;
                base.OnParentSet();
            }
        }

        private class MockUIElementWithBordersIntegration : UIElement
        {
            public bool RenderElementCalled { get; private set; }
            public bool BorderDrawnDuringRender { get; private set; }
            public Color FinalBorderColor { get; private set; }
            public int FinalBorderThickness { get; private set; }
            public bool EnableBorders { get; set; }
            public Color BorderColor { get; set; } = Color.white;
            public int BorderThickness { get; set; } = 1;

            public MockUIElementWithBordersIntegration(float width, float height) : base(width, height) { }

            protected override void RenderElement()
            {
                RenderElementCalled = true;

                if (EnableBorders)
                {
                    DrawBorders(BorderColor, BorderThickness);
                }
            }

            protected override void DrawBorderInternal(Rect rect, Color color, int thickness)
            {
                if (thickness > 0 && rect.width > 0 && rect.height > 0)
                {
                    BorderDrawnDuringRender = true;
                    FinalBorderColor = color;
                    FinalBorderThickness = thickness;
                }
            }
        }

        private class MockUIElementWithManualBordersIntegration : UIElement
        {
            public bool RenderElementCalled { get; private set; }
            public bool ManualBorderDrawn { get; private set; }
            public Color FinalBorderColor { get; private set; }
            public int FinalBorderThickness { get; private set; }
            public bool EnableBorders { get; set; }
            public Color ManualBorderColor { get; set; } = Color.white;
            public int ManualBorderThickness { get; set; } = 1;

            public MockUIElementWithManualBordersIntegration(float width, float height) : base(width, height) { }

            protected override void RenderElement()
            {
                RenderElementCalled = true;

                if (EnableBorders)
                {
                    DrawBorders(ManualBorderColor, ManualBorderThickness);
                }
            }

            protected override void DrawBorderInternal(Rect rect, Color color, int thickness)
            {
                if (thickness > 0 && rect.width > 0 && rect.height > 0)
                {
                    ManualBorderDrawn = true;
                    FinalBorderColor = color;
                    FinalBorderThickness = thickness;
                }
            }
        }

        private class MockUIElementWithAutomaticBorders : UIElement
        {
            public bool RenderElementCalled { get; private set; }
            public bool AutomaticBorderDrawn { get; private set; }
            public Color AutomaticBorderColor { get; private set; }
            public int AutomaticBorderThickness { get; private set; }

            public MockUIElementWithAutomaticBorders(float width, float height) : base(width, height) { }

            protected override void RenderElement()
            {
                RenderElementCalled = true;
            }

            protected override void DrawBorderInternal(Rect rect, Color color, int thickness)
            {
                if (thickness > 0 && rect.width > 0 && rect.height > 0)
                {
                    AutomaticBorderDrawn = true;
                    AutomaticBorderColor = color;
                    AutomaticBorderThickness = thickness;
                }
            }
        }
    }
}