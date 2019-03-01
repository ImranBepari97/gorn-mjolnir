using MemeLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/*______________________Info_________________________
 * Template provided by: MemeMan (MemeLoader creator).
 * 
 * Supports: MemeLoader V0.5.0
 * --------------------------------------------------
 * 
 * ____________________________________________Help!____________________________________________________________________________________________
 * See: Modding: How to | Modding: video guide in your template project for more help, or ask on the Discord! | Below to the `Error Help` region.
 * --------------------------------------------------------------------------------------------------------------------------------------------
 * */

namespace MyCustomWeapon
{

    public class MyCustomWeaponManager
    {

        #region Mod Information
        //Named defined in assembly (Project>ProjectName Properties>Application>Assembly Name)

        public string Creator = "theCH33F", Version = "V2.0.0"; //V1.0.0 -> Major.Minor.Maintenance[Build]
        public string Description = "Thors Hammer! Complete with Recall functionality. Use the gesture when holding to claim a hammer, then use gesture to recall.";

        //This information is displayed in-game.
        #endregion

        //c = Configuration File Name, m = Mod Name, a = AssetBundle name with extension
        public static string c = "WeaponConfiguration", m = "Mjolnir", a = "mjolnir.weapon"; // Change m and a to the corresponding names.

        #region IGNORE 
        WeaponChance thisWeapon = null;
        GameObject spawnInstance = null;

        public void Init()
        {
            Debug.Log("Loading Mjolnir through Init");
            reff = this;

            //This is called when the game starts.

            // ModUtilities.ClearConfig (c,m); //Remove comments this then build to clear the config(Then remove or comment this line), or manually clear it.
            ModUtilities.CreateConfig(c, m);

            SetUpConfig();

            Debug.Log("Loading model...");
            ModUtilities.toInvokeOn.StartCoroutine(ModUtilities.LoadModelFromSource(m, bundleName, modelName, OnModelLoaded));
            Debug.Log(m + " has finished setting up...");
        }


        private void OnModelLoaded(GameObject args)
        {
            weaponPrefab = args;

            thisWeapon = new WeaponChance(weaponPrefab, 100, new bool[]
 {
                canPary,
                canBeParried,
                stickOnDamage,
                hideHandOnGrab,
                isCurrentlyGrabbable,
                setPositionOnGrab,
                setRotationOnGrab,
                canAlsoCut,
                isDamaging
 }, new float[]
 {
                impaleZDamper,
                connectedBMass,
                impaleBreakForce,
                scaleDamage,
                bonusVelocity,
                impaleDepth,
                damageType,
                weaponType,
                addForceToRigidbodyFactor
 });

            ModUtilities.toInvokeOn.AddWeaponToList(thisWeapon);

            Debug.Log(args.name + " has loaded.");
        }

        public void OnEnemySetUp(EnemySetupInfo esi)
        {
            Debug.Log("Setup has been called");
            if (spawnInstance == null)
            { 
                WeaponChance weapon = ModUtilities.GetWeaponFromList(thisWeapon);
                spawnInstance = UnityEngine.Object.Instantiate(weapon.weapon, GameController.Player.position + new Vector3(0, 5, 5), GameController.Player.handLeft.transform.rotation);
                spawnInstance.AddComponent<MyWeaponSetUp>().SetUp(weapon, canRecall);
                Debug.Log(m + " has spawned.");
            }
        }

        private void SetUpConfig()
        {
            Debug.Log("Starting config");

            try
            {
                ModUtilities.AddKeyToConfig(c, m, "[REQUIRED]", "WeaponObjectName = Name goes here");
                ModUtilities.AddKeyToConfig(c, m, "[REQUIRED]", "AssetBundleName = " + a);
                ModUtilities.AddKeyToConfig(c, m, "[SPACE]", "");

                #region ints
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "DamageType = 1");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "WeaponType = 1");
                ModUtilities.AddKeyToConfig(c, m, "[SPACE]", "");
                #endregion

                #region floats
               // ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "SpawnChance = 25");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "ScaleDamage = 1.2");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "BonusVelocity = 0.7");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "ImpaleDepth = 1");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "ImpaleZDamper = 25");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "ImpaledConnectedBodyMassScale = 10");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "ImpaledBreakForce = 5000");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "AddForceToRigidbodyFactor = 0.6");
                #endregion

