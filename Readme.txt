OnlineBookstore/Tables contains the .sql file that creates the needed database

OnlineBookstore/Data contains the .csv files containing our table data and Data.sql, 
which contains all the bulk insert statements to insert that data into the database,
you may need to alter the file paths in Data.sql to fill the tables properly
BookUpdateImagePath.sql when ran should add images for each book inside the shop

OnlineBookstore/SQL_Operations contains Procedures.sql, which contains all of the stored procedure 
aggregate queries used to generate the reports for ReportsModel. It also contains SQL_Statements.txt which 
is an explanation of the pattern used in code to query and insert into the database along with where each 
query is located

Onlinebookstore/Pages contains all the .cshtml files for the RazorPages Application

OnlineBookStore/Diagrams contains the Database structure diagram and the UML diagrams for the applicaton

All .cs object files are stored directly in OnlineBookstore
