using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;

public class NaturalNavigateStrategy : INavigateStrategy
{
	private readonly INavigationRegistrar _navigationRegistrar;

	public NaturalNavigateStrategy(INavigationRegistrar navigationRegistrar)
	{
		_navigationRegistrar = navigationRegistrar;
	}

	public virtual Task<Uri> NavigateAsync(
		NavigationChain chain,
		Uri currentUri,
		string path,
		CancellationToken cancellationToken)
	{
		var newUrl = new Uri(currentUri, path);
		if (!_navigationRegistrar.TryGetNode(newUrl.AbsolutePath, out var node))
			return Task.FromResult(newUrl);

		var defaultNode = node.GetLastDefaultNode();
		if (defaultNode == null) return Task.FromResult(newUrl);

		var uri = new Uri(_navigationRegistrar.RootUri, defaultNode.Route);
		if (newUrl.Query.Length <= 1) return Task.FromResult(uri);

		var query1 = newUrl.Query.Substring(1);
		var query2 = uri.Query.Length > 0 ? uri.Query.Substring(1) : uri.Query;

		var tagIndex1 = query1.IndexOf("#", StringComparison.Ordinal);
		var tagIndex2 = query2.IndexOf("#", StringComparison.Ordinal);

		if (tagIndex1 > 0 && tagIndex2 > 0)
			return Task.FromResult(new Uri(
				_navigationRegistrar.RootUri,
				$"{newUrl.AbsolutePath}?{query1.Insert(tagIndex1, query2.Substring(0, tagIndex2))}"));
		if (tagIndex1 > 0)
			return Task.FromResult(new Uri(
				_navigationRegistrar.RootUri,
				$"{newUrl.AbsolutePath}?{query1.Insert(tagIndex1, "&" + query2)}"));
		if (tagIndex2 > 0)
			return Task.FromResult(new Uri(
				_navigationRegistrar.RootUri,
				$"{newUrl.AbsolutePath}?{query2.Insert(tagIndex2, "&" + query1)}"));

		return Task.FromResult(new Uri(
			_navigationRegistrar.RootUri,
			$"{newUrl.AbsolutePath}?{query1}&{query2}"));

	}

	public virtual Task<Uri?> BackAsync(NavigationChain chain, Uri currentUri, CancellationToken cancellationToken) =>
		Task.FromResult<Uri?>(new Uri(currentUri, ".."));
}
