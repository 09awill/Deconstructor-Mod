using IngredientLib.Util;
using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using MessagePack;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.VFX;

namespace KitchenDeconstructor
{
    public class Deconstructor : CustomAppliance
    {

        public override string UniqueNameID => "Deconstructor";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("Deconstructor");
        public override PriceTier PriceTier => PriceTier.Medium;
        public override bool SellOnlyAsDuplicate => false;
        public override bool IsPurchasable => true;
        public override ShoppingTags ShoppingTags => ShoppingTags.BlueprintStore | ShoppingTags.Misc;
        public override bool IsNonInteractive => false;

        static FieldInfo Animator = ReflectionUtils.GetField<BlueprintStoreView>("Animator", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo Title = ReflectionUtils.GetField<BlueprintStoreView>("Title", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo Renderer = ReflectionUtils.GetField<BlueprintStoreView>("Renderer", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo IsMakingFree = ReflectionUtils.GetField<BlueprintStoreView>("IsMakingFree", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo IsUpgrading = ReflectionUtils.GetField<BlueprintStoreView>("IsUpgrading", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo IsCopying = ReflectionUtils.GetField<BlueprintStoreView>("IsCopying", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo CopyBlueprint = ReflectionUtils.GetField<BlueprintStoreView>("CopyBlueprint", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo CopyTitle = ReflectionUtils.GetField<BlueprintStoreView>("CopyTitle", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo CopyRenderer = ReflectionUtils.GetField<BlueprintStoreView>("CopyRenderer", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo CopyBlueprintMaterial = ReflectionUtils.GetField<BlueprintStoreView>("CopyBlueprintMaterial", BindingFlags.NonPublic | BindingFlags.Instance);
        
        //static FieldInfo pushObject = ReflectionUtils.GetField<BlueprintStoreView>("PushObject", BindingFlags.NonPublic | BindingFlags.Instance);



        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            ( Locale.English, LocalisationUtils.CreateApplianceInfo("Deconstructor", "Allows you to deconstruct your kitchen appliances", new(), new()) )
        };
        public override List<IApplianceProperty> Properties => new List<IApplianceProperty>()
        {
            new CBlueprintStore() { HasBeenCopied = true, HasBeenMadeFree = true, HasBeenUpgraded = true }, new CIsInteractive(), new CIDeconstruct(), new CTakesDuration() { Total = 5f, Active = false, Manual = true, PreserveProgress = true, RequiresRelease = true}, new CDisplayDuration()
        };
        public override void OnRegister(GameDataObject gameDataObject)
        {
            BlueprintStoreView view = Prefab.AddComponent<BlueprintStoreView>();
            
            DeconstructorView deconstructorView = Prefab.AddComponent<DeconstructorView>();
            VisualEffect vfx = Prefab.GetChildFromPath("VFX/Deconstruct").GetComponent<VisualEffect>();
            vfx.visualEffectAsset = Mod.Bundle.LoadAsset<VisualEffectAsset>("VFX_Deconstruct");
            deconstructorView.UpgradeEffect = vfx;
            deconstructorView.CabinetBase = Prefab.GetChildFromPath("WarehouseCabinet/Cabinet Body");
            AudioSource source = Prefab.GetChildFromPath("VFX/Deconstruct").AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.clip = Mod.Bundle.LoadAsset<AudioClip>("deconstructSoundEffect");
            deconstructorView.AudioClip = source.clip;
            deconstructorView.AudioSource = source;


            Animator.SetValue(view, Prefab.GetComponent<Animator>());
            TextMeshPro tmp = Prefab.GetChild("Blueprint/Name").AddComponent<TextMeshPro>();
            tmp.fontSize = 120;
            tmp.fontSizeMax = 120;
            tmp.fontSizeMin = 18;
            tmp.enableAutoSizing = true;
            tmp.enableWordWrapping = true;
            tmp.enableKerning = true;
            tmp.wordWrappingRatios = 0.4f;

            Title.SetValue(view, tmp);

            Renderer.SetValue(view, Prefab.GetChildFromPath("Blueprint/Blueprint/Cube.001").GetComponent<MeshRenderer>());
            IsMakingFree.SetValue(view, Prefab.GetChildFromPath("Indicators/DiscountDesk"));
            IsUpgrading.SetValue(view, Prefab.GetChildFromPath("Indicators/ResearchFlasky"));
            IsCopying.SetValue(view, Prefab.GetChildFromPath("Indicators/CopyDesk (1)"));
            CopyBlueprint.SetValue(view, Prefab.GetChildFromPath("Blueprint Copy"));

            tmp = Prefab.GetChild("Blueprint Copy/Name").AddComponent<TextMeshPro>();
            tmp.fontSize = 0;
            
            CopyTitle.SetValue(view, tmp);
            CopyRenderer.SetValue(view, Prefab.GetChildFromPath("Blueprint Copy/Blueprint/Cube.001").GetComponent<MeshRenderer>());
            CopyBlueprintMaterial.SetValue(view, Prefab.GetChildFromPath("Blueprint Copy/Blueprint/Cube").GetComponent<MeshRenderer>());



            

            Material[] mats = new Material[] { MaterialUtils.GetExistingMaterial("Metal - Soft Green Paint"), MaterialUtils.GetExistingMaterial("Metal Very Dark") };
            Prefab.GetChildFromPath("WarehouseCabinet/Cabinet Body").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Wood 2")};
            Prefab.GetChildFromPath("WarehouseCabinet/Crates").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Plastic - Orange"), MaterialUtils.GetExistingMaterial("Plastic"), MaterialUtils.GetExistingMaterial("Plastic - Black") };
            Prefab.GetChildFromPath("WarehouseCabinet/Lorry").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Paper") };
            Prefab.GetChildFromPath("WarehouseCabinet/Papers").ApplyMaterial(mats);
            
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Blueprint Light") };
            Prefab.GetChildFromPath("Blueprint/Blueprint/Cube").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Flat Image - Faded") };
            Prefab.GetChildFromPath("Blueprint/Blueprint/Cube.001").ApplyMaterial(mats);


            mats = new Material[] { MaterialUtils.GetExistingMaterial("Cake n Truffles Atlas Material_0") };
            Prefab.GetChildFromPath("Blueprint/Name").ApplyMaterial(mats);
            Prefab.GetChildFromPath("Blueprint/Labels/Rarity").ApplyMaterial(mats);

            
            mats = new Material[] { MaterialUtils.GetExistingMaterial("fontello Atlas Material") };
            Prefab.GetChildFromPath("Blueprint/Labels/Unit").ApplyMaterial(mats);
          
            /*
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Blueprint Light") };
            Prefab.GetChildFromPath("Blueprint Copy/Blueprint/Cube").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Flat Image - Faded") };
            Prefab.GetChildFromPath("Blueprint Copy/Blueprint/Cube.001").ApplyMaterial(mats);

            mats = new Material[] { MaterialUtils.GetExistingMaterial("Cake n Truffles Atlas Material_0") };
            Prefab.GetChildFromPath("Blueprint Copy/Name").ApplyMaterial(mats);
            Prefab.GetChildFromPath("Blueprint Copy/Labels/Rarity").ApplyMaterial(mats);

            mats = new Material[] { MaterialUtils.GetExistingMaterial("fontello Atlas Material") };
            Prefab.GetChildFromPath("Blueprint Copy/Labels/Unit").ApplyMaterial(mats);
            */

            mats = new Material[] { MaterialUtils.GetExistingMaterial("Plastic - Purple") };
            Prefab.GetChildFromPath("Indicators/ResearchFlask/Cube").ApplyMaterial(mats);

            mats = new Material[] { MaterialUtils.GetExistingMaterial("Oven Glass") };
            Prefab.GetChildFromPath("Indicators/ResearchFlask/Cube.001").ApplyMaterial(mats);

            mats = new Material[] { MaterialUtils.GetExistingMaterial("Plastic - Grey"), MaterialUtils.GetExistingMaterial("Plastic - Dark Grey") };
            Prefab.GetChildFromPath("Indicators/CopyDesk (1)/Cube").ApplyMaterial(mats);

            mats = new Material[] { MaterialUtils.GetExistingMaterial("Wood - Default") };
            Prefab.GetChildFromPath("Indicators/CopyDesk (1)/Desk Legs").ApplyMaterial(mats);

            mats = new Material[] { MaterialUtils.GetExistingMaterial("Wood - Dark") };
            Prefab.GetChildFromPath("Indicators/CopyDesk (1)/Desk Top").ApplyMaterial(mats);


            mats = new Material[] { MaterialUtils.GetExistingMaterial("Wood 2") };
            Prefab.GetChildFromPath("Indicators/DiscountDesk/Crates").ApplyMaterial(mats);

            mats = new Material[] { MaterialUtils.GetExistingMaterial("Plastic - Orange"), MaterialUtils.GetExistingMaterial("Plastic"), MaterialUtils.GetExistingMaterial("Plastic - Black") };
            Prefab.GetChildFromPath("Indicators/DiscountDesk/Cube").ApplyMaterial(mats);

            mats = new Material[] { MaterialUtils.GetExistingMaterial("Money") };
            Prefab.GetChildFromPath("Indicators/DiscountDesk/Cylinder").ApplyMaterial(mats);

            mats = new Material[] { MaterialUtils.GetExistingMaterial("Wood - Default") };
            Prefab.GetChildFromPath("Indicators/DiscountDesk/Desk Legs").ApplyMaterial(mats);

            mats = new Material[] { MaterialUtils.GetExistingMaterial("Wood - Dark") };
            Prefab.GetChildFromPath("Indicators/DiscountDesk/Desk Top").ApplyMaterial(mats);
            

            //DestroyItemsOvernight
            //CDestroyItemAtDay
        }

        public class DeconstructorView : UpdatableObjectView<DeconstructorView.ViewData>
        {
            [MessagePackObject(false)]
            public struct ViewData : ISpecificViewData, IViewData, IViewResponseData, IViewData.ICheckForChanges<ViewData>
            {
                [Key(0)]
                public bool InUse;

                [Key(1)]
                public bool IsDeconstructing;

                [Key(2)]
                public bool HasDeconstructEvent;

                [Key(3)]
                public bool IsDay;

                public bool IsChangedFrom(ViewData check)
                {
                    return InUse != check.InUse || IsDeconstructing != check.IsDeconstructing || HasDeconstructEvent != check.HasDeconstructEvent || IsDay != check.IsDay;
                }

                public IUpdatableObject GetRelevantSubview(IObjectView view)
                {
                    return view.GetSubView<DeconstructorView>();
                }
            }
            public VisualEffect UpgradeEffect;
            public GameObject CabinetBase;
            public AudioClip AudioClip;
            public AudioSource AudioSource;

            public ViewData Data;
            protected override void UpdateData(ViewData data)
            {
                if(data.IsDeconstructing && !Data.IsDeconstructing && Data.IsDay)
                {
                    UpgradeEffect?.SendEvent("BurstDeconstruct");
                    AudioSource.Play();
                }
                Data = data;
            }

        }
    }
    

}
