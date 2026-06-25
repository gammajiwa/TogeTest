using System.Collections;
using UnityEngine;
using Toge.Entities;

namespace Toge.Core
{
    public class CutsceneDirector : MonoBehaviour
    {
        [SerializeField] private PlayerController _player;
        [SerializeField] private Vector2 _moveDirection = Vector2.right;
        [SerializeField] private float _duration = 1.4f;
        [SerializeField] private bool _playOnStart = true;

        private IEnumerator Start()
        {
            if (_playOnStart) yield return Play();
        }

        public IEnumerator Play()
        {
            if (_player == null) yield break;

            _player.SetScriptedMove(_moveDirection.normalized);
            yield return new WaitForSeconds(_duration);
            _player.SetScriptedMove(null);
        }
    }
}
