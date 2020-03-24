using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panning_zooming : MonoBehaviour
{
    Vector3 touch_start;
    public float zoom_out_min;
    public float zoom_out_max;
    private Map_manager map;

    float cameraSpeed = 0f;
    float cameraAcceleration = 0.0006f;
    Vector3 direction;

    void Awake()
    {
        map = GetComponent<Map_manager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touch_start = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cameraSpeed = 0f;
        }
        if (Input.touchCount == 2 && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled))
            touch_start = Camera.main.ScreenToWorldPoint(Input.GetTouch(1).position);
        else if (Input.touchCount == 2 && (Input.GetTouch(1).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Canceled))
            touch_start = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        else if (Input.touchCount == 2)
        {
            Touch touch_zero = Input.GetTouch(0);
            Touch touch_one = Input.GetTouch(1);

            if (touch_zero.phase == TouchPhase.Moved && touch_one.phase == TouchPhase.Moved)
            {
                Vector2 touch_zero_prev_pos = touch_zero.position - touch_zero.deltaPosition;
                Vector2 touch_one_prev_pos = touch_one.position - touch_one.deltaPosition;

                float prev_magnitude = (touch_zero_prev_pos - touch_one_prev_pos).magnitude;
                float current_magnitude = (touch_zero.position - touch_one.position).magnitude;

                float difference = current_magnitude - prev_magnitude;

                if (difference > 0)
                    difference = 1;
                else if (difference < 0)
                    difference = -1;
                Zoom(Camera.main.ScreenToWorldPoint((touch_zero.position + touch_one.position) / 2), difference * 0.6f);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            direction = touch_start - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = ScreenLimit(direction, Camera.main.transform.position);
            Camera.main.transform.position += direction;// * Time.deltaTime;

        }
        else if (Input.GetMouseButtonUp(0))
            cameraSpeed = 0.96f;
        if (cameraSpeed > 0)
        {
            direction = ScreenLimit(direction * cameraSpeed, Camera.main.transform.position);
            Camera.main.transform.position += direction * cameraSpeed;
            cameraSpeed -= cameraAcceleration; 
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // only for computer, delete for mobile
            Zoom(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            Zoom(Camera.main.ScreenToWorldPoint(Input.mousePosition), -1);
    }

    private void Zoom(Vector3 zoomTowards, float amount)
    {
        if ((amount > 0 && Camera.main.orthographicSize > zoom_out_min) || (amount < 0 && Camera.main.orthographicSize < zoom_out_max))
        {
            float multiplier = (1.0f / Camera.main.orthographicSize) * amount;
            Camera.main.transform.position += (zoomTowards - Camera.main.transform.position) * multiplier;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - amount, zoom_out_min, zoom_out_max);
        }
    }
/*    private void Zoom(float increment)
    {
        if (increment > 0)
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoom_out_min, zoom_out_max);
    }
    */

    private Vector3 ScreenLimit(Vector3 direction, Vector3 position)
    {
        if (Mathf.Abs(position.x + direction.x) * 2 > map.width)
            direction.x = 0;
        if (Mathf.Abs(position.y + direction.y - map.height * 0.35f) * 3f > map.height)
            direction.y = 0;
        return direction;
    }
}
