using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DiaryManagerInWaveScene : MonoBehaviour
{
    public static DiaryManagerInWaveScene Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        // すでに別のDiaryManagerが存在する場合、
        // 新しく生成された方を削除する
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 最初に生成されたものをInstanceにする
        Instance = this;

        // シーンを移動しても削除されないようにする
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        // 本体が本当に削除された場合にInstanceを空にする
        if (Instance == this)
        {
            Instance = null;
        }
    }

    [Header("Raycast UI")]
    [SerializeField]
    private GameObject canvas;

    [Header("テスト用")]
    [SerializeField]
    private bool resetCollectedOnStart = true;

    [Header("日記UI")]
    [FormerlySerializedAs("DiaryCanvas")]
    [SerializeField]
    private GameObject diaryCanvas;

    [Header("日記データ")]
    [FormerlySerializedAs("dialyDateSO")]
    [SerializeField]
    private DialyDateSO diaryDateSO;

   [Header("左ページ")]
[SerializeField]
private TMP_Text leftDateText;

[SerializeField]
private TMP_Text leftBodyText;

// 追記された文章を表示するText
[SerializeField]
private TMP_Text leftAddedBodyText;


[Header("右ページ")]
[SerializeField]
private TMP_Text rightDateText;

[SerializeField]
private TMP_Text rightBodyText;

