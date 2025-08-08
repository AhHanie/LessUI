using FluentAssertions;
using LessUI;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    public class LabeledSliderTests
    {
        [Fact]
        public void Constructor_WithContentSize_ShouldInitializeCorrectly()
        {
            var slider = new LabeledSlider("Volume:", 0.5f, 0f, 1f);

            slider.Label.Should().Be("Volume:");
            slider.Value.Should().Be(0.5f);
            slider.Min.Should().Be(0f);
            slider.Max.Should().Be(1f);
            slider.WidthMode.Should().Be(SizeMode.Content);
            slider.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Constructor_WithDefaultParameters_ShouldUseDefaults()
        {
            var slider = new LabeledSlider("Test:", 50f);

            slider.Label.Should().Be("Test:");
            slider.Value.Should().Be(50f);
            slider.Min.Should().Be(0f);
            slider.Max.Should().Be(100f);
        }

        [Fact]
        public void Constructor_WithFixedSize_ShouldInitializeCorrectly()
        {
            var slider = new LabeledSlider("Test:", 50f, 0f, 100f, 200f, 30f);

            slider.Label.Should().Be("Test:");
            slider.Value.Should().Be(50f);
            slider.Width.Should().Be(200f);
            slider.Height.Should().Be(30f);
            slider.WidthMode.Should().Be(SizeMode.Fixed);
            slider.HeightMode.Should().Be(SizeMode.Fixed);
        }

        [Fact]
        public void Constructor_WithFillMode_ShouldInitializeCorrectly()
        {
            var slider = new LabeledSlider("Test:", 50f, 0f, 100f, SizeMode.Fill);

            slider.Label.Should().Be("Test:");
            slider.Value.Should().Be(50f);
            slider.WidthMode.Should().Be(SizeMode.Fill);
            slider.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Constructor_WithParent_ShouldSetParentCorrectly()
        {
            var parent = new UIElement(SizeMode.Fixed, SizeMode.Fixed);
            parent.Width = 300f;
            parent.Height = 200f;
            var slider = new LabeledSlider(parent, "Volume:", 0.75f, 0f, 1f);

            slider.Parent.Should().Be(parent);
            slider.Label.Should().Be("Volume:");
            slider.Value.Should().Be(0.75f);
            parent.Children.Should().Contain(slider);
        }

        [Fact]
        public void Constructor_WithParentAndFillWidth_ShouldSetParentAndFillMode()
        {
            var parent = new UIElement(300f, 200f);
            var slider = new LabeledSlider(parent, "Volume:", 0.5f, 0f, 1f, SizeMode.Fill);

            slider.Parent.Should().Be(parent);
            slider.WidthMode.Should().Be(SizeMode.Fill);
            slider.HeightMode.Should().Be(SizeMode.Content);
            parent.Children.Should().Contain(slider);
        }

        [Fact]
        public void Constructor_WithOptions_ShouldApplyOptions()
        {
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };
            var slider = new LabeledSlider("Test:", 50f, 0f, 100f, null, options);

            slider.Alignment.Should().Be(Align.MiddleCenter);
        }

        [Fact]
        public void Label_Property_ShouldGetAndSetCorrectly()
        {
            var slider = new LabeledSlider("Original:", 50f);
            string newLabel = "Updated:";

            slider.Label = newLabel;

            slider.Label.Should().Be(newLabel);
        }

        [Fact]
        public void Value_Property_ShouldGetAndSetCorrectly()
        {
            var slider = new LabeledSlider("Test:", 25f, 0f, 100f);
            float newValue = 75f;

            slider.Value = newValue;

            slider.Value.Should().Be(newValue);
        }

        [Fact]
        public void Value_WhenSetBelowMin_ShouldClampToMin()
        {
            var slider = new LabeledSlider("Test:", 50f, 10f, 100f);

            slider.Value = 5f;

            slider.Value.Should().Be(10f);
        }

        [Fact]
        public void Value_WhenSetAboveMax_ShouldClampToMax()
        {
            var slider = new LabeledSlider("Test:", 50f, 0f, 100f);

            slider.Value = 150f;

            slider.Value.Should().Be(100f);
        }

        [Fact]
        public void Min_Property_ShouldGetAndSetCorrectly()
        {
            var slider = new LabeledSlider("Test:", 50f, 0f, 100f);
            float newMin = 25f;

            slider.Min = newMin;

            slider.Min.Should().Be(newMin);
        }

        [Fact]
        public void Min_WhenSetAboveCurrentValue_ShouldAdjustValue()
        {
            var slider = new LabeledSlider("Test:", 20f, 0f, 100f);

            slider.Min = 30f;

            slider.Min.Should().Be(30f);
            slider.Value.Should().Be(30f);
        }

        [Fact]
        public void Max_Property_ShouldGetAndSetCorrectly()
        {
            var slider = new LabeledSlider("Test:", 50f, 0f, 100f);
            float newMax = 200f;

            slider.Max = newMax;

            slider.Max.Should().Be(newMax);
        }

        [Fact]
        public void Max_WhenSetBelowCurrentValue_ShouldAdjustValue()
        {
            var slider = new LabeledSlider("Test:", 80f, 0f, 100f);

            slider.Max = 70f;

            slider.Max.Should().Be(70f);
            slider.Value.Should().Be(70f);
        }

        [Fact]
        public void Tooltip_Property_ShouldGetAndSetCorrectly()
        {
            var slider = new LabeledSlider("Test:", 50f);
            string tooltip = "Adjust the volume level";

            slider.Tooltip = tooltip;

            slider.Tooltip.Should().Be(tooltip);
        }

        [Fact]
        public void IsLabelEmpty_WithEmptyString_ShouldReturnTrue()
        {
            var slider = new LabeledSlider("", 50f);

            slider.IsLabelEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsLabelEmpty_WithNullString_ShouldReturnTrue()
        {
            var slider = new LabeledSlider("Test:", 50f);
            slider.Label = null;

            slider.IsLabelEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsLabelEmpty_WithWhitespaceString_ShouldReturnTrue()
        {
            var slider = new LabeledSlider("   ", 50f);

            slider.IsLabelEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsLabelEmpty_WithValidText_ShouldReturnFalse()
        {
            var slider = new LabeledSlider("Volume:", 50f);

            slider.IsLabelEmpty.Should().BeFalse();
        }

        [Fact]
        public void Format_Property_ShouldGetAndSetCorrectly()
        {
            var slider = new LabeledSlider("Test:", 50f);
            string format = "F2";

            slider.Format = format;

            slider.Format.Should().Be(format);
        }

        [Fact]
        public void RoundTo_Property_ShouldGetAndSetCorrectly()
        {
            var slider = new LabeledSlider("Test:", 50f);
            float roundTo = 0.1f;

            slider.RoundTo = roundTo;

            slider.RoundTo.Should().Be(roundTo);
        }

        [Fact]
        public void RoundTo_DefaultValue_ShouldBeNegativeOne()
        {
            var slider = new LabeledSlider("Test:", 50f);

            slider.RoundTo.Should().Be(-1f);
        }

        [Fact]
        public void OnValueChanged_WhenValueChanges_ShouldBeInvoked()
        {
            var slider = new LabeledSlider("Test:", 50f);
            bool eventCalled = false;
            float newValue = 0f;

            slider.OnValueChanged += (value) => {
                eventCalled = true;
                newValue = value;
            };

            slider.Value = 75f;

            eventCalled.Should().BeTrue();
            newValue.Should().Be(75f);
        }

        [Fact]
        public void OnValueChanged_WhenValueDoesNotChange_ShouldNotBeInvoked()
        {
            var slider = new LabeledSlider("Test:", 50f);
            bool eventCalled = false;

            slider.OnValueChanged += (value) => {
                eventCalled = true;
            };

            slider.Value = 50f;

            eventCalled.Should().BeFalse();
        }

        [Fact]
        public void Width_WithFillMode_ShouldFillParentWidth()
        {
            var parent = new UIElement(400f, 200f);
            var slider = new LabeledSlider("Test:", 50f, 0f, 100f, SizeMode.Fill);
            parent.AddChild(slider);

            slider.Width.Should().Be(400f);
        }

        [Fact]
        public void Width_WithFillMode_WithoutParent_ShouldReturnZero()
        {
            var slider = new LabeledSlider("Test:", 50f, 0f, 100f, SizeMode.Fill);

            slider.Width.Should().Be(0f);
        }

        // Tests for aligned labels (rendered above slider)

        [Fact]
        public void LeftAlignedLabel_Property_ShouldGetAndSetCorrectly()
        {
            var slider = new LabeledSlider("Volume:", 50f);
            string leftLabel = "Quiet";

            slider.LeftAlignedLabel = leftLabel;

            slider.LeftAlignedLabel.Should().Be(leftLabel);
        }

        [Fact]
        public void RightAlignedLabel_Property_ShouldGetAndSetCorrectly()
        {
            var slider = new LabeledSlider("Volume:", 50f);
            string rightLabel = "Loud";

            slider.RightAlignedLabel = rightLabel;

            slider.RightAlignedLabel.Should().Be(rightLabel);
        }

        [Fact]
        public void LeftAlignedLabel_SetToNull_ShouldAcceptNull()
        {
            var slider = new LabeledSlider("Test:", 50f);
            slider.LeftAlignedLabel = "Previous";

            slider.LeftAlignedLabel = null;

            slider.LeftAlignedLabel.Should().BeNull();
        }

        [Fact]
        public void RightAlignedLabel_SetToNull_ShouldAcceptNull()
        {
            var slider = new LabeledSlider("Test:", 50f);
            slider.RightAlignedLabel = "Previous";

            slider.RightAlignedLabel = null;

            slider.RightAlignedLabel.Should().BeNull();
        }

        [Fact]
        public void HasAnyLabels_WithMainLabel_ShouldReturnTrue()
        {
            var slider = new LabeledSlider("Volume:", 50f);

            slider.HasAnyLabels.Should().BeTrue();
        }

        [Fact]
        public void HasAnyLabels_WithLeftAlignedLabel_ShouldReturnTrue()
        {
            var slider = new LabeledSlider("", 50f);
            slider.LeftAlignedLabel = "Min";

            slider.HasAnyLabels.Should().BeTrue();
        }

        [Fact]
        public void HasAnyLabels_WithRightAlignedLabel_ShouldReturnTrue()
        {
            var slider = new LabeledSlider("", 50f);
            slider.RightAlignedLabel = "Max";

            slider.HasAnyLabels.Should().BeTrue();
        }

        [Fact]
        public void HasAnyLabels_WithNoLabels_ShouldReturnFalse()
        {
            var slider = new LabeledSlider("", 50f);

            slider.HasAnyLabels.Should().BeFalse();
        }

        [Fact]
        public void HasAnyLabels_WithAllLabels_ShouldReturnTrue()
        {
            var slider = new LabeledSlider("Volume:", 50f);
            slider.LeftAlignedLabel = "Quiet";
            slider.RightAlignedLabel = "Loud";

            slider.HasAnyLabels.Should().BeTrue();
        }

        [Fact]
        public void CalculateDynamicSize_WithNoLabels_ShouldReturnSliderHeightOnly()
        {
            var slider = new LabeledSlider("", 50f);

            var size = slider.CalculateDynamicSize();

            size.height.Should().Be(22f); // Just slider height, no label space
        }

        [Fact]
        public void Constructor_WithAlignedLabels_ShouldInitializeCorrectly()
        {
            var slider = new LabeledSlider("Volume:", 50f, 0f, 100f);
            slider.LeftAlignedLabel = "0%";
            slider.RightAlignedLabel = "100%";
            slider.RoundTo = 5f;

            slider.LeftAlignedLabel.Should().Be("0%");
            slider.RightAlignedLabel.Should().Be("100%");
            slider.RoundTo.Should().Be(5f);
        }

        [Fact]
        public void Constructor_WithParentAndAlignedLabels_ShouldWork()
        {
            var parent = new UIElement(400f, 100f);
            var slider = new LabeledSlider(parent, "Brightness:", 75f, 0f, 100f);
            slider.LeftAlignedLabel = "Dark";
            slider.RightAlignedLabel = "Bright";

            slider.Parent.Should().Be(parent);
            slider.LeftAlignedLabel.Should().Be("Dark");
            slider.RightAlignedLabel.Should().Be("Bright");
            parent.Children.Should().Contain(slider);
        }

        [Fact]
        public void Constructor_WithFillModeAndAlignedLabels_ShouldInitializeCorrectly()
        {
            var slider = new LabeledSlider("Quality:", 80f, 0f, 100f, SizeMode.Fill);
            slider.LeftAlignedLabel = "Poor";
            slider.RightAlignedLabel = "Excellent";

            slider.WidthMode.Should().Be(SizeMode.Fill);
            slider.LeftAlignedLabel.Should().Be("Poor");
            slider.RightAlignedLabel.Should().Be("Excellent");
        }

        [Fact]
        public void AlignedLabels_WithEmptyMainLabel_ShouldStillWork()
        {
            var slider = new LabeledSlider("", 50f, 0f, 100f);
            slider.LeftAlignedLabel = "Start";
            slider.RightAlignedLabel = "End";

            slider.IsLabelEmpty.Should().BeTrue();
            slider.LeftAlignedLabel.Should().Be("Start");
            slider.RightAlignedLabel.Should().Be("End");
            slider.HasAnyLabels.Should().BeTrue();
        }

        [Fact]
        public void AlignedLabels_WithMainLabelAndFillMode_ShouldWork()
        {
            var parent = new UIElement(300f, 50f);
            var slider = new LabeledSlider("Speed:", 25f, 0f, 100f, SizeMode.Fill);
            slider.LeftAlignedLabel = "Slow";
            slider.RightAlignedLabel = "Fast";
            parent.AddChild(slider);

            slider.WidthMode.Should().Be(SizeMode.Fill);
            slider.Width.Should().Be(300f);
            slider.LeftAlignedLabel.Should().Be("Slow");
            slider.RightAlignedLabel.Should().Be("Fast");
        }
    }
}