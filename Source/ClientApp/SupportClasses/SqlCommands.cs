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
            inner join DocTypes d on t.DocType=d.ID where StateID='6a52791d-7e42-42d6-a521-4252f276bb6c'";

        public const string SetInfoToGridEndWorkCommand =
            @"select t.ID, t.Date, d.Caption, t.FromPersonalName, t.ToRoleName, ts.Caption, r.Name from Tasks t 
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
            @"select t.ID, t.Date, d.Caption, t.FromPersonalName from Tasks t
                inner join DocTypes d on t.DocType=d.ID 
                where StateID='6a52791d-7e42-42d6-a521-4252f276bb6c' and ToRoleID=@UserID";

        public const string SetInfoToGridOtherCommand =
            @"select t.ID, t.Date, d.Caption, t.FromPersonalName from Tasks t
                inner join DocTypes d on t.DocType=d.ID
                where StateID='6a52791d-7e42-42d6-a521-4252f276bb6c' and ToRoleID=@RoleID";

        public const string FindAllRolesCommand =
            @"select ru.RoleID, r.Name, r.Caption from RoleUsers ru join StaticRoles r on ru.RoleID=r.ID where ru.PersonID=@UserID";

        public const string LoginCommand =
            @"select ID, PassWord, isnull(TelegramID, 0), FirstName, LastName, FullName from PersonalRoles where Login=@LoginText";

        public const string LoadTaskCommand =
            @"select ID, Date";
    }
}
