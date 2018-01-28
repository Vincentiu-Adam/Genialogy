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
        if (GeneSlot == null)
        {
            return;
        }

        GeneSlot.DisableToggle();
    }
}

[System.Serializable]
public class AlienColorGeneValue
{
    public bool IsDominant;

    public int Value;

    public ColorScriptableObject ColorGeneData;

    public GeneSlot GeneSlot;

    public void SetDominant(bool isDominant)
    {
        IsDominant = isDominant;

        GeneSlot.EnableToggle(IsDominant);
    }

    public void DisableDominant()
    {
        if (GeneSlot == null)
        {
            return;
        }

        GeneSlot.DisableToggle();
    }
}

public class Alien : MonoBehaviour
{
    [SerializeField]
    private List<AlienGeneValue> m_GeneValues;

    [SerializeField]
    private AlienColorGeneValue m_ColorGeneValue;

    [SerializeField]
    private Transform m_GeneContainer;

    [SerializeField]
    private Alien m_Child;

    [SerializeField]
    private List<Alien> m_Parents;

    public AlienColorGeneValue ColorGeneValue { get { return m_ColorGeneValue; } }

    public Alien Child         { get { return m_Child; } }
    public List<Alien> Parents { get { return m_Parents; } }

    public Action<AlienGeneValue>      OnDominantClicked;
    public Action<AlienColorGeneValue> OnDominantColorGeneClicked;

    public void Init(List<GeneType> pickedGenes, GeneSlot geneSlotPrefab, Action<AlienGeneValue> callback, Action<AlienColorGeneValue> colorCallback)
    {
        //instantiate genes and add to values
        bool hasColorGene = false;
        for (int i = 0; i < pickedGenes.Count; i++)
        {
            GeneType geneType = pickedGenes[i];
            hasColorGene = hasColorGene || geneType == GeneType.COLOR;

            GeneSlot geneSlot = Object.Instantiate(geneSlotPrefab, m_GeneContainer);
            geneSlot.Init(geneType, geneSlotPrefab.name, OnDominantToggleClicked);

            InitGeneValue(geneType, geneSlot);
        }

        //if color gene, set the color of all other gene images to that of the color
        if (hasColorGene)
        {
            SetAllGenesToColor(m_ColorGeneValue);
        }

        OnDominantClicked          += callback;
        OnDominantColorGeneClicked += colorCallback;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < m_GeneValues.Count; i++)
        {
            AlienGeneValue currentGeneValue = m_GeneValues[i];
            if (currentGeneValue.GeneSlot != null)
            {
                currentGeneValue.GeneSlot.RemoveCallback(OnDominantToggleClicked);
            }
        }
    }

    public void RemoveCallback(Action<AlienGeneValue> callback, Action<AlienColorGeneValue> colorCallback)
    {
        OnDominantClicked          -= callback;
        OnDominantColorGeneClicked -= colorCallback;
    }

    public void DisableDominantGenes()
    {
        for (int i = 0; i < m_GeneValues.Count; i++)
        {
            m_GeneValues[i].DisableDominant();
        }

        m_ColorGeneValue.DisableDominant();
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
        if (geneData.GeneType == GeneType.COLOR)
        {
            m_ColorGeneValue.Value = geneData.Value;

            SetGeneColor();
            SetAllGenesToColor(m_ColorGeneValue);

            return;
        }

        AlienGeneValue geneValue = GetGeneFromType(geneData.GeneType);
        geneValue.Value = geneData.Value;

        SetGeneImage(geneValue);
    }

    private void InitGeneValue(GeneType geneType, GeneSlot geneSlot)
    {
        //special case for color gene
        if (geneType == GeneType.COLOR)
        {
            m_ColorGeneValue.GeneSlot = geneSlot;
            SetGeneColor();

            return;
        }

        AlienGeneValue currentGeneValue = GetGeneFromType(geneType);

        currentGeneValue.GeneSlot = geneSlot;
        SetGeneImage(currentGeneValue);
    }

    private Color GetGeneColor(AlienColorGeneValue colorGeneValue)
    {
        if (colorGeneValue.Value < 0)
        {
            return Color.white;
        }

        return colorGeneValue.Value >= colorGeneValue.ColorGeneData.Values.Count ? colorGeneValue.ColorGeneData.DefaultColor :
                                                                                   colorGeneValue.ColorGeneData.Values[colorGeneValue.Value];
    }

    private void SetGeneImage(AlienGeneValue geneValue)
    {
        //leave empty if negative
        if (geneValue.Value < 0 || geneValue.Value >= geneValue.GeneData.Values.Count)
        {
            return;
        }

        GeneValue geneDataValue = geneValue.GeneData.Values[geneValue.Value];

        geneValue.GeneSlot.Image.sprite        = geneDataValue.Image;
        geneValue.GeneSlot.ImageOutLine.sprite = geneDataValue.Outline;
    }

    private void SetGeneColor()
    {
        m_ColorGeneValue.GeneSlot.Image.color          = GetGeneColor(m_ColorGeneValue);
        m_ColorGeneValue.GeneSlot.ImageOutLine.enabled = false; //disable image for color
    }

    private void SetAllGenesToColor(AlienColorGeneValue colorGeneValue)
    {
        Color color = GetGeneColor(colorGeneValue);

        for (int i = 0; i < m_GeneValues.Count; i++)
        {
            AlienGeneValue currentGeneValue = m_GeneValues[i];
            if (currentGeneValue.GeneSlot != null)
            {
                currentGeneValue.GeneSlot.Image.color = color;
            }
        }
    }

    private void OnDominantToggleClicked(GeneType geneType)
    {
        if (geneType == GeneType.COLOR)
        {
            OnDominantColorGeneClicked(ColorGeneValue);
            return;
        }

        AlienGeneValue geneValue = GetGeneFromType(geneType);
        if (OnDominantClicked != null)
        {
            OnDominantClicked(geneValue);
        }
    }
}