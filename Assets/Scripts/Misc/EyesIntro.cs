using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesIntro : MonoBehaviour
{
    [SerializeField] Animator instructionAnim;
    [SerializeField] Animator drawingAnim;
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

    public void ShowInstruction3()
    {
        instructionAnim.SetTrigger("Step3");
    }

    public void ShowInstruction4()
    {
        instructionAnim.SetTrigger("Step4");
    }

    public void DrawMouth()
    {
        anim.SetTrigger("DrawMouth");
    }

    public void DrawLimbs()
    {
        drawingAnim.SetTrigger("DrawLimbs");
    }

    public void BrushToLeft()
    {

    }
}
