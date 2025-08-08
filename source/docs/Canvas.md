# Canvas Class Documentation

## Overview

The `Canvas` class is the root container for all LessUI elements. It bridges between Unity's `Rect` system and LessUI's `UIElement` system, handling all Unity-specific rendering setup automatically.

## Key Features

- **Root Container**: Acts as the top-level parent for all UI elements
- **Unity Integration**: Automatically handles `DrawMenuSection`, `BeginGroup`, and `EndGroup` calls
- **Fill Support**: Provides dimensions for child elements using `SizeMode.Fill`
- **Dynamic Updates**: Can update its rect and properly invalidate child elements

## Basic Usage

### Simple Grid Layout

```csharp
public static void Draw(Rect parent)
{
    var canvas = new Canvas(parent);
    var grid = new FillGrid(3, 3);
    grid.ColumnSpacing = 20f;
    
    for (int i = 0; i < 6; i++)
    {
        grid.AddChild(new Label($"Item {i + 1}"));
    }
    
    canvas.AddChild(grid);
    canvas.Render(); // Handles all Unity calls automatically
}
```

### Complex Layout

```csharp
public static void DrawComplexUI(Rect parent)
{
    var canvas = new Canvas(parent);
    
    // Header
    var header = new Label("My Panel");
    header.X = 10f;
    header.Y = 10f;
    
    // Content area with FillGrid
    var contentGrid = new FillGrid(2, 4);
    contentGrid.X = 10f;
    contentGrid.Y = 50f; // Below header
    contentGrid.ColumnSpacing = 15f;
    contentGrid.RowSpacing = 8f;
    contentGrid.Padding = 5f;
    
    // Add content
    for (int i = 0; i < 8; i++)
    {
        contentGrid.AddChild(new Label($"Content {i + 1}"));
    }
    
    canvas.AddChild(header);
    canvas.AddChild(contentGrid);
    canvas.Render();
}
```

## Constructor

### `Canvas(Rect rect, UIElementOptions options = null)`

Creates a new Canvas from a Unity Rect.

**Parameters:**
- `rect`: The Unity Rect that defines the canvas bounds
- `options`: Optional UI element options (alignment, etc.)

**Example:**
```csharp
var rect = new Rect(0f, 0f, 800f, 600f);
var canvas = new Canvas(rect);
```

## Methods

### `void UpdateRect(Rect rect)`

Updates the canvas to use a new Unity Rect. This is useful for responsive layouts or when the available space changes.

**Parameters:**
- `rect`: The new Unity Rect

**Example:**
```csharp
// Resize canvas when window size changes
canvas.UpdateRect(new Rect(0f, 0f, newWidth, newHeight));
```

### `Rect GetRect()`

Returns the current Unity Rect that represents this canvas.

**Returns:** The Unity Rect

### `override void Render()`

Renders the canvas and all its children. Automatically handles:
- `Widgets.DrawMenuSection(rect)`
- `Widgets.BeginGroup(rect)`
- Rendering all child elements
- `Widgets.EndGroup()`

## Properties

The Canvas inherits all properties from `UIElement`:

- `X`, `Y`: Position (from the rect)
- `Width`, `Height`: Dimensions (from the rect)
- `Children`: Collection of child UI elements
- `WidthMode`, `HeightMode`: Always `SizeMode.Fixed`

## Integration with FillGrid

Canvas works seamlessly with `FillGrid`:

```csharp
var canvas = new Canvas(new Rect(0f, 0f, 600f, 400f));
var fillGrid = new FillGrid(3, 2); // 3 columns, 2 rows
fillGrid.ColumnSpacing = 10f;
fillGrid.Padding = 20f;

canvas.AddChild(fillGrid);

// FillGrid will automatically:
// - Use canvas width (600f) as its width
// - Calculate cell width: (600 - 2*20 - 2*10) / 3 = 180f per cell
```

## Best Practices

1. **Always use Canvas as root**: Never render UIElements directly without a Canvas parent
2. **One Canvas per Draw method**: Each UI drawing method should have exactly one Canvas
3. **Update rect when needed**: If your UI area can change size, use `UpdateRect()`
4. **Position relative to canvas**: All child elements are positioned relative to the canvas, not absolute screen coordinates

## Migration from Direct Rendering

**Before (manual Unity calls):**
```csharp
public static void Draw(Rect parent)
{
    Widgets.DrawMenuSection(parent);
    Widgets.BeginGroup(parent);
    
    var grid = new Grid(3, 3);
    // ... setup grid ...
    grid.Render();
    
    Widgets.EndGroup();
}
```

**After (with Canvas):**
```csharp
public static void Draw(Rect parent)
{
    var canvas = new Canvas(parent);
    var grid = new FillGrid(3, 3); // Can now use FillGrid!
    // ... setup grid ...
    canvas.AddChild(grid);
    canvas.Render(); // Handles everything automatically
}
```

## Error Handling

The Canvas class includes proper error handling:
- `Render()` ensures `EndGroup()` is always called, even if an exception occurs
- `UpdateRect()` safely invalidates child elements
- Null checks prevent crashes from invalid operations