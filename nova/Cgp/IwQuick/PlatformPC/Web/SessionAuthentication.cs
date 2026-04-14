using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.IO;

using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Web.SessionState;

using System.Diagnostics;

//using System.Runtime.InteropServices;

namespace Contal.IwQuick.Web
{
    /// <summary>
    /// class for explicit session-based authetntification management
    /// </summary>
    public class SessionAuthentication
    {
        private const string _authIdName = "auth_id";
        private const string _unauthHTMLMessage = "<h4 style='font:12pt Arial;text-color:red;font-weight:bold;margin:auto'>Unauthorized access</h4>";
        private static HttpSessionState _session = null;

        /// <summary>
        /// will register the authentication mark for the session/page
        /// </summary>
        /// <param name="i_aWebPage">the page that handles the authentication</param>
        public static void RegisterAuth(Page i_aWebPage)
        {
            Debug.Assert(null != i_aWebPage);

            lock (i_aWebPage.Session.SyncRoot)
            {
                i_aWebPage.Session[_authIdName] = GenerateAuthId(i_aWebPage);
            }
        }


        /// <summary>
        /// generates the authentication mark for current session
        /// </summary>
        /// <param name="i_aWebPage"></param>
        /// <returns></returns>
        private static string GenerateAuthId(Page i_aWebPage)
        {
            return Contal.IwQuick.Crypto.QuickHashes.GetSHA512String(i_aWebPage.Request.UserHostAddress + 
                i_aWebPage.Request.Browser.Platform +
                i_aWebPage.Request.Browser.Id);
        }

        /// <summary>
        /// checks, whether the session is authentified
        /// </summary>
        /// <param name="i_aWebPage">instance of any page, at which you're checking the authentication</param>
        /// <returns></returns>
        public static bool IsAuthorized(Page i_aWebPage)
        {
            Debug.Assert(null != i_aWebPage);

            string strSession = (string)i_aWebPage.Session[_authIdName];
            if (null == strSession)
            {
                //i_aWebPage.Response.Write("session:null<br/>");
                return false;
            }
            else
            {
                //i_aWebPage.Response.Write("session:"+strSession+"<br/>");
            }

            return (strSession == GenerateAuthId(i_aWebPage));

        }

        /// <summary>
        /// deletes the authentified mark from the session
        /// meant for logout
        /// </summary>
        /// <param name="session">the session handler</param>
        public static void UnregisterAuth(HttpSessionState session)
        {
            if (null != session)
                lock (session.SyncRoot)
                {
                    session.Remove(_authIdName);
                }
        }

        /// <summary>
        /// deletes the authentified mark from the session
        /// meant for logout
        /// </summary>
        /// <param name="i_aWebPage">the page encapsulating session handler</param>
        public static void UnregisterAuth(Page i_aWebPage)
        {
            Debug.Assert(null != i_aWebPage);

            lock (i_aWebPage.Session.SyncRoot)
            {
                i_aWebPage.Session.Remove(_authIdName);
            }

        }

        public delegate void DSession2Void(HttpSessionState session);
        public static event DSession2Void OnEnd = null;
        //public static event DSession2Void OnStart = null;

        public static bool IsOnEndFilled(DSession2Void handler)
        {
            if (null == OnEnd)
                return false;
            else
                return true;
            /*
            if (null == handler)
                return (null != OnEnd);

            return (handler == OnEnd);*/
        }


        /// <summary>
        /// intended for call in Global::Session_OnStart
        /// </summary>
        /// <param name="session">the session to encapsulate</param>
        public static void Start(HttpSessionState session)
        {
            string strSession = session.SessionID;
            _session = session;
        }

        /// <summary>
        /// intended for call in Global::Session_OnEnd
        /// </summary>
        /// <param name="session">the session to encapsulate</param>
        public static void End(HttpSessionState session)
        {
            if (null != OnEnd)
                OnEnd(session);

            UnregisterAuth(session);
        }

        /// <summary>
        /// invokes the session expiry including authentication mark unregistering
        /// </summary>
        /// <param name="i_aWebPage"></param>
        public static void InvokeEnd(Page i_aWebPage)
        {
            Debug.Assert(null != i_aWebPage);

            End(i_aWebPage.Session);

            
        }

        /// <summary>
        /// checks authentication
        /// if the session unauthorized, the default error or custom error file report is produced
        ///
        /// for error page, the UnauthFilePath option of the web.config might be used
        /// e.g.
        /// <configuration>
        ///  <appSettings>
        ///     <add key="UnauthFilePath" value="unauth.htm"/>
        ///  </appSettings>
        /// </configuration>
        /// 
        /// </summary>
        /// <param name="i_aWebPage">the page at which proceed with check</param>
        /// <param name="errorResponseFile">
        /// the optional response file in cause of unauthorized access
        /// </param>
        /// <returns>
        /// true, if session is authentified
        /// false, if session is unauthorized
        /// </returns>
        public static bool CheckAuth(Page i_aWebPage,string errorResponseFile)
        {
            Debug.Assert(null != i_aWebPage);

            if (!SessionAuthentication.IsAuthorized(i_aWebPage))
            {
                bool bUseDefault = false;

                if (null == errorResponseFile)
                {
                    try
                    {
                        errorResponseFile = ConfigurationManager.AppSettings["UnauthFilePath"];
                        if (null == errorResponseFile)
                            bUseDefault = true;
                    }
                    catch (ConfigurationErrorsException)
                    {
                        bUseDefault = true;
                    }
                }

                if (bUseDefault)
                    i_aWebPage.Response.Write(_unauthHTMLMessage);
                else
                {
                    try
                    {
                        i_aWebPage.Response.WriteFile(errorResponseFile);
                    }
                    catch (FileNotFoundException)
                    {
                        i_aWebPage.Response.Write(_unauthHTMLMessage);
                    }
                }

                i_aWebPage.Response.End();
                return false;
            }

            return true;
        }

        /// <summary>
        /// checks authentication
        /// if the session unauthorized, the default error or custom error file report is produced
        /// </summary>
        /// <param name="i_aWebPage">the page at which proceed with check</param>
        /// <returns>
        /// true, if session is authentified
        /// false, if session is unauthorized
        /// </returns>
        public static bool CheckAuth(Page i_aWebPage)
        {
            return CheckAuth(i_aWebPage, null);
        }

        public static bool CheckAuthSilent(Page i_aWebPage)
        {
            Debug.Assert(null != i_aWebPage);

            if (!IsAuthorized(i_aWebPage))
            {
                i_aWebPage.Response.End();
                return false;
            }

            return true;
        }
    }
}
