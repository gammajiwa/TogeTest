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
        [SerializeField] private Fungus.Flowchart _introDialog;
        [SerializeField] private string _introBlock = "Start";

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

            if (_introDialog != null && _introDialog.HasBlock(_introBlock))
            {
                _introDialog.ExecuteBlock(_introBlock);
                yield return TrackDialog(_introDialog);
            }
        }

        private IEnumerator TrackDialog(Fungus.Flowchart flowchart)
        {
            Toge.Interactive.DialogState.IsActive = true;
            yield return null;
            while (flowchart != null && flowchart.HasExecutingBlocks()) yield return null;
            Toge.Interactive.DialogState.IsActive = false;
        }
    }
}
