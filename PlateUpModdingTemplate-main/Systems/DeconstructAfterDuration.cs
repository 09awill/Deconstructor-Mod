﻿using DeconstructorMod.Components;
using Kitchen;
using Unity.Collections;
using Unity.Entities;

namespace KitchenDeconstructor.Systems
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
            var moneyFromDeconstructing = 0;
            foreach (Entity entity in entities)
            {
                if (Require(entity, out CIsInactive comp)) continue;
                if (Require(entity, out CIDeconstruct deconstruct))
                {
                    if (deconstruct.IsDeconstructed) EntityManager.AddComponent<CIsInactive>(entity);
                    if (Require(entity, out CTakesDuration duration))
                    {

                        if (duration.Remaining > 0f || !duration.Active)
                        {
                            continue;
                        }
                        deconstruct.IsDeconstructed = true;
                        EntityManager.SetComponentData(entity, deconstruct);
                        duration.IsLocked = true;
                        duration.Active = false;
                        EntityManager.SetComponentData(entity, duration);
                        EntityManager.AddComponent<CIsInactive>(entity);
                        var percentMultiplier = 100f / (float)Mod.PrefManager.Get<int>(Mod.RETURN_MONEY_PERCENTAGE_ID);
                        var price = (float)deconstruct.Price / percentMultiplier;
                        moneyFromDeconstructing += (int)price;

                    }
                }
            }
            if(Mod.PrefManager.Get<bool>(Mod.RETURN_MONEY_ID)) SetSingleton<SMoney>(GetSingleton<SMoney>() + moneyFromDeconstructing);
            entities.Dispose();
        }
    }
}
