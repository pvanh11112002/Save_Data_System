using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

#if HAS_NEWTONSOFT
using Newtonsoft.Json;
#endif

namespace AnhPV.SaveSystem
{
    public static class SaveSystem
    {
        // --- CẤU HÌNH ---
        private const bool USE_COMPRESSION = false; // Bật/Tắt nén
        private const bool USE_NEWTONSOFT = true;  // Bật/Tắt Newtonsoft

        private static string _rootPath = "";

        /// <summary>
        /// Khởi tạo đường dẫn lưu trữ từ luồng chính.
        /// Cần thiết để có thể gọi Save/Load từ luồng phụ (ThreadPool).
        /// </summary>
        public static void Initialize(string persistentDataPath)
        {
            _rootPath = persistentDataPath;
            Debug.Log($"[SaveSystem] Initialized with path: {_rootPath}");
        }

        private static readonly object _fileLock = new object();

        // --- LOGIC LƯU (SAVE) ---
        public static void Save<T>(string filename, T data)
        {
            string path = GetPath(filename);
            string tmpPath = path + ".tmp";
            string backupPath = path + ".bak";
            string json = ToJson(data);

            lock (_fileLock)
            {
                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                try
                {
                    // 1. Viết vào file tmp trước (Atomic Save)
                    if (USE_COMPRESSION)
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(json);
                        using (FileStream fs = new FileStream(tmpPath, FileMode.Create))
                        using (GZipStream gzip = new GZipStream(fs, CompressionMode.Compress))
                        {
                            gzip.Write(bytes, 0, bytes.Length);
                        }
                    }
                    else
                    {
                        File.WriteAllText(tmpPath, json);
                    }

                    // 2. Ghi đè file thật (Rollback-safe)
                    if (File.Exists(backupPath)) File.Delete(backupPath);
                    if (File.Exists(path)) File.Move(path, backupPath);
                    File.Move(tmpPath, path);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[SaveSystem] Ghi file gặp lỗi: {e.Message}");
                }
            }

            Debug.Log($"[SaveSystem] Saved (Safe): {filename}");
        }

        // --- LOGIC TẢI (LOAD) ---
        public static T Load<T>(string filename) where T : new()
        {
            string path = GetPath(filename);
            string backupPath = path + ".bak";

            lock (_fileLock)
            {
                if (!File.Exists(path))
                {
                    if (File.Exists(backupPath))
                    {
                        File.Copy(backupPath, path);
                        Debug.LogWarning($"[SaveSystem] Khôi phục file save từ Backup: {filename}");
                    }
                    else
                    {
                        return new T(); // Trả về data mới nếu file không tồn tại
                    }
                }

                try
                {
                    string json = "";
                    if (USE_COMPRESSION)
                    {
                        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
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
        }

        public static void Delete(string filename)
        {
            string path = GetPath(filename);
            if (File.Exists(path)) File.Delete(path);
            Debug.Log($"[SaveSystem] Deleted: {filename}");
        }

        // --- HELPER NỘI BỘ ---
        private static string GetPath(string filename)
        {
            if (string.IsNullOrEmpty(_rootPath))
            {
                // Fallback cho trường hợp chưa kịp Init (chỉ an toàn nếu gọi từ Main Thread)
                _rootPath = Application.persistentDataPath;
            }
            return Path.Combine(_rootPath, filename);
        }

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
}
