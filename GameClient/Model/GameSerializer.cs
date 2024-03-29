using GameClient.model;

namespace GameClient.Model;

public class GameSerializer
{
    public string Id { get; set; }
    public Player[] Players { get; set; }
    public List<Cell> GameField { get; set; } = new();
    public int[] GamePoints { get; set; }
    public bool Side { get; set; }

    public GameSerializer(Player[] players)
    {
        Players = players;
        
        GameField.Add(new Cell() { Position = 0 });
        GameField.Add(new Cell() { Position = 1 });
        GameField.Add(new Cell() { Position = 2 });
        GameField.Add(new Cell() { Position = 3 });
        GameField.Add(new Cell() { Position = 4 });
        GameField.Add(new Cell() { Position = 5 });
        GameField.Add(new Cell() { Position = 6 });
        GameField.Add(new Cell() { Position = 7 });
        GameField.Add(new Cell() { Position = 8 });
    }
}