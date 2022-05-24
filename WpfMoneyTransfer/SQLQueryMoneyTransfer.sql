-- подключение к базе данных
Use moneytransfer

-- создание таблицы карточек
CREATE TABLE Cards
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CardNumber BIGINT NOT NULL,
    ExpirationDate Nvarchar(10) NOT NULL,
	Balance Money NOT NULL,
	BindingPhone BIT NOT NULL,
	ClientId INT NOT NULL,
    FOREIGN KEY (ClientId) REFERENCES Clients (Id) ON DELETE CASCADE
);
-- создание таблицы клиентов
CREATE TABLE Clients
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Surname nvarchar(30) NOT NULL,
    Name nvarchar(30) NOT NULL,
	MiddleName nvarchar(30) NOT NULL,
	DateOfBirth Date NOT NULL,
	NumberOfPhone nvarchar(30) NOT NULL
);

-- вывод таблиц клиентов и карточек
Select*From Clients
Select*From Cards

-- Хранимая процедура, предназначенная для выполнения перевода денежных средств
Create procedure SP_TransCardNumber
@from bigint,
@to bigint,
@sum money
as 
BEGIN TRY
   --Начало транзакции
   BEGIN TRANSACTION
   --Инструкция 1
   UPDATE Cards SET Balance = Balance - @sum
   WHERE CardNumber = @from;
   --Инструкция 2
   UPDATE Cards SET Balance = Balance + @sum
   WHERE CardNumber = @to;
   END TRY
   BEGIN CATCH
      --В случае непредвиденной ошибки
      --Откат транзакции
      ROLLBACK TRANSACTION
      --Выводим сообщение об ошибке
      SELECT ERROR_NUMBER() AS [Номер ошибки],
             ERROR_MESSAGE() AS [Описание ошибки]
   --Прекращаем выполнение инструкции
   RETURN
   END CATCH
   --Если все хорошо. Сохраняем все изменения
   COMMIT TRANSACTION
   GO

-- Хранимая процедура, предназначенная для добавления клиента
Create procedure SP_AddClient
@surname varchar(30),
@name varchar(30),
@middleName varchar(30),
@dateOfBirth date,
@numberOfPhone varchar(30),
@Id int out
as
Begin
Insert into Clients(Surname, Name, MiddleName, DateOfBirth, NumberOfPhone)
Values (@surname,@name,@middleName,@dateOfBirth,@numberOfPhone)
Select @Id = @@Identity
End
Go

-- Хранимая процедура, предназначенная для обновления (редактирования) клиента
Create procedure SP_EditClient
@surname nvarchar(30),
@name nvarchar(30),
@middleName nvarchar(30),
@dateOfBirth date,
@numberOfPhone nvarchar(30),
@Id int
As
Begin
Update Clients
Set Surname = @surname, Name = @name, MiddleName = @middleName, DateOfBirth = @dateOfBirth,
NumberOfPhone = @numberOfPhone
Where Id = @Id
End
Go

-- Хранимая процедура, предназначенная для удаления клиента
Create procedure SP_RemoveClient
@Id int
As
Begin
Delete Clients where Id = @Id
End
Go

-- Хранимая процедура, предназначенная для добавления карты
Create procedure SP_AddCard
@cardNumber bigint,
@expirationDate nvarchar(10),
@balance money,
@bindingPhone bit,
@clientId int
As
Begin
Insert into Cards(CardNumber, ExpirationDate, Balance, BindingPhone, ClientId)
Values (@cardNumber,@expirationDate,@balance,@bindingPhone,@clientId)
End
Go

-- Хранимая процедура, предназначенная для обновления (редактирования) карты
Create procedure SP_EditCard
@cardNumber bigint,
@expirationDate nvarchar(10),
@balance money,
@bindingPhone bit,
@clientId int,
@Id int
As
Begin
Update Cards
Set CardNumber = @cardNumber, ExpirationDate = @expirationDate, Balance = @balance, 
BindingPhone = @bindingPhone, ClientId = @clientId
Where Id = @Id
End
Go

-- Хранимая процедура, предназначенная для удаления карточки
Create procedure SP_RemoveCard
@clientId int
As
Begin
Delete Cards
Where Id = @clientId
End
Go

-- Хранимая процедура, предназначенная для поиска и вывода номера карты по номеру телефона
Create procedure SP_GetCardNumder1
@number nvarchar(30),
@isCard bit,
@cardNumber bigint out,
@clientFio nvarchar (30) out
as
Begin
if @isCard = 0
Select @cardNumber = CardNumber, @clientFio = Surname +' '+ Substring(Name,1,1) +' '+ Substring(MiddleName,1,1)
from Cards as c INNER JOIN Clients as cl on (cl.Id = c.ClientId)
Where BindingPhone = 1 And NumberOfPhone = @number
else
Select @cardNumber = CardNumber, @clientFio = Surname +' '+ Substring(Name,1,1) +' '+ Substring(MiddleName,1,1) 
from Cards as c INNER JOIN Clients as cl on (cl.Id = c.ClientId)
Where CardNumber = Replace(@number,' ','')
End
Go

-- Хранимая процедура, предназначенная для вывода карточных данных указанного клиента
Create procedure SP_FindId
@clientId int
As
Begin
Select c.Id, CardNumber, ExpirationDate, Balance, BindingPhone, c.ClientId from Clients as cl INNER JOIN Cards as c on (cl.Id = c.ClientId)
Where cl.Id = @clientId
End
Go