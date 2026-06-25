using UnityEngine;
using Toge.Interfaces;

namespace Toge.Interactive
{
    public class MessageInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _prompt = "Talk";
        [SerializeField] [TextArea] private string _message = "Hello, traveler!";

        public string InteractionPrompt => _prompt;

        public void Interact() => Debug.Log($"[Interact] {name}: {_message}");
    }
}
