using Newtonsoft.Json;
using System.IO;

public class Utilities
    {
        public static byte[] LoadPngToBytes(string file)    
        {
            FileStream fs = File.OpenRead(file);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, (int) fs.Length);
            return data;
        }
    }