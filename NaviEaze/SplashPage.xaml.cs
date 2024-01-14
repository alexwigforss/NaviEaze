using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NaviEaze
{
    public partial class SplashPage : Page
    {
        public SplashPage()
        {
            InitializeComponent();
        }

        // Change Language Button
        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush brush = new(Colors.Blue);
            if (sender is Ellipse ellipse )
            {
                ellipse.Fill = brush;
            }
        }

        private void SplashButtonPressed(object sender, MouseButtonEventArgs e)
        {
            int nrUid = int.Parse(((FrameworkElement)sender).Uid);
            if (languageImage.Source.ToString().Contains("english.png"))
            {
                MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;
                int nHieght = (int)SystemParameters.PrimaryScreenHeight;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Content = new MainPage(nrUid);
                    mainWindow.Height = nHieght;
                    mainWindow.Top = 0;
                    mainWindow.MainFrame.Visibility = Visibility.Visible;
                }
                Visibility = Visibility.Collapsed;
            }
            else if (languageImage.Source.ToString().Contains("swedish.png"))
            {
                MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;
                int nHieght = (int)SystemParameters.PrimaryScreenHeight;
                if (mainWindow != null)
                {
                    mainWindow.MainFrameSv.Content = new MainPageSv(nrUid);
                    mainWindow.Height = nHieght;
                    mainWindow.Top = 0;
                    mainWindow.MainFrameSv.Visibility = Visibility.Visible;
                }
                Visibility = Visibility.Collapsed;
            }

        }

        private void LanguageSettingToggle(object sender, MouseButtonEventArgs e)
        {
            if (languageImage.Source.ToString().Contains("english.png"))
            {
                languageImage.Source = new BitmapImage(new Uri("/Assets/swedish.png", UriKind.Relative));
            }
            else if (languageImage.Source.ToString().Contains("swedish.png"))
            {
                languageImage.Source = new BitmapImage(new Uri("/Assets/english.png", UriKind.Relative));
            }
        }
    }
}
