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
            new CIsInteractive(), new CIDeconstruct(), new CTakesDuration(){ Total = 5f, Manual = true, ManualNeedsEmptyHands = false, IsInverse = true, Mode = InteractionMode.Items, PreserveProgress = true, IsLocked = true}, KitchenPropertiesUtils.GetCDisplayDuration(false, Mod.DeconstructProcess.ID, false), new CLockDurationTimeOfDay(){ LockDuringNight = true, LockDuringDay = false }
        };
        public override void OnRegister(GameDataObject gameDataObject)
        {
            Mod.LogWarning("Started");
            DeconstructorView deconstructorView = Prefab.AddComponent<DeconstructorView>();

            foreach(GameObject go in Prefab.GetChildren())
            {
                Mod.LogWarning(go.name);
            }
            VisualEffect vfx = Prefab.GetChildFromPath("VFX/Deconstruct").GetComponent<VisualEffect>();

            vfx.visualEffectAsset = Mod.Bundle.LoadAsset<VisualEffectAsset>("VFX_Deconstruct");
            Mod.LogWarning("Got Visual Effect Asset");

            deconstructorView.UpgradeEffect = vfx;
            AudioSource source = Prefab.GetChildFromPath("VFX/Deconstruct").AddComponent<AudioSource>();
            source.playOnAwake = false;
            Mod.LogWarning("Added VFX");

            source.clip = Mod.Bundle.LoadAsset<AudioClip>("deconstructSoundEffect");
            deconstructorView.AudioClip = source.clip;
            deconstructorView.AudioSource = source;
            deconstructorView.Animator = Prefab.GetComponent<Animator>();
            deconstructorView.ObjectMesh = Prefab.GetChild("Box");

            TextMeshPro tmp = Prefab.GetChild("PaperBack/Name").GetComponent<TextMeshPro>();
            tmp.material = MaterialUtils.GetExistingMaterial("Cake n Truffles Atlas Material_0");
            tmp.font = FontUtils.GetExistingTMPFont("Blueprint");
            Mod.LogWarning(tmp.font);
            
            deconstructorView.Title = tmp;
            deconstructorView.Blueprint = Prefab.GetChild("PaperBack");
            deconstructorView.BlueprintRenderer = Prefab.GetChildFromPath("PaperBack/Paper").GetComponent<MeshRenderer>();

            Mod.LogWarning("Halfway There");


            Material[] mats = new Material[] { MaterialUtils.GetExistingMaterial("Metal - Soft Green Paint") };
            Prefab.GetChild("Deconstructor").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Metal Very Dark") };
            Prefab.GetChild("Feet").ApplyMaterial(mats);
            Prefab.GetChild("Crusher").ApplyMaterial(mats);
            Prefab.GetChildFromPath("Crusher/Cube.005").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Wood 2")};
            Prefab.GetChild("Box").ApplyMaterial(mats);


            Mod.LogWarning("Halfway There Again");

            mats = new Material[] { MaterialUtils.GetExistingMaterial("Blueprint Light") };
            Prefab.GetChild("PaperBack").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Flat Image - Faded") };
            Prefab.GetChildFromPath("PaperBack/Paper").ApplyMaterial(mats);


            mats = new Material[] { MaterialUtils.GetExistingMaterial("Cake n Truffles Atlas Material_0") };
            Prefab.GetChildFromPath("PaperBack/Name").ApplyMaterial(mats);
            //Prefab.GetChildFromPath("Blueprint/Labels/Rarity").ApplyMaterial(mats);

            
            //mats = new Material[] { MaterialUtils.GetExistingMaterial("fontello Atlas Material") };
            //Prefab.GetChildFromPath("Blueprint/Labels/Unit").ApplyMaterial(mats);
            

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
                public bool IsDeconstructed;

                [Key(2)]
                public bool HasDeconstructEvent;

                [Key(3)]
                public bool IsDay;

                [Key(4)]
                public int Appliance;
                [Key(5)]
                public bool Deconstructing;

                [Key(6)]
                public float DeconstructionProgress;

                public bool IsChangedFrom(ViewData check)
                {
                    return InUse != check.InUse || IsDeconstructed != check.IsDeconstructed || HasDeconstructEvent != check.HasDeconstructEvent || IsDay != check.IsDay || Deconstructing != check.Deconstructing || DeconstructionProgress != check.DeconstructionProgress;
                }

                public IUpdatableObject GetRelevantSubview(IObjectView view)
                {
                    return view.GetSubView<DeconstructorView>();
                }
            }
            public VisualEffect UpgradeEffect;
            public AudioClip AudioClip;
            public AudioSource AudioSource;
            public Animator Animator;

            public GameObject ObjectMesh;
            //blueprints
            public TextMeshPro Title;
            public GameObject Blueprint;
            public MeshRenderer BlueprintRenderer;

            public ViewData Data;
            private static readonly int ObjectPlacedBool = Animator.StringToHash("ObjectPlacedBool");
            private static readonly int CrushObjectBool = Animator.StringToHash("CrushObjectBool");
            private static readonly int PlaceBlueprintBool = Animator.StringToHash("PlaceBlueprintBool");
            private static readonly int CrushObjectTime = Animator.StringToHash("DeconstructSpeed");

            bool wasDefault = false;
            bool wasObjectPlaced = false;
            bool wasCrush = false;
            bool wasPrint = false;
            protected override void UpdateData(ViewData data)
            {
                if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Default") != wasDefault)
                {
                    Mod.LogWarning($"Default State {!wasDefault}");
                    wasDefault = Animator.GetCurrentAnimatorStateInfo(0).IsName("Default");

                }
                if (Animator.GetCurrentAnimatorStateInfo(0).IsName("ObjectPlaced") != wasObjectPlaced)
                {
                    Mod.LogWarning($"ObjectPlaced State {!wasObjectPlaced}");
                    wasObjectPlaced = Animator.GetCurrentAnimatorStateInfo(0).IsName("ObjectPlaced");

                }
                if (Animator.GetCurrentAnimatorStateInfo(0).IsName("CrushObject") != wasCrush)
                {
                    Mod.LogWarning($"CrushObject State {!wasCrush}");
                    wasCrush = Animator.GetCurrentAnimatorStateInfo(0).IsName("CrushObject");
                }
                if (Animator.GetCurrentAnimatorStateInfo(0).IsName("PrintBlueprint") != wasPrint)
                {
                    Mod.LogWarning($"PrintBlueprint State {!wasPrint}");
                    wasPrint = Animator.GetCurrentAnimatorStateInfo(0).IsName("PrintBlueprint");
                }
                
                if (!data.InUse)
                {
                    Animator.SetBool(CrushObjectBool, value: data.Deconstructing);
                    Animator.SetFloat(CrushObjectTime, value: data.DeconstructionProgress);
                    Animator.SetBool(ObjectPlacedBool, value: false);
                    Animator.SetBool(PlaceBlueprintBool, value: false);
                    return;
                }
                else
                {
                    Animator.SetBool(CrushObjectBool, value: data.Deconstructing);
                    Animator.SetFloat(CrushObjectTime, value: data.DeconstructionProgress);
                    Animator.SetBool(ObjectPlacedBool, value: !data.IsDeconstructed);
                    Animator.SetBool(PlaceBlueprintBool, value: data.IsDeconstructed);
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
                if (data.IsDeconstructed && !Data.IsDeconstructed && Data.IsDay)
                {
                    UpgradeEffect?.SendEvent("BurstDeconstruct");
                    AudioSource.Play();
                }
                Data = data;
            }

        }
    }
    

}
