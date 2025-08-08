namespace LessUI
{
    /// <summary>
    /// A grid that fills the parent's width and automatically calculates cell dimensions based on children.
    /// This is a specialized version of Grid that always uses SizeMode.Fill for width.
    /// </summary>
    public class FillGrid : Grid
    {
        /// <summary>
        /// Creates a grid that fills the parent's width and automatically calculates cell height based on children.
        /// </summary>
        /// <param name="columns">Number of columns</param>
        /// <param name="rows">Number of rows</param>
        /// <param name="options">Optional UI element options</param>
        public FillGrid(int columns, int rows, UIElementOptions options = null)
            : base(columns, rows, SizeMode.Fill, SizeMode.Content, options)
        {
            // CellHeight is already set to 30f in the base constructor
            // _autoCalculateCellSize is already set to true in the base constructor
            // CellWidth will be calculated when parent is set
        }
    }
}