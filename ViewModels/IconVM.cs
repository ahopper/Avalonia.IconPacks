using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.IconPacks.ViewModels
{
    public class IconVM
    {
        public string Name { get; set; }
        public string SourceCode { get; set; }
        public MainViewModel parent { get; set; }

        Drawing _drawing;
        public Drawing drawing
        {
            get
            {
                if(_drawing==null)_drawing= (Drawing)parent.window.FindResource(Name);
                return _drawing;
            }
        }
    }
}