                #region bools
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "CanPary = true");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "CanBeParried = true");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "StickOnDamage = false");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "HideHandOnGrab = true");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "IsCurrentlyGrabbable = true");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "SetPositionOnGrab = true");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "SetRotationOnGrab = true");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "CanAlsoCut = true");
                ModUtilities.AddKeyToConfig(c, m, "[OPTION]", "IsDamaging = true");
                #endregion

                modelName = (string)ModUtilities.GetKeyFromConfig(c, m, "WeaponObjectName");
                bundleName = (string)ModUtilities.GetKeyFromConfig(c, m, "AssetBundleName");

                canPary = (bool)ModUtilities.GetKeyFromConfig(c, m, "CanPary");
                canBeParried = (bool)ModUtilities.GetKeyFromConfig(c, m, "CanBeParried");
                stickOnDamage = (bool)ModUtilities.GetKeyFromConfig(c, m, "StickOnDamage");
                hideHandOnGrab = (bool)ModUtilities.GetKeyFromConfig(c, m, "HideHandOnGrab");
                isCurrentlyGrabbable = (bool)ModUtilities.GetKeyFromConfig(c, m, "IsCurrentlyGrabbable");
                setPositionOnGrab = (bool)ModUtilities.GetKeyFromConfig(c, m, "SetPositionOnGrab");
                setRotationOnGrab = (bool)ModUtilities.GetKeyFromConfig(c, m, "SetRotationOnGrab");
                canAlsoCut = (bool)ModUtilities.GetKeyFromConfig(c, m, "CanAlsoCut");
                isDamaging = (bool)ModUtilities.GetKeyFromConfig(c, m, "IsDamaging");

                //spawnChance = (float)ModUtilities.GetKeyFromConfig(c, m, "SpawnChance");
                scaleDamage = (float)ModUtilities.GetKeyFromConfig(c, m, "ScaleDamage");
                bonusVelocity = (float)ModUtilities.GetKeyFromConfig(c, m, "BonusVelocity");
                impaleDepth = (float)ModUtilities.GetKeyFromConfig(c, m, "ImpaleDepth");
                impaleZDamper = (float)ModUtilities.GetKeyFromConfig(c, m, "ImpaleZDamper");
                connectedBMass = (float)ModUtilities.GetKeyFromConfig(c, m, "ImpaledConnectedBodyMassScale");
                impaleBreakForce = (float)ModUtilities.GetKeyFromConfig(c, m, "ImpaledBreakForce");
                addForceToRigidbodyFactor = (float)ModUtilities.GetKeyFromConfig(c, m, "AddForceToRigidbodyFactor");

                damageType = (float)ModUtilities.GetKeyFromConfig(c, m, "DamageType");
                weaponType = (float)ModUtilities.GetKeyFromConfig(c, m, "WeaponType");
                canRecall = (bool)ModUtilities.GetKeyFromConfig(c, m, "CanRecall");

            }
            catch (Exception e)
            {
                Debug.LogError("UNABLE TO PARSE VALUE, ONE OR MORE MAY HAVE FAILED!\n" + e);
            }
        }

        #region variables
        public static MyCustomWeaponManager reff;
        private GameObject weaponPrefab;
        public string modelName = "", bundleName = "";
        public bool canPary = true, canBeParried = true, stickOnDamage = false, hideHandOnGrab = true, isCurrentlyGrabbable = true, setPositionOnGrab = true, setRotationOnGrab = true, canAlsoCut = true, isDamaging = true, canRecall = true;
        public float spawnChance = 25, impaleZDamper = 25, connectedBMass = 10, impaleBreakForce = 5000, scaleDamage = 1.2f, bonusVelocity = 0.7f, impaleDepth = 1, damageType = 1, weaponType = 1, addForceToRigidbodyFactor = 0.6f;
        private int r = 0;
        #endregion
        #endregion
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
                Debug.Log("Set the hammer to be: " + weaponBase.gameObject);
                currentMjol = this;
            }

            if (InputReader.LeftGestureButtonPress && !BeingWielded)
            {
                Debug.Log("trying recall: " + currentMjol.weaponBase.gameObject);

                if (currentMjol.Equals(this))
                {
                    handPos = GameController.Player.leftArmPos;
                    currentMjol.rb.velocity = (handPos - rb.position) * 5;

                }
            }

            if (InputReader.RightGestureButtonPress && !BeingWielded)
            {
                Debug.Log("trying recall: " + currentMjol.weaponBase.gameObject);

                if (currentMjol.Equals(this))
                {
                    handPos = GameController.Player.rightArmPos;
                    currentMjol.rb.velocity = (handPos - rb.position) * 5;

                }
            }
        }

    }

    public class MyWeaponSetUp : MonoBehaviour
    {

        GameObject handle, blade;

        public void SetUp(WeaponChance weapon, bool canRecall)
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
            wb.canParry = weapon.canPary;
            wb.canBeParried = weapon.canBeParried;
            wb.addForceToRigidbodyFactor = weapon.addForceToRigidbodyFactor;

            wh.grabRigidbody = wh.GetComponent<Rigidbody>();
            wh.hideHandModelOnGrab = weapon.hideHandOnGrab;
            wh.isCurrentlyGrabbale = weapon.isCurrentlyGrabbable;
            wh.setPositionOnGrab = weapon.setPositionOnGrab;
            wh.setRotationOnGrab = weapon.setRotationOnGrab;

            DamagerRigidbody drb = blade.AddComponent<DamagerRigidbody>();

            drb.scaleDamage = weapon.scaleDamage;
            drb.canAlsoCut = weapon.canAlsoCut;
            drb.bonusVelocity = weapon.bonusVelocity;
            drb.isDamaging = weapon.isDamaging;
            drb.impaleDepth = weapon.impaleDepth;
            drb.damageType = (DamageType)weapon.damageType;

            handle.AddComponent<FootStepSound>().soundEffectName = "WeaponDrop";
            handle.GetComponent<FootStepSound>().minVolumeToTrigger = 0.05f;

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

            return;
        }
    }
}

#region Error help

/*Q = Question
 *A = Answer
 *O = Optional
 *I = Additional Information
 * - - - - - - - - - - - - - 
 * -Q: It says I'm missing an assembly reference?-
 * ==============================================
 * A: View>Solution Explorer>References>Right-click>Add>Clear all(if any show up)(Right-click one and clear all)>Browse>Project root>Plugins>Select All.
 * 
 * -Q: My mod won't load!- 
 * ========================
 *  A: Did you remove Init()? If not, everything should work, it'll be your code, double check!
 *  I: I keep dlSpy(.dll deassembler) open so I can see the source to understand what I'm modifying.
 *  
 *  -Q: I accidentally broke the game, help!-
 *  =========================================
 *  A: Delete: GORN_Data>Managed>Assembly-CSharp.dll and the most recent mod you broke it with.
 *  O: Verify file integrity, launching the game will start this automatically.
 */

#endregion