/* 
*   NatCorder
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCorderU.Core.Platforms {

    using UnityEngine;
    using Extensions;
    using NatCamU.Dispatch;
    using FramePool = System.Collections.Generic.Dictionary<int, UnityEngine.RenderTexture>;

    public sealed partial class NatCorderAndroid : AndroidJavaProxy, INatCorder, IAudioListener {

        #region --Op vars--
        private Configuration configuration;
        private SaveCallback saveCallback;
        private IAudioSource audioSource;
        private MainDispatch dispatch;
        private FramePool framePool = new FramePool();
        #pragma warning disable 0414
        private readonly IDispatch renderDispatch;
        #pragma warning restore 0414
        private readonly AndroidJavaObject natcorder;
        #endregion


        #region --Properties--
        public bool IsRecording { get { return natcorder.Call<bool>("isRecording");}}
        public bool Verbose { set { natcorder.Call("setVerboseMode", value);}}
        #endregion


        #region --Operations--

        public NatCorderAndroid () : base("com.yusufolokoba.natcorder.NatCorderDelegate") {
            natcorder = new AndroidJavaObject("com.yusufolokoba.natcorder.NatCorder", this);
            renderDispatch = new RenderDispatch();
            Utilities.Log("Initialized NatCorder 1.0 Android backend");
        }

        public void StartRecording (Configuration configuration, SaveCallback saveCallback, IAudioSource audioSource) {
            this.dispatch = new MainDispatch();
            this.configuration = configuration;
            this.saveCallback = saveCallback;
            this.audioSource = audioSource;
            // Start recording
            natcorder.Call("startRecording",
                configuration.width,
                configuration.height,
                configuration.framerate,
                configuration.bitrate,
                configuration.keyframeInterval,
                audioSource != null,
                audioSource != null ? audioSource.sampleRate : 0,
                audioSource != null ? audioSource.channelCount : 0
            );
            // Start listening for audio
            if (audioSource != null) audioSource.listener = this;
        }

        public void StopRecording () {
            if (audioSource != null) audioSource.Dispose();
            natcorder.Call("stopRecording");
        }

        public Frame Acquire () {
            return new Frame(
                RenderTexture.GetTemporary(
                    configuration.width,
                    configuration.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Default,
                    1
                ),
                (long)(Time.realtimeSinceStartup * 1e+9f)
            );
        }

        public void Commit (Frame frame) {
            var handle = ((RenderTexture)frame).GetNativeTexturePtr().ToInt32();
            framePool.Add(handle, frame);
            natcorder.Call("encodeFrame", handle, frame.timestamp);
        }

        void IAudioListener.OnSampleBuffer (float[] samples, long timestamp) {
            natcorder.Call("encodeSamples", samples, timestamp);
        }
        #endregion


        #region --Callbacks--

        private void onEncode (int frame) {
            dispatch.Dispatch(() => {
                // Release RenderTexture
                var surface = framePool[frame];
                RenderTexture.ReleaseTemporary(surface);
                framePool.Remove(frame);
            });
        }

        private void onVideo (string path) {
            dispatch.Dispatch(() => {
                saveCallback(path);
                dispatch.Release();
                dispatch = null;
            });
        }
        #endregion
    }
}