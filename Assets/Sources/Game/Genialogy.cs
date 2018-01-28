using UnityEngine;
using UnityEngine.UI;

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

[System.Serializable]
public class AlienGeneData
{
    public GeneType GeneType;

    public bool Ignore;

    public int Value;
}

public class Genialogy : MonoBehaviour
{
    [SerializeField]
    private int m_GenesPerCharacter;

    [SerializeField]
    private int m_Generations;

    [SerializeField]
    private int m_DominantGenesAvailable;

    [SerializeField]
    private List<AlienGeneData> m_TargetGeneValues;

    [SerializeField]
    private Transform m_AlienContainer;

    [SerializeField]
    private GeneSlot m_GeneSlotPrefab;

    [SerializeField]
    private Text m_DominantGenesAvailableText;

    private const int MaxStars = 3;

    private const string GeneResourcePath = "Genes/";

    private int m_Tries;

    private AlienGeneData m_AlienGeneData = new AlienGeneData();

    private ColorScriptableObject m_ColorGene;

    private List<GeneScriptableObject> m_Genes = new List<GeneScriptableObject>(); //list of all game genes

    private List<GeneResolution> m_GeneResolutions = new List<GeneResolution>();

    private List<GeneType> m_AllGeneTypes            = new List<GeneType>();
    private List<GeneType> m_PickedGenes             = new List<GeneType>();
    private List<int>      m_AvailablePriorityValues = new List<int>();

    private List<Alien> m_Aliens = new List<Alien>();

    private void Awake()
    {
        //load all genes from resources
        m_ColorGene = Resources.Load<ColorScriptableObject>(GeneResourcePath + "Color");

        m_Genes.AddRange(Resources.LoadAll<GeneScriptableObject>(GeneResourcePath));

        FetchGeneTypes();

        PickGenes(m_GenesPerCharacter);

        FetchAndInitAliens();

        SetDominantGenesAvailableText(m_DominantGenesAvailable);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < m_Aliens.Count; i++)
        {
            m_Aliens[i].RemoveCallback(OnDominantClicked, OnDominantClicked);
        }
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

        //check if last alien has the same gene values as our objective
        if (IsObjectiveReached(m_Aliens[m_Aliens.Count - 1]))
        {
            Debug.Log("WAIT FOR IIIIT!!!");
            Invoke("ObjectiveComplete", 1f);
            return;
        }

