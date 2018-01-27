using UnityEngine;
using UnityEngine.UI;

using System;

public class GeneSlot : MonoBehaviour
{
    [SerializeField]
    private Image m_Sprite;

    [SerializeField]
    private Image m_SpriteOutline;

    [SerializeField]
    private Toggle m_ToggleIsDominant;

    public Image Image        { get { return m_Sprite; } }
    public Image ImageOutLine { get { return m_SpriteOutline; } }

    private GeneType m_GeneType;

    private Action<GeneType> OnDominantToggleClicked;

    private bool m_SkipCallback;

    public void Init(GeneType geneType, string geneSlotName, Action<GeneType> callback)
    {
        m_GeneType = geneType;

        gameObject.name = geneSlotName + "_" + geneType;

        OnDominantToggleClicked += callback;
    }

    public void RemoveCallback(Action<GeneType> callback)
    {
        OnDominantToggleClicked -= callback;
    }

    public void EnableToggle(bool isEnabled)
    {
        m_SkipCallback = isEnabled;

        m_ToggleIsDominant.isOn = isEnabled;
    }

    public void DisableToggle()
    {
        m_ToggleIsDominant.gameObject.SetActive(false);
    }

    public void OnDominantClicked()
    {
        if (m_SkipCallback)
        {
            m_SkipCallback = false;
            return;
        }

        m_SkipCallback = true;

        //disable toggle and handle it via callback
        m_ToggleIsDominant.isOn = false;

        if (OnDominantToggleClicked != null)
        {
            OnDominantToggleClicked(m_GeneType);
        }
    }
}
