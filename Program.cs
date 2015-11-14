using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;
using SharpDX;


namespace PerfectHecarim
{
    class Program
    {
        public static Spell.Active Q;
        public static Spell.Active W;
        public static Spell.Active E;
        public static Spell.Skillshot R;
        public static Menu Menu, SkillMenu, FarmingMenu, MiscMenu, DrawMenu, HarassMenu, ComboMenu, SmiteMenu, UpdateMenu;
        static Item Healthpot;
        static Item Manapot;
        static Item CrystalFlask;
        static Item CorruptingPotion;
        static Item RefillablePotion;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Hecarim")
                return;


            Bootstrap.Init(null);

            Healthpot = new Item(2003, 0);
            Manapot = new Item(2004, 0);
            CrystalFlask = new Item(2041, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);

            uint level = (uint)Player.Instance.Level;
            Q = new Spell.Active(SpellSlot.Q, 350);
            W = new Spell.Active(SpellSlot.W, 525);
            E = new Spell.Active(SpellSlot.E, 450);
            R = new Spell.Skillshot(SpellSlot.R, 1000, SkillShotType.Linear, 250, 800, 200);

            Menu = MainMenu.AddMenu("Perfect Hecarim", "perfecthecarim");
            Menu.AddLabel("Perrrrrrrrrfect Ass");
            Menu.AddSeparator();

            
            ComboMenu = Menu.AddSubMenu("Combo Settings","ComboSettings");            
            ComboMenu.AddLabel("Combo Settings");
            ComboMenu.Add("QCombo", new CheckBox("Use Q"));
            ComboMenu.Add("WCombo", new CheckBox("Use W"));
            ComboMenu.Add("ECombo", new CheckBox("Use E"));
            ComboMenu.Add("RCombo", new CheckBox("Use R"));
            ComboMenu.Add("rCount", new Slider("R Count ", 3, 1, 5));
            ComboMenu.Add("useTiamat", new CheckBox("Use Items"));

            HarassMenu = Menu.AddSubMenu("Harass Settings", "HarassSettings");
            HarassMenu.AddLabel("Harass Settings");
            HarassMenu.Add("QHarass", new CheckBox("Use Q"));

            FarmingMenu = Menu.AddSubMenu("Lane Clear", "FarmSettings");

            FarmingMenu.AddLabel("Lane Clear");
            FarmingMenu.Add("QLaneClear", new CheckBox("Use Q LaneClear"));
            FarmingMenu.Add("QlaneclearMana", new Slider("Mana < %", 45, 0, 100));
            FarmingMenu.Add("WLaneClear", new CheckBox("Use W LaneClear"));
            FarmingMenu.Add("WlaneclearMana", new Slider("Mana < %", 45, 0, 100));
            FarmingMenu.AddLabel("I'm working on the Jungle Clear.");
            FarmingMenu.AddLabel("Ending Soon!");

            FarmingMenu.AddLabel("Last Hit Settings");
            FarmingMenu.Add("Qlasthit", new CheckBox("Use Q LastHit"));
            FarmingMenu.Add("QlasthitMana", new Slider("Mana < %", 45, 0, 100));

            SmiteMenu = Menu.AddSubMenu("Smite Usage", "SmiteUsage");
            SmiteMenu.AddLabel("Smite Usage");
            SmiteMenu.Add("Use Smite?", new CheckBox("Use Smite"));
            SmiteMenu.Add("Red?", new CheckBox("Red"));
            SmiteMenu.Add("Blue?", new CheckBox("Blue"));
            SmiteMenu.Add("Dragon?", new CheckBox("Dragon"));
            SmiteMenu.Add("Baron?", new CheckBox("Baron"));
            

            MiscMenu = Menu.AddSubMenu("More Settings", "Misc");

            MiscMenu.AddLabel("Auto");
            MiscMenu.Add("Auto Ignite", new CheckBox("Auto Ignite"));
            MiscMenu.Add("autoE", new CheckBox("Use Auto E to Flee/Escape"));         
            MiscMenu.Add("autoR", new CheckBox("Use Auto R in Dangerous Spell"));
            MiscMenu.AddSeparator();
            MiscMenu.AddLabel("Items");
            MiscMenu.AddSeparator();
            MiscMenu.AddLabel("BOTRK,Bilgewater Cutlass Settings");
            MiscMenu.Add("botrkHP", new Slider("My HP < %", 60, 0, 100));
            MiscMenu.Add("botrkenemyHP", new Slider("Enemy HP < %", 60, 0, 100));

