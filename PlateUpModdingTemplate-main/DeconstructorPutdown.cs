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

        private EntityQuery m_RebuyChances;
        private EntityQuery m_RefreshLetterChances;
        protected override InteractionType RequiredType => InteractionType.Grab;

        protected override void Initialise()
        {
            m_RebuyChances = GetEntityQuery(typeof(CBlueprintRebuyableChance));
            m_RefreshLetterChances = GetEntityQuery(typeof(CBlueprintRefreshChance));
            base.Initialise();
        }
        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Require<CItemHolder>(data.Interactor, out m_Holder))
            {
                //Mod.LogInfo("Attempted Deconstructor put down but couldn't find ");
                return false;
            }
            if (!Require<CBlueprintStore>(data.Target, out m_Store))
            {
                //Mod.LogInfo("Attempted Deconstructor put down but couldn't find blueprint store on the cabinet");
                return false;
            }
            if (!Require<CIDeconstruct>(data.Target, out m_Deconstruct))
            {
                //Mod.LogInfo("Attempted Deconstructor put down but couldn't find blueprint store on the cabinet");
                return false;
            }
            if (m_Store.InUse)
            {
                m_Pickup = true;
                if(m_Holder.HeldItem != Entity.Null)
                {
                    return false;
                }
                //Mod.LogInfo("Attempted Deconstructor put down but store was in use");
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
                if (Has<CApplianceBlueprint>(m_Holder.HeldItem))
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
                    IsCopy = m_Store.HasBeenCopied
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

                Mod.LogWarning("Starting Store Setup");

                if (m_Store.HasBeenCopied)
                {
                    m_Store.HasBeenCopied = false;
                }
                else
                {
                    m_Store.InUse = false;
                    m_Store.ApplianceID = 0;
                    m_Store.Price = 0;
                    m_Store.BlueprintID = 0;
                }


                m_Store.HasBeenMadeFree = false;
                SetComponent(data.Target, m_Store);
                Mod.LogWarning("Setup All of components");

                //Buy appliance

                NativeArray<CBlueprintRebuyableChance> nativeArray = m_RebuyChances.ToComponentDataArray<CBlueprintRebuyableChance>(Allocator.Temp);
                NativeArray<CBlueprintRefreshChance> nativeArray2 = m_RefreshLetterChances.ToComponentDataArray<CBlueprintRefreshChance>(Allocator.Temp);
                EntityManager.RemoveComponent<CForSale>(entity);
                EntityManager.RemoveComponent<CShowApplianceInfo>(entity);
                EntityManager.RemoveComponent<CApplianceBlueprint>(entity);
                EntityManager.RemoveComponent<CShopEntity>(entity);
                EntityManager.AddComponent<CRemoveView>(entity);
                Set(entity, new CRequiresView
                {
                    Type = ViewType.Appliance
                });
                Set(entity, new CCreateAppliance
                {
                    ID = m_Appliance.ID
                });


                float rebuyable_chance = 0f;
                foreach (CBlueprintRebuyableChance item in nativeArray)
                {
                    rebuyable_chance += (1f - rebuyable_chance) * item.Chance;
                }

                float refresh_chance = 0f;
                foreach (CBlueprintRefreshChance item2 in nativeArray2)
                {
                    refresh_chance += (1f - refresh_chance) * item2.Chance;
                }

                /*

                if (!Blueprint.IsCopy && Random.value < rebuyable_chance)
                {
                    Entity e2 = EntityManager.CreateEntity();
                    Set(e2, new CCreateAppliance
                    {
                        ID = Blueprint.Appliance
                    });
                    Set(e2, new CPosition(Position));
                    Set(e2, new CApplianceBlueprint
                    {
                        Appliance = Blueprint.Appliance,
                    });
                    if (Has<CShowApplianceInfo>(data.Target))
                    {
                        Set(e2, new CShowApplianceInfo
                        {
                            Appliance = Blueprint.Appliance,
                            Price = Sale.Price,
                            ShowPrice = true
                        });
                    }
                    Set(e2, new CForSale
                    {
                        Price = Sale.Price
                    });
                    Set(e2, default(CShopEntity));
                }

                if (!Blueprint.IsCopy && Random.value < refresh_chance)
                {
                    Entity e3 = EntityManager.CreateEntity();
                    Set(e3, new CNewShop
                    {
                        Tags = (ShoppingTags.Technology | ShoppingTags.FrontOfHouse | ShoppingTags.Plumbing | ShoppingTags.Cooking | ShoppingTags.Automation | ShoppingTags.Misc | ShoppingTags.Office),
                        FixedLocation = true,
                        Location = Position.Position.Rounded()
                    });
                }
                */

                nativeArray.Dispose();
                nativeArray2.Dispose();


                /*
                Entity entity = data.Context.CreateEntity();
                data.Context.set

                Mod.LogInfo("Picking up Appliance from Deconstructor");
                Set<CPosition>(m_CabinetHolder.HeldItem, CPosition.Hidden);
                Set<CItemHolder>(data.Interactor, new CItemHolder() { HeldItem = m_CabinetHolder.HeldItem });
                EntityManager.RemoveComponent<CPreservedOvernight>(m_CabinetHolder.HeldItem);
                //EntityManager.RemoveComponent<CCreateAppliance>(m_CabinetHolder.HeldItem);
                */
            }
            else
            {
                m_Store.InUse = true;
                m_Store.ApplianceID = m_Appliance.ID;
                m_Store.Price = 0;
                m_Store.BlueprintID = ApplianceReferences.Blueprint;
                SetComponent(data.Target, m_Store);
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
