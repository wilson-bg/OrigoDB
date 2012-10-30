using System;
using System.Collections.Generic;

namespace LiveDomain.Core
{
	internal static class ConnectionPools
	{
		private static Dictionary<string, ConnectionPool> _connectionPools = new Dictionary<string, ConnectionPool>();

		public static ConnectionPool PoolFor(RemoteClientConfiguration configuration)
		{
			lock (_connectionPools)
			{
				ConnectionPool pool;
				var key = configuration.ToString();
				if(configuration.DedicatedPool) key +=  Guid.NewGuid();

				if (!_connectionPools.TryGetValue(key, out pool))
				{
					pool = new ConnectionPool(() => new RemoteConnection(configuration.Host, configuration.Port), configuration.MaxConnections);
					_connectionPools[key] = pool;

				}
				return pool;
			}
		}
	}
}