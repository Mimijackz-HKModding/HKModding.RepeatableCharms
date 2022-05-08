using HutongGames.PlayMaker;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace RepeatableCharms
{
    public class RepeatableCharms : Mod
    {
        internal static RepeatableCharms Instance;
        //public override List<ValueTuple<string, string>> GetPreloadNames()
        //{
        //    return new List<ValueTuple<string, string>>
        //    {
        //        new ValueTuple<string, string>("White_Palace_18", "White Palace Fly")
        //    };
        //}

        public RepeatableCharms() : base("Repeatable Charms")
        {
            Instance = this;
        }

        public override string GetVersion() => "beta 1.0";

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            On.PlayMakerFSM.OnEnable += OnFSM;
            ModHooks.CharmUpdateHook += OnCharm;
            On.PlayerData.CountCharms += NotchCalculation;
            
            Instance = this;

            Log("Initialized");
        }

        private void OnCharm(PlayerData data, HeroController controller)
        {
            //Grubsong
            if (GetCharmAmount(3) > 0)
            {
                data.equippedCharm_3 = true;
                controller.GRUB_SOUL_MP = GetCharmAmount(3) * 15;
            }
            //Quickslash
            if (GetCharmAmount(32) > 0)
            {
                data.equippedCharm_32 = true;
                controller.ATTACK_COOLDOWN_TIME_CH = Mathf.Pow((float)25/41, GetCharmAmount(32)) * 0.41f;
                controller.ATTACK_DURATION_CH = Mathf.Pow((float)28/35, GetCharmAmount(32)) * 0.35f;
            }
            //Dashmaster
            if (GetCharmAmount(31) > 0)
            {
                data.equippedCharm_31 = true;
                controller.DASH_COOLDOWN_CH = Mathf.Pow((float)4/6, GetCharmAmount(31)) * 0.6f;
            }
            //Nailmaster's Glory
            if (GetCharmAmount(26) > 0)
            {
                data.equippedCharm_26 = true;
                controller.NAIL_CHARGE_TIME_CHARM = Mathf.Pow((float)75/135, GetCharmAmount(26)) * 1.35f;
            }
            //Sprintmaster
            if (GetCharmAmount(37) > 0)
            {
                data.equippedCharm_37 = true;
                controller.RUN_SPEED_CH = GetCharmAmount(37) * 1.7f + 8.3f;
                controller.RUN_SPEED_CH_COMBO = GetCharmAmount(37) * (1.7f + (GetCharmAmount(31) * 1.5f)) + 8.3f; //Dashmaster-Sprintmaster combo
            }
            
        }

        //Apparently the vanilla notch calculation calculates repeating charms as 0 cost so i just changed that calculation
        private void NotchCalculation(On.PlayerData.orig_CountCharms orig, PlayerData self)
        {
            orig(self);
            int slots = 0;
            foreach(int charm in self.equippedCharms)
            {
                
                string charmData = "charmCost_" + charm;
                int cost = self.GetInt(charmData);
                if (cost > 0)
                {
                    slots += cost;
                }
            }
            self.charmSlotsFilled = slots;
        }

        
        //Making the inventory accept more charms
        private void OnFSM(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);
            if (self.FsmName == "charm_show_if_collected")
            {
                HutongGames.PlayMaker.Actions.PlayerDataBoolTest playerDataBoolTest = self.FsmStates[1].Actions[3] as HutongGames.PlayMaker.Actions.PlayerDataBoolTest;
                playerDataBoolTest.isTrue = null;
                foreach(HutongGames.PlayMaker.FsmStateAction action in self.FsmStates[3].Actions)
                {
                    action.Enabled = false;
                }
            }else if (self.FsmName == "UI Charms")
            {
                HutongGames.PlayMaker.Actions.PlayerDataBoolTest playerDataBoolTest = self.FsmStates[23].Actions[2] as HutongGames.PlayMaker.Actions.PlayerDataBoolTest;
                playerDataBoolTest.isTrue = playerDataBoolTest.isFalse;
            }
        }
        public int GetCharmAmount(int id)
        {
            int slots = 0;
            foreach(int charm in PlayerData.instance.equippedCharms)
            {
                if (charm == id) slots++;
            }
            return slots;
        }
    }
}