using Godot;
using System;

public partial class Player : CharacterBody2D
{
    public Random Rand = new Random();
    public const float Speed = 300.0f;
    public Fruit CurrentItem { get; set; } = new Fruit();
    public Fruit NextItem { get; set; } = new Fruit();
    public Sprite2D CurrentItemSprite = new Sprite2D();
    public int Score { get; set; }
    public Sprite2D NextItemSprite = new Sprite2D();
    public Label ScoreLabel = new Label();
    public bool IsRunning { get; set; } = true;
    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public override void _EnterTree()
    {
        ScoreLabel = GetTree().Root.GetNode<Label>("Game/ScoreControl/ScoreLabel");
        NextItemSprite = GetTree().Root.GetNode<Sprite2D>("Game/FruitControl/FruitSprite");
        CurrentItemSprite = GetNode<Sprite2D>("Fruit");
        CurrentItem = GenerateRandomFruit();
        NextItem = GenerateRandomFruit();
        CurrentItemSprite.Texture = CurrentItem.GetNode<Sprite2D>("Collision/Sprite").Texture;
        NextItemSprite.Texture = NextItem.GetNode<Sprite2D>("Collision/Sprite").Texture;
    }
    private Fruit GenerateRandomFruit()
    {
        return Fruit.GenerateFruit((FruitTypes)Rand.Next((int)FruitTypes.Cherry, (int)FruitTypes.Apple), this);
    }
    private void DropFruit()
    {
        var height = CurrentItem.GetNode<CollisionShape2D>("Collision").Shape.GetRect().Size.Y * CurrentItem.Size;
        CurrentItem.Position = new Vector2(this.Position.X, this.Position.Y + 15 + height);
        GetParent().AddChild(CurrentItem);
        CurrentItem = NextItem;
        CurrentItemSprite.Texture = CurrentItem.GetNode<Sprite2D>("Collision/Sprite").Texture;
        NextItem = GenerateRandomFruit();
        NextItemSprite.Texture = NextItem.GetNode<Sprite2D>("Collision/Sprite").Texture;
    }
    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsRunning)
            DropFruit();
        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
    }
    public void ChangeScore(int score)
    {
        Score += score;
        ScoreLabel.Text = Score.ToString();
    }
    public void OnTopCollision()
    {
        var endBox = GetTree().Root.GetNode<ColorRect>("Game/EndBox");
        endBox.GetNode<Label>("EndScoreLabel").Text = Score.ToString();
        endBox.Visible = true;
        IsRunning = false;

    }
    public void Restart()
    {
        GetTree().ChangeSceneToFile("scenes/Main.tscn");
    }
}
