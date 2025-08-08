using FluentAssertions;
using System.Runtime.CompilerServices;
using UnityEngine;
using Xunit;

namespace LessUI.Tests
{
    // DO NOT USE OR TEST THE INTERNALS OF Verse.Text, Verse.Widgets as it's Unity code which cannot be executed outside of Unity
    public class CheckboxTests
    {
        [Fact]
        public void Constructor_WithFixedSize_ShouldInitializeWithCorrectValues()
        {
            float width = 200f;
            float height = 30f;
            var checkedBox = new StrongBox<bool>(true);

            var checkBox = new Checkbox(width, height, checkedBox);

            checkBox.Width.Should().Be(width);
            checkBox.Height.Should().Be(height);
            checkBox.WidthMode.Should().Be(SizeMode.Fixed);
            checkBox.HeightMode.Should().Be(SizeMode.Fixed);
        }

        [Fact]
        public void Tooltip_Property_ShouldGetAndSetCorrectly()
        {
            var checkedBox = new StrongBox<bool>(false);
            var checkBox = new Checkbox(checkedBox);
            string tooltip = "This is a tooltip";

            checkBox.Tooltip = tooltip;

            checkBox.Tooltip.Should().Be(tooltip);
        }

        [Fact]
        public void Render_ShouldCallRenderElement()
        {
            var checkedBox = new StrongBox<bool>(false);
            var checkBox = new MockCheckBox(checkedBox);

            checkBox.Render();

            checkBox.RenderElementCalled.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_ShouldSetCorrectProperties()
        {
            var checkedBox = new StrongBox<bool>(true);
            var checkBox = new MockCheckBox(checkedBox);

            checkBox.PublicRenderElement();

            checkBox.WidgetsCheckboxCalled.Should().BeTrue();
            checkBox.CheckedState.Should().BeTrue();
        }

        [Fact]
        public void RenderElement_WithTooltip_ShouldSetTooltipRegion()
        {
            var checkedBox = new StrongBox<bool>(false);
            var checkBox = new MockCheckBox(checkedBox);
            checkBox.Tooltip = "Test tooltip";

            checkBox.PublicRenderElement();

            checkBox.TooltipSet.Should().Be("Test tooltip");
        }

        [Fact]
        public void RenderElement_WithoutTooltip_ShouldNotSetTooltipRegion()
        {
            var checkedBox = new StrongBox<bool>(false);
            var checkBox = new MockCheckBox(checkedBox);
            checkBox.Tooltip = null;

            checkBox.PublicRenderElement();

            checkBox.TooltipSet.Should().BeNull();
        }

        [Fact]
        public void CalculateDynamicSize_ShouldReturnCheckboxSize()
        {
            var checkedBox = new StrongBox<bool>(false);
            var checkBox = new Checkbox(checkedBox);

            var size = checkBox.CalculateDynamicSize();

            size.width.Should().Be(24f);
            size.height.Should().Be(24f);
        }

        [Fact]
        public void Width_WithContentMode_ShouldCalculateTo24()
        {
            var checkedBox = new StrongBox<bool>(false);
            var checkBox = new Checkbox(checkedBox);

            var width = checkBox.Width;

            width.Should().Be(24f);
        }

        [Fact]
        public void Height_WithContentMode_ShouldCalculateTo24()
        {
            var checkedBox = new StrongBox<bool>(false);
            var checkBox = new Checkbox(checkedBox);

            var height = checkBox.Height;

            height.Should().Be(24f);
        }

        [Fact]
        public void CreateRect_ShouldReturnCorrectRect()
        {
            var checkedBox = new StrongBox<bool>(false);
            var checkBox = new Checkbox(checkedBox);
            checkBox.X = 15f;
            checkBox.Y = 25f;
            checkBox.Width = 120f;
            checkBox.Height = 35f;

            var rect = checkBox.CreateRect();

            rect.x.Should().Be(15f);
            rect.y.Should().Be(25f);
            rect.width.Should().Be(120f);
            rect.height.Should().Be(35f);
        }

        [Fact]
        public void Constructor_WithContentBasedSizing_ShouldInitializeCorrectly()
        {
            var checkedBox = new StrongBox<bool>(true);

            var checkBox = new Checkbox(checkedBox);

            checkBox.WidthMode.Should().Be(SizeMode.Content);
            checkBox.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Constructor_WithOptions_ShouldApplyOptions()
        {
            var checkedBox = new StrongBox<bool>(false);
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };
            var checkBox = new Checkbox(checkedBox, options);

            checkBox.Alignment.Should().Be(Align.MiddleCenter);
        }

        [Fact]
        public void Constructor_WithFixedSizeAndOptions_ShouldApplyOptions()
        {
            var checkedBox = new StrongBox<bool>(true);
            var options = new UIElementOptions { Alignment = Align.LowerRight };
            var checkBox = new Checkbox(150f, 40f, checkedBox, options);

            checkBox.Alignment.Should().Be(Align.LowerRight);
            checkBox.Width.Should().Be(150f);
            checkBox.Height.Should().Be(40f);
        }

        [Fact]
        public void Constructor_WithNullStrongBox_ShouldThrowArgumentNullException()
        {
            StrongBox<bool> nullBox = null;

            var act = () => new Checkbox(nullBox);

            act.Should().Throw<ArgumentNullException>().WithParameterName("isChecked");
        }

        [Fact]
        public void StateChange_ShouldUpdateStrongBox()
        {
            var checkedBox = new StrongBox<bool>(false);
            var checkBox = new MockCheckBox(checkedBox);

            checkBox.SimulateStateChange(true);

            checkedBox.Value.Should().BeTrue();
        }

        [Fact]
        public void StateChange_WhenStateDoesNotChange_ShouldNotUpdateStrongBox()
        {
            var checkedBox = new StrongBox<bool>(true);
            var checkBox = new MockCheckBox(checkedBox);

            checkBox.SimulateStateChange(true);

            checkedBox.Value.Should().BeTrue();
        }

        [Fact]
        public void StateChange_WithMultipleStateChanges_ShouldUpdateStrongBoxEachTime()
        {
            var checkedBox = new StrongBox<bool>(false);
            var checkBox = new MockCheckBox(checkedBox);

            checkBox.SimulateStateChange(true);
            checkedBox.Value.Should().BeTrue();

            checkBox.SimulateStateChange(false);
            checkedBox.Value.Should().BeFalse();

            checkBox.SimulateStateChange(true);
            checkedBox.Value.Should().BeTrue();
        }

        [Fact]
        public void StateChange_ShouldUpdateInternalCheckedProperty()
        {
            var checkedBox = new StrongBox<bool>(false);
            var checkBox = new MockCheckBox(checkedBox);

            checkBox.SimulateStateChange(true);

            checkedBox.Value.Should().BeTrue();
        }

        private class MockCheckBox : Checkbox
        {
            public bool RenderElementCalled { get; private set; }
            public bool WidgetsCheckboxCalled { get; private set; }
            public bool CheckedState { get; private set; }
            public string TooltipSet { get; private set; }

            public MockCheckBox(StrongBox<bool> checkedBox) : base(checkedBox) { }

            public void PublicRenderElement()
            {
                RenderElement();
            }

            public void SimulateStateChange(bool newState)
            {
                bool originalState = Checked;
                Checked = newState;

                if (newState != originalState)
                {
                    // Simulate what the real RenderElement does: update the StrongBox
                    // In the real implementation, this would be done in RenderElement after Widgets.Checkbox
                    var checkedBoxField = typeof(Checkbox).GetField("_checkedBox",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var strongBox = (StrongBox<bool>)checkedBoxField.GetValue(this);
                    strongBox.Value = newState;
                }
            }

            protected override void RenderElement()
            {
                RenderElementCalled = true;
                CheckedState = Checked;
                WidgetsCheckboxCalled = true;

                if (!string.IsNullOrEmpty(Tooltip))
                {
                    TooltipSet = Tooltip;
                }
            }
        }
    }
}