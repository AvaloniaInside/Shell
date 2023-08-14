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

namespace AvaloniaInside.Shell.Platform;

public class PlatformTransitioningContentControl : ContentControl, IPageSwitcher
{
    private CancellationTokenSource? _currentTransition;
    private ContentPresenter? _presenter2;
    private bool _isFirstFull;

    /// <summary>
    /// Defines the <see cref="PageTransition"/> property.
    /// </summary>
    public static readonly StyledProperty<IPageTransition?> PageTransitionProperty =
        AvaloniaProperty.Register<TransitioningContentControl, IPageTransition?>(
            nameof(PageTransition),
            defaultValue: new ImmutableCrossFade(TimeSpan.FromMilliseconds(125)));

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
        if (_presenter2 == null || !pageSwitcherArgs.WithAnimation || Content == null)
        {
            Content = pageSwitcherArgs.To;
            UpdateDefaultContent();
        }
        else
        {
            SwitchContent(pageSwitcherArgs);
        }
    }

    private void SwitchContent(PageSwitcherInfo info)
    {
        if ((_isFirstFull ? Presenter : _presenter2)?.Content == info.To)
            return;

        Debug.WriteLine("Called");
        //Content = info.To;

        _currentTransition?.Cancel();

        if (_presenter2 is not null &&
            Presenter is { } presenter &&
            (info.OverrideTransition ?? PageTransition) is { } transition)
        {
            var cancel = new CancellationTokenSource();
            _currentTransition = cancel;
            
            var from = _isFirstFull ? presenter : _presenter2;
            var to = !_isFirstFull ? presenter : _presenter2;
            _isFirstFull = !_isFirstFull;

            to.Content = info.To;

            var forward = info.Navigate is NavigateType.Normal or NavigateType.Top or NavigateType.Replace or NavigateType.ReplaceRoot;

            _ = transition.Start(from, to, forward, cancel.Token);
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateDefaultContent();
    }

    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (!base.RegisterContentPresenter(presenter) &&
            presenter is ContentPresenter p &&
            p.Name == "PART_ContentPresenter2")
        {
            _presenter2 = p;
            _presenter2.IsVisible = false;
            UpdateDefaultContent();

            return true;
        }

        return false;
    }

    void UpdateDefaultContent()
    {
        if ((_isFirstFull ? Presenter : _presenter2) is { } presenter)
        {
            presenter.Content = Content;
            presenter.IsVisible = true;
        }

        HideOldPresenter();
    }

    private void HideOldPresenter()
    {
        var oldPresenter = _isFirstFull ? _presenter2 : Presenter;
        if (oldPresenter is not null)
        {
            oldPresenter.Content = null;
            oldPresenter.IsVisible = false;
        }
    }

    private class ImmutableCrossFade : IPageTransition
    {
        private readonly CrossFade _inner;

        public ImmutableCrossFade(TimeSpan duration) => _inner = new CrossFade(duration);

        public Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
        {
            return _inner.Start(from, to, cancellationToken);
        }
    }
}
