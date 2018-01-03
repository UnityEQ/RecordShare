/* 
*   NatCorder
*   Copyright (c) 2017 Yusuf Olokoba
*/

/*
* Uncomment this to use FFMPEG for encoding.
* Make sure you have the FFMPEG executable for your platform in StreamingAssets.
* Note that this backend does not support recording audio.
*/
//#define FFMPEG_API

namespace NatCorderU.Core.Platforms {

    using UnityEngine;
    using System;
    using System.Diagnostics;
    using System.IO;
    using Extensions;

    public sealed partial class NatCorderStandalone : INatCorder {

        #region --Op vars--
        #if FFMPEG_API
        private Configuration configuration;
        private SaveCallback saveCallback;
        private RenderTexture readbackBuffer;
        private Texture2D framebuffer;
        private string filename;
        private Process videoWriter;
        private BinaryWriter videoInput;
        private readonly Material transformMat;
        #endif
        #endregion

        #region --Properties--
        public bool IsRecording {get; private set;}
        public bool Verbose {set {}}
        private string FFmpegPath {
            get {
                #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                return Application.streamingAssetsPath + "/NatCorder/win/ffmpeg.exe";
                #elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                return Application.streamingAssetsPath + "/NatCorder/darwin/ffmpeg";
                #elif UNITY_STANDALONE_LINUX
                return Application.streamingAssetsPath + "/NatCorder/linux/ffmpeg";
                #else
                return string.Empty;
                #endif
            }
        }
        #endregion


        #region --Operations--

        public NatCorderStandalone () {
            #if FFMPEG_API
            transformMat = new Material(Shader.Find("Hidden/NatCorder/Transform"));
            Utilities.Log("Initialized NatCorder 1.0 Standalone backend");
            #endif
        }

        public void StartRecording (Configuration configuration, SaveCallback saveCallback, IAudioSource audioSource) {
            #if FFMPEG_API
            this.configuration = configuration;
            this.saveCallback = saveCallback;
            this.readbackBuffer = Acquire();
            this.framebuffer = new Texture2D(configuration.width, configuration.height, TextureFormat.RGB24, false);
            // Create process
            filename = "recording_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
            videoWriter = CreateProcess(string.Format(
                @"-y -f rawvideo -vcodec rawvideo -pixel_format rgb24 
                -video_size {0}x{1} -framerate {2} -loglevel warning
                -i - -vcodec libx264 -pix_fmt yuv420p
                -preset ultrafast {3}",
                configuration.width,
                configuration.height,
                configuration.framerate,
                filename + ".mp4"
            ));
            videoWriter.Start();
            videoInput = new BinaryWriter(videoWriter.StandardInput.BaseStream);
            videoWriter.BeginOutputReadLine();
            videoWriter.BeginErrorReadLine();
            // We don't support audio
            if (audioSource != null) audioSource.Dispose();
            // Start recording
            IsRecording = true;
            #endif
        }

        public void StopRecording () {
            #if FFMPEG_API
            IsRecording = false;
            CloseProcess(videoWriter);
            videoInput = null;
            videoWriter = null;
            Texture2D.Destroy(framebuffer);
            RenderTexture.ReleaseTemporary(readbackBuffer);
            readbackBuffer = null;
            framebuffer = null;
            saveCallback(filename + ".mp4");
            #endif
        }

        public Frame Acquire () {
            #if FFMPEG_API
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
            #else
            return null;
            #endif
        }

        public void Commit (Frame frame) {
            #if FFMPEG_API
            readbackBuffer.DiscardContents();
            Graphics.Blit(frame, readbackBuffer, transformMat);
            RenderTexture.ReleaseTemporary(frame);
            var currentRT = RenderTexture.active;
            RenderTexture.active = readbackBuffer;
            framebuffer.ReadPixels(new Rect(0, 0, configuration.width, configuration.height), 0, 0, false);
            RenderTexture.active = currentRT;
            videoInput.Write(framebuffer.GetRawTextureData());
            videoInput.Flush();
            #endif
        }

        private Process CreateProcess (string args) {
            var info = new ProcessStartInfo(FFmpegPath, args);
            info.UseShellExecute = false;
            info.CreateNoWindow = 
            info.RedirectStandardError = 
            info.RedirectStandardInput = 
            info.RedirectStandardOutput = true;
            var process = new Process();
            process.StartInfo = info;
            process.EnableRaisingEvents = true;
            if (Utilities.verbose) process.OutputDataReceived += (unused, data) => {
                if (!string.IsNullOrEmpty(data.Data.Trim())) UnityEngine.Debug.Log("NatCorder Logging: FFmpeg: " + data.Data);
            };
            process.ErrorDataReceived += (unused, data) => {
                if (!string.IsNullOrEmpty(data.Data.Trim())) UnityEngine.Debug.LogError("NatCorder Error: FFmpeg: " + data.Data);
            };
            return process;
        }

        private void CloseProcess (Process process) {
            process.StandardInput.Close();
            process.WaitForExit();
            process.Close();
            process.Dispose();
        }
        #endregion
    }
}