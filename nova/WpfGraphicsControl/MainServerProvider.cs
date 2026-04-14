using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.Client;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.WpfGraphicsControl;

namespace Cgp.NCAS.WpfGraphicsControl
{
    static class MainServerProvider
    {
        public static IEnumerable<IShortObject> GetObjects(ObjectType objectType)
        {
            try
            {
                Exception error = null;
                IEnumerable<IShortObject> list = null;

                switch (objectType)
                {
                    case ObjectType.Scene:
                        list =
                            GraphicsScene.MainServerProvider.Scenes.ShortSelectByCriteria(null, out error)
                                .Cast<IShortObject>();
                        break;

                    case ObjectType.CardReader:
                        list =
                            GraphicsScene.MainServerProvider.CardReaders.ShortSelectByCriteria(null, out error)
                                .Cast<IShortObject>();
                        break;

                    case ObjectType.AlarmArea:
                        list =
                            GraphicsScene.MainServerProvider.AlarmAreas.ShortSelectByCriteria(null, out error)
                                .Cast<IShortObject>();
                        break;

                    case ObjectType.Output:
                        list =
                            GraphicsScene.MainServerProvider.Outputs.ShortSelectByCriteria(null, out error)
                                .Cast<IShortObject>();
                        break;

                    case ObjectType.DoorEnvironment:
                        list = GraphicsScene.MainServerProvider.DoorEnvironments.GetShortSelectByCriteria(null,
                            out error).Cast<IShortObject>();
                        break;

                    case ObjectType.Input:
                        list = GraphicsScene.MainServerProvider.Inputs.ShortSelectByCriteria(null,
                            out error).Cast<IShortObject>();
                        break;

                    case ObjectType.CCU:
                        list = GraphicsScene.MainServerProvider.CCUs.ShortSelectByCriteria(null,
                            out error).Cast<IShortObject>();
                        break;

                    case ObjectType.DCU:
                        list = GraphicsScene.MainServerProvider.DCUs.SvDcuSelectByCriteria(null,
                            out error).Cast<IShortObject>();
                        break;
                }

                if (error != null)
                    throw (error);

                return list;
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
                return null;
            }
        }
    }
}
