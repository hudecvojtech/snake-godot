using Godot;
using System;

public partial class SnakeSegment : Area2D
{
	[Signal]
	public delegate void GameOverEventHandler();
	
	private void OnAreaShapeEntered(Rid areaRid, Area2D area, long areaShapeIndex, long localShapeIndex)
	{
		EmitSignal(nameof(GameOver));
	}
}
