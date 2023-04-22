using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GunBase[] loadout;
    public Transform weaponParent;
    public GameObject bulletholePrefab;
    public LayerMask canBeShot;
    public ParticleSystem explodePrefab;
    public ParticleSystem bulletEffectPrefab;
    public int counter = 0;
    public float damage = 10f;

    private float currentCooldown;
    private int currentIndex;
    private GameObject currentWeapon;

    void Start()
    {
        //Creating pistol, rework this later.
        Equip(0);
    }

    void Update()
    {
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

    void Equip(int t_index)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }
        currentIndex = t_index;
        GameObject t_newEquipment = Instantiate(loadout[t_index].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
        t_newEquipment.transform.localPosition = Vector3.zero;
        t_newEquipment.transform.localEulerAngles = Vector3.zero;
        currentWeapon = t_newEquipment;
    }

    void Aim(bool t_isAiming)
    {
        Transform t_anchor = currentWeapon.transform.Find("Anchor");
        Transform t_state_ads = currentWeapon.transform.Find("States/ADS");
        Transform t_state_hip = currentWeapon.transform.Find("States/Hip");
        if (t_isAiming)
        {
            t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_ads.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
        }
        else
        {
            t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_hip.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
        }
    }
    void Shoot()
    {
        Transform t_spawn = transform.Find("Main Camera");
        RaycastHit t_hit = new RaycastHit();
        //bloom
        Vector3 t_bloom = t_spawn.position + t_spawn.forward * 1000f;
        t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.up;
        t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.right;
        t_bloom -= t_spawn.position;
        t_bloom.Normalize();
        //raycast
        if (Physics.Raycast(t_spawn.position, t_spawn.position + t_bloom * 1000f, out t_hit, 1000f, canBeShot))
        {
            if (t_hit.collider.gameObject.tag == "Balls")
            {
                ParticleSystem exp = Instantiate(explodePrefab, t_hit.point + t_hit.normal, Quaternion.identity);
                exp.Play();
                Destroy(exp, exp.main.duration - 0.1f);
                Destroy(t_hit.collider.gameObject);
                ++counter;
            }
            else
            {
                ParticleSystem bulletExp = Instantiate(bulletEffectPrefab, t_hit.point + t_hit.normal, Quaternion.identity);
                bulletExp.Play();
                Destroy(bulletExp, bulletExp.main.duration - 0.1f);
                GameObject t_newHole = Instantiate(bulletholePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
                Destroy(t_newHole, 5f);
            }
        }
        //gun effects
        currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
        currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;
        //firerate
        currentCooldown = loadout[currentIndex].firerate;
    }
}
