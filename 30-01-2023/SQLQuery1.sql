--DDL Command (Data Definition Language)
--Database Yaratmaq Ucun Query
Create database P133FirstSqlCommand

--Yaradilan Database Secmek Ucun Query
Use P133FirstSqlCommand

--Yeni Table Yaratmaq Query
Create Table Groups
(
	Id int, 
	Name nvarchar(10), 
	StudentCount int
)

--Table-a Yeni Column elave Olunma Query-si
Alter table Groups 
Add StudentCount int

--Table-dan Column Silinme Query-si
Alter table Groups 
drop column StudentCount

--Table-da Column Uzerinde Deyisklik Olunma Querysi
alter table Groups 
alter column StudentCount int

--Table Silme Query-si
Drop Table Groups

Drop Database P133FirstSqlCommand

--DML Data Manipulation Command
--Insert Command
Insert Into Groups 
Values
(1,'P133',15)

Insert Into Groups(Name ,StudentCount, Id) 
Values
('P229',18,2)

Insert Into Groups(Name,Id) 
Values
('P228',3)

Insert Into Groups 
Values
(4,'P225',12),
(5,'P217',8),
(6,'P129',12)

Select * From Groups

Update Groups Set StudentCount = 20 where Name = 'P217'

Update Groups Set StudentCount = 0,Name = 'P200' where Name = 'P217'

Update Groups Set StudentCount = 18 where StudentCount <= 15

Update Groups Set StudentCount = 18 where StudentCount > 15 OR StudentCount < 10

Update Groups set StudentCount = 15 where Name Like 'P2%'

Update Groups set StudentCount = 10 where Name Like '%9'

Update Groups set StudentCount = 22 where Name Like '%22%'

Update Groups set StudentCount = 2 where StudentCount = 22 AND Name = 'P228'

Delete From Groups Where Id = 3

Select Id, Name, StudentCount From Groups

Select * From Groups

Select Name, StudentCount From Groups

Select Name 'Ad',StudentCount 'Telebelerin Sayi' From Groups

Select Name as 'Ad',StudentCount as 'Telebelerin Sayi' From Groups

Select Name [Ad],StudentCount [Telebelerin Sayi] From Groups

Select Name as [Ad],StudentCount as [Telebelerin Sayi] From Groups

Select * From Groups Where StudentCount <= 18 And StudentCount >= 15

Select * From Groups Where StudentCount Between 15 And 18

Select * From Groups Where StudentCount = 8 OR StudentCount = 15 OR StudentCount = 18

Select * From Groups Where StudentCount In(8,18,15)

Select * From Groups Where StudentCount != 15

Select MIN(StudentCount) From Groups

Select Max(StudentCount) From Groups

Select AVG(StudentCount) From Groups

Select UPPER(Name) From Groups

Select Lower(Name) From Groups

Select Len(Name) From Groups

Select * From Groups Where LEN(Name) = 3

Select CHARINDEX('a',Name) From Groups

Select SUBSTRING(Name,2,LEN(Name)), StudentCount From Groups

Select REPLACE(Name,'P','Code') from Groups

Create Table Students
(
	Id int,
	Name nvarchar(100),
	SurName nvarchar(100),
	Email nvarchar(100)
)

Insert Into Students
Values
(1,'Ramiz','Quliyev','ramiz@gmail.com'),
(1,'Namiq',N'Əbilov','namiq@mail.ru'),
(1,'Aqil','Soltanli','aqil@yahoo.com'),
(1,'Nurlan','Nezerov','nurlan@code.edu.az')

select SUBSTRING(Email,CHARINDEX('@',Email)+1,Len(Email)) from Students

select * from groups where StudentCount = (Select MIN(StudentCount) From Groups)

select * from groups where StudentCount > (Select AVG(StudentCount) From Groups)