using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class Shell : MonoBehaviour
    {
        private TankController lounchedTank;

        private int step_id;

        public void Set(TankController tank, int step)
        {
            lounchedTank = tank;
            step_id = step;
        }

        void OnTriggerEnter(Collider collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Tank":
                    TankController otherTank = collision.gameObject.GetComponentInParent<TankController>();
                    otherTank.OnDamaged();
                    lounchedTank.OnShellHit(MLPlayer.Const.HitRewardPoint);
                    break;
                case "RewardItem":
                    Destroy(collision.gameObject);
                    lounchedTank.OnShellHit(MLPlayer.Const.RewardItemPoint);
                    lounchedTank.GetBomb();
                    break;
                default:
                    lounchedTank.ResetShell();
                    break;
            }
            Destroy(gameObject);
        }
    }
}
