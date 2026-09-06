using UnityEngine;

public class StartUpShader : MonoBehaviour
{
    public float thisScceneShaderStrength;
    public float thisScceneShaderWeakness;


    [Header("水中マテリアル")]
    [SerializeField]
    private Material realUnderWaterMaterial;


    [Header("光芒設定")]

    [Tooltip("光の発生位置 XY")]
    [SerializeField]
    private Vector2 lightOrigin = new Vector2(0.5f, 1.08f);

    [Tooltip("光の進行方向 XY")]
    [SerializeField]
    private Vector2 lightDirection = new Vector2(0.18f, -1f);

    [Tooltip("光芒の強さ")]
    [SerializeField]
    private float lightShaftStrength = 1.09f;

    [Tooltip("光芒の本数")]
    [SerializeField]
    private float lightShaftFrequency = 32.3f;

    [Tooltip("光芒の揺れる速さ")]
    [SerializeField]
    private float lightShaftSpeed = 2.74f;

    [Tooltip("光芒の輪郭")]
    [SerializeField]
    private float lightShaftSharpness = 2.06f;

    [Tooltip("光芒が届く長さ")]
    [SerializeField]
    private float lightShaftLength = 3.95f;


    void Start()
    {
        // 元々の水中感の処理
        UnderWaterShaderManager.Instance.maximumStrength =
            thisScceneShaderStrength;
        UnderWaterShaderManager.Instance.minimumStrength =
            thisScceneShaderWeakness;

        // 光芒だけ即座にこのシーン用の値へ変更
        SetLightShaftSettings();
    }


    private void SetLightShaftSettings()
    {
        if (realUnderWaterMaterial == null)
        {
            Debug.LogError(
                "RealUnderWaterMaterialが設定されていません。"
            );

            return;
        }

        // 光の発生位置
        realUnderWaterMaterial.SetVector(
            "_LightOrigin",
            new Vector4(
                lightOrigin.x,
                lightOrigin.y,
                0f,
                0f
            )
        );

        // 光の進行方向
        realUnderWaterMaterial.SetVector(
            "_LightDirection",
            new Vector4(
                lightDirection.x,
                lightDirection.y,
                0f,
                0f
            )
        );

        // 光芒の強さ
        realUnderWaterMaterial.SetFloat(
            "_LightShaftStrength",
            lightShaftStrength
        );

        // 光芒の本数
        realUnderWaterMaterial.SetFloat(
            "_LightShaftFrequency",
            lightShaftFrequency
        );

        // 光芒の揺れる速さ
        realUnderWaterMaterial.SetFloat(
            "_LightShaftSpeed",
            lightShaftSpeed
        );

        // 光芒の輪郭
        realUnderWaterMaterial.SetFloat(
            "_LightShaftSharpness",
            lightShaftSharpness
        );

        // 光芒が届く長さ
        realUnderWaterMaterial.SetFloat(
            "_LightShaftLength",
            lightShaftLength
        );
    }


    void Update()
    {

    }
}