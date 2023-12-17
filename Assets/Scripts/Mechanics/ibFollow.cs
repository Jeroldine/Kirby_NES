using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ibFollow : MonoBehaviour
{
    Transform followPos;
    Transform playerPos;
    [SerializeField] float xSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = followPos.position;
    }

    public void SetFollowPos(Transform fp, Transform pPos)
    {
        followPos = fp;
        playerPos = pPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Rigidbody2D eRB = collision.GetComponent<Rigidbody2D>();
            if (eRB)
            {
                collision.GetComponentInChildren<BoxCollider2D>().isTrigger = true;
                eRB.gravityScale = 0;
                eRB.velocity = Vector2.zero;
                Enemy e = collision.GetComponentInChildren<Enemy>();
                e.SetFollowPos(playerPos);
                e.SetInhaledState(true);
                e.inhaledSpeedX = (playerPos.position.x < e.transform.position.x) ? -xSpeed : xSpeed;
                Debug.Log(" playerPos X: " + playerPos.position.x.ToString() + "    Enemy X: " + e.transform.position.x.ToString());
            }
        }
    }
}
