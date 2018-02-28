using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.SupportClasses
{
    public static class SqlCommands
    {
        public const string SetInfoToGridWorkCommand =
            @"select t.ID, t.Date, d.Caption, t.FromPersonalName, t.ToRoleName from Tasks t 
                inner join (
                    select RoleID as ID from RoleUsers ru where ru.PersonID=@UserID
                    union
                    select ID as RoleID from PersonalRoles where ID=@UserID
                ) temp on t.ToRoleID=temp.ID
            inner join DocTypes d on t.DocType=d.ID where isCompleted=0";

        public const string SetInfoToGridEndWorkCommand =
            @"select t.ID, t.Date, d.Caption, t.FromPersonalName, t.ToRoleName from Tasks t 
                inner join (
                    select RoleID as ID from RoleUsers ru where ru.PersonID=@UserID
                    union
                    select ID as RoleID from PersonalRoles where ID=@UserID
                ) temp on t.ToRoleID=temp.ID
            inner join DocTypes d on t.DocType=d.ID where isCompleted=1";

        public const string SetInfoToGridPersonalCommand =
            @"select t.ID, t.Date, d.Caption, t.FromPersonalName from Tasks t
                inner join DocTypes d on t.DocType=d.ID where isCompleted=0 and ToRoleID=@UserID";

        public const string SetInfoToGridOtherCommand =
            @"select t.ID, t.Date, d.Caption, t.FromPersonalName from Tasks t
                inner join DocTypes d on t.DocType=d.ID where isCompleted=0 and ToRoleID=@RoleID";

        public const string FindAllRolesCommand =
            @"select ru.RoleID, r.Name, r.Caption from RoleUsers ru join StaticRoles r on ru.RoleID=r.ID where ru.PersonID=@UserID";

        public const string LoginCommand =
            @"select ID, PassWord, isnull(TelegramID, 0), FirstName, LastName, FullName from PersonalRoles where Login=@LoginText";

    }
}
