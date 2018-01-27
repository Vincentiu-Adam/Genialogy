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

public class AlienGeneData
{
    public GeneType GeneType;

    public int Value;
}

public class Genialogy : MonoBehaviour
{
    [SerializeField]
    private int m_GenesPerCharacter;

    [SerializeField]
    private int m_Generations;

    [SerializeField]
    private Transform m_AlienContainer;

    [SerializeField]
    private GeneSlot m_GeneSlotPrefab;

    private const string GeneResourcePath = "Genes";

    private int m_FirstGenerationAlienCount;

    private AlienGeneData m_AlienGeneData = new AlienGeneData();

    private List<GeneScriptableObject> m_Genes = new List<GeneScriptableObject>(); //list of all game genes

    private List<GeneResolution> m_GeneResolutions = new List<GeneResolution>();

    private List<GeneType> m_PickedGenes             = new List<GeneType>();
    private List<int>      m_AvailablePriorityValues = new List<int>();

    private List<Alien> m_Aliens = new List<Alien>();

    private void Awake()
    {
        //load all genes from resources
        m_Genes.AddRange(Resources.LoadAll<GeneScriptableObject>(GeneResourcePath));

        PickGenes(m_GenesPerCharacter);

        FetchAndInitAliens();
    }

    public void FuseAllAliens()
    {
        //while still generations remaining, fuse aliens 2 by 2
        int generationIndex = 0;

        int generationsRemaining = m_Generations;
        while (generationsRemaining > 1)
        {
            int aliensPerGeneration = (int) Mathf.Pow(2, generationsRemaining - 1);

            int aliensGenerationCount = generationIndex + aliensPerGeneration;
            for (int i = generationIndex; i < aliensGenerationCount; i+=2)
            {
                FuseAliens(m_Aliens[i], m_Aliens[i+1]);
            }

            generationIndex += aliensPerGeneration;
            generationsRemaining--;
        }
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

    private void FetchAndInitAliens()
    {
        m_Aliens.Clear();
        foreach(Transform child in m_AlienContainer)
        {
            Alien currentAlien = child.GetComponent<Alien>();
            currentAlien.Init(m_PickedGenes, m_GeneSlotPrefab);

            m_Aliens.Add(currentAlien);
        }
    }

    private void FuseAliens(Alien firstParent, Alien secondParent)
    {
        //fuse all picked gene types for both aliens
        for (int i = 0; i < m_PickedGenes.Count; i++)
        {
            FuseAlienGeneType(m_PickedGenes[i], firstParent, secondParent);
        }
    }

    private void FuseAlienGeneType(GeneType geneType, Alien firstParent, Alien secondParent)
    {
        AlienGeneValue firstParentGeneValue  = firstParent.GetGeneFromType(geneType),
                       secondParentGeneValue = secondParent.GetGeneFromType(geneType);

        m_AlienGeneData.GeneType = geneType;

        //if either parent has a predominant gene then use that gene value for the child
        if (firstParentGeneValue.IsDominant)
        {
            m_AlienGeneData.Value = firstParentGeneValue.Value;
            firstParent.Child.SetGeneData(m_AlienGeneData);

            return;
        }

        if (secondParentGeneValue.IsDominant)
        {
            m_AlienGeneData.Value = secondParentGeneValue.Value;
            firstParent.Child.SetGeneData(m_AlienGeneData); //both parents' child should be the same

            return;
        }

        //otherwise resolve based on priority
        GeneResolution geneResolution = GetGeneResolutionFromType(geneType);

        int firstParentPriorityGeneValue  = geneResolution.PriorityOrder.IndexOf(firstParentGeneValue.Value),
            secondParentPriorityGeneValue = geneResolution.PriorityOrder.IndexOf(secondParentGeneValue.Value);

        //lowest priority wins
        m_AlienGeneData.Value = firstParentPriorityGeneValue < secondParentPriorityGeneValue ? firstParentGeneValue.Value : secondParentGeneValue.Value;
        firstParent.Child.SetGeneData(m_AlienGeneData);
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

    private GeneResolution GetGeneResolutionFromType(GeneType geneType)
    {
        for (int i = 0; i < m_GeneResolutions.Count; i++)
        {
            GeneResolution currentGeneResolution = m_GeneResolutions[i];
            if (currentGeneResolution.GeneType == geneType)
            {
                return currentGeneResolution;
            }
        }

        return null;
    }
}