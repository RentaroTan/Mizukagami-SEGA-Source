using UnityEngine;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Raycast関連")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 6f;
    [SerializeField] private LayerMask interactableLayer;
    private IInteractable currentTarget;
    [Header("Ui")]
    [SerializeField]private TextMeshProUGUI promptText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if(promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInteractable();   
        if(currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            
            currentTarget.Interact();
        }
    }

   private void CheckInteractable()
{
    if (playerCamera == null)
    {
        Debug.LogError("playerCamera が入っていない");
        return;
    }

    Debug.Log("CheckInteractable は動いている");

    Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

    Debug.DrawRay
    (
        playerCamera.transform.position,
        playerCamera.transform.forward * interactDistance,
        Color.red
    );

    // まずLayerを無視してRaycastする
    if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
    {
        Debug.Log("Layer無視なら当たった: " + hit.collider.name + " / Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
    }
    else
    {
        Debug.Log("Layer無視でも何にも当たっていない");
    }

    // いつものLayer指定ありRaycast
    if (Physics.Raycast(ray, out RaycastHit layerHit, interactDistance, interactableLayer))
    {
        Debug.Log("Layer指定ありで当たった: " + layerHit.collider.name);

        IInteractable interactable = layerHit.collider.GetComponent<IInteractable>();

        if (interactable != null)
        {
            Debug.Log("IInteractableを発見");

            if (interactable != currentTarget)
            {
                ClearCurrentTarget();

                currentTarget = interactable;
                currentTarget.OnFocusEnter();
            }

            if (promptText != null)
            {
                promptText.gameObject.SetActive(true);
                promptText.text = currentTarget.GetPromptText();
            }

            return;
        }
        else
        {
            Debug.Log("当たったけどIInteractableがない");
        }
    }
    else
    {
        Debug.Log("Layer指定ありでは何にも当たっていない");
    }

    ClearCurrentTarget();
}

    private void ClearCurrentTarget()
    {
        if(currentTarget != null)
        {
            currentTarget.OnFocusExit();
            currentTarget = null;
        }
        if(promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }



}
