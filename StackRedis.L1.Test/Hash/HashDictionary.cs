using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class HashGetAllDictionary : UnitTestBase
    {
        [TestMethod]
        public void HashGetAllDictionary_Simple()
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
        public void HashDeleteDictionary_Simple()
        {
            const string dicKey = "HashDeleteDictionary_Simple";
            Dictionary<int, string> dic = new Dictionary<int, string>();

            dic.Add(1, "aa");
            dic.Add(2, "bb");
            dic.Add(3, "cc");

            _memDb.HashSetDictionary(dicKey, dic);

            Assert.AreEqual(1, CallsByMemDb);

            _memDb.HashDeleteDictionaryItem<int, string>(dicKey, 3);

            Assert.AreEqual(2, CallsByMemDb);

            string dicValue;
            Assert.IsFalse(_memDb.TryGetHashDictionaryValue(dicKey, 3, out dicValue));
            Assert.AreEqual(3, CallsByMemDb);

            Assert.IsTrue(_memDb.TryGetHashDictionaryValue(dicKey, 2, out dicValue));

            var all = _memDb.HashGetAllDictionary<int, string>(dicKey);
            Assert.AreEqual(2, all.Count);

            _memDb.HashSetDictionaryItem(dicKey, 4, "dd");
            _memDb.HashSetDictionaryItem(dicKey, 5, "ee");
            _memDb.HashSetDictionaryItem(dicKey, 6, "ff");

            _memDb.HashDeleteDictionaryItem<int, string>(dicKey, 5);

            Assert.IsFalse(_memDb.TryGetHashDictionaryValue(dicKey, 5, out dicValue));

            all = _memDb.HashGetAllDictionary<int, string>(dicKey);
            Assert.AreEqual(4, all.Count);
        }

    }
}
