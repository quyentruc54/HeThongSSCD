using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using NovaAlert.Common.Setting;

namespace NovaAlert.Dal
{
    internal static class ConnectionStringHelpers
    {
        const string _metadata = "res://*/NovaAlertModel.csdl|res://*/NovaAlertModel.ssdl|res://*/NovaAlertModel.msl";            

        public static string GetConstr()
        {
            // Initialize the EntityConnectionStringBuilder.
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = "System.Data.SqlClient";

            // Set the Metadata location.
            entityBuilder.Metadata = _metadata;

            // Set the provider-specific connection string.
            //entityBuilder.ProviderConnectionString = @"data source=thuypc;initial catalog=NovaAlert;user id=sa;multipleactiveresultsets=True"; // KK.Common.Setting.RegistryDatabaseSetting.Instance.GetConnectionString();
            entityBuilder.ProviderConnectionString = RegistryDatabaseSetting.Instance.GetConstr();
            
            return entityBuilder.ConnectionString;
        }

        public static EntityConnection CreateConnection()
        {
            return new EntityConnection(GetConstr());
            //return new SqlConnection(RegistryDatabaseSetting.Instance.GetConstr());
        }
    }
}
