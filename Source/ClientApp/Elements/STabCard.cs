using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        public bool ChangesState = false;
        public bool ChangesFile = false;
        public bool ChangesRespond = false;
        public State NewState;
        public List<Guid> DeletedFiles = new List<Guid>();
        public List<Guid> AddedToBaseFiles = new List<Guid>();
        public Dictionary<Guid, FileBase> NewFiles = new Dictionary<Guid, FileBase>();
        public Dictionary<Guid, FileControl> NewFileControls = new Dictionary<Guid, FileControl>();
        public string NewRespond;
        public void RespondChanged(string NewText)
        {
            ChangesRespond = true;
            NewRespond = NewText;
        }
        public void StateChanged(State state)
        {
            if (state.ID != Card.Task.StateID)
            {
                ChangesState = true;
                NewState = state;
                NewState.HasValue = true;
            }
        }
        public void FileDelete(Guid FileID)
        {
            ChangesFile = true;
            DeletedFiles.Add(FileID);
        }

        public void FileAdded(FileBase newFile)
        {
            ChangesFile = true;
            NewFiles.Add(newFile.FileID, newFile);
        }
        
        public void SaveUpdatedCard()
        {
            MessageBoxResult dialogResult = MessageBoxResult.No;
            if (ChangesFile || ChangesState || ChangesRespond)
            {
                dialogResult = MessageBox.Show((string)SystemSingleton.Configuration.mainWindow.FindResource("m_MakeSureSavingCard"),
                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AttentionHeader"),
                    MessageBoxButton.YesNo);
            }
            if (dialogResult == MessageBoxResult.Yes)
            {
                if (ChangesState)
                {
                    if (NewState.HasValue)
                    {
                        if (NewState?.ID != null) ChangeState();
                        if (Card.Task.StateID != NewState.ID.Value && NewState.ID.Value != new Guid("6a52791d-7e42-42d6-a521-4252f276bb6c"))
                        {
                            SetCompletitionToTask();
                            ComboBoxes[CardViewStruct.NewStateComboBox].IsEnabled = false;
                            TextBoxes[CardViewStruct.FourthLineTextBox].IsEnabled = false;
                            Buttons[CardViewStruct.FileButton].Visibility = Visibility.Collapsed;
                            foreach (var item in Card.FilesControls)
                            {
                                item.Value.Button.IsEnabled = false;
                            }
                            Buttons[CardViewStruct.ButtonsSaveButton].Visibility = Visibility.Collapsed;
                            EnvironmentHelper.UpdateView();
                        }
                    }
                }
                if (ChangesFile)
                {
                    foreach (var item in DeletedFiles)
                    {
                        if (Card.Files.FileDic.ContainsKey(item))
                        {
                            Card.Files.FileDic.Remove(item);
                            Card.FilesControls.Remove(item);
                            RemoveFileFromCard(item);
                        }
                        else if (NewFiles.ContainsKey(item))
                        {
                            NewFiles.Remove(item);
                            NewFileControls.Remove(item);
                            if (AddedToBaseFiles.Contains(item))
                            {
                                RemoveFileFromCard(item);
                                AddedToBaseFiles.Remove(item);
                            }
                        }
                        else
                        {
                            EnvironmentHelper.SendDialogBox(
                                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CantPermanentlyDeleteFiles") + "\n" + item.ToString(),
                                "Files Error"
                            );
                        }
                    }
                    DeletedFiles.Clear();
                    foreach (var item in NewFiles)
                    {
                        AddFileToDataBase(item.Value);
                        AddedToBaseFiles.Add(item.Key);
                    }
                }
                if (ChangesRespond)
                {
                    ChangeRespond();
                }

                ChangesRespond = false;
                ChangesFile = false;
                ChangesState = false;
            }
        }

        private void ChangeState()
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    using (var command = new SqlCommand(SqlCommands.SetNewStateCommand, con))
                    {
                        command.Parameters.Add("@StateID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@StateID"].Value = NewState.ID.Value;
                        command.Parameters.Add("@TaskID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@TaskID"].Value = Card.Task.ID.Value;
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
                                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CantSetStateToTask") + "\n" + Card.Task.ID.Value.ToString() + " <= " + NewState.ID.Value.ToString(),
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

        private void ChangeRespond()
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    using (var command = new SqlCommand(SqlCommands.SetNewRespondCommand, con))
                    {
                        command.Parameters.Add("@Respond", SqlDbType.NVarChar);
                        command.Parameters["@Respond"].Value = NewRespond;
                        command.Parameters.Add("@TaskID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@TaskID"].Value = Card.Task.ID.Value;
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
                                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CantSetRespondeToTask") + "\n" + Card.Task.ID.Value.ToString(),
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

        private void RemoveFileFromCard(Guid FileID)
        {
            try
            {
                try
                {
                    Directory.Delete(SystemSingleton.Configuration.FilesPath + FileID + "\\", true);
                }
                catch (Exception ex)
                {
                    EnvironmentHelper.SendDialogBox((string)SystemSingleton.Configuration.mainWindow.FindResource("m_DirectoryToDeleteNotFound"), "Directory Error");
                    return;
                }
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

        private void SetCompletitionToTask()
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    using (var command = new SqlCommand(SqlCommands.SetCompleteInfoCommand, con))
                    {
                        command.Parameters.Add("@UserID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@UserID"].Value = SystemSingleton.CurrentSession.ID;
                        command.Parameters.Add("@TaskID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@TaskID"].Value = Card.Task.ID.Value;
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
                                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CantCompleteTaskFound") + "\n" + Card.Task.ID.Value.ToString(),
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

        private void AddFileToDataBase(FileBase file)
        {
            //TODO: сделать подпись
            try
            {
                Directory.CreateDirectory(SystemSingleton.Configuration.FilesPath + file.FileID);
                File.Copy(file.Path, SystemSingleton.Configuration.FilesPath + file.FileID + "\\" + file.Name);
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    using (var command = new SqlCommand(SqlCommands.AddFileToDataBaseCommand, con))
                    {
                        command.Parameters.Add("@TaskID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@TaskID"].Value = Card.Task.ID.Value;
                        command.Parameters.Add("@FileID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@FileID"].Value = file.FileID;
                        command.Parameters.Add("@FileName", SqlDbType.NVarChar);
                        command.Parameters["@FileName"].Value = file.Name;
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
                                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CantSaveFile") + "\n" + file.Name,
                                "File Error"
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
    }
}
