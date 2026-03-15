using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        PlayerController pc = PlayerController.Instance;
        if (pc == null) return;
        if (pc.IsBoosting)
            Debug.Log("BOOST: " + pc.boostTimer.ToString("F1") + "s");
    }

    public void OnFinish()
    {
        Debug.Log("YOU WIN!");
        Time.timeScale = 0f;
    }

    public void OnRespawn()
    {
        Debug.Log("Player respawned.");
    }
}
