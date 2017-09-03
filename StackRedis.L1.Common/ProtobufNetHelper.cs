using System;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using ProtoBuf;
using StackExchange.Redis;

namespace StackRedis.L1.Common
{
    /// <summary>
    /// Helper class to interact with protobuf-net library.
    /// </summary>
    public class ProtobufNetHelper
    {
        public static string SerializeKey(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static byte[] SerializeValue(object obj)
        {
            return SerializeValue(obj, false);
        }

        public static byte[] SerializeValue(object obj, bool compres)
        {
            return ProtobufNetHelper.ObjectToByteArrayFromProtoBuff(obj);
        }

        public static T DeserializeKey<T>(string serialized)
        {
            return JsonConvert.DeserializeObject<T>(serialized);
        }
        public static T DeserializeValue<T>(RedisValue serialized)
        {
            return DeserializeValue<T>(serialized, false);
        }


        public static T DeserializeValue<T>(RedisValue serialized, bool decompress)
        {
            return ProtobufNetHelper.ByteArrayToObjectFromProtoBuff<T>(serialized);
        }

        public static byte[] ObjectToByteArrayFromProtoBuff(Object obj)
        {
            return ObjectToByteArrayFromProtoBuff(obj, false);
        }

        public static byte[] ObjectToByteArrayFromProtoBuff(Object obj, bool compress)
        {
            byte[] output;
            if (obj == null)
            {
                return null;
            }

            using (MemoryStream inputStream = new MemoryStream())
            {
                Serializer.Serialize(inputStream, obj);

                output = inputStream.ToArray();
            }

            if (compress)
            {
                // Compress it
                output = Compress(output);
            }

            return output;
        }

        public static T ByteArrayToObjectFromProtoBuff<T>(byte[] arrBytes)
        {
            return ByteArrayToObjectFromProtoBuff<T>(arrBytes, false);
        }

        public static T ByteArrayToObjectFromProtoBuff<T>(byte[] arrBytes, bool decompress)
        {
            if (arrBytes != null)
            {
                if (decompress)
                {
                    // Decompress it
                    arrBytes = Decompress(arrBytes);
                }

                using (MemoryStream inputStream = new MemoryStream(arrBytes))
                {
                    T obj;

                    obj = Serializer.Deserialize<T>(inputStream);

                    return obj;
                }
            }
            return default(T);
        }

        public static byte[] Compress(byte[] rawData)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            compressedzipStream.Write(rawData, 0, rawData.Length);
            compressedzipStream.Close();
            return ms.ToArray();
        }

        public static byte[] Decompress(byte[] zippedData)
        {
            MemoryStream ms = new MemoryStream(zippedData);
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();
        }


    }//end class
}//end namespace
