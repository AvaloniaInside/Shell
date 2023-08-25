using Avalonia.Animation.Easings;
using System;

namespace AvaloniaInside.Shell.Platform.Android;

public class FastOutExtraSlowInEasing : Easing
{
    private static readonly float[] VALUES = new float[] {
          0.0000f, 0.0008f, 0.0016f, 0.0024f, 0.0032f, 0.0057f, 0.0083f,
            0.0109f, 0.0134f, 0.0171f, 0.0218f, 0.0266f, 0.0313f, 0.0360f,
            0.0431f, 0.0506f, 0.0581f, 0.0656f, 0.0733f, 0.0835f, 0.0937f,
            0.1055f, 0.1179f, 0.1316f, 0.1466f, 0.1627f, 0.1810f, 0.2003f,
            0.2226f, 0.2468f, 0.2743f, 0.3060f, 0.3408f, 0.3852f, 0.4317f,
            0.4787f, 0.5177f, 0.5541f, 0.5834f, 0.6123f, 0.6333f, 0.6542f,
            0.6739f, 0.6887f, 0.7035f, 0.7183f, 0.7308f, 0.7412f, 0.7517f,
            0.7621f, 0.7725f, 0.7805f, 0.7879f, 0.7953f, 0.8027f, 0.8101f,
            0.8175f, 0.8230f, 0.8283f, 0.8336f, 0.8388f, 0.8441f, 0.8494f,
            0.8546f, 0.8592f, 0.8630f, 0.8667f, 0.8705f, 0.8743f, 0.8780f,
            0.8818f, 0.8856f, 0.8893f, 0.8927f, 0.8953f, 0.8980f, 0.9007f,
            0.9034f, 0.9061f, 0.9087f, 0.9114f, 0.9141f, 0.9168f, 0.9194f,
            0.9218f, 0.9236f, 0.9255f, 0.9274f, 0.9293f, 0.9312f, 0.9331f,
            0.9350f, 0.9368f, 0.9387f, 0.9406f, 0.9425f, 0.9444f, 0.9460f,
            0.9473f, 0.9486f, 0.9499f, 0.9512f, 0.9525f, 0.9538f, 0.9551f,
            0.9564f, 0.9577f, 0.9590f, 0.9603f, 0.9616f, 0.9629f, 0.9642f,
            0.9654f, 0.9663f, 0.9672f, 0.9680f, 0.9689f, 0.9697f, 0.9706f,
            0.9715f, 0.9723f, 0.9732f, 0.9741f, 0.9749f, 0.9758f, 0.9766f,
            0.9775f, 0.9784f, 0.9792f, 0.9801f, 0.9808f, 0.9813f, 0.9819f,
            0.9824f, 0.9829f, 0.9835f, 0.9840f, 0.9845f, 0.9850f, 0.9856f,
            0.9861f, 0.9866f, 0.9872f, 0.9877f, 0.9882f, 0.9887f, 0.9893f,
            0.9898f, 0.9903f, 0.9909f, 0.9914f, 0.9917f, 0.9920f, 0.9922f,
            0.9925f, 0.9928f, 0.9931f, 0.9933f, 0.9936f, 0.9939f, 0.9942f,
            0.9944f, 0.9947f, 0.9950f, 0.9953f, 0.9955f, 0.9958f, 0.9961f,
            0.9964f, 0.9966f, 0.9969f, 0.9972f, 0.9975f, 0.9977f, 0.9979f,
            0.9981f, 0.9982f, 0.9983f, 0.9984f, 0.9986f, 0.9987f, 0.9988f,
            0.9989f, 0.9991f, 0.9992f, 0.9993f, 0.9994f, 0.9995f, 0.9995f,
            0.9996f, 0.9996f, 0.9997f, 0.9997f, 0.9997f, 0.9998f, 0.9998f,
            0.9998f, 0.9999f, 0.9999f, 1.0000f, 1.0000f
    };

    private static readonly float STEP = 1f / (VALUES.Length - 1);

    public override double Ease(double input)
    {
        if (input >= 1.0)
            return 1.0;
        if (input <= 0)
            return 0.0;

        int position = Math.Min((int)(input * (VALUES.Length - 1)), VALUES.Length - 2);

        float quantized = position * STEP;
        float diff = (float)(input - quantized);
        float weight = diff / STEP;

        return VALUES[position] + weight * (VALUES[position + 1] - VALUES[position]);
    }
}
