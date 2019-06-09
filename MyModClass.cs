using System;
using LoaderExtras;
using MemeLoader.ModUtilities;
using UnityEngine;
using Harmony;


#region ERROR HELP
/* Q: I can't use any of Gorn's classes??
* -
* A: You need to add a reference to Gorn's assemblies, do so via:
* 
* View>Solution Explorer>Right-Click References>Add Reference>Browse>Navigate to Gorn's folder>Gorn_Data>Managed and from here add whatever you need.
* --
* Q: My mod won't load!
* -
* A: Have you filled out your ModInformation property? If so, check the CMD window when Gorn launches and look at the error, it'll tell you where the issue is occuring.
* ---
* Q: Gorn keeps saying NullReferenceException!!
* -
* A: 
* 
* Are you setting your variables inside the class deriving from IMod? Create a new class and derive it from MonoBehaviour or whatever.
* Don't use the entry class as the logic holder for your mod, it's supposed to be an overseer for your mod, creating/removing instances of your mod or initializing them.
* ----
* Q: Where's Harmony?
* -
* A: Add a reference to it, it's located where the MemeLauncher is installed.
*/
#endregion

namespace MyModNameSpace
{
    public class MyModEntry : IMod //Only have one class derive from this, it's an entry point.
    {
        #region variables
        public static MyModEntry reff;
        public static GameObject weaponPrefab;
        private int r = 0;
        //GameObject spawnInstance = null;
        #endregion


        public void OnLoaded()
        {
            //This is called when the mods are loaded, use as an initializer.
            Console.WriteLine($"Hello, world! Calling from {ModInformation.Name}!!");

            Debug.Log("Loading Mjolnir through Init");
            reff = this;

            //This is called when the game starts.
            Debug.Log("Loading model...");
            ModUtilities.LoadAssetBundle("/models/mjolnir.weapon", bundle =>
            {
                Console.WriteLine(bundle);
                ModUtilities.GetBundleContent(obj =>
                {
                    Console.WriteLine(obj);
                    weaponPrefab = obj;
                    weaponPrefab.AddComponent<MyWeaponSetUp>().SetUp(true);

                    HarmonyInstance.Create("net.GornMods.Mjolnir").PatchAll();
                }, bundle, "Mjolnir");


            }); //Loads specific bundle
            Debug.Log(" has finished setting up...");

            Console.WriteLine($"{ModInformation.Name} has loaded."); //The CMD window will pick up both Debug.Log and Console.WriteLine.
        }

        public void OnUnload()
        {
            //This is called when mods are unloaded, destroy any instances you've created here.

            Console.WriteLine($"{ModInformation.Name} has unloaded.");
        }


        public ModInformation ModInformation => new ModInformation
        {
            Name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, // CHANGE AT => Project>MyModTemplate Properties>Build>Assembly Name

            Creator = "theCH33F",

            Description = "Thors Hammer! Complete with Recall functionality. Use the gesture when holding to claim a hammer, then use gesture to recall.", //Explination of what mod does, e.g; Slows time down with magical powers!

            Version = "V3.0.0" //Mod version, you can name your versions whatever, example; V1/Version One/VER1/AAAAAAAA1
        };
    }

    public class MyWeaponSetUp : MonoBehaviour
    {

        GameObject handle, blade;

        public void SetUp(bool canRecall)
        {
            gameObject.AddComponent<WeaponBase>().type = WeaponType.ArmorBreaker;

            WeaponBase wb = GetComponent<WeaponBase>();
            //wb.type = WeaponType.ArmorBreaker;

            blade = transform.GetChild(0).gameObject;
            handle = transform.GetChild(1).gameObject;

            if (handle == null || blade == null)
            {
                Debug.LogError("BLADE OR HANDLE IS MISSING!");
                return;
            }


            WeaponHandle wh;

            if (canRecall)
            {
                handle.AddComponent<MjolnirHandle>();
                wh = handle.GetComponent<MjolnirHandle>();
            }
            else
            {
                handle.AddComponent<WeaponHandle>();
                wh = handle.GetComponent<WeaponHandle>();
            }

            wb.grabbable = wh;
            wb.canParry = true;
            wb.canBeParried = true;
            wb.addForceToRigidbodyFactor = 0.6f;

            wh.grabRigidbody = wh.GetComponent<Rigidbody>();
            wh.hideHandModelOnGrab = true;
            wh.isCurrentlyGrabbale = true;
            wh.setPositionOnGrab = true;
            wh.setRotationOnGrab = true;

            DamagerRigidbody drb = blade.AddComponent<DamagerRigidbody>();

            drb.scaleDamage = 1.2f;
            drb.canAlsoCut = false;
            drb.bonusVelocity = 0.7f;
            drb.isDamaging = true;
            drb.impaleDepth = 1f;
            drb.damageType = DamageType.Blunt;

            handle.AddComponent<FootStepSound>().soundEffectName = "WeaponDrop";
            handle.GetComponent<FootStepSound>().minVolumeToTrigger = 0.05f;

            /*
            try
            {
                if (transform.GetChild(2) != null)
                {
                    drb.heartStabPoint = transform.GetChild(2);

                    drb.impaledBreakForce = weapon.impaleBreakForce;
                    drb.impaledZDamper = weapon.impaleZDamper;
                    drb.impaledConnectedBodyMassScale = weapon.connectedBMass;
                }
            }
            catch
            {
                return;
            }
            */

            return;
        }
    }

    public class MjolnirHandle : WeaponHandle
    {
        static MjolnirHandle currentMjol;
        Vector3 handPos;
        Rigidbody rb;

        protected void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        protected override void Update()
        {
            base.Update();

            if ((InputReader.LeftGestureButtonPressDown || InputReader.RightGestureButtonPressDown) && weaponBase.wieldedByPlayer)
            {
                //Debug.Log("Set the hammer to be: " + weaponBase.gameObject);
                currentMjol = this;
            }

            if (InputReader.LeftGestureButtonPress && !BeingWielded)
            {
                //Debug.Log("trying recall: " + currentMjol.weaponBase.gameObject);

                if (currentMjol.Equals(this))
                {
                    handPos = GameController.Player.leftArmPos;
                    currentMjol.rb.velocity = (handPos - rb.position) * 5;

                }
            }

            if (InputReader.RightGestureButtonPress && !BeingWielded)
            {
                //Debug.Log("trying recall: " + currentMjol.weaponBase.gameObject);

                if (currentMjol.Equals(this))
                {
                    handPos = GameController.Player.rightArmPos;
                    currentMjol.rb.velocity = (handPos - rb.position) * 5;

                }
            }
        }

    }

    [HarmonyPatch(typeof(GameController), "SetupLevel")]
    public class Spawn_Weapon
    {
        public static void Postfix(GameController __instance)
        {
            Console.WriteLine("Spawning Mjolnir!");

            UnityEngine.Object.Instantiate(MyModEntry.weaponPrefab, GameController.Player.position + new Vector3(0, 5, 5), GameController.Player.handLeft.transform.rotation);
        }

    }
}

    