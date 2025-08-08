using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

namespace LessUI.Tests
{
    public class DropdownTests
    {
        [Fact]
        public void Constructor_WithContentSize_ShouldInitializeWithCorrectValues()
        {
            var items = new List<string> { "Item1", "Item2", "Item3" };
            var selectedItemBox = new StrongBox<string>(null);
            Func<string, string> displayFunc = s => s.ToUpper();

            var dropdown = new Dropdown<string>(items, selectedItemBox, displayFunc);

            dropdown.Items.Should().BeEquivalentTo(items);
            dropdown.DisplayTextFunc.Should().Be(displayFunc);
            dropdown.SelectedItemBox.Should().BeSameAs(selectedItemBox);
            dropdown.WidthMode.Should().Be(SizeMode.Content);
            dropdown.HeightMode.Should().Be(SizeMode.Content);
            dropdown.HasSelection.Should().BeFalse();
            dropdown.Placeholder.Should().Be("Select...");
        }

        [Fact]
        public void Constructor_WithFixedSize_ShouldInitializeWithCorrectValues()
        {
            var items = new List<int> { 1, 2, 3 };
            float width = 200f;
            float height = 35f;
            var selectedItemBox = new StrongBox<int>(0);
            Func<int, string> displayFunc = i => $"Number {i}";

            var dropdown = new Dropdown<int>(items, width, height, selectedItemBox, displayFunc);

            dropdown.Items.Should().BeEquivalentTo(items);
            dropdown.DisplayTextFunc.Should().Be(displayFunc);
            dropdown.SelectedItemBox.Should().BeSameAs(selectedItemBox);
            dropdown.Width.Should().Be(width);
            dropdown.Height.Should().Be(height);
            dropdown.WidthMode.Should().Be(SizeMode.Fixed);
            dropdown.HeightMode.Should().Be(SizeMode.Fixed);
        }

        [Fact]
        public void Constructor_WithFillMode_ShouldInitializeWithCorrectValues()
        {
            var items = new List<string> { "A", "B", "C" };
            var selectedItemBox = new StrongBox<string>(null);
            Func<string, string> displayFunc = s => s;

            var dropdown = new Dropdown<string>(items, SizeMode.Fill, selectedItemBox, displayFunc);

            dropdown.Items.Should().BeEquivalentTo(items);
            dropdown.DisplayTextFunc.Should().Be(displayFunc);
            dropdown.SelectedItemBox.Should().BeSameAs(selectedItemBox);
            dropdown.WidthMode.Should().Be(SizeMode.Fill);
            dropdown.HeightMode.Should().Be(SizeMode.Content);
        }

        [Fact]
        public void Constructor_WithOptions_ShouldApplyOptions()
        {
            var items = new List<string> { "A", "B" };
            var selectedItemBox = new StrongBox<string>(null);
            var options = new UIElementOptions { Alignment = Align.MiddleCenter };

            var dropdown = new Dropdown<string>(items, selectedItemBox, null, options);

            dropdown.Alignment.Should().Be(Align.MiddleCenter);
        }

