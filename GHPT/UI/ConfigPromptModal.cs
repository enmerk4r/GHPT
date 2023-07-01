using Eto.Drawing;
using Eto.Forms;
using GHPT.Configs;
using GHPT.IO;
using Rhino.UI;
using System.ComponentModel;

namespace GHPT.UI
{
    class ConfigPromptModal : Dialog<DialogResult>
    {
        internal GPTConfig config;


        public ConfigPromptModal()
        {
            Padding = new Padding(5);
            Resizable = false;
            Result = DialogResult.Cancel;
            Title = "New Token Configuration";
            WindowStyle = WindowStyle.Default;
            Size = new Size(240, -1);

            config = new GPTConfig();

            var token_value = new TextBox { PlaceholderText = "OpenAI Token ..." };
            token_value.TextChanged += (sender, e) => config.Token = token_value.Text;

            var config_name = new TextBox { PlaceholderText = "Config Name" };
            config_name.TextChanged += (sender, e) => config.Name = config_name.Text;

            var model_version = new ComboBox { DataStore = Models.ModelOptions.Keys, SelectedIndex = 1 };
            model_version.SelectedValueChanged += (sender, e) =>
            {
                config.Model = model_version.SelectedValue.ToString();
                config.Version = Models.ModelOptions.First(kvp => kvp.Key == config.Model).Value;
            };

            config = new GPTConfig()
            {
                Model = model_version.Text,
                Name = config_name.Text,
                Version = Models.ModelOptions.First(kvp => kvp.Key == model_version.Text).Value
            };

            DefaultButton = new Button { Text = "Save" };
            DefaultButton.Click += (sender, e) => Close(DialogResult.Ok);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close(DialogResult.Cancel);

            var button_layout = new TableLayout
            {
                Padding = new Padding(5, 10, 5, 5),
                Spacing = new Size(5, 5),
                Rows = { new TableRow(config_name, model_version) }
            };

            var token_layout = new TableLayout
            {
                Padding = new Padding(5, 10, 5, 5),
                Spacing = new Size(5, 5),
                Rows = { new TableRow(token_value) }
            };

            var defaults_layout = new TableLayout
            {
                Padding = new Padding(5, 10, 5, 5),
                Spacing = new Size(5, 5),
                Rows = { new TableRow(DefaultButton, AbortButton) }
            };

            Content = new TableLayout
            {
                Width = -1,
                Padding = new Padding(5),
                Spacing = new Size(5, 5),
                Rows =
                {
                  new TableRow(button_layout),
                  new TableRow(token_layout),
                  new TableRow(defaults_layout)
                }
            };

            Topmost = true;
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            this.RestorePosition();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.SavePosition();
            base.OnClosing(e);
        }
    }
}