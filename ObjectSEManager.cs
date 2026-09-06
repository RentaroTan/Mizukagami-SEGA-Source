using UnityEngine;

public class ObjectSEManager : MonoBehaviour
{
    public static ObjectSEManager Instance { get; private set; }
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupDiarySE;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
       
            Instance = this;
    }
     public void PlayDiaryPickupSE()
    {
        if(audioSource != null && pickupDiarySE != null)
        {
            audioSource.PlayOneShot(pickupDiarySE);
        }
    }
}
