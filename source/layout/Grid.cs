using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace LessUI
{
    public class Grid : UIElement
    {
        private float _columnSpacing = 0f;
        private float _rowSpacing = 0f;
        private float _padding = 0f;
        private float _cellWidth = 50f;
        private float _cellHeight = 30f;
        private bool _explicitCellSize = false;

        public int Columns { get; set; } = 1;
        public int Rows { get; set; } = 0;

        public float CellWidth
        {
            get => _cellWidth;
            set
            {
                _cellWidth = value;
                _explicitCellSize = true;
                InvalidateLayout();
            }
        }

        public float CellHeight
        {
            get => _cellHeight;
            set
            {
                _cellHeight = value;
                _explicitCellSize = true;
                InvalidateLayout();
            }
        }

        public float ColumnSpacing
        {
            get => _columnSpacing;
            set
            {
                if (_columnSpacing != value)
                {
                    _columnSpacing = value;
                    InvalidateLayout();
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
                    InvalidateLayout();
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
                    InvalidateLayout();
                }
            }
        }

        public Grid(
            int columns = 1,
            int rows = 0,
            float? cellWidth = null,
            float? cellHeight = null,
            float? columnSpacing = null,
            float? rowSpacing = null,
            float? padding = null,
            float? x = null,
            float? y = null,
            float? width = null,
            float? height = null,
            SizeMode? widthMode = null,
            SizeMode? heightMode = null,
            Align? alignment = null,
            bool? showBorders = null,
            Color? borderColor = null,
            int? borderThickness = null)
            : base(x, y, width, height, widthMode, heightMode, alignment, showBorders, borderColor, borderThickness)
        {
            Columns = columns;
            Rows = rows;
            _cellWidth = cellWidth ?? 50f;
            _cellHeight = cellHeight ?? 30f;
            _columnSpacing = columnSpacing ?? 0f;
            _rowSpacing = rowSpacing ?? 0f;
            _padding = padding ?? 0f;
            _explicitCellSize = cellWidth.HasValue || cellHeight.HasValue;
        }

        public Grid(
            List<UIElement> children,
            int columns = 1,
            int rows = 0,
            float? cellWidth = null,
            float? cellHeight = null,
            float? columnSpacing = null,
            float? rowSpacing = null,
            float? padding = null,
            float? x = null,
            float? y = null,
            float? width = null,
            float? height = null,
            SizeMode? widthMode = null,
            SizeMode? heightMode = null,
            Align? alignment = null,
            bool? showBorders = null,
            Color? borderColor = null,
            int? borderThickness = null)
            : base(children, x, y, width, height, widthMode, heightMode, alignment, showBorders, borderColor, borderThickness)
        {
            Columns = columns;
            Rows = rows;
            _cellWidth = cellWidth ?? 50f;
            _cellHeight = cellHeight ?? 30f;
            _columnSpacing = columnSpacing ?? 0f;
            _rowSpacing = rowSpacing ?? 0f;
            _padding = padding ?? 0f;
            _explicitCellSize = cellWidth.HasValue || cellHeight.HasValue;
        }

        protected override Size ComputeIntrinsicSize()
        {
            foreach (var child in Children)
            {
                if (child.NeedsLayout)
                {
                    child.CalculateIntrinsicSize();
                }
            }

            var gridSize = CalculateGridIntrinsicSize();
            return gridSize;
        }

        private Size CalculateGridIntrinsicSize()
        {
            if (!_explicitCellSize && Children.Any())
            {
                _cellWidth = Children.Max(child => child.IntrinsicSize.width);
                _cellHeight = Children.Max(child => child.IntrinsicSize.height);
            }

            int actualRows = CalculateActualRows();

            float intrinsicWidth = (Columns * _cellWidth) + ((Columns - 1) * _columnSpacing) + (2 * _padding);
            float intrinsicHeight = (actualRows * _cellHeight) + ((actualRows - 1) * _rowSpacing) + (2 * _padding);

            return new Size(intrinsicWidth, intrinsicHeight);
        }

        private int CalculateActualRows()
        {
            if (Rows > 0)
                return Rows;

            if (Children.Count == 0)
                return 1;

            return (int)Math.Ceiling((double)Children.Count / Columns);
        }

        protected override Size ComputeResolvedSize(Size availableSize)
        {
            var resolvedSize = base.ComputeResolvedSize(availableSize);

            if (WidthMode == SizeMode.Fill || HeightMode == SizeMode.Fill)
            {
                RecalculateCellSizesForFill(resolvedSize);
            }

            return resolvedSize;
        }

        private void RecalculateCellSizesForFill(Size resolvedSize)
        {
            if (WidthMode == SizeMode.Fill)
            {
                float availableWidth = resolvedSize.width - (2 * _padding) - ((Columns - 1) * _columnSpacing);
                _cellWidth = Math.Max(1f, availableWidth / Columns);
            }

            if (HeightMode == SizeMode.Fill && Rows > 0)
            {
                float availableHeight = resolvedSize.height - (2 * _padding) - ((Rows - 1) * _rowSpacing);
                _cellHeight = Math.Max(1f, availableHeight / Rows);
            }
        }

        protected override void LayoutChildren()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var (column, row) = GetGridPosition(i);

                if (Rows > 0 && row >= Rows) break;

                float cellX = ComputedX + _padding + (column * (_cellWidth + _columnSpacing));
                float cellY = ComputedY + _padding + (row * (_cellHeight + _rowSpacing));

                var alignedPosition = CalculateAlignedPosition(child, cellX, cellY);
                child.X = alignedPosition.x;
                child.Y = alignedPosition.y;

                var cellSize = new Size(_cellWidth, _cellHeight);
                child.ResolveLayout(cellSize);
            }
        }

        private (float x, float y) CalculateAlignedPosition(UIElement child, float cellX, float cellY)
        {
            float alignedX = CalculateAlignedX(child.Alignment, cellX, child.ComputedWidth);
            float alignedY = CalculateAlignedY(child.Alignment, cellY, child.ComputedHeight);

            return (alignedX, alignedY);
        }

        private float CalculateAlignedX(Align alignment, float cellX, float childWidth)
        {
            if (childWidth >= _cellWidth)
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
                    return cellX + (_cellWidth - childWidth) / 2f;

                case Align.UpperRight:
                case Align.MiddleRight:
                case Align.LowerRight:
                    return cellX + (_cellWidth - childWidth);

                default:
                    return cellX;
            }
        }

        private float CalculateAlignedY(Align alignment, float cellY, float childHeight)
        {
            if (childHeight >= _cellHeight)
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
                    return cellY + (_cellHeight - childHeight) / 2f;

                case Align.LowerLeft:
                case Align.LowerCenter:
                case Align.LowerRight:
                    return cellY + (_cellHeight - childHeight);

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

        public UIElement GetChildAt(int column, int row)
        {
            int index = (row * Columns) + column;
            return index < Children.Count ? Children[index] : null;
        }

        public void InsertChildAt(UIElement child, int column, int row)
        {
            if (child == null) return;

            int targetIndex = (row * Columns) + column;

            if (targetIndex < 0) targetIndex = 0;
            if (targetIndex > Children.Count) targetIndex = Children.Count;

            if (child.Parent != null)
            {
                child.Parent.RemoveChild(child);
            }

            Children.Insert(targetIndex, child);
            child.Parent = this;
            InvalidateLayout();
        }

        public (int column, int row) GetPositionOfChild(UIElement child)
        {
            int index = Children.IndexOf(child);
            return index >= 0 ? GetGridPosition(index) : (-1, -1);
        }

        public int MaxCapacity => Rows > 0 ? Columns * Rows : -1;

        public int AvailableCells
        {
            get
            {
                int maxCapacity = MaxCapacity;
                return maxCapacity > 0 ? maxCapacity - Children.Count : -1;
            }
        }

        public bool IsFull => AvailableCells == 0;

        public int ActualRows => CalculateActualRows();
    }
}