using Godot;
using System;

public partial class Berry : Area2D
{
	[Signal]
	public delegate void BerryEatenEventHandler();
	
	public override void _Ready()
	{
		Position = GetRandomPosition();
	}
	
	private Vector2 GetRandomPosition()
	{
		Vector2 newPosition = GenerateRandomPosition();
		while (PositionOverlaps(newPosition)) 
		{
			newPosition = GenerateRandomPosition();
		}
		return newPosition;
	}
	
	private Vector2 GenerateRandomPosition() {
		return new Vector2(
			GD.RandRange(1, Main.CellsHorizontal - 2) * Main.CellSize, 
			GD.RandRange(1, Main.CellsVertical - 2) * Main.CellSize
		);
	}
		
	private bool PositionOverlaps(Vector2 position)
	{
		foreach (Node node in GetTree().Root.GetNode<Snake>("Main/Snake").GetChildren())
		{
			if (node is Area2D area && area.Position == position)
			{
				return true;
			}
		}
		return false;
	}
	
	private void OnAreaShapeEntered(Rid areaRid, Area2D area, long areaShapeIndex, long localShapeIndex)
	{
		CallDeferred(nameof(ProcessBerryEaten));
	}
	
	private void ProcessBerryEaten() {
		EmitSignal(nameof(BerryEaten));
		Position = GetRandomPosition();
	}
}
