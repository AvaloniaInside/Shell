namespace AvaloniaInside.Shell;

public interface IPresenterProvider
{
	IPresenter For(NavigateType type);
	IPresenter Remove();
	IPresenter Pop();
}
