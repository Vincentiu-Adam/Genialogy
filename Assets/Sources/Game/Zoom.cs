using UnityEngine;

public class Zoom : MonoBehaviour
{
    [SerializeField]
    private float m_Speed;

    [SerializeField]
    private Vector2 m_Limits; //x - lower limit, y - higher limit

    [SerializeField]
    private Canvas m_Canvas;

    private void Update()
    {
        float zoomIncrease = Input.GetAxis("Zoom") * m_Speed * Time.deltaTime;
        SetZoom(zoomIncrease);
    }

    private void SetZoom(float increase)
    {
        float zoom = Mathf.Clamp(m_Canvas.scaleFactor + increase, m_Limits.x, m_Limits.y);
        m_Canvas.scaleFactor = zoom;
    }
}
