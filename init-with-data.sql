use master
go;

create database CompanyDelegations on
    (
        NAME = CompanyDelegationsData,
        FILENAME = '/home/mbilicz/DataGripProjects/ProjektBD/data/company_delegations_data.mdf',
        SIZE = 500,
        MAXSIZE = 2048,
        FILEGROWTH = 5
    ) log on (
        NAME = CompanyDelegationsLog,
        FILENAME = '/home/mbilicz/DataGripProjects/ProjektBD/data/company_delegations_log.ldf',
        SIZE = 5 MB,
        MAXSIZE = 50 MB,
        FILEGROWTH = 5 MB
    );
go;

use CompanyDelegations
go;

create table Employees
(
    employeeId int identity primary key,
    firstName nvarchar(20) not null,
    lastName nvarchar(20) not null,
    position nvarchar(20) not null,
    isActive bit not null
) go;

create table ExpenseTypes
(
    expenseTypeId int identity primary key,
    name nvarchar(100) not null,
    returns float not null,
    isActive bit not null
) go;

create table ConveyanceTypes
(
    conveyanceTypeId int identity primary key,
    name nvarchar(100) not null,
    returns float not null,
    isActive bit not null
) go;

create table CurrencyValues
(
    currencyValueId int identity primary key,
    name nvarchar(5) not null,
    value money not null
) go;

create table Delegations
(
    delegationId int identity primary key,
    startDate DateTime not null,
    endDate DateTime not null,
    departuePlace nvarchar(50) not null,
    destinationPlace nvarchar(50) not null,
    purpose nvarchar(256) not null,
    maxExpensePerEmployee money not null
) go;

create table TravelExpenses
(
    travelExpenseId int identity primary key,
    delegationId int not null references Delegations (delegationId),
    departuePlace nvarchar(50) not null,
    destinationPlace nvarchar(50) not null,
    conveyanceId int not null references ConveyanceTypes (conveyanceTypeId),
    distanceKM float null,
    startDate DateTime not null,
    endDate DateTime not null,
    cost money not null,
    currencyId int not null references CurrencyValues (currencyValueId)
) go;

create table EmployeeExpenses
(
    employeeExpenseId int identity primary key,
    delegationId int not null references Delegations (delegationId),
    employeeId int not null references Employees (employeeId),
    typeId int not null references ExpenseTypes (expenseTypeId),
    cost money not null,
    currencyId int not null references CurrencyValues (currencyValueId)
) go;

use CompanyDelegations
go;

-- Tworzenie procedur do pobierania kluczy przykładowych danych

create or alter function get_currency (@currencyName as nvarchar(5))
    returns int
as
    begin
        declare @id int;
        select top 1 @id = currencyValueId
            from CurrencyValues
            where name like @currencyName;
        return @id;
    end
go;

create or alter function get_employee_by_first_name (@firstName nvarchar(20))
    returns int
as
    begin
        declare @id int;
        select top 1 @id = em.employeeId
            from Employees as em
            where em.firstName like @firstName
              and em.isActive = convert(bit, 1);
        return @id;
    end
go;

create or alter function get_conveyance_type_by_name (@conveyanceName nvarchar(100))
    returns int
as
    begin
        declare @id int;
        select @id = conveyanceTypeId
            from ConveyanceTypes
            where name like @conveyanceName
              and isActive = convert(bit, 1);
        return @id;
    end
go;

create or alter function get_expense_type_by_name (@expenseTypeName nvarchar(100))
    returns int
as
    begin
        declare @id int;
        select top 1 @id = expenseTypeId
            from ExpenseTypes
            where name like @expenseTypeName
              and isActive = convert(bit, 1);
        return @id;
    end
go;

create or alter function get_delegation_by_index (@n int)
    returns int
as
    begin
        declare @id int;
        select @id = delegationId
        from Delegations
        order by delegationId
        offset @n - 1 rows
            fetch next 1 rows only;
        return @id;
    end
go;

-- Procedura do pobierania wartości waluty
create or alter function get_currency_value (@currencyId int)
    returns money
as
    begin
        declare @val money;
        select @val = value
            from CurrencyValues
            where currencyValueId = @currencyId;
        return @val;
    end
go;

-- Procedura do usuwania delegacji
if object_id('delete_delegation', 'P') is not null
drop procedure delete_delegation;
go
create procedure delete_delegation
    @delegationId int
as
    begin
        delete from TravelExpenses
            where delegationId = @delegationId;
        delete from EmployeeExpenses
            where delegationId = @delegationId;
        delete from Delegations
            where delegationId = @delegationId;
    end
go;

-- Procedura do usuwania pracownika [W domyśle nie usuwamy pracownika permanentnie, korzystamy z isActive, aby pozwolić na generowanie raportów z delegacji jeżeli się w nich znajduje]
if object_id('delete_employee', 'P') is not null
drop procedure delete_employee;
go
create procedure delete_employee
    @employeeId int
as
    begin
        if (
            select count(*)
                from EmployeeExpenses
                where employeeId = @employeeId
            ) > 0
                update Employees
                    set isActive = convert(bit, 0)
                    where employeeId = @employeeId;
            else
                delete from Employees
                    where employeeId = @employeeId;
    end
go;

-- Wyszukanie id pracownika, który wydawał najwięcej podczas delegacji
create or alter function get_most_active_employee_on_delegation_expenses (@delegationId int)
    returns int
