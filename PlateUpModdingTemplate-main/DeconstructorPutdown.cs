using DeconstructorMod;
using Kitchen;
using Kitchen.ShopBuilder;
using KitchenData;
using KitchenIMessedUp;
using KitchenLib.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;
using UnityEngine.UIElements;
using static Kitchen.ShopBuilder.CreateShopOptions;
using static UnityEngine.EventSystems.EventTrigger;

namespace KitchenDeconstructor
{
    [UpdateBefore(typeof(PickUpAndDropAppliance))]
    [UpdateBefore(typeof(StoreBlueprint))]
    [UpdateBefore(typeof(RetrieveBlueprint))]
    internal class DeconstructorPutdown : ApplianceInteractionSystem
    {
        private CItemHolder m_Holder;
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
            if (!Require<CBlueprintStore>(data.Target, out m_Store))
            {
                return false;
            }
            if (!Require<CIDeconstruct>(data.Target, out m_Deconstruct))
            {
                return false;
            }
            if (m_Store.InUse)
            {
                m_Pickup = true;
                if(m_Holder.HeldItem != Entity.Null)
                {
                    return false;
                }
            } else
            {
                m_Pickup = false;
                if(m_Holder.HeldItem == Entity.Null)
                {
                    return false;
                }
                if(!Require<CAppliance>(m_Holder.HeldItem, out m_Appliance))
                {
                    return false;
                }
            }
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            if (m_Pickup)
            {
                if (m_Deconstruct.isDeconstructed)
                {
                    //change to deconstructed
                    m_Appliance = m_Store.ApplianceID;
                    Mod.LogWarning("Starting Pickup");
                    //If Appliance should be blueprint
                    Entity entity = data.Context.CreateEntity();
                    data.Context.Set(entity, new CCreateAppliance
                    {
                        ID = m_Store.BlueprintID
                    });
                    data.Context.Set(entity, GetComponent<CPosition>(data.Interactor));
                    data.Context.Set(entity, new CApplianceBlueprint
                    {
                        Appliance = m_Store.ApplianceID,
                    });
                    Mod.LogWarning("Setting require ping");

                    if (!Preferences.Get<bool>(Pref.RequirePingForBlueprintInfo))
                    {
                        data.Context.Set(entity, new CShowApplianceInfo
                        {
                            Appliance = m_Store.ApplianceID,
                            Price = m_Store.Price,
                            ShowPrice = true
                        });
                    }

                    data.Context.Set(entity, new CForSale
                    {
                        Price = m_Store.Price
                    });
                    data.Context.Set(entity, default(CShopEntity));
                    Mod.LogWarning("Assigning holding references");

                    data.Context.Add<CHeldAppliance>(entity);
                    data.Context.Set(entity, new CHeldBy
                    {
                        Holder = data.Interactor
                    });
                    data.Context.Set(data.Interactor, new CItemHolder
                    {
                        HeldItem = entity
                    });
                    Mod.LogWarning("Setup All of components");
                } else
                {
                    Entity entity = data.Context.CreateEntity();
                    data.Context.Set(entity, new CCreateAppliance
                    {
                        ID = m_Store.ApplianceID
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
                CBlueprintStore bps = new CBlueprintStore()
                {
                    InUse = false,
                    HasBeenCopied = true,
                    HasBeenMadeFree = true,
                    HasBeenUpgraded = true,
                    ApplianceID = 0,
                    BlueprintID = 0,
                    Price = 0
                };
                SetComponent(data.Target, bps);
            }
            else
            {
                if (Require<CApplianceBlueprint>(m_Holder.HeldItem, out CApplianceBlueprint bp))
                {
                    if (Require<CForSale>(m_Holder.HeldItem, out CForSale fs))
                    {
                        Mod.LogWarning("has Appliance blueprint so setting to already deconstructed");
                        m_Deconstruct.isDeconstructed = true;
                        SetComponent(data.Target, m_Deconstruct);
                        EntityManager.AddComponent<CIsInactive>(data.Target);
                        m_Store.InUse = true;
                        m_Store.ApplianceID = bp.Appliance;
                        m_Store.Price = fs.Price;
                        m_Store.BlueprintID = ApplianceReferences.Blueprint;
                        SetComponent(data.Target, m_Store);
                    }
                } else
                {
                    Mod.LogWarning("is Appliance blueprint so setting to not deconstructed");

                    m_Deconstruct.isDeconstructed = false;
                    SetComponent(data.Target, m_Deconstruct);
                    EntityManager.RemoveComponent<CIsInactive>(data.Target);
                    m_Store.InUse = true;
                    m_Store.ApplianceID = m_Appliance.ID;
                    m_Store.Price = 0;
                    m_Store.BlueprintID = ApplianceReferences.Blueprint;
                    SetComponent(data.Target, m_Store);
                }
                EntityManager.DestroyEntity(m_Holder.HeldItem);
                data.Context.Set(data.Interactor, default(CItemHolder));
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
