using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
using AvaloniaInside.Shell.Platform.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Platform.Windows;
public class EntranceNavigationTransition : PlatformBasePageTransition
{
    private const float EndingCue = 0.5f;
    private const float StartingCue = 0.5f;
    private const double DistanceFactor = 0.2d;

    /// <summary>
    /// Scale factor to animate in
    /// </summary>
    public float ZoomInFactor { get; set; } = 1.01f;

    /// <summary>
    /// Scale factor to animate out
    /// </summary>
    public float ZoomOutFactor { get; set; } = 0.99f;

    public override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0.35);

    public override Easing Easing { get; set; } = Easing.Parse("0.85, 0.0, 0.0, 1.0");
    public Easing Easing2 { get; set; } = Easing.Parse("0.85, 0.0, 0.75, 1.0");
    public Easing Easing3 { get; set; } = Easing.Parse("0.85, 0.0, 0.0, 1.0");

    protected override CompositionAnimationGroup GetOrCreateEnteranceAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
        var compositor = element.Compositor;

        var scaleAnimation = compositor.CreateVector3DKeyFrameAnimation();
        scaleAnimation.Duration = Duration;
        scaleAnimation.Target = nameof(element.Scale);
        scaleAnimation.InsertKeyFrame(StartingCue, new Vector3D(ZoomOutFactor, ZoomOutFactor, 1), Easing2);
        scaleAnimation.InsertKeyFrame(1.0f, new Vector3D(1, 1, 1), Easing3);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(StartingCue, 0f, Easing2);
        fadeAnimation.InsertKeyFrame(1.0f, 1f, Easing3);

        var slideAnimation = compositor.CreateVector3DKeyFrameAnimation();
        slideAnimation.Duration = Duration;
        slideAnimation.Target = nameof(element.Offset);
        slideAnimation.InsertKeyFrame(StartingCue, new Vector3D(0, heightDistance * DistanceFactor, 0), Easing2);
        slideAnimation.InsertKeyFrame(1.0f, new Vector3D(0, 0, 0), Easing3);

        var enteranceAnimation = compositor.CreateAnimationGroup();
        enteranceAnimation.Add(scaleAnimation);
        enteranceAnimation.Add(fadeAnimation);
        enteranceAnimation.Add(slideAnimation);

        return enteranceAnimation;
    }

    protected override CompositionAnimationGroup GetOrCreateExitAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
        var compositor = element.Compositor;

        var scaleAnimation = compositor.CreateVector3DKeyFrameAnimation();
        scaleAnimation.Duration = Duration;
        scaleAnimation.Target = nameof(element.Scale);
        scaleAnimation.InsertKeyFrame(0f, new Vector3D(1, 1, 1), Easing);
        scaleAnimation.InsertKeyFrame(EndingCue, new Vector3D(ZoomOutFactor, ZoomOutFactor, 1), Easing2);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(0f, 1f, Easing);
        fadeAnimation.InsertKeyFrame(EndingCue, 0f, Easing2);

        var slideAnimation = compositor.CreateVector3DKeyFrameAnimation();
        slideAnimation.Duration = Duration;
        slideAnimation.Target = nameof(element.Offset);
        slideAnimation.InsertKeyFrame(0f, new Vector3D(0, 0, 0), Easing);
        slideAnimation.InsertKeyFrame(EndingCue, new Vector3D(0, heightDistance * DistanceFactor, 0), Easing2);

        var exitAnimation = compositor.CreateAnimationGroup();
        exitAnimation.Add(scaleAnimation);
        exitAnimation.Add(fadeAnimation);
        exitAnimation.Add(slideAnimation);

        return exitAnimation;
    }

    protected override CompositionAnimationGroup GetOrCreateSendBackAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
        var compositor = element.Compositor;

        var scaleAnimation = compositor.CreateVector3DKeyFrameAnimation();
        scaleAnimation.Duration = Duration;
        scaleAnimation.Target = nameof(element.Scale);
        scaleAnimation.InsertKeyFrame(0f, new Vector3D(1, 1, 1), Easing);
        scaleAnimation.InsertKeyFrame(EndingCue, new Vector3D(ZoomOutFactor, ZoomOutFactor, 1), Easing2);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(0f, 1f, Easing);
        fadeAnimation.InsertKeyFrame(EndingCue, 0f, Easing2);

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
        scaleAnimation.InsertKeyFrame(StartingCue, new Vector3D(ZoomOutFactor, ZoomOutFactor, 1), Easing2);
        scaleAnimation.InsertKeyFrame(1f, new Vector3D(1, 1, 1), Easing3);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(StartingCue, 0f, Easing2);
        fadeAnimation.InsertKeyFrame(1f, 1f, Easing3);

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
