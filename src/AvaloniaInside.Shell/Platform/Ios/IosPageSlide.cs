using Avalonia;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;

namespace AvaloniaInside.Shell.Platform.Ios;

public class DefaultIosPageSlide : PlatformBasePageTransition
{
    public static readonly DefaultIosPageSlide Instance = new();

    protected override CompositionAnimationGroup GetOrCreateEnteranceAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
        var compositor = element.Compositor;

        var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
        offsetAnimation.Duration = Duration;
        offsetAnimation.Target = nameof(element.Offset);
        offsetAnimation.InsertKeyFrame(0f, new Vector3D(widthDistance, 0, 0), Easing);
        offsetAnimation.InsertKeyFrame(1.0f, new Vector3D(0, 0, 0), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(0f, 0.9f);
        fadeAnimation.InsertKeyFrame(0.5f, 1f);

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
        offsetAnimation.InsertKeyFrame(1.0f, new Vector3D(widthDistance, 0, 0), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(.5f, 1f);
        fadeAnimation.InsertKeyFrame(1f, 0.9f);

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
        offsetAnimation.InsertKeyFrame(1.0f, new Vector3D(widthDistance / -4d, 0, 0), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(0f, 1f);
        fadeAnimation.InsertKeyFrame(1f, .9f);

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
        offsetAnimation.InsertKeyFrame(0f, new Vector3D(widthDistance / -4d, 0, 0), Easing);
        offsetAnimation.InsertKeyFrame(1.0f, new Vector3D(0, 0, 0), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(0f, .9f);
        fadeAnimation.InsertKeyFrame(1f, 1f);

        var bringBackAnimation = compositor.CreateAnimationGroup();
        bringBackAnimation.Add(offsetAnimation);
        bringBackAnimation.Add(fadeAnimation);
        return bringBackAnimation;
    }
}
