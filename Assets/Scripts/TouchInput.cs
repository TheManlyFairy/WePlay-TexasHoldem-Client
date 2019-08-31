using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TouchInput : MonoBehaviour
{
    public float maxTimeBetweenTaps = 0.2f;
    public LayerMask layerMask;

    [SerializeField]float timeCounter;
    [SerializeField] float tapTime;
    [SerializeField] int tapCount;
    [SerializeField] Vector3 rectStartPos;
    [SerializeField] Vector3 rectDeltaPos;
    [SerializeField] Vector3 touchStartPos;
    [SerializeField] Camera mainCam;
    [SerializeField] Touch touch;
    [SerializeField] RectTransform rectTransform;

    private void Start()
    {
        //rectTransform = GetComponent<RectTransform>();
        mainCam = Camera.main;
        //startPos = rectTransform.position;
        rectDeltaPos = new Vector3(0, 0, mainCam.nearClipPlane);
    }
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            switch (Input.GetTouch(0).phase)
            {
                case TouchPhase.Began:
                    touch = Input.GetTouch(0);
                    if (!ClickedDraggable())
                    {
                        tapCount++;
                    }
                    break;
                case TouchPhase.Moved:
                    if (rectTransform != null)
                        DragHeldObject();
                    break;
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Ended:
                    {
                        if (tapCount == 2)
                        {
                            if (timeCounter <= maxTimeBetweenTaps)
                                Debug.Log("Double Tap");

                            tapCount = 0;
                            timeCounter = 0;
                        }
                        if (rectTransform != null)
                            ReleaseHeldDraggable();
                    }
                    break;
            }
        }
        if (tapCount == 1)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter > maxTimeBetweenTaps)
            {
                tapCount = 0;
                timeCounter = 0;
            }
                
        }
    }
    bool ClickedDraggable()
    {
        RaycastHit2D hit= Physics2D.Raycast(touch.position, Vector2.zero, mainCam.farClipPlane, layerMask);
        //Debug.Log("Touched at " + touch.position);
        if (hit.collider != null)
        {
            Debug.Log("Clicked " + hit.collider.name);
            rectTransform = hit.collider.GetComponent<RectTransform>();
            rectStartPos = rectTransform.position;
            touchStartPos = touch.position;
            return true;
        }
        return false;
    }
    void ReleaseHeldDraggable()
    {
        Debug.Log("Released object");
        rectTransform.position = rectStartPos;
        rectTransform = null;
    }
    void DragHeldObject()
    {
        rectDeltaPos.x = Input.GetTouch(0).position.x - touchStartPos.x;
        rectDeltaPos.y = Input.GetTouch(0).position.y - touchStartPos.y;
        rectTransform.position = rectStartPos + rectDeltaPos;
    }
}
