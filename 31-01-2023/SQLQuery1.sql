Create Database P133Constraint

Use P133Constraint

drop table students

Create Table Students
(
	Id int identity Constraint PK_Id Primary Key,
	Name nvarchar(100) Constraint NN_Studentds_Name Not Null Constraint CK_Students_Name Check(Len(Name) >= 3),
	SurName nvarchar(100)
)

Alter Table Students
Add Constraint CK_Students_SurName Check(Len(SurName)>=5)

Alter Table Students
Add Constraint CK_Students_Name Check(Len(Name) >= 3)

Alter Table Students
Drop Constraint CK_Students_Name

Insert Into Students Values
('NN','Nezerov'),
('Semed','Aliyev')

Create Table Students
(
	Id int identity Primary Key,
	Name nvarchar(100) Not NULL Check(Len(Name) >= 3),
	SurName nvarchar(100) Not NULL Check(Len(SurName) >=5),
	Age TinyInt Not Null,
	Score decimal(5,2) Check(Score <= 100),
	Email nvarchar(100) Not NUll Unique
)


Create Table Groups
(
	Id int identity Primary Key,
	Name nvarchar(10) Not Null Check(Len(Name) >= 4) Unique,
	StudentCount int Not Null Default(12) Check(StudentCount <= 18 AND StudentCount >= 12)
)

Alter Table Students
Add GroupId int Not Null Foreign Key References Groups(Id)

Insert Into Groups
Values
('P133',15),
('P229',18)

Insert Into Students
Values
('Nurlan','Nezerov',29,72,'nurlan@gmail.com',1),
('Namiq','Abilov',19,86,'namiq@gmail.com',1),
('Orxan','Memmedli',26,85,'orxan@gmail.com',1),
('Cavid','Aliyev',25,65,'cavid@gmail.com',1)

Select Students.Name,SurName,age,score,email,Groups.Name,StudentCount From Students
Inner Join Groups
on Students.GroupId = Groups.Id

Select Students.Name,SurName,age,score,email,Groups.Name,StudentCount From Students
Join Groups
on Students.GroupId = Groups.Id

Select * From Groups
Join Students
On Groups.Id = Students.GroupId

Select * From Students Right Outer Join Groups On Students.GroupId = Groups.Id
Select * From Students Right Join Groups On Students.GroupId = Groups.Id

Select * From Groups Left Outer Join Students On Students.GroupId = Groups.Id
Select * From Students Left Join Groups On Students.GroupId = Groups.Id

Select * From Students full Join Groups On Students.GroupId = Groups.Id
Select * From Students full outer Join Groups On Students.GroupId = Groups.Id

Create Table Cerificates
(
	Id int identity Primary Key,
	Name nvarchar(100) Not Null Unique,
	Min decimal(5,2),
	Max decimal(5,2)
)

Insert Into Cerificates
Values
('Standart', 65, 84),
('Honour', 85, 94),
('High Honour', 95, 100)

--Non equal join
Select s.Name, s.SurName,S.score,c.Name From Students s
Join Cerificates c
On s.Score Between c.Min And c.Max

Create Table Positions
(
	Id int identity primary key,
	Name nvarchar(100) Not Null Unique,
	ParentId int Foreign Key References Positions(Id)
)

Insert Into Positions(Name,ParentId)
Values
('CEO'),
('Direktor',1),
('Direktor Muavini',2),
('Meneger',3)


--Self Join
Select p.Id,P.Name,pp.Name Parent From Positions p
Left Join Positions pp
On p.ParentId = pp.Id

Create Table Products
(
	Id Int Identity Primary Key,
	Name nvarchar(100) Not Null Unique
)

Create Table Colours
(
	Id Int Identity Primary Key,
	Name nvarchar(100) Not Null Unique
)

Insert Into Colours
Values
('Red'),
('Green'),
('Blue'),
('Yellow')



Create Table Sizes
(
	Id Int Identity Primary Key,
	Name nvarchar(100) Not Null Unique
)

Select * From Products
cross join Sizes

drop table ProductSizes

Create Table ProductSizes
(
	Id int Identity Primary Key,
	ProductId int Foreign Key References Products(Id),
	SizeId int Foreign Key References Sizes(Id),
	ColorId int Foreign Key References Colours(Id),
	Count int,
	Price money
)

Select 
ISNULL(p.Name,'-') as [Product Name],
ISNULL(ps.Count,'0') as [Sayi],
ps.Price as [Qiymeti],
c.Name as [Rengi],
s.Name as [Olcusu]
From Products p
Full Join ProductSizes ps
on ps.ProductId = p.Id
Full Join Colours c
On c.Id = ps.ColorId
Full Join Sizes s
On s.Id = ps.SizeId

Select COUNT(distinct Name) From Students

Select Name From Sizes 
Union All
Select Name From Colours

Create Table Employees
(
	Id int identity primary key,
	Ad nvarchar(100),
	SoyAd nvarchar(100)
)

Insert Into Employees
Values
('Nurlan','Nezerov'),
('Namiq','Abilov'),
('Hamid','Mammadov'),
('Huseyn','Asurlu')

Select COUNT( Name) From 
(
Select Name From 
(Select s.Name ,s.SurName From Students s
Union all
Select e.Ad,e.SoyAd from Employees e) sq1
Union all
Select Name from(Select Name From Sizes 
Union All
Select Name From Colours) sq2
) allsq

Select COUNT(distinct age) From Students