using System;
using System.Linq;
using Verse;

namespace LessUI
{
    /// <summary>
    /// A container that arranges its children in a grid layout.
    /// Children are automatically positioned in grid cells as they are added.
    /// </summary>
    public class Grid : UIElement
    {
        private float _columnSpacing = 0f;
        private float _rowSpacing = 0f;
        private float _padding = 0f;
        private float _cellWidth;
        private float _cellHeight;

        public int Columns { get; set; }
        public int Rows { get; set; }

        public float CellWidth
        {
            get => _cellWidth;
            set => _cellWidth = value;
        }

        public float CellHeight
        {
            get => _cellHeight;
            set => _cellHeight = value;
        }

        public float ColumnSpacing
        {
            get => _columnSpacing;
            set
            {
                if (_columnSpacing != value)
                {
                    _columnSpacing = value;
                    // Size recalculation will happen on next Render() call
                }
            }
        }

        public float RowSpacing
        {
            get => _rowSpacing;
            set
            {
                if (_rowSpacing != value)
                {
                    _rowSpacing = value;
                    // Size recalculation will happen on next Render() call
                }
            }
        }

        public float Padding
        {
            get => _padding;
            set
            {
                if (_padding != value)
                {
                    _padding = value;
                    // Size recalculation will happen on next Render() call
                }
            }
        }

        /// <summary>
        /// Creates a grid with fixed cell dimensions and Content sizing for the grid itself.
        /// The grid will size itself based on the cell layout.
        /// </summary>
        /// <param name="columns">Number of columns</param>
        /// <param name="rows">Number of rows</param>
        /// <param name="cellWidth">Width of each cell</param>
        /// <param name="cellHeight">Height of each cell</param>
        /// <param name="options">Optional UI element options</param>
        public Grid(int columns, int rows, float cellWidth, float cellHeight, UIElementOptions options = null)
            : base(SizeMode.Content, SizeMode.Content, options)
        {
            Columns = columns;
            Rows = rows;
            _cellWidth = cellWidth;
            _cellHeight = cellHeight;
        }

        /// <summary>
        /// Creates a grid that automatically calculates cell size based on children.
        /// Uses Content sizing for both width and height.
        /// </summary>
        /// <param name="columns">Number of columns</param>
        /// <param name="rows">Number of rows (0 for unlimited rows)</param>
        /// <param name="options">Optional UI element options</param>
        public Grid(int columns, int rows = 0, UIElementOptions options = null)
            : base(SizeMode.Content, SizeMode.Content, options)
        {
            Columns = columns;
            Rows = rows;
            _cellWidth = 50f; // Default cell width
            _cellHeight = 30f; // Default cell height
        }

        /// <summary>
        /// Creates a grid with custom size modes for maximum flexibility.
        /// Supports any combination: Fixed/Content/Fill for both width and height.
        /// Examples:
        /// - Fill width + Content height: Grid fills parent width, height based on content
        /// - Fixed width + Fill height: Grid has fixed width, fills parent height
        /// - Content width + Fixed height: Grid width based on content, fixed height
        /// </summary>
        /// <param name="columns">Number of columns</param>
        /// <param name="rows">Number of rows (0 for unlimited rows)</param>
        /// <param name="widthMode">Width sizing mode (Fixed/Content/Fill)</param>
        /// <param name="heightMode">Height sizing mode (Fixed/Content/Fill)</param>
        /// <param name="options">Optional UI element options</param>
        public Grid(int columns, int rows, SizeMode widthMode, SizeMode heightMode, UIElementOptions options = null)
            : base(widthMode, heightMode, options)
        {
            Columns = columns;
            Rows = rows;
            _cellWidth = 50f; // Default cell width
            _cellHeight = 30f; // Default cell height
        }

        /// <summary>
        /// Creates a grid with custom size modes and explicit cell dimensions.
        /// Supports any combination of Fixed/Content/Fill for both width and height.
        /// When using Fixed modes, you should also call SetFixedSize() after construction.
        /// </summary>
        /// <param name="columns">Number of columns</param>
        /// <param name="rows">Number of rows (0 for unlimited rows)</param>
        /// <param name="widthMode">Width sizing mode (Fixed/Content/Fill)</param>
        /// <param name="heightMode">Height sizing mode (Fixed/Content/Fill)</param>
        /// <param name="cellWidth">Width of each cell</param>
        /// <param name="cellHeight">Height of each cell</param>
        /// <param name="options">Optional UI element options</param>
        public Grid(int columns, int rows, SizeMode widthMode, SizeMode heightMode,
                   float cellWidth, float cellHeight, UIElementOptions options = null)
            : base(widthMode, heightMode, options)
        {
            Columns = columns;
            Rows = rows;
            _cellWidth = cellWidth;
            _cellHeight = cellHeight;
        }

