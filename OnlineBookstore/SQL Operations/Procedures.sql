USE OnlineBookstore;
GO

CREATE OR ALTER PROCEDURE GetGenreSalesReport
    @StartDate DATETIMEOFFSET,
    @EndDate DATETIMEOFFSET
AS
BEGIN
    SELECT
        g.GenreName,
        SUM(ol.Quantity * ol.UnitPrice) AS TotalSales,
        COUNT(DISTINCT o.UserID) AS UniqueCustomers,
        AVG(ol.Quantity * ol.UnitPrice) AS AverageOrderValue,
        RANK() OVER (
            ORDER BY SUM(ol.Quantity * ol.UnitPrice) DESC
        ) AS GenreSalesRank
    FROM [Order] o
    INNER JOIN OrderLine ol ON o.OrderID = ol.OrderID
    INNER JOIN Book b ON ol.BookID = b.BookID
    INNER JOIN Genre g ON b.GenreID = g.GenreID
    WHERE o.OrderDate BETWEEN @StartDate AND @EndDate
    GROUP BY g.GenreName;
END;
GO

CREATE OR ALTER PROCEDURE GetAuthorPerformanceReport
    @MinimumRatings INT
AS
BEGIN
    SELECT
        a.FirstName + ' ' + a.LastName AS AuthorName,
        AVG(CAST(r.Score AS DECIMAL(3,2))) AS AverageBookRating,
        COUNT(r.RatingID) AS TotalRatingsReceived,
        ISNULL(SUM(ol.Quantity), 0) AS TotalBooksSold
    FROM Author a
    INNER JOIN Book b ON a.AuthorID = b.AuthorID
    LEFT JOIN Rating r ON b.BookID = r.BookID
    LEFT JOIN OrderLine ol ON b.BookID = ol.BookID
    GROUP BY a.FirstName, a.LastName
    HAVING COUNT(r.RatingID) >= @MinimumRatings
    ORDER BY AverageBookRating DESC;
END;
GO

CREATE OR ALTER PROCEDURE GetMonthlyCustomerBehavior
    @StartDate DATETIMEOFFSET,
    @EndDate DATETIMEOFFSET
AS
BEGIN
    WITH FirstOrders AS
    (
        SELECT
            UserID,
            MIN(OrderDate) AS FirstOrderDate
        FROM [Order]
        GROUP BY UserID
    )
    SELECT
        YEAR(o.OrderDate) AS OrderYear,
        MONTH(o.OrderDate) AS OrderMonth,
        COUNT(DISTINCT CASE
            WHEN YEAR(f.FirstOrderDate) = YEAR(o.OrderDate)
             AND MONTH(f.FirstOrderDate) = MONTH(o.OrderDate)
            THEN o.UserID
        END) AS NewCustomers,
        COUNT(DISTINCT CASE
            WHEN f.FirstOrderDate < DATEFROMPARTS(YEAR(o.OrderDate), MONTH(o.OrderDate), 1)
            THEN o.UserID
        END) AS RepeatCustomers,
        COUNT(DISTINCT o.UserID) AS TotalActiveCustomers
    FROM [Order] o
    INNER JOIN FirstOrders f ON o.UserID = f.UserID
    WHERE o.OrderDate BETWEEN @StartDate AND @EndDate
    GROUP BY YEAR(o.OrderDate), MONTH(o.OrderDate)
    ORDER BY OrderYear, OrderMonth;
END;
GO

CREATE OR ALTER PROCEDURE GetTopCustomers
    @TopCount INT
AS
BEGIN
    WITH CustomerTotals AS
    (
        SELECT
            u.UserID,
            u.FirstName + ' ' + u.LastName AS CustomerFullName,
            u.Email AS CustomerEmail,
            SUM(ol.Quantity * ol.UnitPrice) AS LifetimeSpend,
            SUM(ol.Quantity) AS TotalBooksPurchased
        FROM [User] u
        INNER JOIN [Order] o ON u.UserID = o.UserID
        INNER JOIN OrderLine ol ON o.OrderID = ol.OrderID
        GROUP BY u.UserID, u.FirstName, u.LastName, u.Email
    ),
    FavoriteGenres AS
    (
        SELECT
            o.UserID,
            g.GenreName,
            ROW_NUMBER() OVER (
                PARTITION BY o.UserID
                ORDER BY SUM(ol.Quantity) DESC
            ) AS rn
        FROM [Order] o
        INNER JOIN OrderLine ol ON o.OrderID = ol.OrderID
        INNER JOIN Book b ON ol.BookID = b.BookID
        INNER JOIN Genre g ON b.GenreID = g.GenreID
        GROUP BY o.UserID, g.GenreName
    )
    SELECT TOP (@TopCount)
        ct.CustomerFullName,
        ct.CustomerEmail,
        ct.LifetimeSpend,
        ct.TotalBooksPurchased,
        fg.GenreName AS FavoriteGenre
    FROM CustomerTotals ct
    LEFT JOIN FavoriteGenres fg
        ON ct.UserID = fg.UserID
       AND fg.rn = 1
    ORDER BY ct.LifetimeSpend DESC;
END;
GO

EXEC GetGenreSalesReport
    @StartDate = '2026-01-01',
    @EndDate = '2026-12-31';

EXEC GetAuthorPerformanceReport @MinimumRatings = 3;

EXEC GetMonthlyCustomerBehavior
    @StartDate = '2026-01-01',
    @EndDate = '2026-12-31';

EXEC GetTopCustomers @TopCount = 10;