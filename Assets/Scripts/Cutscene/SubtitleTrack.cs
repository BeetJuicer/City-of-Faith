using System.Collections;
using System.Collections.Generic;
using TMPro; // For TextMeshProUGUI text handling
using UnityEngine;
using UnityEngine.Timeline; // For Timeline track creation
using UnityEngine.Playables; // For working with Playable API

// A custom Timeline track for managing subtitles.
// This track binds to a TextMeshProUGUI component and works with SubtitleClips to display subtitles.
[TrackBindingType(typeof(TextMeshProUGUI))] // Specifies the type of component this track binds to.
[TrackClipType(typeof(SubtitleClip))] // Specifies the type of clips this track can contain.
public class SubtitleTrack : TrackAsset
{
    // Creates the track's mixer, which blends subtitle clips during playback.
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        // Creates a ScriptPlayable for the SubtitleTrackMixer and returns it as the track mixer.
        return ScriptPlayable<SubtitleTrackMixer>.Create(graph, inputCount);
    }
}
