using DeconstructorMod;
using IngredientLib.Util;
using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using MessagePack;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
            new CIsInteractive(), new CIDeconstruct(), KitchenPropertiesUtils.GetCTakesDuration(5f, 0, false, true, false, DurationToolType.None, InteractionMode.Items, false, true, true, true,0), KitchenPropertiesUtils.GetCDisplayDuration(false, Mod.DeconstructProcess.ID, true)
        };
        public override void OnRegister(GameDataObject gameDataObject)
        {
            /*
            
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
            deconstructorView.Animator = Prefab.GetComponent<Animator>();
            deconstructorView.ObjectMesh = Prefab.GetChildFromPath("WarehouseCabinet/Lorry");

            TextMeshPro tmp = Prefab.GetChild("Blueprint/Name").GetComponent<TextMeshPro>();
            tmp.material = MaterialUtils.GetExistingMaterial("Cake n Truffles Atlas Material_0");
            tmp.font = FontUtils.GetExistingTMPFont("Blueprint");
            Mod.LogWarning(tmp.font);
            
            deconstructorView.Title = tmp;
            deconstructorView.Blueprint = Prefab.GetChild("Blueprint");
            deconstructorView.BlueprintRenderer = Prefab.GetChildFromPath("Blueprint/Blueprint/Cube.001").GetComponent<MeshRenderer>();




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
            */

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

                [Key(4)]
                public int Appliance;

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
            public Animator Animator;

            public GameObject ObjectMesh;
            //blueprints
            public TextMeshPro Title;
            public GameObject Blueprint;
            public MeshRenderer BlueprintRenderer;

            public ViewData Data;
            private static readonly int IsActive = Animator.StringToHash("IsActive");
            protected override void UpdateData(ViewData data)
            {
                if (!data.InUse)
                {
                    Animator.SetBool(IsActive, value: false);
                    return;
                }
                Animator.SetBool(IsActive, value: true);
                if (data.InUse)
                {
                    if (data.IsDeconstructing)
                    {
                        ObjectMesh.SetActive(false);
                        Blueprint.SetActive(true);
                    }
                    else
                    {
                        ObjectMesh.SetActive(true);
                        Blueprint.SetActive(false);
                    }
                }

                if (Data.Appliance != data.Appliance && GameData.Main.TryGet<Appliance>(data.Appliance, out var output))
                {
                    if (Renderer != null)
                    {
                        BlueprintRenderer.material.SetTexture("_Image", PrefabSnapshot.GetSnapshot(output.Prefab));
                    }

                    if (Title != null)
                    {
                        Title.text = output.Name;
                    }

                }
                if (data.IsDeconstructing && !Data.IsDeconstructing && Data.IsDay)
                {
                    UpgradeEffect?.SendEvent("BurstDeconstruct");
                    AudioSource.Play();
                }
                Data = data;
            }

        }
    }
    

}
