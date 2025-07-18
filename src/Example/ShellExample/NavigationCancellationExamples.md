# Navigation Cancellation Examples

This document describes the navigation cancellation examples added to the ShellExample project.

## Overview

The navigation cancellation functionality allows pages to cancel their own navigation during the `InitializeAsync` lifecycle method. This is useful for scenarios where a page determines it shouldn't be displayed based on certain conditions.

## Example Pages

### 1. NavigateCancelledPage

**File**: `Views/NavigateCancelledPage.axaml[.cs]`
**Route**: `/navigate-cancelled`

This page demonstrates the simplest form of navigation cancellation:

- During `InitializeAsync`, it calls `Navigator.BackAsync()`
- This cancels the current navigation and returns to the previous page
- The page should never be visible to the user

**Key Implementation**:
```csharp
public override async Task InitialiseAsync(CancellationToken cancellationToken)
{
    if (Navigator != null)
    {
        await Navigator.BackAsync(); // Cancels the current navigation
    }
    await base.InitialiseAsync(cancellationToken);
}
```

### 2. NavigateAndWaitCancelledPage

**File**: `Views/NavigateAndWaitCancelledPage.axaml[.cs]`
**Route**: `/navigate-and-wait-cancelled`

This page demonstrates nested `NavigateAndWaitAsync` cancellation:

- During `InitializeAsync`, it calls `NavigateAndWaitAsync` to navigate to a helper page
- The helper page calls `Navigator.BackAsync()` which cancels the entire chain
- Neither page should be visible to the user

**Key Implementation**:
```csharp
public override async Task InitialiseAsync(CancellationToken cancellationToken)
{
    if (Navigator != null)
    {
        // This call will be cancelled by the helper page
        var result = await Navigator.NavigateAndWaitAsync("/cancellation-helper");
    }
    await base.InitialiseAsync(cancellationToken);
}
```

### 3. CancellationHelperPage

**File**: `Views/CancellationHelperPage.axaml[.cs]`
**Route**: `/cancellation-helper`

This helper page is used by `NavigateAndWaitCancelledPage`:

- During `InitializeAsync`, it immediately calls `Navigator.BackAsync()`
- This cancels the entire `NavigateAndWaitAsync` chain
- Demonstrates how cancellation propagates through nested navigation calls

### 4. NavigationCancellationDemoPage

**File**: `Views/NavigationCancellationDemoPage.axaml[.cs]`
**Route**: `/cancellation-demo`

This is the main demo page that:

- Explains the cancellation scenarios
- Provides buttons to test each cancellation type
- Shows implementation details and expected behavior
- Is accessible from the home page and side menu

## How to Test

1. **Run the ShellExample project**
2. **Navigate to the demo page** via:
   - Home page → "Navigation Cancellation Demo" button
   - Side menu → "Navigation Cancellation Demo"
3. **Test NavigateAsync cancellation**:
   - Click "Test NavigateAsync Cancellation"
   - The page should immediately return to the demo page
   - Check Debug output for cancellation flow
4. **Test NavigateAndWaitAsync cancellation**:
   - Click "Test NavigateAndWaitAsync Cancellation"
   - The page should immediately return to the demo page
   - Check Debug output for nested cancellation flow

## Expected Behavior

- **Target pages should never be visible** - they cancel before appearing
- **Navigation should return to the previous page** immediately
- **No exceptions should be thrown** - cancellation is handled gracefully
- **Debug output should show the cancellation flow** with detailed logging

## Debug Output

Each cancellation scenario produces detailed debug output showing:

- When `InitializeAsync` is called
- When `Navigator.BackAsync()` is called
- When cancellation completes
- Whether `AppearAsync` is called (it shouldn't be for cancelled navigations)

## Implementation Notes

The cancellation functionality uses:

- **AsyncLocal stack management** for nested `NavigateAndWaitAsync` calls
- **Thread-safe ConcurrentDictionary** for chain-specific cancellation tokens
- **Proper resource cleanup** when navigations are cancelled
- **OperationCanceledException** handling for clean cancellation flow

## Navigation Routes Added

The following routes were added to `MainView.axaml`:

```xml
<Route Path="cancellation-demo" Page="views:NavigationCancellationDemoPage"></Route>
<Route Path="navigate-cancelled" Page="views:NavigateCancelledPage"></Route>
<Route Path="navigate-and-wait-cancelled" Page="views:NavigateAndWaitCancelledPage"></Route>
<Route Path="cancellation-helper" Page="views:CancellationHelperPage"></Route>
```

## UI Updates

- Added side menu item for "Navigation Cancellation Demo"
- Added button on home page linking to the demo
- Added tooltips explaining the functionality