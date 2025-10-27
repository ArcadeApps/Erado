using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Erado.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Erado.UI;
public static class ViewLocatorHelpers
{
    public static IServiceCollection AddView<TViewModel, TView>(this IServiceCollection services)
        where TView : Control, new()
        where TViewModel : ViewModelBase
    {
        services.AddSingleton(new ViewLocator.ViewLocationDescriptor(typeof(TViewModel), () => new TView()));
        return services;
    }
}

public class ViewLocator : IDataTemplate
{
    private readonly Dictionary<Type, Func<Control>> _views = new();
    
    public ViewLocator(IEnumerable<ViewLocationDescriptor> descriptors)
    {
        _views = descriptors.ToDictionary(x => x.ViewModel, x => x.Factory);
    }

    public Control? Build(object? param) => _views[param!.GetType()]();

    public bool Match(object? data) => data is not null && _views.ContainsKey(data.GetType());
    
    public record ViewLocationDescriptor(Type ViewModel, Func<Control> Factory);
}