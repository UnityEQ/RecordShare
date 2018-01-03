/* 
*   NatCorder
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCorderU.Core {

    using UnityEngine;
    using Platforms;
    using Extensions;
    using NatCamU.Dispatch;
    using Util = Extensions.Utilities;

    public static class NatCorder {

        #region --Properties--
        /// <summary>
        /// The backing implementation NatCorder uses on this platform
        /// </summary>
        public static readonly INatCorder Implementation;
        /// <summary>
        /// Is a video being recorded?
        /// </summary>
        public static bool IsRecording { get { return Implementation.IsRecording;}}
        /// <summary>
        /// Enable or disable verbose logging
        /// </summary>
        public static bool Verbose { set { Utilities.verbose = Implementation.Verbose = value;}}
        #endregion


        #region --Operations--

        /// <summary>
        /// Start recording a video with no audio
        /// </summary>
        /// <param name="configuration">Configuration for recording</param>
        /// <param name="saveCallback">Callback to be invoked when the video is saved</param>
        public static void StartRecording (Configuration configuration, SaveCallback saveCallback) {
            if (IsRecording) {
                Util.LogError("Cannot start recording because NatCorder is already recording");
                return;
            } if (saveCallback == null) {
                Util.LogError("Cannot record video without callback");
                return;
            }
            Implementation.StartRecording(configuration, saveCallback, null);
        }

        /// <summary>
        /// Start recording a video with audio
        /// </summary>
        /// <param name="configuration">Configuration for recording</param>
        /// <param name="saveCallback">Callback to be invoked when the video is saved</param>
        /// <param name="audioListener">Audio listener for recording audio</param>
        public static void StartRecording (Configuration configuration, SaveCallback saveCallback, AudioListener audioListener) {
            if (IsRecording) {
                Util.LogError("Cannot start recording because NatCorder is already recording");
                return;
            } if (saveCallback == null) {
                Util.LogError("Cannot record video without callback");
                return;
            } if (audioListener == null) {
                Util.LogError("Cannot record video with null audio source");
                return;
            }
            Implementation.StartRecording(configuration, saveCallback, audioListener.gameObject.AddComponent<NatCorderAudio>());
        }

        /// <summary>
        /// Start recording a video with audio
        /// </summary>
        /// <param name="configuration">Configuration for recording</param>
        /// <param name="saveCallback">Callback to be invoked when the video is saved</param>
        /// <param name="audioSource">Audio source for recording audio</param>
        public static void StartRecording (Configuration configuration, SaveCallback saveCallback, AudioSource audioSource) {
            if (IsRecording) {
                Util.LogError("Cannot start recording because NatCorder is already recording");
                return;
            } if (saveCallback == null) {
                Util.LogError("Cannot record video without callback");
                return;
            } if (audioSource == null) {
                Util.LogError("Cannot record video with null audio source");
                return;
            }
            Implementation.StartRecording(configuration, saveCallback, audioSource.gameObject.AddComponent<NatCorderAudio>());
        }

        /// <summary>
        /// Stop recording a video
        /// </summary>
        public static void StopRecording () {
            if (!IsRecording) {
                Util.LogError("Cannot stop recording because NatCorder is not recording");
                return;
            }
            Implementation.StopRecording();
        }

        /// <summary>
        /// Acquire a frame for encoding
        /// You will call Graphics::Blit on this frame
        /// </summary>
        public static Frame AcquireFrame () {
            if (!IsRecording) {
                Util.LogError("Cannot acquire frame when NatCorder is not recording");
                return null;
            }
            return Implementation.Acquire();
        }

        /// <summary>
        /// Commit a frame for encoding
        /// </summary>
        public static void CommitFrame (Frame frame) {
            if (!IsRecording) {
                Util.LogError("Cannot commit frame when NatCorder is not recording");
                RenderTexture.ReleaseTemporary(frame); // Release the frame
                return;
            }
            Implementation.Commit(frame);
        }
        #endregion


        #region --Initializer--

        static NatCorder () {
            // Create implementation for this platform
            Implementation = 
            #if UNITY_EDITOR || UNITY_STANDALONE
            new NatCorderStandalone();
            #elif UNITY_IOS
            new NatCorderiOS();
            #elif UNITY_ANDROID
            new NatCorderAndroid();
            #else
            new NatCorderNull();
            #endif
            DispatchUtility.onQuit += () => { if (IsRecording) StopRecording();}; // Stop recording if app is closed
        }
        #endregion
    }
}