using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DT_TriggerHandler : MonoBehaviour
{
    [Header("LEVEL TRIGGERS")]
    [Header(" -Drag in a StepStone")]
    [Header(" -Choose text and/or scene change")]
    
    [Space]
    [SerializeField] private DT_TriggerSettings[] triggers;

    private Collider _thisCollider;
    private DT_GameTextManager _gameTextManager;

    void Start()
    {
        // If this level has triggers, prepare them, otherwise log warning
        
        if (triggers != null)
        {
            PrepareTriggers();
            return;
        }

        Debug.LogWarning($"No triggers assigned for this level.");

    }

    void PrepareTriggers()
    {
        foreach (DT_TriggerSettings trigger in triggers)
        {
            //Send the details to the trigger object
            trigger.stepStone.GetComponent<DT_Trigger>().PrepareTrigger(trigger.textToLoad,trigger.sceneToLoad);
        }
    }
}

[System.Serializable]

public class DT_TriggerSettings
{
    public GameObject stepStone;
    public DT_SO_GameText.GameText.TextType textToLoad;
    public GameManager.GameScene sceneToLoad;
}