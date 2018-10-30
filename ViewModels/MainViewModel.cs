using Avalonia.Controls;
using Avalonia.Platform;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Xml;


namespace Avalonia.IconPacks.ViewModels
{
    public class MainViewModel:ReactiveObject
    {
        public List<IconVM> Icons{ get; set; } = new List<IconVM>();
        public List<IconVM> _FilteredIcons;
        public Window window { get; set; }
        private string _StyleSourceCode = "";
        private string _SearchText = "";
      
        public MainViewModel()
        {
            loadAllIcons();
            FilteredIcons = Icons;
        }
        void loadAllIcons()
        {
            loadIcons("resm:Avalonia.IconPacks.Icons.VSImageLib.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.Material.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.MaterialDesign.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.MaterialLight.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.FontAwesome.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.Octicons.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.Modern.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.Entypo+.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.SimpleIcons.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.WeatherIcons.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.Typicons.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.FeatherIcons.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.Ionicons.xaml?assembly=Avalonia.IconPacks");
      
        }
        void loadIcons(string resource)
        { 
            var assetLocator = AvaloniaLocator.Current.GetService<IAssetLoader>();
            using (var stream = assetLocator.Open(new System.Uri(resource)))
            {
                XmlDocument resDoc = new XmlDocument();
                using (XmlTextReader tr = new XmlTextReader(stream))
                {
                     tr.Namespaces = false;
                     resDoc.Load(tr);
                }
                foreach(XmlElement drawing in resDoc.SelectNodes("//Style.Resources/DrawingGroup"))
                {                   
                    Icons.Add(new IconVM() { parent = this, Name = drawing.Attributes["x:Key"].InnerText, SourceCode = drawing.OuterXml });
                }
                foreach (XmlElement drawing in resDoc.SelectNodes("//Style.Resources/GeometryDrawing"))
                {
                    Icons.Add(new IconVM() { parent = this, Name = drawing.Attributes["x:Key"].InnerText, SourceCode = drawing.OuterXml });
                }
            }
        }       
        public string StyleSourceCode
        {
            get
            {
                return _StyleSourceCode;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _StyleSourceCode, value);
                
            }
        }
        public List<IconVM> FilteredIcons
        {
            get
            {
                return _FilteredIcons;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _FilteredIcons, value);

            }
        }
        public string SearchText
        {
            get
            {
                return _SearchText;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _SearchText, value);

            }
        }
        public IconVM SelectedIcon
        {
            get
            {
                return null;
            }
            set
            {
                AddToStyle(value);
            }
        }
        public void AddToStyle(IconVM icon)
        {
             if (icon != null) StyleSourceCode += "\r\n" + icon.SourceCode;
        }
        public void Search()
        {
            if (SearchText == "")
            {
                FilteredIcons = Icons;
            }
            else
            {
                FilteredIcons = Icons.FindAll(x => x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }
        }
        
    }
    static class StringUtils
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
        }
    }
}
