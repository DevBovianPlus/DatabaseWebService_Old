using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP;
using DatabaseWebService.ModelsOTP.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainOTP.Abstract
{
    public interface IClientOTPRepository
    {
        List<ClientSimpleModel> GetAllClients();
        List<ClientSimpleModel> GetAllClients(int employeeID);
        ClientFullModel GetClientByID(int clientID);
        ClientFullModel GetClientByID(int clientID, int employeeID);
        int SaveClient(ClientFullModel model, bool updateRecord = true);
        bool DeleteClient(int clientID);
        int SaveContactPerson(ContactPersonModel model, bool updateRecord = true);
        bool DeleteContactPerson(int contactPersonID, int clientID);
        List<ClientEmployeeModel> GetClientEmployeeModelList(int clientID);
        int SaveClientEmployee(ClientEmployeeModel model, bool updateRecord = true);
        bool DeleteClientEmployee(int clientID, int employeeID);
        bool ClientEmployeeExist(int clientID, int employeeID);
        List<ContactPersonModel> GetContactPersonModelList(int clientID);

        List<ClientSimpleModel> GetAllClientsByType(string typeCode);
        List<ClientSimpleModel> GetAllClientsByType(int employeeID, string typeCode);

        ClientType GetClientTypeByCode(string typeCode);
        ClientType GetClientTypeByID(int typeID);
        List<ClientType> GetClientTypes();
        List<ClientTransportType> GetClientTransportTypes();
        ClientTransportType GetClientTransportTypeByID(int id);
        int SaveClientTransportType(ClientTransportType model, bool updateRecord = true);
        bool DeleteClientTransportType(int trasportTypeID);
        ClientFullModel GetClientByName(string clientName);

        List<LanguageModelOTP> GetLanguages();
    }
}
