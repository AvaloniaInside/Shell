using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AvaloniaInside.Shell.BasicTests;

[TestFixture]
public class NavigationCancellationConceptTests
{
    [Test]
    public void CancellationTokenSource_WhenCancelled_ShouldBeCancelled()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        
        // Act
        cts.Cancel();
        
        // Assert
        Assert.That(cts.Token.IsCancellationRequested, Is.True);
    }

    [Test]
    public async Task AsyncLocal_Stack_ShouldMaintainSeparateContexts()
    {
        // Arrange
        var asyncLocal = new AsyncLocal<Stack<int>>();
        asyncLocal.Value = new Stack<int>();
        asyncLocal.Value.Push(1);
        
        // Act & Assert
        var task1 = Task.Run(() =>
        {
            asyncLocal.Value ??= new Stack<int>();
            asyncLocal.Value.Push(10);
            return asyncLocal.Value.Count;
        });
        
        var task2 = Task.Run(() =>
        {
            asyncLocal.Value ??= new Stack<int>();
            asyncLocal.Value.Push(20);
            return asyncLocal.Value.Count;
        });
        
        var results = await Task.WhenAll(task1, task2);
        
        Assert.That(results[0], Is.EqualTo(1));
        Assert.That(results[1], Is.EqualTo(1));
        Assert.That(asyncLocal.Value.Count, Is.EqualTo(1)); // Original context unchanged
    }

    [Test]
    public async Task OperationCanceledException_ShouldBeThrown_WhenTokenCancelled()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        
        // Act & Assert
        var task = Task.Run(async () =>
        {
            await Task.Delay(100, cts.Token);
        });
        
        cts.Cancel();
        
        Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
    }

    [Test]
    public async Task LinkedCancellationToken_ShouldCancelWhenParentCancels()
    {
        // Arrange
        using var parentCts = new CancellationTokenSource();
        using var childCts = CancellationTokenSource.CreateLinkedTokenSource(parentCts.Token);
        
        // Act
        parentCts.Cancel();
        
        // Assert
        Assert.That(childCts.Token.IsCancellationRequested, Is.True);
        
        // Verify that operations using the child token are cancelled
        Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await Task.Delay(100, childCts.Token);
        });
    }

    [Test]
    public async Task TaskWhenAny_ShouldCompleteWhenFirstTaskCompletes()
    {
        // Arrange
        var tcs1 = new TaskCompletionSource<int>();
        var tcs2 = new TaskCompletionSource<int>();
        
        // Act
        var firstCompleted = Task.WhenAny(tcs1.Task, tcs2.Task);
        tcs1.SetResult(42);
        
        var result = await firstCompleted;
        
        // Assert
        Assert.That(result, Is.EqualTo(tcs1.Task));
        Assert.That(await result, Is.EqualTo(42));
        Assert.That(tcs2.Task.IsCompleted, Is.False);
    }

    [Test]
    public void ConcurrentDictionary_ShouldBeThreadSafe()
    {
        // Arrange
        var dict = new ConcurrentDictionary<string, CancellationTokenSource>();
        var tasks = new Task[10];
        
        // Act
        for (int i = 0; i < 10; i++)
        {
            var index = i;
            tasks[i] = Task.Run(() =>
            {
                var cts = new CancellationTokenSource();
                dict.TryAdd($"key{index}", cts);
                return dict.TryRemove($"key{index}", out _);
            });
        }
        
        Task.WaitAll(tasks);
        
        // Assert
        Assert.That(dict.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task NestedCancellationTokens_ShouldPropagateCorrectly()
    {
        // Arrange
        using var outerCts = new CancellationTokenSource();
        using var middleCts = CancellationTokenSource.CreateLinkedTokenSource(outerCts.Token);
        using var innerCts = CancellationTokenSource.CreateLinkedTokenSource(middleCts.Token);
        
        // Act
        outerCts.Cancel();
        
        // Assert
        Assert.That(middleCts.Token.IsCancellationRequested, Is.True);
        Assert.That(innerCts.Token.IsCancellationRequested, Is.True);
        
        // Verify that all levels are cancelled
        Assert.ThrowsAsync<OperationCanceledException>(async () => await Task.Delay(1, outerCts.Token));
        Assert.ThrowsAsync<OperationCanceledException>(async () => await Task.Delay(1, middleCts.Token));
        Assert.ThrowsAsync<OperationCanceledException>(async () => await Task.Delay(1, innerCts.Token));
    }

    [Test]
    public async Task AsyncLocalStack_SimulatesNavigationStack()
    {
        // This test simulates how the navigation cancellation would work
        var navigationStack = new AsyncLocal<Stack<CancellationTokenSource>>();
        
        async Task SimulateNestedNavigation(int level)
        {
            navigationStack.Value ??= new Stack<CancellationTokenSource>();
            
            using var cts = new CancellationTokenSource();
            navigationStack.Value.Push(cts);
            
            try
            {
                if (level > 0)
                {
                    // Simulate nested navigation
                    await SimulateNestedNavigation(level - 1);
                }
                else
                {
                    // Simulate the deepest level cancelling all
                    while (navigationStack.Value.Count > 0)
                    {
                        var cancelCts = navigationStack.Value.Pop();
                        cancelCts.Cancel();
                    }
                }
            }
            finally
            {
                // Clean up
                if (navigationStack.Value.Count > 0 && navigationStack.Value.Peek() == cts)
                {
                    navigationStack.Value.Pop();
                }
            }
        }
        
        // Act & Assert
        Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await SimulateNestedNavigation(3);
        });
    }
}