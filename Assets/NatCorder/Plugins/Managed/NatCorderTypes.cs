/* 
*   NatCorder
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCorderU.Core {

    using UnityEngine;

    #region --Delegates--
    /// <summary>
    /// A delegate type used to report the location of a saved video
    /// </summary>
    public delegate void SaveCallback (string path);
    #endregion


    #region --Value Types--

    public struct Configuration {
        /// <summary>
        /// Video width
        /// </summary>
        public readonly int width;
        /// <summary>
        /// Video height
        /// </summary>
        public readonly int height;
        /// <summary>
        /// Video framerate
        /// </summary>
        public readonly int framerate;
        /// <summary>
        /// Video bitrate in bits per second
        /// </summary>
        public readonly int bitrate;
        /// <summary>
        /// Video keyframe interval in seconds
        /// </summary>
        public readonly int keyframeInterval;
        /// <summary>
        /// Default recording configuration
        /// </summary>
        public static Configuration Default { get { return new Configuration(Screen.width, Screen.height, Application.targetFrameRate, (int)(960 * 540 * 11.4f), 3);}}
        /// <summary>
        /// Create a recoridng configuration
        /// </summary>
        /// <param name="width">Video width</param>
        /// <param name="height">Video height</param>
        /// <param name="framerate">Video framerate</param>
        /// <param name="bitrate">Video bitrate in bits per second</param>
        /// <param name="keyframeInterval">Video keyframe interval in seconds</param>
        public Configuration (int width, int height, int framerate, int bitrate, int keyframeInterval) {
            this.width = width;
            this.height = height;
            this.framerate = framerate > 0 ? framerate : 30;
            this.bitrate = bitrate;
            this.keyframeInterval = keyframeInterval;
        }
    }

    /// <summary>
    /// Encoder surface for recording frame
    /// </summary>
    public sealed class Frame { // We make it a class instead of struct so we get reference equality
        
        /// <summary>
        /// Frame timestamp in nanoseconds
        /// </summary>
        public long timestamp = -1;
        private readonly RenderTexture surface;

        public Frame (RenderTexture surface, long timestamp = -1) {
            this.surface = surface;
            this.timestamp = timestamp;
        }

        public static implicit operator RenderTexture (Frame frame) {
            return frame.surface;
        }
    }
    #endregion
}