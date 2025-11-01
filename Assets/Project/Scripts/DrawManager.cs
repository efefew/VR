using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Менеджер рисования на объекте с помощью мыши в редакторе или пальца на устройстве.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class DrawManager : MonoBehaviour
{
    [SerializeField] private bool _vrMode;
    [SerializeField] private FingersTracker _fingersTracker;
    private Transform _finger;

    /// <summary>
    /// Максимальная глубина касания при рисовании пальцем.
    /// </summary>
    [SerializeField] private float _drawDistance = 0.04f + 0.1f;
    [SerializeField] private float _drawOffset = -0.1f;

    private GameObject _paintObject;

    /// <summary>
    /// Объект, на котором происходит рисование.
    /// </summary>
    private Renderer _renderer;

    /// <summary>
    /// Экземпляр класса Draw, используемый для работы с текстурой.
    /// </summary>
    public Draw Draw { get; private set; }
    
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_vrMode)
        {
            _fingersTracker.OnRightHandFingersFound += FindFinger;
        }
        else
        {
            Draw = new DrawOnMesh(_renderer, 512, 512);
        }
    }

    private void FindFinger(IList<OVRBone> bones)
    {
        _fingersTracker.OnRightHandFingersFound -= FindFinger;
        _finger = bones.First(bone => bone.Transform.name.ToLower().Contains("indextip")).Transform;
        Draw = new DrawOnMeshByCustomRay(_renderer, _finger, _drawDistance, _drawOffset, 512, 512);
    }

    /// <summary>
    /// Очистка текстуры при уничтожении объекта.
    /// </summary>
    private void OnDestroy()
    {
        DestroyImmediate(Draw.GetTexture());
    }

    /// <summary>
    /// Обновление состояния рисования каждый кадр.
    /// В редакторе — рисование мышью, на устройстве — пальцем.
    /// </summary>
    private void Update()
    {
        if(_vrMode) DrawByFinger();
        else DrawByMouse();
    }

    /// <summary>
    /// Выполняет рисование пальцем, если трансформ пальца задан.
    /// </summary>
    private void DrawByFinger()
    {
        if (!_finger) return;
        Draw.TryDraw();
    }

    /// <summary>
    /// Выполняет рисование с помощью мыши в редакторе.
    /// </summary>
    private void DrawByMouse()
    {
        if (Input.GetMouseButtonDown(0)) Draw.StartDraw(Input.mousePosition);
        if (Input.GetMouseButton(0)) Draw.ContinueDraw(Input.mousePosition);
        if (Input.GetMouseButtonUp(0)) Draw.EndDraw();
    }
}
