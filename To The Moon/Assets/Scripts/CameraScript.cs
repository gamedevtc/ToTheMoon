using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 testDir;
   
    private Vector3 offset;
    float distance;
    Vector3 playerPrevPos, playerMoveDir;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
        distance = offset.magnitude;
        playerPrevPos = player.transform.position;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 5.0f, player.transform.position.z - 10.0f);
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
            Debug.DrawRay(player.transform.position, player.transform.forward, Color.green, 10000);

            playerMoveDir = player.transform.position - playerPrevPos;
            if (playerMoveDir != Vector3.zero)
            {
                playerMoveDir.Normalize();
                transform.position = player.transform.position - playerMoveDir * distance;
                Vector3 pos = transform.position;
                pos.y += 5.0f;
                pos.z -= 10.0f;
                transform.position = pos;
                transform.LookAt(player.transform.position + (player.transform.forward * 10));
                testDir = player.transform.forward;
                playerPrevPos = player.transform.position;
            }
    }
}
