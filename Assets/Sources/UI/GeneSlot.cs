using UnityEngine;
using UnityEngine.UI;

public class GeneSlot : MonoBehaviour
{
    [SerializeField]
    private Image m_Sprite;

    public Image Image { get { return m_Sprite; } }
}
