using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.IO;
using NetFwTypeLib;


namespace Contal.Cgp.Client
{
    [RunInstaller(true)]
    public partial class ClientInstaller : Installer
    {
        public ClientInstaller()
        {
            InitializeComponent();
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);

            try
            {
                InstallContext cont = this.Context;
                FileInfo file = new FileInfo(cont.Parameters["assemblypath"]);
                string pathCgp = file.Directory.ToString();
                pathCgp += @"\Contal Nova Client.exe";

                Type NetFwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
                INetFwMgr manager = (INetFwMgr)Activator.CreateInstance(NetFwMgrType);

                if (manager.LocalPolicy.CurrentProfile.FirewallEnabled)
                {
                    INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                    firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                    firewallRule.Description = "Allow Contal Nova Client";
                    firewallRule.ApplicationName = pathCgp;
                    firewallRule.Enabled = true;
                    firewallRule.InterfaceTypes = "All";
                    firewallRule.Name = "Contal Nova Client";

                    INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                        Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                    if (firewallPolicy != null)
                    {
                        List<INetFwRule> rulesToRemov = new List<INetFwRule>();
                        if (firewallPolicy.Rules != null && firewallPolicy.Rules.Count > 0)
                        {
                            foreach (INetFwRule rule in firewallPolicy.Rules)
                            {
                                if (rule.ApplicationName == pathCgp)
                                {
                                    rulesToRemov.Add(rule);
                                }
                            }

                            if (rulesToRemov != null && rulesToRemov.Count > 0)
                            {
                                foreach (INetFwRule rule in rulesToRemov)
                                {
                                    firewallPolicy.Rules.Remove(rule.Name);
                                }
                            }
                        }

                        firewallPolicy.Rules.Add(firewallRule);
                    }

                }
            }
            catch { }
        }

        protected override void OnAfterUninstall(IDictionary savedState)
        {
            base.OnAfterUninstall(savedState);

            try
            {
                InstallContext cont = this.Context;
                FileInfo file = new FileInfo(cont.Parameters["assemblypath"]);
                string pathCgp = file.Directory.ToString();
                pathCgp += @"\Contal Nova Client.exe";

                Type NetFwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
                INetFwMgr manager = (INetFwMgr)Activator.CreateInstance(NetFwMgrType);

                if (manager.LocalPolicy.CurrentProfile.FirewallEnabled)
                {
                    INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                        Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                    if (firewallPolicy != null)
                    {
                        List<INetFwRule> rulesToRemov = new List<INetFwRule>();
                        if (firewallPolicy.Rules != null && firewallPolicy.Rules.Count > 0)
                        {
                            foreach (INetFwRule rule in firewallPolicy.Rules)
                            {
                                if (rule.ApplicationName == pathCgp)
                                {
                                    rulesToRemov.Add(rule);
                                }
                            }

                            if (rulesToRemov != null && rulesToRemov.Count > 0)
                            {
                                foreach (INetFwRule rule in rulesToRemov)
                                {
                                    firewallPolicy.Rules.Remove(rule.Name);
                                }
                            }
                        }
                    }

                }
            }
            catch { }
        }
    }
}
