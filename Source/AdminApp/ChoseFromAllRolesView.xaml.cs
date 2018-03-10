﻿using AdminApp.Elements;
using AdminApp.SupportClasses;
using AdminApp.SystemClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdminApp
{
    /// <summary>
    /// Interaction logic for ChoseFromAllRolesView.xaml
    /// </summary>
    public partial class ChoseFromAllRolesView : Window
    {
        public ChoseFromAllRolesView()
        {
            InitializeComponent();
            var tempItem = new STabItem();
            tempItem.TabItem = new TabItem
            {
                Header = (String)this.FindResource("m_tab_Role"),
                Name = "AllRolesTab",
                Height = 40,
                FontSize = 14
            };
            tempItem.DataGrid = new DataGrid
            {
                Name = "AllRolesGrid",
                SelectionMode = DataGridSelectionMode.Single,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true
            };
            SetInfoToGridlRoles(ref tempItem.DataGrid);
            tempItem.TabItem.Content = tempItem.DataGrid;
            tabControl.Items.Add(tempItem.TabItem);
            Style rowStyle = new Style(typeof(DataGridRow));
            rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent,
                new MouseButtonEventHandler(Handlers.RowRolesRoleView_DoubleClick)));
            tempItem.DataGrid.RowStyle = rowStyle;
            tempItem.DataGrid.AutoGeneratedColumns += (sender, args) =>
            {
                tempItem.DataGrid.Columns[0].Visibility = Visibility.Collapsed;
                SystemSingleton.CurrentSession.SetCaptionToGrid(this, new KeyValuePair<string, STabItem>("AllRolesTab", tempItem));
            };
        }
        public static void SetInfoToGridlRoles(ref DataGrid dataGrid)
        {
            try
            {
                SqlConnection con = new SqlConnection(SystemSingleton.Configuration.ConnectionString);
                SqlCommand cmd = new SqlCommand(SqlCommands.SetInfoToGridAllRoles, con);
                EnvironmentHelper.SendLogSQL(cmd.CommandText);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Roles");
                sda.Fill(dt);
                dataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
            }
        }
    }
}
