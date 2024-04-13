using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using TimeDate_Application.Model;
using TimeDate_Application.ViewModel;

namespace TimeDate_Application
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;
            this.Loaded += MainWindow_Initialized;
            this.DataContext = new TimeDate_ViewModel();
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string save_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Alarm Clock";

            if (Settings.Instance.Format)
            {
                (this.DataContext as TimeDate_ViewModel).Current = new TimeDate(DateTime.Now.ToLongTimeString(), DateTime.Now.Date.ToString("dd MMMM yyyy"));
            }
            else
            {
                (this.DataContext as TimeDate_ViewModel).Current = new TimeDate();
            }

            if(File.Exists(save_path + "\\save.xml"))
            {
                string time;
                using (var reader = new StreamReader((save_path + "\\save.xml")))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(string));
                    time = (serializer.Deserialize(reader) as string);
                }

                if (this.Time_Text.Text == time)
                {
                    string audio_path = Environment.CurrentDirectory + "\\SoundFiles\\ring.wav";
                    SoundPlayer player = new SoundPlayer(audio_path);
                    player.PlayLooping();
                    if(System.Windows.MessageBox.Show(time.ToUpper() + "!", "Alarm Clock", MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        player.Stop();
                        File.Delete(save_path + "\\save.xml");
                    }
                }
            }
        }

        public void Format_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (int.TryParse(this.Date_Text.Text.Replace(".", ""), out int res))
            {
                Settings.Instance.Format = true;
            }
            else
            {
                Settings.Instance.Format = false;
            }
        }

        public void DateColor_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.ShowDialog();
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Color.FromArgb(dialog.Color.A,dialog.Color.R,dialog.Color.G,dialog.Color.B);
            Settings.Instance.Date_color = brush.Color.ToString();
            this.Date_Text.Foreground = brush;
        }

        public void TimeColor_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.ShowDialog();
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B);
            Settings.Instance.Time_color = brush.Color.ToString();
            this.Time_Text.Foreground = brush;
        }

        public void Background_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.ShowDialog();
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B);
            Settings.Instance.Bg_color = brush.Color.ToString();
            this.TimeDate_DockPanel.Background = brush;
        }

        public void Autorun_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",true);
            key.SetValue("Alarm Clock", AppDomain.CurrentDomain.BaseDirectory + "TimeDate.exe");
        }

        public void DarkTheme_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(App.Current.Resources.MergedDictionaries[0].Source != new Uri("darktheme.xaml", UriKind.Relative))
            {
                Settings.Instance.Dark_theme = true;
                this.Theme.Content = "Light Theme";
                App.Current.Resources.MergedDictionaries[0].Source = new Uri("darktheme.xaml", UriKind.Relative);
            }
            else
            {
                Settings.Instance.Dark_theme = false;
                this.Theme.Content = "Dark Theme";
                App.Current.Resources.MergedDictionaries[0].Source = new Uri("lighttheme.xaml", UriKind.Relative);
            }
        }

        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            string settings_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Alarm Clock";
            if (!Directory.Exists(settings_path))
            {
                Directory.CreateDirectory(settings_path);
            }
            else if (File.Exists(settings_path + "\\settings.xml"))
            {
                using (var reader = new StreamReader((settings_path + "\\settings.xml")))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    Settings.Instance = (serializer.Deserialize(reader) as Settings);
                }
                Settings.Instance.Load(this, e);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Instance.Save(this);
        }

        private void Set_Alarm_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Form form = new Form();
            form.ShowIcon = false;
            form.ShowInTaskbar = false;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.Size = new System.Drawing.Size(250,100);

            DateTimePicker picker = new DateTimePicker();
            picker.Size = new System.Drawing.Size(235, 50);
            picker.Format = DateTimePickerFormat.Time;
            picker.ShowUpDown = true;

            System.Windows.Forms.Button confirm_Button = new System.Windows.Forms.Button();
            confirm_Button.Text = "Confirm";
            confirm_Button.Font = new System.Drawing.Font("Segoe", 12);
            confirm_Button.Location = new System.Drawing.Point(0, picker.Height);
            confirm_Button.Size = new System.Drawing.Size(235, 50);
            confirm_Button.Click += Confirm_Button_Click;


            form.Controls.Add(picker);
            form.Controls.Add(confirm_Button);
            form.ShowDialog();

        }

        private void Confirm_Button_Click(object sender, EventArgs e)
        {
            Form main = ((sender as System.Windows.Forms.Button).Parent as Form);

            string save_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Alarm Clock";
            if (!Directory.Exists(save_path))
            {
                Directory.CreateDirectory(save_path);
            }
            using (var writer = new StreamWriter((save_path + "\\save.xml")))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(string));
                serializer.Serialize(writer, (object)((main.Controls[0] as DateTimePicker).Value.ToLongTimeString()));
            }
            main.Close();
        }

        private void Restore_Settings_DoubleClick(object sender, RoutedEventArgs e)
        {
            string settings_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Alarm Clock" + "\\settings.xml";
            if (File.Exists(settings_path))
            {
                this.Close();
                File.Delete(settings_path);
            }
        }

        private void Format_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void Set_Alarm_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
