using Godot;
using System;
using System.Collections.Generic;

public partial class Snake : Node
{
	public const int InitialLength = 5;
	private Vector2 StartPosition = new Vector2(
		((Main.CellsHorizontal / 2) - 1) * Main.CellSize, 
		((Main.CellsVertical / 2) - 1) * Main.CellSize
	);
	
	[Signal]
	public delegate void GameOverEventHandler();
	public int Length = InitialLength;
	
	private Area2D head;
	private List<Area2D> body = new List<Area2D>();
	private Direction direction = Direction.Right;
	private Direction? directionToSet = null;
	
	private Timer moveTimer;
	private PackedScene snakeSegment;
	
	public override void _Ready()
	{
		snakeSegment = GD.Load<PackedScene>("res://SnakeSegment.tscn");
		GetTree().Root.GetNode<Berry>("Main/Berry")
			.Connect("BerryEaten", new Callable(this, nameof(OnBerryEaten)));
		
		GenerateSnake();
	}

	public override void _Process(double delta)
	{
		UpdateDirectionToSet();
	}
	
	private void UpdateDirectionToSet() 
	{
		if (Input.IsActionJustPressed("move_down"))
		{
			directionToSet = Direction.Down;
		}
		if (Input.IsActionJustPressed("move_up"))
		{
			directionToSet = Direction.Up;
		}
		if (Input.IsActionJustPressed("move_left"))
		{
			directionToSet = Direction.Left;
		}
		if (Input.IsActionJustPressed("move_right"))
		{
			directionToSet = Direction.Right;
		}
	}
	
	private void GenerateSnake() 
	{
		head = (Area2D)snakeSegment.Instantiate();
		head.Position = StartPosition;
		AddChild(head);
		
		for (int i = 1; i < InitialLength; i++) {
			Area2D snakeSegmentInstance = (Area2D)snakeSegment.Instantiate();
			snakeSegmentInstance.Position = new Vector2(
				head.Position.X - (i * Main.CellSize),
				head.Position.Y
			);
			body.Add(snakeSegmentInstance);
			AddChild(snakeSegmentInstance);
		}
	}
	
	private void OnBerryEaten()
	{
		Length++;
	}

	private void OnMoveTimerTimeout()
	{
		UpdateDirection();
		UpdateSnakePosition();
	}
	
	private void UpdateDirection() 
	{
		if (directionToSet != null) {
			SetDirection(directionToSet);
			directionToSet = null;
		}
	}
	
	private void SetDirection(Direction? newDirection)
	{
		if (newDirection != null && newDirection != OppositeDirection(direction))
		{
			direction = (Direction)newDirection;
		}
	}

	private static Direction OppositeDirection(Direction direction)
	{
		return direction switch
		{
			Direction.Up => Direction.Down,
			Direction.Down => Direction.Up,
			Direction.Left => Direction.Right,
			Direction.Right => Direction.Left,
			_ => throw new ArgumentException("Invalid direction"),
		};
	}
	
	private void UpdateSnakePosition() 
	{
		Vector2 previousHeadPosition = head.Position;
		head.Position += directionToVector(direction);
		
		Vector2 lastBodyPart = body[body.Count - 1].Position;
		
		for (int i = 0; i < body.Count; i++)
		{
			Vector2 previousPosition = body[i].Position;
			body[i].Position = previousHeadPosition;
			previousHeadPosition = previousPosition;
		}
		if (Length > body.Count + 1) {
			Area2D newBodyPart = (Area2D)snakeSegment.Instantiate();
			newBodyPart.Position = lastBodyPart;
			AddChild(newBodyPart);
			body.Add(newBodyPart);
		}
	}
	
	private Vector2 directionToVector(Direction direction) 
	{
		if (direction == Direction.Up) {
			return new Vector2(0, -Main.CellSize);
		}
		if (direction == Direction.Down) {
			return new Vector2(0, Main.CellSize);
		}
		if (direction == Direction.Left) {
			return new Vector2(-Main.CellSize, 0);
		}
		if (direction == Direction.Right) {
			return new Vector2(Main.CellSize, 0);
		} 
		return Vector2.Zero;
	}
}
