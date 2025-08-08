using FluentAssertions;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    public class LineTests
    {
        [Fact]
        public void Constructor_WithContentSize_ShouldInitializeWithCorrectValues()
        {
            var line = new Line(LineType.Horizontal);

            line.LineType.Should().Be(LineType.Horizontal);
            line.Thickness.Should().Be(1f);
            line.WidthMode.Should().Be(SizeMode.Content);
            line.HeightMode.Should().Be(SizeMode.Content);
            line.Alignment.Should().Be(Align.UpperLeft);
        }

        [Fact]
        public void Constructor_WithVerticalType_ShouldInitializeCorrectly()
        {
            var line = new Line(LineType.Vertical);

            line.LineType.Should().Be(LineType.Vertical);
            line.Thickness.Should().Be(1f);
        }

        [Fact]
        public void Constructor_WithThickness_ShouldInitializeWithCustomThickness()
        {
            var line = new Line(LineType.Horizontal, 3f);

            line.LineType.Should().Be(LineType.Horizontal);
            line.Thickness.Should().Be(3f);
        }

        [Fact]
        public void Constructor_WithFixedSize_ShouldInitializeWithCorrectValues()
        {
            var line = new Line(LineType.Vertical, 2f, 100f, 200f);

            line.LineType.Should().Be(LineType.Vertical);
            line.Thickness.Should().Be(2f);
            line.Width.Should().Be(100f);
            line.Height.Should().Be(200f);
            line.WidthMode.Should().Be(SizeMode.Fixed);
            line.HeightMode.Should().Be(SizeMode.Fixed);
        }

        [Fact]
        public void Constructor_WithFillMode_ShouldInitializeCorrectly()
        {
            var line = new Line(LineType.Horizontal, SizeMode.Fill);

            line.LineType.Should().Be(LineType.Horizontal);
            line.WidthMode.Should().Be(SizeMode.Fill);
            line.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Constructor_WithParent_ShouldSetParentCorrectly()
        {
            var parent = new UIElement(300f, 200f);
            var line = new Line(parent, LineType.Horizontal);

            line.Parent.Should().Be(parent);
            line.LineType.Should().Be(LineType.Horizontal);
            parent.Children.Should().Contain(line);
        }

        [Fact]
        public void Constructor_WithOptions_ShouldApplyOptions()
        {
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };
            var line = new Line(LineType.Vertical, 1f, options);

            line.Alignment.Should().Be(Align.MiddleCenter);
        }

        [Fact]
        public void Constructor_WithParentAndFillMode_ShouldSetParentAndFillMode()
        {
            var parent = new UIElement(300f, 200f);
            var line = new Line(parent, LineType.Horizontal, SizeMode.Fill);

            line.Parent.Should().Be(parent);
            line.WidthMode.Should().Be(SizeMode.Fill);
            parent.Children.Should().Contain(line);
        }

        [Fact]
        public void Constructor_WithParentFillModeAndOptions_ShouldApplyAllSettings()
        {
            var parent = new UIElement(300f, 200f);
            var options = new UIElementOptions { Alignment = Align.LowerRight };
            var line = new Line(parent, LineType.Vertical, SizeMode.Fill, 1f, options);

            line.Parent.Should().Be(parent);
            line.WidthMode.Should().Be(SizeMode.Fill);
            line.Alignment.Should().Be(Align.LowerRight);
            parent.Children.Should().Contain(line);
        }

        [Fact]
        public void LineType_Property_ShouldGetAndSetCorrectly()
        {
            var line = new Line(LineType.Horizontal);

            line.LineType = LineType.Vertical;

            line.LineType.Should().Be(LineType.Vertical);
        }

        [Fact]
        public void Thickness_Property_ShouldGetAndSetCorrectly()
        {
            var line = new Line(LineType.Horizontal);
            float newThickness = 5f;

            line.Thickness = newThickness;

            line.Thickness.Should().Be(newThickness);
        }

        [Fact]
        public void Thickness_WithZeroValue_ShouldClampToMinimum()
        {
            var line = new Line(LineType.Horizontal);

            line.Thickness = 0f;

            line.Thickness.Should().BeGreaterThan(0f);
        }

        [Fact]
        public void Thickness_WithNegativeValue_ShouldClampToMinimum()
        {
            var line = new Line(LineType.Horizontal);

            line.Thickness = -2f;

            line.Thickness.Should().BeGreaterThan(0f);
        }

        [Fact]
        public void Color_Property_ShouldGetAndSetCorrectly()
        {
            var line = new Line(LineType.Horizontal);
            var newColor = Color.red;

            line.Color = newColor;

            line.Color.Should().Be(newColor);
        }

        [Fact]
        public void Color_DefaultValue_ShouldBeWhite()
        {
            var line = new Line(LineType.Horizontal);

            line.Color.Should().Be(Color.white);
        }

        [Fact]
        public void Tooltip_Property_ShouldGetAndSetCorrectly()
        {
            var line = new Line(LineType.Horizontal);
            string tooltip = "This is a line";

            line.Tooltip = tooltip;

            line.Tooltip.Should().Be(tooltip);
        }

        [Fact]
        public void Width_WithFillMode_ShouldFillParentWidth()
        {
            var parent = new UIElement(400f, 200f);
            var line = new Line(LineType.Horizontal, SizeMode.Fill);
            parent.AddChild(line);

            line.Width.Should().Be(400f);
        }

        [Fact]
        public void Width_WithFillMode_WithoutParent_ShouldReturnZero()
        {
            var line = new Line(LineType.Horizontal, SizeMode.Fill);

            line.Width.Should().Be(0f);
        }

        [Fact]
        public void CalculateDynamicSize_HorizontalLine_ShouldReturnCorrectSize()
        {
            var line = new Line(LineType.Horizontal, 2f);

            var size = line.CalculateDynamicSize();

            size.width.Should().Be(100f);
            size.height.Should().Be(2f);
        }

        [Fact]
        public void CalculateDynamicSize_VerticalLine_ShouldReturnCorrectSize()
        {
            var line = new Line(LineType.Vertical, 3f);

            var size = line.CalculateDynamicSize();

            size.width.Should().Be(3f);
            size.height.Should().Be(100f);
        }

        [Fact]
        public void CreateRect_ShouldReturnCorrectRect()
        {
            var line = new Line(LineType.Horizontal, 2f, 150f, 25f);
            line.X = 10f;
            line.Y = 20f;

            var rect = line.CreateRect();

            rect.x.Should().Be(10f);
            rect.y.Should().Be(20f);
            rect.width.Should().Be(150f);
            rect.height.Should().Be(25f);
        }

        [Fact]
        public void Render_ShouldCallRenderElement()
        {
            var line = new MockLine(LineType.Horizontal);

            line.Render();

            line.RenderElementCalled.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_HorizontalLine_ShouldCallDrawLineHorizontal()
        {
            var line = new MockLine(LineType.Horizontal, 2f);
            line.X = 10f;
            line.Y = 20f;
            line.Width = 150f;
            line.Height = 2f;

            line.PublicRenderElement();

            line.DrawLineHorizontalCalled.Should().BeTrue();
            line.DrawLineVerticalCalled.Should().BeFalse();
            line.RenderedX.Should().Be(10f);
            line.RenderedY.Should().Be(21f);
            line.RenderedWidth.Should().Be(150f);
        }

        [Fact]
        public void RenderElement_VerticalLine_ShouldCallDrawLineVertical()
        {
            var line = new MockLine(LineType.Vertical, 3f);
            line.X = 15f;
            line.Y = 25f;
            line.Width = 3f;
            line.Height = 120f;

            line.PublicRenderElement();

            line.DrawLineVerticalCalled.Should().BeTrue();
            line.DrawLineHorizontalCalled.Should().BeFalse();
            line.RenderedX.Should().Be(16.5f);
            line.RenderedY.Should().Be(25f);
            line.RenderedHeight.Should().Be(120f);
        }

        [Fact]
        public void RenderElement_WithTooltip_ShouldSetTooltipRegion()
        {
            var line = new MockLine(LineType.Horizontal);
            line.Tooltip = "Test tooltip";

            line.PublicRenderElement();

            line.TooltipSet.Should().Be("Test tooltip");
        }

        [Fact]
        public void RenderElement_WithoutTooltip_ShouldNotSetTooltipRegion()
        {
            var line = new MockLine(LineType.Horizontal);
            line.Tooltip = null;

            line.PublicRenderElement();

            line.TooltipSet.Should().BeNull();
        }

        [Fact]
        public void RenderElement_WithCustomColor_ShouldSetColor()
        {
            var line = new MockLine(LineType.Horizontal);
            line.Color = Color.red;

            line.PublicRenderElement();

            line.ColorSet.Should().Be(Color.red);
        }

        [Fact]
        public void RenderElement_HorizontalLineWithUpperLeftAlignment_ShouldPositionAtTop()
        {
            var line = new MockLine(LineType.Horizontal, 2f);
            line.X = 10f;
            line.Y = 20f;
            line.Width = 100f;
            line.Height = 50f;
            line.Alignment = Align.UpperLeft;

            line.PublicRenderElement();

            line.RenderedY.Should().Be(21f);
        }

        [Fact]
        public void RenderElement_HorizontalLineWithMiddleCenterAlignment_ShouldPositionAtCenter()
        {
            var line = new MockLine(LineType.Horizontal, 2f);
            line.X = 10f;
            line.Y = 20f;
            line.Width = 100f;
            line.Height = 50f;
            line.Alignment = Align.MiddleCenter;

            line.PublicRenderElement();

            line.RenderedY.Should().Be(45f);
        }

        [Fact]
        public void RenderElement_HorizontalLineWithLowerRightAlignment_ShouldPositionAtBottom()
        {
            var line = new MockLine(LineType.Horizontal, 2f);
            line.X = 10f;
            line.Y = 20f;
            line.Width = 100f;
            line.Height = 50f;
            line.Alignment = Align.LowerRight;

            line.PublicRenderElement();

            line.RenderedY.Should().Be(69f);
        }

        [Fact]
        public void RenderElement_VerticalLineWithUpperLeftAlignment_ShouldPositionAtLeft()
        {
            var line = new MockLine(LineType.Vertical, 3f);
            line.X = 10f;
            line.Y = 20f;
            line.Width = 100f;
            line.Height = 50f;
            line.Alignment = Align.UpperLeft;

            line.PublicRenderElement();

            line.RenderedX.Should().Be(11.5f);
        }

        [Fact]
        public void RenderElement_VerticalLineWithMiddleCenterAlignment_ShouldPositionAtCenter()
        {
            var line = new MockLine(LineType.Vertical, 3f);
            line.X = 10f;
            line.Y = 20f;
            line.Width = 100f;
            line.Height = 50f;
            line.Alignment = Align.MiddleCenter;

            line.PublicRenderElement();

            line.RenderedX.Should().Be(58.5f);
        }

        [Fact]
        public void RenderElement_VerticalLineWithUpperRightAlignment_ShouldPositionAtRight()
        {
            var line = new MockLine(LineType.Vertical, 3f);
            line.X = 10f;
            line.Y = 20f;
            line.Width = 100f;
            line.Height = 50f;
            line.Alignment = Align.UpperRight;

            line.PublicRenderElement();

            line.RenderedX.Should().Be(108.5f);
        }

        [Fact]
        public void LineType_Enum_ShouldHaveCorrectValues()
        {
            LineType.Horizontal.Should().Be(LineType.Horizontal);
            LineType.Vertical.Should().Be(LineType.Vertical);
        }

        private class MockLine : Line
        {
            public bool RenderElementCalled { get; private set; }
            public bool DrawLineHorizontalCalled { get; private set; }
            public bool DrawLineVerticalCalled { get; private set; }
            public float RenderedX { get; private set; }
            public float RenderedY { get; private set; }
            public float RenderedWidth { get; private set; }
            public float RenderedHeight { get; private set; }
            public string TooltipSet { get; private set; }
            public Color ColorSet { get; private set; }

            public MockLine(LineType lineType, float thickness = 1f) : base(lineType, thickness) { }

            public void PublicRenderElement()
            {
                RenderElement();
            }

            protected override void RenderElement()
            {
                RenderElementCalled = true;

                var rect = CreateRect();
                ColorSet = Color;

                if (LineType == LineType.Horizontal)
                {
                    DrawLineHorizontalCalled = true;
                    var lineY = CalculateHorizontalLineY(rect);
                    RenderedX = rect.x;
                    RenderedY = lineY;
                    RenderedWidth = rect.width;
                }
                else
                {
                    DrawLineVerticalCalled = true;
                    var lineX = CalculateVerticalLineX(rect);
                    RenderedX = lineX;
                    RenderedY = rect.y;
                    RenderedHeight = rect.height;
                }

                if (!string.IsNullOrEmpty(Tooltip))
                {
                    TooltipSet = Tooltip;
                }
            }
        }
    }
}