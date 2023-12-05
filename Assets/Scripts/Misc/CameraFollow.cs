using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    //Transform player;

    [SerializeField] float  minXClamp;
    [SerializeField] float maxXClamp;


    void LateUpdate()
    {
        Vector3 cameraPos;

        cameraPos = transform.position;
        cameraPos.x = Mathf.Clamp(GameManager.Instance.playerInstance.transform.position.x, minXClamp, maxXClamp);

        transform.position = cameraPos;
    }
}
