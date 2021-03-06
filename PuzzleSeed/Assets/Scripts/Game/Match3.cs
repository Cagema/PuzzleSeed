﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour
{
    public ArrayLayout BoardLayout;

    [Header("UI Elements")]
    public Sprite[] pieces;
    public RectTransform gameBoard;
    public RectTransform killedBoard;

    [Header("Prefabs")]
    public GameObject nodePiece;
    public GameObject killedPiece;

    int width = 8;
    int height = 8;
    int[] fills;
    Node[,] board;
    public int comboCount = 0;

    List<NodePiece> update;
    List<FlippedPieces> flipped;
    List<NodePiece> dead;
    List<KilledPiece> killed;

    System.Random random;

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        List<NodePiece> finishedUpdating = new List<NodePiece>();
        for (int i = 0; i < update.Count; i++)
        {
            NodePiece piece = update[i];
            if (!piece.UpdatePiece())
                finishedUpdating.Add(piece);
        }

        for (int i = 0; i < finishedUpdating.Count; i++)
        {
            NodePiece piece = finishedUpdating[i];
            FlippedPieces flip = GetFlipped(piece);
            NodePiece flippedPiece = null;

            int x = (int)piece.index.x;
            fills[x] = Mathf.Clamp(fills[x] - 1, 0, width);

            List<Point> connected = IsConnected(piece.index, true);
            bool wasFlipped = (flip != null);

            if (wasFlipped)
            {

                flippedPiece = flip.GetOtherPiece(piece);
                AddPoints(ref connected, IsConnected(flippedPiece.index, true));
            }
                
            if (connected.Count == 0) // If we didn't make a match
            {
                if (wasFlipped)
                {
                    FlipPieces(piece.index, flippedPiece.index, false); // Flip back
                    GameManager.S.FinishCombo();
                }
            }
            else // If we made a match
            {
                foreach(Point pnt in connected) // Remove the node pieces connected
                {
                    //KillPiece(pnt);
                    Node node = GetNodeAtPoint(pnt);
                    NodePiece nodePiece = node.getPiece();
                    if (nodePiece != null)
                    {
                        nodePiece.gameObject.SetActive(false);
                        dead.Add(nodePiece);
                    }
                    node.SetPiece(null);
                }

                Invoke("ApplyGravityToBoard", 0.3f);
                //ApplyGravityToBoard();
            }
            flipped.Remove(flip); // Remove the flip after update
            update.Remove(piece);   
        }
        //if (comboCount > 0 && GameManager.S.phase == eGamePhase.waiting && comboCount == GameManager.S.comboCount)
        //{
        //    comboCount = 0;
        //    GameManager.S.FinishCombo();
        //}
    }

   

    void ApplyGravityToBoard()
    {
        bool wasHole = false;
        for (int x = 0; x < width; x++)
        {
            for (int y = (height-1); y >= 0; y--)
            {
                Point p = new Point(x, y);
                Node node = GetNodeAtPoint(p);
                int val = GetValueAtPoint(p);
                if (val != 0) continue; // If it is not a hole, do nothing
                wasHole = true;
                for (int ny = (y-1); ny >= -1; ny--)
                {
                    Point next = new Point(x, ny);
                    int nextVal = GetValueAtPoint(next);
                    if (nextVal == 0)
                        continue;
                    if (nextVal != -1) // If we did not hit an end, but its not 0 then use this to fill the current hole
                    {
                        Node got = GetNodeAtPoint(next);
                        NodePiece piece = got.getPiece();

                        // Set the hole
                        node.SetPiece(piece);
                        update.Add(piece);

                        // Replace the hole
                        got.SetPiece(null);
                    }
                    else // Use dead ones or create new pieces to fill holes (hit a -1) only if we choose to
                    {
                        int newVal = FillPiece();
                        NodePiece piece;
                        Point fallPnt = new Point(x, (-1 - fills[x]));
                        if (dead.Count > 0)
                        {
                            NodePiece revived = dead[0];
                            revived.gameObject.SetActive(true);
                            piece = revived;
                            
                            dead.RemoveAt(0);
                        }
                        else
                        {
                            GameObject obj = Instantiate(nodePiece, gameBoard);
                            NodePiece n = obj.GetComponent<NodePiece>();
                            piece = n;
                        }

                        piece.Initialize(newVal, p, pieces[newVal - 1]);
                        piece.rect.anchoredPosition = GetPositionFromPoint(fallPnt);

                        Node hole = GetNodeAtPoint(p);
                        hole.SetPiece(piece);
                        ResetPiece(piece);
                        fills[x]++;
                    }
                    break;
                }
            }
        }
        if (wasHole)
        {
            comboCount++;
            Invoke("ComboCheck", 1f);
        }
    }

    void ComboCheck()
    {
        GameManager.S.FinishMatch();
        if (GameManager.S.comboCount == comboCount)
        {
            GameManager.S.FinishCombo();
            comboCount = 0;
        }
    }

    FlippedPieces GetFlipped(NodePiece p)
    {
        FlippedPieces flip = null;
        for (int i = 0; i < flipped.Count; i++)
        {
            if (flipped[i].GetOtherPiece(p) != null)
            {
                flip = flipped[i];
                break;
            }
        }
        return flip;
    }

    void StartGame()
    {
        fills = new int[width];
        string seed = GetRandomSeed();
        random = new System.Random(seed.GetHashCode());
        update = new List<NodePiece>();
        flipped = new List<FlippedPieces>();
        dead = new List<NodePiece>();
        killed = new List<KilledPiece>();

        InitializeBoard();
        VerifyBoard();
        InstatiateBoard();
    }

    void InitializeBoard()
    {
        board = new Node[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                board[x, y] = new Node((BoardLayout.rows[y].row[x]) ? -1 : FillPiece(), new Point(x, y));
            }
        }
    }

    void VerifyBoard()
    {
        List<int> remove;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Point p = new Point(x, y);
                int val = GetValueAtPoint(p);
                if (val <= 0) continue;

                remove = new List<int>();
                while (IsConnected(p, true).Count > 0)
                {
                    if (!remove.Contains(val))
                        remove.Add(val);
                    SetValueAtPoint(p, NewValue(ref remove));
                }
            }
        }
    }

    void InstatiateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = GetNodeAtPoint(new Point(x, y));

                int val = node.value;
                if (val <= 0) continue;
                GameObject p = Instantiate(nodePiece, gameBoard);
                NodePiece piece = p.GetComponent<NodePiece>();
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                piece.Initialize(val, new Point(x, y), pieces[val - 1]);
                node.SetPiece(piece);
            }
        }
    }

    public void ResetPiece(NodePiece piece)
    {
        piece.ResetPosition();
        update.Add(piece);
    }

    public void FlipPieces(Point one, Point two, bool main)
    {
        if (GetValueAtPoint(one) < 0) return;

        Node nodeOne = GetNodeAtPoint(one);
        NodePiece pieceOne = nodeOne.getPiece();

        if (GetValueAtPoint(two) > 0)
        {
            Node nodeTwo = GetNodeAtPoint(two);
            NodePiece pieceTwo = nodeTwo.getPiece();
            nodeOne.SetPiece(pieceTwo);
            nodeTwo.SetPiece(pieceOne);

            if (main)
                flipped.Add(new FlippedPieces(pieceOne, pieceTwo));

            update.Add(pieceOne);
            update.Add(pieceTwo);
        }
        else
        {
            ResetPiece(pieceOne);
        }
    }

    void KillPiece(Point p)
    {
        List<KilledPiece> available = new List<KilledPiece>();
        for (int i = 0; i < killed.Count; i++)
        {
            if (!killed[i].falling) available.Add(killed[i]);
        }

        KilledPiece set = null;
        if (available.Count > 0)
        {
            set = available[0];
        }
        else
        {
            GameObject kill = Instantiate(killedPiece, killedBoard);
            KilledPiece kPiece = kill.GetComponent<KilledPiece>();
            set = kPiece;
            killed.Add(kPiece);
        }

        int val = GetValueAtPoint(p) - 1;
        if (set != null && val >= 0 && val < pieces.Length)
        {
            set.Initialize(pieces[val], GetPositionFromPoint(p));
        }
    }

    List<Point> IsConnected(Point p, bool main)
    {
        List<Point> connected = new List<Point>();
        int val = GetValueAtPoint(p);
        Point[] directions =
        {
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };

        foreach (Point dir in directions) // Checking if there is 2 or more same shapes in the directions
        {
            List<Point> line = new List<Point>();

            int same = 0;
            for (int i = 1; i < 3; i++)
            {
                Point check = Point.add(p, Point.mult(dir, i));
                if (GetValueAtPoint(check) == val)
                {
                    line.Add(check);
                    same++;
                }
            }

            if (same > 1) // If there are 2 or more of the same shape in the direction the we know it is a match
            {
                AddPoints(ref connected, line); // Add these points to the overarching connected list
            }
        }

        for (int i = 0; i < 2; i++) // Checking if we are in the middle of two of the same shapes
        {
            List<Point> line = new List<Point>();

            int same = 0;
            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[i + 2]) };
            foreach(var next in check) // Check both sides of the pieces, if they are the same value, add them to the list
            {
                if (GetValueAtPoint(next) == val)
                {
                    line.Add(next);
                    same++;
                }
            }

            if (same > 1)
            {
                AddPoints(ref connected, line);
            }
        }

        if (main) // Checks for other matches along the current match
        {
            for (int i = 0; i < connected.Count; i++)
            {
                AddPoints(ref connected, IsConnected(connected[i], false));
            }
        }

        return connected;
    }

    void AddPoints(ref List<Point> points, List<Point> add)
    {
        foreach (Point p in add)
        {
            bool doAdd = true;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Equals(p))
                {
                    doAdd = false;
                    break;
                }
            }

            if (doAdd) points.Add(p);
        }
    }

    private int FillPiece()
    {
        int val = 1;
        val = (random.Next(0, 100) / (100 / pieces.Length)) + 1;
        return val;
    }

    int GetValueAtPoint(Point p)
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return -1;
        return board[p.x, p.y].value;
    }

    void SetValueAtPoint(Point p, int v)
    {
        board[p.x, p.y].value = v;
    }

    Node GetNodeAtPoint(Point p)
    {
        return board[p.x, p.y];
    }

    int NewValue(ref List<int> remove)
    {
        List<int> available = new List<int>();
        for (int i = 0; i < pieces.Length; i++)
        {
            available.Add(i + 1);
        }
        foreach (var i in remove)
        {
            available.Remove(i);
        }

        if (available.Count <= 0) return 0;
        return available[random.Next(0, available.Count)];
    }

    string GetRandomSeed()
    {
        string seed = "";
        string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdifghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
        for(int i = 0; i < 20; i++)
        {
            seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
        }
        return seed;
    }

    public Vector2 GetPositionFromPoint(Point p)
    {
        return new Vector2(32 + (64 * p.x), -32 - (62 * p.y));
    }

    [System.Serializable]
    public class Node
    {
        public int value; // 0 = blank, 1 = cube, 2 = sphere, 3 = cyllinder, 4 = pyramid, 5 = octahedron, -1 = hole
        public Point index;
        NodePiece piece;

        public Node(int v, Point i)
        {
            value = v;
            index = i;
        }

        public void SetPiece(NodePiece p)
        {
            piece = p;
            value = (piece == null) ? 0 : piece.value;
            if (piece == null) return;
            piece.SetIndex(index);
        }

        public NodePiece getPiece()
        {
            return piece;
        }
    }
}

[System.Serializable]
public class FlippedPieces
{
    public NodePiece one;
    public NodePiece two;

    public FlippedPieces(NodePiece o, NodePiece t)
    {
        one = o; two = t;
    }

    public NodePiece GetOtherPiece(NodePiece p)
    {
        if (p == one)
            return two;
        else if (p == two)
            return one;
        else
            return null;
    }
}
