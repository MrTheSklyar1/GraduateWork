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
            @"select ID from DocTypes with(nolock)";

        public const string LoadDocumentTagsCommand =
            @"select TagWords from DocTypes with(nolock) where ID=@ID";
    }
}
