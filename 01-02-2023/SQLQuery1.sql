Create database P133GroupByOrderByViewProcedureFunctionTrigger

Use P133GroupByOrderByViewProcedureFunctionTrigger

create table Brands
(
Id int identity primary key,
Name nvarchar(100) NOT NULL 
)

create table NoteBooks
(
Id int identity primary key,
Name nvarchar(100) NOT NULL ,
Price decimal(18,2),
BrandId int foreign key references Brands(Id)
)

create table Phones
(
Id int identity primary key,
Name nvarchar(100) NOT NULL ,
Price decimal(18,2),
BrandId int foreign key references Brands(Id)
)

insert into Brands 
Values
('Samsung'),
('Acer'),
('MSI'),
('IPhone')

insert into NoteBooks 
Values
('Iphone 14 Pro Max',2849.99,4),
('Acer Aspire A315',1679.99,2),
('MSI Katana GF66',4099.99,3),
('MSI 14 A10M-1029US',2279.99,3),
('Acer Extensa Ex215',999.99,2)

insert into Phones 
Values
('iPhone 14 Plus',2599.99,4),
('iPhone SE 3 (2022)',1249.99,4),
('iPhone 11 ',1199.99,4),
('Samsung Galaxy A04e',249.99,1),
('Samsung Galaxy Z Flip4 ',2249.99,1)

--Group By And Having
Select Brands.Name, COUNT(*) From Brands
Join NoteBooks
On Brands.Id = NoteBooks.BrandId
Group By Brands.Name
Having COUNT(*) > 1

--Order By ASC (Default) / DESC
Select * From Brands
Join NoteBooks
On Brands.Id = NoteBooks.BrandId
Order By NoteBooks.Id Desc

--Notebooks Tabelenden Data Cixardacaqsiniz 
--Amma Nece Olacaq Brandin Adi (BrandName kimi), 
--Hemin Brandda Olan Telefonlarin Pricenin Cemi 
--(TotalPrice Kimi) , Hemin Branda Nece dene Telefon 
--Varsa Sayini (ProductCount Kimi) Olacaq ve 
--Sayi 3-ve 3-den Cox Olan Datalari Cixardan Query.
--Misal
--BrandName:        TotalPrice:        ProductCount:
--Apple                    6750                3
--Samsung              3500                4

--View
--View-nun Yaradilmasi
Create View usv_GetBrandStatistic
as
Select 
b.Name as [BrandName],
SUM(n.Price) as [TotalPrice],
COUNT(*) as [ProductCount]
From NoteBooks n
Join Brands b
On n.BrandId = b.Id
Group By b.Name

--View-nin Editlenmesi
Alter View usv_GetBrandStatistic
as
Select 
b.Name as [BrandName],
SUM(n.Price) as [TotalPrice],
COUNT(*) as [ProductCount]
From NoteBooks n
Join Brands b
On n.BrandId = b.Id
Group By b.Name

--View-nun Isdifade Olunmasi
Select * From usv_GetBrandStatistic 

Select * From usv_GetBrandStatistic Where [Product Count] > 1

Select [Brand Name] From usv_GetBrandStatistic Where [Product Count] > 1

Create View usv_GetAllNotebooksWithBrand
As
Select n.Id as NotebookId, n.Name as NotebookName,b.Id as BrandId , b.Name as BrandName, n.Price From NoteBooks n
Join Brands b
On n.BrandId = b.Id

Select * From usv_GetAllNotebooksWithBrand

--Procedure
--Procedurun Yaradilmasi
Create Procedure usp_GetNotebooksByPrice 
@price decimal(18,2)
As
Begin
	Select * From NoteBooks n
	Join Brands b
	On n.BrandId = b.Id
	Where n.Price > @price
End

--Procedurun Editlenmesi
Alter Procedure usp_GetNotebooksByPrice 
@price decimal(18,2)
As
Begin
	Select * From NoteBooks n
	Join Brands b
	On n.BrandId = b.Id
	Where n.Price > @price
