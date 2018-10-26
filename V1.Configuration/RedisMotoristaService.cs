using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace V1.Configuration
{
    public class RedisMotoristaService<T> : BaseService<T>, IRedisService<T>
    {
        internal readonly IDatabase Db;
        protected readonly IRedisConnectionFactory ConnectionFactory;



        public RedisMotoristaService(IRedisConnectionFactory connectionFactory)
        {
            this.ConnectionFactory = connectionFactory;
            this.Db = this.ConnectionFactory.Connection().GetDatabase();
        }


        public void SetData<T>(string key, T data)
        {
            var output = JsonConvert.SerializeObject(data);
            this.Db.SetAdd(key, output);

        }
        public List<T> GetAllByKey(string key)
        {
            RedisValue[] vals = this.Db.SetMembers(key);
            List<T> opList = new List<T>();
            foreach (RedisValue val in vals)
            {
                string item_val = val.ToString();
                T item = JsonConvert.DeserializeObject<T>(item_val);
                opList.Add(item);
            }
            return opList;
        }

        public void Delete(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Contains(":")) throw new ArgumentException("invalid key");

            key = this.GenerateKey(key);
            this.Db.KeyDelete(key);
        }

        public T Get(string key)
        {
            key = this.GenerateKey(key);
            var hash = this.Db.HashGetAll(key);
            return this.MapFromHash(hash);
        }

        public T GetAll(string key)
        {
            key = this.GenerateKey(key);
            var hash = this.Db.HashGetAll(key);
            return this.MapFromHash(hash);
        }
        public void Save(string key, T obj)
        {
            if (obj != null)
            {
                var hash = this.GenerateHash(obj);
                key = this.GenerateKey(key);

                if (this.Db.HashLength(key) == 0)
                {
                    this.Db.HashSet(key, hash);
                }
                else
                {
                    var props = this.Properties;
                    foreach (var item in props)
                    {
                        if (this.Db.HashExists(key, item.Name))
                        {
                            this.Db.HashIncrement(key, item.Name, Convert.ToInt32(item.GetValue(obj)));
                        }
                    }
                }

            }
        }
    }
}
