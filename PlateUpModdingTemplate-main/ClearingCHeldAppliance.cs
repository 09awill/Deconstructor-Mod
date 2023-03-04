using DeconstructorMod;
using Kitchen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;

namespace KitchenDeconstructor
{
    internal class ClearingCHeldAppliance : NightSystem
    {
        EntityQuery m_ApplianceQuery;
        protected override void Initialise()
        {
            Mod.LogWarning("Initialised Clearing held appliance");
            base.Initialise();
            m_ApplianceQuery = GetEntityQuery(new QueryHelper().All(typeof(CItemHolder), typeof(CIsInteractive), typeof(CIDeconstruct)));
        }
        protected override void OnUpdate()
        {
            /*
            NativeArray<Entity> entities = m_ApplianceQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in entities)
            {
                if (Require(entity, out CIDeconstruct deconstruct))
                {
                    if (Require(entity, out CItemHolder h))
                    {
                        Mod.LogWarning("REMOVING HELD COMPONENT");
                        EntityManager.RemoveComponent<CHeldAppliance>(h.HeldItem);
                        EntityManager.RemoveComponent<CDestroyApplianceAtDay>(h.HeldItem);
                        EntityManager.RemoveComponent<CApplianceBlueprint>(h.HeldItem);
                        EntityManager.RemoveComponent<CForSale>(h.HeldItem);
                        EntityManager.RemoveComponent<CShopEntity>(h.HeldItem);
                        EntityManager.RemoveComponent<CHeldBy>(h.HeldItem);
                        EntityManager.RemoveComponent<CDestroyApplianceAtDay>(h.HeldItem);
                    }
                }
            }

            entities.Dispose();
            */
        }
    }
}
