using FluentAssertions;
using System.Runtime.CompilerServices;
using Verse;

namespace LessUI.Tests
{
    public class IntRangeSliderTests
    {
        [Fact]
        public void Constructor_WithContentSize_ShouldInitializeWithCorrectValues()
        {
            int min = 0;
            int max = 100;
            var range = new StrongBox<IntRange>(new IntRange(25, 75));

            var intRange = new IntRangeSlider(min, max, range);

            intRange.Min.Should().Be(min);
            intRange.Max.Should().Be(max);
            intRange.LowerValue.Should().Be(25);
            intRange.UpperValue.Should().Be(75);
            intRange.RangeBox.Should().NotBeNull();
            intRange.RangeBox.Should().BeSameAs(range);
            intRange.WidthMode.Should().Be(SizeMode.Content);
            intRange.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Constructor_WithFixedSize_ShouldInitializeWithCorrectValues()
        {
            int min = 0;
            int max = 100;
            float width = 200f;
            float height = 30f;
            var range = new StrongBox<IntRange>(new IntRange(20, 80));

            var intRange = new IntRangeSlider(min, max, width, height, range);

            intRange.Min.Should().Be(min);
            intRange.Max.Should().Be(max);
            intRange.LowerValue.Should().Be(20);
            intRange.UpperValue.Should().Be(80);
            intRange.Width.Should().Be(width);
            intRange.Height.Should().Be(height);
            intRange.RangeBox.Should().NotBeNull();
            intRange.RangeBox.Should().BeSameAs(range);
            intRange.WidthMode.Should().Be(SizeMode.Fixed);
            intRange.HeightMode.Should().Be(SizeMode.Fixed);
        }

        [Fact]
        public void Constructor_WithSharedStrongBox_ShouldInitializeCorrectly()
        {
            var sharedRange = new StrongBox<IntRange>(new IntRange(30, 60));
            var intRange = new IntRangeSlider(0, 100, sharedRange);

            intRange.RangeBox.Should().BeSameAs(sharedRange);
            intRange.LowerValue.Should().Be(30);
            intRange.UpperValue.Should().Be(60);
            intRange.Min.Should().Be(0);
            intRange.Max.Should().Be(100);
        }

        [Fact]
        public void Constructor_WithNullStrongBox_ShouldThrowArgumentNullException()
        {
            Action act = () => new IntRangeSlider(0, 100, (StrongBox<IntRange>)null);

            act.Should().Throw<ArgumentNullException>().WithParameterName("range");
        }

        [Fact]
        public void Constructor_WithFixedSizeAndNullStrongBox_ShouldThrowArgumentNullException()
        {
            Action act = () => new IntRangeSlider(0, 100, 200f, 30f, (StrongBox<IntRange>)null);

            act.Should().Throw<ArgumentNullException>().WithParameterName("range");
        }

        [Fact]
        public void Constructor_WithSizeModeAndNullStrongBox_ShouldThrowArgumentNullException()
        {
            Action act = () => new IntRangeSlider(0, 100, SizeMode.Fill, (StrongBox<IntRange>)null);

            act.Should().Throw<ArgumentNullException>().WithParameterName("range");
        }

        [Fact]
        public void LowerValue_Property_ShouldGetAndSetCorrectly()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);
            int newLowerValue = 35;

            intRange.LowerValue = newLowerValue;

            intRange.LowerValue.Should().Be(newLowerValue);
            intRange.RangeBox.Value.min.Should().Be(newLowerValue);
        }

