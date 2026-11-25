using UnityEngine;

/// <summary>
/// Helper de input que funciona tanto em desktop (mouse) quanto em mobile (touch).
/// Uso: substituir Input.GetMouseButton(0) por InputHelper.GetButtonDown(),
///      Input.GetMouseButtonUp(0) por InputHelper.GetButtonUp(),
///      Input.mousePosition por InputHelper.GetPosition().
/// </summary>
public static class InputHelper
{
    public static bool GetButtonDown()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).phase == TouchPhase.Began;
        return Input.GetMouseButtonDown(0);
    }

    public static bool GetButton()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary;
        return Input.GetMouseButton(0);
    }

    public static bool GetButtonUp()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled;
        return Input.GetMouseButtonUp(0);
    }

    public static Vector3 GetPosition()
    {
        if (Input.touchCount > 0)
        {
            Vector3 pos = Input.GetTouch(0).position;
            pos.z = 0f;
            return pos;
        }
        return Input.mousePosition;
    }

    public static Vector3 GetWorldPosition(Camera cam)
    {
        Vector3 pos = GetPosition();
        Vector3 worldPos = cam.ScreenToWorldPoint(pos);
        worldPos.z = 0f;
        return worldPos;
    }

    public static bool HasInput()
    {
        return Input.touchCount > 0 || Input.GetMouseButton(0);
    }
}
