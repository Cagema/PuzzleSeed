using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class NodePiece : MonoBehaviourPun, IPointerDownHandler, IPointerUpHandler, IPunObservable, IPunInstantiateMagicCallback
{
    public int value;
    public Point index;

    [HideInInspector]
    public Vector2 pos;
    [HideInInspector]
    public RectTransform rect;

    bool updating;
    Image img;

    public void Initialize(int v, Point p, Sprite piece)
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        value = v;
        SetIndex(p);
        img.sprite = piece;
    }

    public void SetImage(Sprite piece)
    {
        img.sprite = piece;
    }

    public void SetIndex(Point p)
    {
        index = p;
        ResetPosition();
        UpdateName();
    }

    public void ResetPosition()
    {
        pos = new Vector2(32 + (64 * index.x), -32 - (64 * index.y));
    }

    public void MovePosition(Vector2 move)
    {
        rect.anchoredPosition += move * Time.deltaTime * 16f;
    }

    public void MovePositionTo(Vector2 move)
    {
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, move, Time.deltaTime * 16f);
    }

    public bool UpdatePiece()
    {
        if (Vector3.Distance(rect.anchoredPosition, pos) > 1)
        {
            MovePositionTo(pos);
            updating = true;
            return true;
        }
        else
        {
            rect.anchoredPosition = pos;
            updating = false;
            return false;
        }
        
    }

    void UpdateName()
    {
        transform.name = "Node [" + index.x + ", " + index.y + "]";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (updating || GameManager.S.phase == eGamePhase.waiting) return;
        MovePieces.instance.MovePiece(this);
        //MovePieces.instance.photonView.RPC("MovePiece", RpcTarget.All, this.index.x, this.index.y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (updating || GameManager.S.phase == eGamePhase.waiting) return;

        //MovePieces.instance.DropPiece();
        MovePieces.instance.photonView.RPC("DropPiece", RpcTarget.All);
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)
        //{
        //    //We own this player: send the others our data
        //    stream.SendNext(this.value);
        //    stream.SendNext(this.index.x);
        //    stream.SendNext(this.index.y);
        //}
        //else
        //{
        //    //Network player, receive data
        //    this.value = (int)stream.ReceiveNext();
        //    this.index.x = (int)stream.ReceiveNext();
        //    this.index.y = (int)stream.ReceiveNext();
        //}
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        this.transform.SetParent(GameObject.FindGameObjectWithTag((string)instantiationData[0]).GetComponent<RectTransform>(), false);
        this.value = (int)instantiationData[1];
        this.index.x = (int)instantiationData[2];
        this.index.y = (int)instantiationData[3];
    }
}
