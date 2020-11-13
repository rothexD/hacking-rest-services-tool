using System;
using System.Collections.Generic;
using System.Text;
using Hacking_Rest_SqlInjetor.DatabaseInformations;

namespace Hacking_Rest_SqlInjetor.ServiceHandlers
{
    public interface IServiceHandler
    {
        public void GetFormDataIntoDatabaseInformation(string fakeresponse);
        public IDatabaseInformation Database { get; }
    }
}
