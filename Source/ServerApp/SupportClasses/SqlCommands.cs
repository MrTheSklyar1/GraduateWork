namespace ServerApp.SupportClasses
{
    public static class SqlCommands
    {
        public const string LoadUserCommand =
            @"select ID, State, ChoosenDocType, DocumentTypesPage, ChoosenRole, PersonalRolesPage, CurrentTasksPage, HistoryPage from BotStat with(nolock) where ID=@ID";

    }
}
