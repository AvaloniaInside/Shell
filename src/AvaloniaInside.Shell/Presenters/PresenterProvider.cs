using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Presenters;

public class PresenterProvider : IPresenterProvider
{
	public IPresenter For(NavigateType type)
	{
		return type switch
		{
			NavigateType.Modal => new ModalPresenter(),
			NavigateType.Pop => Pop(),
			_ => new NormalPresenter()
		};
	}

	public IPresenter Remove() => new RemovePresenter();
	public IPresenter Pop() => new RemovePresenter();
}

public class ModalPresenter : PresenterBase
{
	public override Task PresentAsync(NavigationChain chain, CancellationToken cancellationToken) =>
		CurrentShellView?.ModalAsync(chain.Instance, cancellationToken) ?? Task.CompletedTask;
}
