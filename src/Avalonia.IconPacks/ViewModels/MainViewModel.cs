using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Avalonia.IconPacks.ViewModels
{
    public class MainViewModel:ViewModelBase
    {
        private List<IconVM> Icons{ get; set; } = new List<IconVM>();
        private List<IconVM> _FilteredIcons;
        private string _StyleSourceCode = "";
        private string _SearchText = "";
        public ObservableCollection<IconVM> SelectedIcons { get; set; } = new ObservableCollection<IconVM>();

        public MainViewModel()
        {
            
            loadAllIcons();
            _FilteredIcons = Icons;
         
            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => Search());

            this.WhenAnyValue(x => x.StyleSourceCode)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => CreateSelectedIcons());
        }
        void loadAllIcons()
        {
            foreach (var path in Directory.EnumerateFiles("Icons", "*.xaml"))
            {
                using (var stream = File.Open(path, FileMode.Open))
                {
                    loadIcons(stream, Icons);
                }
            }
        }

        private void loadIcons(Stream stream, IList<IconVM> output)
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "GeometryDrawing" || reader.Name == "DrawingGroup")
                        {
                            var src = reader.ReadOuterXml();
                            output.Add(new IconVM(src));
                        }
                    }
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
       
        public void AddToStyle(IconVM? icon)
        {
            if (icon != null)
            {
                var source = icon.SourceCode.Replace("xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"", "")
                   .Replace("xmlns=\"https://github.com/avaloniaui\"", "")
                   .Replace("\t", "  ");

                StyleSourceCode += "\r\n" +source;
            }
        }
        public void CreateSelectedIcons()
        {
            // convert StyleSourceCode to a collection of drawings
            try
            {
                SelectedIcons.Clear();
                if (!String.IsNullOrEmpty(StyleSourceCode))
                {
                   var xaml = "<Styles xmlns=\"https://github.com/avaloniaui\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" >"
                        + StyleSourceCode
                        + "</Styles>";
                    loadIcons(new MemoryStream(Encoding.UTF8.GetBytes(xaml)), SelectedIcons);                    
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
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
