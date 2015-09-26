using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace Dancing_Cassio
{
    internal class Program
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E;
        public static Spell.Skillshot R;
        public static Menu CassioMenu, ComboMenu, HarassMenu, LaneClearMenu;

        private static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            TargetSelector.Init();
            Bootstrap.Init(null);

            Q = new Spell.Skillshot(SpellSlot.Q, 750, SkillShotType.Circular, 750, 0, 40);
            W = new Spell.Skillshot(SpellSlot.W, 850, SkillShotType.Circular, 500, 0, 90);
            E = new Spell.Targeted(SpellSlot.E, 700);
            R = new Spell.Skillshot(SpellSlot.R, 825, SkillShotType.Cone, 600, 0, (int) (80*Math.PI/180));

            CassioMenu = MainMenu.AddMenu("Dancing Cassio", "cassio.enemy");
            CassioMenu.AddGroupLabel("Dancing Cassio");
            CassioMenu.AddSeparator();
            CassioMenu.AddLabel("Improved By Sctub // whoami");

            ComboMenu = CassioMenu.AddSubMenu("Combo Settings", "Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.AddSeparator();
            ComboMenu.Add("combo.q", new CheckBox("Use Q"));
            ComboMenu.Add("combo.w", new CheckBox("Use W"));
            ComboMenu.Add("combo.e", new CheckBox("Use E"));
            ComboMenu.Add("combo.r", new CheckBox("Use R"));

            HarassMenu = CassioMenu.AddSubMenu("Harass Settings", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.AddSeparator();
            HarassMenu.Add("harass.q", new CheckBox("Use Q"));
            HarassMenu.Add("harass.e", new CheckBox("Use E"));

            LaneClearMenu = CassioMenu.AddSubMenu("Laneclear Settings", "Laneclear");
            LaneClearMenu.AddGroupLabel("Laneclear Settings");
            LaneClearMenu.AddSeparator();
            LaneClearMenu.Add("laneclear.q", new CheckBox("Use Q"));
            LaneClearMenu.Add("laneclear.w", new CheckBox("Use W"));
            LaneClearMenu.Add("laneclear.e", new CheckBox("Use E"));

            Game.OnTick += Game_OnTick;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                laneClear();
            }

        }


        private static void Combo()
        {
            var useQ = ComboMenu["combo.q"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["combo.w"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["combo.e"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["combo.r"].Cast<CheckBox>().CurrentValue;
            if (useQ && Q.IsReady())
            {
                foreach (
                    var target in HeroManager.Enemies.Where(o => o.IsValidTarget(Q.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (Q.GetPrediction(target).HitChance >= HitChance.High)
                    {
                        Q.Cast(target);
                    }
                }
            }

            if (useW && W.IsReady())
            {
                foreach (
                    var target in HeroManager.Enemies.Where(o => o.IsValidTarget(W.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (W.GetPrediction(target).HitChance >= HitChance.High)
                    {
                        W.Cast(target);
                    }
                }
            }

            if (useE && E.IsReady())
            {
                foreach (
                    var target in HeroManager.Enemies.Where(o => o.IsValidTarget(E.Range) && !o.IsDead && !o.IsZombie
                                                                 && o.HasBuffOfType(BuffType.Poison)))
                {
                    E.Cast(target);
                }
            }

            if (useR && R.IsReady())
            {
                foreach (
                    var target in HeroManager.Enemies.Where(o => o.IsValidTarget(R.Range) && !o.IsDead && !o.IsZombie
                                                                 && o.IsFacing(ObjectManager.Player) ||
                                                                 DamageLibrary.GetSpellDamage(ObjectManager.Player, o,
                                                                     SpellSlot.R) > o.Health))
                {
                    R.Cast(target.Position);
                }
            }
        }

        private static void Harass()
        {
            var useQ = HarassMenu["harass.q"].Cast<CheckBox>().CurrentValue;
            var useE = HarassMenu["harass.e"].Cast<CheckBox>().CurrentValue;
            if (useQ && Q.IsReady())
            {
                foreach (
                    var target in HeroManager.Enemies.Where(o => o.IsValidTarget(Q.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (Q.GetPrediction(target).HitChance >= HitChance.High)
                    {
                        Q.Cast(target);
                    }
                }
            }

            if (useE && E.IsReady())
            {
                foreach (
                    var target in HeroManager.Enemies.Where(o => o.IsValidTarget(E.Range) && !o.IsDead && !o.IsZombie
                                                                 && o.HasBuffOfType(BuffType.Poison)))
                {
                    E.Cast(target);
                }
            }
        }

        private static void laneClear()
        {
            var useQ = LaneClearMenu["laneclear.q"].Cast<CheckBox>().CurrentValue;
            var useW = LaneClearMenu["laneclear.w"].Cast<CheckBox>().CurrentValue;
            var useE = LaneClearMenu["laneclear.e"].Cast<CheckBox>().CurrentValue;
            var minions = EntityManager.GetLaneMinions();
            if (!minions.Any()) return;
            if (useQ && Q.IsReady())
            {
                var pred = Prediction.Position.PredictCircularMissileAoe(minions.ToArray(), 750,
                    40, 750, 0);
                if (pred.Any())
                {
                    var pred2 = pred.OrderByDescending(a => a.CollisionObjects.Count()).FirstOrDefault();
                    if (pred2 != null && pred2.CollisionObjects.Count() >= 2)
                    {
                        Q.Cast(pred2.CastPosition);
                    }
                }

            }
            if (useW && W.IsReady())
            {
                var pred = Prediction.Position.PredictCircularMissileAoe(minions.ToArray(), 850, 500, 0, 90);
                if (pred.Any())
                {
                    var pred2 = pred.OrderByDescending(a => a.CollisionObjects.Count()).FirstOrDefault();
                    if (pred2 != null && pred2.CollisionObjects.Count() >= 3)
                    {
                        W.Cast(pred2.CastPosition);
                    } 
                }

            }
            if (useE && E.IsReady())
            {
                foreach (
                    var target in
                        minions
                            .Where(o => o.IsValidTarget(Q.Range) && !o.IsDead && o.IsMinion && o.IsEnemy
                                        && o.HasBuffOfType(BuffType.Poison)))
                {
                    E.Cast(target);
                }
            }
        }
    }

}

