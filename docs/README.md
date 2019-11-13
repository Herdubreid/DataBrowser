## Data Browser Features

### Syntax Highlighting Query Editor

![Editor Syntax Highlighting](./editor.gif)

Syntax highlighting has been around for few decades and considered an indispensable feature of any code editor.  Having recognised terms styled in italic like the subject or color coded like the `all` keyword and `=` operator makes the query more readable and syntax errors less likely.

### Compare Results

![Compare Environments](./compare-environments.gif)

Submit the same Query to different environment and compare the results.

Query Results are automatically saved on the Azure File System with a date-time stamp and source environments, which also makes for ideal `before-after` test scenarios.

### Export to Excel

![Excel Download](./excel-download.gif)

The Excel download converts the result into an Excel spreadsheet with value type preservation.  This means that values that look like numbers are not automatically converted to number -- account `40.2020` is not stripped to the number `20.202` for example, as a `.csv` conversion might do.

### Information Sharing

![Information Sharing](./sharing.gif)

Because the back-end persist the data with the Azure File System, it can be shared.  For example user A on the left side above writes and validates a Query that user B on the right side then opens up and executes.  User A on the left side can then view the results.
