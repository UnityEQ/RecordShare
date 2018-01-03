/* 
*   NatCorder
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCorderU.Core.Platforms {

    using AOT;
    using UnityEngine;
    using System;
    using Extensions;
    using NatCamU.Dispatch;
    using FramePool = System.Collections.Generic.Dictionary<System.IntPtr, UnityEngine.RenderTexture>;

    public sealed partial class NatCorderiOS : INatCorder, IAudioListener {

        #region --Op vars--
        private Configuration configuration;
        private SaveCallback saveCallback;
        private IAudioSource audioSource;
        private MainDispatch dispatch;
        private FramePool framePool = new FramePool();
        private readonly Material transformMat;
        private static NatCorderiOS instance { get {return NatCorder.Implementation as NatCorderiOS;}}
        #endregion

        #region --Properties--
        public bool IsRecording { get { return NatCorderBridge.IsRecording();}}
        public bool Verbose { set { NatCorderBridge.SetVerboseMode(value);}}
        #endregion


        #region --Operations--

        public NatCorderiOS () {
            NatCorderBridge.RegisterCallbacks(OnEncode, OnVideo);
            transformMat = new Material(Shader.Find("Hidden/NatCorder/Transform"));
            Utilities.Log("Initialized NatCorder 1.0 iOS backend");
        }

        public void StartRecording (Configuration configuration, SaveCallback saveCallback, IAudioSource audioSource) {
            this.dispatch = new MainDispatch();
            this.configuration = configuration;
            this.saveCallback = saveCallback;
            this.audioSource = audioSource;
            // Start recording
            NatCorderBridge.StartRecording(
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
            NatCorderBridge.StopRecording();
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
            // Make upright
            RenderTexture surface = Acquire();
            Graphics.Blit(frame, surface, transformMat);
            RenderTexture.ReleaseTemporary(frame);
            // Commit
            var handle = surface.GetNativeTexturePtr();
            framePool.Add(handle, surface);
            NatCorderBridge.EncodeFrame(handle, frame.timestamp);
        }

        void IAudioListener.OnSampleBuffer (float[] samples, long timestamp) {
            NatCorderBridge.EncodeSamples(samples, timestamp);
        }
        #endregion


        #region --Callbacks--

        [MonoPInvokeCallback(typeof(NatCorderBridge.EncodeCallback))]
        private static void OnEncode (IntPtr frame) {
            instance.dispatch.Dispatch(() => {
                // Release RenderTexture
                var surface = instance.framePool[frame];
                RenderTexture.ReleaseTemporary(surface);
                instance.framePool.Remove(frame);
            });
        }

        [MonoPInvokeCallback(typeof(SaveCallback))]
        private static void OnVideo (string path) {
            instance.dispatch.Dispatch(() => {
                instance.saveCallback(path);
                instance.dispatch.Release();
                instance.dispatch = null;
            });
        }
        #endregion
    }
}