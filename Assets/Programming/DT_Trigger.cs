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
    private GameManager _gameManager;
    private Collider _thisCollider;

    void Start()
    {
        // Find the Game Manager and assign it to this variable
        _gameManager = GameManager.Instance;
        _thisCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // When player sets off trigger, load dialogue, scene or both
            Debug.Log("Trigger");
            if (isDialogueTrigger && !isSceneLoadTrigger)
            {
                //load dialogue
            }
            else if (isSceneLoadTrigger && !isDialogueTrigger)
            {
                StartCoroutine(_gameManager.LoadScene(sceneToLoad.ToString()));
            }
            else if (isDialogueTrigger && isSceneLoadTrigger)
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
