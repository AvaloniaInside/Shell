using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;

namespace AvaloniaInside.Shell.Platform.Ios;

public class IosModalSlide : PlatformBasePageTransition
{
    public static readonly IosModalSlide Instance = new();

    protected override CompositionAnimationGroup GetOrCreateEntranceAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
        var compositor = element.Compositor;

        var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
        offsetAnimation.Duration = Duration;
        offsetAnimation.Target = nameof(element.Offset);
        offsetAnimation.InsertKeyFrame(0f, new Vector3D(0, heightDistance, 0), Easing);
        offsetAnimation.InsertKeyFrame(1.0f, new Vector3D(0, 40, 0), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(0f, 0.9f);
        fadeAnimation.InsertKeyFrame(0.5f, 1f);

        var entranceAnimation = compositor.CreateAnimationGroup();
        entranceAnimation.Add(offsetAnimation);
        entranceAnimation.Add(fadeAnimation);
        return entranceAnimation;
    }

    protected override CompositionAnimationGroup GetOrCreateExitAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
	    ShouldHideAfterExit = false;

        var compositor = element.Compositor;

        var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
        offsetAnimation.Duration = Duration;
        offsetAnimation.Target = nameof(element.Offset);
        offsetAnimation.InsertKeyFrame(0f, new Vector3D(0, 40, 0), Easing);
        offsetAnimation.InsertKeyFrame(1.0f, new Vector3D(0, heightDistance, 0), Easing);

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
	    ShouldHideAfterExit = false;

        var compositor = element.Compositor;

        var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
        offsetAnimation.Duration = Duration;
        offsetAnimation.Target = nameof(element.Offset);
        offsetAnimation.InsertKeyFrame(0f, new Vector3D(0, 0, 0), Easing);
        offsetAnimation.InsertKeyFrame(1.0f, new Vector3D(0, 10, 0), Easing);

        var scaleAnimation = compositor.CreateVector3DKeyFrameAnimation();
        scaleAnimation.Duration = Duration;
        scaleAnimation.Target = nameof(element.Scale);
        scaleAnimation.InsertKeyFrame(0f, new Vector3D(1f, 1f, 0), Easing);
        scaleAnimation.InsertKeyFrame(1f, new Vector3D(.9f, .97f, 0), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(.5f, 1f);
        fadeAnimation.InsertKeyFrame(1f, 0.8f);

        var sendBackAnimation = compositor.CreateAnimationGroup();
        sendBackAnimation.Add(offsetAnimation);
        sendBackAnimation.Add(scaleAnimation);
        sendBackAnimation.Add(fadeAnimation);
        return sendBackAnimation;
    }

    protected override CompositionAnimationGroup GetOrCreateBringBackAnimation(CompositionVisual element, double widthDistance, double heightDistance)
    {
        var compositor = element.Compositor;

        var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
        offsetAnimation.Duration = Duration;
        offsetAnimation.Target = nameof(element.Offset);
        offsetAnimation.InsertKeyFrame(0f, new Vector3D(0, 10, 0), Easing);
        offsetAnimation.InsertKeyFrame(1.0f, new Vector3D(0, 0, 0), Easing);

        var scaleAnimation = compositor.CreateVector3DKeyFrameAnimation();
        scaleAnimation.Duration = Duration;
        scaleAnimation.Target = nameof(element.Scale);
        scaleAnimation.InsertKeyFrame(0f, new Vector3D(.9f, .97f, 0), Easing);
        scaleAnimation.InsertKeyFrame(1.0f, new Vector3D(1f, 1f, 0), Easing);

        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.Duration = Duration;
        fadeAnimation.Target = nameof(element.Opacity);
        fadeAnimation.InsertKeyFrame(0f, 0.9f);
        fadeAnimation.InsertKeyFrame(0.5f, 1f);

        var bringBackAnimation = compositor.CreateAnimationGroup();
        bringBackAnimation.Add(offsetAnimation);
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
