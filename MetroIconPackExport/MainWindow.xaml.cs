using MahApps.Metro.IconPacks;
using System;
using System.IO;
using System.Windows;

namespace MetroIconPackExport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            exportIcons();
        }
        void exportIcons()
        {
            exportIconPack("Material", new PackIconMaterial());
            exportIconPack("MaterialDesign", new PackIconMaterialDesign());
            exportIconPack("MaterialLight", new PackIconMaterialLight());
            exportIconPack("FontAwesome", new PackIconFontAwesome());
            exportIconPack("Octicons", new PackIconOcticons());
            exportIconPack("Modern", new PackIconModern());
            exportIconPack("Entypo+", new PackIconEntypo());
            exportIconPack("SimpleIcons", new PackIconSimpleIcons());
            exportIconPack("WeatherIcons", new PackIconWeatherIcons());
            exportIconPack("Typicons", new PackIconTypicons());
            exportIconPack("FeatherIcons", new PackIconFeatherIcons());
            exportIconPack("Ionicons", new PackIconIonicons());
        }

        void exportIconPack<P>(string title, PackIconControl<P> pack) where P:Enum
        {
            //TODO use proper xml writer
            using (var file = File.CreateText($"..\\..\\..\\Icons\\{title}.xaml"))
            {
                file.WriteLine("<Styles xmlns=\"https://github.com/avaloniaui\"");
                file.WriteLine("    xmlns:x = \"http://schemas.microsoft.com/winfx/2006/xaml\" >");
                file.WriteLine("    <Style>");
                file.WriteLine("        <Style.Resources>");

                var icons = Enum.GetValues(typeof(P));

                foreach (var icon in icons)
                {
                    var name = icon.ToString();
                    pack.Kind = (P)icon;
                    var data = pack.Data;
                    if (!String.IsNullOrEmpty(data))
                    {
                        file.WriteLine($"            <GeometryDrawing x:Key=\"{title}.{name}\" Brush=\"#FF000000\" Geometry=\"{data}\"/>");
                    }
                }
                file.WriteLine("        </Style.Resources>");
                file.WriteLine("    </Style>");
                file.WriteLine("</Styles>");
            }
        }
    }
}
