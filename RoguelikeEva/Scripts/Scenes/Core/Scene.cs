using Microsoft.Xna.Framework.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Components.Core;

namespace Vegricht.RoguelikeEva.Scenes.Core
{
    /// <summary>
    /// Represents container of GameObjects that interact with each other to create
    /// user experience.
    /// </summary>
    abstract class Scene : IEnumerable<GameObject>
    {
        List<GameObject> Objects = new List<GameObject>();

        public abstract void OnInitiate();
        public abstract void LoadResources(ContentManager Content);

        public void Initiate(ContentManager Content)
        {
            // Create all the GameObjects etc...
            LoadResources(Content);
            OnInitiate();

            foreach (GameObject obj in Objects)
                foreach (Component component in obj)
                {
                    component.OnStart();

                    // Notify SceneManager that this GameObject wishes to be rendered
                    if (component is RenderableComponent)
                        SceneManager.Instance.RegisterRenderableComponent((RenderableComponent)component);

                    if (obj.Active && component.Active)
                        component.OnActivated();
                }
        }

        public void AddGameObject(GameObject gameObject)
        {
            Objects.Add(gameObject);
        }
        
        public IEnumerator<GameObject> GetEnumerator()
        {
            foreach (GameObject obj in Objects)
                yield return obj;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }
}