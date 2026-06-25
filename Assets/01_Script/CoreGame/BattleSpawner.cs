using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Toge.Core;
using Toge.Data;

namespace Toge.Battle
{
    public class BattleSpawner : MonoBehaviour
    {
        [SerializeField] private BattleManager _battle;
        [SerializeField] private BattleUI _ui;
        [SerializeField] private PartyConfigSO _party;
        [SerializeField] private EncounterSO _encounter;
        [SerializeField] private float _playerX = -3.5f;
        [SerializeField] private float _enemyX = 3.5f;
        [SerializeField] private float _spacing = 2.5f;

        private void Start()
        {
            List<BattleUnit> players = SpawnSide(PartyData(), _playerX, true, BattleTeam.Player);
            List<BattleUnit> enemies = SpawnSide(EncounterData(), _enemyX, false, BattleTeam.Enemy);

            if (_ui != null && players.Count > 0 && enemies.Count > 0)
                _ui.Bind(players[0], enemies[0]);

            if (_battle != null) _battle.Begin(players, enemies);
        }

        private List<EntityDataSO> PartyData()
        {
            var list = new List<EntityDataSO>();
            if (_party != null)
                foreach (PlayerDataSO member in _party.members)
                    if (member != null) list.Add(member);
            return list;
        }

        private List<EntityDataSO> EncounterData()
        {
            var list = new List<EntityDataSO>();
            if (_encounter != null)
                foreach (EnemyDataSO enemy in _encounter.enemies)
                    if (enemy != null) list.Add(enemy);
            return list;
        }

        private List<BattleUnit> SpawnSide(List<EntityDataSO> datas, float x, bool faceRight, BattleTeam team)
        {
            var units = new List<BattleUnit>();
            float startZ = -(datas.Count - 1) * _spacing * 0.5f;

            for (int i = 0; i < datas.Count; i++)
            {
                Vector3 pos = new Vector3(x, 0f, startZ + i * _spacing);
                units.Add(Spawn(datas[i], team, pos, faceRight));
            }
            return units;
        }

        private BattleUnit Spawn(EntityDataSO data, BattleTeam team, Vector3 pos, bool faceRight)
        {
            SkeletonAnimation sa = SkeletonAnimation.NewSkeletonAnimationGameObject(data.skeleton);
            GameObject go = sa.gameObject;
            go.name = team + "_" + data.displayName;

            if (!string.IsNullOrEmpty(data.skin)) sa.initialSkinName = data.skin;
            if (!string.IsNullOrEmpty(data.idleAnimation)) sa.AnimationName = data.idleAnimation;
            sa.loop = true;
            sa.Initialize(true);

            float s = Mathf.Max(0.0001f, data.visualScale);
            go.transform.localScale = new Vector3(faceRight ? s : -s, s, s);
            go.transform.position = pos;

            var renderer = go.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.bounds.size.y > 0.01f)
                go.transform.position += Vector3.up * -renderer.bounds.min.y;

            go.AddComponent<CharacterBillboard>();

            var unit = go.AddComponent<BattleUnit>();
            unit.Init(data, team);
            return unit;
        }
    }
}
