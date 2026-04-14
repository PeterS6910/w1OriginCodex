using CrSceneFrameworkCF;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism
{
    partial class CardReaderMechanism
    {
        private interface ICategorizedSensorsMenuSceneGroup : IInstanceProvider<ICategorizedSensorsMenuSceneGroup>
        {
            SceneContext SceneContext
            {
                get;
            }

            ICard Card
            {
                get;
            }

            DB.AlarmAreaSetUnsetSecurityLevel AlarmAreaSetUnsetSecurityLevel
            {
                get;
            }

            CrSceneGroupExitRoute DefaultGroupExitRoute
            {
                get;
            }

            bool IsPinRequired(
                DB.AlarmArea alarmArea,
                bool isUnset);

            void NotifyPinEntered();
        }
    }
}