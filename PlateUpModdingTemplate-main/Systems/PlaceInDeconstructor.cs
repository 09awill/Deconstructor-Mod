using DeconstructorMod.Components;
using Kitchen;
using KitchenData;
using KitchenLib.References;
using Unity.Entities;
using static KitchenDeconstructor.Patches.StorageStructs;

namespace KitchenDeconstructor.Systems
{
    [UpdateBefore(typeof(PickUpAndDropAppliance))]
    [UpdateBefore(typeof(StoreBlueprint))]
    [UpdateBefore(typeof(RetrieveBlueprint))]
    public class PlaceInDeconstructor : ApplianceInteractionSystem
    {
        private CItemHolder m_Holder;
        private CAppliance m_Appliance;
        private CIDeconstruct m_Deconstruct;
        private CStoredTables m_Tables;
        private CStoredPlates m_Plates;
        protected override InteractionType RequiredType => InteractionType.Grab;
        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Require(data.Interactor, out m_Holder))
            {
                return false;
            }
            if (!Require(data.Target, out m_Deconstruct))
            {
                return false;
            }
            if (m_Deconstruct.InUse)
            {
                return false;
            }
            if (m_Holder.HeldItem == Entity.Null)
            {
                return false;
            }
            if (!Require(m_Holder.HeldItem, out m_Appliance))
            {
                return false;
            }
            if (!Require(data.Target, out m_Plates))
            {
                return false;
            }
            if (!Require(data.Target, out m_Tables))
            {
                return false;
            }
            Appliance app = GameData.Main.Get<Appliance>(m_Appliance.ID);
            if ((!app.IsPurchasable && !app.IsPurchasableAsUpgrade) && !Require(m_Holder.HeldItem, out CApplianceBlueprint bp))
            {
                return false;
            }
            if(Require(m_Holder.HeldItem, out CApplianceBlueprint pBP))
            {
                if(pBP.IsCopy)
                {
                    return false;
                }
            }
            if (Has<CApplyDecor>(m_Holder.HeldItem))  // PreventWallpapers/Flooring
            {
                return false;
            }
            if (Require(m_Holder.HeldItem, out CBlueprintStore bpStore))
            {
                if (bpStore.InUse)
                {
                    return false;
                }
            }
            if (Require(m_Holder.HeldItem, out CIDeconstruct deconstruct))
            {
                if (deconstruct.InUse)
                {
                    return false;
                }
            }

            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            if (Require(m_Holder.HeldItem, out CApplianceBlueprint bp))
            {
                PlaceBlueprint(ref data, bp);
            } 
            else
            {
                PlaceAppliance(ref data);
            }
            EntityManager.DestroyEntity(m_Holder.HeldItem);
            data.Context.Set(data.Interactor, default(CItemHolder));

        }
        private void PlaceBlueprint(ref InteractionData pData, CApplianceBlueprint pBlueprint)
        {
            if (Require(m_Holder.HeldItem, out CForSale fs))
            {
                m_Deconstruct.IsDeconstructed = true;
                EntityManager.AddComponent<CIsInactive>(pData.Target);
                m_Deconstruct.InUse = true;
                m_Deconstruct.ApplianceID = pBlueprint.Appliance;
                m_Deconstruct.Price = fs.Price;
                m_Deconstruct.BlueprintID = ApplianceReferences.Blueprint;
                SetComponent(pData.Target, m_Deconstruct);
                CheckNecessaryAppliances(ref pData, pBlueprint.Appliance);
            }
        }
        private void PlaceAppliance(ref InteractionData pData)
        {
            m_Deconstruct.IsDeconstructed = false;
            EntityManager.RemoveComponent<CIsInactive>(pData.Target);
            m_Deconstruct.InUse = true;
            m_Deconstruct.ApplianceID = m_Appliance.ID;
            GameData.Main.TryGet<Appliance>(m_Appliance.ID, out var app);
            m_Deconstruct.Price = PriceUpdater.GetSellPrice(EntityManager, m_Appliance.ID);
            m_Deconstruct.BlueprintID = ApplianceReferences.Blueprint;
            SetComponent(pData.Target, m_Deconstruct);
            CheckNecessaryAppliances(ref pData, m_Appliance.ID);
        }

        private void CheckNecessaryAppliances(ref InteractionData pData, int pApplianceID)
        {

            if (GameData.Main.TryGet<Appliance>(pApplianceID, out var appliance) && appliance.GetProperty<CItemProvider>(out var provider) && provider.DefaultProvidedItem == ItemReferences.Plate)
            {
                m_Plates.PlatesCount += provider.Maximum;
                Set(pData.Target, m_Plates);
            }

            if (appliance.GetProperty<CApplianceTable>(out _))
            {
                m_Tables.Add(appliance.ID);
                Set(pData.Target, m_Tables);
            }
        }
    }
}
