﻿using System;
using System.Linq;
using System.Net.Mime;
using Elements;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public enum GridElementType
{
    None,
    Ship,
    DestroyedShip,
    Miss
}

public class GameController : MonoBehaviour
{
    [SerializeField] private Text _winnerText;
    [SerializeField] private Text _userScoreText;
    [SerializeField] private Text _computerScoreText;
    [SerializeField] private ElementItem _gridCell;
    [SerializeField] private RectTransform _userGridContainer;
    [SerializeField] private RectTransform _computerGridContainer;
    private static int _gameGridSize = 10;
    private ElementItem[,] _userGridsCells = new ElementItem[_gameGridSize, _gameGridSize];
    private ElementItem[,] _computerGridsCells = new ElementItem[_gameGridSize, _gameGridSize];
    private int _userScore = 0;
    private int _computerScore = 0;
    
    

    private void Start()
    {
        GridCreate(_userGridsCells, null, _userGridContainer, OwnerType.User);
        GridCreate(_computerGridsCells, OnElementPressedForAttack, _computerGridContainer, OwnerType.Computer);
    }


    private static void SetRandomShips(ElementItem[,] grid)
    {
        while (grid.Cast<ElementItem>().Where(data => data.GridElementType == GridElementType.Ship).ToList().Count
               < _gameGridSize * _gameGridSize * 0.2f)
        {
            var randRow = Random.Range(0, _gameGridSize);
            var randColumn = Random.Range(0, _gameGridSize);
            if (grid[randRow, randColumn].GridElementType == GridElementType.None)
            {
                grid[randRow, randColumn].GridElementType = GridElementType.Ship;
            }
        }
    }


    private void GridCreate(ElementItem[,] elementItems,
        Action<ElementItem> onElementPressed,
        RectTransform container,
        OwnerType ownerType)
    {
        for (int i = 0; i < _gameGridSize; i++)
        {
            for (int j = 0; j < _gameGridSize; j++)
            {
                var elementItem = Instantiate(_gridCell, container);
                elementItem.Init(new Coordinates(i, j), onElementPressed,
                    GridElementType.None, ownerType);
                elementItems[i, j] = elementItem;
            }
        }

        SetRandomShips(elementItems);
    }

    private void ComputerAttack()
    {
        while (_computerGridsCells.Cast<ElementItem>().Any(data => data.GridElementType == GridElementType.Ship))
        {
            var randRow = Random.Range(0, _gameGridSize);
            var randColumn = Random.Range(0, _gameGridSize);
            var currentElementItem = _userGridsCells[randRow, randColumn];
            switch (currentElementItem.GridElementType)
            {
                case GridElementType.None:
                    currentElementItem.GridElementType = GridElementType.Miss;
                    return;
                    break;
                case GridElementType.Ship:
                    currentElementItem.GridElementType = GridElementType.DestroyedShip;
                    _computerScore++;
                    break;
                case GridElementType.DestroyedShip:
                    break;
                case GridElementType.Miss:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void OnElementPressedForAttack(ElementItem elementItem)
    {
        _userScoreText.text = "Hit: " + _userScore;
        _computerScoreText.text = "Hit: " + _computerScore;
        while (_userGridsCells.Cast<ElementItem>().Any(data => data.GridElementType == GridElementType.Ship)
               && _computerGridsCells.Cast<ElementItem>().Any(data => data.GridElementType == GridElementType.Ship))
        {
            switch (elementItem.GridElementType)
            {
                case GridElementType.None:
                    elementItem.GridElementType = GridElementType.Miss;
                    ComputerAttack();
                    break;
                case GridElementType.Ship:
                    elementItem.GridElementType = GridElementType.DestroyedShip;
                    _userScore++;
                    break;
                case GridElementType.DestroyedShip:
                    return;
                    break;
                case GridElementType.Miss:
                    return;
                    break;
            }
        }

        if (_userGridsCells.Cast<ElementItem>().Any(element => element.GridElementType == GridElementType.Ship))
        {
            _winnerText.text = "You lose";
        }
        else
        {
            _winnerText.text = "You win";
        }

        _winnerText.IsActive();
    }
}