using UnityEngine;

using System.Collections.Generic;

[System.Serializable]
public class GeneResolution
{
    public GeneType GeneType;
   
    public List<int> PriorityOrder = new List<int>(); //priority order for this gene type (lower index = higher priority)

    public GeneResolution(GeneType geneType)
    {
        GeneType = geneType;
    }
}

public class Genialogy : MonoBehaviour
{
    [SerializeField]
    private int m_GenesPerCharacter;

    private const string GeneResourcePath = "Genes";

    private List<GeneScriptableObject> m_Genes = new List<GeneScriptableObject>(); //list of all game genes

    private List<GeneResolution> m_GeneResolutions = new List<GeneResolution>();

    private List<GeneType> m_PickedGenes             = new List<GeneType>();
    private List<int>      m_AvailablePriorityValues = new List<int>();

    private void Awake()
    {
        //load all genes from resources
        m_Genes.AddRange(Resources.LoadAll<GeneScriptableObject>(GeneResourcePath));

        PickGenes(m_GenesPerCharacter);
    }

    private void PickGenes(int genesPerCharacter)
    {
        //randomly pick genes from list and determine a resolution for those types
        m_PickedGenes.Clear();
        m_GeneResolutions.Clear();

        while(m_PickedGenes.Count < genesPerCharacter)
        {
            GeneType randomGeneType = m_Genes[Random.Range(0, m_Genes.Count)].Type;

            if (!m_PickedGenes.Contains(randomGeneType))
            {
                GenerateGeneResolution(randomGeneType);
                m_PickedGenes.Add(randomGeneType);
            }
        }
    }

    private void GenerateGeneResolution(GeneType geneType)
    {
        GeneResolution geneResolution = new GeneResolution(geneType);

        m_AvailablePriorityValues.Clear();
        int maxGeneValues = GetGeneValuesCount(geneType);

        for (int i = 0; i < maxGeneValues; i++)
        {
            m_AvailablePriorityValues.Add(i);
        }

        while (m_AvailablePriorityValues.Count > 0)
        {
            int randomValue = m_AvailablePriorityValues[Random.Range(0, m_AvailablePriorityValues.Count)];

            geneResolution.PriorityOrder.Add(randomValue);
            m_AvailablePriorityValues.Remove(randomValue);
        }

        m_GeneResolutions.Add(geneResolution);
    }

    private int GetGeneValuesCount(GeneType geneType)
    {
        GeneScriptableObject geneScriptableObject = GetGeneFromType(geneType);
        return geneScriptableObject.Values.Count;
    }

    private GeneScriptableObject GetGeneFromType(GeneType geneType)
    {
        for (int i = 0; i < m_Genes.Count; i++)
        {
            GeneScriptableObject currentGeneScriptableObject = m_Genes[i];
            if (currentGeneScriptableObject.Type == geneType)
            {
                return currentGeneScriptableObject;
            }
        }

        return null;
    }
}