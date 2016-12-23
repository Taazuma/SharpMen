using System.Linq;
using AutoSharp.Utils;
using EloBuddy;
using EloBuddy.SDK;

namespace AutoSharp.Auto.HowlingAbyss
{
    internal static class Decisions
    {
        internal static bool HealUp()
        {
            if (Heroes.Player.IsDead)
            {
                Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.None;
                return true;
            }

            if (Heroes.Player.HealthPercent >= 75) return false;

            var closestEnemyBuff = HealingBuffs.EnemyBuffs.FirstOrDefault(eb => eb.IsVisible && eb.IsValid && eb.Position.Distance(Heroes.Player.Position) < 800 && (eb.Position.CountEnemiesInRange(600) == 0 || eb.Position.CountEnemiesInRange(600) < eb.Position.CountAlliesInRange(600)));
            var closestAllyBuff = HealingBuffs.AllyBuffs.FirstOrDefault(ab => ab.IsVisible && ab.IsValid);


            //BUFF EXISTANCE CHECKS;
            if ((closestAllyBuff == null && closestEnemyBuff == null)) return false;

            //BECAUSE WE CHECKED THAT BUFFS CAN'T BE BOTH NULL; IF ONE OF THEM IS NULL IT MEANS THE OTHER ISN'T.
            // ReSharper disable once PossibleNullReferenceException
            var buffPos = closestEnemyBuff != null ? closestEnemyBuff.Position.Randomize(0, 15) : closestAllyBuff.Position.Randomize(0,15);

            if (Heroes.Player.Position.Distance(buffPos) <= 800 && (Heroes.Player.CountEnemiesInRange(800) == 0 || Heroes.Player.CountEnemiesInRange(800) < Heroes.Player.Position.CountAlliesInRange(800)))
            {
                DecisionMaker.Goto(buffPos);
                return true;
            }

            //stay in fight if you can't instantly gratify yourself and u don't really need the buff
            if (Heroes.Player.HealthPercent >= 45 && Heroes.Player.CountEnemiesInRange(900) <= Player.Instance.Position.CountAlliesInRange(900) && Heroes.Player.Distance(buffPos) > 1000) return false;

            //IF BUFFPOS IS VECTOR ZERO OR NOT VALID SOMETHING MUST HAVE GONE WRONG
            if (!buffPos.IsValid()) return false;

            //MOVE TO BUFFPOS
            DecisionMaker.Goto(buffPos);

            //STOP EVERYTHING ELSE TO DO THIS
            return true;
        }



    }
}
