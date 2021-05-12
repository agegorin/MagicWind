using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

enum TurnState
{
    Player_Turn,
    Player_Wait,
    Enemy_Turn,
    Enemy_Wait,
    Game_Over
}

public enum PlayerAttacks
{
    OneLeft,
    OneRight,
    AllLeft,
    AllRight,
    Cross,
    Whirpool,
    WhirpoolRight,
    None
}

public class GameManager : MonoBehaviour
{
    private TurnState currentTurn;
    private float lastTimeTurnChange;
    private float waitTime = 0.7f;

    private List<Vector2Int> enemiesPositions = new List<Vector2Int>();
    private List<Enemy> enemies = new List<Enemy>();

    private PlayerAttacks currentAttack = PlayerAttacks.None;
    private int score;
    private int lives;

    [SerializeField] private GameObject enemiesField;
    [SerializeField] private Enemy skeletonPrefab;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private SkillButtons skillButtons;
    [SerializeField] private GameObject aimArrowPrefab;
    [SerializeField] private TMP_Text uiLives;
    [SerializeField] private TMP_Text uiScore;
    [SerializeField] private TMP_Text uiScoreGameOver;
    [SerializeField] private GameObject GameOverScreen;


    private Vector3 safePoint = new Vector3(-10, -10, 0);
    private List<GameObject> arrows = new List<GameObject>();
    private Vector2Int vRight = new Vector2Int(1, 0);
    private Vector2Int vDown = new Vector2Int(0, -1);
    private Vector2Int vLeft = new Vector2Int(-1, 0);
    private Vector2Int vUp = new Vector2Int(0, 1);


    void Start()
    {
        score = 0;
        lives = 10;

        currentTurn = TurnState.Enemy_Turn;

        int cols = gridManager.getGridCols();
        cols = cols < 4 ? 4 : cols; // we need minimum 4 arrows for cross and squares attacks;
        for (int i = 0; i < cols; i++)
        {
            GameObject arrow = Instantiate(aimArrowPrefab, inGameUI.transform);
            arrow.transform.position = safePoint;
            arrows.Add(arrow);
        }


        enemiesPositions.Add(new Vector2Int(0, 1));
        enemiesPositions.Add(new Vector2Int(3, 0));
        enemiesPositions.Add(new Vector2Int(1, 2));
        enemies.Add(Instantiate(skeletonPrefab, enemiesField.transform.position, skeletonPrefab.transform.rotation, enemiesField.transform));
        enemies.Add(Instantiate(skeletonPrefab, enemiesField.transform.position, skeletonPrefab.transform.rotation, enemiesField.transform));
        enemies.Add(Instantiate(skeletonPrefab, enemiesField.transform.position, skeletonPrefab.transform.rotation, enemiesField.transform));

    }

