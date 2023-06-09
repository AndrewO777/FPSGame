﻿using System.Collections;
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
    public int damage;

    private float currentCooldown;
    //private int currentIndex;
    private GameObject currentWeapon;

    private NetworkVariable<int> currentIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        currentIndex.OnValueChanged += OnWeaponIndexChanged;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Equip(currentIndex.Value);
        }
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            EquipAndSync(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            EquipAndSync(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            EquipAndSync(2);
        {
            Aim(Input.GetMouseButton(1));
            if (loadout[currentIndex.Value].burst != 1)
            {
                if (Input.GetMouseButtonDown(0) && currentCooldown <= 0)
                {
                    Shoot();
                }
            }
            else
            {
                if (Input.GetMouseButton(0) && currentCooldown <= 0)
                {
                    Shoot();
                }
            }
            //elasticity
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
            //firerate
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }
        }
    }

    void EquipAndSync(int index)
{
    currentIndex.Value = index;
    Equip(index);
    EquipClientRpc(index);
}

    [ClientRpc]
    void EquipClientRpc(int index)
    {
        if (IsOwner)
        {
            return;
        }

        Equip(index);
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestEquipServerRpc(int index, ServerRpcParams rpcParams = default)
    {
        currentIndex.Value = index;
        Equip(index);
    }

    void OnWeaponIndexChanged(int oldValue, int newValue)
    {
        Equip(newValue);
    }

    void Equip(int index)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }
        currentIndex.Value = index;
        GameObject newEquipment = Instantiate(loadout[index].prefab, weaponParent.position, weaponParent.rotation) as GameObject;
        newEquipment.transform.SetParent(weaponParent);
        newEquipment.transform.localPosition = Vector3.zero;
        newEquipment.transform.localEulerAngles = Vector3.zero;
        currentWeapon = newEquipment;
        damage = loadout[index].damage;
    }

    void Aim(bool isAiming)
    {
        Transform anchor = currentWeapon.transform.Find("Anchor");
        Transform state_ads = currentWeapon.transform.Find("States/ADS");
        Transform state_hip = currentWeapon.transform.Find("States/Hip");
        if (isAiming)
        {
            anchor.position = Vector3.Lerp(anchor.position, state_ads.position, Time.deltaTime * loadout[currentIndex.Value].aimSpeed);
        }
        else
        {
            anchor.position = Vector3.Lerp(anchor.position, state_hip.position, Time.deltaTime * loadout[currentIndex.Value].aimSpeed);
        }
    }
    void Shoot()
    {
        Transform spawn = transform.Find("Main Camera");
        RaycastHit hit = new RaycastHit();
        int shots = 1;
        if (currentIndex.Value == 1)
            shots = 8;
        for (; shots > 0; --shots)
        {
            //bloom
            Vector3 bloom = spawn.position + spawn.forward * 1000f;
            bloom += Random.Range(-loadout[currentIndex.Value].bloom, loadout[currentIndex.Value].bloom) * spawn.up;
            bloom += Random.Range(-loadout[currentIndex.Value].bloom, loadout[currentIndex.Value].bloom) * spawn.right;
            bloom -= spawn.position;
            bloom.Normalize();
            //raycast
            if (Physics.Raycast(spawn.position, bloom * 1000f, out hit, 1000f, canBeShot))
            {
                // Add a if statement and make this the else for if you don't hit a player
                if (hit.collider.gameObject.layer == 6)
                {
                    ParticleSystem bulletExp = Instantiate(explodePrefab, hit.point, Quaternion.identity);
                    bulletExp.transform.rotation = Quaternion.LookRotation((spawn.transform.position - hit.point).normalized);
                    bulletExp.Play();
                    Destroy(bulletExp, bulletExp.main.duration - 0.1f);
                    PlayerMovement player = hit.collider.GetComponent<PlayerMovement>();
                    if (player != null)
                    {
                        player.TakeDamageServerRpc(player.myID, damage);
                    }
                }
                else
                {
                    ParticleSystem bulletExp = Instantiate(bulletEffectPrefab, hit.point, Quaternion.identity);
                    bulletExp.transform.rotation = Quaternion.LookRotation((spawn.transform.position - hit.point).normalized);
                    bulletExp.Play();
                    Destroy(bulletExp, bulletExp.main.duration - 0.1f);
                    GameObject newHole = Instantiate(bulletholePrefab, hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
                    newHole.transform.LookAt(hit.point + hit.normal);
                    Destroy(newHole, 5f);
                }
            }
        }
        //gun effects
        currentWeapon.transform.Rotate(-loadout[currentIndex.Value].recoil, 0, 0);
        currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex.Value].kickback;
        //firerate
        currentCooldown = loadout[currentIndex.Value].firerate;
    }
}
