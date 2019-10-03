using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.IconPacks.ViewModels
{
    public class IconVM
    {
        public IconVM (string name, string sourceCode, Drawing drawing)
        {
            Name = name;
            SourceCode = sourceCode;
            Drawing = drawing;
        }
        public string Name { get; set; }
        public string SourceCode { get; set; }
        public Drawing Drawing { get; set; }
       
    }
}