// 追記された文章を表示するText
[SerializeField]
private TMP_Text rightAddedBodyText;

    [Header("文字表示演出")]
    [SerializeField]
    private float textFadeDuration = 1.5f;

    [Header("音関連")]
    [SerializeField]
    private AudioSource diaryOpenAndCloseSound;

    [SerializeField]
    private AudioClip diaryOpenAndCloseClip;

    [SerializeField]
    private AudioClip diaryChangePageClip;

    // 現在表示している見開き番号
    private int currentSpreadIndex;

    // 日記帳が開いているか
    public bool openingDiary;

    // イベント中か
    public bool onEvent;

    // すでにフェード演出を行った日記番号
    private HashSet<int> revealedDiaryNumbers
        = new HashSet<int>();

    // 書き換えイベントが発生済みの日記番号
    private HashSet<int> rewrittenDiaryNumbers
        = new HashSet<int>();

        // 追記・書き換えの演出をすでに見た日記番号
    private HashSet<int> changeAnimationPlayedNumbers
    = new HashSet<int>();

    private void Start()
    {
        if (resetCollectedOnStart)
        {
            ResetCollectedDiaries();
        }

        openingDiary = false;
        currentSpreadIndex = 0;

        if (diaryCanvas != null)
        {
            diaryCanvas.SetActive(false);
        }

        // 最初から取得済みにする日記
        CollectDiary(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TurningDiaryMenu();
        }
    }

    // 日記帳を開閉する
    public void TurningDiaryMenu()
    {
        if (onEvent)
        {
            return;
        }

        openingDiary = !openingDiary;

        if (diaryCanvas != null)
        {
            diaryCanvas.SetActive(openingDiary);
        }

        if (openingDiary)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (diaryOpenAndCloseSound != null &&
            diaryOpenAndCloseClip != null)
        {
            diaryOpenAndCloseSound.PlayOneShot(
                diaryOpenAndCloseClip
            );
        }

        if (openingDiary)
        {
            DisplayCurrentSpread();
        }
    }

    // 日記を拾ったときに呼ばれる
    public void CollectDiary(int diaryNumber)
    {
        if (diaryDateSO == null ||
            diaryDateSO.dialyDate == null)
        {
            Debug.LogError(
                "日記データのSOが設定されていません。"
            );

            return;
        }

        foreach (DialyDateSO.DialyDates diary
                 in diaryDateSO.dialyDate)
        {
            if (diary.Number == diaryNumber)
            {
                diary.IsCollected = true;

                Debug.Log(
                    "日記を拾いました。Number：" +
                    diaryNumber
                );

                // 日記を開いている最中なら表示を更新する
                if (openingDiary)
                {
                    DisplayCurrentSpread();
                }

                return;
            }
        }

        Debug.LogWarning(
            "Numberが" + diaryNumber +
            "の日記が見つかりません。"
        );
    }

    /// <summary>
    /// 指定した日記をText2表示に切り替える
    /// </summary>
   public void RewriteDiary(int diaryNumber)
{
    if (diaryDateSO == null ||
        diaryDateSO.dialyDate == null)
    {
        Debug.LogError(
            "日記データのSOが設定されていません。"
        );

        return;
    }

    foreach (DialyDateSO.DialyDates diary
             in diaryDateSO.dialyDate)
    {
        // 指定された日記じゃなかったら次へ
        if (diary.Number != diaryNumber)
        {
            continue;
        }

        // この日記に変化イベントが設定されているか
        if (!diary.MemoryEventFlg)
        {
            Debug.LogWarning(
                "日記番号" + diaryNumber +
                "はMemoryEventFlgがオフです。"
            );

            return;
        }

// ChangeTypeが設定されているか確認
if (diary.ChangeType == DiaryChangeType.None)
{
    Debug.LogWarning(
        "日記番号" + diaryNumber +
        "のChangeTypeがNoneです。"
    );

    return;
}


// AppendなのにAddedTextが空なら終了
if (diary.ChangeType == DiaryChangeType.Append &&
    string.IsNullOrEmpty(diary.AddedText))
{
    Debug.LogWarning(
        "日記番号" + diaryNumber +
        "のAddedTextが入力されていません。"
    );

    return;
}


// ReplaceなのにText2が空なら終了
if (diary.ChangeType == DiaryChangeType.Replace &&
    string.IsNullOrEmpty(diary.Text2))
{
    Debug.LogWarning(
        "日記番号" + diaryNumber +
        "のText2が入力されていません。"
    );

    return;
}

        // 変化イベント発生済みとして登録
        bool newlyRewritten =
            rewrittenDiaryNumbers.Add(diaryNumber);

        if (newlyRewritten)
        {
            Debug.Log(
                "日記番号" + diaryNumber +
                "に変化イベントが発生しました。"
            );
        }

        // 日記を今開いているなら再表示
        if (openingDiary)
        {
            DisplayCurrentSpread();
        }

        return;
    }

    Debug.LogWarning(
        "Numberが" + diaryNumber +
        "の日記が見つかりません。"
    );
}

    /// <summary>
    /// 指定した日記が書き換え済みか確認する
    /// </summary>
    public bool IsDiaryRewritten(int diaryNumber)
    {
        return rewrittenDiaryNumbers.Contains(
            diaryNumber
        );
    }

    /// <summary>
    /// 書き換え状態を解除してText1に戻す
    /// </summary>
   public void ResetRewrittenDiary(int diaryNumber)
{
    bool wasRemoved =
        rewrittenDiaryNumbers.Remove(diaryNumber);

    if (!wasRemoved)
    {
        return;
    }

    changeAnimationPlayedNumbers.Remove(diaryNumber);

    revealedDiaryNumbers.Remove(diaryNumber);

    if (openingDiary)
    {
        DisplayCurrentSpread();
    }
}

    // 次の見開きへ進む
    public void NextPages()
{
    Debug.Log("ボタンは押されてる");

    if (diaryDateSO == null ||
        diaryDateSO.dialyDate == null ||
        diaryDateSO.dialyDate.Count == 0)
    {
        return;
    }

    // 取得済みの日記の中で、一番後ろのインデックスを探す
    int lastCollectedIndex = -1;

    for (int i = 0; i < diaryDateSO.dialyDate.Count; i++)
    {
        if (diaryDateSO.dialyDate[i].IsCollected)
        {
            lastCollectedIndex = i;
        }
    }

    // 1枚も取得していない場合
    if (lastCollectedIndex < 0)
    {
        return;
    }

    // 取得済みの日記が存在する最後の見開き
    int maxSpreadIndex = lastCollectedIndex / 2;

    if (currentSpreadIndex < maxSpreadIndex)
    {
        currentSpreadIndex++;

        DisplayCurrentSpread();

        if (diaryOpenAndCloseSound != null &&
            diaryChangePageClip != null)
        {
            diaryOpenAndCloseSound.PlayOneShot(
                diaryChangePageClip
            );
        }
    }
}

    // 前の見開きへ戻る
    public void PreviousPages()
    {
        Debug.Log("ボタンは押されてる");

        if (currentSpreadIndex > 0)
        {
            currentSpreadIndex--;

            DisplayCurrentSpread();
        }

        if (diaryOpenAndCloseSound != null &&
            diaryChangePageClip != null)
        {
            diaryOpenAndCloseSound.PlayOneShot(
                diaryChangePageClip
            );
        }
    }

    // 現在の左右ページを表示する
    private void DisplayCurrentSpread()
    {
        // 前のページで動いているフェードを停止する
        StopAllCoroutines();

        int leftIndex =
            currentSpreadIndex * 2;

        int rightIndex =
            leftIndex + 1;

        DisplayDiaryOnPage(
    leftIndex,
    leftDateText,
    leftBodyText,
    leftAddedBodyText
);

DisplayDiaryOnPage(
    rightIndex,
    rightDateText,
    rightBodyText,
    rightAddedBodyText
);
    }

    // 指定した1ページを表示する
   private void DisplayDiaryOnPage(
    int index,
    TMP_Text dateText,
    TMP_Text bodyText,
    TMP_Text addedBodyText)
{
    if (dateText == null ||
        bodyText == null)
    {
        return;
    }

    // 前の表示を消す
    dateText.text = "";
    bodyText.text = "";
    bodyText.alpha = 1f;

    if (addedBodyText != null)
    {
        addedBodyText.text = "";
        addedBodyText.alpha = 1f;
    }

    if (diaryDateSO == null ||
        diaryDateSO.dialyDate == null)
    {
        return;
    }

    if (index < 0 ||
        index >= diaryDateSO.dialyDate.Count)
    {
        return;
    }

    DialyDateSO.DialyDates diary =
        diaryDateSO.dialyDate[index];

    if (!diary.IsCollected)
    {
        return;
    }

    dateText.text = diary.Date;


    // =========================================
    // ① まだ変化イベントが起きていない
    // =========================================

    if (!rewrittenDiaryNumbers.Contains(diary.Number))
    {
        bodyText.text = diary.Text1;

        // Text1を初めて見るときだけフェード
        if (revealedDiaryNumbers.Add(diary.Number))
        {
            bodyText.alpha = 0f;

            StartCoroutine(
                FadeInDiaryText(
                    bodyText,
                    diary.Text1
                )
            );
        }

        return;
    }


    // =========================================
    // ② 変化演出をすでに見た
    // =========================================

    if (changeAnimationPlayedNumbers.Contains(diary.Number))
    {
        if (diary.ChangeType == DiaryChangeType.Append)
        {
            bodyText.text = diary.Text1;

            if (addedBodyText != null)
            {
                addedBodyText.text = diary.AddedText;
                addedBodyText.alpha = 1f;
            }
        }
        else if (diary.ChangeType == DiaryChangeType.Replace)
        {
            bodyText.text = diary.Text2;
        }

        return;
    }


    // =========================================
    // ③ イベント後、初めて見る
    // =========================================

    if (diary.ChangeType == DiaryChangeType.Append)
    {
        // 元の文章はそのまま
        bodyText.text = diary.Text1;

        if (addedBodyText == null)
        {
            return;
        }

        // 追記文章だけ透明状態でセット
        addedBodyText.text = diary.AddedText;
        addedBodyText.alpha = 0f;

        StartCoroutine(
            FadeInAddedDiaryText(
                diary.Number,
                addedBodyText,
                diary.AddedText
            )
        );
    }
    else if (diary.ChangeType == DiaryChangeType.Replace)
    {
        // 最初は古い文章を表示
        bodyText.text = diary.Text1;
        bodyText.alpha = 1f;

        StartCoroutine(
            ReplaceDiaryText(
                diary.Number,
                bodyText,
                diary.Text1,
                diary.Text2
            )
        );
    }
}

    // 本文の透明度を徐々に濃くする
    private IEnumerator FadeInDiaryText(
        TMP_Text bodyText,
        string expectedText)
    {
        float elapsedTime = 0f;

        while (elapsedTime < textFadeDuration)
        {
            // 別ページに移動して文字が変わったら終了する
            if (bodyText == null ||
                bodyText.text != expectedText)
            {
                yield break;
            }

            elapsedTime +=
                Time.unscaledDeltaTime;

            bodyText.alpha =
                Mathf.Clamp01(
                    elapsedTime /
                    textFadeDuration
                );

            yield return null;
        }

        // 最後は完全に表示する
        if (bodyText != null &&
            bodyText.text == expectedText)
        {
            bodyText.alpha = 1f;
        }
    }

    // 追記された文章だけフェードイン
