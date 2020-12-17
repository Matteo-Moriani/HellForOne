using System;
using System.Collections.Generic;
using FactoryBasedCombatSystem.Interfaces;
using ReincarnationSystem;
using UnityEngine;

namespace HordeSystem
{
    public class HordeImp : MonoBehaviour, IHitPointsObserver, IReincarnationObserver
    {
        private void Start() => HordeManager.Instance.RegisterImp(this);
        public void OnZeroHp() => HordeManager.Instance.UnregisterImp(this);
        public void Reincarnate() => HordeManager.Instance.UnregisterImp(this);
    }
}