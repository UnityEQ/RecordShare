/* 
*   NatCorder
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCorderU.Core.Platforms {

    using System;
    using System.Runtime.InteropServices;

    public static class NatCorderBridge {

        private const string Assembly =
        #if UNITY_IOS
        "__Internal";
        #else
        "NatCorder";
        #endif

        public delegate void EncodeCallback (IntPtr frame);

        #if INATCORDER_C
        [DllImport(Assembly, EntryPoint = "NCRegisterCallbacks")]
        public static extern void RegisterCallbacks (EncodeCallback encodeCallback, SaveCallback saveCallback);
        [DllImport(Assembly, EntryPoint = "NCStartRecording")]
        public static extern void StartRecording (int width, int height, int framerate, int bitrate, int keyframes, bool audio, int sampleCount, int audioChannels);
        [DllImport(Assembly, EntryPoint = "NCStopRecording")]
        public static extern void StopRecording ();
        [DllImport(Assembly, EntryPoint = "NCIsRecording")]
        public static extern bool IsRecording ();
        [DllImport(Assembly, EntryPoint = "NCEncodeFrame")]
        public static extern void EncodeFrame (IntPtr frame, long timestamp);
        [DllImport(Assembly, EntryPoint = "NCEncodeSamples")]
        public static extern void EncodeSamples (float[] samples, long timestamp);
        [DllImport(Assembly, EntryPoint = "NCSetVerboseMode")]
        public static extern void SetVerboseMode (bool mode);

        #else
        public static void RegisterCallbacks (EncodeCallback encodeCallback, SaveCallback saveCallback) {}
        public static void StartRecording (int width, int height, int framerate, int bitrate, int keyframes, bool audio, int sampleCount, int audioChannels) {}
        public static void StopRecording () {}
        public static bool IsRecording () {return false;}
        public static void EncodeFrame (IntPtr frame, long timestamp) {}
        public static void EncodeSamples (float[] samples, long timestamp) {}
        public static void SetVerboseMode (bool mode) {}
        #endif
    }
}