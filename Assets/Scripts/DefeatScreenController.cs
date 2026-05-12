using UnityEngine;
using UnityEngine.UI;

public class DefeatScreenController : MonoBehaviour
{
    public static bool IsDefeat { get; private set; }

    [SerializeField] GameObject defeatPanel;
    [SerializeField] Text defeatText;

    void Awake()
    {
        IsDefeat = false;
        if (defeatPanel != null)
            defeatPanel.SetActive(false);
    }

    public void ShowDefeat()
    {
        if (IsDefeat)
            return;
        IsDefeat = true;
        if (defeatPanel != null)
            defeatPanel.SetActive(true);
        if (defeatText != null)
            defeatText.text = "Game Over / Defeat";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void BindPanel(GameObject panel, Text text)
    {
        defeatPanel = panel;
        defeatText = text;
        if (defeatPanel != null)
            defeatPanel.SetActive(false);
    }
}
