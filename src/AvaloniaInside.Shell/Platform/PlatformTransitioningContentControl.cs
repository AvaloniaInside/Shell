using Avalonia.Animation;
using Avalonia.Controls.Presenters;
using Avalonia.Controls;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Styling;
using System.Diagnostics;
using Avalonia.Input;
using Avalonia.Controls.Primitives;

namespace AvaloniaInside.Shell.Platform;

public class PlatformTransitioningContentControl : ContentControl, IPageSwitcher
{
    private CancellationTokenSource? _currentTransition;
    private Panel? _panel;
    private object? _topContent;

    /// <summary>
    /// Defines the <see cref="PageTransition"/> property.
    /// </summary>
    public static readonly StyledProperty<IPageTransition?> PageTransitionProperty =
        AvaloniaProperty.Register<TransitioningContentControl, IPageTransition?>(
            nameof(PageTransition),
            defaultValue: new ImmutablePlatform());

    /// <summary>
    /// Gets or sets the animation played when content appears and disappears.
    /// </summary>
    public IPageTransition? PageTransition
    {
        get => GetValue(PageTransitionProperty);
        set => SetValue(PageTransitionProperty, value);
    }

    public void SwitchPage(PageSwitcherInfo pageSwitcherArgs)
    {
        if (_panel == null || !pageSwitcherArgs.WithAnimation || pageSwitcherArgs.To == null)
        {
            _topContent = pageSwitcherArgs.To;
            UpdateDefaultContent();
        }
        else
        {
            SwitchContent(pageSwitcherArgs);
        }
    }

    private void SwitchContent(PageSwitcherInfo info)
    {
        if (_topContent == info.To)
            return;

        _currentTransition?.Cancel();

        if ((info.OverrideTransition ?? PageTransition) is { } transition)
        {
            var cancel = new CancellationTokenSource();
            _currentTransition = cancel;

            var toControl = (Control)info.To;
            if (_panel.Children.Contains(toControl))
                _panel.Children.Remove(toControl);
            _panel.Children.Add(toControl);

            if (_topContent is Control fromControl)
            {
                if (_panel.Children.Contains(fromControl))
                    _panel.Children.Remove(fromControl);
                _panel.Children.Add(fromControl);
            }

            var forward = info.Navigate is NavigateType.Normal or NavigateType.Top or NavigateType.Replace or NavigateType.ReplaceRoot;

            transition.Start(_topContent as Visual, toControl, forward, cancel.Token).ContinueWith(t =>
            {

            });
            _topContent = info.To;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateDefaultContent();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _panel = e.NameScope.Find<Panel>("PART_Panel");
    }

    void UpdateDefaultContent()
    {
        if (_panel == null) return;

        _panel.Children.Clear();
        if (_topContent is Control control)
            _panel.Children.Add(control);
    }

    private class ImmutablePlatform : IPageTransition
    {
        private readonly IPageTransition _inner;

        public ImmutablePlatform()
        {
            _inner = new Ios.DefaultIosPageSlide();
        }

        public Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
        {
            return _inner.Start(from, to, forward, cancellationToken);
        }
    }
}
