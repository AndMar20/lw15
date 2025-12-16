using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace lw15
{
    /// <summary>
    /// Логика взаимодействия для ActivationWindow.xaml
    /// </summary>
    public partial class ActivationWindow : Window
    {
        private const string LicenseCheckUrl = "https://localhost:7272/api/license/activate";
        
        private static readonly string ActivationFlagPath =
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                         "MyApp", "activated.flag");

        public ActivationWindow()
        {
            InitializeComponent();
        }

        private async void BtnActivate_Click(object sender, RoutedEventArgs e)
        {
            string key = TxtLicenseKey.Text.Trim();
            if (string.IsNullOrEmpty(key))
            {
                MessageBox.Show("Пожалуйста, введите лицензионный ключ.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(new { licenseKey = key });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(LicenseCheckUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Сохраняем флаг активации
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ActivationFlagPath)!);
                    File.WriteAllText(ActivationFlagPath, DateTime.UtcNow.ToString("O"));
                    DialogResult = true;
                    Close();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Активация не удалась:\n{error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось подключиться к серверу:\n{ex.Message}", "Ошибка сети", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
