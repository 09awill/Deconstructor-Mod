using DeconstructorMod;
using Kitchen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine.Scripting;

namespace KitchenDeconstructor
{
    [UpdateBefore(typeof(PickUpAndDropAppliance))]
    [UpdateBefore(typeof(StoreBlueprint))]
    [UpdateBefore(typeof(RetrieveBlueprint))]
    internal class DeconstructorPutdown : ApplianceInteractionSystem
    {
        private CItemHolder m_Holder;
        private CItemHolder m_CabinetHolder;
        private CApplianceBlueprint m_Blueprint;
        private CForSale m_Sale;
        private CAppliance m_Appliance;
        private CBlueprintStore m_Store;
        private CIDeconstruct m_Deconstruct;
        private bool m_Pickup = false;
        protected override InteractionType RequiredType => InteractionType.Grab;
        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Require<CItemHolder>(data.Interactor, out m_Holder))
            {
                return false;
            }
            if (!Require<CItemHolder>(data.Target, out m_CabinetHolder))
            {
                return false;
            }
            if (!Require<CBlueprintStore>(data.Target, out m_Store))
            {
                //Mod.LogInfo("Attempted Deconstructor put down but couldn't find blueprint store on the cabinet");
                return false;
            }
            if (m_Store.InUse)
            {
                //Mod.LogInfo("Attempted Deconstructor put down but store was in use");
                return false;
            }
            if (m_Holder.HeldItem == Entity.Null)
            {
                Mod.LogInfo("Player Held Item Null, Setting pickup to true...");
                m_Pickup = true;
                if(m_CabinetHolder.HeldItem == Entity.Null)
                {
                    Mod.LogInfo("Cabinet Item was also null so returning false");
                    return false;
                }
            } else
            {
                Mod.LogInfo("Player Held Item was true, Setting pickup to false...");
                m_Pickup = false;
                if (m_CabinetHolder.HeldItem != Entity.Null)
                {
                    Mod.LogInfo("Cabinet already has held item, Returning false...");
                    return false;
                }
            }
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            if (m_Pickup)
            {
                Mod.LogInfo("Picking up Appliance from Deconstructor");
                Set<CHeldBy>(m_CabinetHolder.HeldItem, data.Interactor);
                Set<CRemoveView>(m_CabinetHolder.HeldItem);
                Set<CPosition>(m_CabinetHolder.HeldItem, CPosition.Hidden);
                Set<CItemHolder>(data.Interactor, new CItemHolder() { HeldItem = m_CabinetHolder.HeldItem });
                EntityManager.RemoveComponent<CPreservedOvernight>(m_CabinetHolder.HeldItem);
                //EntityManager.RemoveComponent<CCreateAppliance>(m_CabinetHolder.HeldItem);
            } else
            {
                Mod.LogInfo("Placing Appliance In Deconstructor");
                Set<CHeldBy>(m_Holder.HeldItem, data.Target);
                Set<CRemoveView>(m_Holder.HeldItem);
                Set<CPosition>(m_Holder.HeldItem, CPosition.Hidden);
                Set<CPreservedOvernight>(m_Holder.HeldItem);
                //Set<CCreateAppliance>(m_Holder.HeldItem, new CCreateAppliance());
                Set<CItemHolder>(data.Target, new CItemHolder() { HeldItem = m_Holder.HeldItem });
                EntityManager.RemoveComponent<CHeldAppliance>(m_Holder.HeldItem);
                EntityManager.RemoveComponent<CRequiresView>(m_Holder.HeldItem);
            }

            //SetOccupant(interact.Location, new Entity(), component.Layer); Might need to use this

            Mod.LogInfo("StoreAppliance.Perform");
            /*
            m_Store.ApplianceID = m_Appliance.ID;
            //m_Store.BlueprintID = m_Appliance.ID;
            m_Store.BlueprintID = m_Appliance.ID;
            m_Store.InUse = true;
            SetComponent(data.Target, m_Store);
            */
            //data.Context.Destroy(m_Holder.HeldItem); //Destroying works but if you take it out again and place it the item dissapears when the game starts
            /*
            if (m_Store.Price == m_Sale.Price && m_Store.ApplianceID == m_Blueprint.Appliance)
            {
                data.Context.Destroy(m_Holder.HeldItem);
                data.Context.Set(data.Interactor, default(CItemHolder));
                m_Store.HasBeenCopied = true;
                SetComponent(data.Target, m_Store);
            }
            */
        }
    }
}
