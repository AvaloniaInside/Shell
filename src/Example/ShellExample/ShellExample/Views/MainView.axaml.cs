using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaInside.Shell;
using Projektanker.Icons.Avalonia;
using ShellExample.Views.ShopViews;

namespace ShellExample.Views;

public partial class MainView : UserControl
{
	public static MainView Current { get; private set; }

	public MainView()
	{
		Current = this;
		InitializeComponent();

		ShellViewMain.Navigation.RegisterHost("/main", typeof(MainTabControl), "/main/home", NavigateType.Normal);
		ShellViewMain.Navigation.RegisterPage("/main/home", typeof(HomePage), NavigateType.Normal);
		ShellViewMain.Navigation.RegisterHost("/main/pets", typeof(PetsTabControlView), "/main/pets/cat",
			NavigateType.Normal);
		ShellViewMain.Navigation.RegisterPage("/main/pets/cat", typeof(CatView), NavigateType.Normal);
		ShellViewMain.Navigation.RegisterPage("/main/pets/dog", typeof(DogView), NavigateType.Normal);
		ShellViewMain.Navigation.RegisterPage("/main/product", typeof(ProductCatalogView), NavigateType.Normal);
		ShellViewMain.Navigation.RegisterPage("/main/product/filter", typeof(ProductCatalogFilterView),
			NavigateType.Modal);
		ShellViewMain.Navigation.RegisterPage("/main/setting", typeof(SettingView), NavigateType.Normal);
		ShellViewMain.Navigation.RegisterPage("/main/setting/profile", typeof(ProfileView), NavigateType.Normal);

		ShellViewMain.Navigation.RegisterPage("/main/home/confirmation", typeof(SimpleDialog), NavigateType.Modal);

		ShellViewMain.Navigation.RegisterPage("/second", typeof(SecondView), NavigateType.Normal);

		ShellViewMain.AddSideMenuItem("Home", "/main/home", GetFAIcon("fa-solid fa-house"));
		ShellViewMain.AddSideMenuItem("Cat", "/main/pets/cat", GetFAIcon("fa-solid fa-cat"));
		ShellViewMain.AddSideMenuItem("Dog", "/main/pets/dog", GetFAIcon("fa-solid fa-dog"));
		ShellViewMain.AddSideMenuItem("Products", "/main/product", GetFAIcon("fa-solid fa-tag"));
		ShellViewMain.AddSideMenuItem("Settings", "/main/setting", GetFAIcon("fa-solid fa-gear"));
	}

	private IImage GetFAIcon(string value) =>
		new Icon
		{
			Value = value,
			Foreground = Brushes.White,
			FontSize = 14
		}.DrawingImage;

	protected override void OnLoaded()
	{
		Current = this;
		base.OnLoaded();
	}
}
