using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        private List<Vector2Int> _listOfNonReachableTargets = new List<Vector2Int>();
        
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////
            
            if (GetTemperature() >= overheatTemperature)
                return;
            
            for (int i = 0; i <= GetTemperature(); i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
            IncreaseTemperature();
            
            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            if (_listOfNonReachableTargets.Any())
            {
                var nextTarget = unit.Pos.CalcNextStepTowards(_listOfNonReachableTargets[_listOfNonReachableTargets.Count - 1]);
                return nextTarget;
            }
            return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            Vector2Int nearestPositionOfEnemy = Vector2Int.zero;
            List<Vector2Int> resultList = new List<Vector2Int>();
            List<Vector2Int> listOfReachableTargets = GetReachableTargets();
            List<Vector2Int> listOfAllTargets = GetAllTargets().ToList();

            if (listOfAllTargets.Any())
            {
                Vector2Int target = GetNearesTarget(listOfAllTargets);
                if (listOfReachableTargets.Contains(target))
                    resultList.Add(target);
                else
                    _listOfNonReachableTargets.Add(target);
            }
            else
            {
                resultList.Add(runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId]);
            }
            return listOfAllTargets;
            ///////////////////////////////////////
        }
        private Vector2Int GetNearesTarget (List<Vector2Int> enemyUnits)
        {
            Vector2Int nearestPositionOfEnemy = Vector2Int.zero;
            float minDistanceToEnemy = float.MaxValue;

            foreach (Vector2Int enemyPosition in enemyUnits)
            {
                float distanceToCurrentEnemy = DistanceToOwnBase(enemyPosition);
                if (distanceToCurrentEnemy < minDistanceToEnemy)
                {
                    minDistanceToEnemy = distanceToCurrentEnemy;
                    nearestPositionOfEnemy = enemyPosition;
                }
            }

            return nearestPositionOfEnemy;
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}