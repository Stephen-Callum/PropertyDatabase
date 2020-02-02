use master
go

drop database if exists PropertyDatabase
go

create database PropertyDatabase
go

use PropertyDatabase
go

create table Listings (
	ListingID INT NOT NULL PRIMARY KEY IDENTITY,
	ListingAddress VARCHAR(120) NOT NULL DEFAULT('N/A'),
	ListingPrice MONEY NULL DEFAULT('N/A'),
	NumBedrooms INT DEFAULT('N/A'),
	IsAvailable BIT NULL,
	DateListed DATE NULL
)

INSERT INTO Listings
VALUES
	('37  Henley Road', 500000.00, 8, 'false', '2019-07-20'),
	('40  Trehafod Road', 200000.00, 3, 'true', '2019-10-16'),
	('120  Leicester Road', 123456.00, 4, 'false', '2019-09-17'),
	('115  Leicester Road', 654321.00, 2, 'true', '2019-09-23'),
	('101  Boroughbridge Road', 222222, 2, 'true', '2020-12-11');

select * from Listings