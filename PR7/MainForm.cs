using PR5;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PR7
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void FileForEncryptionPathButtonOnClick(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                folderForEncryptionPathTextBox.Text = folderDialog.SelectedPath;
            }
        }

        private void EncryptedFilePathButtonOnClick(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                encryptedFolderPathTextBox.Text = folderDialog.SelectedPath;
            }
        }

        private void FileForDecryptionPathButtonOnClick(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                folderForDecryptionPathTextBox.Text = folderDialog.SelectedPath;
            }
        }

        private void DecryptedFilePathButtonOnClick(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                decryptedFolderPathTextBox.Text = folderDialog.SelectedPath;
            }
        }

        private void EncryptFileButtonOnClick(object sender, EventArgs e)
        {
            // Проверка на корректность выбранного файла
            if (!Directory.Exists(folderForEncryptionPathTextBox.Text))
            {
                PrintError("Некорретко указана папка для шифрования!" +
                                  "\n(Выбранная папка не существует)");
                return;
            }

            // Проверка, что пользователь ввёл хоть что-то в названии сохраняемого файла
            if (string.IsNullOrEmpty(encryptedFolderPathTextBox.Text))
            {
                PrintError("Некорретко указан зашифрованный файл (результат шифрования)!");
                return;
            }

            // Создаем папку для сохранения зашифр. файлов, если её нет
            if (!Directory.Exists(encryptedFolderPathTextBox.Text))
            {
                Directory.CreateDirectory(encryptedFolderPathTextBox.Text);
            }

            List<byte[]> bytesOfFiles = new List<byte[]>();
            List<string> filesOfFolder = new List<string>();

            // Записываем имена файлов в папке в список
            filesOfFolder.AddRange(Directory.GetFiles(folderForEncryptionPathTextBox.Text));

            foreach (string fileName in filesOfFolder)
            {
                // Получаем байты файла с размером блока в 64 и записываем результат в список
                bytesOfFiles.Add(RSA.CovertBytesToRSA(File.ReadAllBytes(fileName)));
            }

            // Получаем простые числа в диапозоне от 2 до 100 (Получаем произведение и функцию Эйлера для чисел)
            RSA.GetPartOfKeys(2, 100);
            // Высчитываем открытый ключ e
            RSA.Calculate_e();
            // Высчитываем закрытый ключ d
            RSA.Calculate_d();

            for (int i = 0; i < bytesOfFiles.Count; i++)
            {
                // Производим шифрование файлов в новую папку
                RSA.EncryptFilesInDirectory(bytesOfFiles[i], 
                                            encryptedFolderPathTextBox.Text + 
                                            $"{filesOfFolder[i].Substring(filesOfFolder[i].LastIndexOf("\\"))}");
            }

            MessageBox.Show("Шифрование успешно завершено!",
                            "Внимание!",
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Information);
        }

        private void DecryptFileButtonOnClick(object sender, EventArgs e)
        {
            // Проверка на корректность выбранного файла
            if (!Directory.Exists(folderForDecryptionPathTextBox.Text))
            {
                PrintError("Некорретко указана папка для шифрования!" +
                                  "\n(Выбранная папка не существует)");
                return;
            }

            // Проверка, что пользователь ввёл хоть что-то в названии сохраняемого файла
            if (string.IsNullOrEmpty(decryptedFolderPathTextBox.Text))
            {
                PrintError("Некорретко указан зашифрованный файл (результат шифрования)!");
                return;
            }

            // Создаем папку для сохранения зашифр. файлов, если её нет
            if (!Directory.Exists(decryptedFolderPathTextBox.Text))
            {
                Directory.CreateDirectory(decryptedFolderPathTextBox.Text);
            }

            List<byte[]> bytesOfFiles = new List<byte[]>();
            List<string> filesOfFolder = new List<string>();

            // Записываем имена файлов в папке в список
            filesOfFolder.AddRange(Directory.GetFiles(folderForDecryptionPathTextBox.Text));

            foreach (string fileName in filesOfFolder)
            {
                // Получаем байты файла с размером блока в 64 и записываем результат в список
                bytesOfFiles.Add(File.ReadAllBytes(fileName));
            }

            for (int i = 0; i < bytesOfFiles.Count; i++)
            {
                // Производим шифрование файлов в новую папку
                RSA.DecryptFilesFromDirectory(bytesOfFiles[i],
                                            decryptedFolderPathTextBox.Text +
                                            $"{filesOfFolder[i].Substring(filesOfFolder[i].LastIndexOf("\\"))}");
            }

            MessageBox.Show("Расшифрование успешно завершено!",
                            "Внимание!",
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Information);
        }

        private void PrintError(string errorMessage)
        {
            MessageBox.Show(
                errorMessage,
                "Внимание!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

    }
}
