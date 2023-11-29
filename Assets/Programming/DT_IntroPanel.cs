using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DT_IntroPanel : MonoBehaviour
{
    // Turn self off unless this is the intro
    private void OnEnable()
    {
        if (GameManager.CurrentPlayerLevel != GameManager.PlayerLevel.NewGame)
        {
            gameObject.SetActive(false);
        }
    }
}
