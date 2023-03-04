using DeconstructorMod;
using Kitchen;
using KitchenDeconstructor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;

namespace KitchenDeconstructor
{
    internal class DeconstructAfterDuration : DaySystem
    {
        private EntityQuery m_ApplianceQuery;
        protected override void Initialise()
        {
            //Mod.LogWarning("Initialised Deconstruct after duration");
            base.Initialise();
            m_ApplianceQuery = GetEntityQuery(new QueryHelper().All(typeof(CAppliance), typeof(CItemHolder), typeof(CIsInteractive), typeof(CIDeconstruct)));
        }
        protected override void OnUpdate()
        {
            NativeArray<Entity> entities = m_ApplianceQuery.ToEntityArray(Allocator.Temp);
            
            foreach (Entity entity in entities)
            {
                if(Require(entity, out CIDeconstruct deconstruct))
                {
                    deconstruct.isDeconstructed= true;
                }
            }
            
            entities.Dispose();
        }
    }
}
