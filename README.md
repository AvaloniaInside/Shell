# Shell

Avalonia Inside Shell reduces the complexity of mobile/desktop application development by providing the fundamental features that most applications require, including:

    A single place to describe the visual hierarchy of an application.
    A common navigation user experience.
    A URI-based navigation scheme that permits navigation to any page in the application
    

We welcome feedback, suggestions, and contributions from anyone who is interested in this project. We appreciate your support and patience as we work towards releasing a stable version of this project.


## Screenshots

<img src="https://user-images.githubusercontent.com/956077/226295190-cbe81c7d-4054-4c07-9e5c-7ee7149c1468.png" width="300"/> <img src="https://user-images.githubusercontent.com/956077/226295294-3d4f1f9e-941d-4248-b941-a0c35ca0533a.png" width="300"/>  

## Installation

To use AvaloniaInside.Shell in your Avalonia project, you can install the package via NuGet using the following command in the Package Manager Console:

```bash
dotnet add package AvaloniaInside.Shell --version 1.1.2
```

Alternatively, you can also install the package through Visual Studio's NuGet Package Manager.

Add dependencies to the Locator or call `Program.cs`
```csharp
public static AppBuilder BuildAvaloniaApp()
	=> AppBuilder.Configure<App>()
		.UsePlatformDetect()
		.LogToTrace()
		.UseReactiveUI()
		.UseShell();
```

Add default theme or add your custom theme to the App.axmal file
```xml
<StyleInclude Source="avares://AvaloniaInside.Shell/Default.axaml"></StyleInclude>
```

## Using Shell

#### Adding a ShellView
To use a ShellView in your application, you can add it to your XAML file like this:
```xml
<ShellView>
  <!-- Your application content goes here -->
</ShellView>
```

#### Basic Usage Guide for Shell and Navigation
You can register navigation using the `Host` and `Route` elements. `Host` can be used to group pages under a common root, such as a `TabControl`. Here's an example:

```xml
<ShellView Name="MyShellView">
    <Host Path="main" Page="views:MainTabControl">
        <Route Path="home" Page="views:HomePage">
            <Route Path="confirmation" Page="views:SimpleDialog" Type="Modal"></Route>
        </Route>
        <Host Path="pets" Page="views:PetsTabControlView" Default="cat">
            <Route Path="cat" Page="views:CatView"></Route>
            <Route Path="dog" Page="views:DogView"></Route>
        </Host>
        <Route Path="product" Page="shopViews:ProductCatalogView">
            <Route Path="filter" Page="shopViews:ProductCatalogFilterView" Type="Modal"></Route>
        </Route>
        <Route Path="setting" Page="views:SettingView">
            <Route Path="profile" Page="views:ProfileView"></Route>
        </Route>
    </Host>
</ShellView>
```

And to navigate to a specific page, we can use the Navigator property of the ShellView, as follows:

```csharp
await MyShellView.Navigator.NavigateAsync("/main/home/confirmation", cancellationToken);
```

### NavigationBar 
![image](https://user-images.githubusercontent.com/956077/227613963-9b1a10b5-c2b0-4dcb-ba43-cd72f3a27333.png)

Each page that is currently on top of the navigation stack has access to the navigation bar's title and navigation item. In hierarchical hosts, the currently selected item in the host will be the one that has access to the navigation bar. For example, in the case of /home/pets/cat, the page associated with the cat would be able to modify the navigation bar. This can be done by setting the NavigationBar.Header and NavigationBar.Item properties, as shown in the code snippet below:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             x:Class="ShellExample.Views.ShopViews.ProductCatalogView"
             NavigationBar.Header="Products">
	<NavigationBar.Item>
		<Button Content="Filter" Command="{Binding FilterCommand}"></Button>
	</NavigationBar.Item>
 ...
</UserControl>
```


## ShellView content's support

`IItem` is the base interface for all items that can be added to the `ShellView`'s content. It has the following properties:

#### Route

`Route` is a type of `IItem` that represents a navigation route in the `ShellView`. It has a `Page` property that specifies the view associated with the route, as well as an optional `Type` property that specifies the navigation type, And `Path` is the property to register navigate.

#### Host

`Host` can be used to group pages under a common root, such as a `TabControl`.
It has a `Page` property that specifies the view associated with the host, as well as an optional `Default` property that specifies the default child route. as well as a `Children` property that specifies any child `Route` objects. 

#### SideMenuItem

`SideMenuItem` is another type of item that can be used to display navigation options in the ShellView's side menu. It also inherits from `IItem` and has the following properties:

`Icon`: An icon to display next to the item's label in the side menu.
`Title`: The text label to display for the item in the side menu.
`Path`: The path of navigation.

## Example of Side Menu usage

![side-menu](https://user-images.githubusercontent.com/956077/227538250-f5f86187-6c0f-46a0-803e-951b02abdccd.png)


```xml
<UserControl xmlns="https://github.com/avaloniaui" ...>

    <ShellView Name="ShellViewMain" DefaultRoute="/main">
        <Host Path="main" Page="views:MainTabControl"> ... </Host>
        
        <!-- Side Menu Header  -->
        <ShellView.SideMenuHeader>
		<widgets:UserProfileWidgetView></widgets:UserProfileWidgetView>
	</ShellView.SideMenuHeader>

        <!-- SideMenuItems go here -->
        <SideMenuItem Path="/main/home" Title="Home" Icon="/Assets/Icons/house-solid.png"></SideMenuItem>
        <SideMenuItem Path="/main/pets/cat" Title="Cat" Icon="/Assets/Icons/cat-solid.png"></SideMenuItem>
        <SideMenuItem Path="/main/pets/dog" Title="Dog" Icon="/Assets/Icons/dog-solid.png"></SideMenuItem>
        <SideMenuItem Path="/main/product" Title="Products" Icon="/Assets/Icons/tag-solid.png"></SideMenuItem>
        <SideMenuItem Path="/main/setting" Title="Settings" Icon="/Assets/Icons/user-solid.png"></SideMenuItem>
        <SideMenuItem Path="/second" Title="Second Click" Icon="/Assets/Icons/check-solid.png"></SideMenuItem>
        
        <!-- SideMenu Content -->
        <ShellView.SideMenuContents>
		<widgets:WeatherView Margin="0, 20, 0, 0" />
		<widgets:CalendarWidgetView Margin="0, 20, 0, 0" />
	</ShellView.SideMenuContents>
        
        <!-- SideMenu Footer -->
	<ShellView.SideMenuFooter>
		<Border Background="#11000000" Height="25">
			<TextBlock Text="AvaloniaInside Shell 2023"
				   FontWeight="Light"
				   VerticalAlignment="Center"
				   HorizontalAlignment="Center">
			</TextBlock>
		</Border>
	</ShellView.SideMenuFooter>
    </ShellView>
</UserControl>

```

In the code above side menu will contains 6 navigate route and will link selected item to the current route.
