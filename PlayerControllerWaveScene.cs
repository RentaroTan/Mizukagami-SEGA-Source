using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControllerWaveScene : MonoBehaviour
{
     
    private CharacterController characterController;
    [Header("移動変数")]
    [SerializeField] private float moveSpeed = 5f;
    [Header("カメラ関係")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minCameraAngle = -30f;
    [SerializeField] private float maxCameraAngle = 60f;

    [Header("重力")]
    [SerializeField] private float gravity = -9.81f;

      [Header("日記背景")]
       public bool openingDiary;
       [SerializeField] private GameObject waterBackground;

    private float cameraAngle;
    private float verticalVelocity;

    [Header("歩行SE")]
     public AudioSource walkingAudioSource;
     public AudioClip walkingAudioClip;
     public AudioClip walkingRainAudioClip;
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!DiaryManagerInWaveScene.Instance.openingDiary) // 日記が開いていない場合のみ操作を受け付ける
        {
            RotateView();
            Move();
        }
         if (Input.GetKeyDown(KeyCode.Tab))
        {
            openingDiary =! openingDiary;
        }
        waterBackground.SetActive(openingDiary);
    }

    public void Move()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        if(Input.GetKey(KeyCode.A))
        {
            horizontalInput -= 1f;
        }

        if(Input.GetKey(KeyCode.D))
        {
            horizontalInput += 1f;
        }

        if(Input.GetKey(KeyCode.W))
        {
            verticalInput += 1f;
        }
        if(Input.GetKey(KeyCode.S))
        {
            verticalInput -= 1f;
        }

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 moveDirection = forward * verticalInput + right * horizontalInput;
        moveDirection = moveDirection.normalized * moveSpeed * Time.deltaTime;
       
        if(characterController.isGrounded && verticalVelocity <0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        moveDirection.y = verticalVelocity;
        characterController.Move(moveDirection);
        if(horizontalInput != 0f || verticalInput != 0f)
        {
            PlayWalkingSound(true);
        }
        else if(horizontalInput == 0f && verticalInput == 0f && characterController.isGrounded)
        {
            PlayWalkingSound(false);
        }
    }

public void PlayWalkingSound(bool shouldPlay)
{
    if (walkingAudioSource == null) return;

    // WaveScene5だけ雨用の足音
    AudioClip currentWalkingClip;

    if (SceneManager.GetActiveScene().name == "WaveScene5")
    {
        currentWalkingClip = walkingRainAudioClip;
    }
    else
    {
        currentWalkingClip = walkingAudioClip;
    }

    if (currentWalkingClip == null) return;

    if (shouldPlay)
    {
        if (!walkingAudioSource.isPlaying)
        {
            walkingAudioSource.clip = currentWalkingClip;
            walkingAudioSource.loop = true;
            walkingAudioSource.Play();
        }
    }
    else
    {
        if (walkingAudioSource.isPlaying)
        {
            walkingAudioSource.Stop();
        }
    }
}

    public void RotateView()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        cameraAngle -= mouseY;
        cameraAngle = Mathf.Clamp(cameraAngle, minCameraAngle, maxCameraAngle);
        cameraPivot.localRotation = Quaternion.Euler(cameraAngle,0f,0f);
    }

}