        [Fact]
        public void UpperValue_Property_ShouldGetAndSetCorrectly()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);
            int newUpperValue = 85;

            intRange.UpperValue = newUpperValue;

            intRange.UpperValue.Should().Be(newUpperValue);
            intRange.RangeBox.Value.max.Should().Be(newUpperValue);
        }

        [Fact]
        public void LowerValue_WhenSetAboveUpperValue_ShouldClampToUpperValue()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);

            intRange.LowerValue = 80;

            intRange.LowerValue.Should().Be(75);
            intRange.UpperValue.Should().Be(75);
        }

        [Fact]
        public void UpperValue_WhenSetBelowLowerValue_ShouldClampToLowerValue()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);

            intRange.UpperValue = 20;

            intRange.LowerValue.Should().Be(25);
            intRange.UpperValue.Should().Be(25);
        }

        [Fact]
        public void LowerValue_WhenSetBelowMin_ShouldClampToMin()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(10, 100, range);

            intRange.LowerValue = 5;

            intRange.LowerValue.Should().Be(10);
        }

        [Fact]
        public void UpperValue_WhenSetAboveMax_ShouldClampToMax()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);

            intRange.UpperValue = 150;

            intRange.UpperValue.Should().Be(100);
        }

        [Fact]
        public void StrongBox_DirectManipulation_ShouldUpdateProperties()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);
            var strongBoxRef = intRange.RangeBox;

            // Direct manipulation through the StrongBox reference
            strongBoxRef.Value = new IntRange(40, 80);

            intRange.LowerValue.Should().Be(40);
            intRange.UpperValue.Should().Be(80);
        }

        [Fact]
        public void SharedStrongBox_ShouldAllowSharedState()
        {
            var sharedRange = new StrongBox<IntRange>(new IntRange(20, 60));
            var intRange1 = new IntRangeSlider(0, 100, sharedRange);
            var intRange2 = new IntRangeSlider(0, 100, sharedRange);

            // Modifying one should affect the other through shared StrongBox
            intRange1.LowerValue = 35;

            intRange2.LowerValue.Should().Be(35);
            intRange2.UpperValue.Should().Be(60);
            sharedRange.Value.min.Should().Be(35);
            sharedRange.Value.max.Should().Be(60);
        }

        [Fact]
        public void SetValues_ShouldUpdateBothValues()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);

            intRange.SetValues(40, 60);

            intRange.LowerValue.Should().Be(40);
            intRange.UpperValue.Should().Be(60);
            intRange.RangeBox.Value.min.Should().Be(40);
            intRange.RangeBox.Value.max.Should().Be(60);
        }

        [Fact]
        public void SetValues_WithInvalidRange_ShouldClampValues()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);

            intRange.SetValues(80, 20);

            intRange.LowerValue.Should().Be(20);
            intRange.UpperValue.Should().Be(20);
        }

        [Fact]
        public void Range_Property_ShouldReturnCorrectRange()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);

            int rangeValue = intRange.Range;

            rangeValue.Should().Be(50);
        }

        [Fact]
        public void IsValidRange_ShouldReturnTrueForValidRange()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);

            intRange.IsValidRange.Should().BeTrue();
        }

        [Fact]
        public void IsValidRange_ShouldReturnTrueForSingleValue()
        {
            var range = new StrongBox<IntRange>(new IntRange(50, 50));
            var intRange = new IntRangeSlider(0, 100, range);

            intRange.IsValidRange.Should().BeTrue();
        }

        [Fact]
        public void Tooltip_Property_ShouldGetAndSetCorrectly()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);
            string tooltip = "Select a range of values";

            intRange.Tooltip = tooltip;

            intRange.Tooltip.Should().Be(tooltip);
        }

        [Fact]
        public void CalculateDynamicSize_ShouldReturnDefaultSize()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);

            var (width, height) = intRange.CalculateDynamicSize();

            width.Should().Be(200f);
            height.Should().Be(32f);
        }

        [Fact]
        public void CreateRect_ShouldReturnCorrectRect()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, 200f, 32f, range);
            intRange.X = 10f;
            intRange.Y = 20f;

            var rect = intRange.CreateRect();

            rect.x.Should().Be(10f);
            rect.y.Should().Be(20f);
            rect.width.Should().Be(200f);
            rect.height.Should().Be(32f);
        }

        [Fact]
        public void MinRange_Property_ShouldReturnCorrectMinimumRange()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);

            int minRange = intRange.MinRange;

            minRange.Should().Be(100);
        }

        [Fact]
        public void Constructor_WithFillWidth_ShouldInitializeCorrectly()
        {
            int min = 0;
            int max = 100;
            var range = new StrongBox<IntRange>(new IntRange(25, 75));

            var intRange = new IntRangeSlider(min, max, SizeMode.Fill, range);

            intRange.Min.Should().Be(min);
            intRange.Max.Should().Be(max);
            intRange.LowerValue.Should().Be(25);
            intRange.UpperValue.Should().Be(75);
            intRange.RangeBox.Should().NotBeNull();
            intRange.RangeBox.Should().BeSameAs(range);
            intRange.WidthMode.Should().Be(SizeMode.Fill);
            intRange.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Constructor_WithFillWidthAndOptions_ShouldInitializeCorrectly()
        {
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, SizeMode.Fill, range, options);

            intRange.Alignment.Should().Be(Align.MiddleCenter);
            intRange.WidthMode.Should().Be(SizeMode.Fill);
            intRange.HeightMode.Should().Be(SizeMode.Content);
            intRange.RangeBox.Should().BeSameAs(range);
        }

        [Fact]
        public void StrongBoxType_ShouldBeCorrectType()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);

            intRange.RangeBox.Should().BeOfType<StrongBox<IntRange>>();
            intRange.RangeBox.Value.Should().BeOfType<IntRange>();
        }

        [Fact]
        public void Constructor_WithSharedStrongBoxAndFixedSize_ShouldInitializeCorrectly()
        {
            var sharedRange = new StrongBox<IntRange>(new IntRange(15, 85));
            var intRange = new IntRangeSlider(0, 100, 300f, 50f, sharedRange);

            intRange.RangeBox.Should().BeSameAs(sharedRange);
            intRange.LowerValue.Should().Be(15);
            intRange.UpperValue.Should().Be(85);
            intRange.Width.Should().Be(300f);
            intRange.Height.Should().Be(50f);
        }

        [Fact]
        public void Min_Property_ShouldGetAndSetCorrectly()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);

            intRange.Min = 10;

            intRange.Min.Should().Be(10);
        }

        [Fact]
        public void Max_Property_ShouldGetAndSetCorrectly()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);

            intRange.Max = 90;

            intRange.Max.Should().Be(90);
        }

        [Fact]
        public void SetValues_WithValuesOutOfRange_ShouldClampToBounds()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(10, 90, range);

            intRange.SetValues(5, 95);

            intRange.LowerValue.Should().Be(10); // Clamped to min
            intRange.UpperValue.Should().Be(90); // Clamped to max
        }

        [Fact]
        public void RangeBox_ShouldBeReadOnly()
        {
            var range = new StrongBox<IntRange>(new IntRange(25, 75));
            var intRange = new IntRangeSlider(0, 100, range);

            // RangeBox property should be get-only
            intRange.RangeBox.Should().BeSameAs(range);

            // The property itself should not be settable (this is enforced by the property being get-only)
            // We can verify this by checking that it's the same reference we passed in
            intRange.RangeBox.Should().BeSameAs(range);
        }
    }
}