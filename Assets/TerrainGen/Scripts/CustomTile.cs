using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;
using System.Collections.Generic;

    [CreateAssetMenu(fileName = "CustomTile", menuName = "2D/Tiles/CustomTerraria")]

    public class CustomTile : RuleTile<CustomTile.Neighbor>
    {
    //Ruletile script
    public Item item;
    public TileClass tileClass;
    //   public bool alwaysConnect;
    public TileBase[] tilesToConnect;
        public TileBase[] alwaysConnect;
        public TileBase[] cantConnectStates;
        public class Neighbor : RuleTile.TilingRule.Neighbor
        {
            public const int Any = 3;
            public const int Specified = 4;
            public const int Nothing = 5;
            public const int NotSpecified = 6;
            public const int NotAlwaysConnected = 7;
        }

        public override bool RuleMatch(int neighbor, TileBase tile)
        {
            switch (neighbor)
            {
                case Neighbor.This: return CheckThis(tile);
                case Neighbor.NotThis: return CheckNotThis(tile);
                case Neighbor.Any: return CheckAny(tile);
                case Neighbor.Specified: return CheckSpecified(tile);
                case Neighbor.Nothing: return CheckNothing(tile);
                case Neighbor.NotSpecified: return CheckNotSpecified(tile);
                case Neighbor.NotAlwaysConnected: return CheckNotAlwaysConnect(tile);
            }
            return base.RuleMatch(neighbor, tile);
        }

        private bool CheckNothing(TileBase tile)
        {
            return tile is null;
        }

        private bool CheckSpecified(TileBase tile)
        {
            return tilesToConnect.Contains(tile);
        }

        private bool CheckNotAlwaysConnect(TileBase tile)
        {
            return !alwaysConnect.Contains(tile) && tile != this;
        }

        private bool CheckAny(TileBase tile)
        {
            return CheckSpecified(tile) || tile == this;
        }

        private bool CheckNotSpecified(TileBase tile)
        {
            return (!tilesToConnect.Contains(tile) && tile != this);
        }

        private bool CheckNotThis(TileBase tile)
        {
            return tile != this;
        }

        private bool CheckThis(TileBase tile)
        {
            return tile == this;
        }
}