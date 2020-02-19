using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.IconPacks.Controls
{
    // an experiment in simple mvvm friendly dialog use
    public class SaveAsMenuItem : MenuItem, IStyleable
    {
        Type IStyleable.StyleKey => typeof(MenuItem);

        public String DialogTitle { get; set; } = "Save File As";

        public String DialogFilters { get; set; } = "All File|*.*";
   
        protected async override void OnClick(RoutedEventArgs e)
        {
            if (!e.Handled && Command?.CanExecute(CommandParameter) == true)
            {
                var filterParts = DialogFilters.Split('|');
                var filters = new List<FileDialogFilter>(filterParts.Length/2);
                for(int i=0;i<filterParts.Length;i+=2)
                {
                    filters.Add(new FileDialogFilter()
                    {
                        Name = filterParts[i],
                        Extensions = new List<string> { filterParts[i + 1] }
                    });
                }

                var dlg = new SaveFileDialog()
                {
                    Title = DialogTitle,
                    Filters = filters,
                };

                var filename = await dlg.ShowAsync(GetWindow());
                if (filename != null)
                {
                    Command.Execute(filename);
                }
                e.Handled = true;
            }
        }
        private Window? GetWindow()
        {
            return this.GetLogicalAncestors().OfType<Window>().FirstOrDefault();
        }
    }
}
