using GHPT.Configs;
using GHPT.IO;
using System.Windows;

namespace GHPT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ConfigPromptWindow : Window
    {
        public GPTConfig Config { get; private set; }
        public ConfigPromptWindow()
        {
            InitializeComponent();
            this.Config = new GPTConfig();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Config = new("NAME", GPTVersion.GPT3_5, this.tokenBox.Text, this.modelBox.Text);
            this.Close();
        }
    }
}
