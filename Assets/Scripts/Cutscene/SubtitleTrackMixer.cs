using System.Collections;
using System.Collections.Generic;
using TMPro; // For TextMeshProUGUI text handling
using UnityEngine;
using UnityEngine.Playables; // For working with Playable API

// This class is a custom mixer for a Timeline track that manages subtitles.
// It determines which subtitle text and alpha (transparency) value should be displayed at any given frame.
public class SubtitleTrackMixer : PlayableBehaviour
{
    // Called during every frame of the Timeline to process and apply subtitle changes.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // Attempt to cast the playerData to a TextMeshProUGUI instance for subtitle display.
        TextMeshProUGUI text = playerData as TextMeshProUGUI;

        // Variables to store the current subtitle text and its transparency value.
        string currentText = "";
        float currentAlpha = 0f;

        // If no valid TextMeshProUGUI component is provided, exit early.
        if (!text) { return; }

        // Retrieve the number of inputs connected to the Playable (e.g., clips in the Timeline).
        int inputCount = playable.GetInputCount();

        // Loop through all inputs to determine the active subtitle clip.
        for (int i = 0; i < inputCount; i++)
        {
            // Get the blending weight of the current input.
            float inputWeight = playable.GetInputWeight(i);

            // If this input is active (weight > 0), process its subtitle data.
            if (inputWeight > 0f)
            {
                // Cast the input to a ScriptPlayable of SubtitleBehaviour to access its data.
                ScriptPlayable<SubtitleBehaviour> inputPlayable = (ScriptPlayable<SubtitleBehaviour>)playable.GetInput(i);
                SubtitleBehaviour input = inputPlayable.GetBehaviour();

                // Set the current text and alpha based on the active input's data.
                currentText = input.subtitleText;
                currentAlpha = inputWeight; // Use weight to determine transparency.
            }
        }

        // Update the TextMeshProUGUI component with the computed subtitle text and transparency.
        text.text = currentText;
        text.color = new Color(1, 1, 1, currentAlpha); // White color with varying alpha.
    }
}
