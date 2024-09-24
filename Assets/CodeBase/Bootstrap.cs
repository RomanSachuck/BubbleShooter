using System;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private TextAsset _gameFieldText;
    [SerializeField] private BubblesForShot _bubblesForShot;
    [SerializeField] private GameFIeld _gameFIeld;
    [SerializeField] private BubbleMove _bubbleMove;
    [SerializeField] private BubbleLauncher _bubbleLauncher;
    [SerializeField] private WinLooseUi _winLooseUi;

    private WinLooseActor _winLooseActor;

    private void Awake()
    {
        //игровое поле инициализируется матрицей [10,11]. С четырех краев должны быть столбцы/строки заполненные нулями.
        int[,] field = CreateGameField();

        int[] bubblesQueue = new[] {5, 5, 4, 6, 6, 5, 5, 4, 6, 6, 5, 5, 4, 6, 6};

        _gameFIeld.Build(field);
        _bubblesForShot.Initialize(bubblesQueue);

        _winLooseActor = new WinLooseActor(_gameFIeld);
        _winLooseActor.SetBubbleAmountForWin(field);
        _winLooseActor.OnWin += _gameFIeld.HandleWin;
        _winLooseActor.OnWin += _winLooseUi.OnWin;
        _winLooseActor.OnLoose += _winLooseUi.OnLoose;

        _bubbleLauncher.Initialize(_winLooseActor);

        _bubbleMove.AddObserver(_gameFIeld);
        _bubbleMove.AddObserver(_bubblesForShot);
    }
    public void Restart()
    {
        int[,] field = CreateGameField();

        int[] bubblesQueue = new[] {5, 5, 4, 6, 6, 5, 5, 4, 6, 6, 5, 5, 4, 6, 6};

        _gameFIeld.Restart(field);
        _bubblesForShot.Initialize(bubblesQueue);
        _winLooseActor.SetBubbleAmountForWin(field);
    }
    
    private int[,] CreateGameField()
    {
        char[] levelInfo = _gameFieldText.text.ToCharArray();
        int[,] result = new int[10, 11];
        int x = 0, y = 0;
        
        for (int i = 0; i < levelInfo.Length; i++)
        {
            switch (levelInfo[i])
            {
                case ' ':
                    x++;
                    break;
                case '\r':
                    break;
                case '\n':
                    x = 0;
                    y++;
                    break;
                default:
                    result[y, x] = (int)Char.GetNumericValue(levelInfo[i]);
                    break;
            }
        }

        return result;
    }
}