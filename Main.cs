using Godot;
using System;

public partial class Main : Node2D
{
	public const int CellSize = 20;
	public const int CellsHorizontal = 50;
	public const int CellsVertical = 30;

	private Snake snake;
	private PackedScene wallScene;
	
	public override void _Ready()
	{
		snake = GetNode<Snake>("Snake");
		wallScene = GD.Load<PackedScene>("res://Wall.tscn");
		
		GetTree().Root.GetNode<Area2D>("Main/Snake/SnakeSegment")
			.Connect("GameOver", new Callable(this, nameof(OnGameOver)));
		
		CreateWalls();
	}
	
	private void CreateWalls()
	{
		CreateWall(0, 0, CellsHorizontal, 1);
		CreateWall(0, CellsVertical - 1, CellsHorizontal, 1);
		CreateWall(0, 1, 1, CellsVertical - 2);
		CreateWall(CellsHorizontal - 1, 1, 1, CellsVertical - 2);
	}

	private void CreateWall(int startX, int startY, int width, int height)
	{
		for (int i = startX; i < startX + width; i++)
		{
			for (int j = startY; j < startY + height; j++)
			{
				Area2D wall = (Area2D)wallScene.Instantiate();
				wall.Position = new Vector2(i * CellSize, j * CellSize);
				AddChild(wall);
			}
		}
	}
		
	private void OnGameOver()
	{
		CallDeferred(nameof(ChangeSceneGameOver));
	}
	
	private void ChangeSceneGameOver() 
	{
		var gameOverScene = (PackedScene)ResourceLoader.Load("res://GameOver.tscn");
		var gameOverInstance = (GameOver)gameOverScene.Instantiate();
		gameOverInstance.SetScore(snake.Length - Snake.InitialLength);
		var sceneToSet = new PackedScene();
		sceneToSet.Pack(gameOverInstance);
		GetTree().ChangeSceneToPacked(sceneToSet);
	}
		
}
