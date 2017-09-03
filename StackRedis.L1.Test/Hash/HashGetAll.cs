using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class HashGetAll : UnitTestBase
    {
    
        [TestMethod]
        public void HashGetAll_Users()
        {
            const string dicKey = "AllUsers -D0F5D7A2-88CF-4F58-B9D4-C948F4B121C0";

            var all = _memDb.HashGetAllDictionary<int, string>(dicKey);

        }

        [TestMethod]
        public void HashGetAll_Dictionary()
        {
            const string dicKey = "HashGetAll_Dictionary";
            Dictionary<int, string> dic = new Dictionary<int, string>();

            dic.Add(1, "aa");
            dic.Add(2, "bb");
            dic.Add(3, "cc");

            _memDb.HashSetDictionary(dicKey, dic);

            var all = _memDb.HashGetAllDictionary<int, string>(dicKey);

            Assert.AreEqual(1, CallsByMemDb);

            Assert.AreEqual(3, all.Count);

            _memDb.HashSetDictionaryItem(dicKey, 4, "dd");
            Assert.AreEqual(2, CallsByMemDb);

            _memDb.HashSetDictionaryItem(dicKey, 5, "ee");
            Assert.AreEqual(3, CallsByMemDb);

            all = _memDb.HashGetAllDictionary<int, string>(dicKey);
            Assert.AreEqual(5, all.Count);
        }

        [TestMethod]
        public void HashGetAll_Simple()
        {
            _redisDirectDb.HashSet("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            var all = _memDb.HashGetAll("hashKey");
            Assert.AreEqual("key1", (string)all[0].Name);
            Assert.AreEqual("value1", (string)all[0].Value);
            Assert.AreEqual("key2", (string)all[1].Name);
            Assert.AreEqual("value2", (string)all[1].Value);
            Assert.AreEqual(1, CallsByMemDb);

            //Retrieve an individual value
            Assert.AreEqual("value1", (string)_memDb.HashGet("hashKey", "key1"));
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public void HashGetAll_ClearsOldItems()
        {
            _redisDirectDb.HashSet("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            var all = _memDb.HashGetAll("hashKey");
            Assert.AreEqual(1, CallsByMemDb);

            //Delete an item in redis
            _redisDirectDb.HashDelete("hashKey", "key1");
            all = _memDb.HashGetAll("hashKey");
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(1, all.Length);
            Assert.AreEqual("key2", (string)all[0].Name);
            Assert.AreEqual("value2", (string)all[0].Value);
        }

        [TestMethod]
        public async Task HashGetAllAsync_Simple()
        {
            await _redisDirectDb.HashSetAsync("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            var all = await _memDb.HashGetAllAsync("hashKey");
            Assert.AreEqual("key1", (string)all[0].Name);
            Assert.AreEqual("value1", (string)all[0].Value);
            Assert.AreEqual("key2", (string)all[1].Name);
            Assert.AreEqual("value2", (string)all[1].Value);
            Assert.AreEqual(1, CallsByMemDb);

            //Retrieve an individual value
            Assert.AreEqual("value1", (string)await _memDb.HashGetAsync("hashKey", "key1"));
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public async Task HashGetAllAsync_ClearsOldItems()
        {
            await _redisDirectDb.HashSetAsync("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            var all = await _memDb.HashGetAllAsync("hashKey");
            Assert.AreEqual(1, CallsByMemDb);

            //Delete an item in redis
            await _redisDirectDb.HashDeleteAsync("hashKey", "key1");
            all = await _memDb.HashGetAllAsync("hashKey");
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(1, all.Length);
            Assert.AreEqual("key2", (string)all[0].Name);
            Assert.AreEqual("value2", (string)all[0].Value);
        }
    }
}
