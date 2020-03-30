using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance { get; set; }

    [SerializeField] private Vector3 mousePos;
    [SerializeField] private float timeMouseDown = 0f;
    [SerializeField] private float cameraInertie = 0f;
    [SerializeField] GameConsts.MOUSE_STATE mouseState;

    public static GameConsts.MOUSE_STATE MouseState
    {
        get { return Instance.mouseState; }
        set { Instance.mouseState = value; }
    }
    public static Vector3 MousePos
    {
        get { return Instance.mousePos; }
        set { MousePos = value; }
    }
    public static float TimeMouseDown
    {
        get { return Instance.timeMouseDown; }
        set { TimeMouseDown = value; }
    }
    public static float CameraInertie
    {
        get { return Instance.cameraInertie; }
        set { Instance.cameraInertie = value; }
    }

    private void Awake()
    {
        if (Instance != this)
            Instance = this;
    }

    private void Update()
    {
        switch(mouseState)
        {
            case GameConsts.MOUSE_STATE.NONE:
                {
                    NoState();
                    break;
                }
            case GameConsts.MOUSE_STATE.CLICK:
                {
                    ClickState();
                    break;
                }
            case GameConsts.MOUSE_STATE.ZOOM:
                {
                    ZoomState();
                    break;
                }
            case GameConsts.MOUSE_STATE.DRAGGING:
                {
                    DraggingState();
                    break;
                }
        }
    }

    void NoState()
    {
        if (Input.touchCount >= 2)
            ThisToZoom();
        else if (Input.GetMouseButtonDown(0))
        {
            timeMouseDown = Time.time;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cameraInertie = 0f;
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                Component obj = hit.collider.gameObject.GetComponent(typeof(IInteractable));
                (obj as IInteractable).ActionDown();
            }
        }
        else if (Input.GetMouseButton(0))
            mouseState = GameConsts.MOUSE_STATE.CLICK;
    }
    void ClickState()
    {
        if (Input.touchCount >= 2)
            ThisToZoom();
        else if (Input.GetMouseButtonUp(0))
        {
            mouseState = GameConsts.MOUSE_STATE.NONE;
            cameraInertie = 0.96f;
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (Input.touchCount < 2 && Time.time - timeMouseDown < 0.15f)
            {
                MapManager.Map.SelectTile();
                if (hit.collider != null)
                    hit.collider.gameObject.GetComponent<IInteractable>().ActionUp();
            }
            else if (Input.touchCount < 2 && Time.time - timeMouseDown >= 0.15f)
            {
                if (hit.collider != null)
                    hit.collider.gameObject.GetComponent<IInteractable>().ActionCanceled();
            }

        }
        else
            Panning_zooming.Instance.Pan(mousePos);
    }
    void ZoomState()
    {
        Touch finger1 = Input.GetTouch(0);
        Touch finger2 = Input.GetTouch(1);
        if (finger1.phase == TouchPhase.Canceled || finger1.phase == TouchPhase.Ended)
        {
            if (Input.GetTouch(1).phase == TouchPhase.Canceled || Input.GetTouch(1).phase == TouchPhase.Ended)
            {
                mouseState = GameConsts.MOUSE_STATE.NONE;
                return;
            }
            ZoomToClick(1);
        }
        else if (Input.GetTouch(1).phase == TouchPhase.Canceled || Input.GetTouch(1).phase == TouchPhase.Ended)
        {
            if (finger1.phase == TouchPhase.Canceled || finger1.phase == TouchPhase.Ended)
            {
                mouseState = GameConsts.MOUSE_STATE.NONE;
                return;
            }
            ZoomToClick(0);
        }
        else
        {
            float fingerWay = Panning_zooming.Instance.FingersWay(finger1, finger2);
            Vector2 interPos = Camera.main.ScreenToWorldPoint((finger1.position + finger2.position) * 0.5f);
            Panning_zooming.Instance.Zoom(interPos, fingerWay * Time.deltaTime);
            Panning_zooming.Instance.Pan(mousePos);
        }
    }
    void DraggingState()
    {
        if (Input.GetMouseButtonUp(0))
        {
            MouseState = GameConsts.MOUSE_STATE.NONE;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.touchCount < 2)
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit.collider != null)
                    hit.collider.gameObject.GetComponent<IInteractable>().ActionUp();
            }
        }
    }

    private void ThisToZoom()
    {
        mouseState = GameConsts.MOUSE_STATE.ZOOM;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void ZoomToClick(int touchOnScreen)
    {
        mouseState = GameConsts.MOUSE_STATE.CLICK;
        mousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(touchOnScreen).position);
    }
}