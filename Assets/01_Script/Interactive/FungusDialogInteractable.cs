using UnityEngine;
using Fungus;
using Toge.Interfaces;

namespace Toge.Interactive
{
    public class FungusDialogInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _prompt = "Talk";
        [SerializeField] private Flowchart _flowchart;
        [SerializeField] private string _blockName = "Start";

        public string InteractionPrompt => _prompt;

        public void Interact()
        {
            if (_flowchart == null) return;
            if (_flowchart.HasExecutingBlocks()) return;
            if (_flowchart.HasBlock(_blockName)) _flowchart.ExecuteBlock(_blockName);
        }
    }
}
