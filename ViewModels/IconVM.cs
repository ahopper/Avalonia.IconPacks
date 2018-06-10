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

        public Drawing drawing
        {
            get
            {
                return (Drawing)parent.window.FindResource(Name);
            }
        }
    }
}
