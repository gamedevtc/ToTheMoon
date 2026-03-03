using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AIAssistant : MonoBehaviour
{
   [SerializeField] bool debug = false;
    public static AIAssistant Instance;

    const int numRayDirections = 300;

    public static readonly Vector3[] directions;

    private void Awake()
    {
        if (debug)
        {
            Instance = this;
            return;
        }
        if (GameManagerBase.Instance.isMulti() && !PhotonNetwork.IsMasterClient)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    static AIAssistant()
    {
        directions = new Vector3[numRayDirections];

        float goldenRatio = (1 + Mathf.Sqrt(5)) * 0.5f;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numRayDirections; i++)
        {
            float t = (float)i / numRayDirections;
            float incline = Mathf.Acos(1 - 2 * t);
            float az = angleIncrement * i;

            float x = Mathf.Sin(incline) * Mathf.Cos(az);
            float y = Mathf.Sin(incline) * Mathf.Sin(az);
            float z = Mathf.Cos(incline);

            directions[i] = new Vector3(x, y, z);
        }
    }
}
