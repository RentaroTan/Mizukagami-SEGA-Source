using UnityEngine;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("効果音")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip lockedDoorSE;

    public void Interact()
    {
        // Eキーで触ったとき
        if (audioSource != null && lockedDoorSE != null)
        {
            audioSource.PlayOneShot(lockedDoorSE);
        }
    }

    public void OnFocusEnter()
    {
        // 今は何もしなくてOK
    }

    public void OnFocusExit()
    {
        // 今は何もしなくてOK
    }

    public string GetPromptText()
    {
        return "E：調べる";
    }
}