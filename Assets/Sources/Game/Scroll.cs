using UnityEngine;
using UnityEngine.EventSystems;

public class Scroll : MonoBehaviour 
{
    [SerializeField]
    private Transform m_Board;

    [SerializeField]
    private Vector2 m_Limits; //x - limit on horiz from -value to + value; y - same for vertical
    private Vector3 m_Delta = Vector3.zero;

    public void Drag(BaseEventData dragData)
    {
        PointerEventData pointerData = (PointerEventData) dragData;
        m_Delta.x = pointerData.delta.x;
        m_Delta.y = pointerData.delta.y; 

        Vector3 position = m_Board.transform.localPosition + m_Delta;
        position.x = Mathf.Clamp(position.x, -m_Limits.x, m_Limits.x);
        position.y = Mathf.Clamp(position.y, -m_Limits.y, m_Limits.y);

        m_Board.transform.localPosition = position;
    }

    private void OnValidate()
    {
        m_Limits.x = Mathf.Abs(m_Limits.x);
        m_Limits.y = Mathf.Abs(m_Limits.y);
    }
}
