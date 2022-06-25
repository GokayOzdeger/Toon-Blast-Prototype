using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Utilities {
    [Serializable]
    public class Byteable<T> where T : new() {
        public byte[] ToByteArray() 
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, this);
                return ms.ToArray();
            }
        }
        
        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
    }
}