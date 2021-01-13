﻿using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Components
{
    public class GlanceFish : MonoBehaviour
    {
        public Image fishAvatar;
        public Image mask;
        public Image border;

        public void SetupFish(int id, Constants.FishState state)
        {
            fishAvatar.overrideSprite = SharedRefs.FishAvatars[id];
            switch (state)
            {
                case Constants.FishState.Used:
                    mask.color = new Color(1, 0, 0, 0.3f);
                    border.color = Color.grey;
                    break;
                case Constants.FishState.Using:
                    mask.color = new Color(0, 0, 0, 0);
                    border.color = Color.white;
                    break;
                case Constants.FishState.Free:
                    mask.color = new Color(0, 0, 0, 0.6f);
                    border.color = new Color(0, 128, 0, 0.6f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}