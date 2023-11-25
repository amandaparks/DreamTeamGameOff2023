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
    private DT_EnemyManager _enemyManager;
    private bool _isPrepared;
    private string _triggerType;
    private bool _pauseMovingEnemies;
    private bool _pauseStationaryEnemies;

    public void Start()
    {
        // Find the game text manager
        _gameTextManager = FindObjectOfType<DT_GameTextManager>();

        // Find this object's collider
        _thisCollider = GetComponent<Collider>();
        
        // Find the enemy manager
        _enemyManager = FindObjectOfType<DT_EnemyManager>();

    }

    // Trigger handler will tell this object to prepare if it is to be a trigger in this scene
    public void PrepareTrigger(DT_SO_GameText.GameText.TextType textToLoad, GameManager.GameScene sceneToLoad)
    {
        // Make sure collider is on and trigger enabled
        _thisCollider.enabled = true;
        _thisCollider.isTrigger = true;

        // Assign values received
        _myTextType = textToLoad;
        _mySceneToLoad = sceneToLoad;

        // Determine trigger type
        _triggerType = TriggerType(textToLoad,sceneToLoad);

        if (_triggerType != null)
        {
            // Mark as prepped
            _isPrepared = true;
        }
    }

    private string TriggerType(DT_SO_GameText.GameText.TextType textToLoad, GameManager.GameScene sceneToLoad)
    {
        // If both are not None
        if (textToLoad != DT_SO_GameText.GameText.TextType.None && sceneToLoad != GameManager.GameScene.None)
        {
            Debug.Log($"Trigger prepped: load {textToLoad} text and {sceneToLoad} scene.");
            return "both";
        }
        // If text is not None
        if (textToLoad != DT_SO_GameText.GameText.TextType.None) 
        {
            Debug.Log($"Trigger prepped: load {textToLoad} text only.");
            return "textOnly";
        }
        // If scene is not None
        if (sceneToLoad != GameManager.GameScene.None)
        {
            Debug.Log($"Trigger prepped: load {sceneToLoad} scene only.");
            return "sceneOnly";
        }

        return null;

    }

    private void OnTriggerEnter(Collider other)
    {
        // Do nothing if not prepared
        if (!_isPrepared) return;
        
        // If trigger enter by Player
        if (other.CompareTag("Player"))
        {
            Debug.Log("=== Trigger Entered ===");
            
            switch (_triggerType)
            {
                case "both":
                    _gameTextManager.MakeTextSceneRequest(_myTextType, _mySceneToLoad);
                    break;
                case "textOnly":
                    _gameTextManager.MakeTextSceneRequest(_myTextType, GameManager.GameScene.None);
                    break;
                case "sceneOnly":
                    StartCoroutine(GameManager.LoadScene(_mySceneToLoad));
                    break;
            }
            
            // Disable the collider so the trigger can't be set off again
            _thisCollider.enabled = false;
        }
    }
    
}

