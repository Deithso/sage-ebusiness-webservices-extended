using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Sage.Mas.Ws
{
    [ServiceContract(Namespace = "http://mas90.sage.com/ws")]
    public interface IMasExtended
    {

        [OperationContract]
        bool CreateZip(string PostCode, string City, string StateCode, string CountryCode, string APIKey);

    }
}
