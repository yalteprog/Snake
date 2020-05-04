using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public Camera cam;
    private int[][] world;
    private int[][] snake;
    public int direction;
    public int speed;
    private int world_size;
    private string world_type;
    ArrayList objects;
    public UIController ui_controller;
    public GameObject black;
    public GameObject block;
    public GameObject food;
    public GameObject snake_part;
    public GameObject snake_head;
    public string game_status;
    private float timeAfterMove;
    // Start is called before the first frame update
    void Start()
    {
        game_status = "menu";
        StartCoroutine(ui_controller.showStartMenu());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (game_status == "death") { if (ui_controller.areSettingsReady() == true) { game_status = "menu"; } }
       else if (game_status == "menu") { if (ui_controller.areSettingsReady()==true) { game_status = "start"; } }
        else if (game_status == "start") {
            world_size = ui_controller.getSize();
            world_type = ui_controller.getType();

            objects = new ArrayList();
            world = createWorld();
            timeAfterMove = 0f;
            speed = 7;
        direction = 1;  
        snake=createSnake();
        createFood();
        showWorld();
        game_status = "game";
        }
        else if (game_status == "game")
        {
            timeAfterMove += Time.deltaTime;
            if (timeAfterMove >= (float)((11f - speed)/ 10f))
            {  
                makeMove();
                timeAfterMove = 0f;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                changeDirection(1);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                changeDirection(2);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                changeDirection(3);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                changeDirection(4);
            }
        }
    }
    private int[][] createSnake() {
        int[][] snake;
        int snake_length = 0;
        int head_hor=0;
        int head_vert = 0; 
    for(int i = 0; i < world.Length; i++)
        {
            for (int e = 0; e < world[i].Length; e++) {
            if(world[i][e]!=0&& world[i][e] != 10)
                {
                    snake_length++;
                    if (world[i][e] == 5) { head_vert = i; head_hor = e; }
                }
            }
        }
        int[] point= new int[] { head_vert, head_hor };
        snake = new int[snake_length][];
        snake[snake_length - 1] =point;
        for (int i = 2; i <= snake.Length; i++)
        {
            point = findNeighbour(point) ;
            snake[snake.Length - i] = point;
        }
        return snake;
    }



    private int[] findNeighbour( int[] point)
    {
        int px = point[1];
        int py = point[0];
        int dirc = world[py][px];
        for (int i = point[0] - 1; i <= point[0] + 1; i++)
        {
            for (int e = point[1] - 1; e <= point[1] + 1; e++)
            {
                int x = e;
                int y = i;
               
                if (y < 0) y = world.Length - 1;
                if (y > world.Length - 1) y = 0;
                if (x < 0) x = world[y].Length - 1;
                if (x > world[y].Length - 1) x = 0;
                if (world[y][x] >0&&world[y][x]<10)
                {
                    if (e != point[1] && i != point[0]) { continue; }
                    //1-right
                    //2-left
                    //3-top
                    //4-bottom
                    if (dirc == 1 && (e == point[1] + 1)) { continue; }
                    if (dirc == 2 && e == point[1] - 1) { continue; }
                    if (dirc == 3 && i == point[0] - 1) { continue; }
                    if (dirc == 4 && i == point[0] + 1) { continue; }
                    else {
                        if (e == point[1] && i == point[0]) { continue;  }
                        if ((i == py + 1 && world[y][x] == 3) || (i == py - 1 && world[y][x] == 4) || (e == px - 1 && world[y][x] == 1) || (e == px + 1 && world[y][x] == 2))
                        {
                            return new int[] { y, x };
                        } }
                }
            }
        }
        return null;
    }
    private void makeMove()
    {
        int[] point = snake[snake.Length - 1];
        int x1 = point[1];
        int y1 = point[0];
        if (direction == 1) { x1++;if (x1 == world[y1].Length) x1 = 0; }
        else if (direction == 2) { x1--; if (x1 ==-1 ) x1 = world[y1].Length-1; }
        else if (direction == 3) { y1--; if (y1 == -1) y1 = world.Length - 1;  }
        else if (direction == 4) { y1++; if (y1 ==world.Length) y1 = 0; }

        int[][] snake2 = null;
        if (world[y1][x1] > 10)
        {
            snake2 = new int[snake.Length + 1][];
            eat();
            snake2[snake.Length] =new int[] { y1, x1 };
            for (int i = 0; i < snake.Length; i++) { snake2[i] = snake[i]; }
            world[y1][x1] = 5;
            world[point[0]][point[1]] = direction;
            snake = snake2;
            createFood();
        }
        else if (world[y1][x1] > 0 && (snake[0][0] != y1 || snake[0][1] != x1)) {
           
           
                gameOver();
            
        }

        else {
            snake2 = new int[snake.Length][];
            snake2[snake.Length-1] = new int[] { y1, x1 };
            
            world[point[0]][point[1]] = direction;
            for (int i = 2; i <= snake.Length; i++) { snake2[snake.Length - i] = snake[snake.Length - i + 1];

            }
            world[snake[0][0]] [snake[0][1]] = 0;
            world[y1][x1] = 5;
            snake = snake2;
        }
        if (world_size != 0)
        {
            showWorld();
        }

    }

    private void gameOver() {
        foreach (GameObject obj1 in objects) Destroy(obj1);
        objects = new ArrayList();
        world = null;
        world_size = 0;


        direction = 0;
        speed = 0;
        direction = 0;
        snake = null;
        game_status = "death";
        StartCoroutine(ui_controller.showDeathMenu());
        


        }
    private void eat() { }
 
    private void changeDirection(int dir) {
        if ((direction == 2 && dir == 1) || (direction == 1 && dir == 2) || (direction == 3 && dir == 4) || (direction == 4 && dir == 3) ||( direction == dir)) { }
        else direction = dir;
    }

    private int[][] createWorld()
    {   int[][] world= new int[][]{ };
        if (world_type == "usual" && world_size == 10)
        {
            world = new int[][]{new int[]{ 0,0,0,0,0,0,0,0,0,0},
                                new int[] { 0,0,0,0,0,0,0,0,0,0},
                                 new int[]{ 0,0,0,0,0,0,0,0,0,0},
                                 new int[]{ 0,0,0,0,0,0,0,0,0,0},
                                 new int[]{ 0,0,0,0,0,0,0,0,0,0},
                                new int[] { 0,0,0,0,0,0,0,0,0,0},
                                 new int[]{ 0,0,0,0,0,0,0,0,0,0},
                                 new int[]{ 0,0,1,1,5,0,0,0,0,0},
                                new int[] { 0,0,0,0,0,0,0,0,0,0},
                                new int[] { 0,0,0,0,0,0,0,0,0,0}};
        }
        else if (world_type == "box"&&world_size==10)
        {
            world = new int[][]{new int[]{ 10, 10, 10, 10,10,10,10,10,10,10},
                                new int[] { 10,0,0,0,0,0,0,0,0,10},
                                 new int[]{ 10,0,0,0,0,0,0,0,0,10},
                                 new int[]{ 10,0,0,0,0,0,0,0,0,10},
                                 new int[]{ 10,0,0,0,0,0,0,0,0,10},
                                new int[] { 10,0,0,0,0,0,0,0,0,10},
                                 new int[]{ 10,0,0,0,0,0,0,0,0,10},
                                 new int[]{ 10,0,1,1,5,0,0,0,0,10},
                                new int[] { 10,0,0,0,0,0,0,0,0,10},
                                new int[] { 10,10,10,10,10,10,10,10,10,10}};
        }
        else if (world_type == "box" && world_size == 15)
        {
            world = new int[][]{new int[]{ 10, 10, 10, 10,10,10,10,10,10,10,10,10,10,10,10},
                                new int[] { 10,0,0,0,0,0,0,0, 0, 0, 0, 0, 0,0, 10},
                                 new int[]{ 10,0,0,0,0,0,0,0, 0, 0, 0, 0, 0,0, 10},
                                 new int[]{ 10,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,10},
                                 new int[]{ 10,0,0,0,0,0,0,0, 0, 0, 0, 0, 0,0, 10},
                                new int[] { 10,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,10},
                                 new int[]{ 10,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,10},
                                 new int[] { 10,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,10},
                                 new int[] { 10,0,0,0,0,0,0, 0, 0, 0, 0, 0,0, 0,10},
                                 new int[] { 10,0,0,0,0,0,0, 0, 0, 0, 0, 0,0, 0,10},
                                 new int[]{ 10,0,1,1,5,0,0, 0, 0, 0, 0, 0,0, 0,10},
                                new int[] { 10,0,0,0,0,0,0, 0, 0, 0, 0, 0,0, 0,10},
                                new int[] { 10,0,0,0,0,0,0, 0, 0, 0, 0, 0,0, 0,10},
                                new int[] { 10,0,0,0,0,0,0, 0, 0, 0, 0, 0,0, 0,10},
                                new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10}};
        }
        else if (world_type == "usual" && world_size ==15)
        {
            world = new int[][]{new int[]{ 0,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,0},
                                new int[]{ 0,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,0},
                                 new int[]{ 0,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,0},
                                 new int[]{ 0,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,0},
                              new int[]{ 0,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,0},
                                new int[]{ 0,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,0},
                              new int[]{ 0,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,0},
                              new int[]{ 0,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,0},
                                new int[]{ 0,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,0},
                                 new int[]{ 0,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,0},
                                 new int[]{ 0,0,1,1,5,0,0, 0, 0, 0, 0, 0,0, 0,0},
                               new int[]{ 0,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,0},
                                new int[]{ 0,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,0},
                                new int[]{ 0,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,0},
                               new int[]{ 0,0,0,0,0,0,0, 0,0, 0, 0, 0, 0, 0,0} };
        }
        return world;
    }
    private void createFood()
    { int length = 0;
        float step = cam.pixelHeight / world_size;
        for (int i = 0; i < world.Length; i++)
        {
            for (int e = 0; e < world[i].Length; e++) {
                if (world[i][e] == 0) length++;
            }
        }
        int[][] food_arr = new int[length][];
        int y = 0;
        for (int i = 0; i < world.Length; i++)
        {
            for (int e = 0; e < world[i].Length; e++)
            {
                if (world[i][e] == 0)
                {
                    food_arr[y] = new int[] { i, e };
                    y++;
                }
            }
        }
        System.Random rand = new System.Random();
        int n = rand.Next(length);
        world[food_arr[n][0]][food_arr[n][1]] = 11;
        GameObject obj = Instantiate(food);
        float helpingStep = (float)world_size / 2;
        obj.transform.localScale = new Vector2(step / food.GetComponent<SpriteRenderer>().sprite.rect.size.x, step / food.GetComponent<SpriteRenderer>().sprite.rect.size.y);
        obj.transform.position = new Vector2((food_arr[n][1]) * step + 0.5f * step -helpingStep* step, (world.Length -1-food_arr[n][0]) * step + 0.5f * step - world_size * step);
        objects.Add(obj);
    }
    private void showWorld()
    {
        if (objects.Count != 0) { foreach (GameObject obj1 in objects) Destroy(obj1); }
        float orthographicSize = cam.pixelHeight / 2;
        if (orthographicSize != cam.orthographicSize)
            cam.orthographicSize = orthographicSize;
    
        float stepFromLeft = (cam.pixelWidth - cam.pixelHeight) / 2;
        float helpingStep = (float)world_size / 2;
        black.GetComponent<BoxCollider2D>().size = new Vector2(cam.pixelHeight / world_size, cam.pixelHeight/ world_size) ;
        float step = cam.pixelHeight / world_size;
        black.transform.localScale = new Vector2(world_size * step /black.GetComponent<SpriteRenderer>().sprite.rect.size.x, world_size * step / black.GetComponent<SpriteRenderer>().sprite.rect.size.y);
        GameObject obj = Instantiate(black);
        objects.Add(obj);
        obj.transform.position = new Vector2(0, 0 );
          for (int i = 0; i < world_size; i++)
          {
              for(int e = 0; e < world_size; e++)
              {if (world[world_size - 1 - i][e] == 5)
                {
                    GameObject obj4 = Instantiate(snake_head);
                    obj4.transform.localScale = new Vector2(step / snake_head.GetComponent<SpriteRenderer>().sprite.rect.size.x, step / snake_head.GetComponent<SpriteRenderer>().sprite.rect.size.y);
                    obj4.transform.position = new Vector2((e) * step + 0.5f * step - helpingStep * step, (i) * step + 0.5f * step - helpingStep * step);
                    objects.Add(obj4);
                    if(direction==1) { obj4.transform.Rotate(0, 0, 0); }
                    else if (direction == 3) { obj4.transform.Rotate(0, 0, 90); }
                   else if (direction == 4) { obj4.transform.Rotate(0, 180, 270); }
                    else if (direction == 2) { obj4.transform.Rotate(180, 0, 180); }
                }
                else if (world[world_size-1 - i][e] > 0 && world[world_size-1 - i][e] < 10)
                {
                    GameObject obj2 = Instantiate(snake_part);
                    GameObject obj22 = Instantiate(snake_part);
                    obj2.transform.localScale = new Vector2(step / black.GetComponent<SpriteRenderer>().sprite.rect.size.x, step / black.GetComponent<SpriteRenderer>().sprite.rect.size.y);
                    obj22.transform.localScale = new Vector2(step / black.GetComponent<SpriteRenderer>().sprite.rect.size.x, step / black.GetComponent<SpriteRenderer>().sprite.rect.size.y);
                    
                    if (world[world_size - 1 - i][e]==1&&e!= world[world_size - 1 - i].Length-1) { obj22.transform.position = new Vector2((e) * step + 0.5f * step +0.5f * step -helpingStep* step, (i) * step + 0.5f * step -helpingStep* step);}
                  else if (world[world_size - 1 - i][e]==2 && e !=0) { obj22.transform.position = new Vector2((e) * step + 0.5f * step-0.5f * step -helpingStep* step, (i) * step + 0.5f * step -helpingStep* step);
                    }
                    else if (world[world_size - 1 - i][e]==3 && i != world.Length-1) { obj22.transform.position = new Vector2((e) * step + 0.5f * step -helpingStep* step, (i) * step + 0.5f * step +0.5f * step -helpingStep* step); }
                    else if (world[world_size - 1 - i][e]== 4&& i != 0) { obj22.transform.position = new Vector2((e) * step + 0.5f * step -helpingStep* step, (i) * step + 0.5f * step -0.5f * step-helpingStep* step); }
                    else  {}
                    obj2.transform.position = new Vector2((e) * step + 0.5f * step -helpingStep* step, (i) * step + 0.5f * step -helpingStep* step);
                    objects.Add(obj2);
                    objects.Add(obj22);

                }
                else if (world[world_size - 1 - i][e] == 10)
                {
                    GameObject obj4 = Instantiate(block);
                    obj4.transform.localScale = new Vector2(step / block.GetComponent<SpriteRenderer>().sprite.rect.size.x, step / block.GetComponent<SpriteRenderer>().sprite.rect.size.y);
                    obj4.transform.position = new Vector2((e) * step + 0.5f * step -helpingStep* step, (i) * step + 0.5f * step -helpingStep* step);
                    objects.Add(obj4);
                }
                else if (world[world_size - 1 - i][e] > 10)
                {
                    GameObject obj3 = Instantiate(food);
                    obj3.transform.localScale = new Vector2(step / food.GetComponent<SpriteRenderer>().sprite.rect.size.x, step / food.GetComponent<SpriteRenderer>().sprite.rect.size.y);
                    obj3.transform.position = new Vector2((e) * step + 0.5f * step -helpingStep* step, (i) * step + 0.5f * step -helpingStep* step);
                    objects.Add(obj3);
                }
             

              }
          }
    }

}
