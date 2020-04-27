using DatabaseWebService.Models.Client;
using DatabaseWebService.Models.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.Domain.Abstract
{
    public interface IEventRepository
    {
        List<EventSimpleModel> GetAllEventModelList();
        List<EventSimpleModel> GetAllEventModelList(int employeeID);

        EventFullModel GetEvent(int eventID);
        EventFullModel GetEvent(int eventID, int employeeID);
        List<EventSimpleModel> GetEventsListByClientID(int clientID);
        List<EventStatusModel> GetEventStatuses();
        int SaveEvent(EventFullModel model, bool updateRecord = true);
        bool DeleteEvent(int eventID);
        int SaveMessage(MessageModel model, bool updateRecord = true);
        bool DeleteMessage(int messageID, int eventID);

        Sporocila GetMessageByID(int messageID);

        List<EventSimpleModel> GetEventsByClientIDAndCategorieID(int clientID, int categorieID, DateTime min, DateTime max);

        List<EventMeetingModel> GetEventMeetingsByEventID(int eventID);
        int SaveEventMeeting(EventMeetingModel model, bool updateRecord = true);
        bool DeleteEventMeeting(int eventMeetingID, int eventID);
        DogodekSestanek GetEventMeetingByID(int eventMeetingID);
    }
}
