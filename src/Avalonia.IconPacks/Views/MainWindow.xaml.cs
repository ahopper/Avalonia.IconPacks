using Avalonia;
using Avalonia.Controls;
using Avalonia.IconPacks.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using System;
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

        private void IconTapped(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (e.Source is Control source)
            {                
                if (source.DataContext is IconVM icon)
                {
                    (DataContext as MainViewModel)?.AddToStyle(icon);
                }
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
