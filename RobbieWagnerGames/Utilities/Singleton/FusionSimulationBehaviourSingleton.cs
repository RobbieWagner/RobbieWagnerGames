﻿using Fusion;
using System;

namespace Assets.Scripts.RobbieWagnerGames.Utilities.Singleton
{
    public abstract class FusionSimulationBehaviourSingleton<T> : SimulationBehaviour where T : FusionSimulationBehaviourSingleton<T>
    {
        public static event Action<FusionSimulationBehaviourSingleton<T>> OnInstanceSet;
        private static T instance;
        public static T Instance
        {
            get
            {
                return instance;
            }
            protected set
            {
                if (instance == value)
                    return;
                instance = value;
                OnInstanceSet?.Invoke(instance);
            }
        }

        public static bool hasInstance => instance != null;

        protected virtual void Awake()
        {
            if (instance != null)
                Destroy(gameObject);
            else
            {
                instance = (T)this;
                OnInstanceSet?.Invoke(this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }
    }
}
