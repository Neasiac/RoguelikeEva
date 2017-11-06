using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vegricht.RoguelikeEva.Scenes.Core;
using Vegricht.RoguelikeEva.Components;
using Microsoft.Xna.Framework.Content;
using Vegricht.RoguelikeEva.Animations;
using Vegricht.RoguelikeEva.Pathfinding;
using System.Collections.Generic;

namespace Vegricht.RoguelikeEva.Scenes
{
    class IntroScene : Scene
    {
        Texture2D Wall;
        Texture2D Floor;
        Texture2D Chara;

        int[,] map = new int[30, 20]
            {
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            };

        GameObject GlobalScripts;

        public override void LoadResources(ContentManager Content)
        {
            Wall = Content.Load<Texture2D>("wall");
            Floor = Content.Load<Texture2D>("floor");
            Chara = Content.Load<Texture2D>("chara");
        }

        public override void OnInitiate()
        {
            CreateMechanics();
            CreateMap();
            CreateCharacters();
        }

        void CreateMechanics()
        {
            GameObject camera = new GameObjectBuilder()
                .AddComponent(new Transform(Vector2.Zero))
                .AddComponent(new Camera())
                .Register(this);

            GlobalScripts = new GameObjectBuilder()
                .AddComponent(new Player())
                .AddComponent(new MapPanner(camera.GetComponent<Camera>()))
                .Register(this);
        }

        void CreateMap()
        {
            GameObject[,] tiles = new GameObject[30, 20];

            for (int x = 0; x < 30; x++)
                for (int y = 0; y < 20; y++)
                {
                    Texture2D sprite = map[x, y] == 1 ? Floor : Wall;
                    MapNode node = new MapNode(x, y, map[x, y]);

                    tiles[x, y] = new GameObjectBuilder()
                        .AddComponent(new Transform(new Vector2(x * MapNode.Size, y * MapNode.Size), new Vector2(0.16f, 0.16f)))
                        .AddComponent(new SpriteRenderer(sprite))
                        .AddComponent(node)
                        .AddComponent(new Dimentions(0, 0, MapNode.Size, MapNode.Size))
                        .AddComponent(new Clickable(() => GlobalScripts.GetComponent<Player>().SelectNode(node)))
                        .Register(this);
                }

            MapNode[,] nodes = new MapNode[30, 20];

            for (int x = 0; x < 30; x++)
                for (int y = 0; y < 20; y++)
                    nodes[x, y] = tiles[x, y].GetComponent<MapNode>();

            MapNode.Map = nodes;
        }
        
        void CreateCharacters()
        {
            Player player = GlobalScripts.GetComponent<Player>();
            Animator anim1 = new Animator();
            Animator anim2 = new Animator();

            anim1.Animations.Add("walking",
                new AnimationBuilder()
                    .AddFrame(Chara, 200, new Rectangle(144, 0, 48, 48))
                    .AddFrame(Chara, 200, new Rectangle(192, 0, 48, 48))
                    .AddFrame(Chara, 200, new Rectangle(240, 0, 48, 48))
                    .AddFrame(Chara, 200, new Rectangle(192, 0, 48, 48))
                    .Build()
                );

            anim2.Animations.Add("walking",
                new AnimationBuilder()
                    .AddFrame(Chara, 200, new Rectangle(0, 192, 48, 48))
                    .AddFrame(Chara, 200, new Rectangle(48, 192, 48, 48))
                    .AddFrame(Chara, 200, new Rectangle(96, 192, 48, 48))
                    .AddFrame(Chara, 200, new Rectangle(48, 192, 48, 48))
                    .Build()
                );

            anim1.Play("walking");
            anim2.Play("walking");

            GameObject chara1 = new GameObjectBuilder()
                .AddComponent(new Transform(new Vector2(MapNode.Size * 10, MapNode.Size * 4)))
                .AddComponent(new SpriteRenderer(Chara, .4f))
                .AddComponent(anim1)
                .AddComponent(new Chara(player, MapNode.Map[10, 4], Color.Yellow))
                .Register(this);

            GameObject chara2 = new GameObjectBuilder()
                .AddComponent(new Transform(new Vector2(MapNode.Size * 2, MapNode.Size)))
                .AddComponent(new SpriteRenderer(Chara, .4f))
                .AddComponent(anim2)
                .AddComponent(new Chara(player, MapNode.Map[2, 1], Color.Orange))
                .Register(this);

            player.Mode = Player.PlayerMode.Thinking;
            MapNode.Map[10, 4].OccupiedBy = chara1;
            MapNode.Map[2, 1].OccupiedBy = chara2;
        }
    }
}