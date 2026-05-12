using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy snake continuously chases the object tagged Player.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class SnakeChaseNavAgent : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";

    NavMeshAgent _agent;
    Transform _player;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        var go = GameObject.FindGameObjectWithTag(playerTag);
        if (go != null)
            _player = go.transform;
    }

    void Update()
    {
        if (DefeatScreenController.IsDefeat)
            return;
        if (_player == null)
        {
            var go = GameObject.FindGameObjectWithTag(playerTag);
            if (go != null)
                _player = go.transform;
            return;
        }

        if (!_agent.isOnNavMesh)
            return;
        _agent.SetDestination(_player.position);
    }
}
