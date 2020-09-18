using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Domain.Abstract
{
    public interface IClientRepository
    {
        List<ClientSimpleModel> GetAllClients();
        List<ClientSimpleModel> GetAllClients(int employeeID);

        ClientFullModel GetClientByID(int clientID);
        ClientFullModel GetClientByID(int clientID, int employeeID);

        int SaveClient(ClientFullModel model, bool updateRecord = true);

        bool DeleteClient(int clientID);

        int SaveClientPlan(PlanModel model, bool updateRecord = true);

        bool DeletePlan(int planID, int clientID);

        int SaveContactPerson(ContactPersonModel model, bool updateRecord = true);

        bool DeleteContactPerson(int contactPersonID, int clientID);

        List<CategorieModel> GetCategories();

        List<ClientEmployeeModel> GetClientEmployeeModelList(int clientID);

        int SaveClientEmployee(ClientEmployeeModel model, bool updateRecord = true);

        bool DeleteClientEmployee(int clientID, int employeeID);
        bool ClientEmployeeExist(int clientID, int employeeID);

        ClientSimpleModel GetClientSimpleModelByCode(int clientID);
        CategorieModel GetCategorieByID(int categorieID);

        int SaveDevice(DevicesModel model, bool updateRecord = true);

        bool DeleteDevice(int deviceID, int clientID);

        int SaveNotes(NotesModel model, bool updateRecord = true);

        bool DeleteNotes(int NotesID, int clientID);

        int GetNotesCountByCode(string code);
        int GetClientsCountByCode(string code);
        int GetDevicesCountByCode(string code);

        List<ClientCategorieModel> GetClientCategorieModelList(int clientID);

        int SaveClientCategorie(ClientCategorieModel model, bool updateRecord = true);

        bool DeleteClientCategorie(int clientID, int clientCategorieID);

        List<CategorieModel> GetClientFreeCategories(int clientID, int catToSkip = 0);

        List<ClientSimpleModel> FilterClientsByPropertyNames(string propertyName, string containsValue);
    }
}