using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace lw15
{
    /// <summary>
    /// Логика взаимодействия для ActivationWindow.xaml
    /// </summary>
    public partial class ActivationWindow : Window
    {
        private const string LicenseCheckUrl = "https://localhost:7272/api/license/activate";

        private static readonly string ActivationFlagPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                         "MyApp", "activated.flag");

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

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
                var responseBody = await response.Content.ReadAsStringAsync();
                var activationResponse = JsonSerializer.Deserialize<ActivationApiResponse>(responseBody, JsonOptions);

                if (response.IsSuccessStatusCode && activationResponse?.Success == true)
                {
                    var activationInfo = new ActivationInfo
                    {
                        LicenseKey = activationResponse.LicenseKey ?? key,
                        ActivatedAt = activationResponse.ActivatedAt ?? DateTime.UtcNow
                    };

                    Directory.CreateDirectory(Path.GetDirectoryName(ActivationFlagPath)!);
                    File.WriteAllText(ActivationFlagPath, JsonSerializer.Serialize(activationInfo, JsonOptions));

                    MessageBox.Show(
                        $"Активация завершена успешно!\nКлюч: {activationInfo.LicenseKey}\nВремя: {activationInfo.ActivatedAt:yyyy-MM-dd HH:mm:ss} UTC",
                        "Активация",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    DialogResult = true;
                    Close();
                }
                else
                {
                    string error = activationResponse?.Message ?? responseBody;
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

    public class ActivationApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? LicenseKey { get; set; }
        public DateTime? ActivatedAt { get; set; }
    }
}
