using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DT_EndSceneManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera summitCamera;
    [SerializeField] private GameObject summitDragon;
    [SerializeField] private GameObject summitTrigger;

    private void Awake()
    {
        summitCamera.gameObject.SetActive(false);
        summitDragon.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Prep trigger
        summitTrigger.GetComponent<DT_Trigger>().PrepareTrigger(DT_SO_GameText.GameText.TextType.SpeechBubbles,GameManager.GameScene.None);
        
        //Switch camera
        if (GameManager.CurrentPlayerLevel == GameManager.PlayerLevel.SevenNotes)
        {
            summitCamera.gameObject.SetActive(true);
            mainCamera.gameObject.SetActive(false);
            summitDragon.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
