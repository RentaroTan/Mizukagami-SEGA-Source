using UnityEngine;

public class DiaryCounter : MonoBehaviour
{
    
    public static DiaryCounter Instance { get; private set;}
    [SerializeField] private int diaryCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private  void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
       
            Instance = this;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(diaryCount >= 3)
        {
            Debug.Log("日記を3つ集めた");
        }
    }

    public void AddDiaryCount()
    {
        diaryCount++;
        Debug.Log("日記の数: " + diaryCount);
    }

}

