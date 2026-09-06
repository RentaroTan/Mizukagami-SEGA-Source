using UnityEngine;

public interface IInteractable
{
    void Interact();
    void OnFocusEnter();
    void OnFocusExit();
    string GetPromptText();
}
