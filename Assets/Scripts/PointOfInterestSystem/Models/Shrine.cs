﻿using System;
using DBMS.RunningData;
using PlayerSystem.Controllers;
using PointOfInterestSystem.Views;
using UnityEngine;
using Zenject;

namespace PointOfInterestSystem.Models
{
    [Serializable]
    public class Shrine : PointOfInterest
    {
        private readonly PlayerController _playerController;
        
        public Shrine(PlayerController playerController, PointOfInterestView view)
        {
            _playerController = playerController;
            pointOfInterestView = view;
            IsInteractable = true;
            pointOfInterestView.SetToInteractable();
        }
        public override void OnInteract()
        {
            IsInteractable = false;
            _playerController.moveSpeed += 5;
            _playerController._speed += 5;
            Debug.LogWarning($"player speed increased by 10");
            pointOfInterestView.SetToUninteractable();
        }
    }
}