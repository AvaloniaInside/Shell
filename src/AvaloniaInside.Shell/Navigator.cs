using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;

public partial class Navigator : INavigator
{
    private readonly INavigateStrategy _navigateStrategy;
    private readonly INavigationUpdateStrategy _updateStrategy;
    private readonly NavigationStack _stack;
    private readonly Dictionary<NavigationChain, TaskCompletionSource<NavigateResult>> _waitingList = new();

    private bool _navigating;
    private ShellView? _shellView;

    public ShellView ShellView => _shellView ?? throw new ArgumentNullException(nameof(ShellView));

    public Uri CurrentUri => _stack.Current?.Uri ?? Registrar.RootUri;

    public NavigationChain? CurrentChain => _stack.Current;

    public INavigationRegistrar Registrar { get; }

    public Navigator(
        INavigationRegistrar navigationRegistrar,
        INavigateStrategy navigateStrategy,
        INavigationUpdateStrategy updateStrategy,
        INavigationViewLocator viewLocator)
    {
        Registrar = navigationRegistrar;
        _navigateStrategy = navigateStrategy;
        _updateStrategy = updateStrategy;
        _stack = new(viewLocator);

        _updateStrategy.HostItemChanged += UpdateStrategyOnHostItemChanged;
    }

    public void RegisterShell(ShellView shellView)
    {
        if (_shellView != null) throw new ArgumentException("Register shell can call only once");
        _shellView = shellView;
    }

    public bool HasItemInStack()
    {
        var current = _stack.Current?.Back;
        while (current != null)
        {
            if (current is not HostNavigationChain)
                return true;

            current = current.Back;
        }

        return false;
    }

    private async Task NotifyAsync(
        Uri origin,
        Uri newUri,
        object? argument,
        bool hasArgument,
        object? sender,
        NavigateType? navigateType,
        bool withAnimation,
        CancellationToken cancellationToken = default)
    {
        if (!Registrar.TryGetNode(newUri.AbsolutePath, out var node) || node is null)
        {
            Debug.WriteLine("Warning: Cannot find the path");
            return;
        }

        var finalNavigateType =
            !origin.AbsolutePath.Equals(newUri.AbsolutePath) &&
            Registrar.TryGetNode(origin.AbsolutePath, out var originalNode) &&
            originalNode is not null
                ? navigateType ?? originalNode.Navigate
                : navigateType ?? node.Navigate;

        var fromPage = _stack.Current?.Instance as INavigatorLifecycle;
        if (fromPage != null)
        {
            var args = new NavigatingEventArgs
            {
                Sender = sender,
                From = CurrentUri,
                FromUri = origin,
                ToUri = newUri,
                Argument = argument,
                Navigate = finalNavigateType,
                WithAnimation = withAnimation,
            };

            await fromPage.OnNavigatingAsync(args, cancellationToken);
            if (args.Cancel) return;

            //Check for overrides

            if (argument != args.Argument)
            {
                argument = args.Argument;
                hasArgument = true;
            }

            finalNavigateType = args.Navigate;
            withAnimation = args.WithAnimation;
        }

        _navigating = true;

        var stackChanges = _stack.Push(
            node,
            finalNavigateType,
            newUri);

        foreach (var newChain in stackChanges.NewNavigationChains)
        {
	        SetupPage(newChain);
        }

        var postArgument = new NavigateEventArgs
        {
	        Sender = sender,
	        From = fromPage,
	        To = _stack.Current?.Instance,
	        FromUri = origin,
	        ToUri = newUri,
	        Argument = argument,
	        Navigate = finalNavigateType,
	        WithAnimation = withAnimation
        };

        await _updateStrategy.UpdateChangesAsync(
            ShellView,
            stackChanges,
            finalNavigateType,
            argument,
            hasArgument,
            postArgument,
            cancellationToken);

        CheckWaitingList(stackChanges, argument, hasArgument);

        if (fromPage != null)
        {
	        await fromPage.OnNavigateAsync(postArgument, cancellationToken);
        }

        _navigating = false;
    }

    private void SetupPage(NavigationChain chain)
    {
	    if (chain.Instance is not Page page) return;

	    page.Shell = ShellView;
	    page.Chain = chain;
    }

    private async Task SwitchHostedItem(
        NavigationChain old,
        NavigationChain chain,
        bool withAnimation,
        CancellationToken cancellationToken = default)
    {
        var newUri =
            await _navigateStrategy.NavigateAsync(_stack.Current, CurrentUri, chain.Uri.AbsolutePath,
                cancellationToken);
        if (CurrentUri.AbsolutePath != newUri.AbsolutePath)
        {
            await NotifyAsync(
	            newUri,
	            newUri,
	            null,
	            false,
	            null,
	            NavigateType.HostedItemChange,
	            withAnimation,
	            cancellationToken);
        }
    }

    public Task NavigateAsync(string path, CancellationToken cancellationToken = default) =>
        NavigateAsync(path, null, null, false, null, true, cancellationToken);

    public Task NavigateAsync(string path, object? argument, CancellationToken cancellationToken = default) =>
        NavigateAsync(path, null, argument, true, null, true, cancellationToken);

