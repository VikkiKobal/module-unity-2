using UnityEngine;

/// <summary>
/// Attach to a trigger zone at the finish line.
/// </summary>
public class FinishTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
            GameManager.Instance?.OnFinish();
    }
}
