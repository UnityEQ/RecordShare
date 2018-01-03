/* 
*   NatCorder
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCorderU.Core.Platforms {

    public sealed class NatCorderNull : INatCorder {

        #region --Properties--
        public bool IsRecording { get { return false;}}
        public bool Verbose {set {}}
        #endregion


        #region --Operations--

        public void StartRecording (Configuration configuration, SaveCallback saveCallback, IAudioSource audioSource) {
            // We don't need the audio source
            if (audioSource != null) audioSource.Dispose();
        }

        public void StopRecording () {}

        public Frame Acquire () {return null;}

        public void Commit (Frame frame) {}
        #endregion
    }
}