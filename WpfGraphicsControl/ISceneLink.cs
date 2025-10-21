using System;

namespace Cgp.NCAS.WpfGraphicsControl
{
    interface ISceneLink
    {
        void SetSceneId(Guid idScene);
        Guid GetSceneId();
    }
}
