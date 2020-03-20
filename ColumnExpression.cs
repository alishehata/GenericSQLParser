using Enums;

namespace Models
{
    /// <summary>
    /// Column Expression to store Column Expression specs
    /// </summary>
    public class ColumnExpression
    {
        /// <summary> Column name to query with it </summary>
        public string ColumnName { get; set; }

        /// <summary> Expression Specifications </summary>
        public ColumnFilterExpressions ColumnFilterExpressions { get; set; }

        /// <summary> column data type </summary>
        public ColumnDataType ColumnDataType { get; set; }
    }
}
