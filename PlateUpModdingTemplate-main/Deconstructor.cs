using IngredientLib.Util;
using Kitchen;
using KitchenData;
using KitchenDeconstructor;
using KitchenLib.Customs;
using KitchenLib.References;
using KitchenLib.Utils;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;
using static KitchenData.Appliance;

namespace DeconstructorMod
{
    internal class Deconstructor : CustomAppliance
    {

        public override string UniqueNameID => "Deconstructor";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("Deconstructor");
        public override PriceTier PriceTier => PriceTier.Medium;
        public override bool SellOnlyAsDuplicate => true;
        public override bool IsPurchasable => true;
        public override ShoppingTags ShoppingTags => ShoppingTags.Cooking | ShoppingTags.Misc;

        public override List<ApplianceProcesses> Processes => new List<ApplianceProcesses>()
        {
            new ApplianceProcesses
            {
                Process = Mod.Deconstruct
            }
        };
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            ( Locale.English, LocalisationUtils.CreateApplianceInfo("Deconstructor", "Allows you to deconstruct your kitchen appliances", new(), new()) )
        };
        //add back blueprint store
        public override List<IApplianceProperty> Properties => new List<IApplianceProperty>()
        {
            new CBlueprintStore(), new CItemHolder(), KitchenPropertiesUtils.GetCItemHolderFilter(ItemCategory.Crates, true, false), KitchenPropertiesUtils.GetCItemHolderFilter(ItemCategory.NonLoadoutCrate, true, false), new CTakesDuration(), new CIsInteractive(), new CIDeconstruct(), new CPreservesContentsOvernight()
        };
        public override void OnRegister(GameDataObject gameDataObject)
        {
            Material[] mats = new Material[] { MaterialUtils.GetExistingMaterial("Metal - Soft Green Paint"), MaterialUtils.GetExistingMaterial("Metal Very Dark") };
            Prefab.GetChild("Cabinet Body").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Blueprint Light") };
            Prefab.GetChild("Cube").ApplyMaterial(mats);
            Prefab.GetChild("Cube.002").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Flat Image - Faded") };
            Prefab.GetChild("Cube.001").ApplyMaterial(mats);
            Prefab.GetChild("Cube.003").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Paper") };
            Prefab.GetChild("Papers").ApplyMaterial(mats);
            //DestroyItemsOvernight
            //CDestroyItemAtDay
        }
    }

}
