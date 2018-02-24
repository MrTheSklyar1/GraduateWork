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

ALTER TABLE PersonalRoles ADD PRIMARY KEY (ID);

create table Roles
(
	ID	uniqueidentifier NOT NULL,
	Name nvarchar(50) NOT NULL,
    Caption nvarchar(50) NOT NULL
);

ALTER TABLE Roles ADD PRIMARY KEY (ID);

insert into Roles values (@PersonalRoleID, 'PersonalRole', 'Personal Role');

create table RoleUsers
(
	ID	uniqueidentifier NOT NULL,
	RoleID uniqueidentifier NOT NULL
);

ALTER TABLE RoleUsers ADD FOREIGN KEY(ID) REFERENCES PersonalRoles(ID);
ALTER TABLE RoleUsers ADD FOREIGN KEY(RoleID) REFERENCES Roles(ID);

insert into PersonalRoles values (@AdminID, 'Admin', 'e3afed0047b08059d0fada10f400c1e5', NULL, 'Admin', 'Admin', 1);
insert into RoleUsers values (@AdminID, @PersonalRoleID);

