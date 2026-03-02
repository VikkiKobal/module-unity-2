using UnityEngine;

public class ObstacleA_SegmentAB : MonoBehaviour
{
    [SerializeField] private Vector3 pointA = new Vector3(-3f, 0f, 0f);
    [SerializeField] private Vector3 pointB = new Vector3(3f, 0f, 0f);
    [SerializeField] private float speed = 2f;

    private Vector3 _basePosition;
    private Vector3 _worldPointA;
    private Vector3 _worldPointB;
    private Vector3 _currentTarget;
    private bool _movingToB = true;

    private void Start()
    {
        _basePosition = transform.position;
        _worldPointA = _basePosition + pointA;
        _worldPointB = _basePosition + pointB;
        transform.position = _worldPointA;
        _currentTarget = _worldPointB;
        _movingToB = true;
    }

    private void Update()
    {
        Vector3 currentPos = transform.position;
        Vector3 direction = (_currentTarget - currentPos).normalized;
        float distanceToTarget = Vector3.Distance(currentPos, _currentTarget);
        float step = speed * Time.deltaTime;

        if (distanceToTarget <= step)
        {
            Vector3 delta = _currentTarget - currentPos;
            transform.Translate(delta, Space.World);

            if (_movingToB)
            {
                _currentTarget = _worldPointA;
                _movingToB = false;
            }
            else
            {
                _currentTarget = _worldPointB;
                _movingToB = true;
            }
        }
        else
        {
            Vector3 delta = direction * step;
            transform.Translate(delta, Space.World);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 a = Application.isPlaying ? _worldPointA : transform.position + pointA;
        Vector3 b = Application.isPlaying ? _worldPointB : transform.position + pointB;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawSphere(a, 0.2f);
        Gizmos.DrawSphere(b, 0.2f);
    }
}