End

--Procedurda View nun Isledilmesi
Alter Procedure usp_GetNotebooksByPrice 
@price decimal(18,2)
As
Begin
	Select * From usv_GetAllNotebooksWithBrand
	Where Price > @price
End

--Procedurun Isdifade Olunmasi
exec usp_GetNotebooksByPrice 2000

exec sp_rename 'Phones','Telefonlar'

exec sp_rename 'Telefonlar.Price','Qiymet'

--Custom Function
--Function nun yaradilmasi
Create Function usf_GetNotebooksCountByPrice
(@price  decimal(18,2))
returns int
As
Begin
	declare @count int

	Select @count = COUNT(*) From NoteBooks n
	Join Brands b
	On n.BrandId = b.Id
	where n.Price > @price

	return @count
End

--Functionnun Editlenmesi
Alter Function usf_GetNotebooksCountByPrice
(
	@price  decimal(18,2), 
	@brandId int
)
returns int
As
Begin
	declare @count int

	Select @count = COUNT(*) From usv_GetAllNotebooksWithBrand
	where Price > @price And BrandId = @brandId

	return @count
End

--Functionun Isdifade Olunmasi
Select dbo.usf_GetNotebooksCountByPrice(2000,3)

--Trigerr

Create Table ArchiveNotebooks
(
	Id int,
	Name nvarchar(100),
	Price decimal(18,2),
	BrandId int,
	Date DateTime2,
	StatementType nvarchar(100)
)

Create Trigger NoebooksChanged
on Notebooks
after insert
as
Begin
	declare @id int
	declare @name nvarchar(100)
	declare @price decimal(18,2)
	declare @brandId int
	declare @date DateTime2
	declare @statementType nvarchar(100)

	Select @id = n.Id From inserted n
	Select @name = n.Name From inserted n
	Select @price = n.Price From inserted n
	Select @brandId = n.BrandId From inserted n
	Select @date = GETUTCDATE() From inserted n
	Select @statementType = 'Inserted' From inserted n

	Insert Into ArchiveNotebooks(Id, Name, Price, BrandId, Date, StatementType)
	Values
	(@id,@name,@price,@brandId,@date,@statementType)
End

Alter Trigger NoebooksChanged
on Notebooks
after insert,delete
as
Begin
	declare @id int
	declare @name nvarchar(100)
	declare @price decimal(18,2)
	declare @brandId int
	declare @date DateTime2
	declare @statementType nvarchar(100)

	Select @id = n.Id From inserted n
	Select @name = n.Name From inserted n
	Select @price = n.Price From inserted n
	Select @brandId = n.BrandId From inserted n
	Select @date = GETUTCDATE() From inserted n
	Select @statementType = 'Inserted' From inserted n

	Select @id = n.Id From deleted n
	Select @name = n.Name From deleted n
	Select @price = n.Price From deleted n
	Select @brandId = n.BrandId From deleted n
	Select @date = GETUTCDATE() From deleted n
	Select @statementType = 'Deleted' From deleted n

	Insert Into ArchiveNotebooks(Id, Name, Price, BrandId, Date, StatementType)
	Values
	(@id,@name,@price,@brandId,@date,@statementType)
End

Create Trigger NoebooksUpdated
on Notebooks
after update
as
Begin
	declare @id int
	declare @name nvarchar(100)
	declare @price decimal(18,2)
	declare @brandId int
	declare @date DateTime2
	declare @statementType nvarchar(100)

	Select @id = n.Id From inserted n
	Select @name = n.Name From inserted n
	Select @price = n.Price From inserted n
	Select @brandId = n.BrandId From inserted n
	Select @date = GETUTCDATE() From inserted n
	Select @statementType = 'Updated' From inserted n

	Insert Into ArchiveNotebooks(Id, Name, Price, BrandId, Date, StatementType)
	Values
	(@id,@name,@price,@brandId,@date,@statementType)
End

Insert Into NoteBooks
Values
('MacBook Pro', 2500,1)