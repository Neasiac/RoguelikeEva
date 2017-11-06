using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;

namespace Vegricht.RoguelikeEva.Scenes.Core
{
    /// <summary>
    /// Represents an object in the virtual world, which consists of a set of components.
    /// </summary>
    sealed class GameObject : IEnumerable<Component>
    {
        public event Action OnActivatedQueue;
        public event Action OnDeactivatedQueue;

        List<Component> Components;
        bool _active;
        
        public GameObject()
        {
            Components = new List<Component>();
            Active = true;
        }

        public bool Active
        {
            get { return _active; }
            
            set
            {
                if (_active == value)
                    return;

                _active = value;

                // If we're being activated, invoke all callbacks from components which have been activated or deactivated since this object's deactivation
                if (value)
                {
                    OnActivatedQueue?.Invoke();
                    OnActivatedQueue = null;

                    OnDeactivatedQueue?.Invoke();
                    OnDeactivatedQueue = null;
                }

                // Aditionally, call apropriate callbacks on all active components
                foreach (Component component in Components)
                    if (component.Active)
                    {
                        if (value) component.OnActivated();
                        else       component.OnDeactivated();
                    }
            }
        }
        
        public void AddComponent(Component component)
        {
            Components.Add(component);
        }

        public T GetComponent<T>() where T : Component
        {
            return GetComponent<T>(0);
        }

        public T GetComponent<T>(int index) where T : Component
        {
            // Get i-th component of a given type
            int i = 0;

            foreach (Component component in Components)
                if (component is T && i++ == index)
                    return (T)component;

            return null;
        }

        public void Update(GameTime gameTime)
        {
            foreach (Component component in Components)
                if(component.Active)
                    component.Update(gameTime);
        }

        public IEnumerator<Component> GetEnumerator()
        {
            foreach (Component component in Components)
                yield return component;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }
}