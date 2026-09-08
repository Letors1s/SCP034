using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp1509;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace SCP034
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "SCP-034";
        public override string Author => "Letors1s";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(9, 14, 2);

        public static Plugin Instance;
        public SCP034 Scp034 { get; private set; }

        public List<Player> TransformPlayer = new List<Player>();

        public override void OnEnabled()
        {

            Instance = this;
            Scp034 = new SCP034();
            Scp034.Register();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {

            Scp034.Unregister();
            Scp034 = null;
            Instance = null;
            base.OnDisabled();
        }
    }

    [CustomItem(ItemType.SCP1509)]
    public class SCP034 : CustomWeapon
    {
        public override string Description { get; set; } = "<color=purple>Хорошо заточенный обсидиановый нож.</color>";
        public override string Name { get; set; } = "SCP-034";
        public override uint Id { get; set; } = 51;
        public override ItemType Type { get; set; } = ItemType.SCP1509;
        public override float Weight { get; set; } = 1f;
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,

            StaticSpawnPoints = new List<StaticSpawnPoint>
            {
                new StaticSpawnPoint
                {
                    Position = new Vector3(117.3f, 113.3f, 9.5f),
                    Chance = 100
                }
            }
        };

        public override float Damage => Plugin.Instance?.Config != null
            ? UnityEngine.Random.Range(Plugin.Instance.Config.MinDamage, Plugin.Instance.Config.MaxDamage)
            : 9f;

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Scp1509.Resurrecting += OnResurrectring;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Scp1509.Resurrecting -= OnResurrectring;
            base.UnsubscribeEvents();
        }

        protected void OnResurrectring(ResurrectingEventArgs ev)
        {
            if (ev.Player == null || ev.Player == null || !Check(ev.Player.CurrentItem))
                ev.IsAllowed = false;

            
        }

        protected override void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == null || !Check(ev.Attacker.CurrentItem))
                return;

            if (Plugin.Instance.TransformPlayer.Contains(ev.Attacker))
            {
                return;
            }

            Plugin.Instance.TransformPlayer.Add(ev.Attacker);

            ev.Amount = Damage;

            if (ev.Player.IsHuman && UnityEngine.Random.Range(0, 100) < Plugin.Instance.Config.ChanceToWork)
            {
                Timing.RunCoroutine(EffectSCP034(ev.Attacker, ev.Player, ev.Attacker.Role.Type, ev.Attacker.Health, ev.Player.Health, ev.Attacker.DisplayNickname, ev.Player.DisplayNickname));
            }

            Plugin.Instance.TransformPlayer.Remove(ev.Attacker);
            base.OnHurting(ev);
        }

        private IEnumerator<float> EffectSCP034(Player ev, Player tar, RoleTypeId evRole, float evDamage, float tarDamage, string evDName, string tarDName)
        {
            float Time = Plugin.Instance.Config.MaxTransformTime;
            float LeftTime = 0f;

            ev.ShowHint("<color=red>Вы чувствуете ОЧЕНЬ сильную боль!</color>", 8f);
            ev.Role.Set(tar.Role.Type, RoleSpawnFlags.None);
            ev.Health = tarDamage;
            ev.DisplayNickname = tarDName;

            while (Time > LeftTime)
            {
                if(tar == null || !tar.IsAlive)
                {
                    ev.Role.Set(evRole, RoleSpawnFlags.None);
                    ev.Health = evDamage;
                    ev.DisplayNickname = evDName;
                    Plugin.Instance.TransformPlayer.Remove(ev);
                    ev.ShowHint("<color=red>Вы чувствуете ОЧЕНЬ сильную боль!</color>", 8f);
                    yield break;
                }

                LeftTime += 1;
                yield return Timing.WaitForSeconds(1);
            }

            ev.Role.Set(evRole, RoleSpawnFlags.None);
            ev.Health = evDamage;
            ev.DisplayNickname = evDName;
            Plugin.Instance.TransformPlayer.Remove(ev);
            ev.ShowHint("<color=red>Вы чувствуете ОЧЕНЬ сильную боль!</color>", 8f);
        }
    }
}