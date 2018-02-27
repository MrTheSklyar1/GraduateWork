create database Base;

DECLARE @AdminID uniqueidentifier = NEWID();
DECLARE @PersonalRoleID uniqueidentifier = NEWID();

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

insert into PersonalRoles values (@AdminID, 'Admin', 'e3afed0047b08059d0fada10f400c1e5', NULL, 'Admin A.', 'Admin', 'Admin', 1);
insert into RoleUsers values (@AdminID, @PersonalRoleID);

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
	ToRoleName nvarchar(50) NOT NULL,
	Date datetime NOT NULL,
	DocType uniqueidentifier NOT NULL,
	isCompleted bit NOT NULL,
	Commentary nvarchar(512) NULL,
	FileID uniqueidentifier NULL
);


ALTER TABLE Tasks ADD PRIMARY KEY(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(FromPersonalID) REFERENCES PersonalRoles(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(ToRoleID) REFERENCES Roles(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(DocType) REFERENCES DocTypes(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(FileID) REFERENCES Files(ID);



--Test Var


