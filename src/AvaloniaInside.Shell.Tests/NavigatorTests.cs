using AvaloniaInside.Shell;
using Avalonia.Animation;

namespace AvaloniaInside.Shell.Tests;

public class NavigatorTests : IDisposable
{
    private readonly Mock<INavigationRegistrar> _mockRegistrar;
    private readonly Mock<INavigateStrategy> _mockNavigateStrategy;
    private readonly Mock<INavigationUpdateStrategy> _mockUpdateStrategy;
    private readonly Mock<INavigationViewLocator> _mockViewLocator;
    private readonly Navigator _navigator;
    private readonly ShellView _shellView;

    public NavigatorTests()
    {
        _mockRegistrar = new Mock<INavigationRegistrar>();
        _mockNavigateStrategy = new Mock<INavigateStrategy>();
        _mockUpdateStrategy = new Mock<INavigationUpdateStrategy>();
        _mockViewLocator = new Mock<INavigationViewLocator>();

        _navigator = new Navigator(
            _mockRegistrar.Object,
            _mockNavigateStrategy.Object,
            _mockUpdateStrategy.Object,
            _mockViewLocator.Object);

        _shellView = new ShellView(_navigator);

        SetupBasicMocks();
    }

    private void SetupBasicMocks()
    {
        _mockRegistrar.Setup(r => r.RootUri).Returns(new Uri("app://root"));
        _mockRegistrar.Setup(r => r.TryGetNode(It.IsAny<string>(), out It.Ref<NavigationNode>.IsAny))
            .Returns((string path, out NavigationNode node) =>
            {
                node = CreateMockNavigationNode(path);
                return true;
            });

        _mockNavigateStrategy.Setup(s => s.NavigateAsync(
                It.IsAny<NavigationChain>(), 
                It.IsAny<Uri>(), 
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Uri("app://root/test"));

        _mockNavigateStrategy.Setup(s => s.BackAsync(
                It.IsAny<NavigationChain>(), 
                It.IsAny<Uri>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Uri("app://root"));

        _mockUpdateStrategy.Setup(s => s.UpdateChangesAsync(
                It.IsAny<ShellView>(),
                It.IsAny<NavigationStackChanges>(),
                It.IsAny<NavigateType>(),
                It.IsAny<object>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    private NavigationNode CreateMockNavigationNode(string path)
    {
        return new NavigationNode(path, typeof(Page), NavigationNodeType.Page, NavigateType.Normal, "");
    }

    public void Dispose()
    {

    }

    [Fact]
    public void Constructor_ShouldSetupPropertiesCorrectly()
    {
        // Act & Assert
        _navigator.Registrar.ShouldBe(_mockRegistrar.Object);
        _navigator.CurrentUri.ShouldBe(new Uri("app://root"));
    }

    [Fact]
    public void RegisterShell_WhenCalledTwice_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() => _navigator.RegisterShell(_shellView));
        exception.Message.ShouldContain("Register shell can call only once");
    }

    [Fact]
    public async Task NavigateAsync_WithPath_ShouldCallNavigateStrategy()
    {
        // Arrange

        // Act
        await _navigator.NavigateAsync("/test");

        // Assert
        _mockNavigateStrategy.Verify(s => s.NavigateAsync(
            It.IsAny<NavigationChain>(),
            It.IsAny<Uri>(),
            "/test",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task NavigateAsync_WithPathAndArgument_ShouldCallNavigateStrategy()
    {
        // Arrange
        var argument = new { test = "value" };

        // Act
        await _navigator.NavigateAsync("/test", argument);

        // Assert
        _mockNavigateStrategy.Verify(s => s.NavigateAsync(
            It.IsAny<NavigationChain>(),
            It.IsAny<Uri>(),
            "/test",
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUpdateStrategy.Verify(s => s.UpdateChangesAsync(
            _shellView,
            It.IsAny<NavigationStackChanges>(),
            It.IsAny<NavigateType>(),
            argument,
            true,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task NavigateAsync_WithNavigateType_ShouldCallNavigateStrategy()
    {
        // Arrange
        

        // Act
        await _navigator.NavigateAsync("/test", NavigateType.Normal);

        // Assert
        _mockNavigateStrategy.Verify(s => s.NavigateAsync(
            It.IsAny<NavigationChain>(),
            It.IsAny<Uri>(),
            "/test",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task NavigateAsync_WithCancellationToken_WhenCancelled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        
        using var cts = new CancellationTokenSource();
        
        _mockNavigateStrategy.Setup(s => s.NavigateAsync(
                It.IsAny<NavigationChain>(),
                It.IsAny<Uri>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(async (NavigationChain chain, Uri uri, string path, CancellationToken ct) =>
            {
                await Task.Delay(100, ct);
                return new Uri("app://test");
            });

        // Act
        var task = _navigator.NavigateAsync("/test", cts.Token);
        cts.Cancel();

        // Assert
        await Should.ThrowAsync<OperationCanceledException>(task);
    }

    [Fact]
    public async Task BackAsync_ShouldCallBackStrategy()
    {
        // Arrange
        

        // Act
        await _navigator.BackAsync();

        // Assert
        _mockNavigateStrategy.Verify(s => s.BackAsync(
            It.IsAny<NavigationChain>(),
            It.IsAny<Uri>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BackAsync_WithArgument_ShouldCallBackStrategy()
    {
        // Arrange
        
        var argument = new { result = "success" };

        // Act
        await _navigator.BackAsync(argument, CancellationToken.None);

        // Assert
        _mockNavigateStrategy.Verify(s => s.BackAsync(
            It.IsAny<NavigationChain>(),
            It.IsAny<Uri>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task NavigateAndWaitAsync_WithCancellation_ShouldThrowOperationCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        _mockUpdateStrategy.Setup(s => s.UpdateChangesAsync(
                It.IsAny<ShellView>(),
                It.IsAny<NavigationStackChanges>(),
                It.IsAny<NavigateType>(),
                It.IsAny<object>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns((ShellView s, NavigationStackChanges n, NavigateType t, object o, bool b, CancellationToken ct) =>
            {
                return cts.CancelAsync();
            });

        _mockNavigateStrategy.Setup(s => s.NavigateAsync(
                It.IsAny<NavigationChain>(),
                It.IsAny<Uri>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Uri("app://root/test"));

        // Act
        var task = _navigator.NavigateAndWaitAsync("/test", cts.Token);

        // Assert
        await Should.ThrowAsync<OperationCanceledException>(task);
    }

    [Fact]
    public async Task BackAsync_ShouldCancelPendingNavigations()
    {
        // Arrange
        var navigationTcs = new TaskCompletionSource<Uri>();
        var navigationStarted = new TaskCompletionSource<bool>();

        _mockNavigateStrategy.Setup(s => s.NavigateAsync(
                It.IsAny<NavigationChain>(),
                It.IsAny<Uri>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(async (NavigationChain chain, Uri uri, string path, CancellationToken ct) =>
            {
                navigationStarted.SetResult(true);
                return await navigationTcs.Task.WaitAsync(ct);
            });

        // Act
        var navigationTask = _navigator.NavigateAsync("/test");
        await navigationStarted.Task; // Wait for navigation to start
        
        var backTask = _navigator.BackAsync();

        // Assert
        await Should.ThrowAsync<OperationCanceledException>(navigationTask);
        await backTask; // Should complete without exception
    }

    [Fact]
    public void HasItemInStack_WithEmptyStack_ShouldReturnFalse()
    {
        // Act & Assert
        _navigator.HasItemInStack().ShouldBeFalse();
    }

    [Fact]
    public async Task NavigateAsync_WithInvalidPath_WhenNodeNotFound_ShouldNotThrow()
    {
        // Arrange
        
        _mockRegistrar.Setup(r => r.TryGetNode(It.IsAny<string>(), out It.Ref<NavigationNode>.IsAny))
            .Returns((string path, out NavigationNode node) =>
            {
                node = null;
                return false;
            });

        // Act & Assert
        await Should.NotThrowAsync(() => _navigator.NavigateAsync("/invalid"));
    }

    [Fact]
    public async Task NavigateAsync_WithSameUri_ShouldNotCallUpdateStrategy()
    {
        // Arrange
        
        _mockNavigateStrategy.Setup(s => s.NavigateAsync(
                It.IsAny<NavigationChain>(),
                It.IsAny<Uri>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Uri("app://root")); // Same as current URI

        // Act
        await _navigator.NavigateAsync("/root");

        // Assert
        _mockUpdateStrategy.Verify(s => s.UpdateChangesAsync(
            It.IsAny<ShellView>(),
            It.IsAny<NavigationStackChanges>(),
            It.IsAny<NavigateType>(),
            It.IsAny<object>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}