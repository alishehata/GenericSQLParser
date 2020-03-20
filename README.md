# GenericSQLParser
"SQL WHERE" clause expressions parser and query building.
SQL Parser Documentation for filtering and sorting

Sql parser includes filtering and sorting for all columns with many criteria, you can find the documentation of the filtering and sorting request and all the cases of the payload in the following lines.
 
Payload Example:
  
 
 

Filter Request structure:

In the “filterParams” query parameter we will send an array of objects to represent multiple filtering among the columns, and here the structure of the object for column filtering.
•	Column Name – "columnName": provide the name of the column used in filtering.
•	Column Data Type – "columnDataType":  Column data type to send the filter column data type and here’s the available data types that can be used in the request

1-	Text Data Type      mapped to 0
2-	Date Data Type      mapped to 1
3-	Numeric Data Type   mapped to 2
4-	Boolean Data Type   mapped to 3

•	Expression Specifications - "expressionSpecifications": 
this json object used to specify the Expression filtering criteria and here the description, of all the cases of the expression, 

1-	First Value - "firstValue": contains all the types in a text representation 
2-	Second Value – "secondValue": contains another value in case of date
3-	Operand - "operand": the available columns expressions operands can be one of the following.

- here is the description of all the operands represented as 

Operand Name – “operand value”: operand description 

•	Contains – “contains”: check if the column data contains the first value content.
Used With: Text

•	Not Contains – “not-contains”: 
check if the column data doesn’t contain the first value content.
Used With: Text

•	Matches – “matches”: 
check if the column data doesn’t contain the first value content.
Used With: Text

•	Starts With – “starts-with”:
check if the column data starts with the first value content.
Used With: Text

•	Ends  With – “ends-with”:
check if the column data ends with the first value content.
Used With: Text

•	Between – “between”: check if the column data is between two provided dates First Value and Second Value.
Used With: Date 
Example: 
 "expressionSpecifications":{
    "firstValue":"2019-11-02",
    "secondValue":"2018-10-16”, 
                           "operand": "between" 
                    }
•	In – “in”: used in filtering with SET of elements to choose from multiple elements from the element set provided with the request 
Used With: Array of (Dates, Text, Numbers, Boolean) 
Example:
 "elementSet":["2018-10-16", "2018-11-4"]

•	Equal – “=”: used to check equality 
Used With: Numbers, Text, Dates, Boolean

•	Not Equal – “<>”: used to check that the column not equal to the first value parameter value can be used in these type
Used With: Numbers, Text, Dates, Boolean.

•	Greater Than – “>”:  used to check that the column value greater than the first value parameter value can be used in these type
Used With: Numbers, Dates

•	Less Than – “<”: 
used to check that the column value less than the first value parameter value can be used in these type
Used With: Numbers, Dates

•	Greater Than or Equal – “>=”:
used to check that the column value greater than or equal the first value parameter value can be used in these type
Used With: Numbers, Dates.

•	Less Than or Equal – “<=”:
used to check that the column value less  than or equal the first value parameter value can be used in these type
Used With: Numbers, Dates.

4-	Element Set – “elementSet”: represent an array of any type of the used data types  Used With “in” operand 




Sorting Request structure:

In the “sortOptions” query parameter we will send an array of objects to represent the columns we want to order with it this support multiple column sorting, and here the structure of the object for column sorting.
•	Column Name – "columnName": provide the name of the column used in filtering.
•	Order Type – "orderType": order type can be the following:

1-	Ascending Ordering   mapped to “ASC”
2-	Descending Order     mapped to “DECS” 





