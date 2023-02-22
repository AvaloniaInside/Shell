using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace AvaloniaInside.Shell;

public class StackContentView : TemplatedControl
{
	private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
	private readonly List<object> _controls = new();
	private IContentControl? _contentPresenter;
	private object? _pendingView;

	public static readonly StyledProperty<bool> HasContentProperty =
		AvaloniaProperty.Register<Border, bool>(nameof(HasContent));

	public bool HasContent
	{
		get => GetValue(HasContentProperty);
		private set => SetValue(HasContentProperty, value);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_contentPresenter = e.NameScope.Find<IContentControl>("PART_ContentPresenter");
		_contentPresenter!.Content = _pendingView;
	}

	public object? CurrentView => _controls.LastOrDefault();

	public async Task PushViewAsync(object view, CancellationToken cancellationToken = default)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);
		try
		{
			var currentView = _contentPresenter?.Content ?? _pendingView;
			if (currentView == view) return;

			if (_controls.Contains(view))
				_controls.Remove(view);

			_controls.Add(view);

			if (_contentPresenter != null)
				_contentPresenter!.Content = view;
			else
				_pendingView = view;

			await OnContentUpdateAsync(CurrentView, cancellationToken);
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	public async Task<bool> RemoveViewAsync(object view, CancellationToken cancellationToken)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);
		try
		{
			if (!_controls.Contains(view)) return false;
			var currentView = CurrentView;

			_controls.Remove(view);
			if (view == currentView && _contentPresenter != null)
				_contentPresenter.Content = CurrentView;

			await OnContentUpdateAsync(CurrentView, cancellationToken);
			return true;
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	// public async Task<bool> BackAsync(CancellationToken cancellationToken)
	// {
	// 	await _semaphoreSlim.WaitAsync(cancellationToken);
	// 	try
	// 	{
	// 		if (_controls.Count <= 1) return false;
	// 		_controls.RemoveAt(_controls.Count - 1);
	// 		_contentPresenter!.Content = _controls.Last();
	//
	// 		await OnContentUpdateAsync(CurrentView, cancellationToken);
	// 		return true;
	// 	}
	// 	finally
	// 	{
	// 		_semaphoreSlim.Release();
	// 	}
	// }

	protected virtual Task OnContentUpdateAsync(object? view, CancellationToken cancellationToken)
	{
		HasContent = _controls.Count > 0;
		return Task.CompletedTask;
	}

	public Task ClearStackAsync(CancellationToken cancellationToken)
	{
		_controls.RemoveRange(0, _controls.Count - 1);
		return Task.CompletedTask;
	}
}
