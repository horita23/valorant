using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct GunPair
{
    public AK ak;
    public Button botton;
}

public class GunShop : MonoBehaviour
{
    private Cube playerAvatar;
    public GunPair[] Guns;

    private void Start()
    {

        if (Guns == null || Guns.Length == 0)
        {
            Debug.LogWarning("Guns array is empty or not initialized.");
            return;
        }

        for (int i = 0; i < Guns.Length; i++)
        {
            if (Guns[i].botton != null)
            {
                int index = i; // Capture the index for the lambda
                Guns[i].botton.onClick.AddListener(() => AddToCart(Guns[index].ak));
            }
            else
            {
                Debug.LogWarning($"Button for GunPair at index {i} is not assigned.");
            }
        }
    }

    private void Update()
    {
        var localPlayer = PhotonNetwork.LocalPlayer;
        playerAvatar = localPlayer.TagObject as Cube;

        // Cart total display (if needed)
        // cartTotalText.text = "Total: $" + user.Cart.GetTotalPrice();
    }

    private void AddToCart(AK item)
    {
        playerAvatar.AddItem(item);
    }
}
