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
                loadIcons(File.Open(path, FileMode.Open),Icons);
            }
        }

        private void loadIcons(Stream stream, IList<IconVM> output)
        {
            XmlReader reader = XmlReader.Create(stream);

            reader.MoveToContent();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "GeometryDrawing" || reader.Name == "DrawingGroup")
                    {
                        var src = reader.ReadOuterXml();
                        try
                        {
                            var icon = parseIconVM(src);
                            if (icon != null) output.Add(icon);
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine($"Error parsing icon {src} {e.Message}");
                        }
                    }
                }
            }
        }
        private IconVM? parseIconVM(string src)
        {
            Drawing? drawing=null;
            XmlReader reader = XmlReader.Create(new StringReader(src));
            string? key=null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var tag = reader.Name;
                    switch (tag)
                    {
                        case "GeometryDrawing": drawing = parseGeometryDrawing(reader, out key); break;
                        case "DrawingGroup": drawing = parseDrawingGroup(reader, out key); break;
                    }
                }
            }
            if (key!= null && drawing != null)
            {
                return new IconVM(name: key, sourceCode: src, drawing: drawing);
            }
            else return null;
        }
        private GeometryDrawing parseGeometryDrawing(XmlReader reader, out string? key)
        {
            key = null;
            GeometryDrawing drawing = new GeometryDrawing();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "Brush": drawing.Brush = Brush.Parse(reader.Value); break;
                    case "Geometry": drawing.Geometry = Geometry.Parse(reader.Value); break;
                    case "x:Key": key = reader.Value; break;
                }
            }
            reader.MoveToElement();
            reader = reader.ReadSubtree();
            while(reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "GeometryDrawing.Geometry": drawing.Geometry = parseGeometry(reader.ReadSubtree()); break;
                        case "GeometryDrawing.Brush": drawing.Brush = parseBrush(reader.ReadSubtree());break;
                    }
                }
            }
            return drawing;
        }
        private Geometry? parseGeometry(XmlReader reader)
        {
            while( reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "EllipseGeometry": return parseEllipseGeometry(reader);
                        case "RectangleGeometry": return parseRectangleGeometry(reader);
                    }
                }
            }
            return null;
        }
        private EllipseGeometry parseEllipseGeometry(XmlReader reader)
        {
            EllipseGeometry geometry = new EllipseGeometry();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "Rect": geometry.Rect=Rect.Parse(reader.Value); break;
                }
            }
            return geometry;
        }
        private RectangleGeometry parseRectangleGeometry(XmlReader reader)
        {
            RectangleGeometry geometry = new RectangleGeometry();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "Rect": geometry.Rect = Rect.Parse(reader.Value); break;
                }
            }
            return geometry;
        }
        private Brush? parseBrush(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "LinearGradientBrush": return parseLinearGradientBrush(reader);
                    }
                }
            }
            return null;
        }
        private LinearGradientBrush parseLinearGradientBrush(XmlReader reader)
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "StartPoint": brush.StartPoint = RelativePoint.Parse(reader.Value); break;
                    case "EndPoint": brush.EndPoint = RelativePoint.Parse(reader.Value); break;
                }
            }
            reader.MoveToElement();
            reader = reader.ReadSubtree();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "LinearGradientBrush.GradientStops": brush.GradientStops = parseGradientStops(reader.ReadSubtree()); break;
                    }
                }
            }
            return brush;
        }
        private GradientStops parseGradientStops(XmlReader reader)
        {
            var gradientStops = new GradientStops();
            
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "GradientStop")
                    {
                        gradientStops.Add(parseGradientStop(reader));
                    }
                }
            }
            return gradientStops;
        }
        private GradientStop parseGradientStop(XmlReader reader)
        {
            GradientStop gradientStop = new GradientStop();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "Color": gradientStop.Color = Color.Parse(reader.Value); break;
                    case "Offset": gradientStop.Offset = Double.Parse(reader.Value); break;
                }
            }
            return gradientStop;
        }
        private DrawingGroup parseDrawingGroup(XmlReader reader, out string? key)
        {
            key = null;
            string dummyKey;
            var drawingGroup = new DrawingGroup();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "x:Key": key = reader.Value; break;
                }
            }
            reader.MoveToElement();
            reader = reader.ReadSubtree();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "GeometryDrawing")
                    {
                        drawingGroup.Children.Add(parseGeometryDrawing(reader, out dummyKey));
                    }
                }
            }
            return drawingGroup;            
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
