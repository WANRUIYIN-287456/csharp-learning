-- PART A: Create Schema
CREATE DATABASE ExerciseDB;
GO

USE ExerciseDB;
GO

CREATE TABLE Customers (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE Products (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL,
	Price DECIMAL(10,2) NOT NULL,
	Stock INT NOT NULL DEFAULT 0
);
GO

CREATE TABLE Orders(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	CustomerId INT NOT NULL,
	ProductId INT NOT NULL,
	Quantity INT NOT NULL,
	OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
	FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
	FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
GO

-- PART B: Insert Sample Data
INSERT INTO Customers (Name) VALUES ('Acme Corp'), ('Globex Inc'), ('Initech');
INSERT INTO Products (Name, Price, Stock) VALUES ('Keyboard', 150.00, 20), ('Mouse', 450.00, 0), ('Monitor', 899.00, 5);
INSERT INTO Orders (CustomerId, ProductId, Quantity) VALUES (1, 1, 2), (2, 3, 1), (1, 2, 5);

-- PART C: SELECT QUERIES
-- 1. Select all products with Stock > 0.
SELECT * FROM Products WHERE Stock > 0;
-- 2. Select the top 2 most expensive products (use TOP, not LIMIT).
SELECT TOP 2 * FROM Products ORDER BY Price DESC;
-- 3. Join Orders with Customers and Products to show: CustomerName, ProductName, Quantity, OrderDate — for every order.
SELECT c.Name AS CustomerName, p.Name AS ProductName, o.Quantity, o.OrderDate FROM Orders o INNER JOIN Customers c ON o.CustomerId = c.Id INNER JOIN Products p ON o.ProductId = p.Id;
-- 4. Update the Mouse product's stock to 50.
UPDATE Products SET Stock = 50 WHERE Name = 'Mouse';
-- 5. Write a query using GROUP BY to show each customer's total number of orders.
SELECT c.Name AS CustomerName, COUNT(o.Id) AS TotalOrders FROM Customers c LEFT JOIN Orders o ON o.CustomerId = c.Id GROUP BY c.Name;
GO

-- PART D: Procedure
CREATE PROCEDURE GetCustomerOrders
	@CustomerId INT
AS
BEGIN
	SELECT c.Name AS CustomerName, p.Name AS ProductName, o.Quantity, p.Price * o.Quantity AS OrderValue
	FROM Orders o INNER JOIN Customers c ON c.Id = o.CustomerId INNER JOIN Products p ON p.Id = o.ProductId 
	WHERE c.Id = @CustomerId;
END
GO

EXEC GetCustomerOrders @CustomerId = 1;

-- Drop procedure if needed
DROP PROCEDURE IF EXISTS GetCustomerOrders;
GO