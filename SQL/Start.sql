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

create table TaskState
(
	ID uniqueidentifier NOT NULL,
	Name nvarchar(50) NOT NULL,
	Caption nvarchar(50) NOT NULL
);

ALTER TABLE TaskState ADD PRIMARY KEY (ID);

DECLARE @CompletedID uniqueidentifier = '530e4d08-9ef0-48ce-8bb7-f0a989ae53ae';
DECLARE @CancelledID uniqueidentifier = '3e65b0c5-f533-4e31-956d-c2073df3e58a';
DECLARE @WorkID uniqueidentifier = '6a52791d-7e42-42d6-a521-4252f276bb6c';

insert into TaskState values (@CompletedID, 'Completed', 'Completed');
insert into TaskState values (@CancelledID, 'Cancelled', 'Cancelled');
insert into TaskState values (@WorkID, 'Work', 'Work');

create table Tasks
(
	ID	uniqueidentifier NOT NULL,
	FromPersonalID uniqueidentifier NOT NULL,
	FromPersonalName nvarchar(100) NOT NULL,
	ToRoleID uniqueidentifier NOT NULL,
	ToRoleName nvarchar(100) NOT NULL,
	Date datetime NOT NULL,
	DocType uniqueidentifier NOT NULL,
	StateID uniqueidentifier NOT NULL,
	Commentary nvarchar(512) NULL,
	FileID uniqueidentifier NULL
);

ALTER TABLE Tasks ADD PRIMARY KEY(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(FromPersonalID) REFERENCES  Roles(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(ToRoleID) REFERENCES Roles(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(DocType) REFERENCES DocTypes(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(FileID) REFERENCES Files(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(StateID) REFERENCES TaskState(ID);
