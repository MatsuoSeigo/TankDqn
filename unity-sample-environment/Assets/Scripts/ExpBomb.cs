using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class ExpBomb : MonoBehaviour {
        TankController lounchedTank;

        public void Set(TankController tank)
        {
            lounchedTank = tank;
        }

    	void OnTriggerEnter (Collider col)
        {
			if(col.gameObject.tag == "Tank"){
                TankController otherTank = col.gameObject.GetComponentInParent<TankController>();
                otherTank.OnBombDamaged();
                lounchedTank.OnShellHit(MLPlayer.Const.BombHitPoint);
			}
            lounchedTank.ResetShell();
            Destroy(gameObject);
    	}
    }
}