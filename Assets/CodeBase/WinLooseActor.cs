using System;

public class WinLooseActor
{
    public bool GameOver { get; private set; }
    
    public event Action OnWin;
    public event Action OnLoose;
    
    private int _bubbleAmountForWin;
    
    public WinLooseActor(GameFIeld gameField)
    {
        gameField.GameStateHasBeenUpdated += CheckWin;
    }

    public void SetBubbleAmountForWin(int[,] field)
    {
        GameOver = false;
        int bubbleCountInTheTopRow = CountTheBubbleInTheTopRow(field);
        _bubbleAmountForWin = bubbleCountInTheTopRow / 3;
    }

    private void CheckWin(int[,] field, int numberRemainingBubbles)
    {
        int bubbleCountInTheTopRow= CountTheBubbleInTheTopRow(field);
        
        if(bubbleCountInTheTopRow < _bubbleAmountForWin)
            ProcessTheVictory();
        else if(numberRemainingBubbles < 0)
            ProcessTheLoose();
    }

   
    private void ProcessTheVictory()
    {
        GameOver = true;
        OnWin?.Invoke();
    }
    
    private void ProcessTheLoose()
    {
        GameOver = true;
        OnLoose?.Invoke();
    }

    private int CountTheBubbleInTheTopRow(int[,] field)
    {
        int bubbleCount = 0;
        
        for (int i = 0; i < field.GetLength(1); i++)
        {
            if (field[1, i] != 0)
                bubbleCount++;
        }

        return bubbleCount;
    }
}