/* 
*   NatCorder
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCorderU.Examples {

    using UnityEngine;
    using Core;
    using Extensions;

    public class RecordScreen : MonoBehaviour {

        public RecordingIcon icon;
        public bool replay = true;
		public string pathToReplay;
        
        void Start () {
            // Verbose
            NatCorder.Verbose = true;
        }

        void Update () {
            foreach (Touch t in Input.touches) if (t.phase == TouchPhase.Ended && t.tapCount == 2) ToggleRecording();
            if (Input.GetKeyUp(KeyCode.Space)) ToggleRecording();
            // Set recording icon
            icon.IsRecording = Replay.IsRecording;
        }

        void ToggleRecording () {
            // Start or stop recording
            if (!Replay.IsRecording) Replay.StartRecording(Camera.main, Configuration.Default, OnReplay);
            else Replay.StopRecording();
        }

		// Invoked by NatCorder once recording is finished
		void OnReplay (string path) {
		// Get the video thumbnail
		Texture2D thumbnail = Sharing.GetThumbnail(path, 5f); // Get the thumbnail after 5 seconds of video
		// Do stuff with the thumbnail
		DisplayThumbnail(thumbnail);
		// Save the path to the replay so we can use it in `OnShare`
		this.pathToReplay = path;
		}

		// Invoked by UI when user clicks share button
		public void OnShare () {
			// Share the replay using the native sharing UI
			Sharing.Share(this.pathToReplay);
		}
		
		void DisplayThumbnail(Texture2D thumbnail)
		{
			
		}
    }
}