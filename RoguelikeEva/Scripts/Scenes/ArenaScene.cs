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
    class ArenaScene : Scene
    {
        Texture2D Wall;
        Texture2D Floor;
        Texture2D FloorDark;
        Texture2D Chara;
        Texture2D Monsters;
        Texture2D Items;
        Texture2D NextTurn;
        Texture2D Sword;
        SpriteFont Font;

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

        GameObject Camera;
        GameObject GlobalScripts;
        Map Map;

        public override void LoadResources(ContentManager Content)
        {
            Wall = Content.Load<Texture2D>("wall");
            Floor = Content.Load<Texture2D>("tiles");
            FloorDark = Content.Load<Texture2D>("tiles dark");
            Chara = Content.Load<Texture2D>("chara");
            Monsters = Content.Load<Texture2D>("monsters");
            Items = Content.Load<Texture2D>("items");
            NextTurn = Content.Load<Texture2D>("nextturn");
            Sword = Content.Load<Texture2D>("sword");
            Font = Content.Load<SpriteFont>("georgia");
        }

        public override void OnInitiate()
        {
            CreateMechanics();
            CreateUI();
            CreateMap();
            CreateCharacters();
            CreateMonsters();
            CreateItems();
        }

        void CreateMechanics()
        {
            Camera = new GameObjectBuilder()
                .AddComponent(new Transform(Vector2.Zero))
                .AddComponent(new Camera())
                .Register(this);

            GameObject sword = new GameObjectBuilder()
                .AddComponent(new Transform(new Vector2(1 * Map.Size, 1 * Map.Size)))
                .AddComponent(new SpriteRenderer(Sword, new Rectangle(0, 0, Map.Size, Map.Size), .1f))
                .AddComponent(CreateSwordAnimator())
                .Register(this);

            sword.Active = false;

            GlobalScripts = new GameObjectBuilder()
                .AddComponent(new Player())
                .AddComponent(new MapPanner(Camera.GetComponent<Camera>()))
                .AddComponent(new TurnManager())
                .AddComponent(new CombatManager(sword))
                .Register(this);
        }

        void CreateUI()
        {
            Camera camera = Camera.GetComponent<Camera>();
            FontRenderer font = new FontRenderer(Font, "Hello World!", 0);

            GameObject nextturn = new GameObjectBuilder()
                .AddComponent(new Transform(Vector2.Zero, new Vector2(.2f)))
                .AddComponent(new SpriteRenderer(NextTurn, 0))
                .AddComponent(new Dimentions(0, 0, 50, 50))
                .AddComponent(new Clickable(GlobalScripts.GetComponent<TurnManager>().NextTurn))
                .AddComponent(new CameraFollow(camera, Vector2.Zero))
                .Register(this);

            GameObject infobox = new GameObjectBuilder()
                .AddComponent(new Transform(Vector2.Zero))
                .AddComponent(font)
                .AddComponent(new CameraFollow(camera, new Vector2(0, SceneManager.Instance.ViewportRectangle.Height - 5 * Font.LineSpacing)))
                .Register(this);
            
            font.Active = false;
            GlobalScripts.GetComponent<Player>().Infobox = font;
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
            GlobalScripts.GetComponent<Player>().Mode = Player.PlayerMode.Thinking;
            CreateHero("Lemon", 1, 0, 10, 4, Color.Yellow, 8, 10, 4, CombatManager.CombatType.Lizard);
            CreateHero("Orange", 0, 1, 2, 1, Color.Orange, 8, 10, 4, CombatManager.CombatType.Spock);
        }

        void CreateMonsters()
        {
            MonsterPrototype prototype = new MonsterPrototype("Snake Monster", 8, 15, 4, CombatManager.CombatType.Paper);
            CreateMonster(prototype, 1, 0, 2, 3);
            //CreateMonster(prototype, 1, 0, 9, 9);
        }

        void CreateItems()
        {
            Player player = GlobalScripts.GetComponent<Player>();
            Item itemComponent = new Item(chara =>
                {
                    Character.Status spd = chara.Speed;
                    spd.Remaining = spd.Max;
                    chara.Speed = spd;
                });

            GameObject item1 = new GameObjectBuilder()
                .AddComponent(new Transform(new Vector2(Map.Size * 4, Map.Size * 3)))
                .AddComponent(new SpriteRenderer(Items, new Rectangle(Map.Size * 9, Map.Size * 4, Map.Size, Map.Size)))
                .AddComponent(itemComponent)
                .AddComponent(new Dimentions(0, 0, Map.Size, Map.Size))
                .AddComponent(new Hoverable(() => player.RequestInfoboxOverride(
                        "<Poition of Speed>" + Environment.NewLine +
                        "Fills your speed to max.",
                    itemComponent), () => player.InvalidateInfoboxOverride(itemComponent)))
                .Register(this);

            Map[4, 3].OccupiedBy = item1;
        }

        GameObject CreateHero(string name, int spriteX, int spriteY, int posX, int posY, Color col, int speed, int hp, int atk, CombatManager.CombatType type)
        {
            Hero heroComponent = new Hero(name, GlobalScripts, Map[posX, posY], col, speed, hp, atk, type);
            Player player = GlobalScripts.GetComponent<Player>();

            GameObject hero = new GameObjectBuilder()
                .AddComponent(new Transform(new Vector2(Map.Size * posX, Map.Size * posY)))
                .AddComponent(new SpriteRenderer(Chara))
                .AddComponent(CreateAnimator(Chara, spriteX, spriteY))
                .AddComponent(CreateAnimationStateMachine(Map.Size))
                .AddComponent(heroComponent)
                .AddComponent(new Dimentions(0, 0, Map.Size, Map.Size))
                .AddComponent(new Hoverable(() => player.RequestInfoboxOverride(
                        "<" + heroComponent.Name + ">" + Environment.NewLine +
                        "HP: " + heroComponent.HitPoints.Remaining + " / " + heroComponent.HitPoints.Max + Environment.NewLine +
                        "Speed: " + heroComponent.Speed.Max + Environment.NewLine +
                        "Strength: " + heroComponent.Strength + Environment.NewLine +
                        "Type: " + heroComponent.Type,
                    heroComponent), () => player.InvalidateInfoboxOverride(heroComponent)))
                .Register(this);
            
            Map[posX, posY].OccupiedBy = hero;
            Map[posX, posY].Room.UpdateGraphics(Room.Visibility.Visible);

            GlobalScripts.GetComponent<TurnManager>().Heroes.Add(heroComponent);
            return hero;
        }

        GameObject CreateMonster(MonsterPrototype prototype, int spriteX, int spriteY, int posX, int posY)
        {
            Monster monsterComponent = new Monster(GlobalScripts, Map[posX, posY], prototype);
            Player player = GlobalScripts.GetComponent<Player>();

            GameObject monster = new GameObjectBuilder()
                .AddComponent(new Transform(new Vector2(Map.Size * posX, Map.Size * posY)))
                .AddComponent(new SpriteRenderer(Monsters))
                .AddComponent(CreateAnimator(Monsters, spriteX, spriteY))
                .AddComponent(CreateAnimationStateMachine(Map.Size))
                .AddComponent(monsterComponent)
                .AddComponent(new Dimentions(0, 0, Map.Size, Map.Size))
                .AddComponent(new Hoverable(() => player.RequestInfoboxOverride(
                        "<" + monsterComponent.Species + ">" + Environment.NewLine +
                        "HP: " + monsterComponent.HitPoints.Remaining + " / " + monsterComponent.HitPoints.Max + Environment.NewLine +
                        "Speed: " + monsterComponent.Speed.Max + Environment.NewLine +
                        "Strength: " + monsterComponent.Strength + Environment.NewLine +
                        "Possible Types: " + String.Join(", ", monsterComponent.Species.EnemyAwareness),
                            monsterComponent), () => player.InvalidateInfoboxOverride(monsterComponent)))
                .Register(this);

            Map[posX, posY].OccupiedBy = monster;

            if (Map[posX, posY].Room.View != Room.Visibility.Visible)
                monster.GetComponent<SpriteRenderer>().Active = false;

            GlobalScripts.GetComponent<TurnManager>().Monsters.Add(monsterComponent);
            return monster;
        }
        
        Animator CreateSwordAnimator(int duration = 30, int size = 48)
        {
            Animator animator = new Animator();
            Animation rotate = new Animation();

            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 5 && y < 3 || x < 3 && y == 3; x++)
                    rotate.Frames.Add(new Frame(Sword, duration, new Rectangle(size * x, size * y, size, size)));

            animator.Animations.Add("rotate", rotate);
            animator.Play("rotate");
            return animator;
        }

        Animator CreateAnimator(Texture2D spriteset, int x, int y, int duration = 200, int size = 48)
        {
            Animator animator = new Animator();

            animator.Animations.Add("idle down",
                new AnimationBuilder()
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4), size, size))
                    .Build()
                );

            animator.Animations.Add("idle left",
                new AnimationBuilder()
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4 + 1), size, size))
                    .Build()
                );

            animator.Animations.Add("idle right",
                new AnimationBuilder()
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4 + 2), size, size))
                    .Build()
                );

            animator.Animations.Add("idle up",
                new AnimationBuilder()
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4 + 3), size, size))
                    .Build()
                );

            animator.Animations.Add("walking down",
                new AnimationBuilder()
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3), size * (y * 4), size, size))
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4), size, size))
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3 + 2), size * (y * 4), size, size))
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3), size * (y * 4), size, size))
                    .Build()
                );

            animator.Animations.Add("walking left",
                new AnimationBuilder()
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3), size * (y * 4 + 1), size, size))
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4 + 1), size, size))
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3 + 2), size * (y * 4 + 1), size, size))
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3), size * (y * 4 + 1), size, size))
                    .Build()
                );

            animator.Animations.Add("walking right",
                new AnimationBuilder()
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3), size * (y * 4 + 2), size, size))
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4 + 2), size, size))
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3 + 2), size * (y * 4 + 2), size, size))
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3), size * (y * 4 + 2), size, size))
                    .Build()
                );

            animator.Animations.Add("walking up",
                new AnimationBuilder()
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3), size * (y * 4 + 3), size, size))
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3 + 1), size * (y * 4 + 3), size, size))
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3 + 2), size * (y * 4 + 3), size, size))
                    .AddFrame(spriteset, duration, new Rectangle(size * (x * 3), size * (y * 4 + 3), size, size))
                    .Build()
                );

            animator.Play("idle down");
            return animator;
        }

        AnimationStateMachine CreateAnimationStateMachine(int tileSize)
        {
            AnimationStateMachine asm = new AnimationStateMachine("idle down");

            Predicate<Character> up = chara => chara.Path != null && chara.Tile != null && chara.CurrentWaypoint < chara.Path.Length &&
                (chara.Tile.Position - chara.Path[chara.CurrentWaypoint].Position + new Vector2(0, -tileSize)).LengthSquared() < 1e-6;

            Predicate<Character> left = chara => chara.Path != null && chara.Tile != null && chara.CurrentWaypoint < chara.Path.Length &&
                (chara.Tile.Position - chara.Path[chara.CurrentWaypoint].Position + new Vector2(-tileSize, 0)).LengthSquared() < 1e-6;

            Predicate<Character> right = chara => chara.Path != null && chara.Tile != null && chara.CurrentWaypoint < chara.Path.Length &&
                (chara.Tile.Position - chara.Path[chara.CurrentWaypoint].Position + new Vector2(tileSize, 0)).LengthSquared() < 1e-6;

            Predicate<Character> down = chara => chara.Path != null && chara.Tile != null && chara.CurrentWaypoint < chara.Path.Length &&
                (chara.Tile.Position - chara.Path[chara.CurrentWaypoint].Position + new Vector2(0, tileSize)).LengthSquared() < 1e-6;

            asm.AddTransition("idle down", "walking up", up);
            asm.AddTransition("idle left", "walking up", up);
            asm.AddTransition("idle right", "walking up", up);
            asm.AddTransition("idle up", "walking up", up);

            asm.AddTransition("walking down", "walking up", up);
            asm.AddTransition("walking left", "walking up", up);
            asm.AddTransition("walking right", "walking up", up);

            asm.AddTransition("idle down", "walking left", left);
            asm.AddTransition("idle left", "walking left", left);
            asm.AddTransition("idle right", "walking left", left);
            asm.AddTransition("idle up", "walking left", left);
            
            asm.AddTransition("walking up", "walking left", left);
            asm.AddTransition("walking down", "walking left", left);
            asm.AddTransition("walking right", "walking left", left);

            asm.AddTransition("idle down", "walking right", right);
            asm.AddTransition("idle left", "walking right", right);
            asm.AddTransition("idle right", "walking right", right);
            asm.AddTransition("idle up", "walking right", right);

            asm.AddTransition("walking up", "walking right", right);
            asm.AddTransition("walking down", "walking right", right);
            asm.AddTransition("walking left", "walking right", right);

            asm.AddTransition("idle down", "walking down", down);
            asm.AddTransition("idle left", "walking down", down);
            asm.AddTransition("idle right", "walking down", down);
            asm.AddTransition("idle up", "walking down", down);

            asm.AddTransition("walking up", "walking down", down);
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