using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;
using System.Collections.Generic;

namespace NovaAlert.Bll.Controller
{
    public class CCPK_AlertConferenceController : AlertConferenceController
    {
        public override eTaskType TaskType { get { return eTaskType.CCPK; } }

        public CCPK_AlertConferenceController(ClientAppViewModel app, IEnumerable<UnitPhoneViewModel> units)
            : base(app, units)
        {
        }

        protected override CallLog CreateCallLog()
        {
            var callLog = base.CreateCallLog();
            callLog.CallType = eCallType.CCPK;
            return callLog;
        }
    }
}
