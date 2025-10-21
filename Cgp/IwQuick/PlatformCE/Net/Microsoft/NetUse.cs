using System;

namespace Contal.IwQuick.Net.Microsoft
{
    public class NetUse
    {
		private bool _persistent = false;
		/// <summary>
		/// Option to reconnect drive after log off / reboot ...
		/// </summary>
		public bool Persistent{
            get { return (_persistent); }
            set { _persistent = value; }
		}

        private bool _saveCredentials = false;
		/// <summary>
		/// Option to save credentials are reconnection...
		/// </summary>
		public bool SaveCredentials{
            get { return (_saveCredentials); }
            set
            {
                _saveCredentials = value;
            }
		}

		private bool _force = false;
		/// <summary>
		/// Option to force connection if drive is already mapped...
		/// or force disconnection if network path is not responding...
		/// </summary>
		public bool Force{
			get{return(_force);}
			set{_force=value;}
		}

		private bool _promptForCredentials = false;
		/// <summary>
		/// Option to prompt for user credintals when mapping a drive
		/// </summary>
		public bool PromptForCredentials{
            get { return (_promptForCredentials); }
            set { _promptForCredentials = value; }
		}

		private string _drive = "x:";
		/// <summary>
		/// Drive to be used in mapping / unmapping...
		/// </summary>
		public string LocalDrive{
			get{return(_drive);}
            set
            {
                _drive = CheckDrive(value);
            }
		}
        private string _uNC = null;
		/// <summary>
		/// Share address to map drive to.
		/// </summary>
		public string UNC{
			get{return(_uNC);}
			set{_uNC=value;}
		}

        /*
        // Map network drive
        public void MapDrive(string _username, string psPassword)
        {
            //create struct data
            DllMpr.NetworkResource aNetworkResource = new DllMpr.NetworkResource();
            aNetworkResource._scope = DllMpr.RESOURCE_GLOBALNET;
            aNetworkResource._type = DllMpr.RESOURCETYPE_DISK;
            aNetworkResource._displayType = DllMpr.RESOURCEDISPLAYTYPE_SHARE;
            aNetworkResource._tsage = DllMpr.RESOURCEUSAGE_CONNECTABLE;
            aNetworkResource.m_szRemoteName = _uNC;
            aNetworkResource.m_szLocalName = _drive;
            //prepare params
            int iFlags = 0;

            if (_promptForCredentials) { iFlags += DllMpr.CONNECT_INTERACTIVE + DllMpr.CONNECT_PROMPT; }

            if (_saveCredentials) { iFlags += DllMpr.CONNECT_CMD_SAVECRED; }           

            if (_username == "") { _username = null; }

            if (psPassword == "") { psPassword = null; }

            if (_persistent) { iFlags += DllMpr.CONNECT_UPDATE_PROFILE; }

            

            //if force, unmap ready for new connection
            if (_force)
            {
                try { UnmapDrive(); }
                catch { }
            }

            //call and return
            int i = DllMpr.WNetAddConnection2A(ref aNetworkResource, psPassword, _username, iFlags);
            if (i > 0) { throw new System.ComponentModel.Win32Exception(i); }
        }*/

		/// <summary>
		/// Map network drive
		/// </summary>
        public void MapDrive()
        {
            MapDrive(null, null);
        }

		/// <summary>
		/// Map network drive (using supplied Password)
		/// </summary>
        public void MapDrive(string Password)
        {
            MapDrive(null, Password);
        }

        public static string CheckDrive(string drive)
        {
            Validator.CheckNullString(drive);

            char cFirst = drive[0];
            if ((cFirst >= 'a' && cFirst <= 'z') ||
                (cFirst >= 'A' && cFirst <= 'Z'))
                return cFirst + ":";
            else
                throw new FormatException("Invalid drive specification");
        }

		/// <summary>
		/// Map network drive (using supplied Username and Password)
		/// </summary>
        /// 


        public static void UnmapDrive(string drive, bool i_bForce, bool i_bPersistent)
        {
            drive = CheckDrive(drive);

            int iFlags = 0;
            if (i_bPersistent)
                iFlags += DllMpr.CONNECT_UPDATE_PROFILE;

            int iExitCode = DllMpr.WNetCancelConnection2A(drive, iFlags, Convert.ToInt32(i_bForce));
            if (iExitCode > 0)
                throw new System.ComponentModel.Win32Exception(iExitCode);
        }

        public void UnmapDrive(bool i_bForce)
        {
            UnmapDrive(_drive, i_bForce, _persistent);
        }

        public void UnmapDrive()
        {
            UnmapDrive(_drive, _force, _persistent);
        }

		/// <summary>
		/// Check / restore persistent network drive
		/// </summary>
        public void RestoreDrives()
        {
            //call restore and return
            int i = DllMpr.WNetRestoreConnectionW(0, null);

            if (i > 0)
            {
                throw new System.ComponentModel.Win32Exception(i);
            }
        }
	
		
    }
}
