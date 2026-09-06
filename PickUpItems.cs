using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickUpItems : MonoBehaviour, IInteractable
{
    public enum ItemType
    {
        Diary,
        Key,
        Other
    }

    [Header("アイテムの種類")]
    [SerializeField] private ItemType itemType;

    [Header("アイテムの名前")]
    [SerializeField] private string itemName;

    [Header("日記の場合の番号")]
    [SerializeField] private int diaryNumber;

    [Header("アウトライン")]
    [SerializeField] private Behaviour outline;

    

    public void OnFocusEnter()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }

        Debug.Log(itemName + "を拾える");
    }

    public void OnFocusExit()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void Interact()
    {
        if (itemType == ItemType.Diary)
        {
            PickUpDiary();
        }
        else if (itemType == ItemType.Key)
        {
            Debug.Log("鍵を拾った");
        }
        else
        {
            Debug.Log(itemName + "を拾った");
        }

        Destroy(gameObject);
    }

    private void PickUpDiary()
    {
        Debug.Log("日記を拾った");

        ObjectSEManager.Instance.PlayDiaryPickupSE();
        //DiaryCounter.Instance.AddDiaryCount(); //いったん使わない機能なのでコメント化

        DiaryManagerInWaveScene.Instance.CollectDiary(diaryNumber);
    }

    public string GetPromptText()
    {
        return itemName;
    }
}