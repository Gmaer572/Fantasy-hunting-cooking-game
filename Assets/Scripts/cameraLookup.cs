using UnityEngine;
using Unity.Cinemachine;

[ExecuteAlways]
public class cameraLookup : MonoBehaviour
{
    public CinemachineCamera vCam; // Or CinemachineVirtualCamera for 2.x
    [SerializeField] string playerObjectName = "Player_Place";

    void OnEnable()
    {
        RefreshFollowTarget();
    }

    void Update()
    {
        RefreshFollowTarget();
    }

    void RefreshFollowTarget()
    {
        if (vCam == null)
        {
            vCam = GetComponent<CinemachineCamera>();
        }

        if (vCam == null)
        {
            return;
        }

        GameObject playerObject = GameObject.Find(playerObjectName);
        if (playerObject == null)
        {
            return;
        }

        if (vCam.Follow != playerObject.transform)
        {
            vCam.Follow = playerObject.transform;
        }
    }
}
