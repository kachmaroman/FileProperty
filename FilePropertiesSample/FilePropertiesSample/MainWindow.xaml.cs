using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileProperties_Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentFolderPath;
        private DirectoryInfo directoryInfo;

        public MainWindow()
        {
            InitializeComponent();
            DisableMoveFeatures();
        }

        private void DisplayFolderList(string path)
        {
            ClearAllFields();

            directoryInfo = new DirectoryInfo(path);

            currentFolderPath = directoryInfo.FullName;
            textBoxFolder.Text = directoryInfo.FullName;
            DisableMoveFeatures();

            foreach (var nextFoler in directoryInfo.GetDirectories())
                listBoxFolders.Items.Add(nextFoler.Name);

            foreach (var nextFile in directoryInfo.GetFiles())
                listBoxFiles.Items.Add(nextFile.Name);
        }

        private void DisableMoveFeatures()
        {
            textBoxNewLocation.Clear();
            textBoxNewLocation.IsEnabled = false;
            buttonCopyTo.IsEnabled = false;
            buttonDelete.IsEnabled = false;
            buttonMoveTo.IsEnabled = false;
        }
        private void ClearAllFields()
        {
            listBoxFiles.Items.Clear();
            listBoxFolders.Items.Clear();
            textBoxFolder.Clear();
            textBoxFileName.Clear();
            textBoxCreationTime.Clear();
            textBoxLasAccessTime.Clear();
            textBoxLasAccessTime.Clear();
            textBoxFileSize.Clear();
            textBoxLastWriteTime.Clear();
            textBoxNewLocation.Clear();
        }

        private void DisplayFileInfo(string pathFile)
        {
            FileInfo fileInfo = new FileInfo(pathFile);

            if (fileInfo.Exists)
            {
                textBoxFileName.Text = fileInfo.Name;
                textBoxFileSize.Text = fileInfo.Length.ToString("N") + " bytes";
                textBoxLastWriteTime.Text = fileInfo.LastWriteTime.ToLongDateString();
                textBoxCreationTime.Text = fileInfo.CreationTime.ToLongTimeString();
                textBoxLasAccessTime.Text = fileInfo.LastAccessTime.ToLongDateString();

                textBoxNewLocation.Text = fileInfo.FullName;
                textBoxNewLocation.IsEnabled = true;
                buttonCopyTo.IsEnabled = true;
                buttonDelete.IsEnabled = true;
                buttonMoveTo.IsEnabled = true;
            }
        }

        private void buttonDisplay_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = textBoxInput.Text;

            try
            {
                DirectoryInfo directory = new DirectoryInfo(folderPath);

                if (directory.Exists)
                {
                    DisplayFolderList(directory.FullName);
                    return;
                }

                FileInfo file = new FileInfo(folderPath);

                if (file.Exists)
                {
                    DisplayFolderList(file.Directory.FullName);
                    int index = listBoxFiles.Items.IndexOf(file.Name);
                    listBoxFiles.SelectedIndex = index;
                }
                else
                    throw new FileNotFoundException($"{file.Name}");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"File not found: {ex.Message}", "Error", MessageBoxButton.OKCancel,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OKCancel,
                    MessageBoxImage.Error);
            }
        }

        private void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string folderPath = new FileInfo(currentFolderPath).DirectoryName;
                ClearAllFields();
                DisplayFolderList(folderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void listBoxFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string selectedString = $"{listBoxFiles.SelectedItem ?? ""}";
                string fullFileName = System.IO.Path.Combine(currentFolderPath, selectedString);
                DisplayFileInfo(fullFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void listBoxFolders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string selectedString = listBoxFolders.SelectedItem.ToString();
                string fullPathName = System.IO.Path.Combine(currentFolderPath, selectedString);
                DisplayFolderList(fullPathName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = System.IO.Path.Combine(currentFolderPath, textBoxFileName.Text);
                string query = $"Really delete the file {filePath}?";

                if (MessageBox.Show(query, "Delete File?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    File.Delete(filePath);
                    ClearAllFields();
                    DisplayFolderList(currentFolderPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unabble to delete file. The following exception occurred: {ex.Message}", "Failed");
            }
        }

        private void buttonCopyTo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = System.IO.Path.Combine(currentFolderPath, textBoxFileName.Text);

                string query =  $"Really copy the file {filePath} to {textBoxNewLocation.Text}?";

                if (MessageBox.Show(query, "Copy File?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    File.Copy(filePath, $"{textBoxNewLocation.Text}");
                    DisplayFolderList(currentFolderPath);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to copy file. The following exception occurred {ex.Message}", "Failed");
            }
        }

        private void buttonMoveTo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = System.IO.Path.Combine(currentFolderPath, textBoxFileName.Text);

                string query = $"Really move the file {filePath} to {textBoxNewLocation.Text}?";

                if (MessageBox.Show(query, "Move File?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    File.Move(filePath, $"{textBoxNewLocation.Text}");
                    DisplayFolderList(currentFolderPath);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to move file. The following exception occurred {ex.Message}", "Failed");
            }
        }
    }
}