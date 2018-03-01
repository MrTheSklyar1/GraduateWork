use Base;

DECLARE @DocID uniqueidentifier = '4e54d2e9-20e6-4f58-905f-89d8b4084b2c';
DECLARE @TaskID uniqueidentifier = 'a234793a-6028-414d-8cbe-049b3e890209';
DECLARE @TaskID2 uniqueidentifier = 'a234793a-6024-414d-8cbe-049b3e890209';
DECLARE @TaskID3 uniqueidentifier = 'a234793a-6024-414d-8cbe-049b3e890210';
DECLARE @TaskID4 uniqueidentifier = 'a234793a-6024-414d-8cbe-049b3e890211';
DECLARE @TaskID5 uniqueidentifier = 'a234793a-6024-414d-8cbe-049b3e890212';
DECLARE @RoleID uniqueidentifier = 'c12239f2-d6ee-46ea-b6fa-cc63f1dce800';
DECLARE @AdminID uniqueidentifier = 'f111d495-1aa4-468c-9885-30e4ad13ecd8';
DECLARE @PersonalRoleID uniqueidentifier = 'fffee627-a5a6-4345-bc55-8fba3709dc48';
DECLARE @BuhUchet uniqueidentifier = 'ff5ee627-a5a6-4345-bc55-8fba3709dc48';
DECLARE @HR uniqueidentifier = 'ff5ee627-a5a6-4345-bc55-8fba3709dc49';
DECLARE @WorkID uniqueidentifier = '6a52791d-7e42-42d6-a521-4252f276bb6c';
DECLARE @CompletedID uniqueidentifier = '530e4d08-9ef0-48ce-8bb7-f0a989ae53ae';
DECLARE @CancelledID uniqueidentifier = '3e65b0c5-f533-4e31-956d-c2073df3e58a';

insert into Roles values (@HR, 'Кадры');
insert into StaticRoles values (@HR, 'HR', 'Кадры');
insert into Roles values (@BuhUchet, 'Бухгалтерский учет');
insert into StaticRoles values (@BuhUchet, 'BuhUchet', 'Бухгалтерский учет');
insert into RoleUsers values (@BuhUchet, @AdminID);
insert into DocTypes values (@DocID, '2NDFL', '2 НДФЛ');
insert into Tasks values (@TaskID2, @AdminID, 'Admin A.', @AdminID, 'Admin A.',SYSDATETIME(), @DocID, @WorkID,'Test doc', NULL, NULL);
insert into Tasks values (@TaskID, @AdminID, 'Admin A.', @BuhUchet, 'Бухгалтерский учет',SYSDATETIME(), @DocID, @WorkID,'Test doc2', 'Resp 2', NULL);
insert into Tasks values (@TaskID3, @AdminID, 'Admin A.', @HR, 'Кадры',SYSDATETIME(), @DocID, @WorkID, 'Test doc3', 'Resp 3', NULL);
insert into Tasks values (@TaskID4, @AdminID, 'Admin A.', @BuhUchet, 'Бухгалтерский учет',SYSDATETIME(), @DocID, @CompletedID, 'Test doc4', 'Resp 4', NULL);
insert into Tasks values (@TaskID5, @AdminID, 'Admin A.', @BuhUchet, 'Бухгалтерский учет',SYSDATETIME(), @DocID, @CompletedID, 'Test doc5', 'Resp 5', @AdminID);
