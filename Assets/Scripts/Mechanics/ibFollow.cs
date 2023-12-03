using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ibFollow : MonoBehaviour
{
    Transform followPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = followPos.position;
    }

    public void SetFollowPos(Transform fp)
    {
        followPos = fp;
    }
}
