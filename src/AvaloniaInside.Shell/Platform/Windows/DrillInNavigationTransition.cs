using Avalonia.Animation.Easings;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.Rendering.Composition;
using Avalonia;
using AvaloniaInside.Shell.Platform.Android;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Platform.Windows;
public class DrillInNavigationTransition : PlatformBasePageTransition
{
    private const float EndingCue = 0.7f;
    private const float StartingCue = 0.3f;

    /// <summary>
    /// Gets the duration of the animation.
    /// </summary>
    public override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0.33);

    /// <summary>
    /// Scale factor to animate in
    /// </summary>
    public float ZoomInFactor { get; set; } = 1.25f;

    /// <summary>
    /// Scale factor to animate out
    /// </summary>
    public float ZoomOutFactor { get; set; } = 0.75f;

    /// <summary>
    /// Gets or sets element entrance easing.
    /// </summary>
    public override Easing Easing { get; set; } = Easing.Parse("0.85, 0.0, 0.0, 1.0");

    protected override CompositionAnimationGroup GetOrCreateEnteranceAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
        var compositor = element.Compositor;

        var scaleAnimation = compositor.CreateVector3DKeyFrameAnimation();
        scaleAnimation.Duration = Duration;
        scaleAnimation.Target = nameof(element.Scale);
        scaleAnimation.InsertKeyFrame(StartingCue, new Vector3D(ZoomOutFactor, ZoomOutFactor, 1), Easing);
        scaleAnimation.InsertKeyFrame(1.0f, new Vector3D(1, 1, 1), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(StartingCue, 0f, Easing);
        fadeAnimation.InsertKeyFrame(1.0f, 1f, Easing);

        var enteranceAnimation = compositor.CreateAnimationGroup();
        enteranceAnimation.Add(scaleAnimation);
        enteranceAnimation.Add(fadeAnimation);
        return enteranceAnimation;
    }

    protected override CompositionAnimationGroup GetOrCreateExitAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
        var compositor = element.Compositor;

        var scaleAnimation = compositor.CreateVector3DKeyFrameAnimation();
        scaleAnimation.Duration = Duration;
        scaleAnimation.Target = nameof(element.Scale);
        scaleAnimation.InsertKeyFrame(0f, new Vector3D(1, 1, 1), Easing);
        scaleAnimation.InsertKeyFrame(EndingCue, new Vector3D(ZoomOutFactor, ZoomOutFactor, 1), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(0f, 1f, Easing);
        fadeAnimation.InsertKeyFrame(EndingCue, 0f, Easing);

        var exitAnimation = compositor.CreateAnimationGroup();
        exitAnimation.Add(scaleAnimation);
        exitAnimation.Add(fadeAnimation);
        return exitAnimation;
    }

    protected override CompositionAnimationGroup GetOrCreateSendBackAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
        var compositor = element.Compositor;

        var scaleAnimation = compositor.CreateVector3DKeyFrameAnimation();
        scaleAnimation.Duration = Duration;
        scaleAnimation.Target = nameof(element.Scale);
        scaleAnimation.InsertKeyFrame(0f, new Vector3D(1, 1, 1), Easing);
        scaleAnimation.InsertKeyFrame(EndingCue, new Vector3D(ZoomInFactor, ZoomInFactor, 1), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(0f, 1f, Easing);
        fadeAnimation.InsertKeyFrame(EndingCue, 0f, Easing);

        var sendBackAnimation = compositor.CreateAnimationGroup();
        sendBackAnimation.Add(scaleAnimation);
        sendBackAnimation.Add(fadeAnimation);
        return sendBackAnimation;
    }

    protected override CompositionAnimationGroup GetOrCreateBringBackAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
        var compositor = element.Compositor;

        var scaleAnimation = compositor.CreateVector3DKeyFrameAnimation();
        scaleAnimation.Duration = Duration;
        scaleAnimation.Target = nameof(element.Scale);
        scaleAnimation.InsertKeyFrame(StartingCue, new Vector3D(ZoomInFactor, ZoomInFactor, 1), Easing);
        scaleAnimation.InsertKeyFrame(1f, new Vector3D(1, 1, 1), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(StartingCue, 0f, Easing);
        fadeAnimation.InsertKeyFrame(1f, 1f, Easing);

        var bringBackAnimation = compositor.CreateAnimationGroup();
        bringBackAnimation.Add(scaleAnimation);
        bringBackAnimation.Add(fadeAnimation);
        return bringBackAnimation;
    }

    protected override Task RunAnimationAsync(
        CompositionVisual parentComposition,
        CompositionVisual? fromElement,
        CompositionVisual? toElement,
        bool forward,
        double distance,
        double heightDistance,
        CancellationToken cancellationToken)
    {
        if (toElement != null)
            toElement.CenterPoint = new Vector3D(parentComposition.Size.X * 0.5, parentComposition.Size.Y * 0.5, 0);

        if (fromElement != null)
            fromElement.CenterPoint = new Vector3D(parentComposition.Size.X * 0.5, parentComposition.Size.Y * 0.5, 0);

        return base.RunAnimationAsync(parentComposition, fromElement, toElement, forward, distance, heightDistance, cancellationToken);
    }
}
