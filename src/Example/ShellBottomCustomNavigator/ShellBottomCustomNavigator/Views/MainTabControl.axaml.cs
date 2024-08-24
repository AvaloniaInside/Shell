using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace ShellBottomCustomNavigator.Views;

public partial class MainTabControl : TabControl
{
	protected override Type StyleKeyOverride => typeof(TabControl);

    private Grid _circle;
    public MainTabControl()
    {
        InitializeComponent();

        var aaa = Styles;

        this.SelectionChanged += (sender, args) =>
        {
            if (_circle == null) return;
            int idx = SelectedIndex;
            Canvas.SetLeft(_circle, idx * 80);
        };
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _circle = e.NameScope.Get<Grid>("PART_Circle");
        this.SelectedIndex = 0;
    }
}

