# NatCorder API
NatCorder is a lightweight, easy-to-use, native video recording API for iOS and Android. NatCorder comes with a rich featureset including:
- Record anything that can be rendered into a texture.
- Control recording quality and file size with bitrate and keyframe interval.
- Record at any resolution. You get to specify what resolution recording you want.
- Get path to recorded video in device storage.
- Record game audio with video.
- Use native sharing UI to share recorded videos.
- Get thumbnail for recorded video (at custom times too).
- Experimental support for recording on Standalone platforms (Windows, macOS, and Linux, also in Editor).

There are two main levels of control that developers would usually want with a recording API like NatCorder: the high-level screen recording API; and the lower-level texture recording API.

## Recording Replays
NatCorder provides a very simple screen recording API with the `Replay` class. To start recording from a game camera, simply call:
```csharp
Replay.StartRecording(Camera.main, Configuration.Default, OnReplay);
```

And once you are done with recording, call:
```csharp
Replay.StopRecording();
```

NatCorder will finish recording and invoke your replay callback with the path to the recorded video. You can then use the `Sharing` API's to share the replay and to get a thumbnail:
```csharp
void OnReplay (string path) {
    // Log
    Debug.Log("Saved recording to: "+path);
    // First, show the replay to the user
    Handheld.PlayFullScreenMovie(path);
    // Share the recording with the native sharing UI
    Shharing.Share(path);
    // Do stuff with the recording thumbnail
    Texture2D thumbnail = Sharing.GetThumbnail(path);
    DoStuffWithThumbnail(thumbnail);
}
```

You can pause the recording and resume it as you need to:
```csharp
Replay.PauseRecording();
// ...
Replay.ResumeRecording();
```

You can also record with game audio. You can either record audio from the entire scene (using the scene's `AudioListener`) or from a specific audio source (using `AudioSource`). The former is useful for recording typical replays, whereas the latter is useful for recording specific audio (like microphone audio):
```csharp
public AudioListener audioListener; // Set this to the scene's audio listener
// ...
Replay.StartRecording(Camera.main, Configuration.Default, OnReplay, audioListener);
```

Here is an example using a specific audio source instead:
```csharp
public AudioSource audioSource; // Set this to an audio source
// ...
Replay.StartRecording(Camera.main, Configuration.Default, OnReplay, audioSource);
```

**NOTE**: When recording with audio, make sure that your `AudioSource`s do **not** have `Bypass Effects` or `Bypass Listener Effects` switched on.

## Recording Textures
NatCorder provides a lower-level recording API exposed with the `NatCorder` class. At this low-level, NatCorder works by encoding video frames and audio samples on demand. Like the `Replay` API, you must call `StartRecording` to start recording:
```csharp
NatCorder.StartRecording(Configuration.Default, OnReplay);
```

Once this is done, you then manually record individual frames. To do so, you first acquire an encoder frame with `NatCorder.AcquireFrame`; then you blit what you want to record into the frame with `Graphics.Blit` (and optionally set the `frame.timestamp`); then you commit the frame for encoding with `NatCorder.CommitFrame`:
```csharp
WebCamTexture webcamPreview; // Start this somewhere

void Update () {
    // Check that we are recording, and that the webcamtexture was updated this frame
    if (!NatCorder.IsRecording || !webcamPreview.didUpdateThisFrame) return;
    // Acquire an encoder frame
    var frame = NatCorder.AcquireFrame();
    // Blit the webcam preview to the frame
    Graphics.Blit(webcamPreview, frame);
    // Also set the timestamp // Note that this is optional as NatCorder sets the timestamp to when the frame was acquired by you, so only use it if you want to retime the video frames
    frame.timestamp = (long)(Time.realtimeSinceStartup * 1e+9f); // Timestamp must be in nanoseconds, so we multiply by 1e+9f
    // Now commit the frame to the encoder
    NatCorder.CommitFrame(frame);
}
```

**NOTE**: All frames that are acquired from NatCorder **must be committed**, or else there will be a resource leak.

## Using NatCorder in the Editor
NatCorder comes with an experimental Standalone backend that uses FFmpeg. In order to use the Standalone backend, you must first download the FFpeg executables for your system architecture. I recommend the `ffmpeg-static` [executables by Samuel Lacroix](https://github.com/KeatsPeeks/ffmpeg-static). Place the FFmpeg executable in:
- `Assets/StreamingAssets/NatCorder/darwin` if you are running on macOS.
- `Assets/StreamingAssets/NatCorder/win` if you are running on Windows.
- `Assets/StreamingAssets/NatCorder/linux` if you are running on Linux.

Once you have gotten the executable in place, open NatCorderStandalone.cs (in NatCorder>Plugins>Managed>Platforms) and uncomment the `FFMPEG_API` definition at the top of the file. You should now be all set.

**NOTE**: The Standalone backend does not support recording audio.

## Notes
- NatCorder doesn't have full support for recording UI canvases that are in Screen Space - Overlay mode. See [here](https://forum.unity3d.com/threads/render-a-canvas-to-rendertexture.272754/#post-1804847).
- On Android, NatCorder requires API Level 18 and up.