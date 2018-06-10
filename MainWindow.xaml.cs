using Avalonia;
using Avalonia.Controls;
using Avalonia.IconPacks.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using System.Xml;
using System.Xml.Linq;

namespace Avalonia.IconPacks
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif         
            this.DataContextChanged += MainWindow_DataContextChanged;
        }

        private void MainWindow_DataContextChanged(object sender, System.EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.window = this;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
