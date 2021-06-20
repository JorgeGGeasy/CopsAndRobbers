using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    //GameObjects
    public GameObject board;
    public GameObject[] cops = new GameObject[2];
    public GameObject robber;
    public Text rounds;
    public Text finalMessage;
    public Button playAgainButton;

    //Otras variables
    Tile[] tiles = new Tile[Constants.NumTiles];
    private int roundCount = 0;
    private int state;
    private int clickedTile = -1;
    private int clickedCop = 0;
                    
    void Start()
    {        
        InitTiles();
        InitAdjacencyLists();
        state = Constants.Init;
    }
        
    //Rellenamos el array de casillas y posicionamos las fichas
    void InitTiles()
    {
        for (int fil = 0; fil < Constants.TilesPerRow; fil++)
        {
            GameObject rowchild = board.transform.GetChild(fil).gameObject;            

            for (int col = 0; col < Constants.TilesPerRow; col++)
            {
                GameObject tilechild = rowchild.transform.GetChild(col).gameObject;                
                tiles[fil * Constants.TilesPerRow + col] = tilechild.GetComponent<Tile>();                         
            }
        }
                
        cops[0].GetComponent<CopMove>().currentTile=Constants.InitialCop0;
        cops[1].GetComponent<CopMove>().currentTile=Constants.InitialCop1;
        robber.GetComponent<RobberMove>().currentTile=Constants.InitialRobber;           
    }

    public void InitAdjacencyLists()
    {
        //Matriz de adyacencia
        int[,] matriu = new int[Constants.NumTiles, Constants.NumTiles];
        //int[,] matriu = new int[Constants.TilesPerRow, Constants.TilesPerRow];

        //TODO: Inicializar matriz a 0's        
        
        for(int i=0; i < Constants.NumTiles; i++)
        {
            for(int j = 0; j < Constants.NumTiles; j++)
            {
                matriu[i, j] = 0;
            }
        }
        
        //TODO: Para cada posición, rellenar con 1's las casillas adyacentes (arriba, abajo, izquierda y derecha)
        

        //TODO: Rellenar la lista "adjacency" de cada casilla con los índices de sus casillas adyacentes
        for(int i = 0; i < Constants.NumTiles; i++)
        {

            if(i == 0 || i == 8 || i == 16 || i == 24 || i == 32 || i == 40 || i == 48 || i == 56)
            {
                if (!(i - 8 < 0))
                {
                    tiles[i].adjacency.Add(i - 8);
                }
                if (!(i + 1 > 63))
                {
                    tiles[i].adjacency.Add(i + 1);
                }
                if (!(i + 8 > 63))
                {
                    tiles[i].adjacency.Add(i + 8);
                }
            }
            else if (i == 7 || i == 15 || i == 23 || i == 31 || i == 39 || i == 47 || i == 55 || i == 63)
            {
                if (!(i - 1 < 0))
                {
                    tiles[i].adjacency.Add(i - 1);
                }
                if (!(i - 8 < 0))
                {
                    tiles[i].adjacency.Add(i - 8);
                }
                if (!(i + 8 > 63))
                {
                    tiles[i].adjacency.Add(i + 8);
                }
            }
            else
            {
                if (!(i - 1 < 0))
                {
                    tiles[i].adjacency.Add(i - 1);
                }
                if (!(i - 8 < 0))
                {
                    tiles[i].adjacency.Add(i - 8);
                }
                if (!(i + 1 > 63))
                {
                    tiles[i].adjacency.Add(i + 1);
                }
                if (!(i + 8 > 63))
                {
                    tiles[i].adjacency.Add(i + 8);
                }
            }

        }

    }

    //Reseteamos cada casilla: color, padre, distancia y visitada
    public void ResetTiles()
    {        
        foreach (Tile tile in tiles)
        {
            tile.Reset();
        }
    }

    public void ClickOnCop(int cop_id)
    {
        switch (state)
        {
            case Constants.Init:
            case Constants.CopSelected:                
                clickedCop = cop_id;
                clickedTile = cops[cop_id].GetComponent<CopMove>().currentTile;
                tiles[clickedTile].current = true;

                ResetTiles();
                FindSelectableTiles(true);

                state = Constants.CopSelected;                
                break;            
        }
    }

    public void ClickOnTile(int t)
    {                     
        clickedTile = t;

        switch (state)
        {            
            case Constants.CopSelected:
                //Si es una casilla roja, nos movemos
                if (tiles[clickedTile].selectable)
                {                  
                    cops[clickedCop].GetComponent<CopMove>().MoveToTile(tiles[clickedTile]);
                    cops[clickedCop].GetComponent<CopMove>().currentTile=tiles[clickedTile].numTile;
                    tiles[clickedTile].current = true;   
                    
                    state = Constants.TileSelected;
                }                
                break;
            case Constants.TileSelected:
                state = Constants.Init;
                break;
            case Constants.RobberTurn:
                state = Constants.Init;
                break;
        }
    }

    public void FinishTurn()
    {
        switch (state)
        {            
            case Constants.TileSelected:
                ResetTiles();

                state = Constants.RobberTurn;
                RobberTurn();
                break;
            case Constants.RobberTurn:                
                ResetTiles();
                IncreaseRoundCount();
                if (roundCount <= Constants.MaxRounds)
                    state = Constants.Init;
                else
                    EndGame(false);
                break;
        }

    }

    public void RobberTurn()
    {
        clickedTile = robber.GetComponent<RobberMove>().currentTile;
        tiles[clickedTile].current = true;
        FindSelectableTiles(false);
        /*TODO: Cambia el código de abajo para hacer lo siguiente   
        - Elegimos una casilla aleatoria entre las seleccionables que puede ir el caco
        - Movemos al caco a esa casilla
        - Actualizamos la variable currentTile del caco a la nueva casilla
        */
    }

    public void EndGame(bool end)
    {
        if(end)
            finalMessage.text = "You Win!";
        else
            finalMessage.text = "You Lose!";
        playAgainButton.interactable = true;
        state = Constants.End;
    }

    public void PlayAgain()
    {
        cops[0].GetComponent<CopMove>().Restart(tiles[Constants.InitialCop0]);
        cops[1].GetComponent<CopMove>().Restart(tiles[Constants.InitialCop1]);
        robber.GetComponent<RobberMove>().Restart(tiles[Constants.InitialRobber]);
                
        ResetTiles();

        playAgainButton.interactable = false;
        finalMessage.text = "";
        roundCount = 0;
        rounds.text = "Rounds: ";

        state = Constants.Restarting;
    }

    public void InitGame()
    {
        state = Constants.Init;
    }

    public void IncreaseRoundCount()
    {
        roundCount++;
        rounds.text = "Rounds: " + roundCount;
    }

    public void FindSelectableTiles(bool cop)
    {
                 
        int indexcurrentTile;
        int elOtroCop = 2;
        List<int> adyacentes = new List<int>();
        List<int> casillasRobber = new List<int>();

        // Turno del jugador
        if (cop == true) {

            // Recogemos la casilla del jugador
            indexcurrentTile = cops[clickedCop].GetComponent<CopMove>().currentTile;
            // Buscamos el otro policia
            if (clickedCop == 0)
            {
                elOtroCop = 1;
            }
            else
            {
                elOtroCop = 0;
            }

            // Añadimos las adyacencias
            for (int j = 0; j < tiles[indexcurrentTile].adjacency.Count; j++)
            {
                adyacentes.Add(tiles[indexcurrentTile].adjacency[j]);
            }

            adyacentes.Add(indexcurrentTile);

            for (int j = 0; j < adyacentes.Count; j++)
            {
                for (int i = 0; i < tiles[adyacentes[j]].adjacency.Count; i++)
                {
                    if (!(cops[elOtroCop].GetComponent<CopMove>().currentTile == adyacentes[j]))
                    {
                        // Mientras no sea tu propia casilla puedes selecionarla
                        if (!(tiles[tiles[adyacentes[j]].adjacency[i]].numTile == indexcurrentTile))
                        {
                            tiles[tiles[adyacentes[j]].adjacency[i]].selectable = true;
                            tiles[cops[elOtroCop].GetComponent<CopMove>().currentTile].selectable = false;
                        }

                    }
                }
            }
            // Acabamos de añadir las adyacencias
        }

        // Turno del rober
        else
        {
            // Variables de distancias
            List<int> distancias = new List<int>();
            distancias.Add(0);
            distancias.Add(0);

            int ejeYTotal = 0;
            int ejeXTotal = 0;
            List<int> distanciaTotal = new List<int>();

            int mayorDistancia = 0;
            int casillaAMayorDistancia = 0;

            // Variables del caco
            int posicionCaco = robber.GetComponent<RobberMove>().currentTile;
            List<int> ejeYCaco = new List<int>();
            List<int> ejeXCaco = new List<int>();

            // Variables del los policias
            List<int> copPosition = new List<int>();

            copPosition.Add(cops[0].GetComponent<CopMove>().currentTile);
            copPosition.Add(cops[1].GetComponent<CopMove>().currentTile);

            List<int> ejeY = new List<int>();
            List<int> ejeX = new List<int>();

            // Fin de las variables

            // Empieza la recogida de casillas adyacentes
            indexcurrentTile = robber.GetComponent<RobberMove>().currentTile;

            for (int j = 0; j < tiles[indexcurrentTile].adjacency.Count; j++)
            {
                if (!(tiles[indexcurrentTile].adjacency[j] == copPosition[0] || tiles[indexcurrentTile].adjacency[j] == copPosition[1]))
                {
                    adyacentes.Add(tiles[indexcurrentTile].adjacency[j]);
                }
            }

            adyacentes.Add(indexcurrentTile);

            for (int j = 0; j < adyacentes.Count; j++)
            {
                for (int i = 0; i < tiles[adyacentes[j]].adjacency.Count; i++)
                {
                    if (!(tiles[adyacentes[j]].adjacency[i] == copPosition[0] || tiles[adyacentes[j]].adjacency[i] == copPosition[1]))
                    {
                        tiles[tiles[adyacentes[j]].adjacency[i]].selectable = true;
                        casillasRobber.Add(tiles[adyacentes[j]].adjacency[i]);
                    }
                }
            }

            casillasRobber.Remove(indexcurrentTile);

            // Acaba la recogida de casillas adyacentes     

            // Empieza el calculo de distancia
         
            for (int j = 0; j < casillasRobber.Count; j++)
            {
                ejeXCaco.Add(casillasRobber[j]);
                ejeYCaco.Add(0);

                while (ejeXCaco[j] >= 0)
                {
                    ejeXCaco[j] -= 8;
                    //Debug.Log(posicion);
                    ejeYCaco[j]++;
                }
                
                ejeYCaco[j] -= 1;
                ejeXCaco[j] += 8;

                // Para la casilla actual polis
                for (int i = 0; i < copPosition.Count; i++)
                {
                    ejeX.Add(copPosition[i]);
                    ejeY.Add(0);
                    while (ejeX[i] >= 0)
                    {
                        ejeX[i] -= 8;
                        //Debug.Log(posicion);
                        ejeY[i]++;
                    }
                    ejeY[i] -= 1;
                    ejeX[i] += 8;

                    ejeYTotal = ejeYCaco[j] - ejeY[i];
                    if (ejeYTotal < 0)
                    {
                        ejeYTotal = ejeYTotal * -1;
                    }

                    ejeXTotal = ejeXCaco[j] - ejeX[i];
                    if (ejeXTotal < 0)
                    {
                        ejeXTotal = ejeXTotal * -1;
                    }

                    distancias[i] = ejeYTotal + ejeXTotal;
                }

                distanciaTotal.Add(distancias[0] + distancias[1]);
            }

            for(int i = 0; i < distanciaTotal.Count; i++)
            {
                if (distanciaTotal[i] >= mayorDistancia)
                {
                    mayorDistancia = distanciaTotal[i];
                    casillaAMayorDistancia = i;
                }
            }

            robber.GetComponent<RobberMove>().MoveToTile(tiles[casillasRobber[casillaAMayorDistancia]]);
            robber.GetComponent<RobberMove>().currentTile = casillasRobber[casillaAMayorDistancia];
        }
        
        //La ponemos rosa porque acabamos de hacer un reset
        tiles[indexcurrentTile].current = true;
    }









}
