using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class Shell : MonoBehaviour
    {
        private TankController lounchedTank;

        private int step_id;

        [SerializeField]
        private ParticleSystem shellParticle;

        public void Set(TankController tank, int step)
        {
            lounchedTank = tank;
            step_id = step;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Field")
            {
                Destroy(gameObject);
                return;
            }

            if (collision.gameObject.tag == "Tank")
            {
                TankController otherTank = collision.transform.parent.GetComponent<TankController>();
                otherTank.OnDamaged();
                lounchedTank.OnShellHit();
                Destroy(gameObject);
            }
        }
    }
}
