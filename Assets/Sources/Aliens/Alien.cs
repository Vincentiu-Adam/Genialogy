using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

using Object = UnityEngine.Object;

[System.Serializable]
public class AlienGeneValue
{
    public bool IsDominant;

    public int Value;

    public GeneScriptableObject GeneData;

    public GeneSlot GeneSlot;

    public void SetDominant(bool isDominant)
    {
        IsDominant = isDominant;

        GeneSlot.EnableToggle(IsDominant);
    }

    public void DisableDominant()
    {
        GeneSlot.DisableToggle();
    }
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

    public Action<AlienGeneValue> OnDominantClicked;

    public void Init(List<GeneType> pickedGenes, GeneSlot geneSlotPrefab, Action<AlienGeneValue> callback)
    {
        //instantiate genes and add to values
        for (int i = 0; i < pickedGenes.Count; i++)
        {
            AlienGeneValue currentGeneValue = GetGeneFromType(pickedGenes[i]);

            GeneSlot geneSlot = Object.Instantiate(geneSlotPrefab, m_GeneContainer);
            geneSlot.Init(currentGeneValue.GeneData.Type, geneSlotPrefab.name, OnDominantToggleClicked);

            currentGeneValue.GeneSlot = geneSlot;
            SetGeneImage(currentGeneValue);
        }

        OnDominantClicked += callback;

        //disable background image, only used for scene preview
        GetComponent<Image>().enabled = false;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < m_GeneValues.Count; i++)
        {
            m_GeneValues[i].GeneSlot.RemoveCallback(OnDominantToggleClicked);
        }
    }

    public void RemoveCallback(Action<AlienGeneValue> callback)
    {
        OnDominantClicked -= callback;
    }

    public void DisableDominantGenes()
    {
        for (int i = 0; i < m_GeneValues.Count; i++)
        {
            m_GeneValues[i].DisableDominant();
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
        //leave empty if negative
        if (geneValue.Value < 0 || geneValue.Value >= geneValue.GeneData.Values.Count)
        {
            return;
        }

        geneValue.GeneSlot.Image.sprite = geneValue.GeneData.Values[geneValue.Value].Image;
    }

    private void OnDominantToggleClicked(GeneType geneType)
    {
        AlienGeneValue geneValue = GetGeneFromType(geneType);
        if (OnDominantClicked != null)
        {
            OnDominantClicked(geneValue);
        }
    }
}