as
    begin
        declare @employeeId int;
        select top 1 @employeeId = employeeId
            from EmployeeExpenses
            where delegationId = @delegationId
            group by employeeId
            order by sum(cost * dbo.get_currency_value(currencyId))
        return @employeeId;
    end
go;

---- Wprowadzanie danych

-- Dodawanie walut
insert into CurrencyValues
    (name, value)
values
    ('pln', 1),
    ('eur', 3.91),
    ('usd', 3.84),
    ('nerka', 40000),
    ('gbp', 5.12)
go;

-- Dodawanie pracowników
insert into Employees
    (firstName, lastName, position, isActive)
values
    (N'Juan', N'Kovvalsky', N'CEO', 1),
    (N'Katarzyna', N'Brzęczyszczykiewicz', N'HR Manager', 1),
    (N'Michaelo', N'Pinguin', N'Product owner', 1),
    (N'Wiktoria', N'Kowalska', N'Team leader', 1)
go;

-- Dodawanie środków transportu
insert into ConveyanceTypes
    (name, returns, isActive)
values
    (N'Koń', 30, 1),
    (N'Samolot', 100, 1),
    (N'Autobus', 100, 1),
    (N'Batmobil', 10, 1)
go;

-- Dodawanie rodzajów wydatków pracowników
insert into ExpenseTypes
    (name, returns, isActive)
values
    (N'Bar', 50, 1),
    (N'Restauracja', 99.22, 1),
    (N'Pizzeria', 80, 1),
    (N'Automat do kawy', 100, 1)
go;

-- Dodawanie delegacji
insert into Delegations
    (startDate, endDate, departuePlace, destinationPlace, purpose, maxExpensePerEmployee)
values
    (
        '2022-01-03 22:44:06.000',
        '2022-01-05 22:44:06.000',
         N'Kraków, siedziba firmy',
         N'Bojkowska, 44-100 Gliwice',
         N'Omówienie szczegółów umowy na wykonanie projektu o numerze 119285',
         800
    ),
    (
        '2022-06-24 22:44:06.000',
        '2022-06-30 22:44:06.000',
         N'Kraków, siedziba firmy',
         N'Willowa 2, 43-309 Bielsko-Biała',
         N'Dokształcanie pracowników',
         2500
    ),
    (
        '2022-10-05 22:44:06.000',
        '2022-10-07 22:44:06.000',
         N'Kraków, siedziba firmy',
         N'Misjonarzy Oblatów MN, 40-087 Katowice',
         N'Promocja nowych środków nasennych',
         1000
    ),
    (
        '2023-04-12 22:44:06.000',
        '2023-04-12 22:44:06.000',
         N'Kraków, siedziba firmy',
         N'Józefa Chełmońskiego 130R, 31-301 Kraków',
         N'Odbiór zamówienia',
         50
    ),
    (
        '2012-11-13 05:37:00.000',
        '2012-11-13 23:15:00.000',
         N'Kraków, siedziba firmy',
         N'Kraków, siedziba firmy',
         N'Juan, czyli tam i z powrotem',
         80000
    )
go;

-- Dodawanie kosztów podróży
insert into TravelExpenses
    (delegationId, departuePlace, destinationPlace, conveyanceId, distanceKM, startDate, endDate, cost, currencyId)
values
    (
         dbo.get_delegation_by_index(1),
         N'Kraków, siedziba firmy',
         N'Bojkowska, 44-100 Gliwice',
         dbo.get_conveyance_type_by_name('Batmobil'),
         100,
         '2022-01-03 22:44:06.000',
         '2022-01-03 22:44:06.000',
         1,
         dbo.get_currency('nerka')
    ),
    (
         dbo.get_delegation_by_index(2),
         N'Kraków, siedziba firmy',
         N'Willowa 2, 43-309 Bielsko-Biała',
         dbo.get_conveyance_type_by_name(N'Samolot'),
         80,
         '2022-06-24 22:44:06.000',
         '2022-06-25 22:44:06.000',
         20301,
         dbo.get_currency('pln')
    ),
    (
         dbo.get_delegation_by_index(2),
         N'Willowa 2, 43-309 Bielsko-Biała',
         N'Kraków, siedziba firmy',
         dbo.get_conveyance_type_by_name(N'Samolot'),
         85,
         '2022-06-29 22:44:06.000',
         '2022-06-30 22:44:06.000',
         8200,
         dbo.get_currency('eur')
    )
go;

-- Dodawanie kosztów pracowników
insert into EmployeeExpenses
    (delegationId, employeeId, typeId, cost, currencyId)
values (
            dbo.get_delegation_by_index(1),
            dbo.get_employee_by_first_name('Juan'),
            dbo.get_expense_type_by_name('Bar'),
            2000,
            dbo.get_currency('pln')
       ),(
            dbo.get_delegation_by_index(2),
            dbo.get_employee_by_first_name('Wiktoria'),
            dbo.get_expense_type_by_name('Automat do kawy'),
            2500.1,
            dbo.get_currency('pln')
       ),(
            dbo.get_delegation_by_index(3),
            dbo.get_employee_by_first_name('Katarzyna'),
            dbo.get_expense_type_by_name('Pizzeria'),
            21.37,
            dbo.get_currency('eur')
       )
go;