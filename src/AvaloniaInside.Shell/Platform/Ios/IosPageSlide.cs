using Avalonia.Animation.Easings;
using Avalonia.Animation;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.VisualTree;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;

namespace AvaloniaInside.Shell.Platform.Ios;

public class DefaultIosPageSlide : IPageTransition
{
    private CompositionAnimationGroup? _enteranceAnimation;
    private CompositionAnimationGroup? _exitAnimation;
    private CompositionAnimationGroup? _sendBackAnimation;
    private CompositionAnimationGroup? _bringBackAnimation;

    private double _lastDistance = 0;

    /// <summary>
    /// Gets the duration of the animation.
    /// </summary>
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(.25);

    /// <summary>
    /// Gets or sets element entrance easing.
    /// </summary>
    public Easing SlideInEasing { get; set; } = Easing.Parse("0.42, 0.0, 0.58, 1.0");

    private CompositionAnimationGroup GetOrCreateEnteranceAnimation(CompositionVisual element, double distance)
    {
        if (_enteranceAnimation != null) return _enteranceAnimation;

        var compositor = element.Compositor;
        var easing = Easing.Parse("0.42, 0.0, 0.58, 1.0");

        var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
        offsetAnimation.Duration = Duration;
        offsetAnimation.Target = nameof(element.Offset);
        offsetAnimation.InsertKeyFrame(0f, new Vector3D(distance, 0, 0), easing);
        offsetAnimation.InsertKeyFrame(1.0f, new Vector3D(0, 0, 0), easing);

        var littleFadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        littleFadeAnimation.Duration = Duration;
        littleFadeAnimation.Target = nameof(element.Opacity);
        littleFadeAnimation.InsertKeyFrame(0f, 0.9f);
        littleFadeAnimation.InsertKeyFrame(0.5f, 1f);

        _enteranceAnimation = compositor.CreateAnimationGroup();
        _enteranceAnimation.Add(offsetAnimation);
        _enteranceAnimation.Add(littleFadeAnimation);
        return _enteranceAnimation;
    }

    private CompositionAnimationGroup GetOrCreateExitAnimation(CompositionVisual element, double distance)
    {
        if (_exitAnimation != null) return _exitAnimation;

        var compositor = element.Compositor;
        var easing = Easing.Parse("0.42, 0.0, 0.58, 1.0");

        var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
        offsetAnimation.Duration = Duration;
        offsetAnimation.Target = nameof(element.Offset);
        offsetAnimation.InsertKeyFrame(0f, new Vector3D(0, 0, 0), easing);
        offsetAnimation.InsertKeyFrame(1.0f, new Vector3D(distance, 0, 0), easing);

        var littleFadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        littleFadeAnimation.Duration = Duration;
        littleFadeAnimation.Target = nameof(element.Opacity);
        littleFadeAnimation.InsertKeyFrame(.5f, 1f);
        littleFadeAnimation.InsertKeyFrame(1f, 0.9f);

        _exitAnimation = compositor.CreateAnimationGroup();
        _exitAnimation.Add(offsetAnimation);
        _exitAnimation.Add(littleFadeAnimation);
        return _exitAnimation;
    }

    private CompositionAnimationGroup GetOrCreateSendBackAnimation(CompositionVisual element, double distance)
    {
        if (_sendBackAnimation != null) return _sendBackAnimation;

        var compositor = element.Compositor;
        var easing = Easing.Parse("0.42, 0.0, 0.58, 1.0");

        var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
        offsetAnimation.Duration = Duration;
        offsetAnimation.Target = nameof(element.Offset);
        offsetAnimation.InsertKeyFrame(0f, new Vector3D(0, 0, 0), easing);
        offsetAnimation.InsertKeyFrame(1.0f, new Vector3D(distance / -4d, 0, 0), easing);

        var littleFadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        littleFadeAnimation.Duration = Duration;
        littleFadeAnimation.Target = nameof(element.Opacity);
        littleFadeAnimation.InsertKeyFrame(0f, 1f);
        littleFadeAnimation.InsertKeyFrame(1f, .9f);

        _sendBackAnimation = compositor.CreateAnimationGroup();
        _sendBackAnimation.Add(offsetAnimation);
        _sendBackAnimation.Add(littleFadeAnimation);
        return _sendBackAnimation;
    }

    private CompositionAnimationGroup GetOrCreateBringBackAnimation(CompositionVisual element, double distance)
    {
        if (_bringBackAnimation != null) return _bringBackAnimation;

        var compositor = element.Compositor;
        var easing = Easing.Parse("0.42, 0.0, 0.58, 1.0");

        var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
        offsetAnimation.Duration = Duration;
        offsetAnimation.Target = nameof(element.Offset);
        offsetAnimation.InsertKeyFrame(0f, new Vector3D(distance / -4d, 0, 0), easing);
        offsetAnimation.InsertKeyFrame(1.0f, new Vector3D(0, 0, 0), easing);

        var littleFadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        littleFadeAnimation.Duration = Duration;
        littleFadeAnimation.Target = nameof(element.Opacity);
        littleFadeAnimation.InsertKeyFrame(0f, .9f);
        littleFadeAnimation.InsertKeyFrame(1f, 1f);

        _bringBackAnimation = compositor.CreateAnimationGroup();
        _bringBackAnimation.Add(offsetAnimation);
        _bringBackAnimation.Add(littleFadeAnimation);
        return _bringBackAnimation;
    }

    public async Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        var tasks = new List<Task>();
        var parent = GetVisualParent(from, to);
        var parentComposition = ElementComposition.GetElementVisual(parent)!;

        var distance = parent.Bounds.Width;

        if (distance != _lastDistance)
        {
            _enteranceAnimation = null;
            _exitAnimation = null;
            _sendBackAnimation = null;
            _bringBackAnimation = null;
        }

        if (to != null)
        {
            var toElement = ElementComposition.GetElementVisual(to)!;
            var animation = forward 
                ? GetOrCreateEnteranceAnimation(parentComposition, distance) 
                : GetOrCreateBringBackAnimation(parentComposition, distance);

            toElement.StartAnimationGroup(animation);
        }

        if (from != null)
        {
            var fromElement = ElementComposition.GetElementVisual(from)!;
            var animation = forward
               ? GetOrCreateSendBackAnimation(parentComposition, distance)
               : GetOrCreateExitAnimation(parentComposition, distance);

            fromElement.StartAnimationGroup(animation);
        }

        await Task.Run(() => Task.Delay(Duration, cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Gets the common visual parent of the two control.
    /// </summary>
    /// <param name="from">The from control.</param>
    /// <param name="to">The to control.</param>
    /// <returns>The common parent.</returns>
    /// <exception cref="ArgumentException">
    /// The two controls do not share a common parent.
    /// </exception>
    /// <remarks>
    /// Any one of the parameters may be null, but not both.
    /// </remarks>
    protected static Visual GetVisualParent(Visual? from, Visual? to)
    {
        var p1 = (from ?? to)!.GetVisualParent();
        var p2 = (to ?? from)!.GetVisualParent();

        if (p1 != null && p2 != null && p1 != p2)
        {
            throw new ArgumentException("Controls for PageSlide must have same parent.");
        }

        return p1 ?? throw new InvalidOperationException("Cannot determine visual parent.");
    }
}
