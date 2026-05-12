using UnityEngine;


public class SnakePlayerDefeatTrigger : MonoBehaviour
{
    [SerializeField] DefeatScreenController defeatScreen;

    public void Bind(DefeatScreenController controller)
    {
        defeatScreen = controller;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        defeatScreen?.ShowDefeat();
    }
}