        [Fact]
        public void Constructor_WithNullItems_ShouldInitializeEmptyList()
        {
            var selectedItemBox = new StrongBox<string>(null);

            var dropdown = new Dropdown<string>(null, selectedItemBox);

            dropdown.Items.Should().NotBeNull();
            dropdown.Items.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WithNullDisplayFunc_ShouldUseDefaultDisplayFunc()
        {
            var items = new List<string> { "Test" };
            var selectedItemBox = new StrongBox<string>(null);

            var dropdown = new Dropdown<string>(items, selectedItemBox, null);

            dropdown.DisplayTextFunc.Should().NotBeNull();
            dropdown.DisplayTextFunc("Test").Should().Be("Test");
        }

        [Fact]
        public void Constructor_WithNullSelectedItemBox_ShouldThrowArgumentNullException()
        {
            var items = new List<string> { "A", "B" };

            Action act = () => new Dropdown<string>(items, null);

            act.Should().Throw<ArgumentNullException>().WithParameterName("selectedItem");
        }

        [Fact]
        public void Constructor_WithNullSelectedItemBoxFixedSize_ShouldThrowArgumentNullException()
        {
            var items = new List<string> { "A", "B" };

            Action act = () => new Dropdown<string>(items, 200f, 35f, null);

            act.Should().Throw<ArgumentNullException>().WithParameterName("selectedItem");
        }

        [Fact]
        public void Constructor_WithNullSelectedItemBoxFillMode_ShouldThrowArgumentNullException()
        {
            var items = new List<string> { "A", "B" };

            Action act = () => new Dropdown<string>(items, SizeMode.Fill, null);

            act.Should().Throw<ArgumentNullException>().WithParameterName("selectedItem");
        }

        [Fact]
        public void Constructor_WithPresetSelectedItem_ShouldInitializeFromStrongBox()
        {
            var items = new List<string> { "A", "B", "C" };
            var selectedItemBox = new StrongBox<string>("B");

            var dropdown = new Dropdown<string>(items, selectedItemBox);

            dropdown.SelectedItem.Should().Be("B");
            dropdown.HasSelection.Should().BeTrue();
        }

        [Fact]
        public void Items_Property_ShouldGetAndSetCorrectly()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string>(), selectedItemBox);
            var newItems = new List<string> { "New1", "New2", "New3" };

            dropdown.Items = newItems;

            dropdown.Items.Should().BeEquivalentTo(newItems);
        }

