using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

[System.Serializable]
public class AlienGeneValue
{
    public bool IsDominant;

    public int Value;

    public GeneScriptableObject GeneData;
}

public class Alien : MonoBehaviour
{
    [SerializeField]
    private List<AlienGeneValue> m_GeneValues;

    [SerializeField]
    private Transform m_GeneContainer;

    [SerializeField]
    private Alien m_Child;

    [SerializeField]
    private List<Alien> m_Parents;

    public Alien Child         { get { return m_Child; } }
    public List<Alien> Parents { get { return m_Parents; } }

    private List<Image> m_GeneImages = new List<Image>();

    private void Awake()
    {
        foreach(Transform child in m_GeneContainer)
        {
            m_GeneImages.Add(child.GetComponent<Image>());
        }

        Init();
    }

    private void Init()
    {
        for (int i = 0; i < m_GeneValues.Count; i++)
        {
            SetGeneImage(m_GeneValues[i]);
        }
    }

    public AlienGeneValue GetGeneFromType(GeneType geneType)
    {
        for (int i = 0; i < m_GeneValues.Count; i++)
        {
            AlienGeneValue currentGeneValue = m_GeneValues[i];
            if (currentGeneValue.GeneData.Type == geneType)
            {
                return currentGeneValue;
            }
        }

        return null;
    }

    public void SetGeneData(AlienGeneData geneData)
    {
        AlienGeneValue geneValue = GetGeneFromType(geneData.GeneType);
        geneValue.Value = geneData.Value;

        SetGeneImage(geneValue);
    }

    private void SetGeneImage(AlienGeneValue geneValue)
    {
        int index = m_GeneValues.IndexOf(geneValue);
        m_GeneImages[index].sprite = geneValue.GeneData.Values[geneValue.Value].Image;
    }
}