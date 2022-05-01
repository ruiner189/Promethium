using Battle;
using Battle.StatusEffects;
using Cruciball;
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

        private static FieldInfo GetFieldInfo(Object obj, String field)
        {
            return obj.GetType().GetField(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        }

        private static FieldInfo _cruciballManager;
        public static CruciballManager GetCruciballManager(this BattleController controller)
        {
            if (_cruciballManager == null)
                _cruciballManager = GetFieldInfo(controller, "_cruciballManager");

           return (CruciballManager)_cruciballManager.GetValue(controller);
        }

        private static FieldInfo _playerStatusEffectController;
        public static PlayerStatusEffectController GetPlayerStatusEffectController(this BattleController controller)
        {
            if (_playerStatusEffectController == null)
                _playerStatusEffectController = GetFieldInfo(controller, "_playerStatusEffectController");
            return (PlayerStatusEffectController) _playerStatusEffectController.GetValue(controller);
        }

        private static FieldInfo _battleState;
        public static int GetBattleState(this BattleController controller)
        {
            if (_battleState == null)
                _battleState = GetFieldInfo(controller, "_battleState");
            return (int)_battleState.GetValue(controller);
        }

        private static FieldInfo _damageMultipliers;
        public static List<float> GetBattleMultipliers(this BattleController controller)
        {
            if (_damageMultipliers == null)
                _damageMultipliers = GetFieldInfo(controller, "_damageMultipliers");
            return (List<float>)_damageMultipliers.GetValue(controller);
        }

        private static FieldInfo _playerHealthController;
        public static PlayerHealthController GetPlayerHealthController(this BattleController controller)
        {
            if (_playerHealthController == null)
                _playerHealthController = GetFieldInfo(controller, "_playerHealthController");
            return (PlayerHealthController)_playerHealthController.GetValue(controller);
        }
    }


}
