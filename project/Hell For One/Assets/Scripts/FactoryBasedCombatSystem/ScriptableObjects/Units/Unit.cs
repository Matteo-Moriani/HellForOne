using System.Linq;
using UnityEngine;

namespace FactoryBasedCombatSystem.ScriptableObjects.Units
{
    [CreateAssetMenu(menuName = ("CombatSystem/Unit"),fileName = "Unit", order = 1)]
    public class Unit : ScriptableObject
    {
        [SerializeField] private Unit[] allies;
        [SerializeField] private Unit[] enemies;

        public bool CanHit(Unit target) => !allies.Contains(target) && enemies.Contains(target);
        public bool CanBeHit(Unit attacker) => !allies.Contains(attacker) && enemies.Contains(attacker);
    }
}