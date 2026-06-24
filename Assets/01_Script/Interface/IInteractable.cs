namespace Toge.Interfaces
{
    /// <summary>Implemented by overworld objects the player can interact with (Space).</summary>
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        void Interact();
    }
}
