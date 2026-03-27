using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Camera playerCam;

    void Update()
    {
        transform.LookAt(playerCam.transform);
    }
}
