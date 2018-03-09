namespace AdminApp.SupportClasses
{
    public static class SqlCommands
    {
        public const string SetInfoToGridPersonalRoles =
            @"select t.ID, t.Login, isnull(t.TelegramID,'') as TelegramID, t.FirstName, t.LastName, d.Caption from PersonalRoles t 
            inner join WorkingType d on t.WorkingTypeID=d.ID order by t.Login";

        public const string SetInfoToGridStaticRoles =
            @"select t.ID, t.Caption from StaticRoles t 
            where t.ID!='fffee627-a5a6-4345-bc55-8fba3709dc48' order by t.Caption";

        public const string SetInfoToGridDocTypes =
            @"select ID, Caption from DocTypes order by Caption";

        public const string LoadRoleUsers =
            @"select PersonID from RoleUsers where RoleID = @RoleID";

        public const string DeletePersonalRole =
            @"delete from RoleUsers where PersonID=@ID; delete from PersonalRoles where ID=@ID; delete from Roles where ID=@ID;";

        public const string DeleteStaticRole =
            @"delete from RoleUsers where RoleID=@ID; delete from StaticRoles where ID=@ID; delete from Roles where ID=@ID;";

        public const string CheckDeletePersonalRole =
            @"select count(*) from Tasks where ToRoleID=@ID";

        public const string CheckDeleteStaticRole =
            @"select count(*) from Tasks where ToRoleID=@ID";

        public const string LoginCommand =
            @"select ID, PassWord, isnull(TelegramID, 0), FirstName, LastName, FullName, isAdmin from PersonalRoles where Login=@LoginText";

        public const string CheckLoginCommand =
            @"select count(*) from PersonalRoles where Login=@Login";

        public const string LoadPersonalRoleCommand =
            @"select ID, Login, PassWord, isnull(TelegramID,0), FullName, FirstName, LastName, isAdmin, WorkingTypeID, isEditingNow from PersonalRoles where ID=@RoleID";

        public const string LoadStaticRoleCommand =
            @"select ID, Name, Caption, isEditingNow from StaticRoles where ID=@RoleID";

        public const string SetEditingToPersonalRole =
            @"update PersonalRoles set isEditingNow=1 where ID=@ID";

        public const string SetEditingToStaticRole =
            @"update StaticRoles set isEditingNow=1 where ID=@ID";

        public const string SetStopEditingToPersonalRole =
            @"update PersonalRoles set isEditingNow=0 where ID=@ID";

        public const string SetStopEditingToStaticRole =
            @"update StaticRoles set isEditingNow=0 where ID=@ID";

        public const string LoadWorkingTypeCommand =
            @"select ID, Name, Caption from WorkingType where ID=@ID";

        public const string LoadAllWorkingTypesCommand =
            @"select ID, NAme, Caption from WorkingType";

        public const string DeleteRoleRoleUsers =
            @"delete from RoleUsers where RoleID=@RoleID and PersonID=@PersonID";
    }
}
