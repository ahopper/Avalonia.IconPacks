using Avalonia.IconPacks.Parsers;
using Avalonia.IconPacks.Utils;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.IO;


namespace Avalonia.IconPacks.ViewModels
{
    public class IconVM
    {
        public IconVM (string sourceCode)
        {
            SourceCode = sourceCode;
            Name = GetXmlAttribute(sourceCode, "x:Key");
        }
        public string Name { get; set; }
       
        public string SourceCode { get; set; }
        private Drawing? _drawing;
        public Drawing Drawing 
        { 
            get
            {
                if(_drawing == null)
                {
                    try
                    {
                        _drawing = DrawingParser.Parse(SourceCode);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"Error Parsing Icon {SourceCode} --- {e.Message}");
                    }
                }
                if(_drawing == null)
                {
                    // todo add error icon
                    _drawing = new GeometryDrawing();
                }
                return _drawing;
            }
        }
        private string GetXmlAttribute(string xml, string name)
        {
           
            int i = xml.IndexOf(name);
            if (i > -1)
            {
                int start = xml.IndexOf("\"", i);
                if (start > -1 && xml.Length > start + 1)
                {
                    int end = xml.IndexOf("\"", start + 1);
                    if (end > -1)
                    {
                        return xml.Substring(start + 1, end - start - 1);
                    }
                }
            }
            return "";
        }
        public void SaveAsIcon(string filename)
        {
            if (filename != null)
            {
                switch (Path.GetExtension(filename))
                {
                    case ".ico": IconFile.SaveToICO(Drawing, new List<int> { 16, 32, 64, 256 }, filename); break;
                    case ".icns": IconFile.SaveToICNS(Drawing, new List<int> { 16, 32, 64, 256, 512 }, filename); break;
                    case ".png": IconFile.SaveToPNG(Drawing, 400, filename); break;
                }
            }
        }
    }
}
