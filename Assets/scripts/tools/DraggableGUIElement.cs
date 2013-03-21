using UnityEngine;
using System.Collections;
 
public class DraggableGUIElement : MonoBehaviour
{
    [System.Serializable]
    public class Border
    {
        public float minX,maxX,minY,maxY;
    }
 
    public Border border;
 
    Vector3 lastMousePosition;
 
    void OnMouseDown()
    {
        lastMousePosition = GetClampedMousePosition();
    }
 
    Vector3 GetClampedMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
        mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);
 
        return mousePosition;
    }
 
    void OnMouseDrag()
    {
        Vector3 delta = GetClampedMousePosition() - lastMousePosition;
 
        delta = Camera.main.ScreenToViewportPoint(delta);
 
        transform.position += delta;
 
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, border.minX, border.maxX);
        position.y = Mathf.Clamp(position.y, border.minY, border.maxY);
 
        transform.position = position;
 
        lastMousePosition = GetClampedMousePosition();
    }
}