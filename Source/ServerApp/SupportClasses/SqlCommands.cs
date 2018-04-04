namespace ServerApp.SupportClasses
{
    public static class SqlCommands
    {
        public const string LoadUserCommand =
            @"select 
	            bs.ID, bs.ChatID, bs.State, bs.ChoosenDocType, bs.DocumentTypesPage, bs.ChoosenRole, bs.PersonalRolesPage, bs.CurrentTasksPage, bs.HistoryPage, bs.Commentary 
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

        public const string SelectThreeCurrentTasksByPage =
            @"with OrderedRecords as
            (
                select Number, 
                row_number() over (order by Number) as 'RowNumber'
                from Tasks with(nolock)
                where StateID='6A52791D-7E42-42D6-A521-4252F276BB6C' and FromPersonalID=@ID
            ) 
            select * from OrderedRecords where RowNumber between @PageStart and @PageEnd";

        public const string SelectThreeHistoryTasksByPage =
            @"with OrderedRecords as
            (
                select Number, 
                row_number() over (order by Number) as 'RowNumber'
                from Tasks with(nolock)
                where StateID!='6A52791D-7E42-42D6-A521-4252F276BB6C' and FromPersonalID=@ID
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
	            where ru.RoleID=@ID and pr.WorkingTypeID='642faf68-37f3-4fd6-a97d-7abfe5a9a921'
            ) 
            select * from OrderedRecords where RowNumber between @PageStart and @PageEnd";

        public const string CountDocTypes =
            @"select count(*) from DocTypes with(nolock)";

        public const string CountCurrentTasks =
            @"select count(*) from Tasks with(nolock) where StateID='6A52791D-7E42-42D6-A521-4252F276BB6C' and FromPersonalID=@ID";

        public const string CountHistoryTasks =
            @"select count(*) from Tasks with(nolock) where StateID!='6A52791D-7E42-42D6-A521-4252F276BB6C' and FromPersonalID=@ID";

        public const string CountPersonalRolesFromStatic =
            @"select count(*) from RoleUsers ru with(nolock) 
            inner join PersonalRoles pr with(nolock) on ru.PersonID=pr.ID 
            where RoleID=@ID and pr.WorkingTypeID='642faf68-37f3-4fd6-a97d-7abfe5a9a921'";

        public const string GetRoleFromDocType =
            @"select RoleTypeID from DocTypes  with(nolock) where ID=@ID";

        public const string FindRoleByLastAndFirstName =
            @"select ID from PersonalRoles with(nolock) where LastName + ' ' + FirstName=@text";

        public const string FindNamesForRoleDoc =
            @"select Name from Roles where ID=@RoleID
            union all
            select Caption from DocTypes where ID=@DocTypeID
            union all
            select Name from Roles where ID=@FromID";

        public const string FindNamesForRoles =
            @"select Name from Roles where ID=@RoleID
            union all
            select Name from Roles where ID=@FromID";

        public const string FindLastNumber =
            @"select top 1 Number from Tasks order by MainNumber desc";

        public const string InsertTask =
            @"insert into Tasks values (NEWID(), @StringNumber, @FromID, @FromName, @ToRoleID, @TORoleCaption, SYSDATETIME(), @DocTypeID, '6a52791d-7e42-42d6-a521-4252f276bb6c',@Commentary, NULL, NULL, NULL, 0)";

        public const string FindInfoAboutTask =
            @"select ToRoleName, Respond from Tasks with(nolock) where Number=@Number";

        public const string FindHistoryInfoAboutTask =
            @"select ID, ToRoleName, StateID, Respond from Tasks with(nolock) where Number=@Number";

        public const string FindHistoryInfoAboutTaskByTaskID =
            @"select ToRoleName, StateID, Respond, Number,FromPersonalID  from Tasks with(nolock) where ID=@TaskID";

        public const string FindFiles =
            @"select FileID, Name from Files where ID=@TaskID";

        public const string SelectTaskIDsFromQueue =
            @"select TaskID from CompleteQueue with(nolock)";

        public const string DeleTaskFromQueue =
            @"delete from CompleteQueue where TaskID=@ID";

        public const string FindChatID =
            @"select ChatID from BotStat with(nolock) where ID=@ID";
    }
}
