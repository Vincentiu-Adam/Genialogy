using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

[System.Serializable]
public class AlienGeneValue
{
    public bool IsDominant;

    public int Value;

    public GeneScriptableObject GeneData;

    public GeneSlot GeneSlot;
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

    public void Init(List<GeneType> pickedGenes, GeneSlot geneSlotPrefab)
    {
        //instantiate genes and add to values
        for (int i = 0; i < pickedGenes.Count; i++)
        {
            AlienGeneValue currentGeneValue = GetGeneFromType(pickedGenes[i]);

            GeneSlot geneSlot = Object.Instantiate(geneSlotPrefab, m_GeneContainer);
            geneSlot.name = geneSlotPrefab.name + "_" + currentGeneValue.GeneData.Type;

            currentGeneValue.GeneSlot = geneSlot;
            SetGeneImage(currentGeneValue);
        }

        //disable background image, only used for scene preview
        GetComponent<Image>().enabled = false;
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
        //leave empty if negative
        if (geneValue.Value < 0 || geneValue.Value >= geneValue.GeneData.Values.Count)
        {
            return;
        }

        geneValue.GeneSlot.Image.sprite = geneValue.GeneData.Values[geneValue.Value].Image;
    }
}