using HarmonyLib;
using MemeLoader;
using MemeLoader.Modding;
using static MemeLoader.Utilities.ModUtilities;
using System;
using UnityEngine;

namespace GornMods
{
    public class Entry: ModEntry
    {
        public static Entry reff;
        public static GameObject weaponPrefab;
        /// <summary>
        /// Called when your mod is first loaded by the loader, best used to initialize your mod.
        /// </summary>
        public override void OnModInitialized(Mod mod)
        {
            // Your code goes here.
            RegisterBundle("mjolnir", mod.DirectoryPath);
            GetFromBundle<Transform>("Mjolnir", (mjolnir) =>
            {
                weaponPrefab = mjolnir.gameObject;
                weaponPrefab.AddComponent<MjolnirSetup>().SetUp(true);

                new Harmony("com.GORN.Mods.Mjolnir").PatchAll();
            });


            base.OnModInitialized(mod);
        }

       /// <summary>
       /// Called when your mod is unloaded, you should destroy any objects you created or any instances, remove any traces of your mod here.
       /// </summary>
       public override void OnModUnLoaded()
       {
            // Unload your mod here.
            
            
            base.OnModUnLoaded();
       }
    }

    public class MjolnirSetup : MonoBehaviour
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

            UnityEngine.Object.Instantiate(Entry.weaponPrefab, GameController.Player.position + new Vector3(0, 5, 5), GameController.Player.handLeft.transform.rotation);
        }

    }
}