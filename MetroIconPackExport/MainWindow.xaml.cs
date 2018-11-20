using MahApps.Metro.IconPacks;
using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

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
            exportIconPack("MaterialDesign", new PackIconMaterialDesign(),true);
            exportIconPack("MaterialLight", new PackIconMaterialLight());
            exportIconPack("FontAwesome", new PackIconFontAwesome());
            exportIconPack("Octicons", new PackIconOcticons());
            exportIconPack("Modern", new PackIconModern());
            exportIconPack("Entypo+", new PackIconEntypo());
            exportIconPack("SimpleIcons", new PackIconSimpleIcons());
            exportIconPack("WeatherIcons", new PackIconWeatherIcons());
            exportIconPack("Typicons", new PackIconTypicons(),true);
            exportIconPack("FeatherIcons", new PackIconFeatherIcons());
            exportIconPack("Ionicons", new PackIconIonicons());
        }

        void exportIconPack<P>(string title, PackIconControl<P> pack, bool invert=false) where P:Enum
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
                    if (invert)
                    {
                        data = invertPath(data);
                    }
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
        string invertPath(string path)
        {
            // from https://stackoverflow.com/questions/249971/wpf-how-to-apply-a-generaltransform-to-a-geometry-data-and-return-the-new-geome
            var g=Geometry.Parse(path);
            PathGeometry geometryTransformed = Geometry.Combine(Geometry.Empty, g, GeometryCombineMode.Union, new ScaleTransform(1, -1));
            return geometryTransformed.ToString();
            
        }
    }
}
