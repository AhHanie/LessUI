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

        public int Columns { get; set; } = 1;
        public int Rows { get; set; } = 0;

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

            if (WidthMode == SizeMode.Fixed && WidthCalculated)
            {
                CalculateCellWidthFromFixed();
            }

            if (HeightMode == SizeMode.Fixed && HeightCalculated)
            {
                CalculateCellHeightFromFixed();
            }
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

            if (WidthMode == SizeMode.Fixed && WidthCalculated)
            {
                CalculateCellWidthFromFixed();
            }

            if (HeightMode == SizeMode.Fixed && HeightCalculated)
            {
                CalculateCellHeightFromFixed();
            }
        }

        public override void CalculateFillSize()
        {
            base.CalculateFillSize();

            if (WidthMode == SizeMode.Fill && Parent != null)
            {
                CalculateCellWidthFromFill();
            }

            if (HeightMode == SizeMode.Fill && Parent != null)
            {
                CalculateCellHeightFromFill();
            }
        }

        private void CalculateCellWidthFromFixed()
        {
            CellWidth = (Width - (Padding * 2) - (ColumnSpacing * (Columns - 1))) / Columns;
            CellWidth = Math.Max(1f, CellWidth);
        }

        private void CalculateCellHeightFromFixed()
        {
            CellHeight = (Height - (Padding * 2) - (RowSpacing * (Rows - 1))) / Rows;
            CellHeight = Math.Max(1f, CellHeight);
        }

        private void CalculateCellWidthFromFill()
        {
            if (Parent != null)
            {
                float availableWidth = Parent.CalculateFillWidth(this);
                CellWidth = (availableWidth - (Padding * 2) - (ColumnSpacing * (Columns - 1))) / Columns;
                CellWidth = Math.Max(1f, CellWidth);
            }
        }

        private void CalculateCellHeightFromFill()
        {
            if (Parent != null)
            {
                float availableHeight = Parent.CalculateFillHeight(this);
                CellHeight = (availableHeight - (Padding * 2) - (RowSpacing * (Rows - 1))) / Rows;
                CellHeight = Math.Max(1f, CellHeight);
            }
        }

        public override float CalculateFillWidth(UIElement child)
        {
            return CellWidth;
        }

        public override float CalculateFillHeight(UIElement child)
        {
            return CellHeight;
        }

        protected override float CalculateContentWidthFromChildren()
        {
            base.CalculateContentHeightFromChildren();
            CellWidth = Children.Max(child => child.Width);
            return (CellWidth * Columns) + (ColumnSpacing * (Columns - 1)) + (Padding * 2);
        }

        protected override float CalculateContentHeightFromChildren()
        {
            base.CalculateContentHeightFromChildren();
            CellHeight = Children.Max(child => child.Height);
            int actualRows = Rows > 0 ? Rows : (int)Math.Ceiling((double)Children.Count / Columns);
            if (actualRows == 0) actualRows = 1;

            return (CellHeight * actualRows) + (RowSpacing * (actualRows - 1)) + (Padding * 2);
        }

        protected override void LayoutChildren()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var (column, row) = GetGridPosition(i);

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

        public int ActualRows => Children.Count > 0 ? (int)Math.Ceiling((double)Children.Count / Columns) : 0;
    }
}