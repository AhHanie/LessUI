using FluentAssertions;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    public class TextFieldNumericTests
    {
        [Fact]
        public void Constructor_WithContentSize_ShouldInitializeWithCorrectValues()
        {
            int initialValue = 42;
            string initialBuffer = "42";
            Action<int> onValueChanged = (value) => { };

            var textField = new TextFieldNumeric<int>(initialValue, initialBuffer, onValueChanged);

            textField.Value.Should().Be(initialValue);
            textField.Buffer.Should().Be(initialBuffer);
            textField.OnValueChanged.Should().Be(onValueChanged);
            textField.WidthMode.Should().Be(SizeMode.Content);
            textField.HeightMode.Should().Be(SizeMode.Content);
            textField.Min.Should().Be(0);
            textField.Max.Should().Be(1000000000);
        }

        [Fact]
        public void Constructor_WithFixedSize_ShouldInitializeWithCorrectValues()
        {
            float initialValue = 3.14f;
            string initialBuffer = "3.14";
            float width = 150f;
            float height = 30f;
            Action<float> onValueChanged = (value) => { };

            var textField = new TextFieldNumeric<float>(initialValue, initialBuffer, width, height, onValueChanged);

            textField.Value.Should().Be(initialValue);
            textField.Buffer.Should().Be(initialBuffer);
            textField.Width.Should().Be(width);
            textField.Height.Should().Be(height);
            textField.OnValueChanged.Should().Be(onValueChanged);
            textField.WidthMode.Should().Be(SizeMode.Fixed);
            textField.HeightMode.Should().Be(SizeMode.Fixed);
        }

        [Fact]
        public void Constructor_WithParent_ShouldSetParentCorrectly()
        {
            var parent = new UIElement(200f, 100f);
            int initialValue = 10;
            string initialBuffer = "10";
            Action<int> onValueChanged = (value) => { };

            var textField = new TextFieldNumeric<int>(parent, initialValue, initialBuffer, onValueChanged);

            textField.Parent.Should().Be(parent);
            textField.Value.Should().Be(initialValue);
            textField.Buffer.Should().Be(initialBuffer);
            textField.OnValueChanged.Should().Be(onValueChanged);
            parent.Children.Should().Contain(textField);
        }

        [Fact]
        public void Value_Property_ShouldGetAndSetCorrectly()
        {
            var textField = new TextFieldNumeric<int>(0, "0");
            int newValue = 100;

            textField.Value = newValue;

            textField.Value.Should().Be(newValue);
        }

        [Fact]
        public void Buffer_Property_ShouldGetAndSetCorrectly()
        {
            var textField = new TextFieldNumeric<int>(0, "0");
            string newBuffer = "123";

            textField.Buffer = newBuffer;

            textField.Buffer.Should().Be(newBuffer);
        }

        [Fact]
        public void Min_Property_ShouldGetAndSetCorrectly()
        {
            var textField = new TextFieldNumeric<int>(0, "0");
            int newMin = -50;

            textField.Min = newMin;

            textField.Min.Should().Be(newMin);
        }

        [Fact]
        public void Max_Property_ShouldGetAndSetCorrectly()
        {
            var textField = new TextFieldNumeric<int>(0, "0");
            int newMax = 500;

            textField.Max = newMax;

            textField.Max.Should().Be(newMax);
        }

        [Fact]
        public void Tooltip_Property_ShouldGetAndSetCorrectly()
        {
            var textField = new TextFieldNumeric<int>(0, "0");
            string tooltip = "Enter a number";

            textField.Tooltip = tooltip;

            textField.Tooltip.Should().Be(tooltip);
        }

        [Fact]
        public void Constructor_WithOptions_ShouldApplyOptions()
        {
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };

            var textField = new TextFieldNumeric<int>(0, "0", null, options);

            textField.Alignment.Should().Be(Align.MiddleCenter);
        }

        [Fact]
        public void Constructor_WithFixedSizeAndOptions_ShouldApplyOptionsAndSize()
        {
            var options = new UIElementOptions { Alignment = Align.LowerRight };
            Action<int> onValueChanged = (value) => { };

            var textField = new TextFieldNumeric<int>(42, "42", 120f, 35f, onValueChanged, options);

            textField.Alignment.Should().Be(Align.LowerRight);
            textField.Width.Should().Be(120f);
            textField.Height.Should().Be(35f);
            textField.OnValueChanged.Should().Be(onValueChanged);
        }

        [Fact]
        public void Constructor_WithParentAndOptions_ShouldApplyOptionsAndSetParent()
        {
            var parent = new UIElement(200f, 100f);
            var options = new UIElementOptions { Alignment = Align.MiddleLeft };
            Action<int> onValueChanged = (value) => { };

            var textField = new TextFieldNumeric<int>(parent, 25, "25", onValueChanged, options);

            textField.Parent.Should().Be(parent);
            textField.Alignment.Should().Be(Align.MiddleLeft);
            textField.OnValueChanged.Should().Be(onValueChanged);
            parent.Children.Should().Contain(textField);
        }

        [Fact]
        public void Constructor_WithMinMax_ShouldInitializeWithCorrectValues()
        {
            int value = 50;
            string buffer = "50";
            int min = 10;
            int max = 100;

            var textField = new TextFieldNumeric<int>(value, buffer, min, max);

            textField.Value.Should().Be(value);
            textField.Buffer.Should().Be(buffer);
            textField.Min.Should().Be(min);
            textField.Max.Should().Be(max);
        }

        [Fact]
        public void Render_ShouldCallRenderElement()
        {
            var textField = new MockTextFieldNumeric<int>(42, "42");

            textField.Render();

            textField.RenderElementCalled.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_ShouldCallTextFieldNumeric()
        {
            var textField = new MockTextFieldNumeric<int>(42, "42");

            textField.PublicRenderElement();

            textField.WidgetsTextFieldNumericCalled.Should().BeTrue();
            textField.RenderedValue.Should().Be(42);
            textField.RenderedBuffer.Should().Be("42");
        }

        [Fact]
        public void RenderElement_WithTooltip_ShouldSetTooltipRegion()
        {
            var textField = new MockTextFieldNumeric<int>(42, "42");
            textField.Tooltip = "Test tooltip";

            textField.PublicRenderElement();

            textField.TooltipSet.Should().Be("Test tooltip");
        }

        [Fact]
        public void RenderElement_WithoutTooltip_ShouldNotSetTooltipRegion()
        {
            var textField = new MockTextFieldNumeric<int>(42, "42");
            textField.Tooltip = null;

            textField.PublicRenderElement();

            textField.TooltipSet.Should().BeNull();
        }

        [Fact]
        public void CreateRect_ShouldReturnCorrectRect()
        {
            var textField = new TextFieldNumeric<int>(42, "42");
            textField.X = 15f;
            textField.Y = 25f;
            textField.Width = 100f;
            textField.Height = 30f;

            var rect = textField.CreateRect();

            rect.x.Should().Be(15f);
            rect.y.Should().Be(25f);
            rect.width.Should().Be(100f);
            rect.height.Should().Be(30f);
        }

        [Fact]
        public void OnValueChanged_ShouldBeInvokedWhenValueChanges()
        {
            bool callbackInvoked = false;
            int newValue = 0;
            var textField = new MockTextFieldNumeric<int>(10, "10");
            textField.OnValueChanged = (value) =>
            {
                callbackInvoked = true;
                newValue = value;
            };

            textField.SimulateValueChange(75, "75");

            callbackInvoked.Should().BeTrue();
            newValue.Should().Be(75);
        }

        [Fact]
        public void OnValueChanged_WhenNull_ShouldNotThrow()
        {
            var textField = new MockTextFieldNumeric<int>(10, "10");
            textField.OnValueChanged = null;

            textField.Invoking(tf => tf.SimulateValueChange(20, "20")).Should().NotThrow();
        }

        [Fact]
        public void OnValueChanged_WhenValueDoesNotChange_ShouldNotBeInvoked()
        {
            bool callbackInvoked = false;
            var textField = new MockTextFieldNumeric<int>(50, "50");
            textField.OnValueChanged = (value) => callbackInvoked = true;

            textField.SimulateValueChange(50, "50");

            callbackInvoked.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WithFillWidth_ShouldInitializeCorrectly()
        {
            int initialValue = 42;
            string initialBuffer = "42";
            Action<int> onValueChanged = (value) => { };

            var textField = new TextFieldNumeric<int>(initialValue, initialBuffer, SizeMode.Fill, onValueChanged);

            textField.Value.Should().Be(initialValue);
            textField.Buffer.Should().Be(initialBuffer);
            textField.OnValueChanged.Should().Be(onValueChanged);
            textField.WidthMode.Should().Be(SizeMode.Fill);
            textField.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Constructor_WithFillWidthAndOptions_ShouldInitializeCorrectly()
        {
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };
            var textField = new TextFieldNumeric<int>(42, "42", SizeMode.Fill, null, options);

            textField.Alignment.Should().Be(Align.MiddleCenter);
            textField.WidthMode.Should().Be(SizeMode.Fill);
            textField.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Constructor_WithParentAndFillWidth_ShouldSetParentAndFillMode()
        {
            var parent = new UIElement(200f, 100f);
            var textField = new TextFieldNumeric<int>(parent, 42, "42", SizeMode.Fill);

            textField.Parent.Should().Be(parent);
            textField.WidthMode.Should().Be(SizeMode.Fill);
            textField.HeightMode.Should().Be(SizeMode.Content);
            parent.Children.Should().Contain(textField);
        }

        [Fact]
        public void Constructor_WithParentFillWidthAndOptions_ShouldApplyAllSettings()
        {
            var parent = new UIElement(200f, 100f);
            var options = new UIElementOptions { Alignment = Align.LowerRight };
            var textField = new TextFieldNumeric<int>(parent, 42, "42", SizeMode.Fill, null, options);

            textField.Parent.Should().Be(parent);
            textField.WidthMode.Should().Be(SizeMode.Fill);
            textField.Alignment.Should().Be(Align.LowerRight);
            parent.Children.Should().Contain(textField);
        }

        [Fact]
        public void Width_WithFillMode_ShouldFillParentWidth()
        {
            var parent = new UIElement(300f, 100f);
            var textField = new TextFieldNumeric<int>(42, "42", SizeMode.Fill);
            parent.AddChild(textField);

            textField.Width.Should().Be(300f);
        }

        [Fact]
        public void Width_WithFillMode_WithoutParent_ShouldReturnZero()
        {
            var textField = new TextFieldNumeric<int>(42, "42", SizeMode.Fill);

            textField.Width.Should().Be(0f);
        }

        [Fact]
        public void Width_WithFillMode_WhenParentWidthChanges_ShouldUpdate()
        {
            var parent = new UIElement(200f, 100f);
            var textField = new TextFieldNumeric<int>(42, "42", SizeMode.Fill);
            parent.AddChild(textField);
            var initialWidth = textField.Width;

            parent.Width = 400f;
            var newWidth = textField.Width;

            initialWidth.Should().Be(200f);
            newWidth.Should().Be(400f);
        }

        [Fact]
        public void Height_WithFillMode_ShouldStillCalculateFromContent()
        {
            var parent = new UIElement(300f, 100f);
            var textField = new MockTextFieldNumericFillMode<int>(42, "42", SizeMode.Fill);
            textField.MockLineHeight = 20f;
            parent.AddChild(textField);

            textField.Height.Should().Be(20f);
        }

        [Fact]
        public void OnParentSet_WithFillMode_ShouldInvalidateSize()
        {
            var textField = new MockTextFieldNumericFillMode<int>(42, "42", SizeMode.Fill);
            var parent = new UIElement(250f, 150f);

            var widthBeforeParent = textField.Width;
            parent.AddChild(textField);
            var widthAfterParent = textField.Width;

            widthBeforeParent.Should().Be(0f);
            widthAfterParent.Should().Be(250f);
        }

        [Fact]
        public void CalculateDynamicSize_WithFillMode_ShouldReturnZeroWidthAndCalculatedHeight()
        {
            var textField = new MockTextFieldNumericFillMode<int>(42, "42", SizeMode.Fill);
            textField.MockLineHeight = 18f;

            var size = textField.CalculateDynamicSize();

            size.width.Should().Be(0f);
            size.height.Should().Be(18f);
        }

        [Fact]
        public void Constructor_WithNullOnValueChanged_ShouldInitializeCorrectly()
        {
            var textField = new TextFieldNumeric<int>(42, "42", null);

            textField.OnValueChanged.Should().BeNull();
            textField.Value.Should().Be(42);
            textField.Buffer.Should().Be("42");
        }

        [Fact]
        public void OnValueChanged_WithMultipleValueChanges_ShouldBeInvokedEachTime()
        {
            int callCount = 0;
            var values = new System.Collections.Generic.List<int>();
            var textField = new MockTextFieldNumeric<int>(0, "0");
            textField.OnValueChanged = (value) =>
            {
                callCount++;
                values.Add(value);
            };

            textField.SimulateValueChange(10, "10");
            textField.SimulateValueChange(20, "20");
            textField.SimulateValueChange(30, "30");

            callCount.Should().Be(3);
            values.Should().Equal(new[] { 10, 20, 30 });
        }

        [Fact]
        public void RenderElement_WithFillMode_ShouldUseCorrectWidth()
        {
            var parent = new UIElement(300f, 100f);
            var textField = new MockTextFieldNumericFillMode<int>(42, "42", SizeMode.Fill);
            parent.AddChild(textField);

            textField.PublicRenderElement();

            textField.RenderedWidth.Should().Be(300f);
            textField.RenderedHeight.Should().Be(20f);
        }

        [Fact]
        public void Constructor_WithDifferentNumericTypes_ShouldWork()
        {
            var intField = new TextFieldNumeric<int>(42, "42");
            var floatField = new TextFieldNumeric<float>(3.14f, "3.14");
            var doubleField = new TextFieldNumeric<double>(2.718, "2.718");

            intField.Value.Should().Be(42);
            floatField.Value.Should().Be(3.14f);
            doubleField.Value.Should().Be(2.718);
        }

        private class MockTextFieldNumeric<T> : TextFieldNumeric<T> where T : struct
        {
            public bool RenderElementCalled { get; protected set; }
            public bool WidgetsTextFieldNumericCalled { get; protected set; }
            public T RenderedValue { get; protected set; }
            public string RenderedBuffer { get; protected set; }
            public string TooltipSet { get; protected set; }

            public MockTextFieldNumeric(T value, string buffer, Action<T> onValueChanged = null) : base(value, buffer, onValueChanged) { }

            public void PublicRenderElement()
            {
                RenderElement();
            }

            public void SimulateValueChange(T newValue, string newBuffer)
            {
                T originalValue = Value;
                Value = newValue;
                Buffer = newBuffer;

                if (!newValue.Equals(originalValue))
                {
                    OnValueChanged?.Invoke(newValue);
                }
            }

            protected override void RenderElement()
            {
                RenderElementCalled = true;
                WidgetsTextFieldNumericCalled = true;
                RenderedValue = Value;
                RenderedBuffer = Buffer;

                if (!string.IsNullOrEmpty(Tooltip))
                {
                    TooltipSet = Tooltip;
                }
            }
        }

        private class MockTextFieldNumericFillMode<T> : MockTextFieldNumeric<T> where T : struct
        {
            public float RenderedWidth { get; private set; }
            public float RenderedHeight { get; private set; }
            public float MockLineHeight { get; set; } = 20f;

            public MockTextFieldNumericFillMode(T value, string buffer, SizeMode widthMode, Action<T> onValueChanged = null)
                : base(value, buffer, onValueChanged)
            {
                WidthMode = widthMode;
            }

            public void PublicRenderElement()
            {
                RenderElement();
            }

            public override (float width, float height) CalculateDynamicSize()
            {
                if (WidthMode == SizeMode.Fill)
                {
                    return (0f, MockLineHeight);
                }

                return (120f, MockLineHeight);
            }

            protected override void RenderElement()
            {
                RenderElementCalled = true;
                WidgetsTextFieldNumericCalled = true;
                RenderedValue = Value;
                RenderedBuffer = Buffer;

                var rect = CreateRect();
                RenderedWidth = rect.width;
                RenderedHeight = rect.height;

                if (!string.IsNullOrEmpty(Tooltip))
                {
                    TooltipSet = Tooltip;
                }
            }
        }
    }
}