-- connecting to the database
Use moneytransfer

-- creating a cards table
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
-- creating a clients table
CREATE TABLE Clients
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Surname nvarchar(30) NOT NULL,
    Name nvarchar(30) NOT NULL,
	MiddleName nvarchar(30) NOT NULL,
	DateOfBirth Date NOT NULL,
	NumberOfPhone nvarchar(30) NOT NULL
);

-- output of customer tables and cards
Select*From Clients
Select*From Cards

-- A stored procedure designed to perform a money transfer
Create procedure SP_TransCardNumber
@from bigint,
@to bigint,
@sum money
as 
BEGIN TRY
   --Start of the transaction
   BEGIN TRANSACTION
   --Instruction 1
   UPDATE Cards SET Balance = Balance - @sum
   WHERE CardNumber = @from;
   --Instruction 2
   UPDATE Cards SET Balance = Balance + @sum
   WHERE CardNumber = @to;
   END TRY
   BEGIN CATCH
      --In case of an unexpected error
      --Rolling back a transaction
      ROLLBACK TRANSACTION
      --We output an error message
      SELECT ERROR_NUMBER() AS [Номер ошибки],
             ERROR_MESSAGE() AS [Описание ошибки]
   --We stop executing the instructions
   RETURN
   END CATCH
   --If everything is fine, save all the changes
   COMMIT TRANSACTION
   GO

-- A stored procedure designed to add a client
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

-- A stored procedure designed to update (edit) the client
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

-- A stored procedure designed to delete a client
Create procedure SP_RemoveClient
@Id int
As
Begin
Delete Clients where Id = @Id
End
Go

-- A stored procedure designed to add a card
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

-- A stored procedure designed to update (edit) the card
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

-- A stored procedure designed to delete a card
Create procedure SP_RemoveCard
@clientId int
As
Begin
Delete Cards
Where Id = @clientId
End
Go

-- A stored procedure designed to search for and output a card number by phone number
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

-- A stored procedure designed to output the card data of the specified client
Create procedure SP_FindId
@clientId int
As
Begin
Select c.Id, CardNumber, ExpirationDate, Balance, BindingPhone, c.ClientId from Clients as cl INNER JOIN Cards as c on (cl.Id = c.ClientId)
Where cl.Id = @clientId
End
Go