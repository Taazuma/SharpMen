using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoSharp.Auto;
using AutoSharp.Utils;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using AutoSharp.MainCore;
using AutoSharp.Spells;
using Autosharp;
// ReSharper disable ObjectCreationAsStatement

namespace AutoSharp
{
    class Program
    {
        public static GameMapId Map;
        public static Menu Config;
        public static bool Loaded;

        public static float Timer;

        public static string Activemode;

        public static string Moveto;

        public static bool Moving;

        public static Menu MenuIni;

        private static bool _loaded = false;

        public static void Init()
        {
            Map = Game.MapId;
                Cache.Load(); 
                //Game.OnUpdate += Positioning.OnUpdate;
                Autoplay.Load();
                Game.OnEnd += OnEnd;
                Game.OnUpdate += AntiShrooms2;
                //Spellbook.OnCastSpell += OnCastSpell;
                Obj_AI_Base.OnDamage += OnDamage;
            Chat.Print("AutoSharp loaded", Color.CornflowerBlue);
            Timer = Game.Time;
            Game.OnTick += Game_OnTick;
            MenuIni = MainMenu.AddMenu("DNK", "DNK");
            MenuIni.AddGroupLabel("DNK Settings");
            MenuIni.Add("DisableSpells", new CheckBox("Disable Built-in Casting Logic", false));
            MenuIni.Add("Safe", new Slider("Safe Slider (Recommended 1250)", 1250, 0, 2500));
            MenuIni.AddLabel("More Value = more defensive playstyle");
            // Initialize Bot Functions.
            Brain.Init();
            Chat.Print("Buddy Loaded !");
        }

        private static void Events_OnGameEnd(EventArgs args)
        {
            Core.DelayAction(() => Game.QuitGame(), 250);
        }

        public static void OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (sender == null) return;
            if (args.Target.NetworkId == ObjectManager.Player.NetworkId && (sender is Obj_AI_Turret || sender is Obj_AI_Minion))
            {
                Orbwalker.OrbwalkTo(
                    Heroes.Player.Position.Extend(Wizard.GetFarthestMinion().Position, 500).RandomizePosition());
            }
        }

        private static void AntiShrooms2(EventArgs args)
        {
                if (Heroes.Player.HealthPercent > 0 && Heroes.Player.CountEnemiesInRange(1800) == 0 &&
                    !Turrets.EnemyTurrets.Any(t => t.Distance(Heroes.Player) < 950) &&
                    !Minions.EnemyMinions.Any(m => m.Distance(Heroes.Player) < 950))
                {
                 
                }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (!Loaded)
            {
                if (Game.Time - Timer >= 10)
                {
                    Loaded = true;

                    // Initialize The Bot.
                    Init();
                }
            }
            else
            {
                if (Player.Instance.IsDead)
                {
                    return;
                }
                Brain.Decisions();
            }
        }

        private static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }
        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {

            if (sender.Owner.IsMe)
            {
                if (sender.Owner.IsDead)
                {
                    args.Process = false;
                    return;
                }

                }
            var target = TargetSelector.GetTarget(1000, DamageType.Magical);
            if (Heroes.Player.UnderTurret(true) && target.IsValidTarget(1000))
                {
                    args.Process = false;
                    return;
                }
        }

        private static void OnEnd(GameEndEventArgs args)
        {
            /*
            if (Config.Item("autosharp.quit").GetValue<bool>())
            {
                Thread.Sleep(30000);
                Game.QuitGame();
            }
             * */
            Thread.Sleep(30000);
            Game.QuitGame();
        }
        public static void Main(string[] args)
        {
            Game.OnUpdate += AdvancedLoading;
        }

        private static void AdvancedLoading(EventArgs args)
        {
            if (!_loaded)
            {
                if (ObjectManager.Player.Gold > 0)
                {
                    _loaded = true;
                    Core.DelayAction(Init, new Random().Next(3000, 25000));
                }
            }
        }
        public static void AntiShrooms(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender != null && sender.IsMe)
            {
                if (sender.IsDead)
                {
                    args.Process = false;
                    return;
                }
                var turret = Turrets.ClosestEnemyTurret;
                if (Heroes.Player.CountEnemiesInRange(1800) == 0 &&
                    turret.Distance(Heroes.Player) > 950 && !Minions.EnemyMinions.Any(m => m.Distance(Heroes.Player) < 950))
                {
                    args.Process = false;
                    return;
                }

                //if (args.Order == GameObjectOrder.MoveTo)
                //{
                //    if (args.TargetPosition.IsZero)
                //    {
                //        args.Process = false;
                //        return;
                //    }
                //    if (!args.TargetPosition.IsValid())
                //    {
                //        args.Process = false;
                //        return;
                //    }
                //    if (turret != null && turret.Distance(args.TargetPosition) < 950 &&
                //        turret.CountNearbyAllyMinions(950) < 3)
                //    {
                //        args.Process = false;
                //        return;
                //    }
                //}

                #region BlockAttack
                var target = TargetSelector.GetTarget(1000, DamageType.Magical);
                if (target.IsValidTarget(800))
                {

                    //if (Config.Item("onlyfarm").GetValue<bool>() && args.Target.IsValid<Obj_AI_Hero>())
                    //{
                    //    args.Process = false;
                    //    return;
                    //}

                        if (Minions.AllyMinions.Count(m => m.Distance(Heroes.Player) < 900) <
                            Minions.EnemyMinions.Count(m => m.Distance(Heroes.Player) < 900))
                        {
                            args.Process = false;
                            return;
                        }
                        //if (((myHero)args.Target).UnderTurret(true))
                        //{
                        //    args.Process = false;
                        //    return;
                        //}
                    }
                    if (Heroes.Player.UnderTurret(true))
                    {
                        args.Process = false;
                        return;
                    }
                    if (turret != null && turret.Distance(ObjectManager.Player) < 950 && turret.CountNearbyAllyMinions(950) < 3)
                    {
                        args.Process = false;
                        return;
                    }
                }

                #endregion
            }
            //if (sender != null && args.Target != null && args.Target.IsMe)
            //{
            //    if (sender is Obj_AI_Turret || sender is Obj_AI_Minion)
            //    {
            //        var minion = Wizard.GetClosestAllyMinion();
            //        if (minion != null)
            //        {
            //            Orbwalker.SetOrbwalkingPoint(
            //                Heroes.Player.Position.Extend(Wizard.GetClosestAllyMinion().Position, Heroes.Player.Distance(minion) + 100));
            //        }
            //    }
            //}
        }
    }

