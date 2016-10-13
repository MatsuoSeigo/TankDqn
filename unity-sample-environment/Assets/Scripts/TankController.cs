using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using UnityEngine.UI;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (MLPlayer.Agent))]
    public class TankController : MonoBehaviour
    {
        private MLPlayer.Agent m_agent;
        private Rigidbody m_Rigidbody;

        public float m_Speed = 3f;
        public float m_TurnSpeed = 45f;

        public Rigidbody m_Shell;
        public Rigidbody m_Bomb;
        public Transform m_FireTransform;

        public float m_LaunchForce = 15f;
        private bool m_Fired;
        private bool hasBomb = false;
        [SerializeField] Image hasBombImage;

        private float m_MovementInputValue;
        private float m_TurnInputValue;

        public int damagedCount = 0;

        // Use this for initialization
        private void Start()
        {
            m_agent = GetComponent<MLPlayer.Agent> ();
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (m_agent.action == null)
            {
                m_agent.action = new MLPlayer.Action();
                m_agent.action.Clear();
            }
            m_MovementInputValue = m_agent.action.forward;
            m_TurnInputValue = m_agent.action.rotate;
            Fire();
            Bomb();
        }

        private void FixedUpdate()
        {
            Move();
            Turn();
        }

        private void Move()
        {
            Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
            
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }


        private void Turn()
        {
            float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
            
            Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);
            
            m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
        }

        private void Fire()
        {
            if (!m_agent.action.fire || m_Fired)
            {
                return;
            }
            
            m_Fired = true;
            
            Rigidbody shellInstance = Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
            
            shellInstance.velocity = m_LaunchForce * m_FireTransform.forward;

            shellInstance.GetComponent<Shell>().Set(this, m_agent.state.step_id);
        }

        private void Bomb()
        {
            if (!m_agent.action.bomb || m_Fired || !hasBomb)
            {
                return;
            }

            m_Fired = true;
            hasBomb = false;
            hasBombImage.gameObject.SetActive(false);

            Rigidbody shellInstance = Instantiate (m_Bomb, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

            shellInstance.velocity = m_LaunchForce * m_FireTransform.forward;

            shellInstance.GetComponent<ExpBomb>().Set(this);
        }

        public void GetBomb()
        {
            hasBomb = true;
            hasBombImage.gameObject.SetActive(true);
        }

        public void OnShellHit(float point)
        {
            m_Fired = false;
            AddReward(point);
        }

        public void ResetShell()
        {
            m_Fired = false;
        }

        public void OnDamaged()
        {
            damagedCount+=1;
            AddReward(MLPlayer.Const.DamageRewardPoint);
        }

        public void OnBombDamaged()
        {
            damagedCount+=3;
            AddReward(MLPlayer.Const.DamageRewardPoint);
        }

        public void AddReward(float reward)
        {
            if (!m_agent.state.endEpisode)
            {
                m_agent.state.reward += reward;
            }
        }

        public void ResetState()
        {
            hasBomb = false;
            damagedCount = 0;
        }

        public void CheckDamage()
        {
            if (damagedCount >= 3)
            {
                m_agent.EndEpisode();
            }
        }
    }
}
