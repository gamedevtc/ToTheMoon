using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpriteBillboarding : MonoBehaviour
{
    [SerializeField] AIUnit shipMain;
    [SerializeField] TMP_Text stateText;
    [SerializeField] TMP_Text textField;
    float maxDistance = 2000;
    [SerializeField] Vector3 defaultScale;
    [SerializeField] RectTransform canvas;
    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<RectTransform>();
        defaultScale = canvas.localScale;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (!Camera.main)
        {
            gameObject.SetActive(false);
            return;
        }
        transform.LookAt(Camera.main.transform);

        transform.rotation = Camera.main.transform.rotation;

        float distance = Vector3.Distance(Camera.main.transform.position, this.transform.position);

        float distanceMod = distance / maxDistance;
        textField.text = distance.ToString("####.##");
        canvas.localScale = defaultScale * distanceMod;

        if (shipMain)
        {
            if (GameManagerBase.Instance.getDebug_showAIStates())
            {
                stateText.gameObject.SetActive(true);
                stateText.text = shipMain.getState().ToString();
            }
            else
            {
                stateText.gameObject.SetActive(false);
            }
        }
    }
}
