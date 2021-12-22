using MahApps.Metro.IconPacks;
using MetroIconPackExport.utils;
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
            ExportIcons();
        }
        void ExportIcons()
        {
            ExportIconPack<PackIconMaterialKind>("Material", PackIconMaterialDataFactory.Create());
            ExportIconPack<PackIconMaterialDesignKind>("MaterialDesign", PackIconMaterialDesignDataFactory.Create(),true);
            ExportIconPack<PackIconMaterialLightKind>("MaterialLight", PackIconMaterialLightDataFactory.Create());
            ExportIconPack<PackIconFontAwesomeKind>("FontAwesome", PackIconFontAwesomeDataFactory.Create());
            ExportIconPack<PackIconOcticonsKind>("Octicons", PackIconOcticonsDataFactory.Create());
            ExportIconPack<PackIconModernKind>("Modern", PackIconModernDataFactory.Create());
            ExportIconPack<PackIconEntypoKind>("Entypo+", PackIconEntypoDataFactory.Create());
            ExportIconPack<PackIconSimpleIconsKind>("SimpleIcons", PackIconSimpleIconsDataFactory.Create());
            ExportIconPack<PackIconWeatherIconsKind>("WeatherIcons", PackIconWeatherIconsDataFactory.Create());
            ExportIconPack<PackIconTypiconsKind>("Typicons", PackIconTypiconsDataFactory.Create(),true);
            ExportIconPack<PackIconFeatherIconsKind>("FeatherIcons", PackIconFeatherIconsDataFactory.Create());
            ExportIconPack<PackIconIoniconsKind>("Ionicons", PackIconIoniconsDataFactory.Create());
            ExportIconPack<PackIconJamIconsKind>("JamIcons", PackIconJamIconsDataFactory.Create(),true);
            ExportIconPack<PackIconUniconsKind>("Unicons", PackIconUniconsDataFactory.Create());
            ExportIconPack<PackIconZondiconsKind>("Zondicons", PackIconZondiconsDataFactory.Create());
            ExportIconPack<PackIconEvaIconsKind>("EvaIcons", PackIconEvaIconsDataFactory.Create(),true);
            ExportIconPack<PackIconBoxIconsKind>("BoxIcons", PackIconBoxIconsDataFactory.Create(),true);
            ExportIconPack<PackIconPicolIconsKind>("PicolIcons", PackIconPicolIconsDataFactory.Create());
            ExportIconPack<PackIconRPGAwesomeKind>("RPGAwesome", PackIconRPGAwesomeDataFactory.Create(),true);
            ExportIconPack<PackIconMicronsKind>("Microns", PackIconMicronsDataFactory.Create());

            ExportIconPack<PackIconBootstrapIconsKind>("Bootstrap", PackIconBootstrapIconsDataFactory.Create(),true);
            ExportIconPack<PackIconCodiconsKind>("Codeicons", PackIconCodiconsDataFactory.Create(), true);
            ExportIconPack<PackIconFileIconsKind>("FileIcons", PackIconFileIconsDataFactory.Create(), true);
            ExportIconPack<PackIconFontaudioKind>("Fontaudio", PackIconFontaudioDataFactory.Create(), true);
            ExportIconPack<PackIconForkAwesomeKind>("ForkAwesome", PackIconForkAwesomeDataFactory.Create(), true);
            ExportIconPack<PackIconPixelartIconsKind>("Pixelart", PackIconPixelartIconsDataFactory.Create());
            ExportIconPack<PackIconRadixIconsKind>("Radix", PackIconRadixIconsDataFactory.Create(), true);
            ExportIconPack<PackIconRemixIconKind>("Remix", PackIconRemixIconDataFactory.Create(), true);
            ExportIconPack<PackIconVaadinIconsKind>("Vaadin", PackIconVaadinIconsDataFactory.Create(), true);

            ExportIconPack<PackIconCooliconsKind>("Cool", PackIconCooliconsDataFactory.Create(), true);
            ExportIconPack<PackIconFontistoKind>("Fontisto", PackIconFontistoDataFactory.Create(), true);


            ExportFromSVG("VSCodeDark", "E:\\downloads\\vscode-icons-master\\vscode-icons-master\\icons\\dark");
            ExportFromSVG("VSCodeLight", "E:\\downloads\\vscode-icons-master\\vscode-icons-master\\icons\\light");
        }

        void ExportIconPack<P>(string title, IDictionary<P, string> pack, bool invert = false) where P : Enum
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
                    if (data == null || data.Length < 4) continue;
                  
                    data = TidyPath(data,invert,16.0);
                  
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
                    catch (Exception)
                    {

                    }
                }
                file.WriteLine("        </Style.Resources>");
                file.WriteLine("    </Style>");
                file.WriteLine("</Styles>");
            }
 
        }
        string TidyPath(string path, bool invert, double size)
        {
            var g=Geometry.Parse(path);
            var bounds = g.Bounds;// GetRenderBounds(new Pen(Brushes.Black, 0));
            var scale=size/Math.Max(bounds.Width, bounds.Height);

            return PathUtils.Transform(path, -bounds.Left, invert? -bounds.Bottom : -bounds.Top, scale, invert ? -scale : scale);

        }
    }
   
}
