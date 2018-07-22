using UnityEngine;
using System.Collections;

public class ShellExplosion : MonoBehaviour
{
    [HideInInspector]public int ShooterID;
    [HideInInspector]public int ShellID;
    //public Transform ShellPrefab;
    //public Transform TankPrefab;
    public LayerMask m_TankMask;                        // 爆発が影響するものをフィルタリングするために使用。ここでは、"プレイヤー" に設定されます。
    public ParticleSystem m_ExplosionParticles;         // 爆発時に再生するパーティクルへの参照
    public AudioSource m_ExplosionAudio;                // 爆発時に再生するオーディオへの参照
    public float m_MaxDamage = 100f;                    // タンクが爆心にある場合に、タンクに与えられるダメージ量
    public float m_ExplosionForce = 1000f;              // タンクが爆心にある場合に、タンクに与えられる力の量
    public float m_MaxLifeTime = 2f;                    // 砲弾が削除されるまでの秒数
    public float m_ExplosionRadius = 5f;                // タンクに影響を及ぼすことが可能な爆発からの最大距離


    private void Start()
    {
        //これまでに破棄されていない場合は、生存期間が過ぎたら砲弾を破棄します。
        Destroy(gameObject, m_MaxLifeTime);
        //Transform shell = Instantiate(ShellPrefab) as Transform;
        //Transform tank = Instantiate(TankPrefab) as Transform;
        //if (ShooterID == GetComponent<TankShooting>().m_PlayerNumber)
            //Physics.IgnoreCollision(shell.GetComponent<Collider>(), tank.GetComponent<Collider>());
    }


    private void OnTriggerEnter(Collider other)
    {


        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            if (!targetHealth)
                continue;

            float damage = CalculateDamage(targetRigidbody.position);
            targetHealth.TakeDamage(damage);



        }


        m_ExplosionParticles.transform.parent = null;

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        //duration: obsolete. use "main.duration instead.
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);

        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {

        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        float damage = relativeDistance * m_MaxDamage;

        damage = Mathf.Max(0f, damage);

        return damage;

    }
}

