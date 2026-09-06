using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BookTrigger : MonoBehaviour
{
    [Header("フラグ")]
    private bool canReadBook = false;
    private bool hasReadBook = false;
   
    [Header("本のRenderer")]
   [SerializeField] private Renderer bookRenderer;
    private Material bookMaterial;

    [Header("Emission関連")]
    [SerializeField] private Color emissionColor = Color.white;
    [SerializeField] private float startEmissionPower = 0f; 
    [SerializeField] private float maxEmissionpower = 100f;

    [Header("ホワイトアウト関連")]
    [SerializeField] private float timeDuration =2f;
    [SerializeField] private float whiteOutDruration =1f;
    [SerializeField] private CanvasGroup whiteOutCanvasGroup;

    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bookMaterial = bookRenderer.material;
        bookMaterial.EnableKeyword("_Emission");
        if(whiteOutCanvasGroup != null)
        {
            whiteOutCanvasGroup.alpha = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(canReadBook && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("本を読んだ");
            StartCoroutine(WhiteOutSeaquence());
        }
    }

   public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            // プレイヤーがトリガーに入ったときの処理
            Debug.Log("本が読める");
            canReadBook = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
             Debug.Log("本が読めなくなった");
        canReadBook = false;
        }
       
    }

   private IEnumerator WhiteOutSeaquence()
    {
        float timer = 0;
        while(timer <= timeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / timeDuration;
            float easedT = t*t;//後半徐々に早く、大きくなる処理のための変数
            float emissionPower = Mathf.Lerp(startEmissionPower, maxEmissionpower, easedT);
            SetEmission(emissionPower);
            yield return null;
        }

       timer = 0f;
        while(timer <= whiteOutDruration)
        {
            timer += Time.deltaTime;
            float t = timer / whiteOutDruration;
            if(whiteOutCanvasGroup != null)
            {
                whiteOutCanvasGroup.alpha = t;
            }
            yield return null;
        }

        if(whiteOutCanvasGroup != null)
        {
            whiteOutCanvasGroup.alpha = 1f;
        }

        Debug.Log("ホワイトアウト完了");
        
        SceneManager.LoadScene("WaveScene2");

    }



     private void SetEmission(float power)
    {
        Color finalColor = emissionColor * power;
        bookMaterial.SetColor("_EmissionColor",finalColor);
    }

}   

