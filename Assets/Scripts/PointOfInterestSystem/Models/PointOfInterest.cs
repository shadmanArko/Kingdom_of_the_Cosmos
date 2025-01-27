using System;
using PointOfInterestSystem.Interfaces;
using PointOfInterestSystem.Views;
using Zenject;

namespace PointOfInterestSystem.Models
{
    [Serializable]
    public abstract class PointOfInterest : IInteractable
    {
        //choices 2 to 3
        //title
        //description
        //
        
        protected PointOfInterestView pointOfInterestView;
        public bool IsInteractable { get; set; }

        public virtual void OnInteract()
        {
            
        }
        
    }
}