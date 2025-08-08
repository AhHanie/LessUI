using FluentAssertions;
using System.Runtime.CompilerServices;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    public class ScrollContainerStrongBoxTests
    {

        [Fact]
        public void ScrollContainer_Constructor_WithSharedStrongBox_ShouldUseSharedScrollPosition()
        {
            var sharedScrollBox = new StrongBox<Vector2>(new Vector2(50f, 100f));

            var scrollContainer = new ScrollContainer(400f, 300f, sharedScrollBox);

            scrollContainer.ScrollPosition.Should().Be(new Vector2(50f, 100f));
            scrollContainer.ScrollPositionBox.Should().BeSameAs(sharedScrollBox);
            scrollContainer.ScrollPositionBox.Value.Should().Be(new Vector2(50f, 100f));
        }

        [Fact]
        public void ScrollContainer_Constructor_WithNullStrongBox_ShouldCreateNewStrongBox()
        {
            var scrollContainer = new ScrollContainer(400f, 300f, (StrongBox<Vector2>)null);

            scrollContainer.ScrollPositionBox.Should().NotBeNull();
            scrollContainer.ScrollPosition.Should().Be(Vector2.zero);
        }

        [Fact]
        public void ScrollContainer_ScrollPosition_PropertyGetter_ShouldReturnStrongBoxValue()
        {
            var testPosition = new Vector2(10f, 20f);
            var box = new StrongBox<Vector2>(testPosition);
            var scrollContainer = new ScrollContainer(300f, 200f, box);
            

            scrollContainer.ScrollPositionBox.Value = testPosition;

            scrollContainer.ScrollPosition.Should().Be(testPosition);
        }

        [Fact]
        public void ScrollContainer_SharedStrongBox_ShouldSynchronizeBetweenContainers()
        {
            var sharedScrollBox = new StrongBox<Vector2>(Vector2.zero);
            var container1 = new ScrollContainer(400f, 300f, sharedScrollBox);
            var container2 = new ScrollContainer(400f, 300f, sharedScrollBox);

            container1.ScrollPosition = new Vector2(100f, 150f);

            container2.ScrollPosition.Should().Be(new Vector2(100f, 150f));
            container1.ScrollPositionBox.Should().BeSameAs(container2.ScrollPositionBox);
            sharedScrollBox.Value.Should().Be(new Vector2(100f, 150f));
        }

        [Fact]
        public void ScrollContainer_IndependentStrongBoxes_ShouldHaveIndependentScrollPositions()
        {
            var box = new StrongBox<Vector2>(new Vector2(50f, 75f));
            var box1 = new StrongBox<Vector2>(new Vector2(100f, 125f));
            var container1 = new ScrollContainer(400f, 300f, box);
            var container2 = new ScrollContainer(400f, 300f, box1);

            

            container1.ScrollPosition.Should().Be(new Vector2(50f, 75f));
            container2.ScrollPosition.Should().Be(new Vector2(100f, 125f));
            container1.ScrollPositionBox.Should().NotBeSameAs(container2.ScrollPositionBox);
        }

        [Fact]
        public void ScrollContainer_Constructor_WithParent_ShouldSetParentCorrectly()
        {
            var parent = new UIElement(500f, 400f);

            var scrollContainer = new ScrollContainer(parent, 300f, 200f);

            scrollContainer.Parent.Should().Be(parent);
            scrollContainer.Width.Should().Be(300f);
            scrollContainer.Height.Should().Be(200f);
            parent.Children.Should().Contain(scrollContainer);
            scrollContainer.ScrollPositionBox.Should().NotBeNull();
        }

        [Fact]
        public void ScrollContainer_Constructor_WithParentAndSharedStrongBox_ShouldSetBothCorrectly()
        {
            var parent = new UIElement(500f, 400f);
            var sharedScrollBox = new StrongBox<Vector2>(new Vector2(30f, 40f));

            var scrollContainer = new ScrollContainer(parent, 300f, 200f, sharedScrollBox);

            scrollContainer.Parent.Should().Be(parent);
            scrollContainer.ScrollPositionBox.Should().BeSameAs(sharedScrollBox);
            scrollContainer.ScrollPosition.Should().Be(new Vector2(30f, 40f));
            parent.Children.Should().Contain(scrollContainer);
        }

        [Fact]
        public void ScrollContainer_ShowScrollbars_ShouldGetAndSetCorrectly()
        {
            var box = new StrongBox<Vector2>(new Vector2(50f, 75f));
            var scrollContainer = new ScrollContainer(300f, 200f, box);

            scrollContainer.ShowScrollbars = false;
            scrollContainer.ShowScrollbars.Should().BeFalse();

            scrollContainer.ShowScrollbars = true;
            scrollContainer.ShowScrollbars.Should().BeTrue();
        }

        [Fact]
        public void ScrollContainer_Padding_ShouldGetAndSetCorrectly()
        {
            var box = new StrongBox<Vector2>(new Vector2(50f, 75f));
            var scrollContainer = new ScrollContainer(300f, 200f, box);

            scrollContainer.Padding = 15f;

            scrollContainer.Padding.Should().Be(15f);
        }

        [Fact]
        public void ScrollContainer_CalculateScrollRect_WithPadding_ShouldInsetCorrectly()
        {
            var box = new StrongBox<Vector2>(new Vector2(50f, 75f));
            var scrollContainer = new ScrollContainer(300f, 200f, box) { X = 10f, Y = 20f, Padding = 15f };

            var scrollRect = scrollContainer.CalculateScrollRect();

            scrollRect.x.Should().Be(25f); // 10 + 15
            scrollRect.y.Should().Be(35f); // 20 + 15
            scrollRect.width.Should().Be(270f); // 300 - (2 * 15)
            scrollRect.height.Should().Be(170f); // 200 - (2 * 15)
        }

        [Fact]
        public void ScrollContainer_WithContentMode_ShouldCalculateSizeFromChildren()
        {
            var box = new StrongBox<Vector2>(new Vector2(50f, 75f));
            var scrollContainer = new ScrollContainer(SizeMode.Content, SizeMode.Content, box);
            var child1 = new UIElement(100f, 80f);
            var child2 = new UIElement(150f, 60f);

            scrollContainer.AddChild(child1);
            scrollContainer.AddChild(child2);

            scrollContainer.Width.Should().Be(150f);
            scrollContainer.Height.Should().Be(142f); // 80 + 60 + 2 (spacing)
        }

        [Fact]
        public void ScrollContainer_ExternalStrongBoxModification_ShouldReflectInScrollPosition()
        {
            var externalScrollBox = new StrongBox<Vector2>(Vector2.zero);
            var scrollContainer = new ScrollContainer(400f, 300f, externalScrollBox);

            // Modify through external reference
            externalScrollBox.Value = new Vector2(200f, 300f);

            scrollContainer.ScrollPosition.Should().Be(new Vector2(200f, 300f));
        }

        [Fact]
        public void ScrollContainer_MultipleRendersWithSharedStrongBox_ShouldMaintainState()
        {
            var sharedScrollBox = new StrongBox<Vector2>(Vector2.zero);
            var container1 = new ScrollContainer(400f, 300f, sharedScrollBox);
            var container2 = new ScrollContainer(400f, 300f, sharedScrollBox);

            // Add content to both containers - FIXED: Use StrongBox<string> for Label constructor
            for (int i = 0; i < 20; i++)
            {
                container1.AddChild(new Label(new StrongBox<string>($"Container1 Item {i}")));
                container2.AddChild(new Label(new StrongBox<string>($"Container2 Item {i}")));
            }

            // Simulate Unity updating scroll position during render
            var simulatedScrollPosition = new Vector2(0f, 150f);
            sharedScrollBox.Value = simulatedScrollPosition;

            // Both containers should reflect the same scroll position
            container1.ScrollPosition.Should().Be(simulatedScrollPosition);
            container2.ScrollPosition.Should().Be(simulatedScrollPosition);

            // Modify through one container
            container1.ScrollPosition = new Vector2(50f, 200f);

            // Should be reflected in both containers and the shared box
            container2.ScrollPosition.Should().Be(new Vector2(50f, 200f));
            sharedScrollBox.Value.Should().Be(new Vector2(50f, 200f));
        }

        [Fact]
        public void ScrollContainer_StrongBoxType_ShouldBeCorrectType()
        {
            var box = new StrongBox<Vector2>(new Vector2(50f, 75f));
            var scrollContainer = new ScrollContainer(300f, 200f, box);

            scrollContainer.ScrollPositionBox.Should().BeOfType<StrongBox<Vector2>>();
            scrollContainer.ScrollPositionBox.Value.Should().BeOfType<Vector2>();
        }

        [Fact]
        public void ScrollContainer_StrongBoxReference_ShouldAllowDirectManipulation()
        {
            var box = new StrongBox<Vector2>(new Vector2(50f, 75f));
            var scrollContainer = new ScrollContainer(300f, 200f, box);
            var strongBoxRef = scrollContainer.ScrollPositionBox;

            // Direct manipulation through the StrongBox reference
            strongBoxRef.Value = new Vector2(75f, 125f);

            scrollContainer.ScrollPosition.Should().Be(new Vector2(75f, 125f));

            // Property manipulation should also update the StrongBox
            scrollContainer.ScrollPosition = new Vector2(100f, 200f);
            strongBoxRef.Value.Should().Be(new Vector2(100f, 200f));
        }

        [Fact]
        public void ScrollContainer_WithSizeModesAndSharedStrongBox_ShouldInitializeCorrectly()
        {
            var sharedScrollBox = new StrongBox<Vector2>(new Vector2(25f, 50f));

            var scrollContainer = new ScrollContainer(SizeMode.Fill, SizeMode.Content, sharedScrollBox);

            scrollContainer.WidthMode.Should().Be(SizeMode.Fill);
            scrollContainer.HeightMode.Should().Be(SizeMode.Content);
            scrollContainer.ScrollPosition.Should().Be(new Vector2(25f, 50f));
            scrollContainer.ScrollPositionBox.Should().BeSameAs(sharedScrollBox);
        }
    }
}