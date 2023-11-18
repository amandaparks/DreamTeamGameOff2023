using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class DT_Trigger : MonoBehaviour
{
    [SerializeField] private bool isDialogueTrigger;
    [SerializeField] private bool isSceneLoadTrigger;
    [SerializeField] private GameManager.GameScene sceneToLoad;
    
    private Collider _thisCollider;
    private DT_GameTextManager _gameTextManager;

    void Start()
    {
        //If this stone is not set to trigger anything, turn off collider and do nothing else
        if (!isDialogueTrigger & !isSceneLoadTrigger)
        {
            GetComponent<Collider>().enabled = false;
            return;
        }
        // Otherwise, find the Game Text Manager and Collider
        _thisCollider = GetComponent<Collider>();
        _gameTextManager = FindObjectOfType<DT_GameTextManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // When player sets off trigger, load dialogue, scene or both
            Debug.Log("Trigger");

            if (isDialogueTrigger & !isSceneLoadTrigger)
            {
                _gameTextManager.MakeTextSceneRequest(DT_SO_GameText.GameText.TextType.Conversation,null);
            }
            else if (isSceneLoadTrigger & !isDialogueTrigger)
            {
                StartCoroutine(GameManager.LoadScene(sceneToLoad.ToString()));
            }
            else if (isDialogueTrigger & isSceneLoadTrigger)
            {
                _gameTextManager.MakeTextSceneRequest(DT_SO_GameText.GameText.TextType.Conversation,sceneToLoad.ToString());
            }

            // Disable trigger so only happens once
            _thisCollider.isTrigger = false;
        }
    }
}
