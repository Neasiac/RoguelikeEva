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
using Vegricht.RoguelikeEva.Level;

namespace Vegricht.RoguelikeEva.Scenes
{
    class IntroScene : Scene
    {
        Texture2D Wall;
        Texture2D Floor;
        Texture2D FloorDark;
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
            FloorDark = Content.Load<Texture2D>("tiles dark");
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
                .AddComponent(new CameraFollow(camera.GetComponent<Camera>()))
                .Register(this);
        }

        void CreateMap()
        {
            GameObject[,] tiles = new GameObject[map.GetLength(0), map.GetLength(1)];
            Dictionary<byte, Room> rooms = new Dictionary<byte, Room>();

            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    byte roomID = (byte)(map[x, y] & 0x00FF);
                    byte doorID = (byte)((map[x, y] & 0xFF00) >> 4);

                    if (!rooms.ContainsKey(roomID))
                        rooms.Add(roomID, new Room(roomID));

                    Room room = rooms[roomID];
                    MapNode node = new MapNode(x, y, doorID, room);

                    tiles[x, y] = new GameObjectBuilder()
                        .AddComponent(new Transform(new Vector2(x * Map.Size, y * Map.Size)))
                        .AddComponent(new SpriteRenderer(Wall, .9f))
                        .AddComponent(node)
                        .AddComponent(new Dimentions(0, 0, Map.Size, Map.Size))
                        .AddComponent(new Clickable(() => GlobalScripts.GetComponent<Player>().SelectNode(node)))
                        .Register(this);
                }
            
