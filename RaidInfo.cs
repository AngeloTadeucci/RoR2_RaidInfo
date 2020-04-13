using BepInEx;
using RoR2;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RaidInfo {
    [BepInDependency("com.bepis.r2api")]
    //Change these
    [BepInPlugin("com.angelotadeucci.RaidInfo", "RaidInfo", "1.0.0")]
    public class RaidInfo : BaseUnityPlugin {
        private float totalDamageDealt = 0;
        private Dictionary<string, float> map;
        private string bossName;
        private Stopwatch ttk = new Stopwatch();
        
        public void Awake() {
            BossGroup.onBossGroupStartServer += BossGroup_onBossGroupStartServer;
            BossGroup.onBossGroupDefeatedServer += BossGroup_onBossGroupDefeatedServer;
            On.RoR2.GlobalEventManager.ServerDamageDealt += GlobalEventManager_ServerDamageDealt;
        }

        private void GlobalEventManager_ServerDamageDealt(On.RoR2.GlobalEventManager.orig_ServerDamageDealt orig, DamageReport damageReport) {
            orig(damageReport);
            if (damageReport.attacker == null || damageReport.victimBody == null) {
                return;
            }
            CharacterBody attacker = damageReport.attackerBody;
            CharacterBody victim = damageReport.victimBody;
            if (damageReport.victimIsBoss) {
                var attackerIndex = attacker.GetUserName();
                if (map.ContainsKey(attackerIndex)) {
                    map[attackerIndex] += damageReport.damageDealt;
                } else {
                    map.Add(attackerIndex, damageReport.damageDealt);
                }
                bossName = victim.GetUserName();
                totalDamageDealt += damageReport.damageDealt;
            }
        }

        private void BossGroup_onBossGroupDefeatedServer(BossGroup obj) {
            ttk.Stop();
            foreach (KeyValuePair<string, float> kvp in map) {
                float temp = (kvp.Value / totalDamageDealt) * 100;
                decimal dec = new decimal(temp);
                Message.SendColoured(kvp.Key + " dealt " + dec + " % of " + bossName + " HP.", Colours.Red);
            }
            TimeSpan ts = ttk.Elapsed;
            string elapsedTime = String.Format("{0:00} minutes and {1:00} seconds",
            ts.Minutes, ts.Seconds);
            Message.SendColoured("Killed " + bossName + " in: "+ elapsedTime + " .", Colours.Red);
        }

        private void BossGroup_onBossGroupStartServer(BossGroup obj) {
            map = new Dictionary<string, float>();
            totalDamageDealt = 0;
            bossName = null;
            ttk.Start();  
        }
    }
}
