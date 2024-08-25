using System;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;
using AvaloniaInside.Shell.Data;

namespace AvaloniaInside.Shell;

public partial class ShellView
{
	[Content] public AvaloniaList<IItem> Items { get; } = new();

	private void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.Action != NotifyCollectionChangedAction.Add) throw new Exception("Only add supported for the moment");
		foreach (var item in e.NewItems.Cast<IItem>())
			OnAddItem(item);
	}

	protected virtual void OnAddItem(IItem item)
	{
		switch (item)
		{
			case Host host:
				AddRoute(host, string.Empty);
				break;
			case Route route:
				AddRoute(route, string.Empty);
				break;
			case SideMenuItem sideMenuItem:
				_sideMenuItems.Add(sideMenuItem);
				break;
		}
	}

	private void AddRoute(Route route, string basePath)
	{
		var path = $"{basePath}/{route.Path}";
		var host = route as Host;

		if (host != null && !HostedItemsHelper.CanBeHosted(host.Page))
			throw new AggregateException("Host must inherits from ItemsControl");

		Navigator.Registrar.RegisterRoute(
			path,
			route.Page,
			host == null ? NavigationNodeType.Page : NavigationNodeType.Host,
			route.Type,
			host?.Default);

		foreach (var subRoute in route.Routes)
			AddRoute(subRoute, path);
	}
}
