/* 
*   NatCorder
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCorderU.Core {

    using UnityEngine;
    using System;

    [AddComponentMenu("")]
    public sealed class NatCorderAudio : MonoBehaviour, IAudioSource {
        
        public int channelCount {get {return (int)AudioSettings.speakerMode;}}
        public int sampleRate {get {return AudioSettings.outputSampleRate;}}
        public IAudioListener listener {private get; set;}

        private void OnAudioFilterRead (float[] data, int channels) {
            if (listener != null) listener.OnSampleBuffer(data, (long)(AudioSettings.dspTime * 1e+9f));
        }

        void IDisposable.Dispose () {
            listener = null;
            NatCorderAudio.Destroy(this);
        }
    }
}