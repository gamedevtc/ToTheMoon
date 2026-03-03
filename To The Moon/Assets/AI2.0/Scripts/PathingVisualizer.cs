using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathingVisualizer : MonoBehaviour
{
    [SerializeField] LineRenderer redLine;
    [SerializeField] LineRenderer greenLine;

    List<Vector3> redPoints = new List<Vector3>();
    List<Vector3> greenPoints = new List<Vector3>();

    public enum visColors
    {
        Red = 0,
        Green
    }

    public void addLine(visColors color, Vector3 start, Vector3 end)
    {
        switch (color)
        {
            case visColors.Red:
                redPoints.Add(start);
                redPoints.Add(end);
                redPoints.Add(start);
                break;
            case visColors.Green:
                greenPoints.Add(start);
                greenPoints.Add(end);
                greenPoints.Add(start);
                break;
            default:
                break;
        }
    }

    public void drawRed()
    {
        redLine.positionCount = redPoints.Count;
        redLine.SetPositions(redPoints.ToArray());
    }

    public void drawGreen()
    {
        greenLine.positionCount = greenPoints.Count;
        greenLine.SetPositions(greenPoints.ToArray());
    }

    public void clearAll()
    {
        greenLine.SetPositions(new Vector3[0]);
        redLine.SetPositions(new Vector3[0]);
        greenLine.positionCount = 0;
        redLine.positionCount = 0;
        redPoints.Clear();
        greenPoints.Clear();
    }
}
