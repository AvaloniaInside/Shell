using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;

namespace AvaloniaInside.Shell;

public partial class NavigationView : TemplatedControl
{
	private readonly SemaphoreSlim _semaphoreSlim = new (1, 1);
	private StackContentView? _header;
	private object? _pendingHeader;

	public NavigationView()
	{
		InitializeComponent();
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);

		_header = e.NameScope.Find<StackContentView>("PART_Header") ?? throw new ArgumentNullException("PART_Header");
		if (_pendingHeader != null)
			_header?.PushViewAsync(_pendingHeader);
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public async Task PushNavigationStage(INavigation navigation, CancellationToken cancellationToken = default)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);
		try
		{
			if (_header != null)
				await _header.PushViewAsync(navigation.Header, cancellationToken);
			else
				_pendingHeader = navigation.Header;
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	public async Task BackNavigationStage(CancellationToken cancellationToken = default)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);
		try
		{
			await _header!.BackAsync(cancellationToken);
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}
}

