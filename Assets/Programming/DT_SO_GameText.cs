using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Info for Unity to create the scriptable objects
[CreateAssetMenu(fileName = "NewGameText", menuName = "ScriptableObjects/GameText", order = 1)]
// Create asset menu = we can go to the asset menu in the editor to create the object
// fileName = default filename for a new scriptable object of this type
// menuName = where it appears in the asset>create menu
// order = where it appears in the menu
public class DT_SO_GameText : ScriptableObject
{
    // Each entry created will be able to store variables from the class GameText

    public GameText[] gameText;

    [System.Serializable]
    public class GameText
    {
        // Each entry will have these variables:
        public GameManager.GameScene sceneName;
        public TextType textBoxType;
        // Text lines each have a speaker and text:
        public TextLines[] textLines;
        
        public enum TextType
        {
            None,
            Scroll,
            SpeechBubbles
        }
    }

    [System.Serializable]
    public class TextLines
    {
        public Speaker speaker;
        [TextAreaAttribute]
        public string text;
        
        public enum Speaker
        {
            Player,
            NPC,
            Info
        }
    }
}

