using UnityEngine;
using UnityEngine.InputSystem;

namespace Toge.Data
{
    [CreateAssetMenu(menuName = "Toge/Input Map", fileName = "InputMap")]
    public class InputMapSO : ScriptableObject
    {
        [Header("Movement (Arrow keys always work too)")]
        public Key moveUp = Key.W;
        public Key moveDown = Key.S;
        public Key moveLeft = Key.A;
        public Key moveRight = Key.D;
        public Key run = Key.LeftShift;

        [Header("Actions")]
        public Key jump = Key.Space;
        public Key interact = Key.E;
        public Key attack = Key.F;
    }
}
