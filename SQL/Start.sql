create database Base;

DECLARE @AdminID uniqueidentifier = 'f111d495-1aa4-468c-9885-30e4ad13ecd8';
DECLARE @PersonalRoleID uniqueidentifier = 'fffee627-a5a6-4345-bc55-8fba3709dc48';

create table Roles
(
	ID	uniqueidentifier NOT NULL,
	Name nvarchar(100) NOT NULL
);

ALTER TABLE Roles ADD PRIMARY KEY (ID);

insert into Roles values (@AdminID, 'Admin A.');
insert into Roles values (@PersonalRoleID, 'Personal Role');

create table PersonalRoles
(
	ID	uniqueidentifier NOT NULL,
	Login nvarchar(50) NOT NULL,
	PassWord nvarchar(512) NOT NULL,
	TelegramID int NULL,
	FullName nvarchar(100) NOT NULL,
	FirstName nvarchar(50) NOT NULL,
	LastName nvarchar(50) NOT NULL,
	isAdmin bit NOT NULL
);

ALTER TABLE PersonalRoles ADD FOREIGN KEY(ID) REFERENCES Roles(ID);

insert into PersonalRoles values (@AdminID, 'Admin', 'e3afed0047b08059d0fada10f400c1e5', NULL, 'Admin A.', 'Admin', 'Admin', 1);

create table StaticRoles
(
	ID	uniqueidentifier NOT NULL,
	Name nvarchar(50) NOT NULL,
    Caption nvarchar(50) NOT NULL
);

ALTER TABLE StaticRoles ADD FOREIGN KEY(ID) REFERENCES Roles(ID);

insert into StaticRoles values (@PersonalRoleID, 'PersonalRole', 'Personal Role');

create table RoleUsers
(
	RoleID	uniqueidentifier NOT NULL,
	PersonID uniqueidentifier NOT NULL
);

ALTER TABLE RoleUsers ADD FOREIGN KEY(RoleID) REFERENCES Roles(ID);
ALTER TABLE RoleUsers ADD FOREIGN KEY(PersonID) REFERENCES Roles(ID);

insert into RoleUsers values (@PersonalRoleID, @AdminID);

create table DocTypes
(
	ID uniqueidentifier NOT NULL,
	Name nvarchar(50) NOT NULL,
	Caption nvarchar(50) NOT NULL
);

ALTER TABLE DocTypes ADD PRIMARY KEY (ID);

create table Files
(
	ID uniqueidentifier NOT NULL
);

ALTER TABLE Files ADD PRIMARY KEY (ID);

create table Tasks
(
	ID	uniqueidentifier NOT NULL,
	FromPersonalID uniqueidentifier NOT NULL,
	FromPersonalName nvarchar(100) NOT NULL,
	ToRoleID uniqueidentifier NOT NULL,
	ToRoleName nvarchar(100) NOT NULL,
	Date datetime NOT NULL,
	DocType uniqueidentifier NOT NULL,
	isCompleted bit NOT NULL,
	Commentary nvarchar(512) NULL,
	FileID uniqueidentifier NULL
);

ALTER TABLE Tasks ADD PRIMARY KEY(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(FromPersonalID) REFERENCES  Roles(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(ToRoleID) REFERENCES Roles(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(DocType) REFERENCES DocTypes(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(FileID) REFERENCES Files(ID);

--Test Var
DECLARE @DocID uniqueidentifier = '4e54d2e9-20e6-4f58-905f-89d8b4084b2c';
DECLARE @TaskID uniqueidentifier = 'a234793a-6028-414d-8cbe-049b3e890209';
DECLARE @RoleID uniqueidentifier = 'c12239f2-d6ee-46ea-b6fa-cc63f1dce800';
DECLARE @AdminID uniqueidentifier = 'f111d495-1aa4-468c-9885-30e4ad13ecd8';
DECLARE @PersonalRoleID uniqueidentifier = 'fffee627-a5a6-4345-bc55-8fba3709dc48';
DECLARE @BuhUchet uniqueidentifier = 'ff5ee627-a5a6-4345-bc55-8fba3709dc48';
insert into Roles values (@BuhUchet, 'Бухгалтерский учет');
insert into StaticRoles values (@BuhUchet, 'BuhUchet', 'Бухгалтерский учет');
insert into RoleUsers values (@BuhUchet, @AdminID);
insert into DocTypes values (@DocID, '2NDFL', '2 НДФЛ');
insert into Tasks values (@TaskID, @AdminID, 'Admin A.', @AdminID, 'Admin A.',SYSDATETIME(), @DocID, 0, 'Test doc', NULL);
