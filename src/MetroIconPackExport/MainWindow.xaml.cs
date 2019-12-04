using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

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
            exportIconPack<PackIconMaterialKind>("Material", PackIconMaterialDataFactory.Create());
            exportIconPack<PackIconMaterialDesignKind>("MaterialDesign", PackIconMaterialDesignDataFactory.Create(),true);
            exportIconPack<PackIconMaterialLightKind>("MaterialLight", PackIconMaterialLightDataFactory.Create());
            exportIconPack<PackIconFontAwesomeKind>("FontAwesome", PackIconFontAwesomeDataFactory.Create());
            exportIconPack<PackIconOcticonsKind>("Octicons", PackIconOcticonsDataFactory.Create());
            exportIconPack<PackIconModernKind>("Modern", PackIconModernDataFactory.Create());
            exportIconPack<PackIconEntypoKind>("Entypo+", PackIconEntypoDataFactory.Create());
            exportIconPack<PackIconSimpleIconsKind>("SimpleIcons", PackIconSimpleIconsDataFactory.Create());
            exportIconPack<PackIconWeatherIconsKind>("WeatherIcons", PackIconWeatherIconsDataFactory.Create());
            exportIconPack<PackIconTypiconsKind>("Typicons", PackIconTypiconsDataFactory.Create(),true);
            exportIconPack<PackIconFeatherIconsKind>("FeatherIcons", PackIconFeatherIconsDataFactory.Create());
            exportIconPack<PackIconIoniconsKind>("Ionicons", PackIconIoniconsDataFactory.Create());
            exportIconPack<PackIconJamIconsKind>("JamIcons", PackIconJamIconsDataFactory.Create(),true);
            exportIconPack<PackIconUniconsKind>("Unicons", PackIconUniconsDataFactory.Create());
            exportIconPack<PackIconZondiconsKind>("Zondicons", PackIconZondiconsDataFactory.Create());
            exportIconPack<PackIconEvaIconsKind>("EvaIcons", PackIconEvaIconsDataFactory.Create(),true);
            exportIconPack<PackIconBoxIconsKind>("BoxIcons", PackIconBoxIconsDataFactory.Create(),true);
            exportIconPack<PackIconPicolIconsKind>("PicolIcons", PackIconPicolIconsDataFactory.Create());
            exportIconPack<PackIconRPGAwesomeKind>("RPGAwesome", PackIconRPGAwesomeDataFactory.Create(),true);
            exportIconPack<PackIconMicronsKind>("Microns", PackIconMicronsDataFactory.Create());

            ExportFromSVG("VSCodeDark", "E:\\downloads\\vscode-icons-master\\vscode-icons-master\\icons\\dark");
            ExportFromSVG("VSCodeLight", "E:\\downloads\\vscode-icons-master\\vscode-icons-master\\icons\\light");
        }

        void exportIconPack<P>(string title, IDictionary<P, string> pack, bool invert = false) where P : Enum
        {
            //TODO use proper xml writer
            using (var file = File.CreateText($"..\\..\\..\\Avalonia.IconPacks\\Icons\\{title}.xaml"))
            {
                file.WriteLine("<Styles xmlns=\"https://github.com/avaloniaui\"");
                file.WriteLine("    xmlns:x = \"http://schemas.microsoft.com/winfx/2006/xaml\" >");
                file.WriteLine("    <Style>");
                file.WriteLine("        <Style.Resources>");

                var icons = Enum.GetValues(typeof(P));

                foreach (var icon in icons)
                {
                    var name = icon.ToString();
                    
                    var data = pack[(P)icon];
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
        private void ExportFromSVG(string title, string path)
        {
            using (var file = File.CreateText($"..\\..\\..\\Avalonia.IconPacks\\Icons\\{title}.xaml"))
            {
                file.WriteLine("<Styles xmlns=\"https://github.com/avaloniaui\"");
                file.WriteLine("    xmlns:x = \"http://schemas.microsoft.com/winfx/2006/xaml\" >");
                file.WriteLine("    <Style>");
                file.WriteLine("        <Style.Resources>");


                foreach (var svgpath in Directory.EnumerateFiles(path, "*.svg", SearchOption.AllDirectories))
                {
                    try
                    {
                        XmlDocument drawingDoc = new XmlDocument();
                        using (XmlTextReader tr = new XmlTextReader(svgpath))
                        {
                            tr.Namespaces = false;
                            drawingDoc.Load(tr);
                        }
                        var name = Path.GetFileNameWithoutExtension(svgpath);
                        var dgChildren = drawingDoc.DocumentElement.GetElementsByTagName("path");
                        if (dgChildren.Count == 1)
                        {
                            file.WriteLine($"            <GeometryDrawing x:Key=\"{title}.{name}\" Brush=\"{dgChildren[0].Attributes["fill"].Value}\" Geometry=\"{dgChildren[0].Attributes["d"].Value}\"/>");
                        }
                        else
                        {
                            file.WriteLine($"            <DrawingGroup x:Key=\"{title}.{name}\" >");
                            foreach (XmlElement dp in dgChildren)
                            {
                                file.WriteLine($"              <GeometryDrawing Brush=\"{dp.Attributes["fill"].Value}\" Geometry=\"{dp.Attributes["d"].Value}\"/>");
                            }
                            file.WriteLine($"            </DrawingGroup>");
                        }
                    }
                    catch (Exception e)
                    {

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
