using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

namespace TimeDate_Application.Model
{
    public class Settings
    {
        private static Settings instance;

        public static Settings Instance
        {
            get { if (instance == null) { instance = new Settings(); } return instance; }
            set { instance = value; }
        }


        private bool format = false;

        public bool Format
        {
            get { return format; }
            set { format = value; }
        }

        private string date_color = Colors.Black.ToString();

        public string Date_color
        {
            get { return date_color; }
            set { date_color = value; }
        }

        private string time_color = Colors.Black.ToString();

        public string Time_color
        {
            get { return time_color; }
            set { time_color = value; }
        }

        private string bg_color = Colors.LightGreen.ToString();

        public string Bg_color
        {
            get { return bg_color; }
            set { bg_color = value; }
        }

        private bool dark_theme = false;

        public bool Dark_theme
        {
            get { return dark_theme; }
            set { dark_theme = value; }
        }

        public void Save(MainWindow window)
        {
            this.Date_color = (window.Time_Text.Foreground as SolidColorBrush).Color.ToString();
            this.Time_color = (window.Date_Text.Foreground as SolidColorBrush).Color.ToString();
            this.Bg_color = (window.TimeDate_DockPanel.Background as SolidColorBrush).Color.ToString();

            string settings_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Alarm Clock";
            if (!Directory.Exists(settings_path))
            {
                Directory.CreateDirectory(settings_path);
            }
            using (var writer = new StreamWriter((settings_path + "\\settings.xml")))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                serializer.Serialize(writer, (object)(this));
            }
        }

        public void Load(MainWindow window, EventArgs e)
        {
            if(this.Format == true)
            {
                window.Format_MouseDoubleClick(this, MouseButtonEventArgs.Empty as MouseButtonEventArgs);
            }
            if(this.Dark_theme == true)
            {
                window.DarkTheme_MouseDoubleClick(this, MouseButtonEventArgs.Empty as MouseButtonEventArgs);
            }

            window.Date_Text.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(this.Date_color);

            window.Time_Text.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(this.Time_color);
 
            window.TimeDate_DockPanel.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(this.Bg_color);
        }
        private Settings()
        {

        }
    }
}
