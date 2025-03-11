using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorldPosition : MonoBehaviour
{
    public static MouseWorldPosition Instance { get; private set; }

    private Controls controls;
    private Ray mouseCameraRay;
    private Vector3 resultPosition;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        controls = new Controls();
        controls.Enable();
    }

    public Vector2 GetPosition()
    {
        mouseCameraRay = Camera.main.ScreenPointToRay(controls.Default.Aim.ReadValue<Vector2>());

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        if (plane.Raycast(mouseCameraRay, out float distance))
        {
            resultPosition = mouseCameraRay.GetPoint(distance);
            return new Vector2(resultPosition.x, resultPosition.z);
        }
        else 
            return Vector2.zero;
    }

    public bool GetClick()
    {
        return controls.Default.Shoot.ReadValue<float>() == 1 ? true : false;
    }
}
