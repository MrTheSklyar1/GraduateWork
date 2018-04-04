--create database Base;
use Base;
DECLARE @AdminID uniqueidentifier = 'f111d495-1aa4-468c-9885-30e4ad13ecd8';
DECLARE @PersonalRoleID uniqueidentifier = 'fffee627-a5a6-4345-bc55-8fba3709dc48';
DECLARE @CardReviewerRoleID uniqueidentifier = '9efcd5cd-bf54-47f3-95e3-2953cb235941';

create table Roles
(
	ID	uniqueidentifier NOT NULL,
	Name nvarchar(100) NOT NULL
);

ALTER TABLE Roles ADD PRIMARY KEY (ID);
CREATE INDEX ix_RoleID ON Roles(ID);
insert into Roles values (@AdminID, 'Admin A.');
insert into Roles values (@PersonalRoleID, 'Personal Role');
insert into Roles values (@CardReviewerRoleID, 'Card Reviewer Role');

create table WorkingType
(
	ID uniqueidentifier NOT NULL,
	Name nvarchar(50) NOT NULL,
	Caption nvarchar(50) NOT NULL
);

ALTER TABLE WorkingType ADD PRIMARY KEY (ID);

DECLARE @WorkingID uniqueidentifier = '642faf68-37f3-4fd6-a97d-7abfe5a9a921';
DECLARE @DismissedID uniqueidentifier = 'e2f7f7ee-5eba-4d6f-8a0e-8089283b0d78';
DECLARE @OnLeaveID uniqueidentifier = 'c3df3ff7-2822-465d-bfbf-e8603ae6b1e6';

insert into WorkingType values (@WorkingID, 'Working', 'Working');
insert into WorkingType values (@DismissedID, 'Dismissed', 'Dismissed');
insert into WorkingType values (@OnLeaveID, 'OnLeave', 'On leave');

create table PersonalRoles
(
	ID	uniqueidentifier NOT NULL,
	Login nvarchar(50) NOT NULL,
	PassWord nvarchar(512) NOT NULL,
	TelegramID bigint NULL,
	FullName nvarchar(100) NOT NULL,
	FirstName nvarchar(50) NOT NULL,
	LastName nvarchar(50) NOT NULL,
	isAdmin bit NOT NULL,
	WorkingTypeID uniqueidentifier NOT NULL,
	isEditingNow bit NOT NULL
);
CREATE INDEX ix_PersonalRoleID ON PersonalRoles(ID);
CREATE INDEX ix_TelegramID ON PersonalRoles(TelegramID);
ALTER TABLE PersonalRoles ADD FOREIGN KEY(ID) REFERENCES Roles(ID);
ALTER TABLE PersonalRoles ADD FOREIGN KEY(WorkingTypeID) REFERENCES WorkingType(ID);

insert into PersonalRoles values (@AdminID, 'Admin', 'e3afed0047b08059d0fada10f400c1e5', NULL, 'Admin A.', 'Admin', 'Admin', 1, @WorkingID, 0);

create table StaticRoles
(
	ID	uniqueidentifier NOT NULL,
	Name nvarchar(50) NOT NULL,
    Caption nvarchar(50) NOT NULL,
	isEditingNow bit NOT NULL
);

ALTER TABLE StaticRoles ADD FOREIGN KEY(ID) REFERENCES Roles(ID);
CREATE INDEX ix_StaticRoleID ON StaticRoles(ID);
insert into StaticRoles values (@PersonalRoleID, 'PersonalRole', 'Personal Role', 0);
insert into StaticRoles values (@CardReviewerRoleID, 'CardReviewerRole', 'Card Reviewer Role', 0);

create table RoleUsers
(
	RoleID	uniqueidentifier NOT NULL,
	PersonID uniqueidentifier NOT NULL
);
CREATE INDEX ix_RoleUsersRoleID ON RoleUsers(RoleID);
CREATE INDEX ix_RoleUsersPersonID ON RoleUsers(PersonID);
ALTER TABLE RoleUsers ADD FOREIGN KEY(RoleID) REFERENCES Roles(ID);
ALTER TABLE RoleUsers ADD FOREIGN KEY(PersonID) REFERENCES Roles(ID);

