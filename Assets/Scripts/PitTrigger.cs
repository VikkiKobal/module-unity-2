using UnityEngine;

public class PitTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null) pc.Respawn();
    }
}
