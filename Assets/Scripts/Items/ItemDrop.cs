﻿using UnityEngine;
using System.Collections;

public class ItemDrop : MonoBehaviour {

	public static ItemDrop ItemToPickUp = null;

	public int DropID = 0;

	public float DespawnTime = 0;
	public Item item;

	public int ItemStack;
	public int ItemCharges = 1;

	private bool ready = false;

	private float unitWidth = 0;
	private float unitHeight = 0;

	private bool showHover = false;

	private string hoverText = "";

	//Attributes


	// Use this for initialization
	void Start () {

		if (item == null)
			return;

		Texture2D tex = item.Icon;

		if(tex != null)
		{
			float dispWidth = tex.width;
			if(dispWidth < 32)
				dispWidth = 32;
			((SpriteRenderer)GetComponent(typeof(SpriteRenderer))).sprite =
				Sprite.Create(item.Icon,new Rect(0,0,item.Icon.width,item.Icon.height),new Vector2(0.5f, 0.5f),dispWidth);

			unitWidth = tex.width/32f;
			unitHeight = tex.height/32f;

			unitWidth = 1;
			unitHeight = 1;

			hoverText = item.Name;

			ready = true;


		}
	}
	
	// Update is called once per frame
	void Update () {
		if ((!Network.isServer && !Network.isClient) || GameManager.ControllingInventory == null)
			return;


        //Debug.Log("ItemDrop: " + ready + " - " + !HUD.Instance.HUDFocus);
        if (ready && !HUDN.IsInventory())
        {
            //Debug.Log("ItemDrop: Testing setup");
            Vector2 p = HUD.Instance.transform.position;
            //Check distance to player;
            if (Vector2.Distance(gameObject.transform.position, p) < 2)
            {
                //Debug.Log("Near Item");
                ItemToPickUp = this;
            }
        }


		if (Time.time > DespawnTime && Network.isServer)
		{
			RemoveFromWorld();
		}
	}

	public void RemoveFromWorld()
	{
		ItemManager.RemoveDropFromWorld (DropID);
	}

	[RPC]
	void RemoveNetworkBufferedRPC(NetworkViewID viewId)
	{
		if(Network.isServer)
		{
			Network.RemoveRPCs(viewId);
		}
		else
		{
			networkView.RPC ("RemoveNetworkBufferedRPC", RPCMode.Server, viewId);
		}
	}
}
