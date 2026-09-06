using UnityEngine;

public class ReiwriteSound : MonoBehaviour
{
    public AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Invoke(nameof(PlaySE),1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySE()
    {
        audioSource.Play();
    }
}
