create database Base;

create table PersonalRoles
(
	ID	uniqueidentifier NOT NULL,
	Login nvarchar(50) NOT NULL,
	PassWord nvarchar(512) NOT NULL,
	TelegramID int NULL,
	FirstName nvarchar(50) NOT NULL,
	LastName nvarchar(50) NOT NULL,
	isAdmin bit NOT NULL
);

insert into PersonalRoles values (NEWID(), 'Admin', 'e3afed0047b08059d0fada10f400c1e5', NULL, 'Admin', 'Admin', 1);
