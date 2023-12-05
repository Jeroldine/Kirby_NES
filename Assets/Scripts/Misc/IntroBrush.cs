using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBrush : MonoBehaviour
{
    [SerializeField] Animator faceAnim;
    [SerializeField] Animator drawAnim;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PaintToRight()
    {
        anim.SetTrigger("BrushRight");
    }

    public void SmearLeft()
    {
        faceAnim.SetBool("SmearLeft", true);
    }

    public void SmearRight()
    {
        faceAnim.SetBool("SmearLeft", false);
        faceAnim.SetTrigger("SmearRight");
    }

    public void ColourKirby()
    {
        drawAnim.SetTrigger("Paint");
    }
}
