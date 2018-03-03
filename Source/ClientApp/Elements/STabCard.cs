using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ClientApp.BaseClasses;
using ClientApp.SupportClasses;
using ClientApp.SystemClasses;

namespace ClientApp.Elements
{
    public class STabCard
    {
        //Информация из бд
        public Card Card;
        //Не изменяемые блоки текста
        public Dictionary<string, TextBlock> TextBlocks = new Dictionary<string, TextBlock>();
        //Текст боксы
        public Dictionary<string, TextBox> TextBoxes = new Dictionary<string, TextBox>();
        //Панели
        public Dictionary<string, DockPanel> DocPanels = new Dictionary<string, DockPanel>();
        //Границы
        public Dictionary<string, Border> Borders = new Dictionary<string, Border>();
        //Комбобоксы, правда он один
        public Dictionary<string, ComboBox> ComboBoxes = new Dictionary<string, ComboBox>();
        //Панели строк и другие
        public Dictionary<string, StackPanel> StackPanels = new Dictionary<string, StackPanel>();
        //Опять один
        public Dictionary<string, ListView> ListViews = new Dictionary<string, ListView>();
        //Кнопки
        public Dictionary<string, Button> Buttons = new Dictionary<string, Button>();
        //Вкладка для основной панели
        public TabItem TabItem; 
        public int Changes = 0;
        public List<Guid> DeletedFiles = new List<Guid>();
        public void FileDelete(Guid FileID)
        {
            Changes++;
            DeletedFiles.Add(FileID);
        }
        private void RemoveFileFromCard(Guid FileID)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    using (var command = new SqlCommand(SqlCommands.DeleteFileCommand, con))
                    {
                        command.Parameters.Add("@FileID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@FileID"].Value = FileID;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        int colms = command.ExecuteNonQuery();
                        con.Close();
                        if (colms != 0)
                        {
                            return;
                        }
                        else
                        {
                            EnvironmentHelper.SendDialogBox(
                                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_FileInBaseNotFound") + "\n" + FileID.ToString(),
                                "SQL Error"
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
            }
        }
        public void SaveUpdatedCard()
        {
            while (Changes > 0)
            {
                foreach (var item in DeletedFiles)
                {
                    Card.Files.FileDic.Remove(item);
                    Card.FilesControls.Remove(item);
                    RemoveFileFromCard(item);
                    Changes--;
                }

            }
        }
    }
}
