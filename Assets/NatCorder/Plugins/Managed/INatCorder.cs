/* 
*   NatCorder
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCorderU.Core.Platforms {

    public interface INatCorder {

        #region --Properties--
        bool IsRecording {get;}
        bool Verbose {set;}
        #endregion
        
        #region --Operations--
        void StartRecording (Configuration configuration, SaveCallback saveCallback, IAudioSource audioSource);
        void StopRecording ();
        Frame Acquire ();
        void Commit (Frame frame);
        #endregion
    }
}