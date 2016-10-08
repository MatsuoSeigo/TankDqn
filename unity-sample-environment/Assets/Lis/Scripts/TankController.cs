using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

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
        public Transform m_FireTransform;

        public float m_LaunchForce = 10f;
        private bool m_Fired;
                
        private float m_MovementInputValue;
        private float m_TurnInputValue;

        // Use this for initialization
        private void Start()
        {
            m_agent = GetComponent<MLPlayer.Agent> ();
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            m_MovementInputValue = m_agent.action.forward;
            m_TurnInputValue = m_agent.action.rotate;
            Fire();
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

        public void OnShellHit()
        {
            m_Fired = false;
            AddReward(MLPlayer.Const.HitRewardPoint);
        }

        public void OnDamaged()
        {
            AddReward(MLPlayer.Const.DamageRewardPoint);
        }

        public void AddReward(float reward)
        {
            if (!m_agent.state.endEpisode)
            {
                m_agent.state.reward += reward;
            }
        }
    }
}
