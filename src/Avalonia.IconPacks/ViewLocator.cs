using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.IconPacks.ViewModels;

namespace Avalonia.IconPacks
{
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            var typeName = data.GetType().FullName;
            if (typeName?.Replace("ViewModel", "View") is string name )
            {
                if(Type.GetType(name) is Type type)
                {
                    if(Activator.CreateInstance(type) is Control control)
                    {
                        return control;
                    }
                }
            }
            return new TextBlock { Text = $"View Not Found For: { typeName ?? data.GetType().Name }" };
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}