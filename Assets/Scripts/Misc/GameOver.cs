using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    Animator anim;

    float pic1Duration;
    float fadeDuration;
    float pic2Duration;

    // Start is called before the first frame update
    void Start()
    {
       Destroy(GameManager.Instance.playerInstance.gameObject);
       anim = GetComponent<Animator>();
       GetAnimClipDurations();
       GameManager.Instance.DisableInput(true, (pic1Duration + fadeDuration + pic2Duration));
    }

    private void GetAnimClipDurations()
    {
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            switch (clip.name)
            {
                case "pic1":
                    pic1Duration = clip.length;
                    break;
                case "fade":
                    fadeDuration = clip.length;
                    break;
                case "pic2":
                    pic2Duration = clip.length;
                    break;
            }
        }
    }
}
