namespace ServerApp.SupportClasses
{
    public static class SqlCommands
    {
        public const string LoadUserCommand =
            @"select 
	            bs.ID, pr.TelegramID, bs.State, bs.InputtedLogin, bs.ChoosenDocType, bs.DocumentTypesPage, bs.ChoosenRole, bs.PersonalRolesPage, bs.CurrentTasksPage, bs.HistoryPage 
            from 
	            BotStat bs with(nolock) 
	            inner join PersonalRoles pr with(nolock) on pr.ID=bs.ID
            where
	            pr.TelegramID=@ID";

        public const string LoginCommand =
            @"select ID, PassWord, isnull(TelegramID, 0) from PersonalRoles with(nolock) where Login=@LoginText";

    }
}
