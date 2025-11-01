using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventSystem;
using static UnityEngine.EventSystems.ExecuteEvents;

/// <summary>
/// Скрипт для взаимодействия с UI через кончик пальца в 3D пространстве.
/// </summary>
public class FingerUIInteract : MonoBehaviour
{
    /// <summary>
    /// Максимальное расстояние до UI, при котором взаимодействие считается допустимым.
    /// </summary>
    [SerializeField] private float _maxDistance = 0.02f; 

    /// <summary>
    /// Камера, через которую вычисляется позиция пальца на экране.
    /// </summary>
    private Camera _camera;

    /// <summary>
    /// Трансформ кончика пальца.
    /// </summary>
    private Transform _finger;

    /// <summary>
    /// Последний UI-объект, с которым взаимодействовал палец.
    /// </summary>
    private GameObject _lastTarget;

    /// <summary>
    /// Инициализация камеры и трансформа пальца.
    /// </summary>
    private void Start()
    {
        _camera = Camera.main;
        _finger = transform;
    }

    /// <summary>
    /// Проверка и обработка нажатий по UI в каждом кадре.
    /// </summary>
    private void Update()
    {
        TryClickByFinger();
    }

    /// <summary>
    /// Пытается определить UI-объект под пальцем и выполнить нажатие.
    /// </summary>
    private void TryClickByFinger()
    {
        if (!_finger || !_camera) return;

        // Преобразуем позицию пальца в экранные координаты
        Vector3 screenPos = _camera.WorldToScreenPoint(_finger.position);

        // Создаем PointerEventData для Raycast по UI
        PointerEventData ped = new(current)
        {
            position = screenPos
        };

        // Список результатов Raycast
        List<RaycastResult> results = new List<RaycastResult>();
        current.RaycastAll(ped, results);

        // Обрабатываем нажатие
        Click(results, ped);
    }

    /// <summary>
    /// Выполняет нажатие или отпускание UI-элемента в зависимости от результатов Raycast.
    /// </summary>
    /// <param name="results">Список результатов Raycast по UI.</param>
    /// <param name="ped">Данные события указателя для передачи в ExecuteEvents.</param>
    private void Click(List<RaycastResult> results, PointerEventData ped)
    {
        if (results.Count > 0)
        {
            RaycastResult hit = results[0]; // Берем ближайший UI-элемент
            if (Vector3.Distance(_finger.position, hit.worldPosition) < _maxDistance)
            {
                if (_lastTarget != hit.gameObject)
                {
                    _lastTarget = hit.gameObject;

                    // "Нажатие" на UI-объект
                    Execute(hit.gameObject, ped, pointerEnterHandler);
                    Execute(hit.gameObject, ped, pointerDownHandler);
                    Execute(hit.gameObject, ped, pointerClickHandler);
                }
            }
        }
        else
        {
            if (_lastTarget)
            {
                // Если под пальцем нет UI, отпускаем последний объект
                Execute(_lastTarget, new PointerEventData(current), pointerUpHandler);
                _lastTarget = null;
            }
        }
    }
}
