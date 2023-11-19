using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class DT_Trigger : MonoBehaviour
{

    private Collider _thisCollider;
    private DT_GameTextManager _gameTextManager;
    private DT_SO_GameText.GameText.TextType _myTextType;
    private GameManager.GameScene _mySceneToLoad;
    private bool _isPrepared;
    private string _triggerType;
    
    // Trigger handler will tell this object to prepare if it is to be a trigger in this scene
    public void PrepareTrigger(bool displayText, DT_SO_GameText.GameText.TextType textType,
        bool changeScene, GameManager.GameScene sceneToLoad)
    {
        // Find the game text manager
        _gameTextManager = FindObjectOfType<DT_GameTextManager>();

        // Find this object's collider
        _thisCollider = GetComponent<Collider>();

        // Make sure collider is on and trigger enabled
        _thisCollider.enabled = true;
        _thisCollider.isTrigger = true;

        // Assign values received
        _myTextType = textType;
        _mySceneToLoad = sceneToLoad;

        // Determine trigger type
        _triggerType = TriggerType(displayText,changeScene);

        // Mark as prepped
        _isPrepared = true;
    }

    private string TriggerType(bool isTextTrigger, bool isSceneTrigger)
    {
        if (isTextTrigger && isSceneTrigger)
        {
            return "both";
        }
        
        if (isTextTrigger) 
        {
            return "textOnly";
        }

        if (isSceneTrigger)
        {
            return "sceneOnly";
        }

        return null;

    }

    private void OnTriggerEnter(Collider other)
    {
        // Do nothing if not prepared
        if (!_isPrepared) return;
        Debug.Log("=== Trigger Entered ===");
        
        // If trigger enter by Player
        if (other.CompareTag("Player"))
        {
            switch (_triggerType)
            {
                case "both":
                    _gameTextManager.MakeTextSceneRequest(_myTextType, _mySceneToLoad.ToString());
                    break;
                case "textOnly":
                    _gameTextManager.MakeTextSceneRequest(_myTextType, null);
                    break;
                case "sceneOnly":
                    GameManager.LoadScene(_mySceneToLoad.ToString());
                    break;
            }
            // Disable the collider so the trigger can't be set off again
            _thisCollider.enabled = false;
        }
    }
    
}

