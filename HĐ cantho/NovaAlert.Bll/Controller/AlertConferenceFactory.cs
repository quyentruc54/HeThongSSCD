using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;
using System.Collections.Generic;

namespace NovaAlert.Bll.Controller
{
    public static class AlertConferenceFactory
    {
        public static AlertConferenceController CreateAlertConferenceController(ClientAppViewModel app, IEnumerable<UnitPhoneViewModel> units, eTaskType taskType)
        {
            if (taskType == eTaskType.CTT)
            {
                return new AlertConferenceController(app, units);
            }

            return new CCPK_AlertConferenceController(app, units);
        }
    }
}
