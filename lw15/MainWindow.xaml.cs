using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace lw15
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string ActivationFlagPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                         "MyApp", "activated.flag");

        private ActivationInfo? _activationInfo;

        public MainWindow()
        {
            InitializeComponent();

            if (!IsActivated())
            {
                var activationWindow = new ActivationWindow();
                if (activationWindow.ShowDialog() == true)
                {
                    _activationInfo = LoadActivationInfo();
                }
                else
                {
                    Application.Current.Shutdown();
                    return;
                }
            }
            else
            {
                _activationInfo = LoadActivationInfo();
            }

            // Основной интерфейс приложения после активации
            Title = "✅ Приложение активировано!";

            var message = "Добро пожаловать! Приложение активировано.";
            if (_activationInfo != null)
            {
                message += $"\nКлюч: {_activationInfo.LicenseKey}";
                message += $"\nАктивировано: {_activationInfo.ActivatedAt:yyyy-MM-dd HH:mm:ss} UTC";
            }

            Content = new TextBlock
            {
                Text = message,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 18
            };
        }

        private bool IsActivated()
        {
            return File.Exists(ActivationFlagPath) && LoadActivationInfo() != null;
        }

        private ActivationInfo? LoadActivationInfo()
        {
            try
            {
                if (!File.Exists(ActivationFlagPath))
                {
                    return null;
                }

                var json = File.ReadAllText(ActivationFlagPath);
                return JsonSerializer.Deserialize<ActivationInfo>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return null;
            }
        }
    }
}
