using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] private TMP_Text output;
    
    private TileGrid grid;

    private void Start()
    {
        grid = new TileGrid(15, 15);
        PrintGrid();
    }

    private void PrintGrid()
    {
        output.text = grid.DataAsString();
    }

    public void Place(Card card)
    {
        var p = card.transform.position;
        card.GetPoints().ToList().ForEach(pair =>
        {
            grid.AddNumber(1, ((int)p.x + 2) * 3 + pair.x, (-(int)p.y + 2) * 3 + pair.y); 
        });
        PrintGrid();
    }
}