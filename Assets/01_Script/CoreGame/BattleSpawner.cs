using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Spine.Unity;
using Toge.Core;
using Toge.Data;
using Toge.Variables;

namespace Toge.Battle
{
    public class BattleSpawner : MonoBehaviour
    {
        [SerializeField] private BattleManager _battle;
        [SerializeField] private PartyConfigSO _party;
        [SerializeField] private EncounterSO _encounter;
        [SerializeField] private EncounterAnchorSO _activeEncounter;
        [SerializeField] private RuntimeDeckSO _runtimeDeck;
        [SerializeField] private TMP_FontAsset _font;
        [SerializeField] private string _bootScene = "Boot";
        [SerializeField] private float _playerX = -3.5f;
        [SerializeField] private float _enemyX = 3.5f;
        [SerializeField] private float _spacing = 2.5f;

        private readonly List<GameObject> _spawnedObjects = new();

        private void OnEnable()
        {
            EncounterSO encounter = ResolveEncounter();
            if (encounter == null) return;
            StartCoroutine(SpawnThenBegin(encounter));
        }

        private EncounterSO ResolveEncounter()
        {
            if (_activeEncounter != null && _activeEncounter.IsSet) return _activeEncounter.Value;

            bool standalone = !SceneManager.GetSceneByName(_bootScene).isLoaded;
            return standalone ? _encounter : null;
        }

        private IEnumerator SpawnThenBegin(EncounterSO encounter)
        {
            ClearSpawned();

            List<BattleUnit> players = SpawnSide(PartyData(), _playerX, true, BattleTeam.Player);
            List<BattleUnit> enemies = SpawnSide(EnemyData(encounter), _enemyX, false, BattleTeam.Enemy);

            BattleUnit player = players.Count > 0 ? players[0] : null;
            List<CardSO> deck = ResolveDeck(player);

            yield return null;

            if (_battle != null && player != null) _battle.Begin(player, enemies, deck);
        }

        private void ClearSpawned()
        {
            foreach (GameObject go in _spawnedObjects)
                if (go != null) Destroy(go);
            _spawnedObjects.Clear();
        }

        private List<EntityDataSO> PartyData()
        {
            var list = new List<EntityDataSO>();
            if (_party != null)
                foreach (PlayerDataSO member in _party.members)
                    if (member != null) list.Add(member);
            return list;
        }

        private List<EntityDataSO> EnemyData(EncounterSO encounter)
        {
            var list = new List<EntityDataSO>();
            if (encounter != null)
                foreach (EnemyDataSO enemy in encounter.enemies)
                    if (enemy != null) list.Add(enemy);
            return list;
        }

        private List<CardSO> ResolveDeck(BattleUnit player)
        {
            if (_runtimeDeck != null && _runtimeDeck.IsInitialized && _runtimeDeck.Cards.Count > 0)
                return new List<CardSO>(_runtimeDeck.Cards);

            if (player != null && player.Data is PlayerDataSO data)
                return new List<CardSO>(data.cards);

            return new List<CardSO>();
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
            SceneManager.MoveGameObjectToScene(go, gameObject.scene);
            go.name = team + "_" + data.displayName;
            _spawnedObjects.Add(go);

            if (!string.IsNullOrEmpty(data.skin)) sa.initialSkinName = data.skin;
            if (!string.IsNullOrEmpty(data.idleAnimation)) sa.AnimationName = data.idleAnimation;
            sa.loop = true;
            sa.Initialize(true);

            SpineAnimResolver.Resolve(sa, data.skin, out string idleAnim, out string attackAnim, out string deathAnim);
            sa.AnimationState.SetAnimation(0, idleAnim, true);

            float s = Mathf.Max(0.0001f, data.visualScale);
            float scaleX = faceRight == data.facesRight ? s : -s;
            go.transform.localScale = new Vector3(scaleX, s, s);
            go.transform.position = pos;

            var renderer = go.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.bounds.size.y > 0.01f)
                go.transform.position += Vector3.up * -renderer.bounds.min.y;

            go.AddComponent<CharacterBillboard>();

            var unit = go.AddComponent<BattleUnit>();
            unit.Init(data, team);

            int knockDir = team == BattleTeam.Player ? -1 : 1;
            go.AddComponent<UnitHitFx>().Init(knockDir, idleAnim, attackAnim, deathAnim);

            float headHeight = renderer != null && renderer.bounds.size.y > 0.01f
                ? renderer.bounds.size.y + 0.4f
                : 2.6f;
            go.AddComponent<UnitHealthBar>().Init(unit, headHeight, _font);

            return unit;
        }
    }
}
