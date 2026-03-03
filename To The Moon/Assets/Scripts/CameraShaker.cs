using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance;

    [SerializeField] Animator anim;
    [SerializeField] RuntimeAnimatorController controller;
    [SerializeField] string shakeTrigger = "Shake";
    [SerializeField] string loopingTrigger = "Looping";
    [SerializeField] bool looping = false;

    private void Awake()
    {
        Instance = this;
        anim = gameObject.AddComponent<Animator>();
        anim.runtimeAnimatorController = controller;
        anim.applyRootMotion = true;
        if (looping)
        {
            anim.SetBool(loopingTrigger, true);
            anim.SetTrigger(shakeTrigger);
        }
    }

    public void Shake()
    {
        anim.SetTrigger(shakeTrigger);
    }
}
