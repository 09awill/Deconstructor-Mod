using DeconstructorMod.Components;
using KitchenDeconstructor.Systems;
using Kitchen;
using KitchenData;
using KitchenDeconstructor;
using KitchenLib.Customs;
using KitchenLib.Utils;
using MessagePack;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using static KitchenDeconstructor.Patches.StorageStructs;
using UnityEngine.InputSystem;
using System.Data;
using KitchenLib.References;

namespace DeconstructorMod.Customs
{
    public class Deconstructor : CustomAppliance
    {

        public override string UniqueNameID => "Deconstructor";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("Deconstructor");
        public override PriceTier PriceTier => PriceTier.Expensive;
        public override bool SellOnlyAsDuplicate => false;
        public override bool IsPurchasable => false;
        public override bool IsPurchasableAsUpgrade => true;
        public override RarityTier RarityTier => RarityTier.Uncommon;
        public override ShoppingTags ShoppingTags => ShoppingTags.BlueprintStore | ShoppingTags.Misc;
        public override bool IsNonInteractive => false;
        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            ( Locale.English, LocalisationUtils.CreateApplianceInfo("Deconstructor", "Allows you to deconstruct your kitchen appliances", new(), new()) )
        };
        public override List<Appliance> Upgrades => new List<Appliance>() { Mod.BlueprintCabinet };
        public override List<IApplianceProperty> Properties => new List<IApplianceProperty>()
        {
            new CIsInteractive(), new CIDeconstruct(), new CTakesDuration(){ Total = 5f, Manual = true, ManualNeedsEmptyHands = false, IsInverse = true, Mode = InteractionMode.Items, PreserveProgress = true, IsLocked = true}, KitchenPropertiesUtils.GetCDisplayDuration(false, Mod.DeconstructProcess.ID, false), new CLockDurationTimeOfDay(){ LockDuringNight = true, LockDuringDay = false }, new CStoredPlates(){ PlatesCount = 0}, new CStoredTables(),
        };
        public override void OnRegister(GameDataObject gameDataObject)
        {
            BlinkingLED LED = Prefab.AddComponent<BlinkingLED>();
            AudioSource source = Prefab.GetChild("VFX/Deconstruct").AddComponent<AudioSource>();
            VisualEffect vfx = Prefab.GetChild("VFX/Deconstruct").GetComponent<VisualEffect>();
            TextMeshPro tmp = Prefab.GetChild("PaperBack/Name").GetComponent<TextMeshPro>();
            source.volume = 0.1f;
            source.playOnAwake = false;
            source.clip = Mod.Bundle.LoadAsset<AudioClip>("deconstructSoundEffect");

            vfx.visualEffectAsset = Mod.Bundle.LoadAsset<VisualEffectAsset>("VFX_Deconstruct");

            tmp.material = MaterialUtils.GetExistingMaterial("Cake n Truffles Atlas Material_0");
            tmp.font = FontUtils.GetExistingTMPFont("Blueprint");
            DeconstructorView deconstructorView = Prefab.AddComponent<DeconstructorView>();
            deconstructorView.LED = LED;
            deconstructorView.UpgradeEffect = vfx;
            deconstructorView.AudioClip = source.clip;
            deconstructorView.AudioSource = source;
            deconstructorView.Animator = Prefab.GetComponent<Animator>();
            deconstructorView.ObjectMesh = Prefab.GetChild("Box");
            deconstructorView.Title = tmp;
            deconstructorView.Blueprint = Prefab.GetChild("PaperBack");
            deconstructorView.BlueprintRenderer = Prefab.GetChild("PaperBack/Paper").GetComponent<MeshRenderer>();
            deconstructorView.ObjectRenderer = Prefab.GetChild("Box/ItemRender").GetComponent<MeshRenderer>();




            SetupMaterials();


        }

        public void SetupMaterials()
        {
            Material[] mats = new Material[] { MaterialUtils.GetExistingMaterial("Metal - Soft Green Paint") };
            Prefab.GetChild("Deconstructor").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Metal Very Dark") };
            Prefab.GetChild("Feet").ApplyMaterial(mats);
            Prefab.GetChild("Crusher").ApplyMaterial(mats);
            Prefab.GetChild("SmallGear").ApplyMaterial(mats);
            Prefab.GetChild("LargeGear").ApplyMaterial(mats);
            Prefab.GetChild("SmallGear/SmallGearCentre").ApplyMaterial(mats);
            Prefab.GetChild("LargeGear/LargeGearCentre").ApplyMaterial(mats);
            Prefab.GetChild("Crusher/Cube.005").ApplyMaterial(mats);


            mats = new Material[] { MaterialUtils.GetExistingMaterial("Wood 2") };
            Prefab.GetChild("Box").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Plate") };
            Prefab.GetChild("Cube").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Blueprint Light") };
            Prefab.GetChild("PaperBack").ApplyMaterial(mats);
            mats = new Material[] { MaterialUtils.GetExistingMaterial("Flat Image - Faded") };
            Prefab.GetChild("PaperBack/Paper").ApplyMaterial(mats);
            Prefab.GetChild("Box/ItemRender").ApplyMaterial(mats);


            mats = new Material[] { MaterialUtils.GetExistingMaterial("Plastic - Dark Green") };
            Prefab.GetChild("Deconstructor/PrintedLED").ApplyMaterial(mats);



            mats = new Material[] { MaterialUtils.GetExistingMaterial("Cake n Truffles Atlas Material_0") };
            Prefab.GetChild("PaperBack/Name").ApplyMaterial(mats);
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
            public BlinkingLED LED;
            public MeshRenderer ObjectRenderer;


            public ViewData Data;
            private static readonly int ObjectPlacedBool = Animator.StringToHash("ObjectPlacedBool");
            private static readonly int CrushObjectBool = Animator.StringToHash("CrushObjectBool");
            private static readonly int PlaceBlueprintBool = Animator.StringToHash("PlaceBlueprintBool");
            private static readonly int CrushObjectTime = Animator.StringToHash("DeconstructSpeed");

            protected override void UpdateData(ViewData data)
            {


                if (!data.InUse)
                {
                    Animator.SetBool(CrushObjectBool, value: data.Deconstructing);
                    Animator.SetFloat(CrushObjectTime, value: data.DeconstructionProgress);
                    Animator.SetBool(ObjectPlacedBool, value: false);
                    Animator.SetBool(PlaceBlueprintBool, value: false);
                    LED.SetBlink(false);
                    return;
                }
                else
                {
                    Animator.SetBool(CrushObjectBool, value: data.Deconstructing);
                    Animator.SetFloat(CrushObjectTime, value: data.DeconstructionProgress);
                    Animator.SetBool(ObjectPlacedBool, value: !data.IsDeconstructed);
                    Animator.SetBool(PlaceBlueprintBool, value: data.IsDeconstructed);
                    LED.SetBlink(data.IsDeconstructed);
                }

                if (Data.Appliance != data.Appliance && GameData.Main.TryGet<Appliance>(data.Appliance, out var output))
                {
                    if (BlueprintRenderer != null)
                    {
                        BlueprintRenderer.material.SetTexture("_Image", PrefabSnapshot.GetSnapshot(output.Prefab));
                    }

                    if (Title != null)
                    {
                        Title.text = output.Name;
                    }
                    if (ObjectRenderer != null)
                    {
                        ObjectRenderer.material.SetTexture("_Image", PrefabSnapshot.GetSnapshot(output.Prefab));
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
