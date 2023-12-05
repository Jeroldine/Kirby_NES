using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroKirby : MonoBehaviour
{
    [SerializeField] Animator faceAnim;
    [SerializeField] Animator instructionAnim;
    [SerializeField] Animator brushAnim;
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

    public void DrawEyes()
    {
        faceAnim.SetTrigger("DrawEyes");
    }

    public void ShowInstruction2()
    {
        instructionAnim.SetTrigger("Step2");
    }

    public void PaintToLeft()
    {
        brushAnim.SetBool("BrushLeft", true);
    }

    public void PanOut()
    {
        anim.SetTrigger("PanOut");
    }

    public void GoToTitleScreen()
    {
        GameManager.Instance.currentSceneIndex++;
        GameManager.Instance.LoadLevel(GameManager.Instance.currentSceneIndex);
    }

}
