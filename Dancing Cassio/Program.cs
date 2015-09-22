using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Microsoft.Win32;


namespace Dancing_Cassio
{
    class Program
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E;
        public static Spell.Skillshot R;
        public static Menu CassioMenu, ComboMenu, HarassMenu, LaneMenu;


        static void Main(string[] args)
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
            R = new Spell.Skillshot(SpellSlot.R, 825, SkillShotType.Cone, 600, 0, (int)(80 * Math.PI / 180));

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
            Game.OnTick += Game_OnTick;

        }
        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Harass)
            {
                Harass();
            }
        }

        public static float RDamage(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 150, 250, 350 }[R.Level - 1] + 0.50 * ObjectManager.Player.FlatMagicDamageMod));
        }

        private static void Combo()
        {
            var useQ = ComboMenu["combo.q"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["combo.w"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["combo.e"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["combo.r"].Cast<CheckBox>().CurrentValue;

            if (useQ && Q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(o => o.IsValidTarget(Q.Range) && !o.IsDead))
                {
                    if (Q.GetPrediction(target).HitChance >= HitChance.High)
                    {
                        Q.Cast(target);
                    }
                }
            }
            if (useW && W.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(o => o.IsValidTarget(W.Range) && !o.IsDead))
                {
                    if (W.GetPrediction(target).HitChance >= HitChance.High)
                    {
                        W.Cast(target);
                    }
                }
            }
            if (useE && E.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(o => o.IsValidTarget(E.Range) && !o.IsDead))
                    && o.HasBuffOfType(BuffType.Poison)))
                {
                    E.Cast(target);
                }
            }
            if (useR && R.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(o => o.IsValidTarget(R.Range) && !o.IsDead))
                    && RDamage(o) > o.Health))
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
                foreach (var target in HeroManager.Enemies.Where(o => o.IsValidTarget(Q.Range) && !o.IsDead &&))
                {
                    if (Q.GetPrediction(target).HitChance >= HitChance.High)
                    {
                        Q.Cast(target);
                    }
                }
            }
            if (useE && E.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(o => o.IsValidTarget(E.Range) && !o.IsDead &&))
                    && o.HasBuffOfType(BuffType.Poison)))
                {
                    E.Cast(target);
                }
            }

        }
    }
}
