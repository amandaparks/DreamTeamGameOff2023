using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DT_TriggerHandler : MonoBehaviour
{
    [Header("Add game objects and assign text and/or scene to trigger.")]
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
            //Send all the details to the trigger object
            trigger.objectWithTrigger.GetComponent<DT_Trigger>().PrepareTrigger(trigger.displayText, trigger.textType,
                trigger.changeScene, trigger.sceneToLoad);
        }
    }
}

[System.Serializable]

public class DT_TriggerSettings
{
    public GameObject objectWithTrigger;
    public bool displayText;
    public DT_SO_GameText.GameText.TextType textType;
    public bool changeScene;
    public GameManager.GameScene sceneToLoad;
}