using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Avalonia.IconPacks.Parsers
{
    public class DrawingParser
    {
        public static Drawing? Parse(string src)
        {
            Drawing? drawing = null;
            using (XmlReader reader = XmlReader.Create(new StringReader(src)))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        var tag = reader.Name;
                        switch (tag)
                        {
                            case "GeometryDrawing": drawing = ParseGeometryDrawing(reader); break;
                            case "DrawingGroup": drawing = ParseDrawingGroup(reader); break;
                        }
                    }
                }
            }
            return drawing;
        }

        private static GeometryDrawing ParseGeometryDrawing(XmlReader reader)
        {
            GeometryDrawing drawing = new GeometryDrawing();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "Brush": drawing.Brush = Brush.Parse(reader.Value); break;
                    case "Geometry": drawing.Geometry = Geometry.Parse(reader.Value); break;
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
                        case "GeometryDrawing.Geometry": drawing.Geometry = ParseGeometry(reader.ReadSubtree()); break;
                        case "GeometryDrawing.Brush": drawing.Brush = ParseBrush(reader.ReadSubtree()); break;
                    }
                }
            }
            return drawing;
        }
        private static Geometry? ParseGeometry(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "EllipseGeometry": return ParseEllipseGeometry(reader);
                        case "RectangleGeometry": return ParseRectangleGeometry(reader);
                    }
                }
            }
            return null;
        }
        private static EllipseGeometry ParseEllipseGeometry(XmlReader reader)
        {
            EllipseGeometry geometry = new EllipseGeometry();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "Rect": geometry.Rect = Rect.Parse(reader.Value); break;
                }
            }
            return geometry;
        }
        private static RectangleGeometry ParseRectangleGeometry(XmlReader reader)
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
        private static Brush? ParseBrush(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "LinearGradientBrush": return ParseLinearGradientBrush(reader);
                    }
                }
            }
            return null;
        }
        private static LinearGradientBrush ParseLinearGradientBrush(XmlReader reader)
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
                        case "LinearGradientBrush.GradientStops": brush.GradientStops = ParseGradientStops(reader.ReadSubtree()); break;
                    }
                }
            }
            return brush;
        }
        private static GradientStops ParseGradientStops(XmlReader reader)
        {
            var gradientStops = new GradientStops();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "GradientStop")
                    {
                        gradientStops.Add(ParseGradientStop(reader));
                    }
                }
            }
            return gradientStops;
        }
        private static GradientStop ParseGradientStop(XmlReader reader)
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
        private static DrawingGroup ParseDrawingGroup(XmlReader reader)
        {

            var drawingGroup = new DrawingGroup();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    //      case "x:Key": key = reader.Value; break;
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
                        drawingGroup.Children.Add(ParseGeometryDrawing(reader));
                    }
                }
            }
            return drawingGroup;
        }
    }
}