        /// <summary>
        /// Override fill width calculation to provide cell width instead of grid width.
        /// </summary>
        /// <param name="child">The child element requesting fill width</param>
        /// <returns>The cell width for the child to fill</returns>
        protected override float CalculateFillWidth(UIElement child)
        {
            return CellWidth;
        }

        /// <summary>
        /// Override fill height calculation to provide cell height instead of grid height.
        /// </summary>
        /// <param name="child">The child element requesting fill height</param>
        /// <returns>The cell height for the child to fill</returns>
        protected override float CalculateFillHeight(UIElement child)
        {
            return CellHeight;
        }

        /// <summary>
        /// Calculates the width needed to contain the grid based on cell dimensions.
        /// </summary>
        /// <returns>The calculated content width</returns>
        protected override float CalculateContentWidth()
        {
            return (CellWidth * Columns) + (ColumnSpacing * (Columns - 1)) + (Padding * 2);
        }

        /// <summary>
        /// Calculates the height needed to contain the grid based on cell dimensions and rows.
        /// </summary>
        /// <returns>The calculated content height</returns>
        protected override float CalculateContentHeight()
        {
            // Calculate actual rows needed
            int actualRows = Rows > 0 ? Rows : (int)Math.Ceiling((double)Children.Count / Columns);
            if (actualRows == 0) actualRows = 1; // Minimum one row

            return (CellHeight * actualRows) + (RowSpacing * (actualRows - 1)) + (Padding * 2);
        }

        public override void CalculateDynamicSize()
        {
            base.CalculateDynamicSize();

            foreach (var child in Children)
            {
                child.CalculateDynamicSize();
            }

            if (WidthMode == SizeMode.Fixed)
            {
                CellWidth = Width - (Padding * 2) - (ColumnSpacing * (Columns - 1));
                CellWidth = Math.Max(1f, CellWidth / Columns);
            }
            else if (WidthMode == SizeMode.Content)
            {
                CellWidth = Children.Max(child => child.Width);
            }
            else if (WidthMode == SizeMode.Fill)
            {
                if (Parent != null)
                {
                    CellWidth = Parent.Width - (Padding * 2) - (ColumnSpacing * (Columns - 1));
                    CellWidth = Math.Max(1f, CellWidth / Columns);
                }
                else
                {
                    throw new InvalidOperationException("NEED PARENT");
                }
            }

            if (HeightMode == SizeMode.Fixed)
            {
                CellHeight = Height - (Padding * 2) - (RowSpacing * (Rows - 1));
                CellHeight = Math.Max(1f, CellHeight / Rows);
            }
            else if (HeightMode == SizeMode.Content)
            {
                CellHeight = Children.Max(child => child.Height);
            }
            else if (HeightMode == SizeMode.Fill)
            {
                if (Parent != null)
                {
                    CellHeight = Parent.Height - (Padding * 2) - (RowSpacing * (Rows - 1));
                    CellHeight = Math.Max(1f, CellHeight / Rows);
                }
                else
                {
                    throw new InvalidOperationException("NEED PARENT");
                }
            }

            base.CalculateDynamicSize();
        }

        public override void Render()
        {
            // The base.Render() call will handle CalculateDynamicSize(), which includes
            // our auto-calculation logic, so we don't need to call it separately here
            base.Render();
        }

        /// <summary>
        /// Overrides the default line layout to arrange children in a grid layout instead.
        /// </summary>
        protected override void LayoutChildren()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var (column, row) = GetGridPosition(i);

                // Skip if we exceed our row limit (when Rows > 0)
                if (Rows > 0 && row >= Rows) break;

                float cellX = X + Padding + (column * (CellWidth + ColumnSpacing));
                float cellY = Y + Padding + (row * (CellHeight + RowSpacing));

