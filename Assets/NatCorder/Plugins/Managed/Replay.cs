/* 
*   NatCorder
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCorderU.Core {

    using UnityEngine;
    using Util = Extensions.Utilities;

    public static class Replay {

        #region --Op vars--
        private static CameraRecorder camera;
        #endregion


        #region --Properties--
        public static bool IsRecording { get { return NatCorder.IsRecording;}}
        public static bool IsPaused {get; private set;}
        #endregion


        #region --Operations--

        /// <summary>
        /// Start recording a replay with no audio
        /// </summary>
        /// <param name="recordingCamera">Source camera for recording replay</param>
        /// <param name="configuration">Configuration for recording</param>
        /// <param name="saveCallback">Callback to be invoked when the video is saved</param>
        public static void StartRecording (Camera recordingCamera, Configuration configuration, SaveCallback saveCallback) {
            if (!recordingCamera) {
                Util.LogError("Cannot record replay without source camera");
                return;
            } if (saveCallback == null) {
                Util.LogError("Cannot record replay without callback");
                return;
            }
            NatCorder.StartRecording(configuration, saveCallback);
            camera = recordingCamera.gameObject.AddComponent<CameraRecorder>();
        }

        /// <summary>
        /// Start recording a replay with audio
        /// </summary>
        /// <param name="recordingCamera">Source camera for recording replay</param>
        /// <param name="configuration">Configuration for recording</param>
        /// <param name="saveCallback">Callback to be invoked when the video is saved</param>
        /// <param name="audioListener">Audio listener for recording audio</param>
        public static void StartRecording (Camera recordingCamera, Configuration configuration, SaveCallback saveCallback, AudioListener audioListener) {
            if (!recordingCamera) {
                Util.LogError("Cannot record replay without source camera");
                return;
            } if (saveCallback == null) {
                Util.LogError("Cannot record replay without callback");
                return;
            } if (audioListener == null) {
                Util.LogError("Cannot record replay with null audio source");
            }
            NatCorder.StartRecording(configuration, saveCallback, audioListener);
            camera = recordingCamera.gameObject.AddComponent<CameraRecorder>();
        }

        /// <summary>
        /// Start recording a replay with audio
        /// </summary>
        /// <param name="recordingCamera">Source camera for recording replay</param>
        /// <param name="configuration">Configuration for recording</param>
        /// <param name="saveCallback">Callback to be invoked when the video is saved</param>
        /// <param name="audioSource">Audio source for recording audio</param>
        public static void StartRecording (Camera recordingCamera, Configuration configuration, SaveCallback saveCallback, AudioSource audioSource) {
            if (!recordingCamera) {
                Util.LogError("Cannot record replay without source camera");
                return;
            } if (saveCallback == null) {
                Util.LogError("Cannot record replay without callback");
                return;
            } if (audioSource == null) {
                Util.LogError("Cannot record replay with null audio source");
            }
            NatCorder.StartRecording(configuration, saveCallback, audioSource);
            camera = recordingCamera.gameObject.AddComponent<CameraRecorder>();
        }

        /// <summary>
        /// Stop recording a replay
        /// </summary>
        public static void StopRecording () {
            CameraRecorder.Destroy(camera);
            NatCorder.StopRecording();
        }

        /// <summary>
        /// Pause recording
        /// </summary>
        public static void PauseRecording () { // INCOMPLETE // Handle audio timestamps
            IsPaused = true; // Easy peasy B)
        }

        /// <summary>
        /// Resume recording
        /// </summary>
        public static void ResumeRecording () {
            IsPaused = false;
        }

        [AddComponentMenu(""), DisallowMultipleComponent]
        private class CameraRecorder : MonoBehaviour {

            private void OnRenderImage (RenderTexture src, RenderTexture dst) {
                // Blit to recording frame
                if (!IsPaused) {
                    var encoderFrame = NatCorder.AcquireFrame();
                    encoderFrame.timestamp = (long)(Time.realtimeSinceStartup * 1e+9f);
                    Graphics.Blit(src, encoderFrame);
                    NatCorder.CommitFrame(encoderFrame);
                }
                // Blit to render pipeline
                Graphics.Blit(src, dst);
            }

            private void OnDestroy () { // INCOMPLETE // Handle appropriately // But note that ::StopRecording invokes this
                
            }
        }
        #endregion
    }
}