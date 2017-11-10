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
        Texture2D NextTurn;

        ushort[,] map = new ushort[30, 20]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 2, 2, 2, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 1, 1, 1, 1, 0, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 1, 1, 1, 1, 0, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 1, 1,     1,      1, 0, 0, 0x0602, 2, 2, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 1, 0x0101,0x0201, 1, 5, 5, 0x0605, 5, 5, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 0, 0x0103,0x0203, 0, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 0, 0x0303,0x0403, 0, 0x0505, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 4, 0x0304,0x0404, 4, 0x0504, 4, 0x0704, 0x0706, 6, 6, 6, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 4, 4, 4, 4, 4, 4, 4, 6, 6, 6, 6, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 4, 4, 0, 0, 0, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 4, 4, 0, 4, 0, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };

        GameObject GlobalScripts;
        Map Map;

        public override void LoadResources(ContentManager Content)
        {
            Wall = Content.Load<Texture2D>("wall");
            Floor = Content.Load<Texture2D>("tiles");
            Chara = Content.Load<Texture2D>("chara");
            NextTurn = Content.Load<Texture2D>("nextturn");
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

            GameObject nextturn = new GameObjectBuilder()
                .AddComponent(new Transform(Vector2.Zero, new Vector2(.2f)))
                .AddComponent(new SpriteRenderer(NextTurn, 0))
                .AddComponent(new Dimentions(0, 0, 50, 50))
                .AddComponent(new Clickable(GlobalScripts.GetComponent<Player>().NextTurn))
                .Register(this);
        }

        void CreateMap()
        {
            GameObject[,] tiles = new GameObject[30, 20];

            for (int x = 0; x < 30; x++)
                for (int y = 0; y < 20; y++)
                {
                    MapNode node = new MapNode(x, y, map[x, y]);

                    tiles[x, y] = new GameObjectBuilder()
                        .AddComponent(new Transform(new Vector2(x * Map.Size, y * Map.Size), new Vector2(0.16f)))
                        .AddComponent(new SpriteRenderer(Wall, .9f))
                        .AddComponent(node)
                        .AddComponent(new Dimentions(0, 0, Map.Size, Map.Size))
                        .AddComponent(new Clickable(() => GlobalScripts.GetComponent<Player>().SelectNode(node)))
                        .Register(this);
                }

            Map = new Map(tiles);
            Map.SetNeighbors();
            Map.SetTileGraphics(Floor);
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
                .AddComponent(new Transform(new Vector2(Map.Size * 10, Map.Size * 4)))
                .AddComponent(new SpriteRenderer(Chara))
                .AddComponent(anim1)
                .AddComponent(new Chara(player, Map[10, 4], Color.Yellow))
                .Register(this);

            GameObject chara2 = new GameObjectBuilder()
                .AddComponent(new Transform(new Vector2(Map.Size * 2, Map.Size)))
                .AddComponent(new SpriteRenderer(Chara))
                .AddComponent(anim2)
                .AddComponent(new Chara(player, Map[2, 1], Color.Orange))
                .Register(this);

            player.Mode = Player.PlayerMode.Thinking;
            Map[10, 4].OccupiedBy = chara1;
            Map[2, 1].OccupiedBy = chara2;

            GlobalScripts.GetComponent<Player>().Charas.Add(chara1.GetComponent<Chara>());
            GlobalScripts.GetComponent<Player>().Charas.Add(chara2.GetComponent<Chara>());
        }
    }
}