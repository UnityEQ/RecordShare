/* 
*   NatCorder
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCorderU.Core {

    using System;

    public interface IAudioSource : IDisposable {
        int channelCount {get;}
        int sampleRate {get;}
        IAudioListener listener {set;}
    }

    public interface IAudioListener {
        void OnSampleBuffer (float[] samples, long timestamp);
    }
}