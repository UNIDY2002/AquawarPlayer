﻿using UnityEngine;
using Utils;

namespace Components
{
    public class Glance : MonoBehaviour
    {
        public GlanceFish glanceFishPrefab;

        public void SetupFish(int id, Constants.FishState state)
        {
            var fish = Instantiate(glanceFishPrefab, gameObject.transform);
            fish.GetComponent<Transform>().localPosition = new Vector3(id % 4 * 60 - 90, id / 4 * 64 - 64);
            fish.SetupFish(id, state);
        }
    }
}