            MiscMenu.AddLabel("KillSteal");
            MiscMenu.Add("Qkill", new CheckBox("Use Q KillSteal"));
            MiscMenu.Add("Ekill", new CheckBox("Use E KillSteal"));

            MiscMenu.AddLabel("Activator");
            MiscMenu.Add("useHP", new CheckBox("Use Health Potion"));           
            MiscMenu.Add("useHPV", new Slider("HP < %", 45, 0, 100));
            MiscMenu.Add("useMana", new CheckBox("Use Mana Potion"));
            MiscMenu.Add("useManaV", new Slider("Mana < %", 45, 0, 100));
            MiscMenu.Add("useCrystal", new CheckBox("Use New Potions"));
            MiscMenu.Add("useCrystalHPV", new Slider("HP < %", 45, 0, 100));
            MiscMenu.Add("useCrystalManaV", new Slider("Mana < %", 45, 0, 100));

            DrawMenu = Menu.AddSubMenu("Draw Settings", "Drawings");
            DrawMenu.Add("drawAA", new CheckBox("Draw AA Range"));
            DrawMenu.Add("drawQ", new CheckBox("Draw Q Range"));
            DrawMenu.Add("drawW", new CheckBox("Draw W Range"));
            DrawMenu.Add("drawR", new CheckBox("Draw R Range"));

            UpdateMenu = Menu.AddSubMenu("Last Update Logs", "Updates");
            UpdateMenu.AddLabel("V0.0.2");
            UpdateMenu.AddLabel("-LaneClear Fixed");
            UpdateMenu.AddLabel("-ComboMenu R Count Added");
            UpdateMenu.AddLabel("-New Items Added");

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;

            Chat.Print("Perrrrrrrrrfect Addon", System.Drawing.Color.Red);
        }


        private static void Game_OnTick(EventArgs args)
        {
            var HPpot = MiscMenu["useHP"].Cast<CheckBox>().CurrentValue;
            var Mpot = MiscMenu["useMana"].Cast<CheckBox>().CurrentValue;
            var Crystal = MiscMenu["useCrystal"].Cast<CheckBox>().CurrentValue;
            var HPv = MiscMenu["useHPv"].Cast<Slider>().CurrentValue;
            var Manav = MiscMenu["useManav"].Cast<Slider>().CurrentValue;
            var CrystalHPv = MiscMenu["useCrystalHPv"].Cast<Slider>().CurrentValue;
            var CrystalManav = MiscMenu["useCrystalManav"].Cast<Slider>().CurrentValue;
            var useItem = ComboMenu["useTiamat"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var igntarget = TargetSelector.GetTarget(600, DamageType.True);

            if (HPpot && Player.Instance.HealthPercent < HPv)
            {
                if (Item.HasItem(Healthpot.Id) && Item.CanUseItem(Healthpot.Id) && !Player.HasBuff("RegenerationPotion"))
                {
                    Healthpot.Cast();
                }
            }
            
            if (Crystal && Player.Instance.HealthPercent < CrystalHPv || Crystal && Player.Instance.ManaPercent < CrystalManav)
            {
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id) && !Player.HasBuff("RegenerationPotion") && !Player.HasBuff("FlaskOfCrystalWater") && !Player.HasBuff("ItemCrystalFlask"))
                {
                    CorruptingPotion.Cast();
                }
               
            }

            if (Crystal && Player.Instance.HealthPercent < CrystalHPv || Crystal && Player.Instance.ManaPercent < CrystalManav)
            {
                if (Item.HasItem(RefillablePotion.Id) && Item.CanUseItem(RefillablePotion.Id) && !Player.HasBuff("RegenerationPotion") && !Player.HasBuff("FlaskOfCrystalWater") && !Player.HasBuff("ItemCrystalFlask"))
                {
                    RefillablePotion.Cast();
                }

            }

