using Kitchen;
using KitchenDeconstructor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace DeconstructorMod
{
    [UpdateBefore(typeof(PickUpAndDropAppliance))]
    internal class DeconstructorPickupSystem : ApplianceInteractionSystem
    {
        private CItemHolder m_Holder;
        private CItemHolder m_CabinetHolder;
        private CApplianceBlueprint m_Blueprint;
        private CForSale m_Sale;
        private CAppliance m_Appliance;
        private CBlueprintStore m_Store;
        protected override InteractionType RequiredType => InteractionType.Grab;
        protected override bool IsPossible(ref InteractionData data)
        {
            return false;
            if (!Require<CItemHolder>(data.Interactor, out m_Holder))
            {
                return false;
            }
            if (!Require<CAppliance>(data.Target, out m_Appliance))
            {
                return false;
            }
            if (!Require<CBlueprintStore>(data.Target, out m_Store))
            {
                return false;
            }
            if (!Require<CItemHolder>(data.Target, out m_CabinetHolder))
            {
                return false;
            }
            if (!m_Store.InUse)
            {
                return false;
            }
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            Mod.LogInfo("PickupAppliance.Perform");
            m_Holder.HeldItem = m_CabinetHolder.HeldItem;
            m_Store.InUse = false;


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
