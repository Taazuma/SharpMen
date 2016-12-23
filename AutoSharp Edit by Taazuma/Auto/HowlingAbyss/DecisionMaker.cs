using System;
using System.Linq;
using AutoSharp.Auto.SummonersRift;
using AutoSharp.Utils;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;


namespace AutoSharp.Auto.HowlingAbyss
{
    internal static class DecisionMaker
    {
        private static int _lastUpdate = 0;
        public static Vector3 IntroducedPos;    
        
        public static void OnTick(EventArgs args)
        {
            if (Environment.TickCount - _lastUpdate < 150) return;
            _lastUpdate = Environment.TickCount;

            var player = Heroes.Player;
            if (Heroes.Player.InFountain())
            {
                Shopping.Shop();
                Wizard.AntiAfk();
            }        
        }

    }
}
