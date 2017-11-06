using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components;
using Vegricht.RoguelikeEva.Scenes.Core;

namespace Vegricht.RoguelikeEva.Components.Core
{
    /// <summary>
    /// Ancestor for any component. Provides handful of useful methods to comunicate with
    /// other components at apropriate timing with little effort.
    /// </summary>
    abstract class Component
    {
        bool _active;

        virtual public void OnActivated() { }
        virtual public void OnDeactivated() { }
        virtual public void OnStart() { } // Guaranteed to not be called until all component constructors have been called
        virtual public void Update(GameTime gameTime) { }
        virtual public void OnCollisionEnter(Vector2 direction, Transform other) { }

        public GameObject Parent { get; private set; }

        public bool Active
        {
            // We're active if both the component and parent GameObject are active
            get
            {
                if (!Parent.Active)
                    return false;

                return _active;
            }

            // Change the inner active state
            set
            {
                if (_active == value)
                    return;

                _active = value;

                // Call apropriate callbacks if parent GameObject is active
                if (Parent.Active)
                {
                    if (value) OnActivated();
                    else OnDeactivated();
                }

                // Otherwise schedule apropriate callbacks to be invoked when it becomes active
                else
                {
                    if (value) Parent.OnActivatedQueue += OnActivated;
                    else Parent.OnDeactivatedQueue += OnDeactivated;
                }
            }
        }

        public T GetComponent<T>() where T : Component
        {
            return GetComponent<T>(0);
        }

        public T GetComponent<T>(int index) where T : Component
        {
            // Get i-th component of a given type
            int i = 0;

            foreach (Component component in Parent)
                if (component is T && i++ == index)
                    return (T)component;

            return null;
        }

        public void OnInitiated(GameObject parent)
        {
            Parent = parent;
            Active = true;

            OnStart();
        }

        // Some helper shortcut methods
        protected SceneManager SceneManager
        {
            get
            {
                return SceneManager.Instance;
            }
        }

        protected Scene GetScene()
        {
            return SceneManager.CurrentScene;
        }

        protected void LoadScene(Scene scene)
        {
            SceneManager.SetScene(scene);
        }

        protected void TerminateGame()
        {
            SceneManager.TerminateGame();
        }
    }
}