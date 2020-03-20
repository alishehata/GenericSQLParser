using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    /// <summary>
    /// Column Expression Specifications Used to store Expression Specifications
    /// </summary>
    public class ColumnFilterExpressions
    {
        /// <summary> Column value to filter with </summary>
        public string FirstValue { get; set; }

        /// <summary> Column value to filter with </summary>
        public string SecondValue { get; set; }

        /// <summary> when using selected Set </summary>
        public IList<string> ElementSet { get; set; } 

        /// <summary> comparision Operation used in filter </summary>
        public string Operand { get; set; }
    }
}