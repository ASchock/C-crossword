# X-Word
Windows application to generate a cross word game.
Written in C#

## Setting up the app
The following instructions are to be followed to set the application up.

### Setting up the database
1. Run [schema.sql](/xword/sql/schema.sql) to create the database and the tables for the crossword.
2. To create sample words, run the [sample.sql](/xword/sql/sample.sql)

### Running the app
1. Open up the application in Visual Studio 
2. 
    - Edit the [App.config](/xword/App.config) and replace the value on line 14 with valid database credentials and values.
        ```
        <value>"Data Source=ServerName;Initial Catalog=DatabaseName;User ID=UserName;Password=Password"</value>
        ```
    - Alternatively, go to Project > xword Properties > Settings and edit the `connection_string` to the appropriate db connection string. 

3. Save the [App.config](/xword/App.config) / settings.