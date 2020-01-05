using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TouchInput : MonoBehaviour
{
    [SerializeField] float maxTimeBetweenTaps = 0.2f;
    [SerializeField] LayerMask layerMask;

    UIManager uiManager;
    float timeCounter;
    float tapTime;
    int tapCount;
    Vector3 rectStartPos;
    Vector3 rectDeltaPos;
    Vector3 touchStartPos;
    Rect foldActionArea;
    Touch touch;
    Camera mainCam;
    RectTransform rectTransform;

    private void Start()
    {
        mainCam = Camera.main;
        rectDeltaPos = new Vector3(0, 0, mainCam.nearClipPlane);
        foldActionArea = new Rect(Screen.width / 2, Screen.height * 4 / 5, Screen.width * 6 / 8, Screen.height / 5);
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
                                PhotonGameManager.instance.PlayerCheck();

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
        RaycastHit2D hit = Physics2D.Raycast(touch.position, Vector2.zero, mainCam.farClipPlane, layerMask);
        //Debug.Log("Touched at " + touch.position);
        if (hit.collider != null)
        {
            Debug.Log("Clicked " + hit.collider.name);
            rectTransform = hit.collider.GetComponent<RectTransform>();
            rectStartPos = rectTransform.position;
            return true;
        }
        return false;
    }
    void ReleaseHeldDraggable()
    {
        if (foldActionArea.Contains(Input.GetTouch(0).position))
            PhotonGameManager.instance.PlayerFold();
        rectTransform.position = rectStartPos;
        rectTransform = null;
    }
    void DragHeldObject()
    {
        rectDeltaPos.x = Input.GetTouch(0).position.x - rectStartPos.x;
        rectDeltaPos.y = Input.GetTouch(0).position.y - rectStartPos.y;
        rectTransform.position = rectStartPos + rectDeltaPos;
    }
}
