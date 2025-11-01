using UnityEngine;

public class DrawOnMesh : Draw
{
    protected GameObject _drawPlace;

    public DrawOnMesh(Renderer renderer, int textureWidth = 1024, int textureHeight = 1024) : base(textureWidth, textureHeight)
    {
        _drawPlace = renderer.gameObject;
        renderer.material.mainTexture = SetBackground();
    }
    protected override bool TryGetUV(Vector2 screenPos, out Vector2 uv)
    {
        Ray ray = _raycastCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == _drawPlace)
            {
                uv = hit.textureCoord;
                return true;
            }
        }
        uv = Vector2.zero;
        return false;
    }
}
