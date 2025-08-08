using FluentAssertions;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    // DO NOT USE OR TEST THE INTERNALS OF Verse.Text, Verse.Widgets as it's Unity code which cannot be executed outside of Unity
    public class TextFieldNumericLabeledTests
    {
        [Fact]
        public void Constructor_WithLabelAndContentSize_ShouldInitializeWithCorrectValues()
        {
            // Test the basic constructor with content-based sizing for both width and height
            string label = "Amount:";
            int initialValue = 42;
            string initialBuffer = "42";
            Action<int> onValueChanged = (value) => { };

            var labeledTextField = new LabeledTextFieldNumeric<int>(label, initialValue, initialBuffer, onValueChanged);

            labeledTextField.Label.Should().Be(label);
            labeledTextField.Value.Should().Be(initialValue);
            labeledTextField.Buffer.Should().Be(initialBuffer);
            labeledTextField.OnValueChanged.Should().Be(onValueChanged);
            labeledTextField.WidthMode.Should().Be(SizeMode.Content);
            labeledTextField.HeightMode.Should().Be(SizeMode.Content);
            labeledTextField.Spacing.Should().Be(6f); // Default spacing between label and text field
            labeledTextField.Min.Should().Be(0); // Default minimum value for int
            labeledTextField.Max.Should().Be(1000000000); // Default maximum value for int
        }

        [Fact]
        public void Constructor_WithLabelAndFixedSize_ShouldInitializeWithCorrectValues()
        {
            // Test constructor with fixed dimensions to ensure proper initialization
            string label = "Price:";
            float initialValue = 99.99f;
            string initialBuffer = "99.99";
            float width = 250f;
            float height = 35f;
            Action<float> onValueChanged = (value) => { };

            var labeledTextField = new LabeledTextFieldNumeric<float>(label, initialValue, initialBuffer, width, height, onValueChanged);

            labeledTextField.Label.Should().Be(label);
            labeledTextField.Value.Should().Be(initialValue);
            labeledTextField.Buffer.Should().Be(initialBuffer);
            labeledTextField.Width.Should().Be(width);
            labeledTextField.Height.Should().Be(height);
            labeledTextField.OnValueChanged.Should().Be(onValueChanged);
            labeledTextField.WidthMode.Should().Be(SizeMode.Fixed);
            labeledTextField.HeightMode.Should().Be(SizeMode.Fixed);
        }

        [Fact]
        public void Constructor_WithParent_ShouldSetParentCorrectly()
        {
            // Test that adding the labeled text field to a parent works correctly
            var parent = new UIElement(SizeMode.Fixed, SizeMode.Fixed);
            parent.Width = 300f;
            parent.Height = 200f;
            string label = "Child Field:";
            int initialValue = 123;
            string initialBuffer = "123";
            Action<int> onValueChanged = (value) => { };

            var labeledTextField = new LabeledTextFieldNumeric<int>(parent, label, initialValue, initialBuffer, onValueChanged);

            labeledTextField.Parent.Should().Be(parent);
            labeledTextField.Label.Should().Be(label);
            labeledTextField.Value.Should().Be(initialValue);
            labeledTextField.Buffer.Should().Be(initialBuffer);
            labeledTextField.OnValueChanged.Should().Be(onValueChanged);
            parent.Children.Should().Contain(labeledTextField);
        }

        [Fact]
        public void Constructor_WithOptions_ShouldApplyOptions()
        {
            // Test that UI element options are properly applied during construction
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };

            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, "42", null, options);

            labeledTextField.Alignment.Should().Be(Align.MiddleCenter);
        }

        [Fact]
        public void Constructor_WithFixedSizeAndOptions_ShouldApplyOptionsAndSize()
        {
            // Test that both size and options are applied correctly when both are provided
            var options = new UIElementOptions { Alignment = Align.LowerRight };
            Action<int> onValueChanged = (value) => { };

            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, "42", 180f, 40f, onValueChanged, options);

            labeledTextField.Alignment.Should().Be(Align.LowerRight);
            labeledTextField.Width.Should().Be(180f);
            labeledTextField.Height.Should().Be(40f);
            labeledTextField.OnValueChanged.Should().Be(onValueChanged);
        }

        [Fact]
        public void Constructor_WithParentAndOptions_ShouldApplyOptionsAndSetParent()
        {
            // Test that parent-child relationship and options both work when specified together
            var parent = new UIElement(300f, 200f);
            var options = new UIElementOptions { Alignment = Align.MiddleLeft };
            Action<int> onValueChanged = (value) => { };

            var labeledTextField = new LabeledTextFieldNumeric<int>(parent, "Label:", 42, "42", onValueChanged, options);

            labeledTextField.Parent.Should().Be(parent);
            labeledTextField.Alignment.Should().Be(Align.MiddleLeft);
            labeledTextField.OnValueChanged.Should().Be(onValueChanged);
            parent.Children.Should().Contain(labeledTextField);
        }

        [Fact]
        public void Constructor_WithNullLabel_ShouldUseEmptyString()
        {
            // Test that null labels are handled gracefully by converting to empty string
            var labeledTextField = new LabeledTextFieldNumeric<int>(null, 42, "42");

            labeledTextField.Label.Should().Be("");
            labeledTextField.IsLabelEmpty.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WithNullBuffer_ShouldUseEmptyString()
        {
            // Test that null buffers are handled gracefully by converting to empty string
            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, null);

            labeledTextField.Buffer.Should().Be("");
        }

        [Fact]
        public void Constructor_WithMinMax_ShouldInitializeWithCorrectValues()
        {
            // Test constructor that accepts custom min/max values instead of using defaults
            string label = "Score:";
            int value = 75;
            string buffer = "75";
            int min = 0;
            int max = 100;

            var labeledTextField = new LabeledTextFieldNumeric<int>(label, value, buffer, min, max);

            labeledTextField.Label.Should().Be(label);
            labeledTextField.Value.Should().Be(value);
            labeledTextField.Buffer.Should().Be(buffer);
            labeledTextField.Min.Should().Be(min);
            labeledTextField.Max.Should().Be(max);
        }

        [Fact]
        public void Label_Property_ShouldGetAndSetCorrectly()
        {
            // Test that the Label property can be read and written, and triggers size invalidation
            var labeledTextField = new LabeledTextFieldNumeric<int>("Initial Label:", 42, "42");
            string newLabel = "Updated Label:";

            labeledTextField.Label = newLabel;

            labeledTextField.Label.Should().Be(newLabel);
        }

        [Fact]
        public void Value_Property_ShouldGetAndSetCorrectly()
        {
            // Test that the numeric Value property can be read and written
            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 10, "10");
            int newValue = 50;

            labeledTextField.Value = newValue;

            labeledTextField.Value.Should().Be(newValue);
        }

        [Fact]
        public void Buffer_Property_ShouldGetAndSetCorrectly()
        {
            // Test that the Buffer property (used by RimWorld internally) can be read and written
            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, "42");
            string newBuffer = "123";

            labeledTextField.Buffer = newBuffer;

            labeledTextField.Buffer.Should().Be(newBuffer);
        }

        [Fact]
        public void Min_Property_ShouldGetAndSetCorrectly()
        {
            // Test that the minimum value constraint can be read and written
            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, "42");
            int newMin = -10;

            labeledTextField.Min = newMin;

            labeledTextField.Min.Should().Be(newMin);
        }

        [Fact]
        public void Max_Property_ShouldGetAndSetCorrectly()
        {
            // Test that the maximum value constraint can be read and written
            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, "42");
            int newMax = 500;

            labeledTextField.Max = newMax;

            labeledTextField.Max.Should().Be(newMax);
        }

        [Fact]
        public void Tooltip_Property_ShouldGetAndSetCorrectly()
        {
            // Test that tooltips can be assigned and retrieved
            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, "42");
            string tooltip = "Enter a numeric value";

            labeledTextField.Tooltip = tooltip;

            labeledTextField.Tooltip.Should().Be(tooltip);
        }

        [Fact]
        public void Spacing_Property_ShouldGetAndSetCorrectly()
        {
            // Test that the spacing between label and text field can be customized
            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, "42");
            float customSpacing = 12f;

            labeledTextField.Spacing = customSpacing;

            labeledTextField.Spacing.Should().Be(customSpacing);
        }

        [Fact]
        public void Spacing_DefaultValue_ShouldBe6()
        {
            // Test that the default spacing is 6 pixels, matching other labeled controls
            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, "42");

            labeledTextField.Spacing.Should().Be(6f);
        }

        [Fact]
        public void IsLabelEmpty_WithEmptyString_ShouldReturnTrue()
        {
            // Test that empty string labels are correctly identified as empty
            var labeledTextField = new LabeledTextFieldNumeric<int>("", 42, "42");

            labeledTextField.IsLabelEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsLabelEmpty_WithNullString_ShouldReturnTrue()
        {
            // Test that null labels are correctly identified as empty
            var labeledTextField = new LabeledTextFieldNumeric<int>("test", 42, "42");
            labeledTextField.Label = null;

            labeledTextField.IsLabelEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsLabelEmpty_WithWhitespaceString_ShouldReturnTrue()
        {
            // Test that whitespace-only labels are correctly identified as empty
            var labeledTextField = new LabeledTextFieldNumeric<int>("   ", 42, "42");

            labeledTextField.IsLabelEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsLabelEmpty_WithValidText_ShouldReturnFalse()
        {
            // Test that non-empty labels are correctly identified as non-empty
            var labeledTextField = new LabeledTextFieldNumeric<int>("Valid Label:", 42, "42");

            labeledTextField.IsLabelEmpty.Should().BeFalse();
        }

        [Fact]
        public void Render_ShouldCallRenderElement()
        {
            // Test that the public Render method properly calls the protected RenderElement method
            var labeledTextField = new MockTextFieldNumericLabeled<int>("Label:", 42, "42");

            labeledTextField.Render();

            labeledTextField.RenderElementCalled.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_WithLabel_ShouldCallLabelAndTextFieldNumeric()
        {
            // Test that when a label is present, both the label and numeric text field are rendered
            var labeledTextField = new MockTextFieldNumericLabeled<int>("Amount:", 150, "150");

            labeledTextField.PublicRenderElement();

            labeledTextField.LabelCalled.Should().BeTrue();
            labeledTextField.TextFieldNumericCalled.Should().BeTrue();
            labeledTextField.TextFieldOnlyCalled.Should().BeFalse();
            labeledTextField.RenderedLabel.Should().Be("Amount:");
            labeledTextField.RenderedValue.Should().Be(150);
            labeledTextField.RenderedBuffer.Should().Be("150");
        }

        [Fact]
        public void RenderElement_WithEmptyLabel_ShouldCallTextFieldNumericOnly()
        {
            // Test that when no label is present, only the numeric text field is rendered
            var labeledTextField = new MockTextFieldNumericLabeled<int>("", 75, "75");

            labeledTextField.PublicRenderElement();

            labeledTextField.TextFieldOnlyCalled.Should().BeTrue();
            labeledTextField.LabelCalled.Should().BeFalse();
            labeledTextField.TextFieldNumericCalled.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_WithTooltip_ShouldSetTooltipRegion()
        {
            // Test that tooltips are properly attached to the rendered element
            var labeledTextField = new MockTextFieldNumericLabeled<int>("Label:", 42, "42");
            labeledTextField.Tooltip = "Test tooltip";

            labeledTextField.PublicRenderElement();

            labeledTextField.TooltipSet.Should().Be("Test tooltip");
        }

        [Fact]
        public void RenderElement_WithoutTooltip_ShouldNotSetTooltipRegion()
        {
            // Test that no tooltip handling occurs when tooltip is not set
            var labeledTextField = new MockTextFieldNumericLabeled<int>("Label:", 42, "42");
            labeledTextField.Tooltip = null;

            labeledTextField.PublicRenderElement();

            labeledTextField.TooltipSet.Should().BeNull();
        }

        [Fact]
        public void RenderElement_EmptyLabelWithTooltip_ShouldStillShowTooltip()
        {
            // Test that tooltips work even when there's no label text
            var labeledTextField = new MockTextFieldNumericLabeled<int>("", 42, "42");
            labeledTextField.Tooltip = "Empty label tooltip";

            labeledTextField.PublicRenderElement();

            labeledTextField.TextFieldOnlyCalled.Should().BeTrue();
            labeledTextField.TooltipSet.Should().Be("Empty label tooltip");
        }

        [Fact]
        public void CreateRect_ShouldReturnCorrectRect()
        {
            // Test that the CreateRect method returns accurate position and size information
            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, "42");
            labeledTextField.X = 15f;
            labeledTextField.Y = 25f;
            labeledTextField.Width = 120f;
            labeledTextField.Height = 35f;

            var rect = labeledTextField.CreateRect();

            rect.x.Should().Be(15f);
            rect.y.Should().Be(25f);
            rect.width.Should().Be(120f);
            rect.height.Should().Be(35f);
        }

        [Fact]
        public void OnValueChanged_ShouldBeInvokedWhenValueChanges()
        {
            // Test that the callback is triggered when the numeric value changes
            bool callbackInvoked = false;
            int newValue = 0;
            var labeledTextField = new MockTextFieldNumericLabeled<int>("Label:", 10, "10");
            labeledTextField.OnValueChanged = (value) =>
            {
                callbackInvoked = true;
                newValue = value;
            };

            labeledTextField.SimulateValueChange(95, "95");

            callbackInvoked.Should().BeTrue();
            newValue.Should().Be(95);
        }

        [Fact]
        public void OnValueChanged_WhenNull_ShouldNotThrow()
        {
            // Test that the component doesn't crash when no callback is provided
            var labeledTextField = new MockTextFieldNumericLabeled<int>("Label:", 42, "42");
            labeledTextField.OnValueChanged = null;

            labeledTextField.Invoking(tf => tf.SimulateValueChange(50, "50")).Should().NotThrow();
        }

        [Fact]
        public void OnValueChanged_WhenValueDoesNotChange_ShouldNotBeInvoked()
        {
            // Test that the callback is not triggered when the value remains the same
            bool callbackInvoked = false;
            var labeledTextField = new MockTextFieldNumericLabeled<int>("Label:", 25, "25");
            labeledTextField.OnValueChanged = (value) => callbackInvoked = true;

            labeledTextField.SimulateValueChange(25, "25");

            callbackInvoked.Should().BeFalse();
        }

        [Fact]
        public void CalculateDynamicSize_WithEmptyLabel_ShouldReturnTextFieldSize()
        {
            // Test size calculation when only the text field is present (no label)
            var labeledTextField = new MockTextFieldNumericLabeled<int>("", 42, "42");
            labeledTextField.MockTextFieldWidth = 120f;
            labeledTextField.MockLineHeight = 22f;

            var size = labeledTextField.CalculateDynamicSize();

            size.width.Should().Be(120f);
            size.height.Should().Be(22f);
        }

        [Fact]
        public void CalculateDynamicSize_WithLabel_ShouldIncludeLabelWidthAndSpacing()
        {
            // Test size calculation when both label and text field are present
            var labeledTextField = new MockTextFieldNumericLabeled<int>("Price:", 99, "99");
            labeledTextField.MockLabelWidth = 45f;
            labeledTextField.MockTextFieldWidth = 120f;
            labeledTextField.MockLineHeight = 22f;
            labeledTextField.Spacing = 8f;

            var size = labeledTextField.CalculateDynamicSize();

            // Total width should be: label width + spacing + text field width
            size.width.Should().Be(173f); // 45 + 8 + 120
            size.height.Should().Be(22f);
        }

        [Fact]
        public void Width_WithContentMode_ShouldCalculateCorrectly()
        {
            // Test that content-based width calculation works properly
            var labeledTextField = new MockTextFieldNumericLabeled<int>("Count:", 42, "42");
            labeledTextField.MockLabelWidth = 50f;
            labeledTextField.MockTextFieldWidth = 120f;

            var width = labeledTextField.Width;

            // Width should be: label width + default spacing + text field width
            width.Should().Be(176f); // 50 + 6 + 120
        }

        [Fact]
        public void Height_WithContentMode_ShouldCalculateCorrectly()
        {
            // Test that content-based height calculation uses text line height
            var labeledTextField = new MockTextFieldNumericLabeled<int>("Label:", 42, "42");
            labeledTextField.MockLineHeight = 20f;

            var height = labeledTextField.Height;

            height.Should().Be(20f);
        }

        [Fact]
        public void Label_SettingToSameValue_ShouldNotInvalidateSize()
        {
            // Test performance optimization: size shouldn't be recalculated if label doesn't actually change
            var labeledTextField = new MockTextFieldNumericLabeled<int>("Initial:", 42, "42");
            var initialCalculationCount = labeledTextField.CalculationCount;

            labeledTextField.Label = "Initial:";

            var width = labeledTextField.Width;

            labeledTextField.CalculationCount.Should().Be(initialCalculationCount + 1);
        }

        [Fact]
        public void Value_SettingToDifferentValue_ShouldNotInvalidateSize()
        {
            // Test that changing the numeric value doesn't trigger size recalculation (value doesn't affect layout)
            var labeledTextField = new MockTextFieldNumericLabeled<int>("Label:", 10, "10");
            var width1 = labeledTextField.Width;
            var calculationCount1 = labeledTextField.CalculationCount;

            labeledTextField.Value = 50;
            var width2 = labeledTextField.Width;

            // Size calculation count should not increase since numeric value doesn't affect size
            labeledTextField.CalculationCount.Should().Be(calculationCount1);
        }

        [Fact]
        public void Spacing_SettingToDifferentValue_ShouldInvalidateSize()
        {
            // Test that changing spacing triggers size recalculation since it affects layout
            var labeledTextField = new MockTextFieldNumericLabeled<int>("Label:", 42, "42");
            var width1 = labeledTextField.Width;
            var calculationCount1 = labeledTextField.CalculationCount;

            labeledTextField.Spacing = 12f;
            var width2 = labeledTextField.Width;

            labeledTextField.CalculationCount.Should().Be(calculationCount1 + 1);
        }

        [Fact]
        public void Constructor_WithNullOnValueChanged_ShouldInitializeCorrectly()
        {
            // Test that the component works properly when no callback is provided
            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, "42", null);

            labeledTextField.OnValueChanged.Should().BeNull();
            labeledTextField.Label.Should().Be("Label:");
            labeledTextField.Value.Should().Be(42);
            labeledTextField.Buffer.Should().Be("42");
        }

        [Fact]
        public void OnValueChanged_WithMultipleValueChanges_ShouldBeInvokedEachTime()
        {
            // Test that the callback is triggered for each value change, not just the first one
            int callCount = 0;
            var values = new System.Collections.Generic.List<int>();
            var labeledTextField = new MockTextFieldNumericLabeled<int>("Label:", 0, "0");
            labeledTextField.OnValueChanged = (value) =>
            {
                callCount++;
                values.Add(value);
            };

            labeledTextField.SimulateValueChange(10, "10");
            labeledTextField.SimulateValueChange(20, "20");
            labeledTextField.SimulateValueChange(30, "30");

            callCount.Should().Be(3);
            values.Should().Equal(new[] { 10, 20, 30 });
        }

        // FILL MODE TESTS - Testing the advanced width management feature

        [Fact]
        public void Constructor_WithFillWidth_ShouldInitializeCorrectly()
        {
            // Test initialization of fill mode, which makes the text field expand to use available space
            string label = "Amount:";
            int initialValue = 42;
            string initialBuffer = "42";
            Action<int> onValueChanged = (value) => { };

            var labeledTextField = new LabeledTextFieldNumeric<int>(label, initialValue, initialBuffer, SizeMode.Fill, onValueChanged);

            labeledTextField.Label.Should().Be(label);
            labeledTextField.Value.Should().Be(initialValue);
            labeledTextField.Buffer.Should().Be(initialBuffer);
            labeledTextField.OnValueChanged.Should().Be(onValueChanged);
            labeledTextField.WidthMode.Should().Be(SizeMode.Fill);
            labeledTextField.HeightMode.Should().Be(SizeMode.Content);
            labeledTextField.Spacing.Should().Be(6f);
        }

        [Fact]
        public void Constructor_WithFillWidthAndOptions_ShouldInitializeCorrectly()
        {
            // Test that fill mode works correctly when combined with UI element options
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };
            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, "42", SizeMode.Fill, null, options);

            labeledTextField.Alignment.Should().Be(Align.MiddleCenter);
            labeledTextField.WidthMode.Should().Be(SizeMode.Fill);
            labeledTextField.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Constructor_WithParentAndFillWidth_ShouldSetParentAndFillMode()
        {
            // Test that fill mode works correctly when the component is added to a parent container
            var parent = new UIElement(300f, 200f);
            var labeledTextField = new LabeledTextFieldNumeric<int>(parent, "Label:", 42, "42", SizeMode.Fill);

            labeledTextField.Parent.Should().Be(parent);
            labeledTextField.WidthMode.Should().Be(SizeMode.Fill);
            labeledTextField.HeightMode.Should().Be(SizeMode.Content);
            parent.Children.Should().Contain(labeledTextField);
        }

        [Fact]
        public void Constructor_WithParentFillWidthAndOptions_ShouldApplyAllSettings()
        {
            // Test that parent relationship, fill mode, and options all work together correctly
            var parent = new UIElement(300f, 200f);
            var options = new UIElementOptions { Alignment = Align.LowerRight };
            var labeledTextField = new LabeledTextFieldNumeric<int>(parent, "Label:", 42, "42", SizeMode.Fill, null, options);

            labeledTextField.Parent.Should().Be(parent);
            labeledTextField.WidthMode.Should().Be(SizeMode.Fill);
            labeledTextField.Alignment.Should().Be(Align.LowerRight);
            parent.Children.Should().Contain(labeledTextField);
        }

        [Fact]
        public void Width_WithFillMode_ShouldFillParentWidth()
        {
            // Test that fill mode makes the component use the full width of its parent
            var parent = new UIElement(400f, 200f);
            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, "42", SizeMode.Fill);
            parent.AddChild(labeledTextField);

            labeledTextField.Width.Should().Be(400f);
        }

        [Fact]
        public void Width_WithFillMode_WithoutParent_ShouldReturnZero()
        {
            // Test that fill mode returns zero width when there's no parent to fill
            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, "42", SizeMode.Fill);

            labeledTextField.Width.Should().Be(0f);
        }

        [Fact]
        public void Width_WithFillMode_WhenParentWidthChanges_ShouldUpdate()
        {
            // Test that fill mode dynamically responds to parent size changes
            var parent = new UIElement(300f, 200f);
            var labeledTextField = new LabeledTextFieldNumeric<int>("Label:", 42, "42", SizeMode.Fill);
            parent.AddChild(labeledTextField);
            var initialWidth = labeledTextField.Width;

            parent.Width = 500f;
            var newWidth = labeledTextField.Width;

            initialWidth.Should().Be(300f);
            newWidth.Should().Be(500f);
        }

        [Fact]
        public void Height_WithFillMode_ShouldStillCalculateFromContent()
        {
            // Test that fill mode only affects width; height is still calculated from content
            var parent = new UIElement(400f, 200f);
            var labeledTextField = new MockTextFieldNumericLabeledFillMode<int>("Label:", 42, "42", SizeMode.Fill);
            labeledTextField.MockLineHeight = 25f;
            parent.AddChild(labeledTextField);

            labeledTextField.Height.Should().Be(25f);
        }

        [Fact]
        public void OnParentSet_WithFillMode_ShouldInvalidateSize()
        {
            // Test that size is recalculated when a fill-mode component gets a new parent
            var labeledTextField = new MockTextFieldNumericLabeledFillMode<int>("Label:", 42, "42", SizeMode.Fill);
            var parent = new UIElement(350f, 180f);

            var widthBeforeParent = labeledTextField.Width;
            parent.AddChild(labeledTextField);
            var widthAfterParent = labeledTextField.Width;

            widthBeforeParent.Should().Be(0f);
            widthAfterParent.Should().Be(350f);
        }

        [Fact]
        public void CalculateDynamicSize_WithFillMode_ShouldReturnZeroWidthAndCalculatedHeight()
        {
            // Test that fill mode's size calculation returns zero width (to be filled by parent) and proper height
            var labeledTextField = new MockTextFieldNumericLabeledFillMode<int>("Label:", 42, "42", SizeMode.Fill);
            labeledTextField.MockLineHeight = 20f;

            var size = labeledTextField.CalculateDynamicSize();

            size.width.Should().Be(0f); // Fill mode returns 0 for width calculation
            size.height.Should().Be(20f);
        }

        [Fact]
        public void GetTextFieldWidth_WithFillMode_ShouldCalculateRemainingWidth()
        {
            // Test that in fill mode, the text field gets the remaining width after label and spacing
            var parent = new UIElement(300f, 100f);
            var labeledTextField = new MockTextFieldNumericLabeledFillMode<int>("Count:", 42, "42", SizeMode.Fill);
            labeledTextField.MockLabelWidth = 60f;
            labeledTextField.Spacing = 10f;
            parent.AddChild(labeledTextField);

            var textFieldWidth = labeledTextField.PublicGetTextFieldWidth();

            // Total width: 300, Label width: 60, Spacing: 10
            // Text field width: 300 - 60 - 10 = 230
            textFieldWidth.Should().Be(230f);
        }

        [Fact]
        public void GetTextFieldWidth_WithFillModeAndEmptyLabel_ShouldReturnFullWidth()
        {
            // Test that when there's no label, the text field gets the full parent width in fill mode
            var parent = new UIElement(300f, 100f);
            var labeledTextField = new MockTextFieldNumericLabeledFillMode<int>("", 42, "42", SizeMode.Fill);
            parent.AddChild(labeledTextField);

            var textFieldWidth = labeledTextField.PublicGetTextFieldWidth();

            textFieldWidth.Should().Be(300f);
        }

        [Fact]
        public void GetTextFieldWidth_WithFillModeAndNoParent_ShouldReturnZero()
        {
            // Test that fill mode returns zero width when there's no parent to get width from
            var labeledTextField = new MockTextFieldNumericLabeledFillMode<int>("Label:", 42, "42", SizeMode.Fill);

            var textFieldWidth = labeledTextField.PublicGetTextFieldWidth();

            textFieldWidth.Should().Be(0f);
        }

        [Fact]
        public void GetTextFieldWidth_WithContentMode_ShouldReturnDefaultWidth()
        {
            // Test that content mode returns a fixed default width regardless of parent
            var labeledTextField = new MockTextFieldNumericLabeledFillMode<int>("Label:", 42, "42", SizeMode.Content);

            var textFieldWidth = labeledTextField.PublicGetTextFieldWidth();

            textFieldWidth.Should().Be(120f); // Default text field width
        }

        [Fact]
        public void RenderElement_WithFillMode_ShouldUseCalculatedTextFieldWidth()
        {
            // Test that fill mode properly calculates and uses the correct text field width during rendering
            var parent = new UIElement(280f, 100f);
            var labeledTextField = new MockTextFieldNumericLabeledFillMode<int>("Price:", 99, "99", SizeMode.Fill);
            labeledTextField.MockLabelWidth = 50f;
            labeledTextField.Spacing = 8f;
            parent.AddChild(labeledTextField);

            labeledTextField.PublicRenderElement();

            labeledTextField.RenderedLabelWidth.Should().Be(50f);
            labeledTextField.RenderedTextFieldWidth.Should().Be(222f); // 280 - 50 - 8
            labeledTextField.RenderedTotalWidth.Should().Be(280f);
        }

        [Fact]
        public void RenderElement_WithFillModeAndEmptyLabel_ShouldUseFullWidth()
        {
            // Test that when there's no label in fill mode, the text field uses the full parent width
            var parent = new UIElement(250f, 100f);
            var labeledTextField = new MockTextFieldNumericLabeledFillMode<int>("", 42, "42", SizeMode.Fill);
            parent.AddChild(labeledTextField);

            labeledTextField.PublicRenderElement();

            labeledTextField.RenderedTextFieldOnlyWidth.Should().Be(250f);
        }

        [Fact]
        public void RenderElement_WithFillModeAndLargeLabelWidth_ShouldHandleCorrectly()
        {
            // Test edge case where label is very wide relative to parent, ensuring text field gets minimal space
            var parent = new UIElement(120f, 100f);
            var labeledTextField = new MockTextFieldNumericLabeledFillMode<int>("Very Long Label Text Here:", 42, "42", SizeMode.Fill);
            labeledTextField.MockLabelWidth = 110f; // Very wide label
            labeledTextField.Spacing = 8f;
            parent.AddChild(labeledTextField);

            labeledTextField.PublicRenderElement();

            // Text field width should be minimal: 120 - 110 - 8 = 2
            labeledTextField.RenderedTextFieldWidth.Should().Be(2f);
        }

        [Fact]
        public void Constructor_WithDifferentNumericTypes_ShouldWork()
        {
            // Test that the generic implementation works with various numeric types
            var intField = new LabeledTextFieldNumeric<int>("Int:", 42, "42");
            var floatField = new LabeledTextFieldNumeric<float>("Float:", 3.14f, "3.14");
            var doubleField = new LabeledTextFieldNumeric<double>("Double:", 2.718, "2.718");

            intField.Value.Should().Be(42);
            floatField.Value.Should().Be(3.14f);
            doubleField.Value.Should().Be(2.718);

            // Verify default min/max values are set correctly for each type
            intField.Min.Should().Be(0);
            intField.Max.Should().Be(1000000000);
            floatField.Min.Should().Be(0f);
            floatField.Max.Should().Be(1000000000f);
            doubleField.Min.Should().Be(0.0);
            doubleField.Max.Should().Be(1000000000.0);
        }

        // Mock classes for testing protected methods and internal behavior
        private class MockTextFieldNumericLabeled<T> : LabeledTextFieldNumeric<T> where T : struct
        {
            public bool RenderElementCalled { get; private set; }
            public bool LabelCalled { get; private set; }
            public bool TextFieldNumericCalled { get; private set; }
            public bool TextFieldOnlyCalled { get; private set; }
            public string RenderedLabel { get; private set; }
            public T RenderedValue { get; private set; }
            public string RenderedBuffer { get; private set; }
            public string TooltipSet { get; private set; }
            public float MockLabelWidth { get; set; } = 50f;
            public float MockTextFieldWidth { get; set; } = 120f;
            public float MockLineHeight { get; set; } = 20f;
            public int CalculationCount { get; private set; }

            public MockTextFieldNumericLabeled(string label, T value, string buffer, Action<T> onValueChanged = null) : base(label, value, buffer, onValueChanged) { }

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

            public override (float width, float height) CalculateDynamicSize()
            {
                CalculationCount++;

                if (IsLabelEmpty)
                {
                    return (MockTextFieldWidth, MockLineHeight);
                }

                float totalWidth = MockLabelWidth + Spacing + MockTextFieldWidth;
                float totalHeight = MockLineHeight;

                return (totalWidth, totalHeight);
            }

            protected override void RenderElement()
            {
                RenderElementCalled = true;

                if (IsLabelEmpty)
                {
                    TextFieldOnlyCalled = true;
                    TextFieldNumericCalled = true;
                    if (!string.IsNullOrEmpty(Tooltip))
                    {
                        TooltipSet = Tooltip;
                    }
                }
                else
                {
                    LabelCalled = true;
                    RenderedLabel = Label;
                    RenderedValue = Value;
                    RenderedBuffer = Buffer;
                    TextFieldNumericCalled = true;

                    if (!string.IsNullOrEmpty(Tooltip))
                    {
                        TooltipSet = Tooltip;
                    }
                }
            }
        }

        private class MockTextFieldNumericLabeledFillMode<T> : LabeledTextFieldNumeric<T> where T : struct
        {
            public float RenderedLabelWidth { get; private set; }
            public float RenderedTextFieldWidth { get; private set; }
            public float RenderedTotalWidth { get; private set; }
            public float RenderedTextFieldOnlyWidth { get; private set; }
            public float MockLabelWidth { get; set; } = 50f;
            public float MockLineHeight { get; set; } = 20f;

            public MockTextFieldNumericLabeledFillMode(string label, T value, string buffer, SizeMode widthMode, Action<T> onValueChanged = null)
                : base(label, value, buffer, onValueChanged)
            {
                WidthMode = widthMode;
            }

            public void PublicRenderElement()
            {
                RenderElement();
            }

            public float PublicGetTextFieldWidth()
            {
                return GetTextFieldWidth();
            }

            public override (float width, float height) CalculateDynamicSize()
            {
                if (WidthMode == SizeMode.Fill)
                {
                    return (0f, MockLineHeight);
                }

                // Content mode calculation
                if (IsLabelEmpty)
                {
                    return (120f, MockLineHeight);
                }

                float totalWidth = MockLabelWidth + Spacing + 120f;
                return (totalWidth, MockLineHeight);
            }

            protected override void RenderElement()
            {
                var fullRect = CreateRect();
                RenderedTotalWidth = fullRect.width;

                if (IsLabelEmpty)
                {
                    RenderedTextFieldOnlyWidth = fullRect.width;
                }
                else
                {
                    RenderedLabelWidth = MockLabelWidth;
                    RenderedTextFieldWidth = GetTextFieldWidth();
                }
            }

            protected override float GetTextFieldWidth()
            {
                if (WidthMode == SizeMode.Fill)
                {
                    if (Parent == null) return 0f;

                    if (IsLabelEmpty)
                    {
                        return Width; // Use our own width, not parent's width
                    }

                    float labelWidth = MockLabelWidth;
                    return Math.Max(0f, Width - labelWidth - Spacing); // Use our own width
                }

                return 120f; // Default text field width for content mode
            }
        }
    }
}