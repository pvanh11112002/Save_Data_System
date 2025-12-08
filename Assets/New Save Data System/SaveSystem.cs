using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

#if HAS_NEWTONSOFT
using Newtonsoft.Json;
#endif

public static class SaveSystem
{
    // --- CẤU HÌNH ---
    private const bool USE_COMPRESSION = true; // Bật/Tắt nén
    private const bool USE_NEWTONSOFT = true;  // Bật/Tắt Newtonsoft

    // --- LOGIC LƯU (SAVE) ---
    public static void Save<T>(string filename, T data)
    {
        string path = GetPath(filename);
        string json = ToJson(data);

        // Tạo thư mục nếu chưa có
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        if (USE_COMPRESSION)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (GZipStream gzip = new GZipStream(fs, CompressionMode.Compress))
            {
                gzip.Write(bytes, 0, bytes.Length);
            }
        }
        else
        {
            File.WriteAllText(path, json);
        }

        Debug.Log($"[SaveSystem] Saved: {filename}");
    }

    // --- LOGIC TẢI (LOAD) ---
    public static T Load<T>(string filename) where T : new()
    {
        string path = GetPath(filename);
        if (!File.Exists(path)) return new T(); // Trả về data mới nếu file không tồn tại

        try
        {
            string json = "";
            if (USE_COMPRESSION)
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                using (GZipStream gzip = new GZipStream(fs, CompressionMode.Decompress))
                using (StreamReader reader = new StreamReader(gzip))
                {
                    json = reader.ReadToEnd();
                }
            }
            else
            {
                json = File.ReadAllText(path);
            }
            return FromJson<T>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Load Error: {e.Message}. Recreated new data.");
            return new T();
        }
    }

    public static void Delete(string filename)
    {
        string path = GetPath(filename);
        if (File.Exists(path)) File.Delete(path);
        Debug.Log($"[SaveSystem] Deleted: {filename}");
    }

    // --- HELPER NỘI BỘ ---
    private static string GetPath(string filename) => Path.Combine(Application.persistentDataPath, filename);

    private static string ToJson<T>(T data)
    {
#if HAS_NEWTONSOFT
        if (USE_NEWTONSOFT) return JsonConvert.SerializeObject(data, Formatting.Indented);
#endif
        return JsonUtility.ToJson(data, true);
    }

    private static T FromJson<T>(string json)
    {
#if HAS_NEWTONSOFT
        if (USE_NEWTONSOFT) return JsonConvert.DeserializeObject<T>(json);
#endif
        return JsonUtility.FromJson<T>(json);
    }
}