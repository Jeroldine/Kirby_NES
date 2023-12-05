using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenKirby : MonoBehaviour
{
    int actionChoice;
    float waveChance = 0.01f;
    float blinkChance = 0.01f;

    bool isWaving = false;
    bool isBlinking = false;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isWaving = false;
        isBlinking = false;

        actionChoice = Random.Range(0, 2);
        switch (actionChoice)
        {
            case 0: // wave
                if (Random.Range(0f, 1.0f) < waveChance)
                    isWaving = true;
                break;
            case 1: // blink
                if (Random.Range(0f, 1.0f) < blinkChance)
                    isBlinking = true;
                break;
        }
        anim.SetBool("Wave", isWaving);
        anim.SetBool("Blink", isBlinking);
    }
}
