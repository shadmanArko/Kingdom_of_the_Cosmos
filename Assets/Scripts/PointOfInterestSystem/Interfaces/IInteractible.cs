namespace PointOfInterestSystem.Interfaces
{
    public interface IInteractable
    {
        public bool IsInteractable { get; set; }
        public void OnInteract();
    }
}