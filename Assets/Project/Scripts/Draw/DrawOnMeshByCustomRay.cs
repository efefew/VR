using UnityEngine;

public class DrawOnMeshByCustomRay : DrawOnMesh
{
    private Transform  _raySource;
    private float _rayDistance, _rayOffset;
    private LineRenderer _lineRenderer;
    public DrawOnMeshByCustomRay(Renderer renderer, Transform  raySource, float rayDistance, float rayOffset, int textureWidth = 1024, int textureHeight = 1024) : base(renderer, textureWidth, textureHeight)
    {
        _raySource = raySource;
        _rayDistance = rayDistance;
        _rayOffset = rayOffset;
        BrushColor = Color.red;
        //CreateRay();
    }

    private void CreateRay()
    {
        _lineRenderer = new GameObject().AddComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.widthMultiplier = 0.002f;
        _lineRenderer.endWidth = _lineRenderer.startWidth = 0.002f;
        _lineRenderer.startColor = _lineRenderer.endColor = Color.red;
    }
    protected override bool TryGetUV(Vector2 screenPos, out Vector2 uv)
    {
        Ray ray = new(_raySource.position - _raySource.forward * _rayOffset, _raySource.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, _rayDistance);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject == _drawPlace)
            {
                uv = hits[i].textureCoord;
                return true; 
            }
        }
        EndDraw();
        /*if (Physics.Raycast(ray, out RaycastHit hit, _rayDistance/*, layerMask:6#1#))
        {
            /*_lineRenderer.SetPosition(0, ray.origin);
            _lineRenderer.SetPosition(1, hit.point);#1#
            if (hit.collider.gameObject == _drawPlace)
            {
                uv = hit.textureCoord;
                return true; 
            }
        }
        else
        {
            /*_lineRenderer.SetPosition(0,ray.origin);
            _lineRenderer.SetPosition(1, ray.origin + ray.direction * _rayDistance);#1#
            EndDraw();
        }*/
        
        uv = Vector2.zero;
        return false;
    }
}
