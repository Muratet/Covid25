using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceRefresh : MonoBehaviour
{
    private void OnEnable()
    {
        SyncUISystem.needUpdate = true;
    }
}
