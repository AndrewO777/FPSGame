using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Weapon : NetworkBehaviour
{
    public GunBase[] loadout;
    public Transform weaponParent;
    public GameObject bulletholePrefab;
    public LayerMask canBeShot;
    public ParticleSystem explodePrefab;
    public ParticleSystem bulletEffectPrefab;
    public float damage = 10f;

    private float currentCooldown;
    private int currentIndex;
    private GameObject currentWeapon;

    void Start()
    {
        //Creating pistol, rework this later.
        Equip(1);
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (currentWeapon != null)
        {
            Aim(Input.GetMouseButton(1));
            if (Input.GetMouseButtonDown(0) && currentCooldown <= 0)
            {
                Shoot();
            }
            //elasticity
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
            //firerate
            if(currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }
        }
    }

    void Equip(int index)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }
        currentIndex = index;
        GameObject newEquipment = Instantiate(loadout[index].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
        newEquipment.transform.localPosition = Vector3.zero;
        newEquipment.transform.localEulerAngles = Vector3.zero;
        currentWeapon = newEquipment;
    }

    void Aim(bool isAiming)
    {
        Transform anchor = currentWeapon.transform.Find("Anchor");
        Transform state_ads = currentWeapon.transform.Find("States/ADS");
        Transform state_hip = currentWeapon.transform.Find("States/Hip");
        if (isAiming)
        {
            anchor.position = Vector3.Lerp(anchor.position, state_ads.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
        }
        else
        {
            anchor.position = Vector3.Lerp(anchor.position, state_hip.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
        }
    }
    void Shoot()
    {
        Transform spawn = transform.Find("Main Camera");
        RaycastHit hit = new RaycastHit();
        int shots = 1;
        if (currentIndex == 1)
            shots = 8;
        for (;shots > 0;--shots){
        //bloom
        Vector3 bloom = spawn.position + spawn.forward * 1000f;
        bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * spawn.up;
        bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * spawn.right;
        bloom -= spawn.position;
        bloom.Normalize();
        //raycast
        if (Physics.Raycast(spawn.position, bloom * 1000f, out hit, 1000f, canBeShot))
        {
            //Add a if statement and make this the else for if you don't hit a player
            ParticleSystem bulletExp = Instantiate(bulletEffectPrefab, hit.point, Quaternion.identity);
            bulletExp.transform.rotation = Quaternion.LookRotation((spawn.transform.position - hit.point).normalized);
            bulletExp.Play();
            Destroy(bulletExp, bulletExp.main.duration - 0.1f);
            GameObject newHole = Instantiate(bulletholePrefab, hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
            newHole.transform.LookAt(hit.point + hit.normal);
            Destroy(newHole, 5f);
        }
        }
        //gun effects
        currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
        currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;
        //firerate
        currentCooldown = loadout[currentIndex].firerate;
    }
}
