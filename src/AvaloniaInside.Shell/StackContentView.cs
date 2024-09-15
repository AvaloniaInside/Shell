using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using AvaloniaInside.Shell.Platform;

namespace AvaloniaInside.Shell;

public class StackContentView : Panel
{
	private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

	#region HasContent

	public static readonly StyledProperty<bool> HasContentProperty =
		AvaloniaProperty.Register<Border, bool>(nameof(HasContent));

	public bool HasContent
	{
		get => GetValue(HasContentProperty);
		private set => SetValue(HasContentProperty, value);
	}

	#endregion

	#region PageTransition

	/// <summary>
	/// Defines the <see cref="PageTransition"/> property.
	/// </summary>
	public static readonly StyledProperty<IPageTransition?> PageTransitionProperty =
		AvaloniaProperty.Register<StackContentView, IPageTransition?>(
			nameof(PageTransition),
			defaultValue: PlatformSetup.TransitionForPage);

	/// <summary>
	/// Gets or sets the animation played when content appears and disappears.
	/// </summary>
	public IPageTransition? PageTransition
	{
		get => GetValue(PageTransitionProperty);
		set => SetValue(PageTransitionProperty, value);
	}

	#endregion

	#region CurrentView

	public static readonly DirectProperty<StackContentView, object?> CurrentViewProperty =
		AvaloniaProperty.RegisterDirect<StackContentView, object?>(
			nameof(CurrentView),
			o => o.Children.LastOrDefault());

	public object? CurrentView => Children.LastOrDefault();

	#endregion

	public async Task PushViewAsync(
		object view,
		NavigateType navigateType,
		NavigateEventArgs eventArgs,
		CancellationToken cancellationToken = default)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);
		try
		{
			var current = CurrentView;

			if (current != null && current == view) return;
			if (view is not Control control) return;

			// Bring to front if exists in collection
			if (Children.Contains(control))
				Children.Remove(control);
			Children.Add(control);

			await OnContentUpdateAsync(control, cancellationToken);
			await UpdateCurrentViewAsync(current, control, navigateType, false, eventArgs, cancellationToken);

			RaisePropertyChanged(CurrentViewProperty, current, CurrentView);
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	protected virtual async Task UpdateCurrentViewAsync(
		object? from,
		object? to,
		NavigateType navigateType,
		bool removed,
		NavigateEventArgs eventArgs,
		CancellationToken cancellationToken)
	{
		if (eventArgs.WithAnimation && PageTransition is {} transition)
		{
			await transition.Start(
				from as Visual,
				to as Visual,
				!removed,
				cancellationToken);
		}
		else
		{
			if (from is Visual f) f.IsVisible = false;
			if (to is Visual t) t.IsVisible = true;
		}

		//TODO: Store transition
	}

	public async Task<bool> RemoveViewAsync(
		object view,
		NavigateType navigateType,
		NavigateEventArgs eventArgs,
		CancellationToken cancellationToken)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);
		try
		{
			if (!Children.Contains(view)) return false;

			var current = CurrentView;
			if (CurrentView == view)
			{
				var to = Children.Count > 1 ? Children[^2] : null;
				await UpdateCurrentViewAsync(view, to, navigateType, true, eventArgs, cancellationToken);
			}

			if (view is Control control)
				Children.Remove(control);

			await OnContentUpdateAsync(CurrentView, cancellationToken);

			RaisePropertyChanged(CurrentViewProperty, current, CurrentView);

			return true;
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	protected virtual Task OnContentUpdateAsync(object? view, CancellationToken cancellationToken)
	{
		HasContent = Children.Count > 0;
		return Task.CompletedTask;
	}

	public Task ClearStackAsync(CancellationToken cancellationToken)
	{
		var current = CurrentView;
		while (Children.Count > 1)
			Children.RemoveAt(0);

		RaisePropertyChanged(CurrentViewProperty, current, CurrentView);

		return Task.CompletedTask;
	}
}
