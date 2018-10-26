using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace V1.Configuration
{
    public interface IRedisConnectionFactory
    {
        ConnectionMultiplexer Connection();
    }
}
