using System;

namespace AvaloniaInside.Shell.Presenters;

public class PresenterProvider : IPresenterProvider
{
	public IPresenter For(NavigateType type)
	{
		return type switch
		{
			NavigateType.Modal => new ModalPresenter(),
			_ => new NormalPresenter()
		};
	}

	public IPresenter Remove() => new RemovePresenter();
}
