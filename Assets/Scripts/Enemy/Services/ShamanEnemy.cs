using System;
using Enemy.Models;
using Player;
using Player.Controllers;
using Player.Views;
using UnityEngine;

namespace Enemy.Services
{
    public class ShamanEnemy : BaseEnemy
    {

        [SerializeField] private PlayerAnimationController playerAnimationController;
        public float shamanRadius;
        public float shamanInterval;
        protected override void Start()
        {
            base.Start();
            playerAnimationController.PlayAnimation("run");
        }

        
        public override void Attack(PlayerView target)
        {
        
        }

        public void DoShaman()
        {
            
        }
    }
}
