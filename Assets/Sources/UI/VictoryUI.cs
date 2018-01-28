using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

public class VictoryUI : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Panel;

    [SerializeField]
    private List<Toggle> m_Stars;

    private static int CurrentLevel = 1;

    private static SettingsScriptableObject SettingsScriptableObject;

    private void Awake()
    {
        if (SettingsScriptableObject == null)
        {
            SettingsScriptableObject = Resources.Load<SettingsScriptableObject>("Settings");
        }
    }

    public void Init(int stars)
    {
        for (int i = 0; i < stars; i++)
        {
            m_Stars[i].isOn = true;
        }

        m_Panel.SetActive(true);

        //stop zoom
        GameObject.Find("UI Canvas").GetComponent<Zoom>().enabled = false;
    }

    public void ReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        CurrentLevel = Mathf.Min(SettingsScriptableObject.MaxLevel, CurrentLevel + 1);
        SceneManager.LoadScene("Level " + CurrentLevel);
    }
}
