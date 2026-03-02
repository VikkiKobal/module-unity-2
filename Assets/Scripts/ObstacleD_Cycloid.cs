using UnityEngine;

public class ObstacleD_Cycloid : MonoBehaviour
{
    [SerializeField] private float radius = 2f;
    [SerializeField] private float angularSpeed = 1f;
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private int plane = 0;

    private float _t;
    private Vector3 _worldCenter;

    private void Start()
    {
        _t = 0f;
        _worldCenter = center.sqrMagnitude < 0.0001f ? transform.position : center;
        transform.position = GetCycloidPosition(0f);
    }

    private void Update()
    {
        float prevT = _t;
        _t += angularSpeed * Time.deltaTime;

        Vector3 prevPos = GetCycloidPosition(prevT);
        Vector3 newPos = GetCycloidPosition(_t);
        Vector3 delta = newPos - prevPos;

        transform.Translate(delta, Space.World);
    }

    private Vector3 GetCycloidPosition(float t)
    {
        float x = radius * (t - Mathf.Sin(t));
        float y = radius * (1f - Mathf.Cos(t));
        Vector3 c = Application.isPlaying ? _worldCenter : (center.sqrMagnitude < 0.0001f ? transform.position : center);

        return plane switch
        {
            1 => c + new Vector3(x, y, 0f),
            2 => c + new Vector3(0f, y, x),
            _ => c + new Vector3(x, 0f, y)
        };
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        const int segments = 64;
        for (int i = 0; i < segments; i++)
        {
            float t0 = (i / (float)segments) * Mathf.PI * 2f;
            float t1 = ((i + 1) / (float)segments) * Mathf.PI * 2f;
            Gizmos.DrawLine(GetCycloidPosition(t0), GetCycloidPosition(t1));
        }
    }
}
