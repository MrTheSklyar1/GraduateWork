﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AdminApp.Elements;
using AdminApp.SupportClasses;

namespace AdminApp.SystemClasses
{
    public static class SystemSingleton
    {
        public static class CurrentSession
        {
            public static void CloseSession()
            {
                Login = "";
                ID = Guid.Empty;
                TelegramID = 0;
                FirstName = "";
                LastName = "";
                FullName = "";
                TabItems = new Dictionary<string, STabItem>();
                TabCards = new Dictionary<Guid, STabCard>();
            }
            public static string Login;
            public static Guid ID;
            public static long TelegramID;
            public static string FirstName;
            public static string LastName;
            public static string FullName;
            public static Dictionary<string, STabItem> TabItems = new Dictionary<string, STabItem>();
            public static Dictionary<Guid, STabCard> TabCards = new Dictionary<Guid, STabCard>();
            public static Guid ChosenIDForStaticRole = Guid.Empty;
            public static Window ViewChoose = null;
            public static Guid ChosenIDForDocType = Guid.Empty;
            public static bool SetCaptionToGrid(Window window, KeyValuePair<string, STabItem> item)
            {
                try
                {
                    if (item.Key == StaticTypes.StaticRoleTab || item.Key == StaticTypes.DocTypeTab || item.Key == "AllRolesTab")
                    {
                        item.Value.DataGrid.Columns[1].Header =
                            (String)window.FindResource("m_column_Caption");
                    }
                    else
                    {
                        item.Value.DataGrid.Columns[1].Header =
                            (String)window.FindResource("m_column_Login");
                        item.Value.DataGrid.Columns[2].Header =
                            (String)window.FindResource("m_column_TelegramID");
                        item.Value.DataGrid.Columns[3].Header =
                            (String)window.FindResource("m_column_FirstName");
                        item.Value.DataGrid.Columns[4].Header =
                            (String)window.FindResource("m_column_LastName");
                        item.Value.DataGrid.Columns[5].Header =
                            (String)window.FindResource("m_column_WorkingTypeID");
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static class Configuration
        {
            public static string ConnectionString { get; set; }
            public static bool SQLLog { get; set; }
            public static MainWindow mainWindow { get; set; }
            public static TabControl tabControl { get; set; }
            public static List<SqlConnection> SqlConnections { get; set; }
        }

        public static class BotomTab
        {
            public static string CurrentBottomBarLabelContent { get; set; }
            public static Brush CurrentBottomBarLabelBrush { get; set; }
        }
    }
}
