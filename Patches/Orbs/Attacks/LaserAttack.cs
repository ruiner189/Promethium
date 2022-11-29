using Battle;
using Battle.Attacks;
using Battle.Enemies;
using Battle.StatusEffects;
using Cruciball;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EnemyManager;

namespace Promethium.Patches.Orbs.Attacks
{

    public class LaserAttack : Attack
    {
        [SerializeField]
        public GameObject _laserPrefab;
        [SerializeField]
        public GameObject _criticalLaserPrefab;

        private LaserBehavior _laser;
        private LaserBehavior _criticalLaser;

        private Enemy _target;
        private float _hitDamage;

        private bool _lastAttackCrit;
        
        public void Awake()
        {
            if (locDescStrings == null) locDescStrings = new string[0];
        }

        public void SetAttackPrefabs(GameObject laser, GameObject criticalLaser = null)
        {
            _laserPrefab = laser;
            _criticalLaserPrefab = criticalLaser ? criticalLaser : laser;
        }

        public override void Initialize(Vector2 playerPosition, AttackManager attackManager, RelicManager relicManager, DeckManager deckManager, CruciballManager cruciballManager, PlayerStatusEffectController statusController)
        {
            base.Initialize(playerPosition, attackManager, relicManager, deckManager, cruciballManager, statusController);
            _laser = GameObject.Instantiate(_laserPrefab).GetComponent<LaserBehavior>();
            _criticalLaser = GameObject.Instantiate(_criticalLaserPrefab).GetComponent<LaserBehavior>();

            _laser.gameObject.SetActive(false);
            _criticalLaser.gameObject.SetActive(false);
        }

        public override void Fire(AttackManager attackManager, Enemy target, float[] dmgValues, float dmgMult, int dmgBonus, int critCount = 0)
        {
            _target = target;
            _hitDamage = GetDamage(attackManager, dmgValues, dmgMult, dmgBonus, critCount, false);
            SpawnLaser(_target, critCount);
        }

        public void SpawnLaser(Enemy target, int critCount)
        {
            _lastAttackCrit = (critCount > 0);
            LaserBehavior laserBehavior = (_lastAttackCrit && _hitDamage > 0f) ? _criticalLaser : _laser;
            if (target != null && target.enemyTypes.HasFlag(Enemy.EnemyType.Flying))
            {
                laserBehavior.gameObject.transform.position = new Vector2(-_playerPosition.x - 3.7f, 8.1f);
            } 
            else
            {
                laserBehavior.gameObject.transform.position = new Vector2(-_playerPosition.x - 3.7f, _playerPosition.y);
            }

            laserBehavior.gameObject.SetActive(true);
            laserBehavior.OnLaserHit += HandleLaserHit;
            laserBehavior.OnLaserEnd += HandleLaserEnded;

        }


        public void HandleLaserHit()
        {

            if(_target == null || !_target.enemyTypes.HasFlag(Enemy.EnemyType.Flying))
            {
                // ground
                foreach (EnemySlot slot in _attackManager._enemyManager._groundSlots)
                {
                    if(slot.Enemy != null)
                    {
                        OnEnemyHit(slot.Enemy);
                    }
                }

                foreach(EnemySlot slot in _attackManager._enemyManager._stationarySlots)
                {
                    if(slot.Enemy != null && !slot.Enemy.enemyTypes.HasFlag(Enemy.EnemyType.Flying))
                    {
                        OnEnemyHit(slot.Enemy);
                    }
                }
            } else
            {
                // air
                foreach (EnemySlot slot in _attackManager._enemyManager._flyingSlots)
                {
                    if (slot.Enemy != null)
                    {
                        OnEnemyHit(slot.Enemy);
                    }
                }

                foreach (EnemySlot slot in _attackManager._enemyManager._stationarySlots)
                {
                    if (slot.Enemy != null && slot.Enemy.enemyTypes.HasFlag(Enemy.EnemyType.Flying))
                    {
                        OnEnemyHit(slot.Enemy);
                    }
                }
            }
        }

        public void OnEnemyHit(Enemy enemy)
        {
            if(enemy is ShieldEnemy shieldEnemy)
            {
                Enemy shield = shieldEnemy.shield;
                float shieldMod = GetDamageMod(shield.spellAttackDamageMod);
                shield.DamageWithTypeMods(Mathf.Ceil(_hitDamage / 3f), shieldMod);
            }

            float damageMod = GetDamageMod(enemy.spellAttackDamageMod);
            enemy.DamageWithTypeMods(Mathf.Ceil(_hitDamage / 3f), damageMod);
            foreach(StatusEffect effect in GetStatusEffects())
            {
                if(effect.EffectType != StatusEffectType.None)
                {
                    enemy.ApplyStatusEffect(effect);
                }
            }
        }

        public void HandleLaserEnded()
        {
            _attackManager.AttackAnimationEnded();
        }
    }

}