        [Fact]
        public void Items_WhenSetToNull_ShouldInitializeEmptyList()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string> { "Test" }, selectedItemBox);

            dropdown.Items = null;

            dropdown.Items.Should().NotBeNull();
            dropdown.Items.Should().BeEmpty();
        }

        [Fact]
        public void Items_WhenSelectedItemNotInNewList_ShouldClearSelection()
        {
            var selectedItemBox = new StrongBox<string>("B");
            var dropdown = new Dropdown<string>(new List<string> { "A", "B", "C" }, selectedItemBox);
            dropdown.SelectedItem = "B";

            dropdown.Items = new List<string> { "X", "Y", "Z" };

            dropdown.SelectedItem.Should().Be(default(string));
            dropdown.HasSelection.Should().BeFalse();
            selectedItemBox.Value.Should().Be(default(string));
        }

        [Fact]
        public void Items_WhenSelectedItemInNewList_ShouldKeepSelection()
        {
            var selectedItemBox = new StrongBox<string>("B");
            var dropdown = new Dropdown<string>(new List<string> { "A", "B", "C" }, selectedItemBox);
            dropdown.SelectedItem = "B";

            dropdown.Items = new List<string> { "X", "B", "Y" };

            dropdown.SelectedItem.Should().Be("B");
            dropdown.HasSelection.Should().BeTrue();
            selectedItemBox.Value.Should().Be("B");
        }

        [Fact]
        public void SelectedItem_Property_ShouldGetAndSetCorrectly()
        {
            var items = new List<string> { "Option1", "Option2", "Option3" };
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(items, selectedItemBox);

            dropdown.SelectedItem = "Option2";

            dropdown.SelectedItem.Should().Be("Option2");
            dropdown.HasSelection.Should().BeTrue();
        }

        [Fact]
        public void SelectedItem_WhenSetToItemNotInList_ShouldNotChange()
        {
            var items = new List<string> { "A", "B", "C" };
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(items, selectedItemBox);
            dropdown.SelectedItem = "A";

            dropdown.SelectedItem = "NotInList";

            dropdown.SelectedItem.Should().Be("A");
        }

        [Fact]
        public void SelectedItem_WhenSetToDefault_ShouldClearSelection()
        {
            var items = new List<string> { "A", "B", "C" };
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(items, selectedItemBox);
            dropdown.SelectedItem = "B";

            dropdown.SelectedItem = default(string);

            dropdown.SelectedItem.Should().Be(default(string));
            dropdown.HasSelection.Should().BeFalse();
        }

        [Fact]
        public void SelectedItem_WhenChanged_ShouldUpdateStrongBox()
        {
            var items = new List<string> { "A", "B", "C" };
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(items, selectedItemBox);

            dropdown.SelectedItem = "B";

            selectedItemBox.Value.Should().Be("B");
        }

        [Fact]
        public void SelectedItem_WhenNotChanged_ShouldNotUpdateStrongBox()
        {
            var items = new List<string> { "A", "B", "C" };
            var selectedItemBox = new StrongBox<string>("B");
            var dropdown = new Dropdown<string>(items, selectedItemBox);
            dropdown.SelectedItem = "B";

            // Set to same value
            dropdown.SelectedItem = "B";

            selectedItemBox.Value.Should().Be("B");
        }

        [Fact]
        public void DisplayTextFunc_Property_ShouldGetAndSetCorrectly()
        {
            var selectedItemBox = new StrongBox<int>(0);
            var dropdown = new Dropdown<int>(new List<int> { 1, 2, 3 }, selectedItemBox);
            Func<int, string> newFunc = i => $"Value: {i}";

            dropdown.DisplayTextFunc = newFunc;

            dropdown.DisplayTextFunc.Should().Be(newFunc);
            dropdown.DisplayTextFunc(5).Should().Be("Value: 5");
        }

        [Fact]
        public void DisplayTextFunc_WhenSetToNull_ShouldUseDefaultFunc()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string> { "Test" }, selectedItemBox);

            dropdown.DisplayTextFunc = null;

            dropdown.DisplayTextFunc.Should().NotBeNull();
            dropdown.DisplayTextFunc("Hello").Should().Be("Hello");
        }

        [Fact]
        public void Tooltip_Property_ShouldGetAndSetCorrectly()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string>(), selectedItemBox);
            string tooltip = "Select an option";

            dropdown.Tooltip = tooltip;

            dropdown.Tooltip.Should().Be(tooltip);
        }

        [Fact]
        public void Placeholder_Property_ShouldGetAndSetCorrectly()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string>(), selectedItemBox);
            string placeholder = "Choose...";

            dropdown.Placeholder = placeholder;

            dropdown.Placeholder.Should().Be(placeholder);
        }

        [Fact]
        public void Placeholder_WhenSetToNull_ShouldUseEmptyString()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string>(), selectedItemBox);

            dropdown.Placeholder = null;

            dropdown.Placeholder.Should().Be("");
        }

        [Fact]
        public void SelectedDisplayText_WithNoSelection_ShouldReturnPlaceholder()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string> { "A", "B" }, selectedItemBox);
            dropdown.Placeholder = "Pick one";

            dropdown.SelectedDisplayText.Should().Be("Pick one");
        }

        [Fact]
        public void SelectedDisplayText_WithNoSelectionAndNullPlaceholder_ShouldReturnDefault()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string> { "A", "B" }, selectedItemBox);
            dropdown.Placeholder = null;

            dropdown.SelectedDisplayText.Should().Be("Select...");
        }

        [Fact]
        public void SelectedDisplayText_WithSelection_ShouldReturnDisplayText()
        {
            var selectedItemBox = new StrongBox<int>(0);
            var dropdown = new Dropdown<int>(new List<int> { 1, 2, 3 }, selectedItemBox, i => $"Item {i}");
            dropdown.SelectedItem = 2;

            dropdown.SelectedDisplayText.Should().Be("Item 2");
        }

        [Fact]
        public void HasSelection_WithNoSelection_ShouldReturnFalse()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string> { "A", "B" }, selectedItemBox);

            dropdown.HasSelection.Should().BeFalse();
        }

        [Fact]
        public void HasSelection_WithSelection_ShouldReturnTrue()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string> { "A", "B" }, selectedItemBox);
            dropdown.SelectedItem = "A";

            dropdown.HasSelection.Should().BeTrue();
        }

        [Fact]
        public void Width_WithFillMode_ShouldFillParentWidth()
        {
            var parent = new UIElement(400f, 200f);
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string> { "A", "B" }, SizeMode.Fill, selectedItemBox);
            parent.AddChild(dropdown);

            dropdown.Width.Should().Be(400f);
        }

        [Fact]
        public void Width_WithFillMode_WithoutParent_ShouldReturnZero()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string> { "A", "B" }, SizeMode.Fill, selectedItemBox);

            dropdown.Width.Should().Be(0f);
        }

        [Fact]
        public void CalculateDynamicSize_WithFillMode_ShouldReturnZeroWidthAndCalculatedHeight()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string> { "A", "B" }, SizeMode.Fill, selectedItemBox);

            var size = dropdown.CalculateDynamicSize();

            size.width.Should().Be(0f);
            size.height.Should().Be(30f);
        }

        [Fact]
        public void CreateRect_ShouldReturnCorrectRect()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string>(), 180f, 32f, selectedItemBox);
            dropdown.X = 10f;
            dropdown.Y = 20f;

            var rect = dropdown.CreateRect();

            rect.x.Should().Be(10f);
            rect.y.Should().Be(20f);
            rect.width.Should().Be(180f);
            rect.height.Should().Be(32f);
        }

        [Fact]
        public void ClearSelection_ShouldRemoveCurrentSelection()
        {
            var selectedItemBox = new StrongBox<string>("B");
            var dropdown = new Dropdown<string>(new List<string> { "A", "B", "C" }, selectedItemBox);
            dropdown.SelectedItem = "B";

            dropdown.ClearSelection();

            dropdown.HasSelection.Should().BeFalse();
            dropdown.SelectedItem.Should().Be(default(string));
            selectedItemBox.Value.Should().Be(default(string));
        }

        [Fact]
        public void SelectFirst_WithItems_ShouldSelectFirstItem()
        {
            var items = new List<string> { "First", "Second", "Third" };
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(items, selectedItemBox);

            dropdown.SelectFirst();

            dropdown.SelectedItem.Should().Be("First");
            selectedItemBox.Value.Should().Be("First");
        }

        [Fact]
        public void SelectFirst_WithEmptyItems_ShouldNotThrow()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string>(), selectedItemBox);

            dropdown.Invoking(d => d.SelectFirst()).Should().NotThrow();
            dropdown.HasSelection.Should().BeFalse();
        }

        [Fact]
        public void SelectLast_WithItems_ShouldSelectLastItem()
        {
            var items = new List<string> { "First", "Second", "Third" };
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(items, selectedItemBox);

            dropdown.SelectLast();

            dropdown.SelectedItem.Should().Be("Third");
            selectedItemBox.Value.Should().Be("Third");
        }

        [Fact]
        public void SelectLast_WithEmptyItems_ShouldNotThrow()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string>(), selectedItemBox);

            dropdown.Invoking(d => d.SelectLast()).Should().NotThrow();
            dropdown.HasSelection.Should().BeFalse();
        }

        [Fact]
        public void GetSelectedIndex_WithSelection_ShouldReturnCorrectIndex()
        {
            var items = new List<string> { "A", "B", "C", "D" };
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(items, selectedItemBox);
            dropdown.SelectedItem = "C";

            var index = dropdown.GetSelectedIndex();

            index.Should().Be(2);
        }

        [Fact]
        public void GetSelectedIndex_WithNoSelection_ShouldReturnMinusOne()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string> { "A", "B", "C" }, selectedItemBox);

            var index = dropdown.GetSelectedIndex();

            index.Should().Be(-1);
        }

        [Fact]
        public void SelectByIndex_WithValidIndex_ShouldSelectCorrectItem()
        {
            var items = new List<string> { "Zero", "One", "Two" };
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(items, selectedItemBox);

            dropdown.SelectByIndex(1);

            dropdown.SelectedItem.Should().Be("One");
            selectedItemBox.Value.Should().Be("One");
        }

        [Fact]
        public void SelectByIndex_WithInvalidIndex_ShouldNotChangeSelection()
        {
            var items = new List<string> { "A", "B", "C" };
            var selectedItemBox = new StrongBox<string>("B");
            var dropdown = new Dropdown<string>(items, selectedItemBox);
            dropdown.SelectedItem = "B";

            dropdown.SelectByIndex(-1);
            dropdown.SelectedItem.Should().Be("B");

            dropdown.SelectByIndex(5);
            dropdown.SelectedItem.Should().Be("B");
        }

        [Fact]
        public void SelectedItemBox_WithMultipleSelectionChanges_ShouldBeUpdatedEachTime()
        {
            var items = new List<string> { "A", "B", "C" };
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(items, selectedItemBox);

            dropdown.SelectedItem = "A";
            selectedItemBox.Value.Should().Be("A");

            dropdown.SelectedItem = "B";
            selectedItemBox.Value.Should().Be("B");

            dropdown.SelectedItem = "C";
            selectedItemBox.Value.Should().Be("C");
        }

        [Fact]
        public void DisplayTextFunc_DefaultImplementation_ShouldHandleNullValues()
        {
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string>(), selectedItemBox);

            var result = dropdown.DisplayTextFunc(null);

            result.Should().Be("");
        }

        [Fact]
        public void Constructor_WithDifferentTypes_ShouldWork()
        {
            var intSelectedBox = new StrongBox<int>(0);
            var stringSelectedBox = new StrongBox<string>(null);
            var customSelectedBox = new StrongBox<TestClass>(null);

            var intDropdown = new Dropdown<int>(new List<int> { 1, 2, 3 }, intSelectedBox);
            var stringDropdown = new Dropdown<string>(new List<string> { "A", "B" }, stringSelectedBox);
            var customDropdown = new Dropdown<TestClass>(new List<TestClass>(), customSelectedBox);

            intDropdown.Items.Should().HaveCount(3);
            stringDropdown.Items.Should().HaveCount(2);
            customDropdown.Items.Should().BeEmpty();
        }

        [Fact]
        public void Width_WithFillMode_WhenParentWidthChanges_ShouldUpdate()
        {
            var parent = new UIElement(200f, 100f);
            var selectedItemBox = new StrongBox<string>(null);
            var dropdown = new Dropdown<string>(new List<string> { "A", "B" }, SizeMode.Fill, selectedItemBox);
            parent.AddChild(dropdown);
            var initialWidth = dropdown.Width;

            parent.Width = 500f;
            var newWidth = dropdown.Width;

            initialWidth.Should().Be(200f);
            newWidth.Should().Be(500f);
        }

        [Fact]
        public void DisplayTextFunc_WithCustomFunction_ShouldFormatCorrectly()
        {
            var items = new List<TestClass>
            {
                new TestClass { Name = "Test1" },
                new TestClass { Name = "Test2" }
            };
            var selectedItemBox = new StrongBox<TestClass>(null);
            var dropdown = new Dropdown<TestClass>(items, selectedItemBox, tc => $"[{tc.Name}]");
            dropdown.SelectedItem = items[0];

            dropdown.SelectedDisplayText.Should().Be("[Test1]");
        }

        [Fact]
        public void StrongBox_SharedBetweenDropdowns_ShouldSynchronizeSelection()
        {
            var items = new List<string> { "A", "B", "C" };
            var sharedSelectedItemBox = new StrongBox<string>(null);
            var dropdown1 = new Dropdown<string>(items, sharedSelectedItemBox);
            var dropdown2 = new Dropdown<string>(items, sharedSelectedItemBox);

            dropdown1.SelectedItem = "B";

            dropdown1.SelectedItem.Should().Be("B");
            dropdown2.SelectedItem.Should().Be("B");
            sharedSelectedItemBox.Value.Should().Be("B");
        }

        private class TestClass
        {
            public string Name { get; set; }

            public override string ToString()
            {
                return Name ?? "Test";
            }
        }
    }
}