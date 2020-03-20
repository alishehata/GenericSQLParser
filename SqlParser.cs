using Enums;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parser
{
	public class SqlParser
	{
		/// <summary>
		/// column Expressions to be mapped to sql query
		/// </summary>
		private readonly IEnumerable<ColumnExpression> columnExpressions;

		/// <summary>
		/// column Expressions to be mapped to sql query
		/// </summary>
		private readonly IEnumerable<ColumnSortExpressions> columnSortExpressions;

		/// <summary>
		/// mapping dictionary between sql and column Expressions data
		/// </summary>
		private IEnumerable<KeyValuePair<string, string>> sqlColumnsMap;

		/// <summary>
		/// SQL Where Clause Query String
		/// </summary>
		public string FilterExpressionsString { get; set; } = string.Empty;

		/// <summary>
		/// Sorting Options String
		/// </summary>
		public string SortExpressionsString { get; set; } = string.Empty;

		public SqlParser(IEnumerable<ColumnExpression> columnExpressions, IEnumerable<ColumnSortExpressions> columnSortExpressions)
		{
			this.columnExpressions = columnExpressions;
			this.columnSortExpressions = columnSortExpressions;
		}

		/// <summary>
		/// Stringify comming column Expressions into sql where clause string 
		/// </summary>
		/// <param name="sqlColumnsNamesMapSorting"></param>
		/// <returns></returns>
		public string StringifyColumnSortExpressions(Dictionary<string, string> sqlColumnsNamesMapSorting)
		{
			this.sqlColumnsMap = sqlColumnsNamesMapSorting;

			var fullColumnSortExpressionsString = string.Empty;
			var columnSortOptionsStringList = new List<string>();

			if (columnSortExpressions != null)
			{

				foreach (var sortOption in columnSortExpressions)
				{
					var colName = this.GetSQLColumnName(sortOption.ColumnName);
					if (!string.IsNullOrWhiteSpace(colName))
					{
						columnSortOptionsStringList.Add($"{colName} {sortOption.OrderType}");
					}
				}
			}

			fullColumnSortExpressionsString = $" {string.Join(",", columnSortOptionsStringList)}";

			return  $" {fullColumnSortExpressionsString} ";
		}

		/// <summary>
		/// Stringify comming column Expressions into sql where clause string 
		/// </summary>
	    /// <param name="sqlColumnsNamesMapFilter"></param>
		/// <returns></returns>
		public string StringifyColumnFilterExpressionsToSqlQuery(Dictionary<string, string> sqlColumnsNamesMapFilter)
		{
			this.sqlColumnsMap = sqlColumnsNamesMapFilter;

			var fullColumnFilterExpressionsString = string.Empty; 
			var columnFilterExpressionStringList = new List<string>(); 
			 
			if (this.columnExpressions == null)
			{
				return string.Empty;
			}

			foreach (var expression in this.columnExpressions)
			{
				// expression text value 
				var expressionText = string.Empty; 

				// column name 
				var colName = this.GetSQLColumnName(expression.ColumnName);

				// generate stringified expression
				if (!string.IsNullOrWhiteSpace(colName)
					&& expression.ColumnFilterExpressions != null
					&& ( !string.IsNullOrWhiteSpace(expression.ColumnFilterExpressions?.FirstValue)
					    || expression.ColumnFilterExpressions.ElementSet != null )
					&& !string.IsNullOrWhiteSpace(expression.ColumnFilterExpressions?.Operand))
				{
					switch (expression.ColumnDataType)
					{
						case ColumnDataType.Text:
							expressionText = this.GetStringifiedExpression(expression, colName);
							break;
						case ColumnDataType.Date:
							expressionText = this.GetStringifiedExpression(expression, colName);
							break;
						case ColumnDataType.Numeric:
							expressionText = this.GetStringifiedExpression(expression, colName);
							break;
						case ColumnDataType.Boolean:
							expressionText = this.GetStringifiedExpression(expression, colName);
							break;
						default:
							expressionText = string.Empty;
							break;
					}
				}
				 
				if (!string.IsNullOrWhiteSpace(expressionText))
				{
					columnFilterExpressionStringList.Add(string.Format(" {0} ", expressionText));
				}
			}
			 
			// join to generate the full sql where clause string
			fullColumnFilterExpressionsString = string.Join(" AND ", columnFilterExpressionStringList);

			return $" {fullColumnFilterExpressionsString} ";
		}

		/// <summary>
		/// Stringifiy the Column Expression
		/// </summary> 
		/// <param name="expression"></param>
		/// <param name="sqlColName"></param>
		/// <returns>return Column Expression string</returns>
		private string GetStringifiedExpression(ColumnExpression expression, string sqlColName)
		{
			var expressionText = string.Empty;

			if (string.IsNullOrWhiteSpace(expression.ColumnFilterExpressions.FirstValue) 
				&& expression.ColumnFilterExpressions.ElementSet == null)
			{
				return expressionText;
			}

			switch (expression.ColumnFilterExpressions.Operand.Trim().ToLower())
			{
				case ExpressionOperands.Contains:
					expressionText = $" {sqlColName} Like '%{EscapeWildCardChars(expression.ColumnFilterExpressions.FirstValue)}%' ESCAPE '/' ";
					break;
				case ExpressionOperands.NotContains:
					expressionText = $" {sqlColName} NOT Like '%{EscapeWildCardChars(expression.ColumnFilterExpressions.FirstValue)}%'  ESCAPE '/' ";
					break;
				case ExpressionOperands.StartsWith:
					expressionText = $" {sqlColName} Like '{EscapeWildCardChars(expression.ColumnFilterExpressions.FirstValue)}%'  ESCAPE '/' ";
					break;
				case ExpressionOperands.EndsWith:
					expressionText = $" {sqlColName} Like '%{EscapeWildCardChars(expression.ColumnFilterExpressions.FirstValue)}'  ESCAPE '/' ";
					break;
				case ExpressionOperands.Matches:
					expressionText = $" {sqlColName} Like '{expression.ColumnFilterExpressions.FirstValue}' ";
					break;
				case ExpressionOperands.In:
					var isNullElementSet = expression.ColumnFilterExpressions.Operand == ExpressionOperands.In
						&& (expression.ColumnFilterExpressions.ElementSet == null
						|| expression.ColumnFilterExpressions.ElementSet.Count == 0);

					if (!isNullElementSet)
					{
						var elementSetString = StringifiyElementSet(expression.ColumnFilterExpressions.ElementSet, expression.ColumnDataType);

						if (!string.IsNullOrWhiteSpace(elementSetString))
						{
							expressionText = $" {sqlColName} IN ({elementSetString})";
						}
					}
					break;
				case ExpressionOperands.Between:
					if (!string.IsNullOrWhiteSpace(expression.ColumnFilterExpressions.SecondValue))
					{
						if (DateTime.TryParse(expression.ColumnFilterExpressions.FirstValue, out DateTime firstDateValue)
							&& DateTime.TryParse(expression.ColumnFilterExpressions.SecondValue, out DateTime secondDateValue))
						{
							expressionText = $" {sqlColName} BETWEEN CONVERT(datetime,'{firstDateValue.ToString("yyyy-MM-dd")}') AND CONVERT(datetime,'{secondDateValue.ToString("yyyy-MM-dd")}') ";
						}
					}

					break;
				default:
					if (expression.ColumnDataType == ColumnDataType.Date)
					{
						if (!DateTime.TryParse(expression.ColumnFilterExpressions.FirstValue, out DateTime firstDateValue))
						{
							break;
						}
						expressionText = $" {sqlColName} {expression.ColumnFilterExpressions.Operand} CONVERT(datetime,'{firstDateValue.ToString("yyyy-MM-dd")}') ";
						break;
					}
					else
					{
						expressionText = $" {sqlColName} {expression.ColumnFilterExpressions.Operand} {expression.ColumnFilterExpressions.FirstValue} ";
					}
					break;
			}

			return expressionText;
		}

		/// <summary>
		/// Escape WildCard Chars for SQL query
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private string EscapeWildCardChars(string value)
		{
			value = value.Replace("/", "//");
			var wildCardChars = new List<string>
			{
				 "%", "^", "_", "]", "[", "&", "?", "!", "-", "#", "*"
			};

			foreach (var wildcard in wildCardChars)
			{
				value = value.Replace(wildcard, string.Format("/{0}", wildcard));
			}

			return value;
		}

		/// <summary>
		/// Stringifiy the sql IN Element Set
		/// </summary>
		/// <param name="elementSet"></param>
		/// <param name="columnDataType"></param>
		/// <returns>return list converted to string</returns>
		private string StringifiyElementSet(IList<string> elementSet, ColumnDataType columnDataType)
		{
			var elementSetString = string.Empty;

			switch (columnDataType)
			{
				case ColumnDataType.Numeric:
					elementSetString = string.Join(",", elementSet);
					break;
				case ColumnDataType.Date:

					for (var counter = 0; counter < elementSet.Count; counter++)
					{
						var element = elementSet[counter];

						if (DateTime.TryParse(element, out DateTime dateValue))
						{
							elementSet[counter] = string.Format("CONVERT(datetime,'{0}')", dateValue.ToString("yyyy-MM-dd"));
						}
						else
						{
							return string.Empty;
						}
					}
					elementSetString = string.Join(",", elementSet);
					break;
				case ColumnDataType.Text:

					for (var counter = 0; counter < elementSet.Count; counter++)
					{
						var element = elementSet[counter];
						elementSet[counter] = string.Format("'{0}'", element);
					}
					elementSetString = string.Join(",", elementSet);
					break;
				case ColumnDataType.Boolean:
					for (var counter = 0; counter < elementSet.Count; counter++)
					{
						var element = elementSet[counter]; 
						var result = false;

						if (bool.TryParse(element.ToLower(), out result))
						{
							elementSet[counter] = result ? "1" : "0";
						}
						else
						{
							elementSet[counter] = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(elementSet[counter]))
						{
							break;
						}
					}
					elementSetString = string.Join(",", elementSet);
					break;
				default:
					elementSetString = string.Empty;
					break;
			}

			return elementSetString;
		}

		/// <summary>
		/// Map Client Side Params to SQL Column Names
		/// </summary>
		/// <returns>returns acutal column name </returns>
		public string GetSQLColumnName(string colName)
		{
			var actualColName = string.Empty;

			if (!string.IsNullOrWhiteSpace(colName))
			{
				if (this.sqlColumnsMap.ToDictionary().ContainsKey(colName.ToLower()))
				{
					this.sqlColumnsMap.ToDictionary().TryGetValue(colName.ToLower(), out actualColName);
				}
				else
				{
					actualColName = colName;
				}
			}
			else
			{
				actualColName = string.Empty;
			}
			return actualColName;
		}
	}
}
