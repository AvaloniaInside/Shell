using Avalonia.Controls;
using AvaloniaInside.Shell;
using ShellExample.Helpers;
using ShellExample.Views.ShopViews;

namespace ShellExample.Views;

public partial class MainView : UserControl
{
	public static MainView Current { get; private set; }

	public MainView()
	{
		Current = this;
		InitializeComponent();

		ShellViewMain.Navigator.RegisterHost("/main", typeof(MainTabControl), "/main/home", NavigateType.Normal);
		ShellViewMain.Navigator.RegisterPage("/main/home", typeof(HomePage), NavigateType.Normal);
		ShellViewMain.Navigator.RegisterHost("/main/pets", typeof(PetsTabControlView), "/main/pets/cat",
			NavigateType.Normal);
		ShellViewMain.Navigator.RegisterPage("/main/pets/cat", typeof(CatView), NavigateType.Normal);
		ShellViewMain.Navigator.RegisterPage("/main/pets/dog", typeof(DogView), NavigateType.Normal);
		ShellViewMain.Navigator.RegisterPage("/main/product", typeof(ProductCatalogView), NavigateType.Normal);
		ShellViewMain.Navigator.RegisterPage("/main/product/filter", typeof(ProductCatalogFilterView),
			NavigateType.Modal);
		ShellViewMain.Navigator.RegisterPage("/main/setting", typeof(SettingView), NavigateType.Normal);
		ShellViewMain.Navigator.RegisterPage("/main/setting/profile", typeof(ProfileView), NavigateType.Normal);

		ShellViewMain.Navigator.RegisterPage("/main/home/confirmation", typeof(SimpleDialog), NavigateType.Modal);

		ShellViewMain.Navigator.RegisterPage("/second", typeof(SecondView), NavigateType.Normal);

		ShellViewMain.AddSideMenuItem("Home", "/main/home", "/Assets/Icons/house-solid.png".GetBitmapFromAssets());
		ShellViewMain.AddSideMenuItem("Cat", "/main/pets/cat", "/Assets/Icons/cat-solid.png".GetBitmapFromAssets());
		ShellViewMain.AddSideMenuItem("Dog", "/main/pets/dog", "/Assets/Icons/dog-solid.png".GetBitmapFromAssets());
		ShellViewMain.AddSideMenuItem("Products", "/main/product", "/Assets/Icons/tag-solid.png".GetBitmapFromAssets());
		ShellViewMain.AddSideMenuItem("Settings", "/main/setting", "/Assets/Icons/user-solid.png".GetBitmapFromAssets());
		ShellViewMain.AddSideMenuItem("Second Click", "/second", "/Assets/Icons/check-solid.png".GetBitmapFromAssets());
	}
	protected override void OnLoaded()
	{
		Current = this;
		base.OnLoaded();
	}
}
