using System;
using System.Collections.Generic;
using System.Text;

namespace V1.Configuration
{
    public interface IRedisService<T>
    {
        T Get(string key);

        void Save(string key, T obj);

        void Delete(string key);
    }
}
