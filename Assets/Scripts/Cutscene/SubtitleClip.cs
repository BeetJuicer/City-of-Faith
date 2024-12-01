using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables; // For working with Playable API

// This class represents a single subtitle clip in the Timeline.
// It defines the subtitle text to be displayed and creates a playable for it.
public class SubtitleClip : PlayableAsset
{
    // The text of the subtitle for this clip.
    public string subtitleText;

    // Called by the Timeline to create a playable instance for this subtitle clip.
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // Create a ScriptPlayable instance for the SubtitleBehaviour.
        var playable = ScriptPlayable<SubtitleBehaviour>.Create(graph);

        // Retrieve the SubtitleBehaviour associated with the playable.
        SubtitleBehaviour subtitleBehaviour = playable.GetBehaviour();

        // Assign the subtitle text to the behaviour, which will later be used in the mixer.
        subtitleBehaviour.subtitleText = subtitleText;

        // Return the configured playable instance.
        return playable;
    }
}
