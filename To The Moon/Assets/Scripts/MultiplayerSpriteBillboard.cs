using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class MultiplayerSpriteBillboard : MonoBehaviour
{
    [SerializeField] GameObject shipMain;
    [SerializeField] TMP_Text stateText;
    [SerializeField] TMP_Text textField;
    float maxDistance = 2000;
    [SerializeField] Vector3 defaultScale;
    [SerializeField] RectTransform canvas;
    [SerializeField] PhotonView PV;
   

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<RectTransform>();
        defaultScale = canvas.localScale;
        if (PV && PV.IsMine)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
       
        if (PV.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }

        //if (!Camera.main)
        //{

        //    return;
        //}
        //else
        //{
        //    gameObject.SetActive(true);
        //}

        Camera[] cams = FindObjectsOfType<Camera>();

        foreach(Camera cam in cams)
        {
           if (cam.isActiveAndEnabled && cam.tag != "DeathCam")
            {
                transform.LookAt(cam.transform);

                transform.rotation = cam.transform.rotation;
                float distance = Vector3.Distance(cam.transform.position, this.transform.position);

                float distanceMod = distance / maxDistance;
                textField.text = distance.ToString("####.##");
                stateText.text = PV.Owner.NickName;

                canvas.localScale = defaultScale * distanceMod;
            }
        }
    }
}
