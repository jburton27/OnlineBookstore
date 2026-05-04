USE OnlineBookstore;
GO

DROP TABLE IF EXISTS AuthorImport;
GO
DROP TABLE IF EXISTS GenreImport;
GO
DROP TABLE IF EXISTS BookImport;
GO
DROP TABLE IF EXISTS UserImport;
GO
DROP TABLE IF EXISTS OrderImport;
GO
DROP TABLE IF EXISTS OrderLineImport;
GO
DROP TABLE IF EXISTS RatingImport;
GO

CREATE TABLE AuthorImport
(
    FirstName VARCHAR(50),
    LastName VARCHAR(50)
);
GO

CREATE TABLE GenreImport
(
    GenreName VARCHAR(50) NOT NULL,

    UNIQUE(GenreName)
);
GO

CREATE TABLE BookImport
(
    ISBN VARCHAR(20) NOT NULL,
    AuthorID INT NOT NULL,
    StoreID INT NOT NULL,
    GenreID INT NOT NULL,
    Title VARCHAR(150) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    PublicationYear INT NULL,
    [Condition] VARCHAR(30) NULL,
    CoverType VARCHAR(30) NULL,

    UNIQUE(ISBN),

    FOREIGN KEY(AuthorID)
        REFERENCES Author(AuthorID),

    FOREIGN KEY(StoreID)
        REFERENCES BookStore(StoreID),

    FOREIGN KEY(GenreID)
        REFERENCES Genre(GenreID),

    CHECK(Price >= 0)
);
GO

BULK INSERT AuthorImport
FROM 'C:\CIS 560\OnlineBookstore\OnlineBookstore\Data\Author.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\n',
    TABLOCK
);
GO

INSERT INTO Author (FirstName, LastName)
SELECT FirstName, LastName
FROM AuthorImport;
GO

BULK INSERT GenreImport
FROM 'C:\CIS 560\OnlineBookstore\OnlineBookstore\Data\Genre.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\n',
    TABLOCK
);
GO

INSERT INTO Genre (GenreName)
SELECT GenreName
FROM GenreImport;
GO

INSERT INTO BookStore ([Name], [Address], City, [State])
VALUES ('Amazon Books', '410 Terry Ave N', 'Seattle', 'WA');

BULK INSERT BookImport
FROM 'C:\CIS 560\OnlineBookstore\OnlineBookstore\Data\Book.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\n',
    TABLOCK
);
GO

INSERT INTO Book
(
    ISBN,
    AuthorID,
    StoreID,
    GenreID,
    Title,
    Price,
    PublicationYear,
    [Condition],
    CoverType
)
SELECT
    ISBN,
    AuthorID,
    StoreID,
    GenreID,
    Title,
    Price,
    PublicationYear,
    [Condition],
    CoverType
FROM BookImport;
GO

CREATE TABLE UserImport
(
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    PhoneNumber VARCHAR(50) NULL,
    [Address] VARCHAR(150) NULL,

    UNIQUE(Email)
);
GO

BULK INSERT UserImport
FROM 'C:\CIS 560\OnlineBookstore\OnlineBookstore\Data\User.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\n',
    TABLOCK
);
GO

INSERT INTO [User]
(
    FirstName,
    LastName,
    Email,
    PasswordHash,
    PhoneNumber,
    [Address]
)
SELECT
    FirstName,
    LastName,
    Email,
    PasswordHash,
    PhoneNumber,
    [Address]
FROM UserImport;
GO

CREATE TABLE OrderImport
(
    UserID INT NOT NULL,

    FOREIGN KEY(UserID)
        REFERENCES [User](UserID)
);
GO

BULK INSERT OrderImport
FROM 'C:\CIS 560\OnlineBookstore\OnlineBookstore\Data\Order.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\n',
    TABLOCK
);
GO

INSERT INTO [Order]
(
    UserID
)
SELECT
    UserID
FROM OrderImport;
GO

CREATE TABLE OrderLineImport
(
    OrderID INT NOT NULL,
    BookID INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,

    FOREIGN KEY(OrderID)
        REFERENCES [Order](OrderID),
    FOREIGN KEY(BookID)
        REFERENCES Book(BookID),

    CHECK(Quantity > 0),
    CHECK(UnitPrice >= 0)
);
GO

BULK INSERT OrderLineImport
FROM 'C:\CIS 560\OnlineBookstore\OnlineBookstore\Data\OrderLine.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\n',
    TABLOCK
);
GO

INSERT INTO OrderLine
(
    OrderID,
    BookID,
    Quantity,
    UnitPrice
)
SELECT
    OrderID,
    BookID,
    Quantity,
    UnitPrice
FROM OrderLineImport;
GO

CREATE TABLE RatingImport
(
    UserID INT NOT NULL,
    BookID INT NOT NULL,
    Score INT NOT NULL,

    FOREIGN KEY(UserID)
        REFERENCES [User](UserID),
    
    FOREIGN KEY(BookID)
        REFERENCES Book(BookID),

    UNIQUE(UserID, BookID),

    CHECK(Score BETWEEN 1 AND 5)
);
GO

BULK INSERT RatingImport
FROM 'C:\CIS 560\OnlineBookstore\OnlineBookstore\Data\Rating.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\n',
    TABLOCK
);
GO

INSERT INTO Rating
(
    UserID,
    BookID,
    Score
)
SELECT
    UserID,
    BookID,
    Score
FROM RatingImport;
GO