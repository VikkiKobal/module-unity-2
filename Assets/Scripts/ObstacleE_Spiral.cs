using UnityEngine;

public class ObstacleE_Spiral : MonoBehaviour
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private float angularSpeed = 1f;
    [SerializeField] private float verticalSpeed = 0.5f;
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private int axis = 0;

    private float _angle;
    private Vector3 _worldCenter;

    private void Start()
    {
        _angle = 0f;
        _worldCenter = center.sqrMagnitude < 0.0001f ? transform.position : center;
        transform.position = GetSpiralPosition(0f);
    }

    private void Update()
    {
        float prevAngle = _angle;
        _angle += angularSpeed * Time.deltaTime;

        Vector3 prevPos = GetSpiralPosition(prevAngle);
        Vector3 newPos = GetSpiralPosition(_angle);
        Vector3 delta = newPos - prevPos;

        transform.Translate(delta, Space.World);
    }

    private Vector3 GetSpiralPosition(float angle)
    {
        float x = radius * Mathf.Cos(angle);
        float z = radius * Mathf.Sin(angle);
        float height = verticalSpeed * angle;
        Vector3 c = Application.isPlaying ? _worldCenter : (center.sqrMagnitude < 0.0001f ? transform.position : center);

        return axis switch
        {
            1 => c + new Vector3(height, x, z),
            2 => c + new Vector3(x, z, height),
            _ => c + new Vector3(x, height, z)
        };
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        const int segments = 48;
        float maxAngle = Mathf.PI * 4f;
        for (int i = 0; i < segments; i++)
        {
            float a0 = (i / (float)segments) * maxAngle;
            float a1 = ((i + 1) / (float)segments) * maxAngle;
            Gizmos.DrawLine(GetSpiralPosition(a0), GetSpiralPosition(a1));
        }
    }
}
