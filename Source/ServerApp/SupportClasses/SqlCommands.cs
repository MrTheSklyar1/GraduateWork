namespace ServerApp.SupportClasses
{
    public static class SqlCommands
    {
        public const string LoadUserCommand =
            @"select 
	            bs.ID, bs.State, bs.ChoosenDocType, bs.DocumentTypesPage, bs.ChoosenRole, bs.PersonalRolesPage, bs.CurrentTasksPage, bs.HistoryPage 
            from 
	            BotStat bs with(nolock) 
	            inner join PersonalRoles pr with(nolock) on pr.ID=bs.ID
            where
	            pr.TelegramID=@ID";

        public const string LoginCommand =
            @"select ID, PassWord from PersonalRoles with(nolock) where Login=@Login";

        public const string GetLoginCommand =
            @"select Login from PersonalRoles with(nolock) where TelegramID=@ID";

        public const string LoadDocumentTypesCommand =
            @"select ID, Caption from DocTypes with(nolock)";

        public const string LoadDocumentTagsCommand =
            @"select TagWords from DocTypes with(nolock) where ID=@ID";

        public const string FindDocStaticRoleCommand =
            @"select count(*) from DocTypes dt with(nolock) inner join StaticRoles sr with(nolock) on dt.RoleTypeID=sr.ID where dt.ID=@ID";

        public const string SelectThreeDocTypesByPage =
            @"with OrderedRecords as
            (
                select Caption, 
                row_number() over (order by Caption) as 'RowNumber'
                from DocTypes with(nolock)
            ) 
            select * from OrderedRecords where RowNumber between @PageStart and @PageEnd";

        public const string SelectThreePersRolesByPage =
            @"with OrderedRecords as
            (
                select LastName + ' ' + FirstName as FullName, 
                row_number() over (order by (LastName + ' ' + FirstName)) as 'RowNumber'
                from PersonalRoles pr with(nolock) 
                inner join RoleUsers ru with(nolock) 
                on pr.ID=ru.PersonID
	            where ru.RoleID=@ID
            ) 
            select * from OrderedRecords where RowNumber between @PageStart and @PageEnd";

        public const string CountDocTypes =
            @"select count(*) from DocTypes with(nolock)";

        public const string CountPersonalRolesFromStatic =
            @"select count(*) from RoleUsers with(nolock) where RoleID=@ID";

        public const string GetRoleFromDocType =
            @"select RoleTypeID from DocTypes  with(nolock) where ID=@ID";

        public const string FindRoleByLatAndFirstName =
            @"select ID from PersonalRoles with(nolock) where LastName + ' ' + FirstName=@text";
    }
}