insert into RoleUsers values (@PersonalRoleID, @AdminID);
insert into RoleUsers values (@CardReviewerRoleID, @AdminID);

create table DocTypes
(
	ID uniqueidentifier NOT NULL,
	Name nvarchar(50) NOT NULL,
	Caption nvarchar(50) NOT NULL,
	TagWords nvarchar(MAX) NOT NULL,
	RoleTypeID uniqueidentifier NOT NULL,
	isEditingNow bit NOT NULL
);
CREATE INDEX ix_DocTypesID ON DocTypes(ID);
ALTER TABLE DocTypes ADD PRIMARY KEY (ID);
ALTER TABLE DocTypes ADD FOREIGN KEY(RoleTypeID) REFERENCES  Roles(ID);

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
	MainNumber bigint NOT NULL IDENTITY(1,1),
	ID	uniqueidentifier NOT NULL,
	Number nvarchar(20) NOT NULL,
	FromPersonalID uniqueidentifier NOT NULL,
	FromPersonalName nvarchar(100) NOT NULL,
	ToRoleID uniqueidentifier NOT NULL,
	ToRoleName nvarchar(100) NOT NULL,
	Date datetime NOT NULL,
	DocType uniqueidentifier NOT NULL,
	StateID uniqueidentifier NOT NULL,
	Commentary nvarchar(2048) NULL,
	Respond nvarchar(2048) NULL,
	CompletedByID uniqueidentifier NULL,
	CompleteDate datetime NULL,
	isEditingNow bit NOT NULL
);
CREATE INDEX ix_MainNumber ON Tasks(MainNumber);
CREATE INDEX ix_TasksID ON Tasks(ID);
ALTER TABLE Tasks ADD PRIMARY KEY(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(FromPersonalID) REFERENCES  Roles(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(ToRoleID) REFERENCES Roles(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(DocType) REFERENCES DocTypes(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(StateID) REFERENCES TaskState(ID);
ALTER TABLE Tasks ADD FOREIGN KEY(CompletedByID) REFERENCES Roles(ID);

create table Files
(
	ID uniqueidentifier NOT NULL,
	FileID uniqueidentifier NOT NULL,
	Name nvarchar(MAX) NOT NULL
);
CREATE INDEX ix_FileID ON Files(FileID);
ALTER TABLE Files ADD PRIMARY KEY(FileID);
ALTER TABLE Files ADD FOREIGN KEY(ID) REFERENCES Tasks(ID);

create table CompleteQueue
(
	TaskID uniqueidentifier NOT NULL
);
CREATE INDEX ix_TaskID ON CompleteQueue(TaskID);
ALTER TABLE CompleteQueue ADD FOREIGN KEY(TaskID) REFERENCES Tasks(ID);

create table BotStat
(
	ID uniqueidentifier NOT NULL,
	ChatID bigint NULL,
	State int NOT NULL,
	ChoosenDocType uniqueidentifier  NULL,
	DocumentTypesPage int NOT NULL,
	ChoosenRole uniqueidentifier  NULL,
	PersonalRolesPage int NOT NULL,
	CurrentTasksPage int NOT NULL,
	HistoryPage int NOT NULL,
	Commentary nvarchar(MAX) NULL
);
CREATE INDEX ix_BotStatID ON BotStat(ID);
ALTER TABLE BotStat ADD FOREIGN KEY(ID) REFERENCES Roles(ID);
ALTER TABLE BotStat ADD FOREIGN KEY(ChoosenDocType) REFERENCES DocTypes(ID);
ALTER TABLE BotStat ADD FOREIGN KEY(ChoosenRole) REFERENCES Roles(ID);
insert into BotStat values (@AdminID, null, 0, null, 1, null, 1, 1, 1, null);
use master;