using UnityEngine;

public class ForceRefresh : MonoBehaviour
{
    private void OnEnable()
    {
        SyncUISystem.needUpdate = true;
    }
}
