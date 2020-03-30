using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panning_zooming : MonoBehaviour
{
    public static Panning_zooming Instance { get; set; }

    [SerializeField] private Vector2 zoomMax = Vector2.zero;
    [SerializeField] private float cameraAcceleration = 0.5f;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float timeTouch = 2.5f;

    private void Awake()
    {
        if (Instance != this)
            Instance = this;
    }

    void Update()
    {
        if (MouseManager.CameraInertie > 0)
        {
            direction = ScreenLimit(direction * MouseManager.CameraInertie, Camera.main.transform.position);
            Camera.main.transform.position += direction * MouseManager.CameraInertie;
            MouseManager.CameraInertie -= cameraAcceleration * Time.deltaTime;
        }
        //COMPUTER
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            Zoom(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            Zoom(Camera.main.ScreenToWorldPoint(Input.mousePosition), -1);
        //...........
    }

    public void Pan(Vector3 pos)
    {
        direction = pos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = ScreenLimit(direction, Camera.main.transform.position);
        Camera.main.transform.position += direction;
    }

    public void Zoom(Vector3 zoomTowards, float amount)
    {
        if ((amount > 0 && Camera.main.orthographicSize > zoomMax.x) || (amount < 0 && Camera.main.orthographicSize < zoomMax.y))
        {
            float multiplier = (1.0f / Camera.main.orthographicSize) * amount;
            Camera.main.transform.position += (zoomTowards - Camera.main.transform.position) * multiplier;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - amount, zoomMax.x, zoomMax.y);
        }
    }

    public Vector3 ScreenLimit(Vector3 direction, Vector3 position)
    {
        if (Mathf.Abs(position.x + direction.x) * 2 > MapManager.Width)
            direction.x = 0;
        if (Mathf.Abs(position.y + direction.y - MapManager.Height * 0.35f) * 3f > MapManager.Height)
            direction.y = 0;
        return direction;
    }

    public float FingersWay(Touch a, Touch b)
    {
        Vector2 touch_zero_prev_pos = a.position - a.deltaPosition;
        Vector2 touch_one_prev_pos = b.position - b.deltaPosition;

        float prev_magnitude = (touch_zero_prev_pos - touch_one_prev_pos).magnitude;
        float current_magnitude = (a.position - b.position).magnitude;
        float difference = current_magnitude - prev_magnitude;
        return Mathf.Abs(difference) < timeTouch ? 0 : difference;
    }
}