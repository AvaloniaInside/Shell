using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using AvaloniaInside.Shell.Platform.Windows;

namespace AvaloniaInside.Shell;

public class StackContentViewPanel : Panel
{
    public Task? AnimationToBeDone { get; set; }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var rcChild = new Rect(finalSize);
        var zindex = 0;
        foreach (var control in Children)
        {
            control.ZIndex = zindex++;
            control.Arrange(rcChild);
        }

        return finalSize;
    }

    protected override async void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Remove && AnimationToBeDone != null)
        {
            await AnimationToBeDone;
            AnimationToBeDone = null;
        }
        base.ChildrenChanged(sender, e);
    }
}

public class StackContentViewItem : ContentControl
{

}

public class StackContentView : ItemsControl
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private NavigateType? _pendingNavigateType;
    private Control? _lastContainer;
    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<Border, bool>(nameof(HasContent));

    /// <summary>
    /// Defines the <see cref="PageTransition"/> property.
    /// </summary>
    public static readonly StyledProperty<IPageTransition?> PageTransitionProperty =
        AvaloniaProperty.Register<TransitioningContentControl, IPageTransition?>(
            nameof(PageTransition),
            defaultValue: new ListSlideNavigationTransition());

    /// <summary>
    /// Gets or sets the animation played when content appears and disappears.
    /// </summary>
    public IPageTransition? PageTransition
    {
        get => GetValue(PageTransitionProperty);
        set => SetValue(PageTransitionProperty, value);
    }

    public StackContentView()
    {
        //ItemsPanel = new ItemsPanelTemplate() { Content = new StackContentViewPanel() };
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) =>
        new StackContentViewItem();

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) =>
        NeedsContainer<StackContentViewItem>(item, out recycleKey);

    protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
    {
        base.PrepareContainerForItemOverride(element, item, index);
    }

    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);

        if (index == Items.Count - 1)
        {
            _lastContainer = container;
            _ = PageTransition?.Start(
                Items.Count > 1 ? ContainerFromIndex(index - 1) : null,
                container,
                true,
                CancellationToken.None);
        }
    }

    protected override async void ClearContainerForItemOverride(Control container)
    {
        var currentContainer = ContainerFromIndex(Items.Count - 1);

        if (_lastContainer != container)
        {
            base.ClearContainerForItemOverride(container);
            return;
        }

        var task = (PageTransition?.Start(
            container,
            currentContainer,
            false,
            CancellationToken.None) ?? Task.CompletedTask);

        if (ItemsPanelRoot is StackContentViewPanel panel)
            panel.AnimationToBeDone = task;

        await task;

        base.ClearContainerForItemOverride(container);
        _lastContainer = currentContainer;
    }

    public bool HasContent
    {
        get => GetValue(HasContentProperty);
        private set => SetValue(HasContentProperty, value);
    }

    public object? CurrentView => Items.LastOrDefault();

    public async Task PushViewAsync(object view,
        NavigateType navigateType,
        CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            var current = CurrentView;

            if (current != null && current == view) return;
            if (view is not Control control) return;

            // Bring to front if exists in collection
            if (Items.Contains(control))
                Items.Remove(control);
            Items.Add(control);

            UpdateCurrentView(current, control, navigateType, false);

            await OnContentUpdateAsync(control, cancellationToken);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    protected virtual void UpdateCurrentView(object? from, object? to, NavigateType navigateType, bool removed)
    {
        //TODO: Apply specific animation type
    }

    public async Task<bool> RemoveViewAsync(object view, NavigateType navigateType, CancellationToken cancellationToken)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            if (!Items.Contains(view)) return false;
            var from = CurrentView;

            Items.Remove(view as Control);

            var to = CurrentView;
            if (from != to)
            {
                UpdateCurrentView(from, to, navigateType, true);
                await OnContentUpdateAsync(CurrentView, cancellationToken);
            }
            return true;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    protected virtual Task OnContentUpdateAsync(object? view, CancellationToken cancellationToken)
    {
        HasContent = Items.Count > 0;
        return Task.CompletedTask;
    }

    public Task ClearStackAsync(CancellationToken cancellationToken)
    {
        while (Items.Count > 1)
            Items.RemoveAt(0);

        return Task.CompletedTask;
    }
}
