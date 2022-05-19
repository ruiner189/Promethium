using Battle;
using Battle.StatusEffects;
using Cruciball;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Extensions
{
    public static class BattleControllerExtension
    {

        private static Dictionary<String, FieldInfo> _fieldInfoDict = new Dictionary<String,FieldInfo>();
         
        private static T GetPrivateFieldValue<T>(this BattleController controller, String fieldName)
        {
            if (!_fieldInfoDict.ContainsKey(fieldName))
                _fieldInfoDict.Add(fieldName, controller.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));
            return (T)_fieldInfoDict[fieldName].GetValue(controller);
        }

        public static CruciballManager GetCruciballManager(this BattleController controller)
        {
            return controller.GetPrivateFieldValue<CruciballManager>("_cruciballManager");
        }

        public static RelicManager GetRelicManager(this BattleController controller)
        {
            return controller.GetPrivateFieldValue<RelicManager>("_relicManager");
        }

        public static PlayerStatusEffectController GetPlayerStatusEffectController(this BattleController controller)
        {
            return controller.GetPrivateFieldValue<PlayerStatusEffectController>("_playerStatusEffectController");
        }

        public static int GetBattleState(this BattleController controller)
        {
            return controller.GetPrivateFieldValue<int>("_battleState");
        }

        public static List<float> GetDamageMultipliers(this BattleController controller)
        {
            return controller.GetPrivateFieldValue<List<float>>("_damageMultipliers");
        }

        public static PlayerHealthController GetPlayerHealthController(this BattleController controller)
        {
            return controller.GetPrivateFieldValue<PlayerHealthController>("_playerHealthController");
        }
    }
}
