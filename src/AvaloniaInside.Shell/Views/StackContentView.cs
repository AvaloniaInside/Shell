using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace AvaloniaInside.Shell.Views;

public class StackContentView : TemplatedControl
{
	private readonly SemaphoreSlim _semaphoreSlim = new (1, 1);
	private readonly Stack<object> _controls = new();
	private ContentPresenter? _contentPresenter;
	private object? _pendingView;

	public StackContentView()
	{
		Template = new FuncControlTemplate((parent, scope) =>
			new ContentPresenter
			{
				Name = "PART_ContentPresenter"
			}.RegisterInNameScope(scope));
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_contentPresenter = e.NameScope.Find<ContentPresenter>("PART_ContentPresenter");
		_contentPresenter!.Content = _pendingView;
	}

	public object? CurrentView => _contentPresenter?.Content ?? _pendingView;

	public bool IsExistsInStack(object view) => _controls.Contains(view);

	public async Task PushViewAsync(object view, CancellationToken cancellationToken = default)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);
		try
		{
			var currentView = _contentPresenter?.Content ?? _pendingView;
			if (currentView != null)
				_controls.Push(currentView);

			if (_contentPresenter != null)
				_contentPresenter!.Content = view;
			else
				_pendingView = view;
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	public async Task<bool> BackAsync(CancellationToken cancellationToken)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);
		try
		{
			if (_controls.Count == 0) return false;
			_contentPresenter!.Content = _controls.Pop();
			return true;
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}
}
