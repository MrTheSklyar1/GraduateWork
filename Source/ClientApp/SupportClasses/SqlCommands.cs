
namespace ClientApp.SupportClasses
{
    public static class SqlCommands
    {
        public const string SetInfoToGridWorkCommand =
            @"select t.ID, t.Number, t.Date, d.Caption, t.FromPersonalName, t.ToRoleName from Tasks t 
                inner join (
                    select RoleID as ID from RoleUsers ru where ru.PersonID=@UserID
                    union
                    select ID as RoleID from PersonalRoles where ID=@UserID
                ) temp on t.ToRoleID=temp.ID
            inner join DocTypes d on t.DocType=d.ID where StateID='6a52791d-7e42-42d6-a521-4252f276bb6c'";

        public const string SetInfoToGridEndWorkCommand =
            @"select t.ID, t.Number, t.Date, d.Caption, t.FromPersonalName, t.ToRoleName, ts.Caption, r.Name, t.CompleteDate from Tasks t 
                inner join (
                    select RoleID as ID from RoleUsers ru where ru.PersonID=@UserID
                    union
                    select ID as RoleID from PersonalRoles where ID=@UserID
                ) temp on t.ToRoleID=temp.ID
            inner join DocTypes d on t.DocType=d.ID 
            inner join TaskState ts on t.StateID=ts.ID
            inner join Roles r on r.ID=t.CompletedByID
            where StateID='530e4d08-9ef0-48ce-8bb7-f0a989ae53ae' or StateID='3e65b0c5-f533-4e31-956d-c2073df3e58a'";

        public const string SetInfoToGridPersonalCommand =
            @"select t.ID, t.Number, t.Date, d.Caption, t.FromPersonalName from Tasks t
                inner join DocTypes d on t.DocType=d.ID 
                where StateID='6a52791d-7e42-42d6-a521-4252f276bb6c' and ToRoleID=@UserID";

        public const string SetInfoToGridOtherCommand =
            @"select t.ID, t.Number, t.Date, d.Caption, t.FromPersonalName from Tasks t
                inner join DocTypes d on t.DocType=d.ID
                where StateID='6a52791d-7e42-42d6-a521-4252f276bb6c' and ToRoleID=@RoleID";

        public const string FindAllRolesCommand =
            @"select ru.RoleID, r.Name, r.Caption from RoleUsers ru join StaticRoles r on ru.RoleID=r.ID where ru.PersonID=@UserID";

        public const string LoginCommand =
            @"select ID, PassWord, isnull(TelegramID, 0), FirstName, LastName, FullName from PersonalRoles where Login=@LoginText";

        public const string LoadTaskCommand =
            @"select ID, Number, FromPersonalID, FromPersonalName, ToRoleID, ToRoleName, Date, DocType, StateID, isnull(Commentary,''), isnull(Respond,''), isnull(CompletedByID, cast(cast(0 as binary) as uniqueidentifier)), isnull(CompleteDate, convert(datetime, '2000')), MainNumber , isEditingNow
                from Tasks where ID=@TaskID";

        public const string LoadPersonalRoleCommand =
            @"select ID, Login, PassWord, isnull(TelegramID,0), FullName, FirstName, LastName, isAdmin, WorkingTypeID from PersonalRoles where ID=@RoleID";

        public const string LoadStaticRoleCommand =
            @"select r.ID, isnull(sr.Caption, r.Name) as Caption from StaticRoles sr
                right join Roles r on r.ID=sr.ID where r.ID=@RoleID";

        public const string LoadDocTypeCommand =
            @"select ID, Name, Caption from DocTypes where ID=@TypeID";

        public const string LoadFilesCommand =
            @"select FileID, Name from Files where ID=@TaskID";

        public const string LoadStateCommand =
            @"select ID, Name, Caption from TaskState where ID=@StateID";

        public const string LoadAllStatesCommand =
            @"select ID, Name, Caption from TaskState";

        public const string DeleteFileCommand =
            @"delete from Files where FileID=@FileID";

        public const string SetNewStateCommand =
            @"update Tasks set StateID=@StateID where ID=@TaskID";

        public const string SetNewRespondCommand =
            @"update Tasks set Respond=@Respond where ID=@TaskID";

        public const string SetCompleteInfoCommand =
            @"update Tasks set CompletedByID=@UserID, CompleteDate=SYSDATETIME() where ID=@TaskID";

        public const string AddFileToDataBaseCommand =
            @"insert into Files values (@TaskID, @FileID, @FileName);";

        public const string SetEditingToTask =
            @"update Tasks set isEditingNow=1 where ID=@TaskID";

        public const string SetStopEditingToTask =
            @"update Tasks set isEditingNow=0 where ID=@TaskID";

        public const string DeleteTaskAndStaff =
            @"delete from Files where ID=@TaskID; delete from Tasks where ID=@TaskID;";
    }
}
