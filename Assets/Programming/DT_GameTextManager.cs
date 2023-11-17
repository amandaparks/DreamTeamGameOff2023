using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DT_GameTextManager : MonoBehaviour
{
    [SerializeField] private DT_SO_GameText gameText;
    [SerializeField] private 
    
    
    //Have static access to:
    //GameManager.CurrentScene
    //GameManager.PlayerLevel

// manager knows: current level, info text box, player text box, NPC text box
    private void Awake()
    {
        if (gameText == null)
        {
            Debug.LogError("Game Text Scriptable Object not assigned");
        }
    }

    private void Start()
    {
        
    }


    // On trigger, other game objects invoke this method
    // method takes parameters : text type (info or dialogue), scene to be loaded after (if not null)

    public void RunGameText(DT_SO_GameText.GameText.TextType textType, GameManager.GameScene sceneToLoad)
    {
        
    }

    
    
    // text can be cycled through with clicking or pressing step forward button
    // 





}

/*
    private int currentEntryIndex = 0; // Tracks the current position in the dialogue array

    private void Start()
    {
        Time.timeScale = 0; // This freezes the game
                            // Disable any player inputs or other scripts that should not function during the dialogue
    

        // Display the first entry of dialogue when the scene starts, if any are available
        if (dialogueEntries.Length > 0)
        {
            DisplayDialogueEntry(currentEntryIndex);
        }
    }

    private void Update()
    {
        // Detect if the player has clicked to advance the dialogue
        if (Input.GetMouseButtonDown(0))
        {
            AdvanceDialogue();
        }
    }

    public void AdvanceDialogue()
    {
        currentEntryIndex++; // Move to the next dialogue entry

        // Check if there are more entries to display
        if (currentEntryIndex < dialogueEntries.Length)
        {
            DisplayDialogueEntry(currentEntryIndex);
        }
        else
        {
            EndDialogue(); // Call the end dialogue function if no more entries exist
        }
    }

    private void DisplayDialogueEntry(int index)
    {
        // Update the UI with the current dialogue entry's character name and text
        characterNameText.text = dialogueEntries[index].characterName;
        dialogueText.text = dialogueEntries[index].dialogueText;

        // Optionally adjust alignment based on the character speaking
        if (dialogueEntries[index].characterName == playerName)
        {
            characterNameText.alignment = TextAlignmentOptions.Left;
        }
        else
        {
            characterNameText.alignment = TextAlignmentOptions.Right;
        }

        // Update the character images for the current dialogue entry
        playerImage.sprite = dialogueEntries[index].playerSprite;
        nonPlayerImage.sprite = dialogueEntries[index].nonPlayerSprite;

        // Enable or disable image components based on whether sprites are assigned
        playerImage.enabled = dialogueEntries[index].playerSprite != null;
        nonPlayerImage.enabled = dialogueEntries[index].nonPlayerSprite != null;
    }

    private void EndDialogue()
    {
        // Perform any final actions when the dialogue sequence ends
        // For example, deactivate the dialogue UI, activate/deactivate game objects, etc.

        gameObject.SetActive(false); // Deactivate the dialogue UI canvas
    }
}
*/