        m_Tries++;
    }

    private void PickGenes(int genesPerCharacter)
    {
        //randomly pick genes from list and determine a resolution for those types
        m_PickedGenes.Clear();
        m_GeneResolutions.Clear();

        while (m_PickedGenes.Count < genesPerCharacter)
        {
            GeneType randomGeneType = m_AllGeneTypes[Random.Range(0, m_AllGeneTypes.Count)];
            m_AllGeneTypes.Remove(randomGeneType);

            if (!m_PickedGenes.Contains(randomGeneType))
            {
                GenerateGeneResolution(randomGeneType);
                m_PickedGenes.Add(randomGeneType);
            }
        }

        //setup picked genes in a prefedined order
        m_PickedGenes.Clear();
        m_PickedGenes.AddRange(new GeneType[] {GeneType.HAT, GeneType.HEAD, GeneType.BODY, GeneType.LEG, GeneType.COLOR});
    }

    private void GenerateGeneResolution(GeneType geneType)
    {
        //skip color type
        if (geneType == GeneType.COLOR)
        {
            return;
        }

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

    private void FetchGeneTypes()
    {
        m_AllGeneTypes.Clear();

        for (int i = 0; i < m_Genes.Count; i++)
        {
            m_AllGeneTypes.Add(m_Genes[i].Type);
        }

        m_AllGeneTypes.Add(m_ColorGene.Type);
    }

    private void FetchAndInitAliens()
    {
        m_Aliens.Clear();
        foreach(Transform child in m_AlienContainer)
        {
            Alien currentAlien = child.GetComponent<Alien>();
            currentAlien.Init(m_PickedGenes, m_GeneSlotPrefab, OnDominantClicked, OnDominantClicked);

            m_Aliens.Add(currentAlien);
        }

        //disable dominant genes for last alien
        m_Aliens[m_Aliens.Count - 1].DisableDominantGenes();
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

        AlienColorGeneValue firstParentColorGeneValue  = firstParent.ColorGeneValue,
                            secondParentColorGeneValue = secondParent.ColorGeneValue;

        m_AlienGeneData.GeneType = geneType;

        bool isColorGene = geneType == GeneType.COLOR;

        //if either parent has a predominant gene then use that gene value for the child
        if ((isColorGene && firstParentColorGeneValue.IsDominant) || (!isColorGene && firstParentGeneValue.IsDominant))
        {
            m_AlienGeneData.Value = isColorGene ? firstParentColorGeneValue.Value : firstParentGeneValue.Value;
            firstParent.Child.SetGeneData(m_AlienGeneData);

            return;
        }

        if ((isColorGene && secondParentColorGeneValue.IsDominant) || (!isColorGene && secondParentGeneValue.IsDominant))
        {
            m_AlienGeneData.Value = isColorGene ? secondParentColorGeneValue.Value : secondParentGeneValue.Value;
            firstParent.Child.SetGeneData(m_AlienGeneData); //both parents' child should be the same

            return;
        }

        //otherwise resolve based on priority
        if (isColorGene)
        {
            m_AlienGeneData.Value = ResolveColorRules(firstParentColorGeneValue, secondParentColorGeneValue);
        }
        else
        {
            GeneResolution geneResolution = GetGeneResolutionFromType(geneType);
            
            int firstParentPriorityGeneValue  = geneResolution.PriorityOrder.IndexOf(firstParentGeneValue.Value),
            secondParentPriorityGeneValue = geneResolution.PriorityOrder.IndexOf(secondParentGeneValue.Value);
            
            //lowest priority wins
            m_AlienGeneData.Value = firstParentPriorityGeneValue < secondParentPriorityGeneValue ? firstParentGeneValue.Value : secondParentGeneValue.Value;
        }
            
        firstParent.Child.SetGeneData(m_AlienGeneData);
    }

    private void ObjectiveComplete()
    {
        int stars = (int) Mathf.Max(0f, MaxStars - m_Tries);
        Debug.Log("Objective completed with " + stars + " stars");
    }

    private bool IsObjectiveReached(Alien finalAlien)
    {
        for (int i = 0; i < m_PickedGenes.Count; i++)
        {
            GeneType currentGeneType = m_PickedGenes[i];

            AlienGeneData alienGeneData = GetAlienGeneFromType(currentGeneType);
            if (alienGeneData.Ignore)
            {
                continue;
            }

            bool isColor = currentGeneType == GeneType.COLOR;

            int targetAlienValue = alienGeneData.Value,
                finalAlienValue  = isColor ? finalAlien.ColorGeneValue.Value : finalAlien.GetGeneFromType(currentGeneType).Value;

            if (targetAlienValue != finalAlienValue)
            {
                return false;
            }
        }

        return true;
    }

    private int ResolveColorRules(AlienColorGeneValue firstParentColorGene, AlienColorGeneValue secondParentColorGene)
    {
        List<ColorRule> colorRules = firstParentColorGene.ColorGeneData.ColorRules;
        for (int i = 0; i < colorRules.Count; i++)
        {
            ColorRule currentColorRule = colorRules[i];
            if (currentColorRule.FirstColor == firstParentColorGene.Value && currentColorRule.SecondColor == secondParentColorGene.Value)
            {
                return currentColorRule.ResultColor;
            }
        }
        
        return firstParentColorGene.ColorGeneData.Values.IndexOf(firstParentColorGene.ColorGeneData.DefaultColor);
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

    public AlienGeneData GetAlienGeneFromType(GeneType geneType)
    {
        for (int i = 0; i < m_TargetGeneValues.Count; i++)
        {
            AlienGeneData currentGeneValue = m_TargetGeneValues[i];
            if (currentGeneValue.GeneType == geneType)
            {
                return currentGeneValue;
            }
        }

        return null;
    }

    private void SetDominantGenesAvailableText(int dominantGenesAvailable)
    {
        m_DominantGenesAvailableText.text = dominantGenesAvailable.ToString();
    }

    public void OnDominantClicked(AlienGeneValue geneValue)
    {
        //toggle dominant
        bool isDominant = !geneValue.IsDominant;

        //don't activate if none available
        if (isDominant && m_DominantGenesAvailable == 0)
        {
            return;
        }

        geneValue.SetDominant(isDominant);
        m_DominantGenesAvailable = isDominant ? m_DominantGenesAvailable - 1 : m_DominantGenesAvailable + 1;

        SetDominantGenesAvailableText(m_DominantGenesAvailable);
    }

    public void OnDominantClicked(AlienColorGeneValue geneValue)
    {
        bool isDominant = !geneValue.IsDominant;

        //don't activate if none available
        if (isDominant && m_DominantGenesAvailable == 0)
        {
            return;
        }

        geneValue.SetDominant(isDominant);
        m_DominantGenesAvailable = isDominant ? m_DominantGenesAvailable - 1 : m_DominantGenesAvailable + 1;

        SetDominantGenesAvailableText(m_DominantGenesAvailable);
    }
}