private IEnumerator FadeInAddedDiaryText(
    int diaryNumber,
    TMP_Text addedBodyText,
    string expectedText)
{
    float elapsedTime = 0f;

    while (elapsedTime < textFadeDuration)
    {
        if (addedBodyText == null ||
            addedBodyText.text != expectedText)
        {
            yield break;
        }

        elapsedTime += Time.unscaledDeltaTime;

        addedBodyText.alpha =
            Mathf.Clamp01(
                elapsedTime / textFadeDuration
            );

        yield return null;
    }

    if (addedBodyText != null &&
        addedBodyText.text == expectedText)
    {
        addedBodyText.alpha = 1f;

        // 変化演出を見たと記録
        changeAnimationPlayedNumbers.Add(
            diaryNumber
        );
    }
}

// Text1を消してText2へ書き換える
private IEnumerator ReplaceDiaryText(
    int diaryNumber,
    TMP_Text bodyText,
    string oldText,
    string newText)
{
    float elapsedTime = 0f;

    // Text1をフェードアウト
    while (elapsedTime < textFadeDuration)
    {
        if (bodyText == null ||
            bodyText.text != oldText)
        {
            yield break;
        }

        elapsedTime += Time.unscaledDeltaTime;

        bodyText.alpha =
            1f -
            Mathf.Clamp01(
                elapsedTime / textFadeDuration
            );

        yield return null;
    }

    if (bodyText == null)
    {
        yield break;
    }

    // 文章そのものをText2へ変更
    bodyText.text = newText;
    bodyText.alpha = 0f;

    elapsedTime = 0f;

    // Text2をフェードイン
    while (elapsedTime < textFadeDuration)
    {
        if (bodyText == null ||
            bodyText.text != newText)
        {
            yield break;
        }

        elapsedTime += Time.unscaledDeltaTime;

        bodyText.alpha =
            Mathf.Clamp01(
                elapsedTime / textFadeDuration
            );

        yield return null;
    }

    if (bodyText != null &&
        bodyText.text == newText)
    {
        bodyText.alpha = 1f;

        // 変化演出を見たと記録
        changeAnimationPlayedNumbers.Add(
            diaryNumber
        );
    }
}

    // 拾った日記の枚数を外部に渡す
    public int CollectedDiaryCount
    {
        get
        {
            if (diaryDateSO == null ||
                diaryDateSO.dialyDate == null)
            {
                return 0;
            }

            int count = 0;

            foreach (DialyDateSO.DialyDates diary
                     in diaryDateSO.dialyDate)
            {
                if (diary.IsCollected)
                {
                    count++;
                }
            }

            return count;
        }
    }

    // テスト開始時に日記をすべて未取得に戻す
    private void ResetCollectedDiaries()
    {
        if (diaryDateSO == null ||
            diaryDateSO.dialyDate == null)
        {
            return;
        }

        foreach (DialyDateSO.DialyDates diary
                 in diaryDateSO.dialyDate)
        {
            diary.IsCollected = false;
        }

        // テスト開始時は書き換え状態も解除
        rewrittenDiaryNumbers.Clear();
        revealedDiaryNumbers.Clear();
        changeAnimationPlayedNumbers.Clear();

    }

    // 今開いている見開き番号を外部に渡す
    public int GiveCurrentSpread
    {
        get
        {
            return currentSpreadIndex;
        }
    }

    public void StartEvent()
    {
        onEvent = true;
        openingDiary = false;

        if (diaryCanvas != null)
        {
            diaryCanvas.SetActive(false);
        }

        StopAllCoroutines();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (canvas != null)
        {
            canvas.SetActive(false);
        }
    }

    public void EndEvent()
    {
        onEvent = false;

        if (canvas != null)
        {
            canvas.SetActive(true);
        }
    }
}