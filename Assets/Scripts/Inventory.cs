using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : ScriptableObject
{
  private bool bullets;
  private bool gun;
  private bool keys;

  public void Initialize() {
    bullets = false;
    gun = false;
    keys = false;
  }

  public void pickupBullets() {
    bullets = true;
  }

  public void pickupGun() {
    gun = true;
  }

  public void pickupItem(string item) {
    if(item == "keys") {
      pickupKeys();
    } else if(item == "gun") {
      pickupGun();
    } else if(item == "bullets") {
      pickupBullets();
    }
  }

  public void pickupKeys() {
    keys = true;
  }

  public bool checkInventory(string key) {
    bool result = false;

    if(key == "bullets") {
      result = bullets;
    } else if(key == "gun") {
      result = gun;
    } else if(key == "keys") {
      result = keys;
    }

    return result;
  }
}
