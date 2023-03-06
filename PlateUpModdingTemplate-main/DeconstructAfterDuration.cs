using Kitchen;
using KitchenDeconstructor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;

namespace KitchenDeconstructor
{
    public class DeconstructAfterDuration : DaySystem
    {
        private EntityQuery m_ApplianceQuery;
        protected override void Initialise()
        {
            base.Initialise();
            m_ApplianceQuery = GetEntityQuery(new QueryHelper().All(typeof(CAppliance), typeof(CIDeconstruct)));
        }
        protected override void OnUpdate()
        {
            NativeArray<Entity> entities = m_ApplianceQuery.ToEntityArray(Allocator.Temp);
            
            foreach (Entity entity in entities)
            {
                if (Require<CIsInactive>(entity, out CIsInactive comp)) continue;
                if (Require(entity, out CIDeconstruct deconstruct))
                {
                    if(deconstruct.isDeconstructed) EntityManager.AddComponent<CIsInactive>(entity);
                    if (Require<CTakesDuration>(entity, out CTakesDuration duration))
                    {

                        if (duration.Remaining > 0f || !duration.Active)
                        {
                            continue;
                        }
                        deconstruct.isDeconstructed = true;
                        EntityManager.SetComponentData(entity, deconstruct);
                        duration.IsLocked = true;
                        duration.Active = false;
                        
                        EntityManager.SetComponentData(entity, duration);
                        EntityManager.AddComponent<CIsInactive>(entity);
                    }
                }
            }
            entities.Dispose();
        }
    }
}
