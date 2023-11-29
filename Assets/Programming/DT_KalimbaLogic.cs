using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DT_KalimbaLogic : MonoBehaviour
{
    [SerializeField] private GameObject scale1;
    [SerializeField] private GameObject scale2;
    [SerializeField] private GameObject scale3;
    [SerializeField] private GameObject scale4;
    [SerializeField] private GameObject scale5;
    [SerializeField] private GameObject scale6;
    [SerializeField] private GameObject scale7;
    
    private Animator _kalimbaAnimator;
    private static readonly int Hover = Animator.StringToHash("Hover");
    private static readonly int IntroFinish = Animator.StringToHash("IntroFinish");

    // Start is called before the first frame update
    void Start()
    {
        // Get animator
        _kalimbaAnimator = GetComponent<Animator>();
        
        // Enable scales depending on player level
        EnableScales();
        
        // If it's not a new game, stop here
        if (GameManager.CurrentPlayerLevel != GameManager.PlayerLevel.NewGame) return;
        
        // Run opening coroutine
        StartCoroutine(Opening());
    }

    private IEnumerator Opening()
    {
        yield return new WaitForFixedUpdate();
        
        // Play start animation
        _kalimbaAnimator.SetTrigger(Hover);
        
        Debug.Log("KALIMBA SAYS new game");
        // Wait until state is no longer "new game"
        while (GameManager.CurrentPlayerLevel == GameManager.PlayerLevel.NewGame)
        {
            yield return null;
        }
        Debug.Log("KALIMBA SAYS not new game");
        
        // Play end animation
        _kalimbaAnimator.SetTrigger(IntroFinish);
    }

    private void EnableScales()
    {
        // First, Disable all scales
        scale1.SetActive(false);
        scale2.SetActive(false);
        scale3.SetActive(false);
        scale4.SetActive(false);
        scale5.SetActive(false);
        scale6.SetActive(false);
        scale7.SetActive(false);
        
        
        // Then, Enable depending on level
        switch (GameManager.CurrentPlayerLevel)
        {
            case GameManager.PlayerLevel.NewGame:
                scale5.SetActive(true);
                break;
            case GameManager.PlayerLevel.OneNote:
                scale5.SetActive(true);
                break;
            case GameManager.PlayerLevel.TwoNotes:
                scale4.SetActive(true);
                scale5.SetActive(true);
                break;
            case GameManager.PlayerLevel.ThreeNotes:
                scale3.SetActive(true);
                scale4.SetActive(true);
                scale5.SetActive(true);
                break;
            case GameManager.PlayerLevel.FourNotes:
                scale2.SetActive(true);
                scale3.SetActive(true);
                scale4.SetActive(true);
                scale5.SetActive(true);
                break;
            case GameManager.PlayerLevel.FiveNotes:
                scale2.SetActive(true);
                scale3.SetActive(true);
                scale4.SetActive(true);
                scale5.SetActive(true);
                scale6.SetActive(true);
                break;
            case GameManager.PlayerLevel.SixNotes:
                scale1.SetActive(true);
                scale2.SetActive(true);
                scale3.SetActive(true);
                scale4.SetActive(true);
                scale5.SetActive(true);
                scale6.SetActive(true);
                break;
            case GameManager.PlayerLevel.SevenNotes:
                scale1.SetActive(true);
                scale2.SetActive(true);
                scale3.SetActive(true);
                scale4.SetActive(true);
                scale5.SetActive(true);
                scale6.SetActive(true);
                scale7.SetActive(true);
                break;
            case GameManager.PlayerLevel.Winner:
                scale1.SetActive(true);
                scale2.SetActive(true);
                scale3.SetActive(true);
                scale4.SetActive(true);
                scale5.SetActive(true);
                scale6.SetActive(true);
                scale7.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
