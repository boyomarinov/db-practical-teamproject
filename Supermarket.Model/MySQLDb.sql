CREATE DATABASE Supermarket;

USE Supermarket;

CREATE TABLE Vendors(
	VendorId int NOT NULL,
	VendorName nvarchar(50) NOT NULL,
	CONSTRAINT PK_Vendors PRIMARY KEY(VendorId)
);

CREATE TABLE Measures(
	MeasureId int,
	MeasureName nvarchar(50),
	CONSTRAINT PK_Measures PRIMARY KEY(MeasureId)
);

CREATE TABLE Products(
	ProductId int AUTO_INCREMENT,
	VendorId int NOT NULL,
	ProductName nvarchar(50) NOT NULL,
	MeasureId int NOT NULL,
	BasePrice decimal(10, 2) NOT NULL,
	CONSTRAINT PK_Products PRIMARY KEY(ProductId),
	CONSTRAINT FK_Products_Vendors FOREIGN KEY(VendorId) REFERENCES Vendors(VendorId),
	CONSTRAINT FK_Products_Measures FOREIGN KEY(MeasureId) REFERENCES Measures(MeasureId)
);

INSERT INTO Measures(MeasureId, MeasureName)
VALUES (100, 'liters'),
		(200, 'pieces'),
		(300,'kilograms'),
		(400,'milliliters'),
		(500,'cups');

INSERT INTO Vendors(VendorId, VendorName)
VALUES (10, 'Nestle Sofia Corp.'),
		(20, 'Zagorka Corp.'),
		(30, 'Targovishte Bottling Company Ltd.'),
		(40,'Coca Cola'),
		(50,'Pepsi'),
		(60,'Tuborg'),
		(70,'Kamentza'),
		(80,'Ledenika'),
		(90,'Garlsbers'),
		(100,'Hainiken'),
		(110,'Svoge'),
		(120,'Staropramen'),
		(130,'Lindor'),
		(140,'Rodopea'),
		(150,'Verea'),
		(160,'Madjarov');

INSERT INTO Products(VendorId, ProductName, MeasureId, BasePrice)
VALUES 
		(20, 'Beer "Zagorka"', 100, 0.86),
		(30, 'Vodka "Targovishte"', 100, 7.56),
		(70, 'Beer "Kamenitza"', 100, 0.75),
		(80, 'Beer "Ledenika"', 100, 0.65),
		(90, 'Beer "Pirinsko"', 100, 0.7),
		(40, 'Beer "Coca Cola"', 100, 0.5),
		(40, 'Beer "Fanta"', 100, 0.5),
		(40, 'Beer "Sprite"', 100, 0.5),
		(40, 'Beer "Tonik"', 100, 0.5),
		(50, 'Beer "Pepsi"', 100, 0.45),
		(50, 'Beer "Fanta"', 100, 0.45),
		(50, 'Beer "7UP"', 100, 0.45),
		(50, 'Beer "Mirinda"', 100, 0.45),
		(110,'Svoge Shokolad',200,1.1),	
		(130,'Londor Shokolad',200,2.8),
		(140,'Yogurt',100,0.9),
		(150,'Yogurt',100,0.95),
		(160,'Cheese',300,8.5),
		(140,'Cheese',300,7.8),
		(150,'Cheese',300,8.2),	
		(160,'Yellow Cheese',300,9.5),
		(140,'Yellow Cheese',300,8.5),
		(150,'Yellow Cheese',300,8.8),
		(160,'Milk',100,1.5),
		(140,'Milk',100,1.9),
		(150,'Milk',100,1.7);
		
		
			
		

