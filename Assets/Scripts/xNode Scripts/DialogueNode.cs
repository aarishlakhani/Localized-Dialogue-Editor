using UnityEngine;
using XNode;

public class DialogueNode : BaseNode
{
    [Input] public int entry;       // Entry port
    [Output] public int choiceA;    // Choice A port
    [Output] public int choiceB;    // Choice B port
    [Output] public int choiceC;    // Choice C port (New)
    [Output] public int choiceD;     // Choice D port (New)
    public string dialogueKey;      // Localization key for the dialogue line
    public string choiceAKey;       // Localization key for Choice A
    public string choiceBKey;       // Localization key for Choice B
    public string choiceCKey;       // Localization key for Choice C (New)
    public string choiceDKey;       // Localization key for Choice D (New)

    public override string GetString()
    {
        // Return data for all choices
        return "DialogueNode/" + dialogueKey + "/" + choiceAKey + "/" + choiceBKey + "/" + choiceCKey + "/" + choiceDKey;
    }
}