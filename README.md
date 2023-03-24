# Shell

Avalonia Inside Shell reduces the complexity of mobile/desktop application development by providing the fundamental features that most applications require, including:

    A single place to describe the visual hierarchy of an application.
    A common navigation user experience.
    A URI-based navigation scheme that permits navigation to any page in the application
    

This project is currently in alpha stage of development. This means that it is still being designed and built, and it has not been thoroughly tested yet. It may contain serious errors, bugs, or missing features that could affect its functionality and performance. Use it at your own risk and do not rely on it for any critical purposes.

We welcome feedback, suggestions, and contributions from anyone who is interested in this project. We appreciate your support and patience as we work towards releasing a stable version of this project.


## Screenshots

![image](https://user-images.githubusercontent.com/956077/226295190-cbe81c7d-4054-4c07-9e5c-7ee7149c1468.png)

![image](https://user-images.githubusercontent.com/956077/226295294-3d4f1f9e-941d-4248-b941-a0c35ca0533a.png)

## Installation

To use AvaloniaInside.Shell in your Avalonia project, you can install the package via NuGet using the following command in the Package Manager Console:

```bash
dotnet add package AvaloniaInside.Shell --version 0.1.1-alpha
```

Alternatively, you can also install the package through Visual Studio's NuGet Package Manager.

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

