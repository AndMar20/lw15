using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lw15
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string ActivationFlagPath =
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                 "MyApp", "activated.flag");

        public MainWindow()
        {
            InitializeComponent();

            if (!IsActivated())
            {
                var activationWindow = new ActivationWindow();
                if (activationWindow.ShowDialog() == true)
                {
                    // Активация успешна — продолжаем
                }
                else
                {
                    Application.Current.Shutdown();
                    return;
                }
            }

            // Здесь — основной интерфейс вашего приложения
            Title = "✅ Приложение активировано!";
            Content = new TextBlock
            {
                Text = "Добро пожаловать! Приложение активировано.",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 18
            };
        }

        private bool IsActivated()
        {
            return File.Exists(ActivationFlagPath);
        }
    }
}