using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

/// <summary>
/// Point-and-click movement for the shrew using NavMeshAgent.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class ShrewNavMeshClickMove : MonoBehaviour
{
    [SerializeField] Camera targetCamera;
    [SerializeField] LayerMask raycastMask = Physics.DefaultRaycastLayers;
    [SerializeField] float sampleMaxDistance = 2f;
    [SerializeField] float raycastMaxDistance = 200f;

    NavMeshAgent _agent;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        if (DefeatScreenController.IsDefeat)
            return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
        if (!Input.GetMouseButtonDown(0))
            return;
        if (targetCamera == null)
            return;

        var ray = targetCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray.origin, ray.direction, out var hit, raycastMaxDistance, raycastMask))
            return;

        if (NavMesh.SamplePosition(hit.point, out var navHit, sampleMaxDistance, NavMesh.AllAreas))
            _agent.SetDestination(navHit.position);
    }
}
