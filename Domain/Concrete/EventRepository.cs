using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Models.Client;
using DatabaseWebService.Models.Event;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Domain.Concrete
{
    public class EventRepository : IEventRepository
    {
        AnalizaProdajeEntities context = new AnalizaProdajeEntities();
        
        public EventRepository(AnalizaProdajeEntities dummyEntites)
        {
            context = dummyEntites;
        }

        public List<EventSimpleModel> GetAllEventModelList()
        {
            try
            {
                var query = from events in context.Dogodek
                            where events.idDogodek.Equals(events.idDogodek)
                            select new EventSimpleModel
                            {
                                DatumOtvoritve = events.DatumOtvoritve.HasValue ? events.DatumOtvoritve.Value : DateTime.MinValue,
                                DatumZadZaprtja = events.DatumZadZaprtja,
                                idDogodek = events.idDogodek,
                                idKategorija = events.idKategorija.HasValue ? events.idKategorija.Value : 0,
                                idStatus = events.idStatus.HasValue ? events.idStatus.Value : 0,
                                idStranka = events.idStranka.HasValue ? events.idStranka.Value : 0,
                                Izvajalec = events.Izvajalec.HasValue ? events.Izvajalec.Value : 0,
                                Kategorija = events.Kategorija.Naziv,
                                Opis = events.Opis,
                                OsebeIzvajalec = events.Osebe.Ime + " " + events.Osebe.Priimek,
                                OsebeSkrbnik = events.Osebe1.Ime + " " + events.Osebe1.Priimek,
                                Rok = events.Rok.HasValue ? events.Rok.Value : DateTime.MinValue,
                                Skrbnik = events.Skrbnik.HasValue ? events.Skrbnik.Value : 0,
                                StatusDogodek = events.StatusDogodek.Naziv,
                                Stranka = events.Stranka.NazivPrvi,
                                ts = events.ts.HasValue ? events.ts.Value : DateTime.MinValue,
                                tsIDOsebe = events.tsIDOsebe.HasValue ? events.tsIDOsebe.Value : 0
                            };

                return query.OrderByDescending(o => o.DatumOtvoritve).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<EventSimpleModel> GetAllEventModelList(int employeeID)
        {
            try
            {
                var query = from events in context.Dogodek
                            where events.Izvajalec.Value.Equals(employeeID)
                            select new EventSimpleModel
                            {
                                DatumOtvoritve = events.DatumOtvoritve.HasValue ? events.DatumOtvoritve.Value : DateTime.MinValue,
                                DatumZadZaprtja = events.DatumZadZaprtja,
                                idDogodek = events.idDogodek,
                                idKategorija = events.idKategorija.HasValue ? events.idKategorija.Value : 0,
                                idStatus = events.idStatus.HasValue ? events.idStatus.Value : 0,
                                idStranka = events.idStranka.HasValue ? events.idStranka.Value : 0,
                                Izvajalec = events.Izvajalec.HasValue ? events.Izvajalec.Value : 0,
                                Kategorija = events.Kategorija.Naziv,
                                Opis = events.Opis,
                                OsebeIzvajalec = events.Osebe.Ime + " " + events.Osebe.Priimek,
                                OsebeSkrbnik = events.Osebe1.Ime + " " + events.Osebe1.Priimek,
                                Rok = events.Rok.HasValue ? events.Rok.Value : DateTime.MinValue,
                                Skrbnik = events.Skrbnik.HasValue ? events.Skrbnik.Value : 0,
                                StatusDogodek = events.StatusDogodek.Naziv,
                                Stranka = events.Stranka.NazivPrvi,
                                ts = events.ts.HasValue ? events.ts.Value : DateTime.MinValue,
                                tsIDOsebe = events.tsIDOsebe.HasValue ? events.tsIDOsebe.Value : 0
                            };

                return query.OrderByDescending(o => o.DatumOtvoritve).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<EventSimpleModel> GetEventsListByClientID(int clientID)
        {
            try
            {
                var query = from events in context.Dogodek
                            where events.idStranka.Value.Equals(clientID)
                            select new EventSimpleModel
                            {
                                DatumOtvoritve = events.DatumOtvoritve.HasValue ? events.DatumOtvoritve.Value : DateTime.MinValue,
                                DatumZadZaprtja = events.DatumZadZaprtja,
                                idDogodek = events.idDogodek,
                                idKategorija = events.idKategorija.HasValue ? events.idKategorija.Value : 0,
                                idStatus = events.idStatus.HasValue ? events.idStatus.Value : 0,
                                idStranka = events.idStranka.HasValue ? events.idStranka.Value : 0,
                                Izvajalec = events.Izvajalec.HasValue ? events.Izvajalec.Value : 0,
                                Kategorija = events.Kategorija.Naziv,
                                Opis = events.Opis,
                                OsebeIzvajalec = events.Osebe.Ime + " " + events.Osebe.Priimek,
                                OsebeSkrbnik = events.Osebe1.Ime + " " + events.Osebe1.Priimek,
                                Rok = events.Rok.HasValue ? events.Rok.Value : DateTime.MinValue,
                                Skrbnik = events.Skrbnik.HasValue ? events.Skrbnik.Value : 0,
                                StatusDogodek = events.StatusDogodek.Naziv,
                                Stranka = events.Stranka.NazivPrvi,
                                ts = events.ts.HasValue ? events.ts.Value : DateTime.MinValue,
                                tsIDOsebe = events.tsIDOsebe.HasValue ? events.tsIDOsebe.Value : 0
                            };

                return query.OrderByDescending(o =>o.DatumOtvoritve).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public EventFullModel GetEvent(int eventID)
        {
            try
            {
                var query = from events in context.Dogodek
                            where events.idDogodek.Equals(eventID)
                            select new EventFullModel
                            {
                                DatumOtvoritve = events.DatumOtvoritve.HasValue ? events.DatumOtvoritve.Value : DateTime.MinValue,
                                DatumZadZaprtja = events.DatumZadZaprtja,
                                idDogodek = events.idDogodek,
                                idKategorija = events.idKategorija.HasValue ? events.idKategorija.Value : 0,
                                idStatus = events.idStatus.HasValue ? events.idStatus.Value : 0,
                                idStranka = events.idStranka.HasValue ? events.idStranka.Value : 0,
                                Izvajalec = events.Izvajalec.HasValue ? events.Izvajalec.Value : 0,
                                //Kategorija = events.Kategorija.Naziv,
                                Opis = events.Opis,
                                //OsebeIzvajalec = events.Osebe.Ime + " " + events.Osebe.Priimek,
                                //OsebeSkrbnik = events.Osebe1.Ime + " " + events.Osebe1.Priimek,
                                Rok = events.Rok.HasValue ? events.Rok.Value : DateTime.MinValue,
                                Skrbnik = events.Skrbnik.HasValue ? events.Skrbnik.Value : 0,
                                //StatusDogodek = events.StatusDogodek.Naziv,
                                //Stranka = events.Stranka.NazivPrvi,
                                ts = events.ts.HasValue ? events.ts.Value : DateTime.MinValue,
                                tsIDOsebe = events.tsIDOsebe.HasValue ? events.tsIDOsebe.Value : 0,
                                Tip = events.Tip,
                                RokIzvedbe = events.RokIzvedbe
                            };

                EventFullModel model = query.FirstOrDefault();
                if (model != null)
                {
                    model.Sporocila = new List<MessageModel>();
                    model.Sporocila = GetMessagesByEventID(eventID);

                    model.SestanekDokumenti = new List<EventMeetingModel>();
                    model.SestanekDokumenti = GetEventMeetingsByEventID(eventID);
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public EventFullModel GetEvent(int eventID, int employeeID)
        {
            try
            {
                var query = from events in context.Dogodek
                            where events.idDogodek.Equals(eventID) && events.Izvajalec.Value.Equals(employeeID)
                            select new EventFullModel
                            {
                                DatumOtvoritve = events.DatumOtvoritve.HasValue ? events.DatumOtvoritve.Value : DateTime.MinValue,
                                DatumZadZaprtja = events.DatumZadZaprtja,
                                idDogodek = events.idDogodek,
                                idKategorija = events.idKategorija.HasValue ? events.idKategorija.Value : 0,
                                idStatus = events.idStatus.HasValue ? events.idStatus.Value : 0,
                                idStranka = events.idStranka.HasValue ? events.idStranka.Value : 0,
                                Izvajalec = events.Izvajalec.HasValue ? events.Izvajalec.Value : 0,
                                //Kategorija = events.Kategorija.Naziv,
                                Opis = events.Opis,
                                //OsebeIzvajalec = events.Osebe.Ime + " " + events.Osebe.Priimek,
                                //OsebeSkrbnik = events.Osebe1.Ime + " " + events.Osebe1.Priimek,
                                Rok = events.Rok.HasValue ? events.Rok.Value : DateTime.MinValue,
                                Skrbnik = events.Skrbnik.HasValue ? events.Skrbnik.Value : 0,
                                //StatusDogodek = events.StatusDogodek.Naziv,
                                //Stranka = events.Stranka.NazivPrvi,
                                ts = events.ts.HasValue ? events.ts.Value : DateTime.MinValue,
                                tsIDOsebe = events.tsIDOsebe.HasValue ? events.tsIDOsebe.Value : 0,
                                Tip = events.Tip,
                                RokIzvedbe = events.RokIzvedbe
                            };

                EventFullModel model = query.FirstOrDefault();
                if (model != null)
                {
                    model.Sporocila = new List<MessageModel>();
                    model.Sporocila = GetMessagesByEventID(eventID);

                    model.SestanekDokumenti = new List<EventMeetingModel>();
                    model.SestanekDokumenti = GetEventMeetingsByEventID(eventID);
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<MessageModel> GetMessagesByEventID(int eventID)
        {
            try
            {
                var query = from message in context.Sporocila
                            where message.IDDogodek.Value.Equals(eventID)
                            select new MessageModel
                            {
                                idSporocila = message.idSporocila,
                                IDDogodek = message.IDDogodek.HasValue ? message.IDDogodek.Value : 0,
                                OpisDel = message.OpisDel,
                                ts = message.ts.HasValue ? message.ts.Value : DateTime.MinValue,
                                tsIDOsebe = message.tsIDOsebe.HasValue ? message.tsIDOsebe.Value : 0
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<EventMeetingModel> GetEventMeetingsByEventID(int eventID)
        {
            try
            {
                var query = from meeting in context.DogodekSestanek
                            where meeting.DogodekID.Equals(eventID)
                            group meeting by meeting into m
                            select new EventMeetingModel
                            {
                                DogodekSestanekID = m.Key.DogodekSestanekID,
                                DogodekID = m.Key.DogodekID,
                                Datum = m.Key.Datum,
                                Opis = m.Key.Opis,
                                Tip = m.Key.Tip,
                                ts = m.Key.ts.HasValue ? m.Key.ts.Value : DateTime.MinValue,
                                tsIDOsebe = m.Key.tsIDOsebe.HasValue ? m.Key.tsIDOsebe.Value : 0,
                                ImeInPriimekOsebe = context.Osebe
                                                        .Where(o => o.idOsebe == m.Key.tsIDOsebe)
                                                        .Select(ip => new { ImeInPriimekOsebe  = ip.Ime + " " + ip.Priimek})
                                                        .FirstOrDefault().ImeInPriimekOsebe
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<EventStatusModel> GetEventStatuses()
        {
            try
            {
                var query = from eventStatus in context.StatusDogodek
                            where eventStatus.idStatusDogodek.Equals(eventStatus.idStatusDogodek)
                            select new EventStatusModel
                            {
                                idStatusDogodek = eventStatus.idStatusDogodek,
                                Koda = eventStatus.Koda,
                                Naziv = eventStatus.Naziv,
                                ts = eventStatus.ts.HasValue ? eventStatus.ts.Value : DateTime.MinValue,
                                tsIDOsebe = eventStatus.tsIDOsebe.HasValue ? eventStatus.tsIDOsebe.Value : 0
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }


        public int SaveEvent(EventFullModel model, bool updateRecord = true)
        {
            try
            {
                Dogodek newEvent = new Dogodek();
                newEvent.idDogodek = model.idDogodek;
                newEvent.idStranka = model.idStranka;
                newEvent.idKategorija = model.idKategorija;
                //newEvent.Skrbnik = model.Skrbnik;
                newEvent.Skrbnik = context.OsebeNadrejeni.Where(od => od.idOseba == model.Izvajalec.Value).FirstOrDefault().idNadrejeni;
                newEvent.Izvajalec = model.Izvajalec;
                newEvent.idStatus = model.idStatus;
                newEvent.Opis = model.Opis;
                newEvent.DatumOtvoritve = model.DatumOtvoritve;
                newEvent.Rok = model.Rok;
                newEvent.DatumZadZaprtja = model.DatumZadZaprtja;
                newEvent.Tip = model.Tip;
                newEvent.RokIzvedbe = model.RokIzvedbe;

                if (newEvent.idDogodek == 0)
                {
                    newEvent.ts = DateTime.Now;
                    newEvent.tsIDOsebe = model.tsIDOsebe;

                    context.Dogodek.Add(newEvent);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Dogodek original = context.Dogodek.Where(d => d.idDogodek == newEvent.idDogodek).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(newEvent);
                        context.SaveChanges();
                    }
                }

                return newEvent.idDogodek;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }

        }

        public bool DeleteEvent(int eventID)
        {
            try
            {
                var events = context.Dogodek.Where(d => d.idDogodek == eventID).FirstOrDefault();

                if (events != null)
                {
                    context.Dogodek.Remove(events);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }


        public int SaveMessage(MessageModel model, bool updateRecord = true)
        {
            try
            {
                Sporocila message = new Sporocila();

                message.idSporocila = model.idSporocila;
                message.IDDogodek = model.IDDogodek;
                message.OpisDel = model.OpisDel;

                if (message.idSporocila == 0)
                {
                    message.ts = DateTime.Now;
                    message.tsIDOsebe = model.tsIDOsebe;

                    context.Sporocila.Add(message);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Sporocila original = context.Sporocila.Where(s => s.idSporocila == message.idSporocila).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(message);
                        context.SaveChanges();
                    }
                }

                return message.idSporocila;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteMessage(int messageID, int eventID)
        {
            try
            {
                var events = context.Sporocila.Where(s => s.idSporocila == messageID && s.IDDogodek == eventID).FirstOrDefault();

                if (events != null)
                {
                    context.Sporocila.Remove(events);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }


        public Sporocila GetMessageByID(int messageID)
        {
            try
            {
                var message = context.Sporocila.Where(s => s.idSporocila == messageID).FirstOrDefault();

                return message;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }

        public List<EventSimpleModel> GetEventsByClientIDAndCategorieID(int clientID, int categorieID, DateTime min, DateTime max)
        {
            try
            {
                var query = from events in context.Dogodek
                            where events.idStranka.Value.Equals(clientID) && events.idKategorija.Value.Equals(categorieID) &&
                            events.DatumOtvoritve.Value.CompareTo(min) >= 0 && events.DatumOtvoritve.Value.CompareTo(max) <= 0
                            select new EventSimpleModel
                            {
                                DatumOtvoritve = events.DatumOtvoritve.HasValue ? events.DatumOtvoritve.Value : DateTime.MinValue,
                                DatumZadZaprtja = events.DatumZadZaprtja,
                                idDogodek = events.idDogodek,
                                idKategorija = events.idKategorija.HasValue ? events.idKategorija.Value : 0,
                                idStatus = events.idStatus.HasValue ? events.idStatus.Value : 0,
                                idStranka = events.idStranka.HasValue ? events.idStranka.Value : 0,
                                Izvajalec = events.Izvajalec.HasValue ? events.Izvajalec.Value : 0,
                                Kategorija = events.Kategorija.Naziv,
                                Opis = events.Opis,
                                OsebeIzvajalec = events.Osebe.Ime + " " + events.Osebe.Priimek,
                                OsebeSkrbnik = events.Osebe1.Ime + " " + events.Osebe1.Priimek,
                                Rok = events.Rok.HasValue ? events.Rok.Value : DateTime.MinValue,
                                Skrbnik = events.Skrbnik.HasValue ? events.Skrbnik.Value : 0,
                                StatusDogodek = events.StatusDogodek.Naziv,
                                Stranka = events.Stranka.NazivPrvi,
                                ts = events.ts.HasValue ? events.ts.Value : DateTime.MinValue,
                                tsIDOsebe = events.tsIDOsebe.HasValue ? events.tsIDOsebe.Value : 0
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }

        }

        public int SaveEventMeeting(EventMeetingModel model, bool updateRecord = true)
        {
            try
            {
                DogodekSestanek eventMeeting = new DogodekSestanek();

                eventMeeting.DogodekSestanekID = model.DogodekSestanekID;
                eventMeeting.DogodekID = model.DogodekID;
                eventMeeting.Tip = model.Tip;
                eventMeeting.Datum = model.Datum;
                eventMeeting.Opis = model.Opis;

                if (eventMeeting.DogodekSestanekID == 0)
                {
                    eventMeeting.ts = DateTime.Now;
                    eventMeeting.tsIDOsebe = model.tsIDOsebe;

                    context.DogodekSestanek.Add(eventMeeting);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        DogodekSestanek original = context.DogodekSestanek.Where(ds => ds.DogodekSestanekID == eventMeeting.DogodekSestanekID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(eventMeeting);
                        context.SaveChanges();
                    }
                }

                return eventMeeting.DogodekSestanekID;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteEventMeeting(int eventMeetingID, int eventID)
        {
            try
            {
                var events = context.DogodekSestanek.Where(ds => ds.DogodekSestanekID== eventMeetingID && ds.DogodekID == eventID).FirstOrDefault();

                if (events != null)
                {
                    context.DogodekSestanek.Remove(events);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }

        public DogodekSestanek GetEventMeetingByID(int eventMeetingID)
        {
            try
            {
                var meeting = context.DogodekSestanek.Where(ds => ds.DogodekSestanekID == eventMeetingID).FirstOrDefault();

                return meeting;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }
    }
}