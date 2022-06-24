using UnityEngine;

/// <summary>
/// This component is used on screen data (curves and population pyramid) to force update when it is enabled
/// </summary>
public class ForceRefresh : MonoBehaviour
{
    private void OnEnable()
    {
        SyncUISystem.needUpdate = true;
    }
}
