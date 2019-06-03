using Avalonia;
using Avalonia.Controls;
using Avalonia.IconPacks.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using System.Xml;
using System.Xml.Linq;

namespace Avalonia.IconPacks.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif         
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
