--Kitabxana database-i qurursunuz
Create Database LibraryDb

Use LibraryDb

--Books (Id, Name, PageCount)
--Books-un Name columu minimum 2 simvol maksimum 100 simvol deyer ala bileceyi serti olsun.
--Books-un PageCount columu minimum 10 deyerini ala bileceyi serti olsun.
Create Table Books
(
	Id int identity primary key,
	Name nvarchar(100) Check(Len(Name) >= 2),
	PageCount int Check(PageCount >= 10)
)

insert into Books(Name,PageCount,AuthorId)
values
('The Great Gatsby',218,1),
('To Kill a Mockingbird',281,1),
('The Lord of the Rings',1178,2),
('One Hundred Years of Solitude',417,3),
('The Dangerous Book for Boys ',249,3),
('The Mammoth Book of Short Crime Stories',530,4)

--Authors (Id, Name, Surname)
Create Table Authors
(
	Id int identity primary key,
	Name nvarchar(100),
	SurName nvarchar(100)
)

insert into Authors
values
('Agatha','Christie'),
('Arthur','Conan Doyle'),
('Conn','Iggulden'),
('Scott','Fitzgerald'),
('John Ronald','Reuel Tolkien'),
('Hal','Iggulden'),
('Gabriel','Marquez'),
('Harper','Lee')

--Books ve Authors table-larinizin mentiqi uygun elaqesi olsun.

Alter Table Books
Add AuthorId int Foreign Key References Authors(Id)

--Id, Name, PageCount ve AuthorFullName columnlarinin valuelarini qaytaran bir view yaradin
Create View usv_GetAllBooksWithAuthorFullName
As
Select b.Id,b.Name,b.PageCount, CONCAT(a.Name,' ',a.SurName) 'AuthorFullName' From Books b
Join Authors a
On b.AuthorId = a.Id

Select * From usv_GetAllBooksWithAuthorFullName

--Gonderilmis axtaris deyirene gore hemin axtaris deyeri name ve ya authorFullNamelerinde 
--olan Book-lari Id, Name, PageCount, AuthorFullName columnlari seklinde gostern procedure yazin
Create Procedure usp_GetBooksWithAuthorFullNameBySearch
@search nvarchar(100)
as
Begin
	Select * From usv_GetAllBooksWithAuthorFullName 
	where 
	Name like '%'+@search+'%' OR 
	AuthorFullName Like '%'+@search+'%'
End

exec usp_GetBooksWithAuthorFullNameBySearch 'gatha'

--Authors tableinin insert, update ve deleti ucun (her biri ucun ayrica) procedure yaradin
Create Procedure usp_CreateAuthor
@name nvarchar(100),
@surName nvarchar(100)
as
Begin
	insert into Authors(Name,SurName)
	Values
	(@name,@surName)
End

exec usp_CreateAuthor 'Nizami','Gencevi'

Create Procedure usp_UpdateAuthor
@id int,
@name nvarchar(100),
@surName nvarchar(100)
As
Begin
	Update Authors set Name = @name, SurName = @surName where Id = @id
End

exec usp_UpdateAuthor 9,'Fuzuli','Genceli'

Create Procedure usp_DeleteAuthor
@id int
As
Begin
	Delete From Authors where Id = @id
End

exec usp_DeleteAuthor 7

--Authors-lari Id,FullName,BooksCount,MaxPageCount seklinde qaytaran view yaradirsiniz 
--Id-author id-si, FullName - Name ve Surname birlesmesi, BooksCount - Hemin authorun elaqeli 
--oldugu kitablarin sayi, MaxPageCount - hemin authorun elaqeli oldugu kitablarin icerisindeki 
--max pagecount deyeri
Create view usv_AuthorsInfo
As
Select a.Id,(a.Name+' '+a.SurName) 'FullName',Count(*) 'BooksCount',Max(PageCount) 'MaxPageCount' From Authors a
Join Books b
On a.Id = b.AuthorId
Group By a.Id,a.Name,a.SurName

Select * From usv_AuthorsInfo