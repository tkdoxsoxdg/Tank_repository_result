using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;     //player1 => Fire1, player2 => Fire2 参照する入力先の参照。
    public Rigidbody m_Shell;
    public Transform m_FireTransform;    //砲弾の位置。発射されたときの方向の参照。
    public Slider m_AimSlider;           //作成したFillの参照。
    public AudioSource m_ShootingAudio;  //発射したときの音の参照
    public AudioClip m_ChargingClip;     //装填した時の音の参照
    public AudioClip m_FireClip;         //砲弾が着弾した時の音の参照。
    public float m_MinLaunchForce = 15f; //砲弾の威力の最小値
    public float m_MaxLaunchForce = 30f; //砲弾の威力の最大値
    public float m_MaxChargeTime = 0.75f; //発射するまでの時間


    private string m_FireButton;      //発射コマンドの参照
    private float m_CurrentLaunchForce;
    private float m_ChargeSpeed;  //発射スピード
    private bool m_Fired;         //発射の判定


    //Tankが倒されたときの挙動
    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }


    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
        m_AimSlider.value = m_MinLaunchForce;

        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            //at max charge, not fired
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        else if (Input.GetButtonDown(m_FireButton))
        {
            // have we pressed fire for the first time?
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        else if (Input.GetButtonDown(m_FireButton) && !m_Fired)
        {
            // Holding the fire button, not yet fired
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            // we released the button, having not fired yet
            Fire();
        }
    }





    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}