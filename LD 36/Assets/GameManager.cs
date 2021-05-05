using UnityEngine;
using System.Collections;
using CardGame;
using CardGame.CardComponents;

public class GameManager : MonoBehaviour {

    static GameManager the_one_and_only;


    private static Vector3 Player_stock_position = new Vector3(7.05f, 0, -1.87f);
    public static Vector3 Player_show_card = new Vector3(2.1f, 6.0f, -0.8f);
    public static Vector3 Player_tooltip_card = new Vector3(-2.1f, 6.0f, -0.8f);
    private static Vector3 Player_hand_position = new Vector3(0, 4, -3f);
    private static Vector3 Player_board_position = new Vector3(-0.44f, 4.5f, -1.0f);
    private static Vector3 Player_supplies_position = new Vector3(7.37f, 0, -0.46f);
    public static float Player_hand_threshold = -2.1f;

    private static Vector3 Enemy_stock_position = new Vector3(7.05f, 0, 1.87f);
    public static Vector3  Enemy_show_card = new Vector3(2.1f, 6.0f, 0.8f);
    public static Vector3  Enemy_tooltip_card = new Vector3(-2.1f, 6.0f, 0.8f);
    private static Vector3 Enemy_hand_position = new Vector3(0, 4, 3f);
    private static Vector3 Enemy_board_position = new Vector3(-0.44f, 4.5f, 1.0f);
    private static Vector3 Enemy_supplies_position = new Vector3(-7.37f, 0, 0.46f);

    public static Vector3 Hide_cards_position = new Vector3(0,20,0);
    

    private Hand _player_hand;
    public Hand Player_hand { get { return _player_hand; } private set { _player_hand = value; } }

    private Board _player_board;
    public Board Player_board { get { return _player_board; } private set { _player_board = value; } }

    private Stock _player_stock;
    public Stock Player_stock { get { return _player_stock; } private set { _player_stock = value; } }

    private Stock _enemy_stock;
    public Stock Enemy_stock { get { return _enemy_stock; } private set { _enemy_stock = value; } }

    private Hand _enemy_hand;
    public Hand Enemy_hand { get { return _enemy_hand; } private set { _enemy_hand = value; } }

    private Board _enemy_board;
    public Board Enemy_board { get { return _enemy_board; } private set { _enemy_board = value; } }

    public EnemyAI Enemy_AI { get; set; }

    public Timer Timer { get; set; }

    public Supplies Player_Supplies { get; set; }
    public Supplies Enemy_Supplies { get; set; }

    public GameQueue Game_Queue { get; set; }

    public bool Player_Turn { get; set; }

    public System.Random Random { get; set; }

    public static GameManager GetInstance() { return the_one_and_only; }

    public bool AncientEvilSummoned = false;

	// Use this for initialization
	void Awake () {
	    if(the_one_and_only == null)
        {
            the_one_and_only = this;

            Random = new System.Random();

            CreateTimer();
            CreateSupplyBars();
            CreateDesk();
            CreateStocks();
            CreateHands();
            CreateBoards();
            CreateAI();


            Game_Queue = gameObject.AddComponent<GameQueue>();

            Game_Queue.StartPlayerTurn();
            Player_Turn = true;
        }
        else
        {
            Destroy(this);
        }
	}

    private void CreateDesk()
    {
        GameObject desk = GameObject.Instantiate(Resources.Load("Desk") as GameObject);
        desk.name = "Desk";
        desk.transform.FindChild("PlayerStock").transform.position = Player_stock_position;
        desk.transform.FindChild("EnemyStock").transform.position = Enemy_stock_position;
    }

    private void CreateTimer()
    {
        GameObject timer_go = GameObject.Instantiate(Resources.Load("Timer") as GameObject);
        Timer = timer_go.AddComponent<Timer>();
    }

    private void CreateSupplyBars()
    {
        GameObject player_go = new GameObject("PlayerSupplies");
        player_go.transform.position = Player_supplies_position;
        Player_Supplies = player_go.AddComponent<Supplies>();
        Player_Supplies.Create();

        GameObject enemy_go = new GameObject("EnemySupplies");
        Enemy_Supplies = enemy_go.AddComponent<Supplies>();
        enemy_go.transform.position = Enemy_supplies_position;
        Enemy_Supplies.Player_Owned = false;
        Enemy_Supplies.Create();
    }

    private void CreateStocks()
    {
        GameObject player_stock_go = new GameObject("PlayerStock");
        _player_stock = player_stock_go.AddComponent<Stock>();

        player_stock_go.transform.position = Player_stock_position;

        _player_stock.Fill();

        GameObject enemy_stock_go = new GameObject("EnemyStock");
        _enemy_stock = enemy_stock_go.AddComponent<Stock>();

        enemy_stock_go.transform.position = Enemy_stock_position;
        _enemy_stock.Player_stock = false;
        _enemy_stock.Fill();
    }

    private void CreateBoards()
    {
        GameObject player_board_go = new GameObject("PlayerBoard");
        _player_board = player_board_go.AddComponent<Board>();
        player_board_go.transform.position = Player_board_position;

        GameObject enemy_board_go = new GameObject("EnemyBoard");
        _enemy_board = enemy_board_go.AddComponent<Board>();
        enemy_board_go.transform.position = Enemy_board_position;
    }

    private void CreateHands()
    {
        GameObject player_hand_go = new GameObject("PlayerHand");
        _player_hand = player_hand_go.AddComponent<Hand>();

        player_hand_go.transform.position = Player_hand_position;

        _player_hand.Fill();

        GameObject enemy_hand_go = new GameObject("EnemyHand");
        _enemy_hand = enemy_hand_go.AddComponent<Hand>();
        _enemy_hand.Player_hand = false;

        enemy_hand_go.transform.position = Enemy_hand_position;

        _enemy_hand.Fill();
    }

    private void CreateAI()
    {
        GameObject enemy_ai_go = new GameObject();
        enemy_ai_go.transform.parent = gameObject.transform;
        Enemy_AI = enemy_ai_go.AddComponent<EnemyAI>();
    }


    public void ManipulationEnabled(bool value)
    {
        _player_hand.ManipulationEnabled(value);
        Timer.ManipulationEnable(value);
    }

    public void InspectionEnabled(bool value)
    {
        _player_board.InspectionEnabled(value);
        _enemy_board.InspectionEnabled(value);
    }
}
