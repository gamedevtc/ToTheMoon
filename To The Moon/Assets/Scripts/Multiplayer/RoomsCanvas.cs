using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsCanvas : MonoBehaviour
{

    [SerializeField]
    private CreateJoinCanvas _createrJoinCanvas;

    public CreateJoinCanvas CreateJoinCanvas { get { return _createrJoinCanvas; } }


    [SerializeField]
    private CurrentRoomCanvas _currentRoomCanvus;

    public CurrentRoomCanvas CurrentRoomCanvus { get { return _currentRoomCanvus; } }

    private void Awake()
    {
        FirstInitialize();
    }



    private void FirstInitialize()
    {
        CreateJoinCanvas.FirstInitialize(this);
        CurrentRoomCanvus.FirstInitialize(this);
    }
}
