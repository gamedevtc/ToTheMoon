using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WarpSpeedScript : MonoBehaviour
{
    [SerializeField] VisualEffect warpSpeed;
    [SerializeField] float rate = 0.03f;
    private bool enabled;
    private bool warpActive;
    // Start is called before the first frame update
    void Start()
    {
        warpSpeed.Stop();
        warpSpeed.SetFloat("WarpAmount", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (enabled)
        {
            warpActive = true;
            StartCoroutine(ActivateParticles());
        }
        else
        {
            warpActive = false;
            StartCoroutine(ActivateParticles());
        }
    }
    private void OnEnable()
    {
        enabled = true;
    }
    private void OnDisable()
    {
        enabled = false;
    }

    IEnumerator ActivateParticles()
    {
        if (warpActive)
        {
            warpSpeed.Play();
            float amount = warpSpeed.GetFloat("WarpAmount");
            while (amount <1 && warpActive)
            {
                amount += rate;
                warpSpeed.SetFloat("WarpAmount", amount);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            float amount = warpSpeed.GetFloat("WarpAmount");
            while (amount > 0 && !warpActive)
            {
                amount -= rate;
                warpSpeed.SetFloat("WarpAmount", amount);
                yield return new WaitForSeconds(0.1f);

                if (amount <= 0+rate)
                {
                    amount = 0;
                    warpSpeed.SetFloat("WarpAmount", amount);
                    warpSpeed.Stop();
                }
            }
        }
    }
}