    void Update()
    {
        //if (Input.GetKeyDown("q")) currentAttack = PlayerAttacks.AllLeft;
        //if (Input.GetKeyDown("w")) currentAttack = PlayerAttacks.Cross;
        //if (Input.GetKeyDown("e")) currentAttack = PlayerAttacks.AllRight;
        //if (Input.GetKeyDown("a")) currentAttack = PlayerAttacks.OneLeft;
        //if (Input.GetKeyDown("d")) currentAttack = PlayerAttacks.OneRight;
        //if (Input.GetKeyDown("s")) currentAttack = PlayerAttacks.Whirpool;
        //if (Input.GetKeyDown("x")) currentAttack = PlayerAttacks.WhirpoolRight;

        uiScore.text = "" + score;
        uiLives.text = "" + lives;

        // TEMP TURN CHANGER
        if (
            Input.GetKeyDown("space") &&
            currentTurn != TurnState.Enemy_Wait &&
            currentTurn != TurnState.Player_Wait
        )
        {
            nextTurn();
        }

        if (currentTurn == TurnState.Enemy_Turn)
        {
            for (int i = enemies.Count - 1; i >= 0; i--) // remove destroied enemies
            {
                if (enemies[i] == null)
                {
                    enemies.RemoveAt(i);
                    enemiesPositions.RemoveAt(i);
                }
            }

            for (int i = 0; i < enemies.Count; i++) // move all enemies forward
            {
                enemiesPositions[i] = new Vector2Int(enemiesPositions[i].x, enemiesPositions[i].y + 1);
                enemies[i].StartMove(gridManager.toWorldPos(enemiesPositions[i]));

                if (enemiesPositions[i].y == gridManager.getGridRows() + 1) lives -= 1;
                if (lives <= 0)
                {
                    currentTurn = TurnState.Game_Over;
                    uiScoreGameOver.text = "" + score;
                    GameOverScreen.SetActive(true);
                }
            }

            nextTurn();
        }

        // PLAYER TURN
        if (
            currentTurn == TurnState.Player_Turn
        )
        {
            // SHOW AIM
            Vector2Int mouseOnGrid = gridManager.toGridPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (gridManager.isInGrid(mouseOnGrid))
            {
                for(int i = 0; i < arrows.Count; i++) // пр€чем все за экран, чтобы не оставались стрелки при переключении режимов.
                {
                    arrows[i].transform.position = safePoint;
                }

                if (currentAttack == PlayerAttacks.OneRight)
                {
                    arrows[0].transform.position = gridManager.toWorldPos(mouseOnGrid);
                    arrows[0].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
                else if (currentAttack == PlayerAttacks.OneLeft)
                {
                    arrows[0].transform.position = gridManager.toWorldPos(mouseOnGrid);
                    arrows[0].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                }
                else if (currentAttack == PlayerAttacks.AllRight)
                {
                    for (int i = 0; i < gridManager.getGridCols(); i++)
                    {
                        arrows[i].transform.position = gridManager.toWorldPos(new Vector2Int(i, mouseOnGrid.y));
                        arrows[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    }
                }
                else if (currentAttack == PlayerAttacks.AllLeft)
                {
                    for (int i = 0; i < gridManager.getGridCols(); i++)
                    {
                        arrows[i].transform.position = gridManager.toWorldPos(new Vector2Int(i, mouseOnGrid.y));
                        arrows[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                    }
                }
                else if (currentAttack == PlayerAttacks.Cross)
                {
                    if (mouseOnGrid.x + 1 < gridManager.getGridCols())
                    {
                        arrows[0].transform.position = gridManager.toWorldPos(new Vector2Int(mouseOnGrid.x + 1, mouseOnGrid.y));
                        arrows[0].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    }

                    if (mouseOnGrid.y - 1 >= 0)
                    {
                        arrows[1].transform.position = gridManager.toWorldPos(new Vector2Int(mouseOnGrid.x, mouseOnGrid.y - 1));
                        arrows[1].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    }

                    if (mouseOnGrid.x - 1 >= 0)
                    {
                        arrows[2].transform.position = gridManager.toWorldPos(new Vector2Int(mouseOnGrid.x - 1, mouseOnGrid.y));
                        arrows[2].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                    }

                    if (mouseOnGrid.y + 1 < gridManager.getGridRows())
                    {
                        arrows[3].transform.position = gridManager.toWorldPos(new Vector2Int(mouseOnGrid.x, mouseOnGrid.y + 1));
                        arrows[3].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                    }
                }
                else if (currentAttack == PlayerAttacks.Whirpool)
                {
                    if (mouseOnGrid.x + 1 < gridManager.getGridCols())
                    {
                        arrows[0].transform.position = gridManager.toWorldPos(new Vector2Int(mouseOnGrid.x + 1, mouseOnGrid.y));
                        arrows[0].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    }

                    if (mouseOnGrid.y - 1 >= 0)
                    {
                        arrows[1].transform.position = gridManager.toWorldPos(new Vector2Int(mouseOnGrid.x, mouseOnGrid.y - 1));
                        arrows[1].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                    }

                    if (mouseOnGrid.x - 1 >= 0)
                    {
                        arrows[2].transform.position = gridManager.toWorldPos(new Vector2Int(mouseOnGrid.x - 1, mouseOnGrid.y));
                        arrows[2].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                    }

                    if (mouseOnGrid.y + 1 < gridManager.getGridRows())
                    {
                        arrows[3].transform.position = gridManager.toWorldPos(new Vector2Int(mouseOnGrid.x, mouseOnGrid.y + 1));
                        arrows[3].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    }
                }
                else if (currentAttack == PlayerAttacks.WhirpoolRight)
                {
                    if (mouseOnGrid.x + 1 < gridManager.getGridCols())
                    {
                        arrows[0].transform.position = gridManager.toWorldPos(new Vector2Int(mouseOnGrid.x + 1, mouseOnGrid.y));
                        arrows[0].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                    }

                    if (mouseOnGrid.y - 1 >= 0)
                    {
                        arrows[1].transform.position = gridManager.toWorldPos(new Vector2Int(mouseOnGrid.x, mouseOnGrid.y - 1));
                        arrows[1].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    }

                    if (mouseOnGrid.x - 1 >= 0)
                    {
                        arrows[2].transform.position = gridManager.toWorldPos(new Vector2Int(mouseOnGrid.x - 1, mouseOnGrid.y));
                        arrows[2].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    }

                    if (mouseOnGrid.y + 1 < gridManager.getGridRows())
                    {
                        arrows[3].transform.position = gridManager.toWorldPos(new Vector2Int(mouseOnGrid.x, mouseOnGrid.y + 1));
                        arrows[3].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                    }
                }
            }
            else
            {
                for (int i = 0; i < gridManager.getGridCols(); i++)
                {
                    arrows[i].transform.position = safePoint;
                }
            }

            // Player fire
            if (Input.GetMouseButtonDown(0))
            {
                Vector2Int aim = gridManager.toGridPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                // атакуем только если выбран скилл и кликнули по полю
                if (currentAttack != PlayerAttacks.None && aim.x >= 0 && aim.x < gridManager.getGridCols() && aim.y >= 0 && aim.y < gridManager.getGridRows())
                {
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (
                            (currentAttack == PlayerAttacks.AllLeft && enemiesPositions[i].y == aim.y) ||
                            (currentAttack == PlayerAttacks.OneLeft && enemiesPositions[i].x == aim.x && enemiesPositions[i].y == aim.y) ||
                            (currentAttack == PlayerAttacks.Cross && enemiesPositions[i].x == aim.x - 1 && enemiesPositions[i].y == aim.y) ||
                            (currentAttack == PlayerAttacks.Whirpool && enemiesPositions[i].x == aim.x && enemiesPositions[i].y == aim.y - 1) ||
                            (currentAttack == PlayerAttacks.WhirpoolRight && enemiesPositions[i].x == aim.x && enemiesPositions[i].y == aim.y + 1)
                        )
                        {
                            enemiesPositions[i] = enemiesPositions[i] + vLeft;
                        }
                        else if (
                            (currentAttack == PlayerAttacks.AllRight && enemiesPositions[i].y == aim.y) ||
                            (currentAttack == PlayerAttacks.OneRight && enemiesPositions[i].x == aim.x && enemiesPositions[i].y == aim.y) ||
                            (currentAttack == PlayerAttacks.Cross && enemiesPositions[i].x == aim.x + 1 && enemiesPositions[i].y == aim.y) ||
                            (currentAttack == PlayerAttacks.Whirpool && enemiesPositions[i].x == aim.x && enemiesPositions[i].y == aim.y + 1) ||
                            (currentAttack == PlayerAttacks.WhirpoolRight && enemiesPositions[i].x == aim.x && enemiesPositions[i].y == aim.y - 1)
                        )
                        {
                            enemiesPositions[i] = enemiesPositions[i] + vRight;
                        }
                        else if (
                            (currentAttack == PlayerAttacks.Cross && enemiesPositions[i].x == aim.x && enemiesPositions[i].y == aim.y + 1) ||
                            (currentAttack == PlayerAttacks.Whirpool && enemiesPositions[i].x == aim.x - 1 && enemiesPositions[i].y == aim.y) ||
                            (currentAttack == PlayerAttacks.WhirpoolRight && enemiesPositions[i].x == aim.x + 1 && enemiesPositions[i].y == aim.y)
                        )
                        {
                            enemiesPositions[i] = enemiesPositions[i] + vUp;
                        }
                        else if (
                            (currentAttack == PlayerAttacks.Cross && enemiesPositions[i].x == aim.x && enemiesPositions[i].y == aim.y - 1) ||
                            (currentAttack == PlayerAttacks.Whirpool && enemiesPositions[i].x == aim.x + 1 && enemiesPositions[i].y == aim.y) ||
                            (currentAttack == PlayerAttacks.WhirpoolRight && enemiesPositions[i].x == aim.x - 1 && enemiesPositions[i].y == aim.y)
                        )
                        {
                            enemiesPositions[i] = enemiesPositions[i] + vDown;
                        }

                        if (enemies[i] != null)
                        {
                            if (
                                (enemiesPositions[i].x == -1 || enemiesPositions[i].x == gridManager.getGridCols()) &&
                                (enemiesPositions[i].y == 0 || enemiesPositions[i].y == 1)
                            )
                            { // кра€ сверху карты
                                enemies[i].WindMove(gridManager.toWorldPos(enemiesPositions[i]), false);
                                enemies[i].GetDamage();
                                score += 10;
                            } else
                            {
                                bool isFall = enemiesPositions[i].x < 0 || enemiesPositions[i].x >= gridManager.getGridCols();
                                enemies[i].WindMove(gridManager.toWorldPos(enemiesPositions[i]), isFall);
                                if (isFall) score += 10;
                            }
                            
                        }
                    }

                    for (int i = 0; i < enemies.Count; i++)
                    {
                        for (int j = 0; j < enemies.Count; j++)
                        {
                            if (i != j && enemiesPositions[i] == enemiesPositions[j])
                            {
                                if (enemies[i] != null) enemies[i].GetDamage();
                                if (enemies[j] != null) enemies[j].GetDamage();
                                score += 50;
                            }
                        }
                    }

                    skillButtons.DestroySelected();
                    currentAttack = PlayerAttacks.None;
                    nextTurn();
                }
            }
        }

        if (currentTurn != TurnState.Player_Turn)
        {
            for (int i = 0; i < gridManager.getGridCols(); i++)
            {
                arrows[i].transform.position = safePoint;
            }
        }

        // AFTER PLAYER TURN WAIT
        if (
            currentTurn == TurnState.Player_Wait &&
            (Time.time - lastTimeTurnChange) > waitTime
        )
        {
            nextTurn();
        }

        // AFTER ENEMY TURN WAIT
        if (
            currentTurn == TurnState.Enemy_Wait &&
            (Time.time - lastTimeTurnChange) > waitTime
        )
        {
            nextTurn();
        }
    }

    void applyTurn()
    {
        lastTimeTurnChange = Time.time;
        switch (currentTurn)
        {
            case TurnState.Player_Turn:
                // show actions
                break;
            case TurnState.Player_Wait:
                // spawn mobs
                SpawnEnemy();
                break;
            case TurnState.Enemy_Turn:
                
                break;
            case TurnState.Enemy_Wait:

                break;
        }
    }

    void nextTurn()
    {
        switch (currentTurn)
        {
            case TurnState.Player_Turn:
                currentTurn = TurnState.Player_Wait;
                break;
            case TurnState.Player_Wait:
                currentTurn = TurnState.Enemy_Turn;
                break;
            case TurnState.Enemy_Turn:
                currentTurn = TurnState.Enemy_Wait;
                break;
            case TurnState.Enemy_Wait:
                currentTurn = TurnState.Player_Turn;
                score += 5; // for each wave;
                break;
        }
        applyTurn();
    }

    void SpawnEnemy()
    {
        float rand = Random.Range(0, 1f);

        if (rand < 0.1f) return;

        if (rand < 0.3f)
        {

            Vector2Int newEnemyPosition1 = new Vector2Int(0, -1);
            Vector2Int newEnemyPosition2 = new Vector2Int(3, -1);

            Enemy newEnemy1 = Instantiate(skeletonPrefab, enemiesField.transform.position, skeletonPrefab.transform.rotation, enemiesField.transform);
            Enemy newEnemy2 = Instantiate(skeletonPrefab, enemiesField.transform.position, skeletonPrefab.transform.rotation, enemiesField.transform);
            newEnemy1.StartMove(gridManager.toWorldPos(newEnemyPosition1));
            newEnemy2.StartMove(gridManager.toWorldPos(newEnemyPosition2));

            enemiesPositions.Add(newEnemyPosition1);
            enemiesPositions.Add(newEnemyPosition2);
            enemies.Add(newEnemy1);
            enemies.Add(newEnemy2);

            return;
        }

        if (rand < 0.4f)
        {

            Vector2Int newEnemyPosition1 = new Vector2Int(0, -1);
            Vector2Int newEnemyPosition2 = new Vector2Int(3, -1);
            Vector2Int newEnemyPosition3 = new Vector2Int(2, -1);

            Enemy newEnemy1 = Instantiate(skeletonPrefab, enemiesField.transform.position, skeletonPrefab.transform.rotation, enemiesField.transform);
            Enemy newEnemy2 = Instantiate(skeletonPrefab, enemiesField.transform.position, skeletonPrefab.transform.rotation, enemiesField.transform);
            Enemy newEnemy3 = Instantiate(skeletonPrefab, enemiesField.transform.position, skeletonPrefab.transform.rotation, enemiesField.transform);
            newEnemy1.StartMove(gridManager.toWorldPos(newEnemyPosition1));
            newEnemy2.StartMove(gridManager.toWorldPos(newEnemyPosition2));
            newEnemy3.StartMove(gridManager.toWorldPos(newEnemyPosition3));

            enemiesPositions.Add(newEnemyPosition1);
            enemiesPositions.Add(newEnemyPosition2);
            enemiesPositions.Add(newEnemyPosition3);
            enemies.Add(newEnemy1);
            enemies.Add(newEnemy2);
            enemies.Add(newEnemy3);

            return;
        }

        Vector2Int newEnemyPosition = new Vector2Int(gridManager.getRandomCol(), -1);

        Enemy newEnemy = Instantiate(skeletonPrefab, enemiesField.transform.position, skeletonPrefab.transform.rotation, enemiesField.transform);
        newEnemy.StartMove(gridManager.toWorldPos(newEnemyPosition));

        enemiesPositions.Add(newEnemyPosition);
        enemies.Add(newEnemy);
    }

    public void SelectAttackType(PlayerAttacks type, SkillButton button)
    {
        if (currentTurn != TurnState.Game_Over)
        {
            currentAttack = type;
            skillButtons.SelectButton(button);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
