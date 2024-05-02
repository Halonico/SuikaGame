using Godot;
using System;

public partial class Fruit : RigidBody2D
{
    public FruitTypes FruitType { get; set; }
    public float Size { get; set; }
    public int Score { get; set; }
    public Player Player { get; set; }
    public float SpriteOffsetX { get; set; } = 0;
    public float SpriteOffsetY { get; set; } = 0;
    public float SpriteScale { get; set; } = 0;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
    public void Init(Player player, FruitTypes type, int score, float size = 0.5f, float spriteOffsetX = 0, float spriteOffsetY = 0, float spriteScale = 0)
    {
        Player = player;
        FruitType = type;
        Score = score;
        Size = size;
        var sprite = GetNode<Sprite2D>("Collision/Sprite");
        GetNode<CollisionShape2D>("Collision").Scale = new Vector2(size, size);
        sprite.Texture = (Texture2D)GD.Load($"res://assets//{FruitType.ToString().ToLower()}.png");
        if (spriteScale > 0)
        {
            sprite.Scale = new Vector2(spriteScale, spriteScale);
        }
        sprite.Offset = new Vector2(spriteOffsetX, spriteOffsetY);
    }
    public static Fruit GenerateFruit(FruitTypes type, Player player)
    {
        var fruitScene = GD.Load<PackedScene>("res://scenes/Fruit.tscn");
        Fruit fruit = fruitScene.Instantiate<Fruit>();
        switch (type)
        {
            case FruitTypes.Cherry:
                fruit.Init(player, FruitTypes.Cherry, 10, 0.2f);
                break;
            case FruitTypes.Strawberry:
                fruit.Init(player, FruitTypes.Strawberry, 20, 0.3f, 2.5f, -5, 1.2f);
                break;
            case FruitTypes.Grape:
                fruit.Init(player, FruitTypes.Grape, 30, 0.5f, 0, 0, 1.2f);
                break;
            case FruitTypes.Lemon:
                fruit.Init(player, FruitTypes.Lemon, 40, 0.6f, 5, -12, 1.4f);
                break;
            case FruitTypes.Orange:
                fruit.Init(player, FruitTypes.Orange, 50, 0.7f, 0, -15, 1.3f);
                break;
            case FruitTypes.Apple:
                fruit.Init(player, FruitTypes.Apple, 60, 0.8f, 5, -10, 1.3f);
                break;
            case FruitTypes.Grapefruit:
                fruit.Init(player, FruitTypes.Grapefruit, 70, 0.85f);
                break;
            case FruitTypes.Peach:
                fruit.Init(player, FruitTypes.Peach, 80, 0.9f, 0, 0, 1.2f);
                break;
            case FruitTypes.Pineapple:
                fruit.Init(player, FruitTypes.Pineapple, 90, 1f);
                break;
            case FruitTypes.Melon:
                fruit.Init(player, FruitTypes.Melon, 100, 1.1f);
                break;
            case FruitTypes.Watermelon:
                fruit.Init(player, FruitTypes.Watermelon, 200, 1.2f);
                break;
            default:
                fruit.Init(player, FruitTypes.Cherry, 10, 0.2f);
                break;
        }
        return fruit;
    }
    private void OnCollision(Node body)
    {
        if (body is not Fruit)
        {
            return;
        }
        var fruit = (Fruit)body;
        //If it's the same fruit, then continue the logic of merging
        if (fruit.FruitType == this.FruitType)
        {
            fruit.Hide();
            Hide();
            fruit.GetNode<CollisionShape2D>("Collision").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
            GetNode<CollisionShape2D>("Collision").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
            if (FruitType != FruitTypes.Watermelon && fruit.FruitType != FruitTypes.Watermelon)
            {
                //When the fruits collides, each of them will call this method, thus, we need to generate a new fruit only once because it will cause an infinite loop
                if (this.GetInstanceId() > fruit.GetInstanceId())
                {
                    var nextFruit = GenerateFruit((FruitTypes)fruit.FruitType + 1, Player);
                    nextFruit.Position = new Vector2((fruit.Position.X + this.Position.X) / 2, (fruit.Position.Y + this.Position.Y) / 2);
                    try
                    {
                        GetParent().CallDeferred("add_child", nextFruit);
                    }
                    catch (Exception ex)
                    {
                        GD.Print(ex.Message);
                    }
                }
            }
            if (this.GetInstanceId() > fruit.GetInstanceId())
                Player.ChangeScore(Score);
        }
    }
}
