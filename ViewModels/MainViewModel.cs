using Avalonia.Controls;
using Avalonia.Platform;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Avalonia.IconPacks.ViewModels
{
    public class MainViewModel:ReactiveObject
    {
        public List<IconVM> Icons{ get; set; } = new List<IconVM>();
        public Window window { get; set; }
        private string _StyleSourceCode = "";
       
        public MainViewModel()
        {
            loadAllIcons();
        }
        void loadAllIcons()
        {
            loadIcons("resm:Avalonia.IconPacks.Icons.VSImageLib.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.Material.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.MaterialLight.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.FontAwesome.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.Octicons.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.Modern.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.Entypo+.xaml?assembly=Avalonia.IconPacks");
            loadIcons("resm:Avalonia.IconPacks.Icons.SimpleIcons.xaml?assembly=Avalonia.IconPacks");

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
            if(icon!=null) StyleSourceCode += "\r\n" + icon.SourceCode;
        }
    }
}
