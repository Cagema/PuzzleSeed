using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MovePieces : MonoBehaviourPun
{
    public static MovePieces instance;
    Match3 game;

    NodePiece moving;
    Point newIndex;
    Vector2 mouseStart;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        game = GetComponent<Match3>();
    }

    private void Update()
    {
        if (moving != null)
        {
            if (mouseStart == Vector2.zero)
                return;

            Vector2 dir = ((Vector2)Input.mousePosition - mouseStart);
            Vector2 nDir = dir.normalized;
            Vector2 aDir = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

            newIndex = Point.clone(moving.index);
            Point add = Point.zero;
            if (dir.magnitude > 32) // If our mouse is 32 pixels away from the starting point of the mouse
            {
                // make add either (1, 0) | (-1, 0) | (0, 1) | (0, -1) depending of the mouse point
                if (aDir.x > aDir.y)
                {
                    add = new Point((nDir.x > 0) ? 1 : -1, 0);
                }
                else if (aDir.y > aDir.x)
                {
                    add = new Point(0, (nDir.y > 0) ? -1 : 1);
                }
            }
            newIndex.add(add);

            Vector2 pos = game.GetPositionFromPoint(moving.index);
            if (!newIndex.Equals(moving.index))
            {
                pos += Point.mult(new Point(add.x, -add.y), 16).ToVector();
            }
            moving.MovePositionTo(pos);
            this.photonView.RPC("SetNewIndex", RpcTarget.Others, newIndex.x, newIndex.y);
        }
    }

    public void MovePiece(NodePiece piece)
    {
        if (moving != null) return;
        moving = piece;
        mouseStart = Input.mousePosition;
        this.photonView.RPC("SetMovingPiece", RpcTarget.Others, piece.index.x, piece.index.y);
    }

    [PunRPC]
    void SetMovingPiece(int x, int y)
    {
        NodePiece piece = game.GetNodeAtPoint(new Point(x, y)).getPiece();
        moving = piece;
        newIndex = moving.index;
        mouseStart = Vector2.zero;
    }

    [PunRPC]
    void SetNewIndex(int x, int y)
    {
        newIndex = new Point(x, y);
    }

    [PunRPC]
    public void DropPiece()
    {
        if (moving == null) return;

        if (!newIndex.Equals(moving.index))
        {
            GameManager.S.FinishTurn();
            game.FlipPieces(moving.index, newIndex, true);
            //game.photonView.RPC("FlipPieces", Photon.Pun.RpcTarget.All, moving.index.x, moving.index.y, newIndex.x, newIndex.y, true);
        }
        else
        {
            GameManager.S.ResetTurn();
            game.ResetPiece(moving);
        }
            
        moving = null;
    }
}
