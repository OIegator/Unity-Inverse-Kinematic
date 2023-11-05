using UnityEngine;

public class LineBetweenObjects : MonoBehaviour
{
    public Transform startObject;
    public Transform endObject;

    private LineRenderer _lineRenderer;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        if (startObject != null && endObject != null)
        {
            _lineRenderer.SetPosition(0, startObject.position);
            _lineRenderer.SetPosition(1, endObject.position);
        }
    }
}