            Map = new Map(tiles, rooms.Values);
            Map.SetupRooms();
            Map.SetNeighbors();
            Map.SetTileGraphics(Floor, FloorDark, Wall);
            GlobalScripts.GetComponent<MapPanner>().Map = Map;
        }
        
        void CreateCharacters()
        {
            Player player = GlobalScripts.GetComponent<Player>();
            
            GameObject chara1 = new GameObjectBuilder()
                .AddComponent(new Transform(new Vector2(Map.Size * 10, Map.Size * 4)))
                .AddComponent(new SpriteRenderer(Chara))
                .AddComponent(CreateAnimator(1, 0))
                .AddComponent(CreateAnimationStateMachine(Map.Size))
                .AddComponent(new Chara(player, Map[10, 4], Color.Yellow))
                .Register(this);

            GameObject chara2 = new GameObjectBuilder()
                .AddComponent(new Transform(new Vector2(Map.Size * 2, Map.Size)))
                .AddComponent(new SpriteRenderer(Chara))
                .AddComponent(CreateAnimator(0, 1))
                .AddComponent(CreateAnimationStateMachine(Map.Size))
                .AddComponent(new Chara(player, Map[2, 1], Color.Orange))
                .Register(this);

            player.Mode = Player.PlayerMode.Thinking;

            Map[10, 4].OccupiedBy = chara1;
            Map[2, 1].OccupiedBy = chara2;
            Map[10, 4].Room.UpdateGraphics(Room.Visibility.Visible);
            Map[2, 1].Room.UpdateGraphics(Room.Visibility.Visible);

            GlobalScripts.GetComponent<Player>().Charas.Add(chara1.GetComponent<Chara>());
            GlobalScripts.GetComponent<Player>().Charas.Add(chara2.GetComponent<Chara>());
        }

        Animator CreateAnimator(int x, int y, int duration = 200, int size = 48)
        {
            Animator animator = new Animator();

            animator.Animations.Add("idle down",
                new AnimationBuilder()
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4), size, size))
                    .Build()
                );

            animator.Animations.Add("idle left",
                new AnimationBuilder()
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4 + 1), size, size))
                    .Build()
                );

            animator.Animations.Add("idle right",
                new AnimationBuilder()
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4 + 2), size, size))
                    .Build()
                );

            animator.Animations.Add("idle up",
                new AnimationBuilder()
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4 + 3), size, size))
                    .Build()
                );

            animator.Animations.Add("walking down",
                new AnimationBuilder()
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3), size * (y * 4), size, size))
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4), size, size))
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3 + 2), size * (y * 4), size, size))
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3), size * (y * 4), size, size))
                    .Build()
                );

            animator.Animations.Add("walking left",
                new AnimationBuilder()
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3), size * (y * 4 + 1), size, size))
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4 + 1), size, size))
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3 + 2), size * (y * 4 + 1), size, size))
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3), size * (y * 4 + 1), size, size))
                    .Build()
                );

            animator.Animations.Add("walking right",
                new AnimationBuilder()
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3), size * (y * 4 + 2), size, size))
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4 + 2), size, size))
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3 + 2), size * (y * 4 + 2), size, size))
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3), size * (y * 4 + 2), size, size))
                    .Build()
                );

            animator.Animations.Add("walking up",
                new AnimationBuilder()
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3), size * (y * 4 + 3), size, size))
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4 + 3), size, size))
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3 + 2), size * (y * 4 + 3), size, size))
                    .AddFrame(Chara, duration, new Rectangle(size * (x * 3), size * (y * 4 + 3), size, size))
                    .Build()
                );

            animator.Play("idle down");
            return animator;
        }

        AnimationStateMachine CreateAnimationStateMachine(int tileSize)
        {
            AnimationStateMachine asm = new AnimationStateMachine("idle down");

            Predicate<Chara> up = chara => chara.Path != null && chara.Tile != null && chara.CurrentWaypoint < chara.Path.Length &&
                (chara.Tile.Position - chara.Path[chara.CurrentWaypoint].Position + new Vector2(0, -tileSize)).LengthSquared() < 1e-6;

            Predicate<Chara> left = chara => chara.Path != null && chara.Tile != null && chara.CurrentWaypoint < chara.Path.Length &&
                (chara.Tile.Position - chara.Path[chara.CurrentWaypoint].Position + new Vector2(-tileSize, 0)).LengthSquared() < 1e-6;

            Predicate<Chara> right = chara => chara.Path != null && chara.Tile != null && chara.CurrentWaypoint < chara.Path.Length &&
                (chara.Tile.Position - chara.Path[chara.CurrentWaypoint].Position + new Vector2(tileSize, 0)).LengthSquared() < 1e-6;

            Predicate<Chara> down = chara => chara.Path != null && chara.Tile != null && chara.CurrentWaypoint < chara.Path.Length &&
                (chara.Tile.Position - chara.Path[chara.CurrentWaypoint].Position + new Vector2(0, tileSize)).LengthSquared() < 1e-6;

            asm.AddTransition("idle down", "walking up", up);
            asm.AddTransition("idle left", "walking up", up);
            asm.AddTransition("idle right", "walking up", up);
            asm.AddTransition("idle up", "walking up", up);
            asm.AddTransition("walking left", "walking up", up);
            asm.AddTransition("walking right", "walking up", up);

            asm.AddTransition("idle down", "walking left", left);
            asm.AddTransition("idle left", "walking left", left);
            asm.AddTransition("idle right", "walking left", left);
            asm.AddTransition("idle up", "walking left", left);
            asm.AddTransition("walking up", "walking left", left);
            asm.AddTransition("walking down", "walking left", left);

            asm.AddTransition("idle down", "walking right", right);
            asm.AddTransition("idle left", "walking right", right);
            asm.AddTransition("idle right", "walking right", right);
            asm.AddTransition("idle up", "walking right", right);
            asm.AddTransition("walking up", "walking right", right);
            asm.AddTransition("walking down", "walking right", right);

            asm.AddTransition("idle down", "walking down", down);
            asm.AddTransition("idle left", "walking down", down);
            asm.AddTransition("idle right", "walking down", down);
            asm.AddTransition("idle up", "walking down", down);
            asm.AddTransition("walking left", "walking down", down);
            asm.AddTransition("walking right", "walking down", down);

            asm.AddTransition("walking down", "idle down", chara => chara.Path == null);
            asm.AddTransition("walking left", "idle left", chara => chara.Path == null);
            asm.AddTransition("walking right", "idle right", chara => chara.Path == null);
            asm.AddTransition("walking up", "idle up", chara => chara.Path == null);

            return asm;
        }
    }
}