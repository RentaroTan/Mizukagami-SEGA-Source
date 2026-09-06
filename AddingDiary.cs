using UnityEngine;

public class AddingDiary : MonoBehaviour
{
    [Header("指定枚数に到達したときの自動取得")]
    [SerializeField]
    private int diaryNumber;

    [SerializeField]
    private int triggerCollectedDiaryNumber;

    [Header("シーン開始時の自動取得")]
    [SerializeField]
    private bool collectOnSceneLoad;

    [SerializeField]
    private int[] collectDiaryNumbersOnSceneLoad;

    private bool hasCollectedDiary;

    private void Start()
    {
        if (!collectOnSceneLoad)
        {
            return;
        }

        foreach (int diaryNumberOnSceneLoad in collectDiaryNumbersOnSceneLoad)
        {
            DiaryManagerInWaveScene.Instance.CollectDiary(
                diaryNumberOnSceneLoad
            );
        }
    }

    private void Update()
    {
        if (hasCollectedDiary)
        {
            return;
        }

        if (DiaryManagerInWaveScene.Instance.CollectedDiaryCount
            == triggerCollectedDiaryNumber)
        {
            DiaryManagerInWaveScene.Instance.CollectDiary(diaryNumber);

            hasCollectedDiary = true;
        }
    }
}