            if (useItem && target.IsValidTarget(400) && !target.IsDead && !target.IsZombie && target.HealthPercent < 100)
            {
                HandleItems();
            }


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
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }
            KillSteal();
            

        }
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var useQ = ComboMenu["QCombo"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["WCombo"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["ECombo"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["RCombo"].Cast<CheckBox>().CurrentValue;
            var useItem = ComboMenu["useTiamat"].Cast<CheckBox>().CurrentValue;
            var rCount = ComboMenu["rCount"].Cast<Slider>().CurrentValue;

            if (W.IsReady() && useW && target.IsValidTarget(W.Range) && !target.IsDead && !target.IsZombie)
            {
                W.Cast();
            }
            if (E.IsReady() && useE && target.IsValidTarget(E.Range) && !target.IsDead && !target.IsZombie)
            {
                E.Cast();
            }
            if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && !target.IsDead && !target.IsZombie)
            {
                Q.Cast();
            }
            foreach (AIHeroClient enemie in EntityManager.Heroes.Enemies)
            {
                if (R.IsReady() && useR && EntityManager.Heroes.Enemies.Where(enemy => enemy != _Player && enemy.Distance(_Player) <= 1000).Count() > rCount && !target.IsDead && !target.IsZombie)
                {
                    R.Cast(enemie);
                }
                
            }
            if (useItem && target.IsValidTarget(400) && !target.IsDead && !target.IsZombie)
            {
                HandleItems();
            }

        }
        private static void KillSteal()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var useQ = MiscMenu["Qkill"].Cast<CheckBox>().CurrentValue;
            var useE = MiscMenu["Ekill"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range) && !target.IsZombie && target.Health <= _Player.GetSpellDamage(target, SpellSlot.Q))
            {
                Q.Cast();
            }
            if (E.IsReady() && useE && target.IsValidTarget(E.Range) && !target.IsZombie && target.Health <= _Player.GetSpellDamage(target, SpellSlot.E))
            {
                E.Cast();
            }
        }

        internal static void HandleItems()
        {
            var botrktarget = TargetSelector.GetTarget(550, DamageType.Physical);
            var useItem = ComboMenu["useTiamat"].Cast<CheckBox>().CurrentValue;
            var useBotrkHP = MiscMenu["botrkHP"].Cast<Slider>().CurrentValue;
            var useBotrkEnemyHP = MiscMenu["botrkenemyHP"].Cast<Slider>().CurrentValue;
            //HYDRA
            if (useItem && Item.HasItem(3077) && Item.CanUseItem(3077))
                Item.UseItem(3077);

            //TİAMAT
            if (useItem && Item.HasItem(3074) && Item.CanUseItem(3074))
                Item.UseItem(3074);

            //NEW ITEM
            if (useItem && Item.HasItem(3748) && Item.CanUseItem(3748))
                Item.UseItem(3748);

            //BİLGEWATER CUTLASS
            if (useItem && Item.HasItem(3144) && Item.CanUseItem(3144) && botrktarget.HealthPercent <= useBotrkEnemyHP && _Player.HealthPercent <= useBotrkHP)
                Item.UseItem(3144, botrktarget);

            //BOTRK
            if (useItem && Item.HasItem(3153) && Item.CanUseItem(3153) && botrktarget.HealthPercent <= useBotrkEnemyHP && _Player.HealthPercent <= useBotrkHP)
                Item.UseItem(3153, botrktarget);

            //YOUMU
            if (useItem && Item.HasItem(3142) && Item.CanUseItem(3142))
                Item.UseItem(3142);
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var useQ = HarassMenu["QHarass"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range) && !target.IsDead && !target.IsZombie)
            {
                Q.Cast();
            }

        }
        private static void LaneClear()
        {
            var useQ = FarmingMenu["QLaneClear"].Cast<CheckBox>().CurrentValue;
            var useW = FarmingMenu["WLaneClear"].Cast<CheckBox>().CurrentValue;
            var Qmana = FarmingMenu["QlaneclearMana"].Cast<Slider>().CurrentValue;
            var Wmana = FarmingMenu["WlaneclearMana"].Cast<Slider>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && Player.Instance.ManaPercent > Qmana && minion.Health > _Player.GetSpellDamage(minion, SpellSlot.Q) && minions.Count() >= 2)
                {
                    Q.Cast();
                }
                if (useW && W.IsReady() && Player.Instance.ManaPercent > Wmana && minion.IsValidTarget(W.Range) && Player.Instance.HealthPercent < 50 && minions.Count() >= 2)
                {
                    W.Cast();
                }
            }
        }
        private static void LastHit()
        {
            var useQ = FarmingMenu["Qlasthit"].Cast<CheckBox>().CurrentValue;
            var mana = FarmingMenu["QlasthitMana"].Cast<Slider>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && Player.Instance.ManaPercent > mana && minion.Health < _Player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    Q.Cast();
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = Q.Range }.Draw(_Player.Position);
            }
            if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = W.Range }.Draw(_Player.Position);
            }
            if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = R.Range }.Draw(_Player.Position);
            }
        }
    }
}