    public Task NavigateAsync(
        string path,
        NavigateType? navigateType,
        CancellationToken cancellationToken = default) =>
        NavigateAsync(path, navigateType, null, false, null, true, cancellationToken);

    public Task NavigateAsync(
        string path,
        NavigateType? navigateType,
        object? argument,
        CancellationToken cancellationToken = default) =>
        NavigateAsync(path, navigateType, argument, true, null, true, cancellationToken);
    public Task NavigateAsync(
        string path,
        NavigateType? navigateType,
        object? sender,
        bool withAnimation = true,
        CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, navigateType, null, false, sender, withAnimation, cancellationToken);
    public Task NavigateAsync(
        string path,
        NavigateType? navigateType,
        object? argument,
        object? sender,
        bool withAnimation,
        CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, navigateType, argument, true, sender, withAnimation, cancellationToken);

    private async Task NavigateAsync(
        string path,
        NavigateType? navigateType,
        object? argument,
        bool hasArgument,
        object? sender,
        bool withAnimation,
        CancellationToken cancellationToken = default)
    {
        var originalUri = new Uri(CurrentUri, path);
        var newUri = await _navigateStrategy.NavigateAsync(_stack.Current, CurrentUri, path, cancellationToken);
        if (CurrentUri.AbsolutePath != newUri.AbsolutePath)
            await NotifyAsync(
	            originalUri,
	            newUri,
	            argument,
	            hasArgument,
	            sender,
	            navigateType,
	            withAnimation,
	            cancellationToken);
    }

    public Task BackAsync(CancellationToken cancellationToken = default) =>
        BackAsync(null, false, null, true, cancellationToken);
    public Task BackAsync(object? argument, CancellationToken cancellationToken = default) =>
        BackAsync(argument, true, null, true, cancellationToken);
    public Task BackAsync(
        object? sender,
        bool withAnimation = true,
        CancellationToken cancellationToken = default) => BackAsync(null, false, sender, withAnimation, cancellationToken);
    public Task BackAsync(
        object? argument,
        object? sender,
        bool withAnimation,
        CancellationToken cancellationToken = default) => BackAsync(argument, true, sender, withAnimation, cancellationToken);

    private async Task BackAsync(
        object? argument,
        bool hasArgument,
        object? sender,
        bool withAnimation,
        CancellationToken cancellationToken = default)
    {
        var newUri = await _navigateStrategy.BackAsync(_stack.Current, CurrentUri, cancellationToken);
        if (newUri != null && CurrentUri.AbsolutePath != newUri.AbsolutePath)
            await NotifyAsync(
	            newUri,
	            newUri,
	            argument,
	            hasArgument,
	            sender,
	            NavigateType.Pop,
	            withAnimation,
	            cancellationToken);
    }

    public Task<NavigateResult> NavigateAndWaitAsync(string path, CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, null, null, false, null, true, cancellationToken);

    public Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        object? argument,
        CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, null, argument, true, null, true, cancellationToken);

    public Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        NavigateType navigateType,
        CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, navigateType, null, false, null, true, cancellationToken);

    public Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        object? argument,
        NavigateType navigateType,
        CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, navigateType, argument, true, null, true, cancellationToken);
    public Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        object? sender,
        NavigateType navigateType,
        bool withAnimation = true,
        CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, navigateType, null, false, sender, withAnimation, cancellationToken);
    public Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        object? argument,
        object? sender,
        NavigateType navigateType,
        bool withAnimation,
        CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, navigateType, argument, true, sender, withAnimation, cancellationToken);

    private async Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        NavigateType? navigateType,
        object? argument,
        bool hasArgument,
        object? sender,
        bool withAnimation,
        CancellationToken cancellationToken = default)
    {
        var originalUri = new Uri(CurrentUri, path);
        var newUri = await _navigateStrategy.NavigateAsync(_stack.Current, CurrentUri, path, cancellationToken);
        if (CurrentUri.AbsolutePath == newUri.AbsolutePath)
            return new NavigateResult(false, null); // Or maybe we should throw exception.

        await NotifyAsync(
	        originalUri,
	        newUri,
	        argument,
	        hasArgument,
	        sender,
	        navigateType,
	        withAnimation,
	        cancellationToken);
        var chain = _stack.Current!;

        if (!_waitingList.TryGetValue(chain, out var tcs))
            _waitingList[chain] = tcs = new TaskCompletionSource<NavigateResult>();

        try
        {
            return await tcs.Task;
        }
        finally
        {
            _waitingList.Remove(chain);
        }
    }

    private void CheckWaitingList(
        NavigationStackChanges navigationStackChanges,
        object? argument,
        bool hasArgument)
    {
        if (navigationStackChanges.Removed == null) return;
        foreach (var chain in navigationStackChanges.Removed)
        {
            if (_waitingList.TryGetValue(chain, out var tcs))
                tcs.TrySetResult(new NavigateResult(hasArgument, argument));
        }
    }

    private void UpdateStrategyOnHostItemChanged(object? sender, HostItemChangeEventArgs e)
    {
        if (e.OldChain != null && e.NewChain != e.OldChain && !_navigating)
        {
            _ = SwitchHostedItem(e.OldChain, e.NewChain, true);
        }
    }
}
