using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SG
{


    public class BonFire : Interactable
    {   public ActorController actorController;
        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);
            actorController.PlayTargetAnimFromOther("StartRest");
        }
    }
}