using NovaAlert.Communication.Base;
using System;

namespace NovaAlert.Communication.ATModem
{
    public class ItuV250Modem : AbstractModem
    {

        public ItuV250Modem(IDataLink link)
            : base(link)
        {
        }

        public override string ManufacturerId
        {
            get
            {
                return ExtractLineData(SendAndExtractData("AT+GMI"));
            }
        }
        
        public override string ModelId
        {
            get
            {
                return ExtractLineData(SendAndExtractData("AT+GMM"));
            }
        }

        public override string RevisionId
        {
            get
            {
                return ExtractLineData(SendAndExtractData("AT+GMR"));
            }
        }

        public override string SerialNumber
        {
            get
            {
                return ExtractLineData(SendAndExtractData("AT+GSN"));
            }
        }
                
        public override string ObjectId
        {
            get
            {
                return ExtractLineData(SendAndExtractData("AT+GOI"));
            }
        }

        public override ModemCapabilities[] Capabilities
        {
            get
            {
                //return ModemCapabilities.parseCapabilities(extractLineData(SendAndExtractData("AT+GCAP")));
                throw new NotImplementedException();
            }
        }

        public override string CountryOfInstallation
        {
            get
            {
                return SendAndExtractData("AT+GCI?")[0];
            }
        }

        public override void Dispose()
        {
            var dl = this.DataLink as IDisposable;
            if(dl != null)
            {
                dl.Dispose();
            }
        }
    }
}
