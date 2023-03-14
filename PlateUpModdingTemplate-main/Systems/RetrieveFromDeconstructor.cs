using DeconstructorMod.Components;
using Kitchen;
using KitchenData;
using KitchenDeconstructor;
using KitchenLib.References;
using Unity.Entities;
using static KitchenDeconstructor.Patches.StorageStructs;

namespace KitchenDeconstructor.Systems
{
    [UpdateBefore(typeof(PickUpAndDropAppliance))]
    [UpdateBefore(typeof(StoreBlueprint))]
    [UpdateBefore(typeof(RetrieveBlueprint))]
    public class RetrieveFromDeconstructor : ApplianceInteractionSystem
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
                if (m_Holder.HeldItem != Entity.Null)
                {
                    return false;
                }
            }
            if (!Require(data.Target, out m_Plates))
            {
                return false;
            }
            if (!Require(data.Target, out m_Tables))
            {
                return false;
            }
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            if (m_Deconstruct.IsDeconstructed)
            {
                m_Appliance = m_Deconstruct.ApplianceID;
                Entity entity = data.Context.CreateEntity();
                data.Context.Set(entity, new CCreateAppliance
                {
                    ID = m_Deconstruct.BlueprintID
                });
                data.Context.Set(entity, GetComponent<CPosition>(data.Interactor));
                data.Context.Set(entity, new CApplianceBlueprint
                {
                    Appliance = m_Deconstruct.ApplianceID,
                });

                if (!Preferences.Get<bool>(Pref.RequirePingForBlueprintInfo))
                {
                    data.Context.Set(entity, new CShowApplianceInfo
                    {
                        Appliance = m_Deconstruct.ApplianceID,
                        Price = m_Deconstruct.Price,
                        ShowPrice = true
                    });
                }

                data.Context.Set(entity, new CForSale
                {
                    Price = m_Deconstruct.Price
                });
                data.Context.Set(entity, default(CShopEntity));

                data.Context.Add<CHeldAppliance>(entity);
                data.Context.Set(entity, new CHeldBy
                {
                    Holder = data.Interactor
                });
                data.Context.Set(data.Interactor, new CItemHolder
                {
                    HeldItem = entity
                });
            }
            else
            {
                Entity entity = data.Context.CreateEntity();
                data.Context.Set(entity, new CCreateAppliance
                {
                    ID = m_Deconstruct.ApplianceID
                });

                data.Context.Set(entity, GetComponent<CPosition>(data.Interactor));
                data.Context.Add<CHeldAppliance>(entity);

                data.Context.Set(entity, new CHeldBy
                {
                    Holder = data.Interactor
                });
                data.Context.Set(data.Interactor, new CItemHolder
                {
                    HeldItem = entity
                });
            }
            if (GameData.Main.TryGet<Appliance>(m_Deconstruct.ApplianceID, out var appliance) && appliance.GetProperty<CItemProvider>(out var provider) && provider.DefaultProvidedItem == ItemReferences.Plate)
            {
                m_Plates.PlatesCount -= provider.Maximum;
                Set(data.Target, m_Plates);
            }

            if (appliance.GetProperty<CApplianceTable>(out _))
            {
                m_Tables.Remove(appliance.ID);
                Set(data.Target, m_Tables);
            }
            CIDeconstruct deconstruct = new CIDeconstruct()
            {
                InUse = false,
                ApplianceID = 0,
                BlueprintID = 0,
                Price = 0
            };
            SetComponent(data.Target, deconstruct);
            EntityManager.AddComponent<CIsInactive>(data.Target);


        }
    }
}
