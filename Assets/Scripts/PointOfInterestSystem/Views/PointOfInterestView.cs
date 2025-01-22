using System;
using PlayerSystem.Controllers;
using PointOfInterestSystem.Interfaces;
using PointOfInterestSystem.Models;
using UnityEngine;
using Zenject;

namespace PointOfInterestSystem.Views
{
    public class PointOfInterestView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private IInteractable _interactable;

        [Inject] private PlayerController _playerController;
        private void Start()
        {
            _interactable = new Shrine(_playerController, this);
        }

        public void SetToUninteractable()
        {
            _renderer.color = Color.red;
        }

        public void SetToInteractable()
        {
            _renderer.color = Color.green;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if(!_interactable.IsInteractable) return;
            if(!other.gameObject.CompareTag("Player")) return;
            if(!Input.GetKeyDown(KeyCode.E)) return;
            _interactable.OnInteract();
            
        }
    }
}