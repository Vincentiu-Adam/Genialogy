using UnityEngine;

public enum GeneType
{
    HEAD,
    BODY,
    ARMS,
    COLOR
}

[System.Serializable]
public class BaseGene 
{
    [SerializeField]
    private GeneType m_Type;
}
