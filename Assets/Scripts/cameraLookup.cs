using UnityEngine;
using Unity.Cinemachine;
public class cameraLookup : MonoBehaviour
{



    public CinemachineCamera vCam; // Or CinemachineVirtualCamera for 2.x


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vCam = gameObject.GetComponent<CinemachineCamera>();
        vCam.Follow = GameObject.Find("Player_Place").transform;


    }

    // Update is called once per frame
    void Update()
    {
    }
}
