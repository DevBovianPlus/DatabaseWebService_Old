using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsPDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainNOZ.Abstract
{
    public interface IClientNOZRepository
    {
        List<ClientSimpleModel> GetAllClients();
        List<ClientSimpleModel> GetAllClients(int employeeID);
        ClientFullModel GetClientByID(int clientID);
        ClientFullModel GetClientByID(int clientID, int employeeID);
        int SaveClient(ClientFullModel model, bool updateRecord = true);
        bool DeleteClient(int clientID);
        /*ContactPersonModel GetContactPersonByID(int contactPersonID);
        int SaveContactPerson(ContactPersonModel model, bool updateRecord = true);
        bool DeleteContactPerson(int contactPersonID, int clientID);*/
        List<ClientEmployeeModel> GetClientEmployeeModelList(int clientID);
        int SaveClientEmployee(ClientEmployeeModel model, bool updateRecord = true);
        bool DeleteClientEmployee(int clientID, int employeeID);
        bool ClientEmployeeExist(int clientID, int employeeID);

        
        List<ClientSimpleModel> GetAllClientsByType(string typeCode, int employeeID = 0);
        /*List<ContactPersonModel> GetContactPersonModelListByName(string SupplierName);
        List<ContactPersonModel> GetContactPersonModelList(int ClientID);*/

        ClientType GetClientTypeByCode(string typeCode);
        ClientType GetClientTypeByID(int typeID);
        List<ClientType> GetClientTypes();
        /*List<LanguageModel> GetLanguages();
        List<DepartmentModel> GetDepartments();*/

        ClientFullModel GetClientByName(string clientName);
        int GetClientByNameOrInsert(string clientName);
    }
}
