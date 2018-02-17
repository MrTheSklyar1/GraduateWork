DECLARE @AdminID uniqueidentifier = NEWID();
DECLARE @PersonalRoleID uniqueidentifier = NEWID();

create database Base;

use Base;

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

insert into PersonalRoles values (@AdminID, 'Admin', 'e3afed0047b08059d0fada10f400c1e5', NULL, 'Admin', 'Admin', 1);

create table Roles
(
	ID	uniqueidentifier NOT NULL,
	Name nvarchar(50) NOT NULL,
  Caption nvarchar(50) NOT NULL
);

insert into Roles values (@PersonalRoleID, 'PersonalRole', 'Personal Role');

create table RoleUsers
(
	ID	uniqueidentifier NOT NULL,
	RoleID uniqueidentifier NOT NULL
);

insert into RoleUsers values (@AdminID, @PersonalRoleID);

