using System;
using UnityEngine;

/// <summary>
/// Абстрактный базовый класс для рисования на текстуре с возможностью задания кисти, цвета и фона.
/// </summary>
public abstract class Draw
{
    private const int MIN_BRUSH_SIZE = 1;
    private const int MAX_BRUSH_SIZE = 128;

    /// <summary>
    /// Размер кисти. Автоматически ограничивается минимальным и максимальным значением.
    /// </summary>
    public int BrushSize
    {
        get => _brushSize;
        private set => _brushSize = Mathf.Clamp(value, MIN_BRUSH_SIZE, MAX_BRUSH_SIZE);
    }

    /// <summary>
    /// Цвет фона текстуры. При установке автоматически обновляет текстуру.
    /// </summary>
    public Color Background
    {
        get => _background;
        set
        {
            _background = value;
            SetBackground();
        }
    }

    /// <summary>
    /// Цвет кисти для рисования.
    /// </summary>
    public Color BrushColor { get; set; } = Color.black;

    /// <summary>
    /// Камера, используемая для расчета координат UV при рисовании.
    /// </summary>
    protected Camera _raycastCamera = Camera.main;

    private Texture2D _tex;
    private bool _drawing;
    private Vector2 _prevUV;
    private Color32[] _bgCache;

    private int _textureWidth;
    private int _textureHeight;

    private Color _background = Color.white;
    private int _brushSize = 16;

    /// <summary>
    /// Создает экземпляр класса Draw с заданными размерами текстуры.
    /// </summary>
    /// <param name="textureWidth">Ширина текстуры.</param>
    /// <param name="textureHeight">Высота текстуры.</param>
    protected Draw(int textureWidth, int textureHeight)
    {
        _textureWidth = textureWidth;
        _textureHeight = textureHeight;
    }

    /// <summary>
    /// Очищает текстуру, заполняя её фоновым цветом.
    /// </summary>
    [ContextMenu("Clear")]
    public void Clear()
    {
        _tex.SetPixels32(_bgCache);
        _tex.Apply();
    }

    /// <summary>
    /// Получает текущую текстуру для отображения или сохранения.
    /// </summary>
    /// <returns>Текущая текстура Texture2D.</returns>
    public Texture2D GetTexture() => _tex;

    /// <summary>
    /// Устанавливает текстуру из объекта TextureData.
    /// </summary>
    /// <param name="data">Данные текстуры для установки.</param>
    public void SetTexture(TextureData data)
    {
        if (data == null || data.Width != _textureWidth || data.Height != _textureHeight)
        {
            return;
        }

        Color32[] pixels = data.GetPixels();
        _tex.SetPixels32(pixels);
        _tex.Apply();
    }

    /// <summary>
    /// Создает или обновляет текстуру с фоновым цветом.
    /// </summary>
    /// <returns>Обновленная текстура Texture2D.</returns>
    protected Texture2D SetBackground()
    {
        if (_tex == null)
        {
            _tex = new Texture2D(_textureWidth, _textureHeight, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
        }

        _bgCache = new Color32[_textureWidth * _textureHeight];
        for (int i = 0; i < _bgCache.Length; i++) _bgCache[i] = Background;

        _tex.SetPixels32(_bgCache);
        _tex.Apply();
        return _tex;
    }

    /// <summary>
    /// Абстрактный метод для получения UV-координат из экранной позиции.
    /// </summary>
    /// <param name="screenPos">Позиция на экране.</param>
    /// <param name="uv">UV-координаты на текстуре.</param>
    /// <returns>True, если UV удалось получить; иначе false.</returns>
    protected abstract bool TryGetUV(Vector2 screenPos, out Vector2 uv);

    /// <summary>
    /// Пытается нарисовать в текущей позиции (вызывает ContinueDraw с Vector2.zero).
    /// </summary>
    public void TryDraw()
    {
        ContinueDraw(Vector2.zero);
    }

    /// <summary>
    /// Начинает рисование с указанной позиции на экране.
    /// </summary>
    /// <param name="screenPos">Позиция на экране для начала рисования.</param>
    public void StartDraw(Vector2 screenPos)
    {
        if (!TryGetUV(screenPos, out Vector2 uv)) return;

        _drawing = true;
        _prevUV = uv;
        DrawDotUV(uv);
        _tex.Apply();
    }

    /// <summary>
    /// Продолжает рисование до указанной позиции на экране.
    /// </summary>
    /// <param name="screenPos">Новая позиция на экране.</param>
    public void ContinueDraw(Vector2 screenPos)
    {
        if (!_drawing)
        {
            StartDraw(screenPos);
            return;
        }

        if (!TryGetUV(screenPos, out Vector2 uv)) return;

        DrawLineUV(_prevUV, uv);
        _prevUV = uv;
        _tex.Apply();
    }

    /// <summary>
    /// Завершает рисование.
    /// </summary>
    public void EndDraw() => _drawing = false;

    // Вспомогательные методы рисования
    private void DrawDotUV(Vector2 uv) => DrawDotPx(UVtoPx(uv));
    private void DrawDotPx(Vector2Int px) => DrawCircle(px, BrushSize, BrushColor);
    private void DrawLineUV(Vector2 a, Vector2 b) => DrawLinePx(UVtoPx(a), UVtoPx(b));

    private void DrawLinePx(Vector2Int p0, Vector2Int p1)
    {
        int steps = Mathf.CeilToInt(Vector2.Distance(p0, p1));
        for (int i = 0; i <= steps; i++)
        {
            float t = steps == 0 ? 0 : (float)i / steps;
            Vector2Int p = Vector2Int.RoundToInt(Vector2.Lerp(p0, p1, t));
            DrawCircle(p, BrushSize, BrushColor);
        }
    }

    private Vector2Int UVtoPx(Vector2 uv) => new(
        Mathf.Clamp(Mathf.RoundToInt(uv.x * (_tex.width - 1)), 0, _tex.width - 1),
        Mathf.Clamp(Mathf.RoundToInt(uv.y * (_tex.height - 1)), 0, _tex.height - 1)
    );

    /// <summary>
    /// Рисует заполненный круг заданного цвета на текстуре.
    /// </summary>
    /// <param name="center">Центр круга в пикселях.</param>
    /// <param name="radius">Радиус круга в пикселях.</param>
    /// <param name="color">Цвет круга.</param>
    private void DrawCircle(Vector2Int center, int radius, Color color)
    {
        int r2 = radius * radius;
        int x0 = Mathf.Max(center.x - radius, 0);
        int x1 = Mathf.Min(center.x + radius, _tex.width - 1);
        int y0 = Mathf.Max(center.y - radius, 0);
        int y1 = Mathf.Min(center.y + radius, _tex.height - 1);

        for (int y = y0; y <= y1; y++)
        {
            int dy = y - center.y;
            for (int x = x0; x <= x1; x++)
            {
                int dx = x - center.x;
                if (dx * dx + dy * dy <= r2)
                    _tex.SetPixel(x, y, color);
            }
        }
    }
}
