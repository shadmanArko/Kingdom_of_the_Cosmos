using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
using System;
using Pickup_System.Manager;

namespace Pickup_System
{
    public class PickupController : ITickable, IPickupSystem, IDisposable
    {
      private readonly ConcurrentDictionary<int, IPickupable> activePickups = new ConcurrentDictionary<int, IPickupable>();
        private readonly IPickupCollector collector;
        private readonly IPickupDistanceCalculator distanceCalculator;
        private readonly IPickupPoolManager pickupPoolManager;
        
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly Subject<IPickupable> onPickupProcessed = new Subject<IPickupable>();
        private CancellationTokenSource cancellationTokenSource;
        
        private const int BATCH_SIZE = 32;
        
        public PickupController(
            IPickupCollector collector,
            IPickupDistanceCalculator distanceCalculator,
            IPickupPoolManager pickupPoolManager)
        {
            this.collector = collector;
            this.distanceCalculator = distanceCalculator;
            this.pickupPoolManager = pickupPoolManager;
            
            cancellationTokenSource = new CancellationTokenSource();
            InitializeStreams();
        }

        private void InitializeStreams()
        {
            onPickupProcessed
                .Buffer(TimeSpan.FromMilliseconds(16))
                .Where(pickups => pickups.Any())
                .ObserveOnMainThread() // Ensure processing happens on main thread
                .Subscribe(ProcessPickupBatch)
                .AddTo(disposables);
        }

        public void RegisterPickup(IPickupable pickup)
        {
            activePickups.TryAdd(pickup.GetHashCode(), pickup);
        }

        public void UnregisterPickup(IPickupable pickup)
        {
            activePickups.TryRemove(pickup.GetHashCode(), out _);
        }

        public void Tick()
        {
            ProcessPickupsAsync().Forget();
        }

        private async UniTaskVoid ProcessPickupsAsync()
        {
            try
            {
                // Ensure we're on the main thread when accessing Unity components
                await UniTask.SwitchToMainThread();

                var pickups = activePickups.Values.ToArray();
                if (pickups.Length == 0) return;

                // Process in batches
                for (int i = 0; i < pickups.Length; i += BATCH_SIZE)
                {
                    var batch = pickups.Skip(i).Take(BATCH_SIZE).ToArray();
                    await ProcessPickupBatchAsync(batch);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing pickups: {e}");
            }
        }

        private async UniTask ProcessPickupBatchAsync(IPickupable[] pickups)
        {
            foreach (var pickup in pickups)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested) break;

                // Perform distance check on main thread since it might involve Transform
                if (await CheckPickupDistanceAsync(pickup))
                {
                    onPickupProcessed.OnNext(pickup);
                }
            }
        }

        private async UniTask<bool> CheckPickupDistanceAsync(IPickupable pickup)
        {
            // Ensure we're on the main thread for Unity operations
            await UniTask.SwitchToMainThread();
            return distanceCalculator.IsInRange(collector, pickup);
        }

        private void ProcessPickupBatch(IList<IPickupable> pickups)
        {
            foreach (var pickup in pickups)
            {
                if (!activePickups.ContainsKey(pickup.GetHashCode())) continue;

                if (pickup.CanBePickedUp(collector) && collector.CanCollectPickup(pickup))
                {
                    ProcessSinglePickup(pickup).Forget();
                }
            }
        }

        private async UniTaskVoid ProcessSinglePickup(IPickupable pickup)
        {
            try
            {
                // Ensure we're on the main thread for Unity operations
                await UniTask.SwitchToMainThread();

                pickup.OnPickup(collector);
                collector.CollectPickup(pickup);
                pickupPoolManager.ReturnToPool(pickup.PickupView);
                UnregisterPickup(pickup);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing pickup: {e}");
            }
        }

        public void Dispose()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            disposables.Dispose();
            activePickups.Clear();
        }
    }
}