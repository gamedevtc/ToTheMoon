using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player4Camera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] Transform cameraPos;
    [SerializeField] Transform cameraPosUp;
    [SerializeField] Transform cameraPosDown;
    [SerializeField] Camera myCamera;
    [SerializeField] private float cameraSmoother = 4.0f;
    [SerializeField] RectTransform crosshair;
    [SerializeField] Player4Base player;
    [SerializeField] GameObject m_audioSource;

    Player4Base.HUDData data = new Player4Base.HUDData();

    private void Awake()
    {
        m_audioSource = this.gameObject;
    }
    public void setCameraActive(bool io)
    {
        m_audioSource.SetActive(false);
        myCamera.gameObject.SetActive(io);
    }

    public void setCrosshairActive(bool io)
    {
        crosshair.gameObject.SetActive(io);
    }

    private void FixedUpdate()
    {
       


        player.updateHUD(out data);
        if (!data.pausedState)
        {
            if (!crosshair.gameObject.activeInHierarchy)
            {
                crosshair.gameObject.SetActive(true);
            }
            crosshair.position = myCamera.WorldToScreenPoint(transform.position + transform.forward * 100);

        }
        else
        {
            if (crosshair.gameObject.activeInHierarchy)
            {
                crosshair.gameObject.SetActive(false);
            }
        }
        if (data.pausedState && !GameManagerBase.Instance.isMulti())
        {
            return;
        }
        #region CameraUpdate
        float camDistance = Vector3.Distance(this.transform.position, myCamera.transform.position);
        cameraPos.position = Vector3.Lerp(cameraPosDown.position, cameraPosUp.position, camDistance * Time.deltaTime);

        myCamera.transform.position = Vector3.Lerp(myCamera.transform.position, cameraPos.position, Time.deltaTime * cameraSmoother);
        myCamera.transform.rotation = Quaternion.Lerp(myCamera.transform.rotation, cameraPos.rotation, Time.deltaTime * cameraSmoother);
        #endregion
    }
}
