using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using System.ServiceModel.Channels;

namespace Sage.Mas.Ws
{
    public class MasExtended : IMasExtended
    {
        public bool CreateZip(string PostCode, string City, string StateCode, string CountryCode, string APIKey)
        {
            try
            {
                InsertZip(PostCode, City, StateCode, CountryCode, APIKey);
                
                //add event log writing for successful inserts
                string sEvent;
                sEvent = "ZipCode Insertion Successful" + "\n\nPostal Code : " + PostCode + "\n\nCity : " + City + "\n\nState Code : " + StateCode +  "\n\nCountry Code : " + CountryCode + "\n\nClient IP : " + GetRemoteClientIP();
                EventLog.WriteEntry("Sage.MAS90.WebServices.Extended", sEvent, EventLogEntryType.Information, 1);
                return true;
            }
            
            catch(Exception e)
            {
                //add event log writing for errors
                string sEvent;
                sEvent = e.Message + "\n\nClient IP:" + GetRemoteClientIP();
                EventLog.WriteEntry("Sage.MAS90.WebServices.Extended", sEvent, EventLogEntryType.Error, 1);
                throw new FaultException(new FaultReason(e.Message), new FaultCode("Error"));
            }         
        }

        private void InsertZip(string ZipCode, string City, string StateCode, string CountryCode, string APIKey)
        {
            if (APIKey != "YOUR_API_KEY")
            {
                throw new ArgumentException("Incorrect API Key");
            }

            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["MAS200"];
            string connectionString = connectionStringSettings.ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand check = new SqlCommand("SELECT * FROM SY_ZipCode WHERE ZipCode = @ZipValue", connection))
                {
                    check.Parameters.Add(new SqlParameter("ZipValue", ZipCode));

                    using (SqlDataReader reader = check.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            //zip exists
                            throw new ArgumentException("Zip Code already exists in Database");
                        }
                    }
                }

                // Truncate to field limits
                ZipCode = Truncate(ZipCode, 10);
                City = Truncate(City, 20);
                StateCode = Truncate(StateCode, 2);
                CountryCode = Truncate(CountryCode, 3);

                //Insert into DB
                using (SqlCommand insert = new SqlCommand("INSERT INTO SY_ZipCode (ZipCode, City, StateCode, CountryCode) VALUES (@ZipCode,@City,@StateCode,@CountryCode)", connection))
                {
                    insert.Parameters.Add(new SqlParameter("ZipCode", ZipCode));
                    insert.Parameters.Add(new SqlParameter("City", City));
                    insert.Parameters.Add(new SqlParameter("StateCode", StateCode));
                    insert.Parameters.Add(new SqlParameter("CountryCode", CountryCode));

                    int ret = insert.ExecuteNonQuery();

                    if (ret != 1)
                    {
                        throw new InvalidOperationException("Insertion Failed!");
                    }

                }

            }

        } //end insert class

        private static string Truncate(string source, int length)
        {
            if (source.Length > length)
            {
                source = source.Substring(0, length);
            }
            return source;
        }

        private string GetRemoteClientIP()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties messageProperties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            return endpointProperty.Address;
        }


        
    }
}
