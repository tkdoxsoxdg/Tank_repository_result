using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public int m_shellNumber = 1;
    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;
    public float m_PitchRange = 0.2f;


    private string m_MovementAxisName;
    private string m_TurnAxisName;
    private Rigidbody m_Rigidbody;
    private float m_MovementInputValue;
    private float m_TurnInputValue;
    private float m_OriginalPitch;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber; //垂直方向の動き。inputaxisにて設定した番号を呼び出すために、string変数に格納している。
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;　//水平方向の動き。inputaxisにて設定した番号を呼び出すために、string変数に格納している。

        m_OriginalPitch = m_MovementAudio.pitch;
    }


    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        EngineAudio();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        // Mathf.Abs >> 絶対値を返す。今回は、前進しようが後退（後ろに下がる場合は、マイナス値。y軸のマイナス値）しようが同じ値（絶対値）を与えてエンジン音を作動するようにスクリプトを記述する
        //アイドリング状態の場合の音声再生を、以下のif文で記述している。
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            //エンジン音の判定。オーディオクリップが定義されたもなら、再生する。
            //上記アイドリング状態が「正」としてオーディオクリップがドライビングだった場合に、アイドリング音にするif文を作成する。
            if(m_MovementAudio.clip == m_EngineDriving)
            {
                //上記アイドリング状態からドライビング音を拾ってきた場合に、アイドリング音に変更するように設定する。
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange); //ピッチのパラメーター；元のピッチから指定のピッチまでの範囲をランダムな数値でピッチを変える。
                m_MovementAudio.Play();//オーディオソースが再生中に変更された場合は、停止されるのでまた再生（変更したオーディオソース）するように設定。
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                //ドライビング状態からアイドリング音を拾ってきた場合に、ドライビング音に変更するように設定する。
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange); //ピッチのパラメーター；元のピッチから指定のピッチまでの範囲をランダムな数値でピッチを変える。
                m_MovementAudio.Play();//オーディオソースが再生中に変更された場合は、停止されるのでまた再生（変更したオーディオソース）するように設定。
            }
        }
    }


    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;//前進　>>　デルタタイム（毎フレーム/秒）とスピード（定義された）を乗算した数値で動くように設定。

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);//移動した位置を相対的に設定。動いた分に物理演算した位置を加えて設定。
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);//Quaternion Euler (float x, float y, float z); >> 今回はy軸を軸に回転（左右方向に）させる設定。
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);//回転（向きを変更）する時は、乗算（113行目のturnRotation変数）する。
    }
}