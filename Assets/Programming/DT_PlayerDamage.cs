using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DT_PlayerDamage : MonoBehaviour
{
    [Header("DAMAGE RECEIVER")]
    [Header(" -Makes Player fly back to start after damage")]
    [Space]
    
    [SerializeField] private float timeToReturn;
    public void DamagePlayer()
    {
        Debug.Log("PLAYER DAMAGED");
        // Update state (this also triggers animation)
        GameManager.CurrentPlayerState = GameManager.PlayerState.Damaged;
        
        // Send player back to start
        StartCoroutine(BackToStart());
        

    }

    private IEnumerator BackToStart()
    {
        //Set the target points
        var startPoint = transform.position;
        var endPoint = GameManager.PlayerStartPos;

        // Start counting
        float elapsedTime = 0f;
        
        // Climb until there is no time left
        while (elapsedTime < timeToReturn)
        {
            // Move to where Player should be this frame
            transform.position = Vector3.Lerp(startPoint, endPoint, elapsedTime / timeToReturn);

            // Increment for next frame
            elapsedTime += Time.deltaTime;

            // Wait
            yield return null;
        }
        
        // Set state back to Idle
        GameManager.CurrentPlayerState = GameManager.PlayerState.Idle;
    }
}
