using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target; // Obiekt, który ma byæ œledzony
    public float distance = 5.0f; // Odleg³oœæ kamery od obiektu
    public float xSpeed = 120.0f; // Szybkoœæ obracania kamery w poziomie
    public float ySpeed = 120.0f; // Szybkoœæ obracania kamery w pionie

    public float yMinLimit = 0f; // Ograniczenie dolne k¹ta patrzenia
    public float yMaxLimit = 60f; // Ograniczenie górne k¹ta patrzenia

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Zablokuj kursor w centrum ekranu i ukryj go
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
