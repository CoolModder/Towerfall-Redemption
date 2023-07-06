using System.Collections.Generic;
using Monocle;
using TowerFall;

namespace TowerBall;

public class SceneHelper
{
	public static void addItemsToScene(Scene scene, List<MenuItem> items)
	{
		scene.Add(items);
	}

	public static void addItemsToScene(Scene scene, List<MapButton> items)
	{
		scene.Add(items);
	}
}
