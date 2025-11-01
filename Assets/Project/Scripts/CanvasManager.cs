using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Менеджер для работы с Canvas: управление цветом кисти, сохранением, загрузкой и очисткой.
/// </summary>
public class CanvasManager : MonoBehaviour
{
    /// <summary>
    /// Цвет кисти, используемый при включенном переключателе.
    /// </summary>
    [SerializeField] private Color _red = Color.red;

    /// <summary>
    /// Цвет кисти, используемый при выключенном переключателе.
    /// </summary>
    [SerializeField] private Color _blue = Color.blue;

    /// <summary>
    /// UI-переключатель для смены цвета кисти.
    /// </summary>
    [SerializeField] private Toggle _changeColorToggle;

    /// <summary>
    /// Кнопка для сохранения текущей текстуры.
    /// </summary>
    [SerializeField] private Button _saveButton;

    /// <summary>
    /// Кнопка для загрузки сохраненной текстуры.
    /// </summary>
    [SerializeField] private Button _loadButton;

    /// <summary>
    /// Кнопка для очистки холста.
    /// </summary>
    [SerializeField] private Button _clearButton;

    /// <summary>
    /// Менеджер рисования для взаимодействия с кистью и текстурой.
    /// </summary>
    [SerializeField] private DrawManager _drawManager;

    /// <summary>
    /// Инициализация подписок на события кнопок и переключателя.
    /// Устанавливает начальный цвет кисти.
    /// </summary>
    private void Start()
    {
        _changeColorToggle.onValueChanged.AddListener(SwitchColor);
        _saveButton.onClick.AddListener(Save);
        _loadButton.onClick.AddListener(Load);
        _clearButton.onClick.AddListener(Clear);
    }

    /// <summary>
    /// Сохраняет текущую текстуру с холста через JsonDraw.
    /// </summary>
    private void Save()
    {
        SaveTexture.Save(_drawManager.Draw.GetTexture());
    }

    private void Clear()
    {
        _drawManager.Draw.Clear();
    }
    /// <summary>
    /// Загружает текстуру с JsonDraw и устанавливает на холст.
    /// </summary>
    private void Load()
    {
        _drawManager.Draw.SetTexture(SaveTexture.Load());
    }

    /// <summary>
    /// Меняет цвет кисти в зависимости от состояния переключателя.
    /// </summary>
    /// <param name="on">True, если переключатель включен, иначе False.</param>
    private void SwitchColor(bool on)
    {
        _drawManager.Draw.BrushColor = on ? _red : _blue;
    }
}
