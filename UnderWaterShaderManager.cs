using UnityEngine;

/// <summary>
/// 水中フルスクリーンシェーダーの「水中感の強さ」を管理します。
///
/// 点滅中は次の動きを繰り返します。
/// 最小値 → 最大値 → 最大値で停止 → 最小値 → 最小値で停止
/// </summary>
[DisallowMultipleComponent]
public sealed class UnderWaterShaderManager : MonoBehaviour
{
    public static UnderWaterShaderManager Instance { get; private set; }

    private static readonly int UnderwaterStrengthId =
        Shader.PropertyToID("_UnderwaterStrength");

    private enum BlinkingState
    {
        MovingToMaximum,
        HoldingAtMaximum,
        MovingToMinimum,
        HoldingAtMinimum
    }

    [Header("水中シェーダー")]
    [Tooltip("Full Screen Pass Renderer Featureで使用している水中マテリアル")]
    [SerializeField]
    private Material underwaterMaterial;

    [Header("点滅設定")]
    [Tooltip("ゲーム開始時から点滅させるか")]
    [SerializeField]
    private bool blinkingOnStart = true;

    [Tooltip("点滅時の水中感の最小値")]
    
    public float minimumStrength = 0.35f;

    [Tooltip("点滅時の水中感の最大値")]
    [SerializeField, Range(0f, 1f)]
    public float maximumStrength = 0.85f;

    [Tooltip("水中感が上下する速度")]
    [SerializeField, Min(0f)]
    private float blinkingSpeed = 0.5f;

    [Header("停止時間")]
    [Tooltip("最大値に到達した後、その値を維持する秒数")]
    [SerializeField, Min(0f)]
    private float maximumHoldDuration = 2f;

    [Tooltip("最小値に到達した後、その値を維持する秒数")]
    [SerializeField, Min(0f)]
    private float minimumHoldDuration = 2f;

    [Header("時間設定")]
    [Tooltip("Time.timeScaleが0でも点滅と停止時間を進めるか")]
    [SerializeField]
    private bool useUnscaledTime;

    private bool isBlinking;
    private float currentStrength;
    private float holdTimer;
    private BlinkingState blinkingState;

    public bool IsBlinking => isBlinking;
    public float CurrentStrength => currentStrength;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (underwaterMaterial == null)
        {
            Debug.LogError(
                $"{nameof(UnderWaterShaderManager)}: " +
                "Underwater Materialが設定されていません。",
                this
            );

            enabled = false;
            return;
        }

        CorrectInspectorValues();

        currentStrength = Mathf.Clamp(
            underwaterMaterial.GetFloat(UnderwaterStrengthId),
            minimumStrength,
            maximumStrength
        );

        isBlinking = blinkingOnStart;
        blinkingState = DecideInitialMovingState();
        holdTimer = 0f;

