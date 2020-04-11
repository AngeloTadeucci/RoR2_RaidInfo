using BepInEx;
using RoR2;
using System.Collections.Generic;

namespace RaidInfo {
    [BepInDependency("com.bepis.r2api")]
    //Change these
    [BepInPlugin("com.angelotadeucci.RaidInfo", "RaidInfo", "1.0.0")]
    public class RaidInfo : BaseUnityPlugin {
        private float totalDamageDealt = 0;
        private Dictionary<string, float> map;

        public void Awake() {
            BossGroup.onBossGroupStartServer += BossGroup_onBossGroupStartServer;
            BossGroup.onBossGroupDefeatedServer += BossGroup_onBossGroupDefeatedServer;
            On.RoR2.GlobalEventManager.ServerDamageDealt += GlobalEventManager_ServerDamageDealt;
        }

        private void GlobalEventManager_ServerDamageDealt(On.RoR2.GlobalEventManager.orig_ServerDamageDealt orig, DamageReport damageReport) {
            CharacterBody body = damageReport.attacker.GetComponent<CharacterBody>();
            if (body != null) {
                var attackerIndex = body.GetUserName();
                if (damageReport.victimIsBoss) {
                    if (map.ContainsKey(attackerIndex)) {
                        map[attackerIndex] += damageReport.damageDealt;
                    } else {
                        map.Add(attackerIndex, damageReport.damageDealt);
                    }
                    totalDamageDealt += damageReport.damageDealt;
                }
            }
            orig(damageReport);
        }

        private void BossGroup_onBossGroupDefeatedServer(BossGroup obj) {
            foreach (KeyValuePair<string, float> kvp in map) {
                float temp = (kvp.Value / totalDamageDealt) * 100;
                decimal dec = new decimal(temp);
                Message.SendColoured("Player: " + kvp.Key + " dealt " + dec + " % of boss HP.", Colours.Red);
            }
        }

        private void BossGroup_onBossGroupStartServer(BossGroup obj) {
            map = new Dictionary<string, float>();
            totalDamageDealt = 0;
        }
    }
}
