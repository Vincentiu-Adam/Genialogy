using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameMenuUI : MonoBehaviour
{
    [SerializeField]
    private AudioMixer m_Mixer;

    [SerializeField]
    private Toggle m_Toggle;

    private VictoryUI m_VictoryUI;

    private static bool IsSoundOn = true;

    private void Awake()
    {
        m_VictoryUI = GameObject.FindGameObjectWithTag("VictoryUI").GetComponent<VictoryUI>();

        m_Toggle.isOn = IsSoundOn;
    }

    public void ToggleSound(bool isOn)
    {
        IsSoundOn = isOn;

        float volume = isOn ? 0f : -80f;
        m_Mixer.SetFloat("Volume", volume);
    }

    public void ReplayLevel()
    {
        m_VictoryUI.ReplayLevel();
    }
}
