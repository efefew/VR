using UnityEngine;

/// <summary>
/// Класс для хранения данных текстуры в сериализуемом формате.
/// </summary>
[System.Serializable]
public class TextureData
{
    /// <summary>
    /// Ширина текстуры.
    /// </summary>
    public int Width;

    /// <summary>
    /// Высота текстуры.
    /// </summary>
    public int Height;

    /// <summary>
    /// Массив байтов пикселей (RGB, 3 байта на пиксель).
    /// </summary>
    public byte[] Pixels;

    /// <summary>
    /// Создает новый объект TextureData с заданными размерами.
    /// </summary>
    /// <param name="width">Ширина текстуры.</param>
    /// <param name="height">Высота текстуры.</param>
    public TextureData(int width, int height)
    {
        Width = width;
        Height = height;
        Pixels = new byte[width * height * 3];
    }

    /// <summary>
    /// Устанавливает массив пикселей из Color32[] в внутренний формат RGB.
    /// </summary>
    /// <param name="colors">Массив цветов пикселей.</param>
    public void SetPixels(Color32[] colors)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            int idx = i * 3;
            Pixels[idx + 0] = colors[i].r;
            Pixels[idx + 1] = colors[i].g;
            Pixels[idx + 2] = colors[i].b;
        }
    }

    /// <summary>
    /// Получает массив пикселей в формате Color32 из внутреннего массива байтов.
    /// </summary>
    /// <returns>Массив пикселей Color32.</returns>
    public Color32[] GetPixels()
    {
        var result = new Color32[Width * Height];
        for (int i = 0; i < result.Length; i++)
        {
            int idx = i * 3;
            result[i] = new Color32(
                Pixels[idx + 0],
                Pixels[idx + 1],
                Pixels[idx + 2],
                255
            );
        }
        return result;
    }
}

/// <summary>
/// Статический класс для сохранения и загрузки текстур в PlayerPrefs с использованием JSON.
/// </summary>
public static class SaveTexture
{
    /// <summary>
    /// Ключ для хранения данных в PlayerPrefs.
    /// </summary>
    private const string KEY = "draw";

    /// <summary>
    /// Удаляет сохраненные данные текстуры из PlayerPrefs.
    /// </summary>
    public static void Delete() => PlayerPrefs.DeleteKey(KEY);

    /// <summary>
    /// Загружает текстуру из PlayerPrefs.
    /// </summary>
    /// <returns>Объект TextureData с сохраненными пикселями.</returns>
    public static TextureData Load()
    {
        return JsonUtility.FromJson<TextureData>(PlayerPrefs.GetString(KEY));
    }

    /// <summary>
    /// Сохраняет текстуру в PlayerPrefs в формате JSON.
    /// </summary>
    /// <param name="texture">Текстура для сохранения.</param>
    public static void Save(Texture2D texture)
    {
        // Создаём объект для сериализации
        TextureData data = new(texture.width, texture.height);
        data.SetPixels(texture.GetPixels32());
        PlayerPrefs.SetString(KEY, JsonUtility.ToJson(data));
    }
}
