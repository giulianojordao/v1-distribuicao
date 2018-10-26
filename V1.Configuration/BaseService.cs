using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace V1.Configuration
{
    public abstract class BaseService<T>
    {
        protected string Name => this.Type.Name;

        protected PropertyInfo[] Properties => this.Type.GetProperties();

        protected Type Type => typeof(T);

        /// <summary>
        /// Gera uma chave para uma Entrada Redis, segue a Convenção de Nome Redis de inserir dois pontos: para identificar valores
        /// </summary>
        /// <param name="key">Chave de identificador Redis</param>
        /// <returns>concatena a chave com o nome do tipo</returns>
        protected string GenerateKey(string key)
        {
            // return string.Concat(key.ToLower(), ":", this.Name.ToLower());
            return string.Concat(this.Name.ToLower(), ":", key.ToLower());
        }

        protected HashEntry[] GenerateHash(T obj)
        {
            var props = this.Properties;
            var hash = new HashEntry[props.Count()];

            for (var i = 0; i < props.Count(); i++)
                hash[i] = new HashEntry(props[i].Name, props[i].GetValue(obj).ToString());

            return hash;
        }

        protected T MapFromHash(HashEntry[] hash)
        {
            var obj = (T)Activator.CreateInstance(this.Type); //nova instância de T
            var props = this.Properties;

            for (var i = 0; i < props.Count(); i++)
            {
                for (var j = 0; j < hash.Count(); j++)
                {
                    if (props[i].Name == hash[j].Name)
                    {
                        var val = hash[j].Value;
                        var type = props[i].PropertyType;

                        if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                            if (string.IsNullOrEmpty(val))
                            {
                                props[i].SetValue(obj, null);
                            }
                        props[i].SetValue(obj, Convert.ChangeType(val, type));
                    }
                }
            }
            return obj;
        }
    }
}
