using UnityEngine;

public class Game_manager : MonoBehaviour
{
	public static Game_manager gameInstance = null;
	private GameObject background;
	private GameObject play_button;

	private Map_manager map;

	void Awake()
    {
        if (gameInstance == null)
			gameInstance = this;
        else if (gameInstance != this)
			Destroy(gameObject);
		map = GetComponent<Map_manager>();
		Init_game();
	}

	public void Init_game()
	{
		background = GameObject.Find("background");
		play_button = GameObject.Find("play_button");
		background.SetActive(false);
		play_button.SetActive(false);
		map.MapSetup();
		Camera.main.transform.position = map.grid.CellToWorld(new Vector3Int(map.startPosition.x, map.startPosition.y, -10));
	}
}