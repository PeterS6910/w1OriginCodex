using System;
using System.Collections.Generic;

using NetFwTypeLib;

namespace Contal.Cgp.Server
{
    public class FirewallController
    {
        public static void AddAllowUdpPortRule(int udpPort)
        {
            var manager = 
                (INetFwMgr)
                    Activator.CreateInstance(
                        Type.GetTypeFromProgID(
                            "HNetCfg.FwMgr", 
                            false));

            if (!manager.LocalPolicy.CurrentProfile.FirewallEnabled)
                return;

            var firewallInboundRule = 
                (INetFwRule)
                    Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

            firewallInboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            firewallInboundRule.Description = "Allow time protocol";
            firewallInboundRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            firewallInboundRule.Enabled = true;
            firewallInboundRule.InterfaceTypes = "All";
            firewallInboundRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP;
            firewallInboundRule.LocalPorts = udpPort.ToString();
            firewallInboundRule.Name = "Time protocol";

            var firewallOutboundRule = 
                (INetFwRule)
                    Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

            firewallOutboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            firewallOutboundRule.Description = "Allow time protocol";
            firewallOutboundRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
            firewallOutboundRule.Enabled = true;
            firewallOutboundRule.InterfaceTypes = "All";
            firewallOutboundRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP;
            firewallOutboundRule.LocalPorts = udpPort.ToString();
            firewallOutboundRule.Name = "Time protocol";

            var firewallPolicy = 
                (INetFwPolicy2)
                    Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            if (firewallPolicy.Rules == null)
                return;

            var rulesToRemove = new List<INetFwRule>();

            foreach (INetFwRule rule in firewallPolicy.Rules)
                if (rule.Protocol == (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP &&
                    rule.LocalPorts == udpPort.ToString())
                    rulesToRemove.Add(rule);

            foreach (var rule in rulesToRemove)
                firewallPolicy.Rules.Remove(rule.Name);

            firewallPolicy.Rules.Add(firewallInboundRule);
            firewallPolicy.Rules.Add(firewallOutboundRule);
        }

        public static void SetApplicationRule(string applicationPath)
        {
            try
            {
                var manager =
                    (INetFwMgr)
                        Activator.CreateInstance(
                            Type.GetTypeFromProgID(
                                "HNetCfg.FwMgr",
                                false));

                if (manager.LocalPolicy.CurrentProfile.FirewallEnabled)
                {
                    var firewallRule =
                        (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

                    firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                    firewallRule.Description = "Allow Contal Nova Server";
                    firewallRule.ApplicationName = applicationPath;
                    firewallRule.Enabled = true;
                    firewallRule.InterfaceTypes = "All";
                    firewallRule.Name = "Contal Nova Server";

                    var firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                        Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

                    if (firewallPolicy.Rules != null)
                    {
                        var rulesToRemove = new List<INetFwRule>();

                        foreach (INetFwRule rule in firewallPolicy.Rules)
                            if (rule.ApplicationName == applicationPath)
                                rulesToRemove.Add(rule);

                        foreach (var rule in rulesToRemove)
                            firewallPolicy.Rules.Remove(rule.Name);

                        firewallPolicy.Rules.Add(firewallRule);
                    }
                }
            }
            catch
            {
            }
        }

        public static void RemoveApplicationRule(string applicationPath)
        {
            try
            {
                var manager =
                    (INetFwMgr)
                        Activator.CreateInstance(
                            Type.GetTypeFromProgID(
                                "HNetCfg.FwMgr",
                                false));

                if (!manager.LocalPolicy.CurrentProfile.FirewallEnabled)
                    return;

                var firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                    Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

                if (firewallPolicy.Rules == null)
                    return;

                var rulesToRemove = new List<INetFwRule>();

                foreach (INetFwRule rule in firewallPolicy.Rules)
                    if (rule.ApplicationName == applicationPath)
                        rulesToRemove.Add(rule);

                foreach (var rule in rulesToRemove)
                    firewallPolicy.Rules.Remove(rule.Name);
            }
            catch
            {
                
            }
        }
    }
}