                var alignedPosition = CalculateAlignedPosition(child, cellX, cellY);
                child.X = alignedPosition.x;
                child.Y = alignedPosition.y;
            }
        }

        private (float x, float y) CalculateAlignedPosition(UIElement child, float cellX, float cellY)
        {
            float alignedX = CalculateAlignedX(child.Alignment, cellX, child.Width);
            float alignedY = CalculateAlignedY(child.Alignment, cellY, child.Height);

            return (alignedX, alignedY);
        }

        private float CalculateAlignedX(Align alignment, float cellX, float childWidth)
        {
            // If child is larger than or equal to cell width, position at left edge of cell
            if (childWidth >= CellWidth)
            {
                return cellX;
            }

            switch (alignment)
            {
                case Align.UpperLeft:
                case Align.MiddleLeft:
                case Align.LowerLeft:
                    return cellX;

                case Align.UpperCenter:
                case Align.MiddleCenter:
                case Align.LowerCenter:
                    return cellX + (CellWidth - childWidth) / 2f;

                case Align.UpperRight:
                case Align.MiddleRight:
                case Align.LowerRight:
                    return cellX + (CellWidth - childWidth);

                default:
                    return cellX;
            }
        }

        private float CalculateAlignedY(Align alignment, float cellY, float childHeight)
        {
            // If child is larger than or equal to cell height, position at top edge of cell
            if (childHeight >= CellHeight)
            {
                return cellY;
            }

            switch (alignment)
            {
                case Align.UpperLeft:
                case Align.UpperCenter:
                case Align.UpperRight:
                    return cellY;

                case Align.MiddleLeft:
                case Align.MiddleCenter:
                case Align.MiddleRight:
                    return cellY + (CellHeight - childHeight) / 2f;

                case Align.LowerLeft:
                case Align.LowerCenter:
                case Align.LowerRight:
                    return cellY + (CellHeight - childHeight);

                default:
                    return cellY;
            }
        }

        private (int column, int row) GetGridPosition(int childIndex)
        {
            int column = childIndex % Columns;
            int row = childIndex / Columns;
            return (column, row);
        }

        /// <summary>
        /// Gets the child at the specified grid position.
        /// </summary>
        /// <param name="column">Column index (0-based)</param>
        /// <param name="row">Row index (0-based)</param>
        /// <returns>The child element at that position, or null if empty</returns>
        public UIElement GetChildAt(int column, int row)
        {
            int index = (row * Columns) + column;
            return index < Children.Count ? Children[index] : null;
        }

        /// <summary>
        /// Inserts a child at a specific grid position, shifting other children as needed.
        /// </summary>
        /// <param name="child">The child to insert</param>
        /// <param name="column">Column index</param>
        /// <param name="row">Row index</param>
        public void InsertChildAt(UIElement child, int column, int row)
        {
            if (child == null) return;

            int targetIndex = (row * Columns) + column;

            // Ensure the index is valid
            if (targetIndex < 0) targetIndex = 0;
            if (targetIndex > Children.Count) targetIndex = Children.Count;

            // Remove from old parent if needed
            if (child.Parent != null)
            {
                child.Parent.RemoveChild(child);
            }

            // Insert at the target position
            Children.Insert(targetIndex, child);
            child.Parent = this;
        }

        /// <summary>
        /// Gets the grid position of a specific child.
        /// </summary>
        /// <param name="child">The child element</param>
        /// <returns>The grid position, or (-1, -1) if not found</returns>
        public (int column, int row) GetPositionOfChild(UIElement child)
        {
            int index = Children.IndexOf(child);
            return index >= 0 ? GetGridPosition(index) : (-1, -1);
        }

        /// <summary>
        /// Gets the maximum number of children this grid can hold.
        /// Returns -1 for unlimited (when Rows = 0).
        /// </summary>
        public int MaxCapacity => Rows > 0 ? Columns * Rows : -1;

        /// <summary>
        /// Gets the number of available empty cells.
        /// Returns -1 for unlimited grids.
        /// </summary>
        public int AvailableCells
        {
            get
            {
                int maxCapacity = MaxCapacity;
                return maxCapacity > 0 ? maxCapacity - Children.Count : -1;
            }
        }

        /// <summary>
        /// Checks if the grid is full.
        /// </summary>
        public bool IsFull => AvailableCells == 0;

        /// <summary>
        /// Gets the actual number of rows currently used by children.
        /// </summary>
        public int ActualRows => Children.Count > 0 ? (int)Math.Ceiling((double)Children.Count / Columns) : 0;
    }
}