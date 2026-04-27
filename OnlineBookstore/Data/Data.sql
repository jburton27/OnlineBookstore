USE OnlineBookstore;
GO

DROP TABLE IF EXISTS AuthorImport;
GO
DROP TABLE IF EXISTS GenreImport;
GO
DROP TABLE IF EXISTS BookImport;
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
FROM 'C:\CIS 560\Author.csv'
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
FROM 'C:\CIS 560\Genre.csv'
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
FROM 'C:\CIS 560\Book.csv'
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