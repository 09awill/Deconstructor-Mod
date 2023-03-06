using Kitchen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static KitchenDeconstructor.Deconstructor;

namespace KitchenDeconstructor
{
    public class DeconstructViewSystem : ViewSystemBase
    {
        EntityQuery m_DeconstructViewQuery;
        protected override void Initialise()
        {
            base.Initialise();
            m_DeconstructViewQuery = GetEntityQuery(new QueryHelper().All(typeof(CIDeconstruct), typeof(CBlueprintStore), typeof(CLinkedView)));
        }
        protected override void OnUpdate()
        {
            using var views = m_DeconstructViewQuery.ToComponentDataArray<CLinkedView>(Allocator.Temp);
            using var deconstructs = m_DeconstructViewQuery.ToComponentDataArray<CIDeconstruct>(Allocator.Temp);
            using var blueprintStores = m_DeconstructViewQuery.ToComponentDataArray<CBlueprintStore>(Allocator.Temp);
            bool isDay = HasSingleton<SIsDayTime>();

            for (int i = 0; i < views.Length;i++)
            {
                var deconstruct = deconstructs[i];
                var blueprintStore = blueprintStores[i];
                SendUpdate(views[i], new DeconstructorView.ViewData
                {
                    InUse = blueprintStore.InUse,
                    IsDeconstructing = deconstruct.isDeconstructed,
                    HasDeconstructEvent = deconstruct.isDeconstructed,
                    IsDay = isDay,
                });
            }
            
            
        }
    }
}
