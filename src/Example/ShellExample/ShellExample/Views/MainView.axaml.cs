using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using AvaloniaInside.Shell;
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
		ShellViewMain.Navigation.RegisterHost("/main/pets", typeof(PetsTabControlView), "/main/pets/cat", NavigateType.Normal);
		ShellViewMain.Navigation.RegisterPage("/main/pets/cat", typeof(CatView), NavigateType.Normal);
		ShellViewMain.Navigation.RegisterPage("/main/pets/dog", typeof(DogView), NavigateType.Normal);
		ShellViewMain.Navigation.RegisterPage("/main/product", typeof(ProductCatalogView), NavigateType.Normal);
		ShellViewMain.Navigation.RegisterPage("/main/product/filter", typeof(ProductCatalogFilterView), NavigateType.Modal);
		ShellViewMain.Navigation.RegisterPage("/main/setting", typeof(SettingView), NavigateType.Normal);
		ShellViewMain.Navigation.RegisterPage("/main/setting/profile", typeof(ProfileView), NavigateType.Normal);

		ShellViewMain.Navigation.RegisterPage("/main/home/confirmation", typeof(SimpleDialog), NavigateType.Modal);

		ShellViewMain.Navigation.RegisterPage("/second", typeof(SecondView), NavigateType.Normal);
	}

	protected override void OnLoaded()
	{
		Current = this;
		base.OnLoaded();
	}
}
