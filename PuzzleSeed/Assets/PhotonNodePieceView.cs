using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[AddComponentMenu("Photon Networking/Photon Transform View")]
public class PhotonNodePieceView : MonoBehaviourPun, IPunObservable
{
    private int value;
    private Point index;
    private Vector2 pos;
    private RectTransform rect;

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(this.value);
            stream.SendNext(this.index);
            stream.SendNext(this.pos);
            stream.SendNext(this.rect);
        }
        else
        {
            //Network player, receive data
            this.value = (int)stream.ReceiveNext();
            this.index = (Point)stream.ReceiveNext();
            this.pos = (Vector2)stream.ReceiveNext();
            this.rect = (RectTransform)stream.ReceiveNext();
        }
    }
}
