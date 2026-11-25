using UnityEngine;
using UnityEngine.Events;

public class ContinuousDragInteraction : MonoBehaviour
{
    public Collider2D targetArea;
    public Transform draggable;

    [HideInInspector]public float requiredTime = 1f;
    [HideInInspector]public float minMovement = 0.02f;

    [HideInInspector] public UnityEvent onCompleted;

    float validTime;
    bool inContact;
    Vector3 lastPos;
    Vector3 offset;

    void Update()
    {
        HandleDrag();

        if (!inContact) return;

        float movement = Vector3.Distance(draggable.position, lastPos);

        if (movement >= minMovement)
        {
            validTime += Time.deltaTime;

            if (validTime >= requiredTime)
                onCompleted?.Invoke();
        }

        lastPos = draggable.position;
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
            offset = draggable.position - MouseWorld();

        if (Input.GetMouseButton(0))
            draggable.position = MouseWorld() + offset;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == targetArea)
        {
            inContact = true;
            lastPos = draggable.position;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other == targetArea)
            inContact = false;
    }

    Vector3 MouseWorld()
    {
        Vector3 m = Input.mousePosition;
        m.z = 10f;
        return Camera.main.ScreenToWorldPoint(m);
    }

    public void ResetInteraction()
    {
        validTime = 0;
        inContact = false;
    }
}
