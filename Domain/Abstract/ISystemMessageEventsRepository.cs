using DatabaseWebService.Common.Enums;
using DatabaseWebService.Models;
using DatabaseWebService.Models.EmailMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Domain.Abstract
{
    public interface ISystemMessageEventsRepository
    {
        List<SystemMessageEvents> GetUnProcessedMesseges();

        void ProcessNewMessage(SystemMessageEvents message);
        void ProcessAutoMessage(SystemMessageEvents message);
        void ProcessEventMessage(SystemMessageEvents message, Enums.SystemMessageEventCodes eventCode = Enums.SystemMessageEventCodes.EVENT_DOGODEK);

        void SaveEmailEventMessage(EmailMessageModel model);

        void GetUnProcessedRecordsAvtomatika();

        List<AutomaticsModel> GetAutomaticsByEmployeeIDAndStatus(int employeeID, int status);

        void UpdateSystemMessageEvents(SystemMessageEvents message);

        void GetEventsWithNoReportForMeeting();
        void GetEventsWithNoPreparationForMeeting();

        bool EventPreparationOrReport(Enums.SystemMessageEventCodes eventCode);
        bool EventPreparationOrReport(string eventCode);
        bool EventPreparation(Enums.SystemMessageEventCodes eventCode);
        int GetEmployeeHierarchyLevel(Enums.SystemMessageEventCodes eventCode);
    }
}