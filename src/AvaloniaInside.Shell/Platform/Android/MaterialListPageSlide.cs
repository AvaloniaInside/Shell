using Avalonia.Animation.Easings;
using Avalonia;
using System;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;

namespace AvaloniaInside.Shell.Platform.Android;

public class MaterialListPageSlide : PlatformBasePageTransition
{
    private const float EndingCue = 0.75f;
    private const float StartingCue = 0.25f;

    /// <summary>
    /// Gets the duration of the animation.
    /// </summary>
    public override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(.3);

    /// <summary>
    /// Gets or sets element entrance easing.
    /// </summary>
    public override Easing Easing { get; set; } = new FastOutExtraSlowInEasing();

    protected override CompositionAnimationGroup GetOrCreateEnteranceAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
        var compositor = element.Compositor;

        var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
        offsetAnimation.Duration = Duration;
        offsetAnimation.Target = nameof(element.Offset);
        offsetAnimation.InsertKeyFrame(StartingCue, new Vector3D(widthDistance / 2d, 0, 0), Easing);
        offsetAnimation.InsertKeyFrame(1.0f, new Vector3D(0, 0, 0), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(StartingCue, 0f, Easing);
        fadeAnimation.InsertKeyFrame(1.0f, 1f, Easing);

        var enteranceAnimation = compositor.CreateAnimationGroup();
        enteranceAnimation.Add(offsetAnimation);
        enteranceAnimation.Add(fadeAnimation);
        return enteranceAnimation;
    }

    protected override CompositionAnimationGroup GetOrCreateExitAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
        var compositor = element.Compositor;

        var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
        offsetAnimation.Duration = Duration;
        offsetAnimation.Target = nameof(element.Offset);
        offsetAnimation.InsertKeyFrame(0f, new Vector3D(0, 0, 0), Easing);
        offsetAnimation.InsertKeyFrame(EndingCue, new Vector3D(widthDistance / 2d, 0, 0), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(0f, 1f, Easing);
        fadeAnimation.InsertKeyFrame(EndingCue, 0f, Easing);

        var exitAnimation = compositor.CreateAnimationGroup();
        exitAnimation.Add(offsetAnimation);
        exitAnimation.Add(fadeAnimation);
        return exitAnimation;
    }

    protected override CompositionAnimationGroup GetOrCreateSendBackAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
        var compositor = element.Compositor;

        var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
        offsetAnimation.Duration = Duration;
        offsetAnimation.Target = nameof(element.Offset);
        offsetAnimation.InsertKeyFrame(0f, new Vector3D(0, 0, 0), Easing);
        offsetAnimation.InsertKeyFrame(EndingCue, new Vector3D(-widthDistance / 2d, 0, 0), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(0f, 1f, Easing);
        fadeAnimation.InsertKeyFrame(EndingCue, 0f, Easing);

        var sendBackAnimation = compositor.CreateAnimationGroup();
        sendBackAnimation.Add(offsetAnimation);
        sendBackAnimation.Add(fadeAnimation);
        return sendBackAnimation;
    }

    protected override CompositionAnimationGroup GetOrCreateBringBackAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
        var compositor = element.Compositor;

        var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
        offsetAnimation.Duration = Duration;
        offsetAnimation.Target = nameof(element.Offset);
        offsetAnimation.InsertKeyFrame(StartingCue, new Vector3D(-widthDistance / 2d, 0, 0), Easing);
        offsetAnimation.InsertKeyFrame(1f, new Vector3D(0, 0, 0), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(StartingCue, 0f, Easing);
        fadeAnimation.InsertKeyFrame(1f, 1f, Easing);

        var bringBackAnimation = compositor.CreateAnimationGroup();
        bringBackAnimation.Add(offsetAnimation);
        bringBackAnimation.Add(fadeAnimation);
        return bringBackAnimation;
    }
}
