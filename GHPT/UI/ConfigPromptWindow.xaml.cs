using GHPT.IO;
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
            this.Config.Token = this.tokenBox.Text;
            this.Config.Model = this.modelBox.Text;
            this.Close();
        }
    }
}
