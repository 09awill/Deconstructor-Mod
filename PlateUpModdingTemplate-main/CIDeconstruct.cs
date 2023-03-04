using Kitchen;
using KitchenData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace DeconstructorMod
{
    public struct CIDeconstruct : IComponentData, IApplianceProperty, IAttachableProperty
    {
        public bool isDeconstructed;

    }
}
