using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vegricht.RoguelikeEva.Components.Core;

namespace Vegricht.RoguelikeEva.Scenes.Core
{
    /// <summary>
    /// Helps to create GameObjects.
    /// </summary>
    class GameObjectBuilder
    {
        GameObject GameObject;

        public GameObjectBuilder()
        {
            GameObject = new GameObject();
        }

        public GameObjectBuilder AddComponent(Component component)
        {
            return AddComponent(component, true);
        }

        public GameObjectBuilder AddComponent(Component component, bool condition)
        {
            if (condition)
                GameObject.AddComponent(component);

            return this;
        }

        public GameObject Register(Scene scene)
        {
            scene.AddGameObject(GameObject);
            return GameObject;
        }
    }
}