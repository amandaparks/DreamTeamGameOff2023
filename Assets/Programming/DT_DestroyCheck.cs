using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DT_DestroyCheck : MonoBehaviour
{
    private void OnDestroy()
    {
        Debug.Log($"{gameObject} destroyed");
    }
}
