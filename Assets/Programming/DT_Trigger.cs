using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class DT_Trigger : MonoBehaviour
{
    [SerializeField] private bool isDialogueTrigger;
    [SerializeField] private bool isSceneLoadTrigger;

    [SerializeField] private GameManager.CurrentScene sceneToLoad;
    [Header("For Playtest Builds (Will Override Scene To Load):")]
    [SerializeField] private string sceneName;
    private GameManager _gameManager;
    private Collider _thisCollider;

    void Start()
    {
        //If this stone is not set to trigger anything, turn off collider and do nothing else
        if (!isDialogueTrigger & !isSceneLoadTrigger)
        {
            GetComponent<Collider>().enabled = false;
            return;
        }
        // Otherwise, find the Game Manager and Collider
        _gameManager = GameManager.Instance;
        _thisCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // When player sets off trigger, load dialogue, scene or both
            Debug.Log("Trigger");
            
            //THIS PART FOR TESTING ONLY

            if (sceneName!= null)
            {
                StartCoroutine(_gameManager.LoadScene(sceneName));
                return;
            }
            //END OF TESTING SECTION
            
            if (isDialogueTrigger & !isSceneLoadTrigger)
            {
                //load dialogue
            }
            else if (isSceneLoadTrigger & !isDialogueTrigger)
            {
                StartCoroutine(_gameManager.LoadScene(sceneToLoad.ToString()));
            }
            else if (isDialogueTrigger & isSceneLoadTrigger)
            {
                StartCoroutine(TalkAndLeave());
            }

            // Disable trigger so only happens once
            _thisCollider.isTrigger = false;
        }
    }

    private IEnumerator TalkAndLeave()
    {
        // Talk
        
        // Wait until done
        
        // Load scene
        yield return null;
    }

}