        ApplyStrength(currentStrength);
    }

    private void Update()
    {
        if (!isBlinking)
        {
            return;
        }

        CorrectInspectorValues();

        float deltaTime = useUnscaledTime
            ? Time.unscaledDeltaTime
            : Time.deltaTime;

        switch (blinkingState)
        {
            case BlinkingState.MovingToMaximum:
                MoveToMaximum(deltaTime);
                break;

            case BlinkingState.HoldingAtMaximum:
                HoldAtMaximum(deltaTime);
                break;

            case BlinkingState.MovingToMinimum:
                MoveToMinimum(deltaTime);
                break;

            case BlinkingState.HoldingAtMinimum:
                HoldAtMinimum(deltaTime);
                break;
        }
    }

    private void MoveToMaximum(float deltaTime)
    {
        currentStrength = Mathf.MoveTowards(
            currentStrength,
            maximumStrength,
            blinkingSpeed * deltaTime
        );

        ApplyStrength(currentStrength);

        if (!Mathf.Approximately(currentStrength, maximumStrength))
        {
            return;
        }

        currentStrength = maximumStrength;
        ApplyStrength(currentStrength);

        holdTimer = maximumHoldDuration;
        blinkingState = BlinkingState.HoldingAtMaximum;
    }

    private void HoldAtMaximum(float deltaTime)
    {
        currentStrength = maximumStrength;
        ApplyStrength(currentStrength);

        holdTimer -= deltaTime;

        if (holdTimer > 0f)
        {
            return;
        }

        holdTimer = 0f;
        blinkingState = BlinkingState.MovingToMinimum;
    }

    private void MoveToMinimum(float deltaTime)
    {
        currentStrength = Mathf.MoveTowards(
            currentStrength,
            minimumStrength,
            blinkingSpeed * deltaTime
        );

        ApplyStrength(currentStrength);

        if (!Mathf.Approximately(currentStrength, minimumStrength))
        {
            return;
        }

        currentStrength = minimumStrength;
        ApplyStrength(currentStrength);

        holdTimer = minimumHoldDuration;
        blinkingState = BlinkingState.HoldingAtMinimum;
    }

    private void HoldAtMinimum(float deltaTime)
    {
        currentStrength = minimumStrength;
        ApplyStrength(currentStrength);

        holdTimer -= deltaTime;

        if (holdTimer > 0f)
        {
            return;
        }

        holdTimer = 0f;
        blinkingState = BlinkingState.MovingToMaximum;
    }

    public void StartBlinking()
    {
        CorrectInspectorValues();

        isBlinking = true;
        holdTimer = 0f;
        blinkingState = DecideInitialMovingState();
    }

    public void StopBlinking()
    {
        isBlinking = false;
    }

    public void SetBlinking(bool shouldBlink)
    {
        if (shouldBlink)
        {
            StartBlinking();
        }
        else
        {
            StopBlinking();
        }
    }

    public void SetStrengthInstant(float strength)
    {
        isBlinking = false;
        holdTimer = 0f;
        currentStrength = Mathf.Clamp01(strength);

        ApplyStrength(currentStrength);
    }

    public void SetBlinkingRange(float minimum, float maximum)
    {
        minimumStrength = Mathf.Clamp01(minimum);
        maximumStrength = Mathf.Clamp01(maximum);

        CorrectInspectorValues();

        currentStrength = Mathf.Clamp(
            currentStrength,
            minimumStrength,
            maximumStrength
        );

        holdTimer = 0f;
        blinkingState = DecideInitialMovingState();

        ApplyStrength(currentStrength);
    }

    public void SetBlinkingSpeed(float speed)
    {
        blinkingSpeed = Mathf.Max(0f, speed);
    }

    public void SetHoldDurations(
        float maximumDuration,
        float minimumDuration
    )
    {
        maximumHoldDuration = Mathf.Max(0f, maximumDuration);
        minimumHoldDuration = Mathf.Max(0f, minimumDuration);
    }

    private BlinkingState DecideInitialMovingState()
    {
        float distanceToMinimum =
            Mathf.Abs(currentStrength - minimumStrength);

        float distanceToMaximum =
            Mathf.Abs(currentStrength - maximumStrength);

        return distanceToMaximum <= distanceToMinimum
            ? BlinkingState.MovingToMinimum
            : BlinkingState.MovingToMaximum;
    }

    private void ApplyStrength(float strength)
    {
        underwaterMaterial.SetFloat(
            UnderwaterStrengthId,
            Mathf.Clamp01(strength)
        );
    }

    private void CorrectInspectorValues()
    {
        minimumStrength = Mathf.Clamp01(minimumStrength);
        maximumStrength = Mathf.Clamp01(maximumStrength);

        if (minimumStrength > maximumStrength)
        {
            (minimumStrength, maximumStrength) =
                (maximumStrength, minimumStrength);
        }

        blinkingSpeed = Mathf.Max(0f, blinkingSpeed);
        maximumHoldDuration = Mathf.Max(0f, maximumHoldDuration);
        minimumHoldDuration = Mathf.Max(0f, minimumHoldDuration);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CorrectInspectorValues();
    }
#endif
}