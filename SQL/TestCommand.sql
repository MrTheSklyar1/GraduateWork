DECLARE @DocID uniqueidentifier = '4e54d2e9-20e6-4f58-905f-89d8b4084b2c';
DECLARE @TaskID uniqueidentifier = 'a234793a-6028-414d-8cbe-049b3e890209';
DECLARE @TaskID2 uniqueidentifier = 'a234793a-6024-414d-8cbe-049b3e890209';
DECLARE @TaskID3 uniqueidentifier = 'a234793a-6024-414d-8cbe-049b3e890210';
DECLARE @RoleID uniqueidentifier = 'c12239f2-d6ee-46ea-b6fa-cc63f1dce800';
DECLARE @AdminID uniqueidentifier = 'f111d495-1aa4-468c-9885-30e4ad13ecd8';
DECLARE @PersonalRoleID uniqueidentifier = 'fffee627-a5a6-4345-bc55-8fba3709dc48';
DECLARE @BuhUchet uniqueidentifier = 'ff5ee627-a5a6-4345-bc55-8fba3709dc48';
DECLARE @HR uniqueidentifier = 'ff5ee627-a5a6-4345-bc55-8fba3709dc49';

insert into Roles values (@HR, 'Кадры');
insert into StaticRoles values (@HR, 'HR', 'Кадры');
insert into Roles values (@BuhUchet, 'Бухгалтерский учет');
insert into StaticRoles values (@BuhUchet, 'BuhUchet', 'Бухгалтерский учет');
insert into RoleUsers values (@BuhUchet, @AdminID);
insert into DocTypes values (@DocID, '2NDFL', '2 НДФЛ');
insert into Tasks values (@TaskID2, @AdminID, 'Admin A.', @AdminID, 'Admin A.',SYSDATETIME(), @DocID, 0, 'Test doc', NULL);
insert into Tasks values (@TaskID, @AdminID, 'Admin A.', @BuhUchet, 'Бухгалтерский учет',SYSDATETIME(), @DocID, 0, 'Test doc2', NULL);
insert into Tasks values (@TaskID3, @AdminID, 'Admin A.', @HR, 'Кадры',SYSDATETIME(), @DocID, 0, 'Test doc3', NULL);
