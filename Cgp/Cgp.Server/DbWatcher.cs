using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Server
{
    [Serializable()]
    public class DbWatcher : MarshalByRefObject
    {
        private static volatile DbWatcher _singleton = null;
        private static object _syncRoot = new object();

        public static DbWatcher Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new DbWatcher();
                    }

                return _singleton;
            }
        }

        public DbWatcher GetSingleton()
        {
            if (_singleton == null)
            {
                _singleton = new DbWatcher();
            }
            return _singleton;
        }

        public event Action<object, ObjectType, ObjectDatabaseAction> CgpDBObjectChanged;
        public void DbObjectChanged(Beans.AOrmObject ormObject, ObjectDatabaseAction objectDatabaseAction)
        {
            if (ormObject == null)
            {
                return;
            }

            if (CgpDBObjectChanged != null)
            {
                try
                {
                    CgpDBObjectChanged(ormObject.GetId(), ormObject.GetObjectType(), objectDatabaseAction);
                }
                catch
                { }
            }
        }

        public event Action<Person, ObjectDatabaseAction> CgpDBPersonChanged;
        public void DbPersonChanged(Beans.Person person, ObjectDatabaseAction objectDatabaseAction)
        {
            if (person == null)
            {
                return;
            }

            if (CgpDBPersonChanged != null)
            {
                try
                {
                    CgpDBPersonChanged(person, objectDatabaseAction);
                }
                catch
                { }
            }
        }

        public event Action<Person, Card> CgpDBCardChanged;
        public void DbCardChanged(Beans.Person person, Card card)
        {
            if (card == null)
            {
                return;
            }

            if (CgpDBCardChanged != null)
            {
                try
                {
                    CgpDBCardChanged(person, card);
                }
                catch
                { }
            }
        }

        public event Action<Guid, ObjectDatabaseAction> CgpDBPersonBeforeUD;
        public void DbPersonBeforeUD(Beans.Person person, ObjectDatabaseAction objectDatabaseAction)
        {
            if (person == null)
            {
                return;
            }

            if (CgpDBPersonBeforeUD != null)
            {
                try
                {
                    CgpDBPersonBeforeUD(person.IdPerson, objectDatabaseAction);
                }
                catch
                { }
            }
        }

        public event Action<Person, Person> CgpDBPersonAfterUpdate;
        public void DbPersonAfterUpdate(
            Person newPerson,
            Person oldPersonBeforeUpdate)
        {
            if (newPerson == null
                || oldPersonBeforeUpdate == null)
            {
                return;
            }

            if (CgpDBPersonAfterUpdate != null)
            {
                try
                {
                    CgpDBPersonAfterUpdate(
                        newPerson,
                        oldPersonBeforeUpdate);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        public event Action<object, ObjectType, ObjectDatabaseAction> CgpDBTimeZoneChanged;
        public void DbTimeZoneChanged(Beans.TimeZone timeZone, ObjectDatabaseAction objectDatabaseAction)
        {
            if (timeZone == null)
            {
                return;
            }

            if (CgpDBTimeZoneChanged != null)
            {
                try
                {
                    CgpDBTimeZoneChanged(timeZone.GetId(), timeZone.GetObjectType(), objectDatabaseAction);
                }
                catch (Exception ex)
                {
                    int a = 10;
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public event Contal.IwQuick.DVoid2Void CgpGeneralNtpSettignsChanged;
        public void DbGeneralNtpSettignsChanged()
        {
            if (CgpGeneralNtpSettignsChanged != null)
            {
                try
                {
                    CgpGeneralNtpSettignsChanged();
                }
                catch
                { }
            }
        }
    }
}
